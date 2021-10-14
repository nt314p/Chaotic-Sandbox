using System;
using System.Collections.Generic;
using System.Data;

public static class Parser
{
    public static List<EquationToken> ConvertToEquationTokens(List<Token> tokens)
    {
        var equationTokens = new List<EquationToken>();
        var previousEquationTokenType = EquationTokenType.Bad;
        var index = 0;
        var parenthesisCount = 0;
        var containsEqualOperator = false;

        while (index < tokens.Count)
        {
            var token = tokens[index];
            var tokenType = token.Type;
            
            var equationTokenType = EquationTokenType.Bad;
            switch (tokenType)
            {
                case TokenType.Bad:
                    throw new InvalidExpressionException($"Bad token '{token.Value}'");
                case TokenType.Whitespace:
                    //previousEquationTokenType = EquationTokenType.Whitespace;
                    index++;
                    continue;
                case TokenType.Identifier:
                    equationTokenType = ConvertIdentifierToEquationToken(token.Value);
                    break;
                case TokenType.OpenParenthesis:
                    equationTokenType = EquationTokenType.OpenParenthesis;
                    parenthesisCount++;
                    break;
                case TokenType.CloseParenthesis:
                    if (previousEquationTokenType == EquationTokenType.OpenParenthesis)
                    {
                        throw new InvalidExpressionException("Cannot have empty parenthesis");
                    }

                    if (previousEquationTokenType.IsOperator())
                    {
                        var previousOperator = equationTokens[equationTokens.Count - 1];
                        throw new InvalidExpressionException(
                            $"Operator '{previousOperator.Value}' is missing an operand");
                    }
                    equationTokenType = EquationTokenType.CloseParenthesis;
                    parenthesisCount--;
                    break;
                case TokenType.Plus:
                    equationTokenType = EquationTokenType.AdditionOperator;
                    break;
                case TokenType.Minus:
                    equationTokenType = !previousEquationTokenType.IsOperand()
                        ? EquationTokenType.NegationOperator
                        : EquationTokenType.SubtractionOperator;
                    break;
                case TokenType.Asterisk:
                    equationTokenType = EquationTokenType.MultiplicationOperator;
                    break;
                case TokenType.ForwardSlash:
                    equationTokenType = EquationTokenType.DivisionOperator;
                    break;
                case TokenType.Caret:
                    equationTokenType = EquationTokenType.ExponentiationOperator;
                    break;
                case TokenType.Equals:
                    containsEqualOperator = true;
                    equationTokenType = EquationTokenType.EqualsOperator;
                    break;
                case TokenType.NumberLiteral:
                    ParseNumericalOperand(token);
                    equationTokenType = EquationTokenType.NumericalOperand;
                    break;
            }

            if (HasImpliedMultiply(previousEquationTokenType, equationTokenType))
            {
                equationTokens.Add(new EquationToken("*", EquationTokenType.MultiplicationOperator));
            }
            
            /* u: unary operator, b: binary operator
             * ub: sin+    :(
             * uu: sin sin :)
             * bb: + /     :(
             * bu: +sin    :)
             */
            if (previousEquationTokenType.IsOperator() &&
                equationTokenType.IsOperator() &&
                equationTokenType.IsBinaryOperator())
            {
                throw new InvalidExpressionException($"Expected operand but got operator '{token.Value}'");
            }

            if (parenthesisCount < 0) throw new InvalidExpressionException("Unmatched close parenthesis");

            var equationToken = equationTokenType.IsOperand()
                ? new EquationToken(double.Parse(token.Value), equationTokenType)
                : new EquationToken(token.Value, equationTokenType);
            
            equationTokens.Add(equationToken);
            previousEquationTokenType = equationTokenType;
            index++;
        }

        if (previousEquationTokenType.IsOperator())
            throw new InvalidExpressionException("Expression cannot end with operator");

        if (parenthesisCount != 0) throw new InvalidExpressionException("Unmatched open parenthesis");

        if (containsEqualOperator)
        {
            if (equationTokens[0].Type != EquationTokenType.VariableOperand ||
                equationTokens[1].Type != EquationTokenType.EqualsOperator)
            {
                throw new InvalidExpressionException("Left hand side of assignment must be a single variable");
            }
        }
        
        return equationTokens;
    }
    
