using System;

public enum EquationTokenType
{
    Bad,
        
    VariableOperand,
    NumericalOperand,
    AdditionOperator,
    SubtractionOperator,
    MultiplicationOperator,
    DivisionOperator,
    ExponentiationOperator,
    NegationOperator,
    SineOperator,
    CosineOperator,
    TangentOperator,
    AbsoluteOperator,
    FloorOperator,
    CeilingOperator,
    SquareRootOperator,
    CubeRootOperator,
    EqualsOperator,
        
    OpenParenthesis,
    CloseParenthesis
}

public static class EquationTokenTypeExtensions
{
    public static bool IsOperand(this EquationTokenType equationTokenType)
    {
        return equationTokenType == EquationTokenType.NumericalOperand ||
               equationTokenType == EquationTokenType.VariableOperand;
    }
    
    public static bool IsUnaryOperator(this EquationTokenType equationTokenType)
    {
        return equationTokenType == EquationTokenType.NegationOperator ||
               equationTokenType == EquationTokenType.SineOperator ||
               equationTokenType == EquationTokenType.CosineOperator ||
               equationTokenType == EquationTokenType.TangentOperator ||
               equationTokenType == EquationTokenType.AbsoluteOperator ||
               equationTokenType == EquationTokenType.FloorOperator ||
               equationTokenType == EquationTokenType.CeilingOperator ||
               equationTokenType == EquationTokenType.SquareRootOperator ||
               equationTokenType == EquationTokenType.CubeRootOperator;
    }
    
    public static bool IsBinaryOperator(this EquationTokenType equationTokenType)
    {
        return equationTokenType == EquationTokenType.AdditionOperator ||
               equationTokenType == EquationTokenType.SubtractionOperator ||
               equationTokenType == EquationTokenType.MultiplicationOperator ||
               equationTokenType == EquationTokenType.DivisionOperator ||
               equationTokenType == EquationTokenType.ExponentiationOperator ||
               equationTokenType == EquationTokenType.EqualsOperator;
    }
    
    public static bool IsOperator(this EquationTokenType equationTokenType)
    {
        return IsUnaryOperator(equationTokenType) || IsBinaryOperator(equationTokenType);
    }
    
    public static bool IsParenthesis(this EquationTokenType equationTokenType)
    {
        return equationTokenType == EquationTokenType.OpenParenthesis ||
               equationTokenType == EquationTokenType.CloseParenthesis;
    }
    
    public static int OperatorPriority(this EquationTokenType equationTokenType)
    {
        switch (equationTokenType)
        {
            case EquationTokenType.AdditionOperator:
            case EquationTokenType.SubtractionOperator:
                return 1;
            case EquationTokenType.MultiplicationOperator:
            case EquationTokenType.DivisionOperator:
                return 2;
            case EquationTokenType.ExponentiationOperator:
                return 3;
            case EquationTokenType.NegationOperator:
            case EquationTokenType.SineOperator:
            case EquationTokenType.CosineOperator:
            case EquationTokenType.TangentOperator:
            case EquationTokenType.AbsoluteOperator:
            case EquationTokenType.FloorOperator:
            case EquationTokenType.CeilingOperator:
            case EquationTokenType.SquareRootOperator:
            case EquationTokenType.CubeRootOperator:
                return 4;
            default:
                throw new InvalidOperationException($"Priority is undefined for token type '{equationTokenType.ToString()}'");
        }
    }
}