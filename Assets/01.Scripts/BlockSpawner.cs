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

        float halfWidth = _topBlock.HalfWidth;
        float blockX = _topBlock.transform.position.x;
        float minX = blockX - halfWidth;
        float maxX = blockX + halfWidth;

        float spawnX = Random.Range(minX, maxX);
        float spawnY = _topBlock.TopY + _spawnHeightOffset;

        return new Vector2(spawnX, spawnY);
    }

    private void RegisterBlock(Block block)
    {
        _stackedBlocks.Add(block);
        _topBlock = block;
    }

    public void RemoveBlock(Block block)
    {
        _stackedBlocks.Remove(block);
        UpdateTopBlock();
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

    public void RemoveOreoBlocks(Cookie topCookie, int creamCount)
    {
        int removeCount = creamCount + 1;
        int startIndex = _stackedBlocks.Count - removeCount;

        if (startIndex < 0)
        {
            startIndex = 0;
        }

        var blocksToRemove = _stackedBlocks.GetRange(startIndex, _stackedBlocks.Count - startIndex);
        _stackedBlocks.RemoveRange(startIndex, _stackedBlocks.Count - startIndex);

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
