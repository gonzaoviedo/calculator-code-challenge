using System.Text.RegularExpressions;

namespace Calculator;

public class Calculator
{
    private static readonly string[] BaseDelimiters = [",", "\n"];
    private const int MaxPossibleValueToAdd = 1000;

    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var delimiters = new List<string>(BaseDelimiters);
        var numbersPortion = input;

        if (TryParseCustomDelimiters(input, out var customDelimiters, out var withoutHeader))
        {
            delimiters.AddRange(customDelimiters);
            numbersPortion = withoutHeader;
        }

        var parts = SplitInput(numbersPortion, delimiters);
        var (sum, negativeNumbers) = AggregateValues(parts);
        EnsureNoNegatives(negativeNumbers);

        return sum.ToString();
    }

    private static bool TryParseCustomDelimiters(string input, out List<string> delimiters, out string numbersPortion)
    {
        delimiters = new List<string>();
        numbersPortion = input;

        if (!input.StartsWith("//"))
            return false;

        var newlineIndex = input.IndexOf('\n');
        if (newlineIndex < 0)
            return false;

        var header = input.Substring(2, newlineIndex - 2);
        numbersPortion = input.Substring(newlineIndex + 1);

        if (string.IsNullOrEmpty(header))
            return false;

        // Format 1 & 3: //[{delimiter}]...\n{numbers}  (one or many delimiters of any length)
        if (header.StartsWith("["))
        {
            var i = 0;
            while (i < header.Length)
            {
                if (header[i] != '[')
                    return false;

                var closingBracketIndex = header.IndexOf(']', i + 1);
                if (closingBracketIndex < 0)
                    return false;

                var delimiter = header.Substring(i + 1, closingBracketIndex - i - 1);
                if (delimiter.Length == 0)
                    return false;

                delimiters.Add(delimiter);
                i = closingBracketIndex + 1;
            }

            return delimiters.Count > 0;
        }

        // Format 2: //{singleChar}\n{numbers} (or any non-bracket header treated as a single delimiter)
        delimiters.Add(header);
        return true;
    }

    private static IEnumerable<string> SplitInput(string input, IEnumerable<string> delimiters)
    {
        var pattern = string.Join("|", delimiters.Select(Regex.Escape));
        return Regex.Split(input, pattern);
    }

    private static (int sum, List<int> negatives) AggregateValues(IEnumerable<string> parts)
    {
        var sum = 0;
        var negatives = new List<int>();

        foreach (var part in parts)
        {
            if (TryGetValidNumber(part, negatives, out var value))
            {
                sum += value;
            }
        }

        return (sum, negatives);
    }

    private static bool TryGetValidNumber(string part, List<int> negatives, out int value)
    {
        value = 0;
        var trimmed = part.Trim();

        if (string.IsNullOrEmpty(trimmed))
            return false; // missing number treated as 0

        if (!int.TryParse(trimmed, out var parsed))
            return false; // invalid numbers are ignored (treated as 0)

        if (parsed < 0)
        {
            negatives.Add(parsed);
            return false;
        }

        if (parsed > MaxPossibleValueToAdd)
            return false; // values larger than max threshold are ignored

        value = parsed;
        return true;
    }

    private static void EnsureNoNegatives(List<int> negativeNumbers)
    {
        if (negativeNumbers.Count == 0)
            return;

        var negatives = string.Join(",", negativeNumbers);
        throw new ArgumentException($"Negative numbers are not allowed: {negatives}");
    }
}
