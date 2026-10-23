using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public InteractionMenuUI ui;

    public static bool InputLocked { get; private set; }

    readonly List<Interactable> inRange = new List<Interactable>();

    // The options currently shown on screen (includes the auto "Back" entry when inside a sub-menu).
    readonly List<InteractionOption> options = new List<InteractionOption>();

    // The raw options for the level currently shown, WITHOUT the auto "Back" entry. Used when pushing history.
    List<InteractionOption> currentLevelRaw;

    // Parent levels to return to, pushed each time a branch option is chosen, popped when "Back" is chosen.
    readonly Stack<List<InteractionOption>> menuHistory = new Stack<List<InteractionOption>>();

    // The auto-injected "Back" entry for the current level, or null at the root level. Used to identify it in SelectOption.
    InteractionOption backOption;

    bool menuOpen;
    int selectedIndex;

    // The interactable currently being prompted/interacted with (the most recently entered one in range).
    Interactable Current => inRange.Count > 0 ? inRange[inRange.Count - 1] : null;

    // Adds an interactable to range and refreshes the prompt. Called by Interactable.OnTriggerEnter2D.
    public void Register(Interactable interactable)
    {
        if (!inRange.Contains(interactable))
            inRange.Add(interactable);
        RefreshPrompt();
    }

    // Removes an interactable from range, closing the menu if it was the active one. Called by Interactable.OnTriggerExit2D.
    public void Unregister(Interactable interactable)
    {
        bool wasCurrent = Current == interactable;
        inRange.Remove(interactable);

        if (wasCurrent && menuOpen)
            CloseMenu();

        RefreshPrompt();
    }

    // Each frame: keeps the UI anchored to the current target, and reads E/W/S/Esc for opening, navigating, and closing the menu.
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        Interactable current = Current;
        if (current == null)
        {
            if (ui != null) ui.Hide();
            InputLocked = false;
            return;
        }

        if (ui != null)
            ui.SetAnchorPosition(current.transform.position + current.promptOffset);

        if (!menuOpen)
        {
            if (keyboard.eKey.wasPressedThisFrame)
                OpenMenu();
        }
        else
        {
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                CloseMenu();
            }
            else if (keyboard.wKey.wasPressedThisFrame)
            {
                MoveSelection(-1);
            }
            else if (keyboard.sKey.wasPressedThisFrame)
            {
                MoveSelection(1);
            }
            else if (keyboard.eKey.wasPressedThisFrame)
            {
                SelectOption(selectedIndex);
            }
        }
    }

    // Opens the option menu at the root level for the current target, locking player movement/combat input.
    public void OpenMenu()
    {
        Interactable current = Current;
        if (current == null) return;

        menuHistory.Clear();

        List<InteractionOption> rootOptions = new List<InteractionOption>();
        current.GetOptions(rootOptions);

        menuOpen = true;
        InputLocked = true;
        ShowLevel(rootOptions);
    }

    // Displays one level of options (root or sub-menu), auto-appending a "Back" entry if a parent level exists.
    void ShowLevel(List<InteractionOption> levelOptions)
    {
        currentLevelRaw = levelOptions;

        options.Clear();
        options.AddRange(levelOptions);

        backOption = null;
        if (menuHistory.Count > 0)
        {
            backOption = new InteractionOption("Back", GoBack);
            options.Add(backOption);
        }

        selectedIndex = 0;

        Interactable current = Current;
        if (ui != null && current != null)
            ui.ShowDialogue(current.displayName, current.GetStatusText(), options, selectedIndex);
    }

    // Returns to the parent level by popping the history stack, or does nothing if already at the root.
    void GoBack()
    {
        if (menuHistory.Count == 0) return;
        List<InteractionOption> parentOptions = menuHistory.Pop();
        ShowLevel(parentOptions);
    }

    // Moves the highlighted option by delta (wrapping around) and updates the UI highlight.
    void MoveSelection(int delta)
    {
        if (options.Count == 0) return;

        selectedIndex = (selectedIndex + delta + options.Count) % options.Count;

        if (ui != null)
            ui.SetSelectedIndex(selectedIndex, options.Count);
    }

    // Closes the option menu entirely, unlocks player input, clears navigation history, and restores the idle prompt.
    public void CloseMenu()
    {
        menuOpen = false;
        InputLocked = false;
        menuHistory.Clear();
        RefreshPrompt();
    }

    // Runs the chosen option: steps back a level for "Back", descends into a sub-menu for a branch option,
    // or runs the action and closes the whole menu for a regular leaf option.
    public void SelectOption(int index)
    {
        if (index < 0 || index >= options.Count) return;

        InteractionOption chosen = options[index];

        if (chosen == backOption)
        {
            chosen.execute?.Invoke(); // runs GoBack(), which shows the parent level and keeps the menu open
            return;
        }

        if (chosen.openSubMenu != null)
        {
            menuHistory.Push(currentLevelRaw);
            List<InteractionOption> subOptions = chosen.openSubMenu();
            ShowLevel(subOptions);
            return;
        }

        chosen.execute?.Invoke();
        CloseMenu();
    }

    // Shows the idle "[E] Interact" prompt for the current target, or hides the UI if nothing is in range.
    void RefreshPrompt()
    {
        if (ui == null) return;

        Interactable current = Current;
        if (current == null)
        {
            ui.Hide();
        }
        else if (!menuOpen)
        {
            ui.ShowPrompt(current.displayName, current.GetStatusText());
        }
    }
}
