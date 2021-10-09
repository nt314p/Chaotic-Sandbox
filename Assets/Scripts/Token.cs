public class Token
{
    public enum TokenType
    {
        Bad,
        
        // Basic operators, + - * / ^
        Plus,
        Minus,
        Asterisk,
        ForwardSlash,
        Caret,

        OpenParenthesis,
        CloseParenthesis,
        
        Equals,
        Dot,
        
        StringLiteral, // variables and multi character operators like sin, abs, ceil
        NumberLiteral
    }

    //private static string[] basicOperators = {"+","-","*","/","^"};

    public string Value { get; }
    public TokenType Type { get; }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public Token(char tokenValue, TokenType type)
    {
        Value = tokenValue.ToString();
        Type = type;
    }

    // public static bool IsBasicOperator(string token)
    // {
    //     for (var index = 0; index < basicOperators.Length; index++)
    //     {
    //         if (basicOperators[index] == token) return true;
    //     }
    //
    //     return false;
    // }

    // public static bool IsMultiCharOperator(string token)
    // {
    //     return Operator.StringToOperation(token) != Operator.Operation.Undefined;
    // }

    public override string ToString()
    {
        return $"{Type.ToString()}: {Value}";
    }
}