using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float _gameTime = 60f;

    private GameState _currentState = GameState.WaitingFirstCookie;
    private float _remainingTime;
    private bool _isFirstCookiePlaced;

    public GameState CurrentState => _currentState;
    public bool IsPlaying => _currentState == GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (_currentState != GameState.Playing)
        {
            return;
        }

        UpdateTimer();
    }

    private void InitializeGame()
    {
        _remainingTime = _gameTime;
        _isFirstCookiePlaced = false;
        ChangeState(GameState.WaitingFirstCookie);
        GameEvents.RaiseTimerChanged(_remainingTime);
    }

    private void UpdateTimer()
    {
        _remainingTime -= Time.deltaTime;
        GameEvents.RaiseTimerChanged(_remainingTime);

        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            EndGame(GameOverReason.TimeUp);
        }
    }

    private void ChangeState(GameState newState)
    {
        _currentState = newState;
        GameEvents.RaiseGameStateChanged(newState);
    }

    public void OnFirstCookiePlaced()
    {
        if (_currentState != GameState.WaitingFirstCookie)
        {
            return;
        }

        _isFirstCookiePlaced = true;
        ChangeState(GameState.Playing);
    }

    public void EndGame(GameOverReason reason)
    {
        if (_currentState == GameState.GameOver)
        {
            return;
        }

        ChangeState(GameState.GameOver);

        int finalScore = reason == GameOverReason.FloorCollision ? -1 : ScoreManager.Instance.CurrentScore;
        GameEvents.RaiseGameOver(reason, finalScore);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
