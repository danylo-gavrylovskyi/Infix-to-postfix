﻿bool IsNumberToken(string token)
{
    return int.TryParse(token, out _);
}

bool IsOperatorToken(string token)
{
    return token is "-" or "+" or "*" or "/" or "^";
}

bool IsLeftBracketToken(string token)
{
    return token == "(";
}

bool IsRightBracketToken(string token)
{
    return token == ")";
}

int Priority(string token)
{
    switch (token)
    {
        case "(":
        case ")":
            return 0;
        case "+":
        case "-":
            return 1;
        case "*":
        case "/":
            return 2;
        case "^":
            return 3;
        default:
            return -1;
    }
}

bool IsLeftAssociative(string token)
{
    return IsOperatorToken(token) && token != "^";
}

string[] Tokenizer(string input)
{
    string[] tokens = new string[20];
    int tokenIndex = -1;
    string postfix = "";

    void Push(string token)
    {
        tokens[++tokenIndex] = token;
    }

    foreach (char el in input)
    {
        if (Char.IsNumber(el))
        {
            postfix += el;
        }
        else if (el is '-' or '+' or '*' or '/' or '^' or '(' or ')')
        {

            if (postfix.Length > 0)
            {
                Push(postfix);
                postfix = "";
            }

            Push(el.ToString());
        }

    }

    if (postfix.Length > 0)
    {
        Push(postfix);
    }

    return tokens;
}

string[] ConvertToRPN(string[] tokens)
{
    string[] queue = new string[tokens.Length];
    int queueIndex = -1;
    void PushQueue(string token)
    {
        queue[++queueIndex] = token;
    }
    string PopQueue()
    {
        string number = queue[queueIndex];
        queue[queueIndex] = null;
        queueIndex--;
        return number;
    }

    string[] operators = new string[20];
    int operatorsIndex = -1;
    void PushOperators(string token)
    {
        operators[++operatorsIndex] = token;
    }
    string PopOperators()
    {
        string number = operators[operatorsIndex];
        operators[operatorsIndex] = null;
        operatorsIndex--;
        return number;
    }

    foreach (string token in tokens)
    {
        if (IsNumberToken(token))
        {
            PushQueue(token);
        }
        else if (IsOperatorToken(token))
        {
            while (operators[0] != null &&
                !IsLeftBracketToken(operators[operatorsIndex]) &&
                (Priority(operators[operatorsIndex]) > Priority(token) ||
                (Priority(operators[operatorsIndex]) == Priority(token) &&
                IsLeftAssociative(token))))
            {
                PushQueue(PopOperators());
            }

            PushOperators(token);
        }
        else if (IsLeftBracketToken(token))
        {
            PushOperators(token);
        }
        else if (IsRightBracketToken(token))
        {
            while (!IsLeftBracketToken(operators[operatorsIndex]))
            {
                if (operators.Length == 0)
                {
                    throw new Exception("Error: Missmatched parentheses!");
                }

                PushQueue(PopOperators());
            }

            if (!IsLeftBracketToken(operators[operatorsIndex]))
            {
                throw new Exception("No left brackets token at the operators stack");
            }

            PopOperators();
        }

    }

    while (operators[0] != null)
    {
        if (IsLeftBracketToken(operators[operatorsIndex]))
        {
            throw new Exception("Error: mismatched parentheses!");
        }

        PushQueue(PopOperators());
    }

    return queue;
}

string[] Evaluation(string[] rpnTokens)
{
    string[] result = new string[20];
    int resultIndex = -1;
    void PushToResult(string token)
    {
        result[++resultIndex] = token;
    }
    string PopResult()
    {
        string value = result[resultIndex];
        result[resultIndex] = null;
        resultIndex--;
        return value;
    }

    foreach (string token in rpnTokens)
    {
        if (IsNumberToken(token))
        {
            PushToResult(token);
        }
        else if (IsOperatorToken(token))
        {
            string secondNum = PopResult();
            string firstNum = PopResult();
            PushToResult(Calculations(firstNum, secondNum, token));
        }
    }
    return result;
}

string Calculations(string num1, string num2, string operatorToken)
{
    int firstNumber, secondNumber;
    int.TryParse(num1, out firstNumber);
    int.TryParse(num2, out secondNumber);
    switch (operatorToken)
    {
        case "+":
            return (firstNumber + secondNumber).ToString();
        case "-":
            return (firstNumber - secondNumber).ToString();
        case "*":
            return (firstNumber * secondNumber).ToString();
        case "/":
            return (firstNumber / secondNumber).ToString();
        case "^":
            return (Math.Pow(firstNumber, secondNumber)).ToString();
        default: throw new Exception("Error in calculations");
    }
}

void Main()
{
    Console.WriteLine("Enter your expression: ");
    string input = Console.ReadLine()!;

    string[] tokens = Tokenizer(input);
    string[] rpnTokens = ConvertToRPN(tokens);

    Console.WriteLine(String.Join(" ", Evaluation(rpnTokens)));
}

Main();