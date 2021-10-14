using System;
using System.Collections.Generic;

public class EquationSystem
{
    private Dictionary<string, double> variableDictionary;
    private List<List<EquationToken>> equations;
    
    public EquationSystem(List<List<EquationToken>> equations)
    {
        this.equations = equations;
    }
    
    private double EvaluatePostfixExpression(List<EquationToken> equationTokens)
    {
        var tokenStack = new Stack<EquationToken>();
        for (var index = 0; index < equationTokens.Count; index++)
        {
            var currentToken = equationTokens[index];
            var currentTokenType = currentToken.Type;
            if (currentTokenType.IsOperand())
            {
                if (currentTokenType == EquationTokenType.VariableOperand)
                {
                    continue;
                    //throw new InvalidExpressionException("Variable operands not yet supported");
                    // TODO: evaluate variable value (possibly recursively calling equation eval)
                }
                tokenStack.Push(currentToken);
                continue;
            }

            if (currentTokenType.IsUnaryOperator())
            {
                var n = tokenStack.Pop().NumericalValue;
                switch (currentTokenType)
                { 
                    case EquationTokenType.NegationOperator:
                        n = -n;
                        break;
                    case EquationTokenType.SineOperator:
                        n = Math.Sin(n);
                        break;
                    case EquationTokenType.CosineOperator:
                        n = Math.Cos(n);
                        break;
                    case EquationTokenType.TangentOperator:
                        n = Math.Tan(n);
                        break;
                    case EquationTokenType.AbsoluteOperator:
                        n = Math.Abs(n);
                        break;
                    case EquationTokenType.FloorOperator:
                        n = Math.Floor(n);
                        break;
                    case EquationTokenType.CeilingOperator:
                        n = Math.Ceiling(n);
                        break;
                    case EquationTokenType.SquareRootOperator:
                        n = Math.Sqrt(n);
                        break;
                    case EquationTokenType.CubeRootOperator:
                        n = Math.Pow(n, 0.333333333333333D);
                        break;
                }
                tokenStack.Push(new EquationToken(n, EquationTokenType.NumericalOperand));
                continue;
            }

            var b = tokenStack.Pop().NumericalValue;
            var a = tokenStack.Pop().NumericalValue;
            double result = 0;

            switch (currentTokenType)
            {
                case EquationTokenType.AdditionOperator:
                    result = a + b;
                    break;
                case EquationTokenType.SubtractionOperator:
                    result = a - b;
                    break;
                case EquationTokenType.MultiplicationOperator:
                    result = a * b;
                    break;
                case EquationTokenType.DivisionOperator:
                    result = a / b;
                    break;
                case EquationTokenType.ExponentiationOperator:
                    result = Math.Pow(a, b);
                    break;
                case EquationTokenType.EqualsOperator:
                    // TODO: assign value of b to variable a
                    break;
            }

            tokenStack.Push(new EquationToken(result, EquationTokenType.NumericalOperand));
        }

        return tokenStack.Pop().NumericalValue;
    }
}