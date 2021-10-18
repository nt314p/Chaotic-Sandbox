using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class EquationSystem
{
    private Dictionary<string, double> variableDictionary;
    private List<Equation> equations;
    private Dictionary<Equation, HashSet<string>> equationDependencies;

    public EquationSystem(List<Equation> equations)
    {
        this.equations = equations;
        equationDependencies = new Dictionary<Equation, HashSet<string>>();
        variableDictionary = new Dictionary<string, double>();

        foreach (var equation in equations)
        {
            variableDictionary.Add(equation.LeftHandVariable, double.NaN);
            equationDependencies.Add(equation, new HashSet<string>(equation.RightHandVariables));
        }

        foreach (var equation in equations)
        {
            var rightHandVariables = equation.RightHandVariables;
            foreach (var variable in rightHandVariables)
            {
                if (!variableDictionary.ContainsKey(variable))
                    throw new InvalidExpressionException($"Undefined variable '{variable}'");
            }
        }
    }

    public Dictionary<string, double> EvaluateSystem()
    {
        var index = 0;
        while (index < equationDependencies.Count)
        {
            var equation = equationDependencies.Keys.ToList()[index];
            if (equationDependencies[equation].Count != 0)
            {
                index++;
                continue;
            }

            var equationTokens = equation.EquationTokens;
            equationTokens.RemoveRange(0, 2); // remove variable and equal sign
            var result = EvaluatePostfixExpression(Parser.ConvertInfixToPostfix(equationTokens));
            variableDictionary[equation.LeftHandVariable] = result; 
            equationDependencies.Remove(equation);
            index = 0;
                
            foreach (var equationDep in equationDependencies.Keys)
            {
                // remove variable from dependencies as it has been evaluated
                equationDependencies[equationDep].Remove(equation.LeftHandVariable);
            }
        }
        
        if (equationDependencies.Count > 0)
            throw new InvalidExpressionException("System contains circular variable references");

        return variableDictionary;
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
                    tokenStack.Push(new EquationToken(variableDictionary[currentToken.Value], EquationTokenType.NumericalOperand));
                    continue;
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