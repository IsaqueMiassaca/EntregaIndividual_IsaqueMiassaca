using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float minX, maxX;
    public Sprite[] walk;
    public Sprite squashed;

    Rigidbody2D rb;
    SpriteRenderer sr;
    int dir = -1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        var p = rb.position + Vector2.right * (dir * speed * Time.fixedDeltaTime);
        if (p.x < minX || p.x > maxX) dir = -dir;
        rb.MovePosition(p);
        sr.sprite = walk[(int)(Time.time * 4f) % walk.Length];
    }

    public void Squash()
    {
        enabled = false;
        GetComponent<Collider2D>().enabled = false;
        sr.sprite = squashed;
        Destroy(gameObject, 0.5f);
    }
}
