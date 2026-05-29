using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    public int col;
    public int row;

    // The permanent base color of this square — set once, never changes
    private Color baseColor;
    private Renderer rend;
    private bool initialized = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Called by BoardManager after setting the square's color
    public void SetBaseColor(Color color)
    {
        baseColor = color;
        initialized = true;
        if (rend != null)
            rend.material.color = color;
    }

    public void Highlight(Color highlightColor)
    {
        if (rend != null)
            rend.material.color = highlightColor;
    }

    public void ClearHighlight()
    {
        if (rend != null && initialized)
            rend.material.color = baseColor;
    }
}