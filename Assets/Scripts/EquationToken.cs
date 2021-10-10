public class EquationToken
{
    public enum TokenType
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
    
    public string Value { get; }
    public TokenType Type { get; }

    public EquationToken(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public override string ToString()
    {
        return $"{Type.ToString()}: {Value}";
    }
}