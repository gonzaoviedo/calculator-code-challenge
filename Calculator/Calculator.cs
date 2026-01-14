namespace Calculator;

public class Calculator
{
    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        // Supporting both comma and newline as delimiters
        var parts = input.Split(',', '\n');

        var sum = 0;
        var negativeNumbers = new List<int>();

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue; // missing number treated as 0

            if (int.TryParse(trimmed, out var value))
            {
                if (value < 0)
                {
                    negativeNumbers.Add(value);
                    continue;
                }

                sum += value; // valid non-negative number
            }
            // invalid numbers are ignored (treated as 0)
        }

        if (negativeNumbers.Count > 0)
        {
            var negatives = string.Join(",", negativeNumbers);
            throw new ArgumentException($"Negative numbers are not allowed: {negatives}");
        }

        return sum.ToString();
    }
}
