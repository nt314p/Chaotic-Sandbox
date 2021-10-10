using System;
using System.Collections.Generic;
using System.Data;

public class Parser
{
    public static List<EquationToken> ConvertToEquationTokens(List<Token> tokens)
    {
        var equationTokens = new List<EquationToken>();
        var previousEquationTokenType = EquationToken.TokenType.Bad;
        var numberOperandString = "";
        var index = 0;

        while (index < tokens.Count)
        {
            var token = tokens[index];

            if (token.Type == Token.TokenType.NumberLiteral || token.Type == Token.TokenType.Dot)
            {
                numberOperandString += token.Value;
                index++;
                continue;
            }

            if (numberOperandString.Length > 0) // token type is not numerical operand, add number token
            {
                var numericalOperandToken = ParseNumericalOperand(numberOperandString);
                equationTokens.Add(numericalOperandToken);
                previousEquationTokenType = EquationToken.TokenType.NumericalOperand;
                numberOperandString = "";
            }

            if (token.Type == Token.TokenType.StringLiteral)
            {
                var stringEquationTokenType = ConvertStringToEquationToken(token.Value);
                equationTokens.Add(new EquationToken(token.Value, stringEquationTokenType));
                previousEquationTokenType = stringEquationTokenType;
                index++;
                continue;
            }

            var equationTokenType = EquationToken.TokenType.Bad;
            switch (token.Type)
            {
                case Token.TokenType.OpenParenthesis:
                    equationTokenType = EquationToken.TokenType.OpenParenthesis;
                    break;
                case Token.TokenType.CloseParenthesis:
                    equationTokenType = EquationToken.TokenType.CloseParenthesis;
                    break;
                case Token.TokenType.Plus:
                    equationTokenType = EquationToken.TokenType.AdditionOperator;
                    break;
                case Token.TokenType.Minus:
                    equationTokenType = !IsOperand(previousEquationTokenType)
                        ? EquationToken.TokenType.NegationOperator
                        : EquationToken.TokenType.SubtractionOperator;
                    break;
                case Token.TokenType.Asterisk:
                    equationTokenType = EquationToken.TokenType.MultiplicationOperator;
                    break;
                case Token.TokenType.ForwardSlash:
                    equationTokenType = EquationToken.TokenType.DivisionOperator;
                    break;
                case Token.TokenType.Caret:
                    equationTokenType = EquationToken.TokenType.ExponentiationOperator;
                    break;
                case Token.TokenType.Equals:
                    equationTokenType = EquationToken.TokenType.EqualsOperator;
                    break;
            }
            
            equationTokens.Add(new EquationToken(token.Value, equationTokenType));
            previousEquationTokenType = equationTokenType;
            index++;
        }
        
        if (numberOperandString.Length > 0)
        {
            var numericalOperandToken = ParseNumericalOperand(numberOperandString);
            equationTokens.Add(numericalOperandToken);
        }

        return equationTokens;
    }
    
