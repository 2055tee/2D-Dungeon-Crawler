using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAim : MonoBehaviour
{
    public Camera cam;

    Rigidbody2D rb;

    // Caches the Rigidbody2D and defaults to the main camera if none was assigned.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cam == null) cam = Camera.main;
    }

    // Rotates the player each physics step to face the mouse cursor's world position.
    void FixedUpdate()
    {
        if (cam == null || Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        float distanceToPlane = -cam.transform.position.z;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceToPlane));

        Vector2 direction = (Vector2)mouseWorldPos - rb.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }
}
