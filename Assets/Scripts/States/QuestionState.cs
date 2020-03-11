using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestionState
{
    private static int currentQuestion = 0;
    
    public static void Init()
    {
        currentQuestion = 0;
    }

    public static void NextQuestion()
    {
        currentQuestion++;
    }

    public static int GetCurrentQuestion()
    {
        return currentQuestion;
    }
}