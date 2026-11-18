using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 13f;
    public float fallLimit = -9f;
    public Sprite idle, jump;
    public Sprite[] run;
    public AudioClip jumpClip, stompClip, deathClip;
    public GameManager game;

    Rigidbody2D rb;
    SpriteRenderer sr;
    AudioSource sfx;
    ContactFilter2D groundFilter;
    bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sfx = GetComponent<AudioSource>();
        groundFilter.SetNormalAngle(45f, 135f);
    }

    void Update()
    {
        if (dead || Time.timeScale == 0f) return;

        var kb = Keyboard.current;
        float x = 0f;
        if (kb.leftArrowKey.isPressed || kb.aKey.isPressed) x -= 1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) x += 1f;

        var v = new Vector2(x * speed, rb.linearVelocity.y);
        bool grounded = rb.IsTouching(groundFilter) && v.y <= 0.01f;
        if (grounded && kb.spaceKey.wasPressedThisFrame)
        {
            v.y = jumpForce;
            grounded = false;
            sfx.PlayOneShot(jumpClip);
        }
        if (kb.spaceKey.wasReleasedThisFrame && v.y > 0f) v.y *= 0.5f;

        float left = Camera.main.ViewportToWorldPoint(Vector3.zero).x + 0.5f;
        if (rb.position.x < left)
        {
            rb.position = new Vector2(left, rb.position.y);
            v.x = Mathf.Max(0f, v.x);
        }
        rb.linearVelocity = v;

        if (x != 0f) sr.flipX = x < 0f;
        sr.sprite = !grounded ? jump : x != 0f ? run[(int)(Time.time * 10f) % run.Length] : idle;

        if (transform.position.y < fallLimit) Die();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        var enemy = c.collider.GetComponent<Enemy>();
        if (enemy == null || dead) return;

        if (c.GetContact(0).normal.y > 0.5f)
        {
            enemy.Squash();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.7f);
            sfx.PlayOneShot(stompClip);
        }
        else Die();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard")) Die();
    }

    void Die()
    {
        if (dead) return;
        dead = true;
        sfx.PlayOneShot(deathClip);
        game.GameOver();
    }
}
