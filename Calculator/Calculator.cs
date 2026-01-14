namespace Calculator;

public class Calculator
{
    private static readonly char[] Delimiters = [',', '\n'];
    const int MaxPossibleValueToAdd = 1000;

    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var parts = SplitInput(input);
        var (sum, negativeNumbers) = AggregateValues(parts);
        EnsureNoNegatives(negativeNumbers);

        return sum.ToString();
    }

    private static IEnumerable<string> SplitInput(string input)
    {
        return input.Split(Delimiters);
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
