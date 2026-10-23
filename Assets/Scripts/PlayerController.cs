using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    Rigidbody2D rb;

    // Caches the Rigidbody2D, sets up the Mainland/Island physics-layer split, and starts the player on the Mainland layer.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        WorldLayers.EnsureIgnoreSetup();
        gameObject.layer = WorldLayers.Mainland;
    }

    // Reads WASD each physics step and moves the player via Rigidbody2D velocity.
    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || PlayerInteractor.InputLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;

        rb.linearVelocity = input.normalized * moveSpeed;
    }
}
