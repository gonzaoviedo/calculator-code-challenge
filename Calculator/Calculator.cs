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

        if (TryParseCustomDelimiter(input, out var customDelimiter, out var withoutHeader))
        {
            delimiters.Add(customDelimiter);
            numbersPortion = withoutHeader;
        }

        var parts = SplitInput(numbersPortion, delimiters);
        var (sum, negativeNumbers) = AggregateValues(parts);
        EnsureNoNegatives(negativeNumbers);

        return sum.ToString();
    }

    private static bool TryParseCustomDelimiter(string input, out string delimiter, out string numbersPortion)
    {
        delimiter = string.Empty;
        numbersPortion = input;

        if (!input.StartsWith("//"))
            return false;

        // Format 1: //[{delimiter}]\n{numbers}  (delimiter of any length)
        if (input.StartsWith("//["))
        {
            var closingBracketIndex = input.IndexOf(']', 3);
            if (closingBracketIndex < 0 || closingBracketIndex + 1 >= input.Length || input[closingBracketIndex + 1] != '\n')
                return false;

            delimiter = input.Substring(3, closingBracketIndex - 3);
            numbersPortion = input.Substring(closingBracketIndex + 2);
            return true;
        }

        // Format 2: //{singleChar}\n{numbers}
        if (input.Length < 4 || input[3] != '\n')
            return false;

        delimiter = input[2].ToString();
        numbersPortion = input.Substring(4);
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
