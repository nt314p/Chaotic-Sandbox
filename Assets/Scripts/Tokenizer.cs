using System.Collections.Generic;
using System.Text;

public class Tokenizer
{
    private readonly string line;
    private int cursorIndex = 0;
    private StringBuilder stringBuilder;

    public Tokenizer(string line)
    {
        this.line = line;
        stringBuilder = new StringBuilder();
    }

    private bool HasNextToken()
    {
        return cursorIndex < line.Length;
    }

    public List<Token> GetTokens()
    {
        var tokens = new List<Token>();
        while (HasNextToken())
        {
            tokens.Add(GetNextToken());
        }
        
        return tokens;
    }

    private Token GetNextToken()
    {
        stringBuilder.Clear();
        var currentToken = line[cursorIndex];
        Token.TokenType tokenType;

        switch (currentToken)
        {
            case '(':
                cursorIndex++;
                tokenType = Token.TokenType.OpenParenthesis;
                break;
            case ')':
                cursorIndex++;
                tokenType = Token.TokenType.CloseParenthesis;
                break;
            case '+':
                cursorIndex++;
                tokenType = Token.TokenType.Plus;
                break;
            case '-':
                cursorIndex++;
                tokenType = Token.TokenType.Minus;
                break;
            case '*':
                cursorIndex++;
                tokenType = Token.TokenType.Asterisk;
                break;
            case '/':
                cursorIndex++;
                tokenType = Token.TokenType.ForwardSlash;
                break;
            case '^':
                cursorIndex++;
                tokenType = Token.TokenType.Caret;
                break;
            case '=':
                cursorIndex++;
                tokenType = Token.TokenType.Equals;
                break;
            case '.':
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
                while (cursorIndex < line.Length && IsNumerical(line[cursorIndex]))
                {
                    stringBuilder.Append(line[cursorIndex]);
                    cursorIndex++;
                }

                tokenType = Token.TokenType.NumberLiteral; 
                break;
            case 'a': case 'b': case 'c': case 'd':
            case 'e': case 'f': case 'g': case 'h':
            case 'i': case 'j': case 'k': case 'l':
            case 'm': case 'n': case 'o': case 'p':
            case 'q': case 'r': case 's': case 't':
            case 'u': case 'v': case 'w': case 'x':
            case 'y': case 'z': case 'A': case 'B':
            case 'C': case 'D': case 'E': case 'F':
            case 'G': case 'H': case 'I': case 'J':
            case 'K': case 'L': case 'M': case 'N':
            case 'O': case 'P': case 'Q': case 'R':
            case 'S': case 'T': case 'U': case 'V':
            case 'W': case 'X': case 'Y': case 'Z':
                while (cursorIndex < line.Length && IsLetter(line[cursorIndex]))
                {
                    stringBuilder.Append(line[cursorIndex]);
                    cursorIndex++;
                }

                tokenType = Token.TokenType.Identifier;
                break;
            case ' ':
            case '\t':
                cursorIndex++;
                tokenType = Token.TokenType.Whitespace;
                break;
            default:
                cursorIndex++;
                tokenType = Token.TokenType.Bad;
                break;
        }

        var finalToken = stringBuilder.Length > 0 ? stringBuilder.ToString() : currentToken.ToString();
        
        return new Token(finalToken, tokenType);
    }

    private static bool IsNumerical(char token)
    {
        return char.IsDigit(token) || token == '.';
    }

    private static bool IsLetter(char token)
    {
        return char.IsLetter(token);
    }
}