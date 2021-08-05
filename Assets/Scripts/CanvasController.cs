using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private TMP_Text resultText;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void Evaluate()
    {
        var text = userInput.text;
        if (text.Length == 0) return;
        var tokenizer = new Tokenizer(userInput.text);
        var tokens = new List<Token>();
        try
        {
            tokens = Parser.ConvertInfixToPostfix(tokenizer.GetTokens());
            //var evaluated = Parser.EvaluatePostfixExpression(tokens).ToString();
            resultText.color = Color.white;
            //resultText.text = evaluated;
            string output = "";
            for (var index = 0; index < tokens.Count; index++)
            {
                output += tokens[index] + "\n";
            }

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
