using System;
using System.Collections.Generic;
using System.Data;

public class Parser
{
    public static List<EquationToken> ConvertToEquationTokens(List<Token> tokens)
    {
        var equationTokens = new List<EquationToken>();
        var previousEquationTokenType = EquationToken.TokenType.Bad;
        var index = 0;
        var parenthesisCount = 0;

        while (index < tokens.Count)
        {
            var token = tokens[index];
            var tokenType = token.Type;
            
            var equationTokenType = EquationToken.TokenType.Bad;
            switch (tokenType)
            {
                case Token.TokenType.Bad:
                    throw new InvalidExpressionException($"Bad token '{token.Value}'");
                case Token.TokenType.Whitespace:
                    //previousEquationTokenType = EquationToken.TokenType.Whitespace;
                    index++;
                    continue;
                case Token.TokenType.Identifier:
                    equationTokenType = ConvertIdentifierToEquationToken(token.Value);
                    break;
                case Token.TokenType.OpenParenthesis:
                    equationTokenType = EquationToken.TokenType.OpenParenthesis;
                    parenthesisCount++;
                    break;
                case Token.TokenType.CloseParenthesis:
                    if (previousEquationTokenType == EquationToken.TokenType.OpenParenthesis)
                    {
                        throw new InvalidExpressionException("Cannot have empty parenthesis");
                    }

                    if (IsOperator(previousEquationTokenType))
                    {
                        var previousOperator = equationTokens[equationTokens.Count - 1];
                        throw new InvalidExpressionException(
                            $"Operator '{previousOperator.Value}' is missing an operand");
                    }
                    equationTokenType = EquationToken.TokenType.CloseParenthesis;
                    parenthesisCount--;
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
                case Token.TokenType.NumberLiteral:
                    ParseNumericalOperand(token);
                    equationTokenType = EquationToken.TokenType.NumericalOperand;
                    break;
            }

            if (HasImpliedMultiply(previousEquationTokenType, equationTokenType))
            {
                equationTokens.Add(new EquationToken("*", EquationToken.TokenType.MultiplicationOperator));
            }
            
            /* u: unary operator, b: binary operator
             * ub: sin+    :(
             * uu: sin sin :)
             * bb: + /     :(
             * bu: +sin    :)
             */
            if (IsOperator(previousEquationTokenType) &&
                IsOperator(equationTokenType) &&
                IsBinaryOperator(equationTokenType))
            {
                throw new InvalidExpressionException($"Expected operand but got operator '{token.Value}'");
            }

            if (parenthesisCount < 0) throw new InvalidExpressionException("Unmatched close parenthesis");

            equationTokens.Add(new EquationToken(token.Value, equationTokenType));
            previousEquationTokenType = equationTokenType;
            index++;
        }

        if (IsOperator(previousEquationTokenType))
            throw new InvalidExpressionException("Expression cannot end with operator");

        if (parenthesisCount != 0) throw new InvalidExpressionException("Unmatched open parenthesis");

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
            
            if (IsOperand(currentTokenType))
            {
                postfixTokens.Add(currentToken);
                continue;
            }

            if (IsOperator(currentTokenType))
            {
                var currentOperatorPriority = OperatorPriority(currentTokenType);
                while (operatorStack.Count > 0 &&
                       !IsParenthesis(operatorStack.Peek().Type) &&
                       OperatorPriority(operatorStack.Peek().Type) > currentOperatorPriority
                )
                {
                    postfixTokens.Add(operatorStack.Pop());
                }

                operatorStack.Push(currentToken);
            }
            
            switch (currentTokenType)
            {
                case EquationToken.TokenType.OpenParenthesis:
                    operatorStack.Push(currentToken);
                    break;
                case EquationToken.TokenType.CloseParenthesis:
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
            if (!IsParenthesis(operatorToken.Type)) postfixTokens.Add(operatorToken);
        }

        return postfixTokens;
    }
    
    
    public static double EvaluatePostfixExpression(List<EquationToken> equationTokens)
    {
        var tokenStack = new Stack<EquationToken>();
        for (var index = 0; index < equationTokens.Count; index++)
        {
            var currentToken = equationTokens[index];
            var currentTokenType = currentToken.Type;
            if (IsOperand(currentTokenType))
            {
                if (currentTokenType == EquationToken.TokenType.VariableOperand)
                {
                    throw new InvalidExpressionException("Variable operands not yet supported");
                    // TODO: evaluate variable value
                }
                tokenStack.Push(currentToken);
                continue;
            }

            if (IsUnaryOperator(currentTokenType))
            {
                var n = double.Parse(tokenStack.Pop().Value);
                switch (currentTokenType)
                { 
                    case EquationToken.TokenType.NegationOperator:
                        n = -n;
                        break;
                    case EquationToken.TokenType.SineOperator:
                        n = Math.Sin(n);
                        break;
                    case EquationToken.TokenType.CosineOperator:
                        n = Math.Cos(n);
                        break;
                    case EquationToken.TokenType.TangentOperator:
                        n = Math.Tan(n);
                        break;
                    case EquationToken.TokenType.AbsoluteOperator:
                        n = Math.Abs(n);
                        break;
                    case EquationToken.TokenType.FloorOperator:
                        n = Math.Floor(n);
                        break;
                    case EquationToken.TokenType.CeilingOperator:
                        n = Math.Ceiling(n);
                        break;
                    case EquationToken.TokenType.SquareRootOperator:
                        n = Math.Sqrt(n);
                        break;
                    case EquationToken.TokenType.CubeRootOperator:
                        n = Math.Pow(n, 1.0 / 3.0);
                        break;
                }
                tokenStack.Push(new EquationToken(n.ToString(), EquationToken.TokenType.NumericalOperand));
                continue;
            }

            var b = double.Parse(tokenStack.Pop().Value);
            var a = double.Parse(tokenStack.Pop().Value);
            double result = 0;

            switch (currentTokenType)
            {
                case EquationToken.TokenType.AdditionOperator:
                    result = a + b;
                    break;
                case EquationToken.TokenType.SubtractionOperator:
                    result = a - b;
                    break;
                case EquationToken.TokenType.MultiplicationOperator:
                    result = a * b;
                    break;
                case EquationToken.TokenType.DivisionOperator:
                    result = a / b;
                    break;
                case EquationToken.TokenType.ExponentiationOperator:
                    result = Math.Pow(a, b);
                    break;
            }

            tokenStack.Push(new EquationToken(result.ToString(), EquationToken.TokenType.NumericalOperand));
        }

        return double.Parse(tokenStack.Pop().Value);
    }

