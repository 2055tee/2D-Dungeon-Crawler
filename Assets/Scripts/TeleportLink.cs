using System.Collections.Generic;
using UnityEngine;

// A reusable one-way teleport point: walking up offers a single option that moves the player
// to a configured destination. No "fix" step - unlike PortalInteractable, this always works.
public class TeleportLink : Interactable
{
    public Vector3 destination;
    public string optionLabel = "Teleport";
    public int destinationLayer = WorldLayers.Mainland;

    // Gives this link a default display name if one wasn't set in the Inspector.
    void Awake()
    {
        if (string.IsNullOrEmpty(displayName) || displayName == "Object")
            displayName = "Portal";
    }

    // Always reports "Ready" since this link has no broken/fixed state.
    public override string GetStatusText()
    {
        return "Ready";
    }

    // Always offers the single teleport option.
    public override void GetOptions(List<InteractionOption> options)
    {
        options.Add(new InteractionOption(optionLabel, Teleport));
    }

    // Moves the player directly to this link's destination and switches them onto the matching physics layer.
    public void Teleport()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null) return;

        player.transform.position = destination;
        player.gameObject.layer = destinationLayer;
        Debug.Log($"Teleported to {destination}.");
    }
}
