namespace Calculator;

public class Calculator
{
    private static readonly char[] BaseDelimiters = [',', '\n'];
    const int MaxPossibleValueToAdd = 1000;

    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var delimiters = new List<char>(BaseDelimiters);
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

    private static bool TryParseCustomDelimiter(string input, out char delimiter, out string numbersPortion)
    {
        delimiter = default;
        numbersPortion = input;

        if (!input.StartsWith("//"))
            return false;

        // Expected format: //{delimiter}\n{numbers}
        if (input.Length < 4 || input[3] != '\n')
            return false;

        delimiter = input[2];
        numbersPortion = input.Substring(4);
        return true;
    }

    private static IEnumerable<string> SplitInput(string input, IEnumerable<char> delimiters)
    {
        return input.Split(delimiters.ToArray());
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
