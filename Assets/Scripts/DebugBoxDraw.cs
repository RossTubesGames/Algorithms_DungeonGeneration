using UnityEngine;

public class DebugBoxDraw : MonoBehaviour
{
    [Header("Box Settings")]

    // The center position of the box
    [SerializeField]
    private Vector3 center = Vector3.zero;

    // Width of the box (X direction)
    [SerializeField]
    private float width = 5f;

    // length of the box (Z direction)
    [SerializeField]
    private float length = 5f;

    // height of the box (Y direction)
    [SerializeField]
    private float height = 0f;

    // Called by Unity every frame for debug drawing
    private void OnDrawGizmos()
    {
        // Only draw when the game is running
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        // Half sizes help calculate the corners relative to the center
        float halfWidth = width / 2f;
        float halfHeight = length / 2f;

        // Calculate the four corners of the box
        Vector3 cornerA = center + new Vector3(-halfWidth, height, -halfHeight);
        Vector3 cornerB = center + new Vector3(halfWidth, height, -halfHeight);
        Vector3 cornerC = center + new Vector3(halfWidth, height, halfHeight);
        Vector3 cornerD = center + new Vector3(-halfWidth, height, halfHeight);
        Vector3 cornerE = center + new Vector3(-halfWidth, height + 5f, -halfHeight);
        Vector3 cornerF = center + new Vector3(halfWidth, height + 5f, -halfHeight);
        Vector3 cornerG = center + new Vector3(halfWidth, height + 5f, halfHeight);
        Vector3 cornerH = center + new Vector3(-halfWidth, height + 5f, halfHeight);

        // Draw the four lines that make the box
        Gizmos.DrawLine(cornerA, cornerB);
        Gizmos.DrawLine(cornerB, cornerC);
        Gizmos.DrawLine(cornerC, cornerD);
        Gizmos.DrawLine(cornerD, cornerA);

        Gizmos.DrawLine(cornerE, cornerF);
        Gizmos.DrawLine(cornerF, cornerG);
        Gizmos.DrawLine(cornerG, cornerH);
        Gizmos.DrawLine(cornerH, cornerE);

        Gizmos.DrawLine(cornerA, cornerE);
        Gizmos.DrawLine(cornerB, cornerF);
        Gizmos.DrawLine(cornerC, cornerG);
        Gizmos.DrawLine(cornerD, cornerH);

        // Draw spheres so we can see the corners clearly
        Gizmos.DrawSphere(cornerA, 0.1f);
        Gizmos.DrawSphere(cornerB, 0.1f);
        Gizmos.DrawSphere(cornerC, 0.1f);
        Gizmos.DrawSphere(cornerD, 0.1f);

        Gizmos.DrawSphere(cornerE, 0.1f);
        Gizmos.DrawSphere(cornerF, 0.1f);
        Gizmos.DrawSphere(cornerG, 0.1f);
        Gizmos.DrawSphere(cornerH, 0.1f);
    }
}
