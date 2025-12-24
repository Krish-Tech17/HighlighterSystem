using UnityEngine;

public class ExampleCaller : MonoBehaviour
{
    public GameObject target;

    public void Start()
    {
        // Example - auto highlight for default duration
       // Highlighter.Instance.Highlight(target);

        // Or highlight for 5 seconds:
        // Highlighter.Instance.Highlight(target, 5f);
    }

    public void ButtonClicked()
    {
        Highlighter.Instance.Highlight(target);
       // Highlighter.Instance.Highlight(target,5f);

    }

    public void ButtonClickedWithTime()
    {
        Highlighter.Instance.Highlight(target,5f);
        // Highlighter.Instance.Highlight(target,5f);

    }
}
