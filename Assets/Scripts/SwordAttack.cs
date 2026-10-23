using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{
    public Transform swordPivot;
    public Collider2D swordHitbox;

    public float restAngle;
    public float swingStartAngle;
    public float swingEndAngle;
    public float swingDuration;
    public float cooldown;

    bool isSwinging;
    float cooldownTimer;

    // Sets the sword to its resting angle and disables the hitbox at startup.
    void Awake()
    {
        if (swordPivot != null)
            swordPivot.localRotation = Quaternion.Euler(0f, 0f, restAngle);

        if (swordHitbox != null)
            swordHitbox.enabled = false;
    }

    // Ticks the swing cooldown and starts a swing when the player left-clicks (unless an interaction menu is open).
    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (PlayerInteractor.InputLocked) return;

        Mouse mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame && !isSwinging && cooldownTimer <= 0f)
        {
            StartCoroutine(SwingRoutine());
        }
    }

    // Animates the sword through its swing arc, enabling the hitbox only while swinging.
    IEnumerator SwingRoutine()
    {
        isSwinging = true;
        cooldownTimer = cooldown;

        if (swordHitbox != null)
            swordHitbox.enabled = true;

        float elapsed = 0f;
        Quaternion startRot = Quaternion.Euler(0f, 0f, swingStartAngle);
        Quaternion endRot = Quaternion.Euler(0f, 0f, swingEndAngle);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / swingDuration);
            if (swordPivot != null)
                swordPivot.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        if (swordHitbox != null)
            swordHitbox.enabled = false;

        if (swordPivot != null)
            swordPivot.localRotation = Quaternion.Euler(0f, 0f, restAngle);

        isSwinging = false;
    }
}