    public static List<EquationToken> ConvertInfixToPostfix(List<EquationToken> tokens)
    {
        var postfixTokens = new List<EquationToken>();
        var operatorStack = new Stack<EquationToken>();

        for (var index = 0; index < tokens.Count; index++)
        {
            var currentToken = tokens[index];
            var currentTokenType = currentToken.Type;
            
            if (currentTokenType.IsOperand())
            {
                postfixTokens.Add(currentToken);
                continue;
            }

            if (currentTokenType.IsOperator())
            {
                var currentOperatorPriority = currentTokenType.OperatorPriority();
                while (operatorStack.Count > 0 &&
                       !operatorStack.Peek().Type.IsParenthesis() &&
                       operatorStack.Peek().Type.OperatorPriority() > currentOperatorPriority
                )
                {
                    postfixTokens.Add(operatorStack.Pop());
                }

                operatorStack.Push(currentToken);
            }
            
            switch (currentTokenType)
            {
                case EquationTokenType.OpenParenthesis:
                    operatorStack.Push(currentToken);
                    break;
                case EquationTokenType.CloseParenthesis:
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().Value != "(")
                    {
                        postfixTokens.Add(operatorStack.Pop());
                    }

                    if (operatorStack.Count > 0) operatorStack.Pop();
                    break;
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            var operatorToken = operatorStack.Pop();
            if (!operatorToken.Type.IsParenthesis()) postfixTokens.Add(operatorToken);
        }

        return postfixTokens;
    }

    private static EquationTokenType ConvertIdentifierToEquationToken(string token)
    {
        switch (token)
        {
            case "sin":
                return EquationTokenType.SineOperator;
            case "cos":
                return EquationTokenType.CosineOperator;
            case "tan":
                return EquationTokenType.TangentOperator;
            case "abs":
                return EquationTokenType.AbsoluteOperator;
            case "floor":
                return EquationTokenType.FloorOperator;
            case "ceil":
                return EquationTokenType.CeilingOperator;
            case "sqrt":
                return EquationTokenType.SquareRootOperator;
            case "cbrt":
                return EquationTokenType.CubeRootOperator;
            default:
                return EquationTokenType.VariableOperand;
        }
    }

    /*
     * Implied multiply between
     * v: variable operand
     * u: unary operator (excluding negation)
     * n: number operand
     * 
     * )v = )*v
     * )u = )*u
     * )( = )*(
     * )n = )*n
     * 
     * nv = n*v
     * nu = n*u
     * n( = n*(
     * 
     * v( = v*(
     */
    private static bool HasImpliedMultiply(EquationTokenType previousTokenType,
        EquationTokenType currentTokenType)
    {
        // previous token must be ), n, v
        if (previousTokenType != EquationTokenType.CloseParenthesis &&
            previousTokenType != EquationTokenType.NumericalOperand &&
            previousTokenType != EquationTokenType.VariableOperand) return false;
        
        // current token must be v, u, (, n
        if (currentTokenType != EquationTokenType.VariableOperand &&
            !(currentTokenType.IsUnaryOperator() && currentTokenType != EquationTokenType.NegationOperator) &&
            currentTokenType != EquationTokenType.OpenParenthesis &&
            currentTokenType != EquationTokenType.NumericalOperand) return false;
        
        switch (previousTokenType)
        {
            case EquationTokenType.CloseParenthesis:
                return true;
            case EquationTokenType.NumericalOperand:
                return currentTokenType != EquationTokenType.NumericalOperand;
            case EquationTokenType.VariableOperand:
                return currentTokenType == EquationTokenType.OpenParenthesis;
        }

        return false;
    }

    private static void ParseNumericalOperand(Token token)
    {
        var decimalCount = 0;
        var tokenValue = token.Value;
        foreach (var c in tokenValue)
        {
            if (c != '.') continue;
            decimalCount++;
            if (decimalCount >= 2)
            {
                throw new InvalidExpressionException(
                    $"Numerical operand '{tokenValue}' contains too many decimal points");
            }
        }

        if (!double.TryParse(tokenValue, out _))
        {
            throw new InvalidExpressionException($"Numerical operand '{tokenValue}' cannot be parsed");
        }
    }
}