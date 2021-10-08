using System;
using System.Collections.Generic;
using System.Data;

public class Tokenizer
{
    private readonly string line;
    private int cursorIndex = 0;
    private Token.Type previousTokenType;

    private enum StringTokenType
    {
        MultiCharacterOperator,
        NumericalOperand,
        VariableOperand
    }

    public Tokenizer(string line)
    {
        this.line = line;
        previousTokenType = Token.Type.Undefined;
    }

    private bool HasNextToken()
    {
        return cursorIndex < line.Length;
    }

    public List<Token> GetTokens()
    {
        var tokens = new List<Token>();
        var parenthesisCount = 0;
        while (HasNextToken())
        {
            var token = GetNextToken();
            if (token == null) continue;
            if (token.TokenType == Token.Type.Parenthesis)
            {
                if (token.TokenValue == "(")
                    parenthesisCount++;
                else if (token.TokenValue == ")") 
                    parenthesisCount--;
            }

            if (parenthesisCount < 0)
            {
                throw new InvalidExpressionException($"Mismatched close parenthesis at char {cursorIndex}");
            }

            tokens.Add(token);
        }

        if (previousTokenType == Token.Type.UnaryOperator || previousTokenType == Token.Type.BinaryOperator)
        {
            throw new InvalidExpressionException("Expression cannot end with operator");
        }

        if (parenthesisCount > 0)
        {
            throw new InvalidExpressionException("Parenthesis mismatch");
        }

        return tokens;
    }

    private Token GetNextToken()
    {
        var currentToken = line.Substring(cursorIndex, 1);
        while (cursorIndex < line.Length - 1 && currentToken[0] == ' ')
        {
            currentToken = line.Substring(++cursorIndex, 1);
        }

        if (currentToken == "(" || currentToken == ")")
        {
            cursorIndex++;
            previousTokenType = Token.Type.Parenthesis;
            return new Token(currentToken, Token.Type.Parenthesis);
        }
        
        if (Token.IsBasicOperator(currentToken))
        {
            if ((previousTokenType == Token.Type.UnaryOperator || previousTokenType == Token.Type.BinaryOperator|| previousTokenType == Token.Type.Undefined)
                && currentToken != "-")
            {
                throw new InvalidExpressionException($"Unexpected operator '{currentToken}' at char {cursorIndex + 1}");
            }

            Operator.Operation operationType;
            var tokenOperatorType = Token.Type.BinaryOperator;
            switch (currentToken)
            {
                case "+":
                    operationType = Operator.Operation.Addition;
                    break;
                case "-":
                    operationType = previousTokenType == Token.Type.UnaryOperator 
                                    || previousTokenType == Token.Type.BinaryOperator 
                                    || previousTokenType == Token.Type.Undefined
                                    || previousTokenType == Token.Type.Parenthesis
                        ? Operator.Operation.Negation
                        : Operator.Operation.Subtraction;
                    if (operationType == Operator.Operation.Negation) tokenOperatorType = Token.Type.UnaryOperator;
                    break;
                case "*":
                    operationType = Operator.Operation.Multiplication;
                    break;
                case "/":
                    operationType = Operator.Operation.Division;
                    break;
                case "^":
                    operationType = Operator.Operation.Exponentiation;
                    break;
                default:
                    throw new InvalidOperationException($"Undefined operator {currentToken}");
            }

            cursorIndex++;
            previousTokenType = tokenOperatorType;

            return new Operator(currentToken, operationType);
        }

        var multiCharacterToken = ParseMultiCharacterToken(ref currentToken);
        if (multiCharacterToken != null)
        {
            //cursorIndex++;
            return multiCharacterToken;
        }

        if (currentToken.Length == 0 || char.IsWhiteSpace(currentToken[0]))
        {
            cursorIndex++;
            return null;
        }

        throw new InvalidExpressionException($"Unexpected symbol '{currentToken}' at char {cursorIndex + 1}");
    }

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
                if (previousTokenType == Token.Type.Operand)
                {
                    throw new InvalidExpressionException(
                        $"Unexpected operand '{currentToken}' at char {beginningIdentifierIndex + 1}");
                }

                cursorIndex++;
                previousTokenType = Token.Type.Operand;
                return new Token(currentToken, Token.Type.Operand);
            }

            throw new InvalidExpressionException(
                $"Unable to parse operand '{currentToken}' at char {beginningIdentifierIndex + 1} - {endingIdentifierIndex + 1}");
        }

        if (stringTokenType == StringTokenType.VariableOperand)
        {
            if (previousTokenType == Token.Type.Operand)
            {
                throw new InvalidExpressionException(
                    $"Unexpected operand '{currentToken}' at char {beginningIdentifierIndex + 1}");
            }

            cursorIndex++;
            previousTokenType = Token.Type.Operand;
            return new Token(currentToken, Token.Type.Operand);
        }

        if (stringTokenType == StringTokenType.MultiCharacterOperator)
        {
            cursorIndex++;
            return new Operator(currentToken, Operator.StringToOperation(currentToken));
        }

        return null;
    }

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