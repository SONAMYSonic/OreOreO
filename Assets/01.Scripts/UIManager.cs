using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private GameObject _startPrompt;

    [Header("Result UI")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _resultScoreText;
    [SerializeField] private TextMeshProUGUI _resultMessageText;
    [SerializeField] private Button _restartButton;

    private void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateScoreDisplay;
        GameEvents.OnTimerChanged += UpdateTimerDisplay;
        GameEvents.OnGameStateChanged += HandleGameStateChanged;
        GameEvents.OnGameOver += ShowResultPanel;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateScoreDisplay;
        GameEvents.OnTimerChanged -= UpdateTimerDisplay;
        GameEvents.OnGameStateChanged -= HandleGameStateChanged;
        GameEvents.OnGameOver -= ShowResultPanel;
    }

    private void Start()
    {
        if (_restartButton != null)
        {
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }

        UpdateScoreDisplay(0);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"점수: {score}";
        }
    }

    private void UpdateTimerDisplay(float remainingTime)
    {
        if (_timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            _timerText.text = $"시간: {seconds}초";
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (_startPrompt != null)
        {
            _startPrompt.SetActive(newState == GameState.WaitingFirstCookie);
        }
    }

    private void ShowResultPanel(GameOverReason reason, int finalScore)
    {
        if (_resultPanel == null)
        {
            return;
        }

        _resultPanel.SetActive(true);

        if (reason == GameOverReason.FloorCollision)
        {
            if (_resultMessageText != null)
            {
                _resultMessageText.text = "오레오가 무너졌습니다!";
            }

            if (_resultScoreText != null)
            {
                _resultScoreText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_resultMessageText != null)
            {
                _resultMessageText.text = "시간 종료!";
            }

            if (_resultScoreText != null)
            {
                _resultScoreText.gameObject.SetActive(true);
                _resultScoreText.text = $"최종 점수: {finalScore}";
            }
        }
    }

    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
