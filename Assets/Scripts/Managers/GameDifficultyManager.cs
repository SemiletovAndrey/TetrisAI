using UnityEngine;

public class GameDifficultyManager : MonoBehaviour
{
    private ScoreView _scoreView;
    private Piece _piece;
    private int _currentScoreForDificult = 0;

    [SerializeField] private int _speedIncreaseFrequency = 20;
    [SerializeField] private float _difficultyMultiplier = 0.8f;

    private void Start()
    {
        _scoreView = FindObjectOfType<ScoreView>();
        _piece = FindObjectOfType<Piece>();
        _currentScoreForDificult = _scoreView.Score + _speedIncreaseFrequency;
    }

    
    public void IncreaseDifficulty()
    {
        if (_scoreView.Score >= _currentScoreForDificult)
        {
            _piece.PitchDelay *= _difficultyMultiplier;
            _currentScoreForDificult = _scoreView.Score + _speedIncreaseFrequency;
        }
    }
}
