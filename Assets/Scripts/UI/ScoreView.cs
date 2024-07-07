using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _score = 0;
    [SerializeField] private int _scoring = 10;

    public int Score { get { return _score; }  }

    private void Start()
    {
        _scoreText = GetComponent<TMP_Text>();
        _scoreText.text = $"Score:\n{_score}";
        EventManager.ScoringEvent += OnAccrualOfPoints;
        EventManager.ClearScore += OnClearScore;
    }

    private void OnDestroy()
    {
        EventManager.ClearScore -= OnClearScore;
        EventManager.ScoringEvent -= OnAccrualOfPoints;
    }
    private void OnAccrualOfPoints()
    {
        _score+= _scoring;
        _scoreText.text = $"Score:\n{_score}";
    }
    private void OnClearScore()
    {
        _score = 0 ;
        _scoreText.text = $"Score:\n{_score}";
    }
}
