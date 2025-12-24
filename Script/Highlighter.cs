using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Highlighter : MonoBehaviour
{
    public static Highlighter Instance;

    [Header("Global Highlight Settings")]
    public Color defaultColor = Color.yellow;
    public float defaultWidth = 4f;
    public bool defaultBlink = true;

    [Header("Optional Auto Stop")]
    public bool useAutoStop = false;   // if ON, default duration below is used
    public float defaultAutoStopDuration = 3f;

    private Dictionary<GameObject, Coroutine> activeHighlights = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // -------------------------------------------------
    // PUBLIC API
    // -------------------------------------------------

    /// <summary>
    /// Highlight object indefinitely (until ClearHighlight is called)
    /// </summary>
    public void Highlight(GameObject target)
    {
        Debug.Log("useAutoStop = " + useAutoStop);

        if (useAutoStop)
            Highlight(target, defaultAutoStopDuration);
        else
            ApplyHighlight(target, null);
    }

    /// <summary>
    /// Highlight object for X seconds then auto stop.
    /// </summary>
    public void Highlight(GameObject target, float duration)
    {
        ApplyHighlight(target, duration);
    }

    /// <summary>
    /// Stop highlight manually anytime.
    /// </summary>
    public void ClearHighlight(GameObject target)
    {
        if (target == null) return;

        if (activeHighlights.ContainsKey(target))
        {
            StopCoroutine(activeHighlights[target]);
            activeHighlights.Remove(target);
        }

        Outline outline = target.GetComponent<Outline>();
        if (outline)
        {
            outline.blink = false;
            outline.enabled = false;
        }
    }

    /// <summary>
    /// Clear all highlighted objects at once (optional).
    /// </summary>
    public void ClearAllHighlights()
    {
        foreach (var entry in activeHighlights)
        {
            Outline outline = entry.Key.GetComponent<Outline>();
            if (outline) outline.enabled = false;
        }

        activeHighlights.Clear();
    }


    // -------------------------------------------------
    // INTERNAL LOGIC
    // -------------------------------------------------

    private void ApplyHighlight(GameObject target, float? duration)
    {
        if (target == null) return;

        Outline outline = target.GetComponent<Outline>();
        if (!outline) outline = target.AddComponent<Outline>();

        outline.OutlineColor = defaultColor;
        outline.OutlineWidth = defaultWidth;
        outline.blink = defaultBlink;
        outline.enabled = true;

        // handle timer and prevent duplicates
        if (activeHighlights.ContainsKey(target))
        {
            StopCoroutine(activeHighlights[target]);
            activeHighlights.Remove(target);
        }

        if (duration.HasValue)
        {
            Coroutine routine = StartCoroutine(AutoRemove(outline, duration.Value));
            activeHighlights[target] = routine;
        }
    }

    private IEnumerator AutoRemove(Outline outline, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (outline != null)
        {
            outline.blink = false;
            outline.enabled = false;
        }

        activeHighlights.Remove(outline.gameObject);
    }
}
