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
        return Operator.StringToOperation(token) != Operator.Operation.Undefined;
    }

    public override string ToString()
    {
        return $"{TokenType.ToString()}: {TokenValue}";
    }
}