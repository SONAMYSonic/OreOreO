using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _cookiePrefab;
    [SerializeField] private GameObject _creamPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnHeightOffset = 2f;
    [SerializeField] private float _firstCookieYPosition = -3f;

    [Header("Cream Spawn Range")]
    [SerializeField] private float _creamSpawnRange = 1f;

    private InputHandler _inputHandler;
    private readonly List<Block> _stackedBlocks = new();
    private Block _topBlock;
    private int _currentCreamCount;

    private void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
    }

    private void OnEnable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnCookieKeyPressed += HandleCookieKeyPressed;
            _inputHandler.OnCreamKeyPressed += HandleCreamKeyPressed;
        }
    }

    private void OnDisable()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnCookieKeyPressed -= HandleCookieKeyPressed;
            _inputHandler.OnCreamKeyPressed -= HandleCreamKeyPressed;
        }
    }

    private void HandleCookieKeyPressed()
    {
        SpawnCookie();
    }

    private void HandleCreamKeyPressed()
    {
        SpawnCream();
    }

    private void SpawnCookie()
    {
        Vector2 spawnPosition = CalculateCookieSpawnPosition();
        GameObject cookieObject = Instantiate(_cookiePrefab, spawnPosition, Quaternion.identity);
        var cookie = cookieObject.GetComponent<Cookie>();

        if (cookie == null)
        {
            return;
        }

        bool isFirstCookie = _topBlock == null;
        cookie.Initialize(isFirstCookie, _currentCreamCount);

        if (isFirstCookie)
        {
            cookie.SetAsBase();
            GameManager.Instance.OnFirstCookiePlaced();
        }

        RegisterBlock(cookie);
        _currentCreamCount = 0;
        GameEvents.RaiseCookiePlaced();
    }

    private void SpawnCream()
    {
        if (_topBlock == null)
        {
            return;
        }

        Vector2 spawnPosition = CalculateCreamSpawnPosition();
        GameObject creamObject = Instantiate(_creamPrefab, spawnPosition, Quaternion.identity);
        var cream = creamObject.GetComponent<Cream>();

        if (cream == null)
        {
            return;
        }

        RegisterBlock(cream);
        _currentCreamCount++;
        GameEvents.RaiseCreamPlaced();
        GameEvents.RaiseCreamStacked();
    }

    private Vector2 CalculateCookieSpawnPosition()
    {
        if (_topBlock == null)
        {
            return new Vector2(0f, _firstCookieYPosition);
        }

        float topY = _topBlock.TopY;
        float spawnY = topY + _spawnHeightOffset;
        float spawnX = _topBlock.transform.position.x;

        return new Vector2(spawnX, spawnY);
    }

    private Vector2 CalculateCreamSpawnPosition()
    {
        if (_topBlock == null)
        {
            return Vector2.zero;
        }

        float blockX = _topBlock.transform.position.x;
        float spawnX = Random.Range(blockX - _creamSpawnRange, blockX + _creamSpawnRange);
        float spawnY = _topBlock.TopY + _spawnHeightOffset;

        return new Vector2(spawnX, spawnY);
    }

    private void RegisterBlock(Block block)
    {
        _stackedBlocks.Add(block);
        _topBlock = block;
    }

    private void UpdateTopBlock()
    {
        if (_stackedBlocks.Count == 0)
        {
            _topBlock = null;
            return;
        }

        _topBlock = _stackedBlocks[^1];
    }

    public Cookie FindBottomCookieFor(Cookie topCookie)
    {
        int topIndex = _stackedBlocks.IndexOf(topCookie);
        if (topIndex < 0)
        {
            return null;
        }

        for (int i = topIndex - 1; i >= 0; i--)
        {
            if (_stackedBlocks[i] is Cookie cookie)
            {
                return cookie;
            }
        }

        return null;
    }

    public void RemoveOreoBetween(Cookie topCookie, Cookie bottomCookie)
    {
        int topIndex = _stackedBlocks.IndexOf(topCookie);
        int bottomIndex = _stackedBlocks.IndexOf(bottomCookie);

        if (topIndex < 0 || bottomIndex < 0)
        {
            return;
        }

        int startIndex = Mathf.Min(topIndex, bottomIndex);
        int endIndex = Mathf.Max(topIndex, bottomIndex);
        int count = endIndex - startIndex + 1;

        var blocksToRemove = _stackedBlocks.GetRange(startIndex, count);
        _stackedBlocks.RemoveRange(startIndex, count);

        foreach (var block in blocksToRemove)
        {
            if (block != null && block.gameObject != null)
            {
                Destroy(block.gameObject);
            }
        }

        UpdateTopBlock();
    }
}
