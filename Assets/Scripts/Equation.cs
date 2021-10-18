using System.Collections.Generic;

public class Equation
{
    public List<EquationToken> EquationTokens { get; }
    public string LeftHandVariable { get; }
    public List<string> RightHandVariables { get; }

    public Equation(List<EquationToken> equationTokens)
    {
        EquationTokens = equationTokens;
        RightHandVariables = new List<string>();

        if (equationTokens[0].Type == EquationTokenType.VariableOperand)
        {
            LeftHandVariable = equationTokens[0].Value;
        }

        for (var i = 2; i < equationTokens.Count; i++)
        {
            var token = equationTokens[i];
            if (token.Type == EquationTokenType.VariableOperand)
            {
                RightHandVariables.Add(token.Value);
            }
        }
    }

    public string ToCodeString()
    {
        var code = "";
        foreach (var token in EquationTokens)
        {
            code += token.Type == EquationTokenType.NumericalOperand ? token.NumericalValue.ToString() : token.Value;
        }

        return code;
    }
}