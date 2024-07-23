using System;
using UnityEngine;
using Zenject;

[Serializable]
public class GameDifficultyManager
{
    private ScoreView _scoreView;
    private Piece _piece;
    [SerializeField] private int _currentScoreForDificult = 0;
    [SerializeField] private DifficultyData _difficultyData;

    public GameDifficultyManager(ScoreView scoreView, Piece piece, DifficultyData difficultyData)
    {
        _scoreView = scoreView;
        _piece = piece;
        _difficultyData = difficultyData;
        _currentScoreForDificult = _scoreView.Score + _difficultyData.SpeedIncreaseFrequency;

    }

    
    public void IncreaseDifficulty()
    {
        if (_scoreView.Score >= _currentScoreForDificult)
        {
            _piece.PitchDelay *= _difficultyData.DifficultyMultiplier;
            _currentScoreForDificult = _scoreView.Score + _difficultyData.SpeedIncreaseFrequency;
            Debug.Log("Increase");
        }
    }
}
