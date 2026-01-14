var calculator = new Calculator.Calculator();

while (true)
{
    Console.WriteLine("Please enter the numbers you want to Add");
    string? input = Console.ReadLine();

    if (input == null)
        break;

    string result = calculator.Add(input);
    Console.WriteLine($"The result is: {result}");
}
