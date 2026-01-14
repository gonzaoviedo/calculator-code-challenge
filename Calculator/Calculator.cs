namespace Calculator;

public class Calculator
{
    public string Add(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var parts = input.Split(',');

        var sum = 0;

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue; // missing number treated as 0

            if (int.TryParse(trimmed, out var value))
                sum += value; // valid number
            // invalid numbers are treated as 0 (ignored)
        }

        return sum.ToString();
    }
}
