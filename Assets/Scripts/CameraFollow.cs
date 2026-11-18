using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float offsetX = 4f;

    void LateUpdate()
    {
        var p = transform.position;
        p.x = Mathf.Max(p.x, target.position.x + offsetX);
        transform.position = p;
    }
}
