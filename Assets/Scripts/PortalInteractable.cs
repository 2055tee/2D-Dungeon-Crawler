using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInteractable : Interactable
{
    public SpriteRenderer coreRenderer;
    public Color brokenColor = new Color(0.25f, 0.2f, 0.3f);
    public Color fixedColor = new Color(0.3f, 0.9f, 1f);
    public float fixDuration = 0.6f;

    bool isFixed;
    public bool IsFixed => isFixed;

    // How far along Z the Island sits from the Mainland. Same infinite XY plane/world, not a separate scene yet -
    // the Mainland is meant to be an infinite plane, so layers are separated by Z instead of by moving in XY.
    public float islandZOffset = 50f;

    // Sets the display name and tints the core to its "broken" color at startup.
    void Awake()
    {
        displayName = "Portal";
        if (coreRenderer != null && !isFixed)
            coreRenderer.color = brokenColor;
    }

    // Reports whether the portal is currently broken or fixed.
    public override string GetStatusText()
    {
        return isFixed ? "Fixed" : "Broken";
    }

    // Offers the "Fix Portal" option only while the portal is still broken.
    public override void GetOptions(List<InteractionOption> options)
    {
        if (!isFixed)
            options.Add(new InteractionOption("Fix Portal", Fix));
        if (isFixed)
            options.Add(new InteractionOption("Teleport", Teleport));

        options.Add(new InteractionOption("Leave", Leave));

        // Future: a submenu would look like:
        // options.Add(new InteractionOption("Investigate", () => new List<InteractionOption> { ... }));
        // "Back" is now added automatically by PlayerInteractor whenever a sub-menu is open - no need to add it here.
    }

    // Marks the portal fixed, logs the placeholder confirmation, and starts the color-fade coroutine.
    public void Fix()
    {
        if (isFixed) return;
        isFixed = true;

        Debug.Log("Portal fixed! (placeholder: no cost, no Island transition yet)");
        StartCoroutine(FixRoutine());
    }

    public void Leave()
    {
        Debug.Log("nothing Happened");
    }

    // Moves the player to the Island's spawn point and switches them onto the Island physics layer. Placeholder:
    // same world/scene, just offset in Z - the layer switch is what actually keeps Mainland and Island objects
    // from colliding/triggering each other despite sharing the same XY (Physics2D ignores Z entirely).
    public void Teleport()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null) return;

        // Lands 4 units south of the Island's return portal (same offset used for the Mainland spawn vs this
        // portal), so the player doesn't arrive standing inside the return portal's own solid collider.
        player.transform.position = new Vector3(0f, -4f, islandZOffset);
        player.gameObject.layer = WorldLayers.Island;
        Debug.Log("Teleported to the Island (placeholder: same world, offset Z-layer, no real scene boundary yet).");
    }

    // Fades the portal core's color from broken to fixed over fixDuration seconds.
    IEnumerator FixRoutine()
    {
        float elapsed = 0f;
        Color startColor = coreRenderer != null ? coreRenderer.color : brokenColor;

        while (elapsed < fixDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fixDuration);
            if (coreRenderer != null)
                coreRenderer.color = Color.Lerp(startColor, fixedColor, t);
            yield return null;
        }

        if (coreRenderer != null)
            coreRenderer.color = fixedColor;
    }
}
