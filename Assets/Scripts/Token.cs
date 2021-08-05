public class Token
{
    public enum Type
    {
        UnaryOperator,
        BinaryOperator,
        Operand,
        Parenthesis,
        Undefined
    }

    private static string[] basicOperators = {"+","-","*","/","^"};
    private static string[] multiCharacterOperators = {"sin", "cos", "tan", "abs"};

    public string TokenValue { get; }
    public Type TokenType { get; }

    public Token(string tokenValue, Type tokenType)
    {
        TokenValue = tokenValue;
        TokenType = tokenType;
    }

    public static bool IsBasicOperator(string token)
    {
        for (var index = 0; index < basicOperators.Length; index++)
        {
            if (basicOperators[index] == token) return true;
        }

        return false;
    }

    public static bool IsMultiCharOperator(string token)
    {
        for (var index = 0; index < multiCharacterOperators.Length; index++)
        {
            if (multiCharacterOperators[index] == token) return true;
        }

        return false;
    }

    public override string ToString()
    {
        return $"{TokenType.ToString()}: {TokenValue}";
    }
}