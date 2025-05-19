using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.FPS.Game;

[System.Serializable]
public class Edge
{
    public GameObject fromNode;             // Serialize the GameObject references
    public GameObject toNode;               // Serialize the GameObject references

    [HideInInspector]
    public int fromNodeId;                  // ID of the prerequisite node
    [HideInInspector]
    public int toNodeId;                    // ID of the target node

    private UILineRenderer uiLineRenderer;  // Keep this for internal use

    public void InitialiseLineRenderer(GameObject edgeCollection)
    {
        if (fromNode == null || toNode == null)
        {
            Debug.LogError("Edge is missing a fromNode or toNode reference");
            return;
        }

        // Create the UILineRenderer component dynamically
        if (uiLineRenderer == null)
        {
            GameObject edgeObject = new("Edge")
            {
                layer = LayerMask.NameToLayer("UI")
            };

            edgeObject.transform.SetParent(edgeCollection.transform, false);

            // Add the UILineRenderer component
            uiLineRenderer = edgeObject.AddComponent<UILineRenderer>();

            // Set the colour and thickness for the line
            uiLineRenderer.color = Color.white;
            uiLineRenderer.thickness = 5f;

            fromNodeId = fromNode.GetComponent<Node>().id;
            toNodeId = toNode.GetComponent<Node>().id;


            RectTransform rectTransform = edgeObject.GetComponent<RectTransform>();
            SetStretchAnchor(rectTransform);

        }

        EventManager.AddListener<SkillUnlockedEvent>(ChangeEdgeColour);

        // Update the positions of the line
        UpdateLinePosition();
    }

    private void ChangeEdgeColour(SkillUnlockedEvent evt)
    {
        if (evt.node == toNode)
        {
            uiLineRenderer.color = Color.yellow;
        }
    }

    public void UpdateLinePosition()
    {
        if (uiLineRenderer != null && fromNode != null && toNode != null)
        {
            // Get the anchored positions in the canvas's local space
            Vector2 fromPosition = fromNode.GetComponent<RectTransform>().anchoredPosition;
            Vector2 toPosition = toNode.GetComponent<RectTransform>().anchoredPosition;

            // Assign points directly for UILineRenderer
            uiLineRenderer.points = new Vector2[] { fromPosition, toPosition };
        }
    }

    public void SetStretchAnchor(RectTransform rectTransform)
    {
        // Set the anchor to stretch in both directions (top-left and bottom-right corners)
        rectTransform.anchorMin = new Vector2(0, 0);    // Bottom-left corner
        rectTransform.anchorMax = new Vector2(1, 1);    // Top-right corner

        // Set the pivot to the center
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Set the position to zero or any other value if needed
        rectTransform.anchoredPosition = Vector3.zero;  // Center it or adjust as needed
        rectTransform.localPosition = Vector3.zero;

        // Optionally, you can also set the sizeDelta if you want to explicitly control the size after stretching
        rectTransform.sizeDelta = Vector2.zero;
    }
}
