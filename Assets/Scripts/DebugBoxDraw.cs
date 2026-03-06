using UnityEngine;

public class DebugBoxDraw : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Box Settings")]
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private float width = 5f;
    [SerializeField] private float length = 5f;
    [SerializeField] private float height = 0f;
    [SerializeField] private float boxHeight = 5f;

    [Header("Node Grid Settings")]
    [SerializeField] private int gridSize = 5;
    [SerializeField] private float nodeSpacing = 2f;
    [SerializeField] private float nodeRadius = 0.15f;

    private bool drawNodes = false;
    private bool drawBox = false;
    private bool drawConnectedNodes = false;

    private void Awake()
    {
        // If no camera is assigned in the Inspector, use the main camera
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (drawBox)
        {
            DrawBoxGizmo();
        }

        if (drawNodes)
        {
            DrawNodeGridGizmo();
        }

        if (drawConnectedNodes)
        {
            DrawConnectedNodeGridGizmo();
        }
    }

    private void DrawBoxGizmo()
    {
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
    }

    private void DrawNodeGridGizmo()
    {
        Gizmos.color = Color.yellow;

        float offset = (gridSize - 1) * nodeSpacing * 0.5f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float xPos = x * nodeSpacing - offset;
                float zPos = z * nodeSpacing - offset;

                Vector3 nodePosition = center + new Vector3(xPos, height, zPos);
                Gizmos.DrawSphere(nodePosition, nodeRadius);
            }
        }
    }
    private void DrawConnectedNodeGridGizmo()
    {
        Gizmos.color = Color.red;

        float offset = (gridSize - 1) * nodeSpacing * 0.5f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float xPos = x * nodeSpacing - offset;
                float zPos = z * nodeSpacing - offset;

                Vector3 nodePosition = center + new Vector3(xPos, height, zPos);

                // Draw the node
                Gizmos.DrawSphere(nodePosition, nodeRadius);

                // ----- CONNECT TO RIGHT NEIGHBOR -----
                if (x < gridSize - 1)
                {
                    float rightX = (x + 1) * nodeSpacing - offset;
                    float rightZ = zPos;

                    Vector3 rightNeighbor =
                        center + new Vector3(rightX, height, rightZ);

                    Gizmos.DrawLine(nodePosition, rightNeighbor);
                }

                // ----- CONNECT TO TOP NEIGHBOR -----
                if (z < gridSize - 1)
                {
                    float topX = xPos;
                    float topZ = (z + 1) * nodeSpacing - offset;

                    Vector3 topNeighbor =
                        center + new Vector3(topX, height, topZ);

                    Gizmos.DrawLine(nodePosition, topNeighbor);
                }
            }
        }
    }

    public void DrawBox()
    {
        drawBox = true;
        drawNodes = false;
        drawConnectedNodes = false;

        SetCameraForBoxView();
    }

    public void DrawNodes()
    {
        drawNodes = true;
        drawConnectedNodes = false;
        drawBox = false;

        SetCameraForNodeView();
    }

    public void DrawConnectedNodes()
    {
        drawConnectedNodes = true;
        drawBox = false;
        drawNodes = false;

        SetCameraForNodeView();
    }

    public void ClearBox()
    {
        drawBox = false;
        drawNodes = false;
        drawConnectedNodes = false;
    }

    private void SetCameraForBoxView()
    {
        if (cam == null) return;

        // Nice angled 3D view of the box
        cam.transform.position = center + new Vector3(0f, 8f, -12f);
        cam.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
    }

    private void SetCameraForNodeView()
    {
        if (cam == null) return;

        // Top-down view for the node grid
        cam.transform.position = center + new Vector3(0f, 12f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}