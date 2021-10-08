using System;

public class Operator : Token
{
    public enum Operation
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Exponentiation,
        Negation,
        Sine,
        Cosine,
        Tangent,
        Absolute,
        Floor,
        Ceiling,
        SquareRoot,
        CubeRoot,
        Undefined
    }

    public Operation OperationType { get; }

    public Operator(string tokenValue, Operation operation) : base(tokenValue, OperationDimension(operation))
    {
        OperationType = operation;
    }

    public int Priority()
    {
        switch (OperationType)
        {
            case Operation.Addition:
            case Operation.Subtraction:
                return 1;
            case Operation.Multiplication:
            case Operation.Division:
                return 2;
            case Operation.Exponentiation:
                return 4;
            case Operation.Negation:
            case Operation.Sine:
            case Operation.Cosine:
            case Operation.Tangent:
            case Operation.Absolute:
            case Operation.Floor:
            case Operation.Ceiling:
            case Operation.SquareRoot:
            case Operation.CubeRoot:
                return 3;
            default:
                return -1;
        }
    }

    public static Operation StringToOperation(string token, bool forceUnary = false)
    {
        switch (token)
        {
            case "+": return Operation.Addition;
            case "-": return forceUnary ? Operation.Negation : Operation.Subtraction;
            case "*": return Operation.Multiplication;
            case "/": return Operation.Division;
            case "^": return Operation.Exponentiation;
            case "sin": return Operation.Sine;
            case "cos": return Operation.Cosine;
            case "tan": return Operation.Tangent;
            case "abs": return Operation.Absolute;
            case "floor": return Operation.Floor;
            case "ceil": return Operation.Ceiling;
            case "sqrt": return Operation.SquareRoot;
            case "cbrt": return Operation.CubeRoot;
        }

        //throw new ArgumentException($"{token} is not a valid operator");
        return Operation.Undefined;
    }

    public static Type OperationDimension(Operation operation)
    {
        switch (operation)
        {
            case Operation.Addition:
            case Operation.Subtraction:
            case Operation.Multiplication:
            case Operation.Division:
            case Operation.Exponentiation:
                return Type.BinaryOperator;
            case Operation.Negation:
            case Operation.Sine:
            case Operation.Cosine:
            case Operation.Tangent:
            case Operation.Absolute:
            case Operation.Floor:
            case Operation.Ceiling:
            case Operation.SquareRoot:
            case Operation.CubeRoot:
                return Type.UnaryOperator;
        }

        return Type.Undefined;
    }

    public override string ToString()
    {
        return base.ToString() + $"; Operation: {OperationType.ToString()}";
    }
}