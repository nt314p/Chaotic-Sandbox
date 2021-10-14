public class Token
{
    public string Value { get; }
    public TokenType Type { get; }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public override string ToString()
    {
        return $"{Type.ToString()}: {Value}";
    }
}