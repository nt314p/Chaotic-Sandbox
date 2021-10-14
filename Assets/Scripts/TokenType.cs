public enum TokenType
{
    Bad,
    Whitespace,
        
    // Basic operators, + - * / ^
    Plus,
    Minus,
    Asterisk,
    ForwardSlash,
    Caret,

    OpenParenthesis,
    CloseParenthesis,
        
    Equals,
    
    Identifier, // variables like a, dx, k; multi character operators like sin, abs, ceil
    NumberLiteral
}