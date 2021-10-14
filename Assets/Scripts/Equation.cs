using System.Collections.Generic;

public class Equation
{
    private List<EquationToken> equationTokens;
    public string LeftHandVariable { get; }
    public List<string> RightHandVariables { get; }

    public Equation(List<EquationToken> equationTokens)
    {
        this.equationTokens = equationTokens;
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

    public List<EquationToken> GetEquationTokens()
    {
        return equationTokens;
    }
}