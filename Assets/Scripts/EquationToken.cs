using System;

public class EquationToken
{
    public EquationTokenType Type { get; }
    public string Value { get; }
    private double numericalValue;
    public double NumericalValue
    {
        get
        {
            if (!Type.IsOperand()) throw new InvalidOperationException("Non operand token has no numerical value");
            return numericalValue;
        }
        set => numericalValue = value;
    }

    public EquationToken(string value, EquationTokenType type)
    {
        Value = value;
        Type = type;
        NumericalValue = double.NaN;
    }

    public EquationToken(double numericalValue, EquationTokenType type)
    {
        NumericalValue = numericalValue;
        Type = type;
    }
    
    public override string ToString()
    {
        return $"{Type.ToString()}: {Value}";
    }
}