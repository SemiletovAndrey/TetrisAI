using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action ScoringEvent;
    public static event Action GameOverEvent;
    public static event Action ClearScore;

    public static void OnScoring()
    {
        ScoringEvent?.Invoke();
    }
    
    public static void OnGameOver()
    {
        GameOverEvent?.Invoke();
    }

    public static void ClearScoringEvent()
    {
        ScoringEvent = null;
    }
    public static void ClearGameOverEvent()
    {
        GameOverEvent = null;
    }

    public static void OnClearScore()
    {
        ClearScore?.Invoke();
    }
}
