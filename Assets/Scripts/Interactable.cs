using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionOption
{
    public readonly string label;
    public readonly Action execute;
    public readonly Func<List<InteractionOption>> openSubMenu;

    // Leaf option: runs an action immediately when chosen (e.g. "Fix Portal"). Closes the whole menu afterward.
    public InteractionOption(string label, Action execute)
    {
        this.label = label;
        this.execute = execute;
        this.openSubMenu = null;
    }

    // Branch option: opens a nested list of options when chosen (e.g. "Investigate" -> its own sub-choices),
    // instead of performing an action directly. The menu stays open, and a "Back" entry is added automatically.
    public InteractionOption(string label, Func<List<InteractionOption>> openSubMenu)
    {
        this.label = label;
        this.execute = null;
        this.openSubMenu = openSubMenu;
    }
}

public abstract class Interactable : MonoBehaviour
{
    public string displayName = "Object";
    public Vector3 promptOffset = new Vector3(0f, 1.2f, 0f);

    // Returns a short status string shown in the prompt/dialogue (e.g. "Broken"/"Fixed").
    public abstract string GetStatusText();

    // Fills the given list with the options currently available on this interactable.
    public abstract void GetOptions(List<InteractionOption> options);

    // Resolves the PlayerInteractor that owns a colliding body, ignoring other trigger colliders (e.g. the sword hitbox).
    static PlayerInteractor FindInteractor(Collider2D other)
    {
        if (other.isTrigger) return null;
        if (other.attachedRigidbody == null) return null;
        return other.attachedRigidbody.GetComponent<PlayerInteractor>();
    }

    // Registers this interactable with the player when they enter its trigger zone.
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInteractor interactor = FindInteractor(other);
        if (interactor != null) interactor.Register(this);
    }

    // Unregisters this interactable from the player when they leave its trigger zone.
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerInteractor interactor = FindInteractor(other);
        if (interactor != null) interactor.Unregister(this);
    }
}
