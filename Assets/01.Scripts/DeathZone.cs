using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        var cream = other.GetComponent<Cream>();
        if (cream == null)
        {
            return;
        }

        GameManager.Instance.EndGame(GameOverReason.FloorCollision);
    }
}
