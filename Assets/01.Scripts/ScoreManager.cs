using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const int CreamStackScore = 10;
    private const int OreoScoreMultiplier = 5;

    private int _currentScore;

    public int CurrentScore => _currentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnCreamStacked += HandleCreamStacked;
        GameEvents.OnOreoCompleted += HandleOreoCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnCreamStacked -= HandleCreamStacked;
        GameEvents.OnOreoCompleted -= HandleOreoCompleted;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void HandleCreamStacked()
    {
        AddScore(CreamStackScore);
    }

    private void HandleOreoCompleted(int creamCount)
    {
        int oreoScore = creamCount * creamCount * OreoScoreMultiplier;
        AddScore(oreoScore);
    }

    private void AddScore(int amount)
    {
        _currentScore += amount;
        GameEvents.RaiseScoreChanged(_currentScore);
    }

    public void ResetScore()
    {
        _currentScore = 0;
        GameEvents.RaiseScoreChanged(_currentScore);
    }
}
