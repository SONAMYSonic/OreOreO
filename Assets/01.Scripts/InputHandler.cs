using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _creamCooldown = 0.25f;

    private float _lastCreamTime;
    private bool _canInput = true;

    public event Action OnCookieKeyPressed;
    public event Action OnCreamKeyPressed;

    private void OnEnable()
    {
        GameEvents.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (!_canInput)
        {
            return;
        }

        CheckCookieInput();
        CheckCreamInput();
    }

    private void CheckCookieInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnCookieKeyPressed?.Invoke();
        }
    }

    private void CheckCreamInput()
    {
        if (!Input.GetKeyDown(KeyCode.X))
        {
            return;
        }

        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (Time.time - _lastCreamTime < _creamCooldown)
        {
            return;
        }

        _lastCreamTime = Time.time;
        OnCreamKeyPressed?.Invoke();
    }

    private void HandleGameStateChanged(GameState newState)
    {
        _canInput = newState != GameState.GameOver;
    }
}
