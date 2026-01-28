using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Block : MonoBehaviour
{
    protected Rigidbody2D Rigidbody;
    protected Collider2D Collider;

    public float HalfWidth => Collider != null ? Collider.bounds.extents.x : 0.5f;
    public float TopY => Collider != null ? Collider.bounds.max.y : transform.position.y + 0.5f;

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }
}
