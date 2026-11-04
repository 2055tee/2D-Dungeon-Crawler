using UnityEngine;

// The 8 compass directions, in the same order as the sprite sheet's columns (col0=N ... col7=NW, clockwise).
public enum FacingDirection { N, NE, E, SE, S, SW, W, NW }

// Swaps the SpriteRenderer's sprite based on movement direction/speed - drives the 8-direction walk cycle without an Animator Controller.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    public Sprite[] directionFrames = new Sprite[24]; // index = direction * 3 + pose (pose 0/2 = step frames, pose 1 = neutral/idle)
    public float framesPerSecond = 8f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    FacingDirection currentDirection = FacingDirection.S;
    float animTimer;
    int frameIndex = 1;

    // Caches the Rigidbody2D and SpriteRenderer.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Each frame, picks the facing direction from velocity and advances the walk-cycle frame while moving.
    void Update()
    {
        Vector2 v = rb.linearVelocity;
        bool moving = v.sqrMagnitude > 0.01f;

        if (moving)
        {
            currentDirection = DirectionFromVector(v);
            animTimer += Time.deltaTime;
            if (animTimer >= 1f / framesPerSecond)
            {
                animTimer -= 1f / framesPerSecond;
                frameIndex = (frameIndex + 1) % 3;
            }
        }
        else
        {
            frameIndex = 1;
            animTimer = 0f;
        }

        int dirIndex = (int)currentDirection;
        Sprite frame = directionFrames[dirIndex * 3 + frameIndex];
        if (frame != null) sr.sprite = frame;
    }

    // Snaps a movement vector to the nearest of the 8 compass directions.
    static FacingDirection DirectionFromVector(Vector2 v)
    {
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg; // 0=E, 90=N, -90=S, +-180=W
        if (angle < 0f) angle += 360f;
        int sector = Mathf.RoundToInt(angle / 45f) % 8; // 0=E,1=NE,2=N,3=NW,4=W,5=SW,6=S,7=SE

        switch (sector)
        {
            case 0: return FacingDirection.E;
            case 1: return FacingDirection.NE;
            case 2: return FacingDirection.N;
            case 3: return FacingDirection.NW;
            case 4: return FacingDirection.W;
            case 5: return FacingDirection.SW;
            case 6: return FacingDirection.S;
            default: return FacingDirection.SE;
        }
    }
}
