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

    public override string ToString()
    {
        return $"{Type.ToString()}: {Value}";
    }
}