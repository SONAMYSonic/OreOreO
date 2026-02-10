using System;

public enum GameState
{
    WaitingFirstCookie,
    Playing,
    GameOver
}

public enum GameOverReason
{
    None,
    FloorCollision,
    CookieBaseCollision,
    TimeUp
}

public static class GameEvents
{
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnTimerChanged;
    public static event Action<GameOverReason, int> OnGameOver;
    public static event Action OnCreamStacked;
    public static event Action<int> OnOreoCompleted;
    public static event Action OnCookiePlaced;
    public static event Action OnCreamPlaced;

    public static void RaiseGameStateChanged(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    public static void RaiseScoreChanged(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }

    public static void RaiseTimerChanged(float remainingTime)
    {
        OnTimerChanged?.Invoke(remainingTime);
    }

    public static void RaiseGameOver(GameOverReason reason, int finalScore)
    {
        OnGameOver?.Invoke(reason, finalScore);
    }

    public static void RaiseCreamStacked()
    {
        OnCreamStacked?.Invoke();
    }

    public static void RaiseOreoCompleted(int creamCount)
    {
        OnOreoCompleted?.Invoke(creamCount);
    }

    public static void RaiseCookiePlaced()
    {
        OnCookiePlaced?.Invoke();
    }

    public static void RaiseCreamPlaced()
    {
        OnCreamPlaced?.Invoke();
    }
}
