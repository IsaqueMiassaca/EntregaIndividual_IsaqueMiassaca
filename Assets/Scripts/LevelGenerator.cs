using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform player;
    public Sprite groundTop, groundBody, spike;
    public Sprite[] decorations;
    public Enemy enemyPrefab;
    public float aheadDistance = 30f, behindDistance = 25f;

    readonly Queue<(GameObject go, float end)> segments = new();
    float nextX = -12f;
    float lastY = -3f;

    void Update()
    {
        while (nextX < player.position.x + aheadDistance) Spawn();
        while (segments.Count > 0 && segments.Peek().end < player.position.x - behindDistance)
            Destroy(segments.Dequeue().go);
    }

    void Spawn()
    {
        bool first = segments.Count == 0;
        int width = first ? 24 : Random.Range(4, 11);
        float x = nextX + (first ? 0 : Random.Range(2, 5));
        float y = first ? lastY : Mathf.Clamp(lastY + Random.Range(-2, 2), -5f, -1f);

        var root = new GameObject("Segment").transform;
        root.position = new Vector3(x, y);
        Tile(root, groundTop, new Vector2(width, 1f), new Vector2(width / 2f, -0.5f), 0);
        Tile(root, groundBody, new Vector2(width, 8f), new Vector2(width / 2f, -5f), 0);
        var col = root.gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(width, 9f);
        col.offset = new Vector2(width / 2f, -4.5f);

        int roll = first ? -1 : Random.Range(0, 3);
        if (roll == 0 && width >= 5)
        {
            int n = Random.Range(1, 3);
            var s = Tile(root, spike, new Vector2(n, 1f), new Vector2(Random.Range(2, width - 1 - n) + n / 2f, 0.5f), 1);
            s.tag = "Hazard";
            var trigger = s.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(n - 0.3f, 0.4f);
            trigger.offset = new Vector2(0f, -0.3f);
        }
        else if (roll == 1 && width >= 6)
        {
            var e = Instantiate(enemyPrefab, new Vector3(x + width / 2f, y), Quaternion.identity, root);
            e.minX = x + 0.7f;
            e.maxX = x + width - 0.7f;
        }

        if (Random.value < 0.6f)
        {
            var d = new GameObject("Decoration").AddComponent<SpriteRenderer>();
            d.transform.SetParent(root, false);
            d.transform.localPosition = new Vector3(Random.Range(0.5f, width - 0.5f), 0f);
            d.sprite = decorations[Random.Range(0, decorations.Length)];
            d.sortingOrder = -1;
        }

        segments.Enqueue((root.gameObject, x + width));
        nextX = x + width;
        lastY = y;
    }

    static GameObject Tile(Transform parent, Sprite sprite, Vector2 size, Vector2 pos, int order)
    {
        var sr = new GameObject(sprite.name).AddComponent<SpriteRenderer>();
        sr.transform.SetParent(parent, false);
        sr.transform.localPosition = pos;
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = size;
        sr.sortingOrder = order;
        return sr.gameObject;
    }
}
