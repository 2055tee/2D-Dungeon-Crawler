using UnityEngine;

// Central place for the Mainland/Island physics-layer split. Physics2D ignores the Z axis entirely, so two
// world "layers" placed at different Z but the same XY (like the Mainland and Island portals) would otherwise
// collide/trigger each other. These Unity physics layers stop that, regardless of Z or XY overlap.
public static class WorldLayers
{
    public const int Mainland = 8;
    public const int Island = 9;

    // Call once at startup so Mainland and Island objects never collide or trigger across layers.
    public static void EnsureIgnoreSetup()
    {
        Physics2D.IgnoreLayerCollision(Mainland, Island, true);
    }
}
