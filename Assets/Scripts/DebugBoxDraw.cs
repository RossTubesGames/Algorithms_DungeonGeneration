using UnityEngine;

public class DebugBoxDraw : MonoBehaviour
{
    [Header("Box Settings")]

    // Center position of the box
    [SerializeField]
    private Vector3 center = Vector3.zero;

    // Width of the box (X axis)
    [SerializeField]
    private float width = 5f;

    // Length of the box (Z axis)
    [SerializeField]
    private float length = 5f;

    // Height offset
    [SerializeField]
    private float height = 0f;

    // Actual box height
    [SerializeField]
    private float boxHeight = 5f;

    // This controls whether the box should be drawn
    private bool drawBox = false;

    private void OnDrawGizmos()
    {
        // Only draw when playing AND when drawBox is true
        if (!Application.isPlaying || !drawBox)
            return;

        Gizmos.color = Color.green;

        float halfWidth = width / 2f;
        float halfLength = length / 2f;

        Vector3 cornerA = center + new Vector3(-halfWidth, height, -halfLength);
        Vector3 cornerB = center + new Vector3(halfWidth, height, -halfLength);
        Vector3 cornerC = center + new Vector3(halfWidth, height, halfLength);
        Vector3 cornerD = center + new Vector3(-halfWidth, height, halfLength);

        Vector3 cornerE = center + new Vector3(-halfWidth, height + boxHeight, -halfLength);
        Vector3 cornerF = center + new Vector3(halfWidth, height + boxHeight, -halfLength);
        Vector3 cornerG = center + new Vector3(halfWidth, height + boxHeight, halfLength);
        Vector3 cornerH = center + new Vector3(-halfWidth, height + boxHeight, halfLength);

        // Bottom square
        Gizmos.DrawLine(cornerA, cornerB);
        Gizmos.DrawLine(cornerB, cornerC);
        Gizmos.DrawLine(cornerC, cornerD);
        Gizmos.DrawLine(cornerD, cornerA);

        // Top square
        Gizmos.DrawLine(cornerE, cornerF);
        Gizmos.DrawLine(cornerF, cornerG);
        Gizmos.DrawLine(cornerG, cornerH);
        Gizmos.DrawLine(cornerH, cornerE);

        // Vertical lines
        Gizmos.DrawLine(cornerA, cornerE);
        Gizmos.DrawLine(cornerB, cornerF);
        Gizmos.DrawLine(cornerC, cornerG);
        Gizmos.DrawLine(cornerD, cornerH);
    }

    // Draw Box button
    public void DrawBox()
    {
        drawBox = true;
    }

    // Clear Box button
    public void ClearBox()
    {
        drawBox = false;
    }
}