    /* 
    public static List<Token> ConvertInfixToPostfix(List<Token> tokens)
    {
        var postfixTokens = new List<Token>();
        var operatorStack = new Stack<Token>();

        for (var index = 0; index < tokens.Count; index++)
        {
            var currentToken = tokens[index];
            if (currentToken.Type == Token.Type.Operand)
            {
                postfixTokens.Add(currentToken);
                continue;
            }

            if (currentToken.Type == Token.Type.UnaryOperator || currentToken.Type == Token.Type.BinaryOperator)
            {
                var currentOperator = (Operator) currentToken;
                var currentOperatorPriority = currentOperator.Priority();
                while (operatorStack.Count > 0 &&
                       operatorStack.Peek().Type != Token.Type.Parenthesis &&
                       ((Operator) operatorStack.Peek()).Priority() >= currentOperatorPriority
                )
                {
                    postfixTokens.Add(operatorStack.Pop());
                }

                operatorStack.Push(currentOperator);
            }

            if (currentToken.Type == Token.Type.Parenthesis)
            {
                if (currentToken.Value == "(") operatorStack.Push(currentToken);
                if (currentToken.Value == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().Value != "(")
                    {
                        postfixTokens.Add(operatorStack.Pop());
                    }

                    if (operatorStack.Count > 0) operatorStack.Pop();
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            var operatorToken = operatorStack.Pop();
            if (operatorToken.Type != Token.Type.Parenthesis) postfixTokens.Add(operatorToken);
        }

        return postfixTokens;
    }

    public static double EvaluatePostfixExpression(List<Token> tokens)
    {
        var tokenStack = new Stack<Token>();
        for (var index = 0; index < tokens.Count; index++)
        {
            var currentToken = tokens[index];
            if (currentToken.Type == Token.Type.Operand)
            {
                tokenStack.Push(currentToken);
                continue;
            }

            var tokenOperator = (Operator) currentToken;
            if (tokenOperator.OperationType == Operator.Operation.Negation)
            {
                var tokenValue = double.Parse(tokenStack.Pop().Value);
                tokenStack.Push(new Token((-tokenValue).ToString(), Token.Type.Operand));
                continue;
            }

            var b = double.Parse(tokenStack.Pop().Value);
            var a = double.Parse(tokenStack.Pop().Value);
            double result = 0;
            if (tokenOperator.OperationType == Operator.Operation.Addition)
                result = a + b;
            else if (tokenOperator.OperationType == Operator.Operation.Subtraction)
                result = a - b;
            else if (tokenOperator.OperationType == Operator.Operation.Multiplication)
                result = a * b;
            else if (tokenOperator.OperationType == Operator.Operation.Division)
                result = a / b;
            else if (tokenOperator.OperationType == Operator.Operation.Exponentiation)
                result = Math.Pow(a, b);

            tokenStack.Push(new Token(result.ToString(), Token.Type.Operand));
        }

        return double.Parse(tokenStack.Pop().Value);
    }*/

    private static EquationToken.TokenType ConvertStringToEquationToken(string token)
    {
        switch (token)
        {
            case "sin":
                return EquationToken.TokenType.SineOperator;
            case "cos":
                return EquationToken.TokenType.CosineOperator;
            case "tan":
                return EquationToken.TokenType.TangentOperator;
            case "abs":
                return EquationToken.TokenType.AbsoluteOperator;
            case "floor":
                return EquationToken.TokenType.FloorOperator;
            case "ceil":
                return EquationToken.TokenType.CeilingOperator;
            case "sqrt":
                return EquationToken.TokenType.SquareRootOperator;
            case "cbrt":
                return EquationToken.TokenType.CubeRootOperator;
            default:
                return EquationToken.TokenType.VariableOperand;
        }
    }

    private static bool IsOperand(EquationToken.TokenType tokenType)
    {
        return tokenType == EquationToken.TokenType.NumericalOperand ||
               tokenType == EquationToken.TokenType.VariableOperand;
    }

    private static bool IsUnaryOperator(EquationToken.TokenType tokenType)
    {
        return tokenType == EquationToken.TokenType.NegationOperator ||
               tokenType == EquationToken.TokenType.SineOperator ||
               tokenType == EquationToken.TokenType.CosineOperator ||
               tokenType == EquationToken.TokenType.TangentOperator ||
               tokenType == EquationToken.TokenType.AbsoluteOperator ||
               tokenType == EquationToken.TokenType.FloorOperator ||
               tokenType == EquationToken.TokenType.CeilingOperator ||
               tokenType == EquationToken.TokenType.SquareRootOperator ||
               tokenType == EquationToken.TokenType.CubeRootOperator;
    }
    
    private static bool IsBinaryOperator(EquationToken.TokenType tokenType)
    {
        return tokenType == EquationToken.TokenType.AdditionOperator ||
               tokenType == EquationToken.TokenType.SubtractionOperator ||
               tokenType == EquationToken.TokenType.MultiplicationOperator ||
               tokenType == EquationToken.TokenType.DivisionOperator ||
               tokenType == EquationToken.TokenType.ExponentiationOperator ||
               tokenType == EquationToken.TokenType.EqualsOperator;
    }

    private static bool IsOperator(EquationToken.TokenType tokenType)
    {
        return IsUnaryOperator(tokenType) || IsBinaryOperator(tokenType);
    }

    private static EquationToken ParseNumericalOperand(string numericalOperandString)
    {
        var decimalCount = 0;
        for (var i = 0; i < numericalOperandString.Length; i++)
        {
            if (numericalOperandString[i] != '.') continue;
            decimalCount++;
            if (decimalCount >= 2)
            {
                throw new InvalidExpressionException(
                    $"Numerical operand '{numericalOperandString}' contains too many decimal points");
            }
        }
        return new EquationToken(numericalOperandString, EquationToken.TokenType.NumericalOperand);
    }
}