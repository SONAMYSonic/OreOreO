using UnityEngine;

public class CookieBase : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        var cream = collision.gameObject.GetComponent<Cream>();
        if (cream == null)
        {
            return;
        }

        GameManager.Instance.EndGame(GameOverReason.CookieBaseCollision);
    }
}