    private static EquationToken.TokenType ConvertIdentifierToEquationToken(string token)
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
    private static bool HasImpliedMultiply(EquationToken.TokenType previousTokenType,
        EquationToken.TokenType currentTokenType)
    {
        // previous token must be ), n, v
        if (previousTokenType != EquationToken.TokenType.CloseParenthesis &&
            previousTokenType != EquationToken.TokenType.NumericalOperand &&
            previousTokenType != EquationToken.TokenType.VariableOperand) return false;
        
        // current token must be v, u, (, n
        if (currentTokenType != EquationToken.TokenType.VariableOperand &&
            !(IsUnaryOperator(currentTokenType) && currentTokenType != EquationToken.TokenType.NegationOperator) &&
            currentTokenType != EquationToken.TokenType.OpenParenthesis &&
            currentTokenType != EquationToken.TokenType.NumericalOperand) return false;
        
        switch (previousTokenType)
        {
            case EquationToken.TokenType.CloseParenthesis:
                return true;
            case EquationToken.TokenType.NumericalOperand:
                return currentTokenType != EquationToken.TokenType.NumericalOperand;
            case EquationToken.TokenType.VariableOperand:
                return currentTokenType == EquationToken.TokenType.OpenParenthesis;
        }

        return false;
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

    private static bool IsParenthesis(EquationToken.TokenType tokenType)
    {
        return tokenType == EquationToken.TokenType.OpenParenthesis ||
               tokenType == EquationToken.TokenType.CloseParenthesis;
    }

    private static int OperatorPriority(EquationToken.TokenType tokenType)
    {
        switch (tokenType)
        {
            case EquationToken.TokenType.AdditionOperator:
            case EquationToken.TokenType.SubtractionOperator:
                return 1;
            case EquationToken.TokenType.MultiplicationOperator:
            case EquationToken.TokenType.DivisionOperator:
                return 2;
            case EquationToken.TokenType.ExponentiationOperator:
                return 3;
            case EquationToken.TokenType.NegationOperator:
                return 4;
            case EquationToken.TokenType.SineOperator:
            case EquationToken.TokenType.CosineOperator:
            case EquationToken.TokenType.TangentOperator:
            case EquationToken.TokenType.AbsoluteOperator:
            case EquationToken.TokenType.FloorOperator:
            case EquationToken.TokenType.CeilingOperator:
            case EquationToken.TokenType.SquareRootOperator:
            case EquationToken.TokenType.CubeRootOperator:
                return 4;
            default:
                return -1;
        }
    }

    private static void ParseNumericalOperand(Token token)
    {
        var decimalCount = 0;
        var tokenValue = token.Value;
        for (var i = 0; i < tokenValue.Length; i++)
        {
            if (tokenValue[i] != '.') continue;
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