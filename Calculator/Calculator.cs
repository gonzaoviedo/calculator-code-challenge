using System.Text.RegularExpressions;

namespace Calculator;

public class Calculator
{
    private static readonly string[] BaseDelimiters = [",", "\n"];
    private const int MaxPossibleValueToAdd = 1000;

    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0 = 0";

        var delimiters = new List<string>(BaseDelimiters);
        var numbersPortion = input;

        if (TryParseCustomDelimiters(input, out var customDelimiters, out var withoutHeader))
        {
            delimiters.AddRange(customDelimiters);
            numbersPortion = withoutHeader;
        }

        var parts = SplitInput(numbersPortion, delimiters);
        var calculation = AggregateValues(parts);
        EnsureNoNegatives(calculation.Negatives);

        if (calculation.Terms.Count == 0)
            return "0 = 0";

        var formula = string.Join("+", calculation.Terms);
        return $"{formula} = {calculation.Sum}";
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

    private sealed class CalculationResult
    {
        public int Sum { get; private set; }
        public List<int> Terms { get; } = new();
        public List<int> Negatives { get; } = new();

        public void AddIgnoredTerm()
        {
            Terms.Add(0);
        }

        public void AddNegative(int value)
        {
            Negatives.Add(value);
            Terms.Add(0);
        }

        public void AddTerm(int value)
        {
            Terms.Add(value);
            Sum += value;
        }
    }

    private static CalculationResult AggregateValues(IEnumerable<string> parts)
    {
        var result = new CalculationResult();

        foreach (var part in parts)
        {
            ProcessToken(part, result);
        }

        return result;
    }

    private enum TokenClassification
    {
        Ignored,
        Negative,
        Valid
    }

    private static void ProcessToken(string part, CalculationResult result)
    {
        var classification = ClassifyToken(part, out var value);

        switch (classification)
        {
            case TokenClassification.Ignored:
                result.AddIgnoredTerm();
                break;

            case TokenClassification.Negative:
                result.AddNegative(value);
                break;

            case TokenClassification.Valid:
                result.AddTerm(value);
                break;
        }
    }

    private static TokenClassification ClassifyToken(string part, out int value)
    {
        value = 0;
        var trimmed = part.Trim();

        if (string.IsNullOrEmpty(trimmed))
            return TokenClassification.Ignored; // missing number treated as 0

        if (!int.TryParse(trimmed, out var parsed))
            return TokenClassification.Ignored; // invalid numbers are treated as 0

        if (parsed < 0)
        {
            value = parsed;
            return TokenClassification.Negative; // negative numbers contribute 0 (but will cause exception)
        }

        if (parsed > MaxPossibleValueToAdd)
            return TokenClassification.Ignored; // numbers greater than max threshold are treated as 0

        value = parsed;
        return TokenClassification.Valid;
    }

    private static void EnsureNoNegatives(List<int> negativeNumbers)
    {
        if (negativeNumbers.Count == 0)
            return;

        var negatives = string.Join(",", negativeNumbers);
        throw new ArgumentException($"Negative numbers are not allowed: {negatives}");
    }
}
