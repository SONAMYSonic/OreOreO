using UnityEngine;

public class Cookie : Block
{
    private bool _isBase;
    private int _creamCountBelow;
    private bool _isOreoCompleted;

    public bool IsBase => _isBase;

    public void Initialize(bool isFirstCookie, int creamCountBelow)
    {
        _creamCountBelow = creamCountBelow;
    }

    public void SetAsBase()
    {
        _isBase = true;
        Rigidbody.bodyType = RigidbodyType2D.Static;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isBase || _isOreoCompleted)
        {
            return;
        }

        // 쿠키끼리 충돌하면 바로 오레오 완성
        var otherCookie = collision.gameObject.GetComponent<Cookie>();
        if (otherCookie != null && !otherCookie._isOreoCompleted)
        {
            _isOreoCompleted = true;
            otherCookie._isOreoCompleted = true;

            CompleteOreo(otherCookie);
            return;
        }

        // 크림 위에 착지한 경우
        var cream = collision.gameObject.GetComponent<Cream>();
        if (cream != null && _creamCountBelow > 0)
        {
            _isOreoCompleted = true;

            var spawner = FindAnyObjectByType<BlockSpawner>();
            if (spawner != null)
            {
                var bottomCookie = spawner.FindBottomCookieFor(this);
                if (bottomCookie != null)
                {
                    bottomCookie._isOreoCompleted = true;
                    CompleteOreo(bottomCookie);
                }
            }
        }
    }

    private void CompleteOreo(Cookie bottomCookie)
    {
        GameEvents.RaiseOreoCompleted(_creamCountBelow);

        var spawner = FindAnyObjectByType<BlockSpawner>();
        if (spawner == null)
        {
            return;
        }

        spawner.RemoveOreoBetween(this, bottomCookie);
    }
}
