using System;
using System.Collections.Generic;

public class Parser
{
    public static List<Token> ConvertInfixToPostfix(List<Token> tokens)
    {
        var postfixTokens = new List<Token>();
        var operatorStack = new Stack<Token>();

        for (var index = 0; index < tokens.Count; index++)
        {
            var currentToken = tokens[index];
            if (currentToken.TokenType == Token.Type.Operand)
            {
                postfixTokens.Add(currentToken);
                continue;
            }

            if (currentToken.TokenType == Token.Type.UnaryOperator || currentToken.TokenType == Token.Type.BinaryOperator)
            {
                var currentOperator = (Operator) currentToken;
                var currentOperatorPriority = currentOperator.Priority();
                while (operatorStack.Count > 0 &&
                       operatorStack.Peek().TokenType != Token.Type.Parenthesis &&
                       ((Operator) operatorStack.Peek()).Priority() >= currentOperatorPriority
                )
                {
                    postfixTokens.Add(operatorStack.Pop());
                }

                operatorStack.Push(currentOperator);
            }

            if (currentToken.TokenType == Token.Type.Parenthesis)
            {
                if (currentToken.TokenValue == "(") operatorStack.Push(currentToken);
                if (currentToken.TokenValue == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().TokenValue != "(")
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
            if (operatorToken.TokenType != Token.Type.Parenthesis) postfixTokens.Add(operatorToken);
        }

        return postfixTokens;
    }

    public static double EvaluatePostfixExpression(List<Token> tokens)
    {
        var tokenStack = new Stack<Token>();
        for (var index = 0; index < tokens.Count; index++)
        {
            var currentToken = tokens[index];
            if (currentToken.TokenType == Token.Type.Operand)
            {
                tokenStack.Push(currentToken);
                continue;
            }

            var tokenOperator = (Operator) currentToken;
            if (tokenOperator.OperationType == Operator.Operation.Negation)
            {
                var tokenValue = double.Parse(tokenStack.Pop().TokenValue);
                tokenStack.Push(new Token((-tokenValue).ToString(), Token.Type.Operand));
                continue;
            }

            var b = double.Parse(tokenStack.Pop().TokenValue);
            var a = double.Parse(tokenStack.Pop().TokenValue);
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

        return double.Parse(tokenStack.Pop().TokenValue);
    }
}