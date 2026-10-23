using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    // Logs whatever the sword hitbox touches, ignoring the player itself.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root == transform.root) return;
        Debug.Log($"Sword hit: {other.name}");
    }
}
