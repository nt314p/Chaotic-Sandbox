using System.Collections.Generic;
using System.Data;

public class Tokenizer
{
    private readonly string line;
    private int cursorIndex = 0;

    public Tokenizer(string line)
    {
        this.line = line;
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
        
        // combine individual string and number literals into a single token
        var index = 0;
        var tokenString = "";
        var previousTokenType = Token.TokenType.Bad;
        
        while (index < tokens.Count)
        {
            var token = tokens[index];

            if (token.Type != previousTokenType) // a change in token type means we reached the end of consecutive tokens
            {
                if (tokenString.Length > 0)
                {
                    tokens.Insert(index, new Token(tokenString, previousTokenType));
                    tokenString = "";
                }
            }

            if (token.Type != Token.TokenType.StringLiteral && 
                token.Type != Token.TokenType.NumberLiteral) // ignore non string and number tokens
            {
                index++;
                previousTokenType = token.Type;
                continue;
            }
            
            // only string and number tokens
            tokens.RemoveAt(index);
            tokenString += token.Value;
            previousTokenType = token.Type;
        }

        if (tokenString.Length > 0)
        {
            tokens.Insert(index, new Token(tokenString, previousTokenType));
        }
        
        return tokens;
    }

    private Token GetNextToken()
    {
        var currentToken = line.Substring(cursorIndex, 1)[0];
        var tokenType = Token.TokenType.Bad;
        while (cursorIndex < line.Length - 1 && currentToken == ' ')
        {
            cursorIndex++;
            currentToken = line.Substring(cursorIndex, 1)[0];
        }

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
                cursorIndex++;
                tokenType = Token.TokenType.Dot;
                break;
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
                cursorIndex++;
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
                cursorIndex++;
                tokenType = Token.TokenType.StringLiteral;
                break;
        }

        return new Token(currentToken, tokenType);
    }

    /*
    private Token ParseMultiCharacterToken(ref string currentToken)
    {
        
        var stringTokenType = StringTokenType.MultiCharacterOperator;
        var beginningIdentifierIndex = cursorIndex;
        
        while (cursorIndex < line.Length - 1)
        {
            var nextToken = line[cursorIndex + 1].ToString();
            if (IsParenthesis(nextToken) || Token.IsBasicOperator(nextToken) || nextToken == " ")
            {
                break;
            }

            currentToken += nextToken;
            cursorIndex++;
        }

        var endingIdentifierIndex = cursorIndex;

        var containsLetter = ContainsLetter(currentToken);
        var containsDigit = ContainsDigit(currentToken);
        var containsPeriod = currentToken.Contains(".");
        
        if (containsPeriod && (!containsDigit || containsLetter)) BadIdentifier();
        else if (containsDigit && !containsLetter) stringTokenType = StringTokenType.NumericalOperand;
        else if (Token.IsMultiCharOperator(currentToken)) stringTokenType = StringTokenType.MultiCharacterOperator;
        else if (containsLetter) stringTokenType = StringTokenType.VariableOperand;

        if (stringTokenType == StringTokenType.NumericalOperand)
        {
            if (double.TryParse(currentToken, out _))
            {
                if (_previousTokenTokenType == Token.Type.Operand)
                {
                    throw new InvalidExpressionException(
                        $"Unexpected operand '{currentToken}' at char {beginningIdentifierIndex + 1}");
                }

                cursorIndex++;
                _previousTokenTokenType = Token.Type.Operand;
                return new Token(currentToken, Token.Type.Operand);
            }

            throw new InvalidExpressionException(
                $"Unable to parse operand '{currentToken}' at char {beginningIdentifierIndex + 1} - {endingIdentifierIndex + 1}");
        }

        if (stringTokenType == StringTokenType.VariableOperand)
        {
            if (_previousTokenTokenType == Token.Type.Operand)
            {
                throw new InvalidExpressionException(
                    $"Unexpected operand '{currentToken}' at char {beginningIdentifierIndex + 1}");
            }

            cursorIndex++;
            _previousTokenTokenType = Token.Type.Operand;
            return new Token(currentToken, Token.Type.Operand);
        }

        if (stringTokenType == StringTokenType.MultiCharacterOperator)
        {
            cursorIndex++;
            return new Operator(currentToken, Operator.StringToOperation(currentToken));
        }

        return null;
    }*/

    private static bool IsNumerical(string token)
    {
        return IsDigit(token) || token == ".";
    }

    private void BadIdentifier() => throw new InvalidExpressionException("Bad identifier");

    private static bool IsDigit(string token)
    {
        return char.IsDigit(token[0]);
    }

    private static bool IsParenthesis(string token)
    {
        return token == "(" || token == ")";
    }

    private static bool ContainsLetter(string token)
    {
        foreach (var c in token)
        {
            if (char.IsLetter(c)) return true;
        }

        return false;
    }
    
    private static bool ContainsDigit(string token)
    {
        foreach (var c in token)
        {
            if (char.IsDigit(c)) return true;
        }

        return false;
    }
}