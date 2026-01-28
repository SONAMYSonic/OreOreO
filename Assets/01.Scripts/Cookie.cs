using UnityEngine;

public class Cookie : Block
{
    private bool _isFirstCookie;
    private bool _isBase;
    private int _creamCountBelow;
    private bool _isSettled;
    private float _settleCheckDelay = 0.5f;
    private float _spawnTime;

    public bool IsBase => _isBase;

    protected override void Awake()
    {
        base.Awake();
        _spawnTime = Time.time;
    }

    public void Initialize(bool isFirstCookie, int creamCountBelow)
    {
        _isFirstCookie = isFirstCookie;
        _creamCountBelow = creamCountBelow;
    }

    public void SetAsBase()
    {
        _isBase = true;
        Rigidbody.bodyType = RigidbodyType2D.Static;
        _isSettled = true;
    }

    private void Update()
    {
        if (_isBase || _isSettled)
        {
            return;
        }

        if (Time.time - _spawnTime < _settleCheckDelay)
        {
            return;
        }

        CheckSettled();
    }

    private void CheckSettled()
    {
        if (Rigidbody.linearVelocity.magnitude > 0.1f)
        {
            return;
        }

        _isSettled = true;
        CompleteOreo();
    }

    private void CompleteOreo()
    {
        GameEvents.RaiseOreoCompleted(_creamCountBelow);

        var spawner = FindAnyObjectByType<BlockSpawner>();
        if (spawner != null)
        {
            spawner.RemoveOreoBlocks(this, _creamCountBelow);
        }
    }
}
