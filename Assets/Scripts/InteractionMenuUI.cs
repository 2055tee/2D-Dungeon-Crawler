using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenuUI : MonoBehaviour
{
    [Header("Main dialogue Background")]
    public Image dialogueBackground;

    [Header("Idle prompt (before opening menu)")]
    public GameObject promptPanel;
    public Text promptLabel;

    [Header("Main dialogue window (while menu open)")]
    public GameObject dialoguePanel;
    public Text dialogueLabel;

    [Header("Option windows (pool, shown/hidden as needed)")]
    public GameObject[] optionPanels;
    public Image[] optionBackgrounds;
    public Text[] optionLabels;

    public Color optionNormalColor = new Color(0f, 0f, 0f, 0.6f);
    public Color optionSelectedColor = new Color(0.95f, 0.78f, 0.2f, 0.9f);

    public float firstOptionGap = 20f;
    public float optionSpacing = 56f;

    // Ensures all labels have a usable font, then hides everything at startup.
    void Awake()
    {
        EnsureFont(promptLabel);
        EnsureFont(dialogueLabel);
        if (optionLabels != null)
            foreach (Text t in optionLabels) EnsureFont(t);

        Hide();
    }

    // Assigns the built-in legacy font to a Text if it doesn't already have one.
    static void EnsureFont(Text t)
    {
        if (t != null && t.font == null)
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    // Shows the idle "walk up" prompt window and hides the dialogue/option windows.
    public void ShowPrompt(string title, string status)
    {
        HideMenu();
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptLabel != null) promptLabel.text = $"{title} ({status})\n[E] Interact";
    }

    // Shows the main dialogue window plus one small window per available option, highlighting the selected one.
    public void ShowDialogue(string title, string status, IReadOnlyList<InteractionOption> options, int selectedIndex)
    {
        if (promptPanel != null) promptPanel.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueLabel != null)
            dialogueLabel.text = $"{title} ({status})\n[W/S] Move  [E] Confirm  [Esc] Cancel";

        for (int i = 0; i < optionPanels.Length; i++)
        {
            bool active = i < options.Count;
            optionPanels[i].SetActive(active);
            if (!active) continue;

            optionLabels[i].text = options[i].label;
            optionBackgrounds[i].color = (i == selectedIndex) ? optionSelectedColor : optionNormalColor;
        }
    }

    // Repaints just the highlight colors across the visible option windows, without rebuilding their text.
    public void SetSelectedIndex(int index, int count)
    {
        for (int i = 0; i < optionBackgrounds.Length && i < count; i++)
            optionBackgrounds[i].color = (i == index) ? optionSelectedColor : optionNormalColor;
    }

    // Hides the main dialogue window and every option window.
    public void HideMenu()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (optionPanels != null)
            foreach (GameObject p in optionPanels)
                if (p != null) p.SetActive(false);
    }

    // Hides the idle prompt window as well as the dialogue/option windows.
    public void Hide()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
        HideMenu();
    }

    // Repositions the prompt/dialogue window at worldPosition (converted to screen space) and stacks the option windows beneath it.
    public void SetAnchorPosition(Vector3 worldPosition)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);

        if (promptPanel != null) promptPanel.transform.position = screenPos;
        if (dialoguePanel != null) dialoguePanel.transform.position = screenPos;

        for (int i = 0; i < optionPanels.Length; i++)
        {
            if (optionPanels[i] == null) continue;
            float yOffset = -(firstOptionGap + i * optionSpacing);
            optionPanels[i].transform.position = screenPos + new Vector3(0f, yOffset, 0f);
        }
    }
}
