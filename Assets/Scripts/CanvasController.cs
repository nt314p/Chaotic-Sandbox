using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private TMP_Text resultText;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    public void Evaluate()
    {
        var text = userInput.text;
        if (text.Length == 0) return;
        var textEquations = text.Split('\n');
        var equations = new List<Equation>();
        
        try
        {
            
            foreach (var textEquation in textEquations)
            {
                var tokenizer = new Tokenizer(textEquation);
                var tokens = tokenizer.GetTokens();
                var equationTokens = Parser.ConvertToEquationTokens(tokens);
                var equation = new Equation(equationTokens);
                equations.Add(equation);
            }

            var equationSystem = new EquationSystem(equations);
            var results = equationSystem.EvaluateSystem();
            
            //equationTokens = Parser.ConvertInfixToPostfix(equationTokens);
            //var evaluated = Parser.EvaluatePostfixExpression(equationTokens).ToString();
            resultText.color = Color.white;
            
            var output = "Solutions:\n";
            foreach (var v in results)
            {
                output += $"{v.Key} = {v.Value}\n";
            }

            //resultText.text = evaluated;
            resultText.text = output;
        }
        catch (InvalidExpressionException e)
        {
            resultText.color = new Color(250/255f, 77/255f, 77/255f);
            resultText.text = $"Syntax error: {e.Message}";
        }
        
        // for (var index = 0; index < tokens.Count; index++)
        // {
        //     var token = tokens[index];
        //     Debug.Log(token);
        // }
        // Debug.Log(Parser.EvaluatePostfixExpression(tokens));
    }
}
