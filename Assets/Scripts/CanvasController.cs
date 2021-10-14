using System;
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
        var tokenizer = new Tokenizer(userInput.text);
        try
        {
            var tokens = tokenizer.GetTokens();
            var equationTokens = Parser.ConvertToEquationTokens(tokens);
            equationTokens = Parser.ConvertInfixToPostfix(equationTokens);
            //var evaluated = Parser.EvaluatePostfixExpression(equationTokens).ToString();
            resultText.color = Color.white;
            
            var output = "";
            for (var index = 0; index < equationTokens.Count; index++)
            {
                output += equationTokens[index] + "\n";
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
