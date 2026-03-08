using UnityEngine;

public class DungeonGen1 : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    /*
    ==========================================
    ROOM SETTINGS
    ==========================================
    center
        The world position where the dungeon
        generation starts.

    roomSize
        The size of each room square.

    yLevel
        The vertical height where we draw the
        dungeon gizmos.

    roomNodeRadius
        Size of the yellow sphere that marks
        the center of each room.
    */
    [Header("Room Settings")]
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private float roomSize = 2f;
    [SerializeField] private float yLevel = 0f;
    [SerializeField] private float roomNodeRadius = 0.15f;

    [Header("Colors")]
    [SerializeField] private Color roomColor = Color.green;
    [SerializeField] private Color nodeColor = Color.yellow;
    [SerializeField] private Color edgeColor = Color.red;

    private bool dungeonVisible = false;

    private Vector3 roomA;
    private Vector3 roomB;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void OnDrawGizmos()
    {
        // Only draw when the game is running
        if (!Application.isPlaying || !dungeonVisible)
            return;

        // Draw both rooms
        DrawRoom(roomA);
        DrawRoom(roomB);

        // Draw the connection between them
        DrawConnection(roomA, roomB);
    }

    public void GenerateDungeon1()
    {
        // First room is placed exactly at the center
        roomA = center;

        // Second room is placed one room size to the right
        roomB = center + new Vector3(roomSize, 0f, 0f);

        // Enable drawing
        dungeonVisible = true;

        // Move camera so the dungeon is clearly visible
        SetCameraForDungeonView();
    }

    public void ClearDungeon1()
    {
        dungeonVisible = false;
    }

    /*
    ==========================================
    DRAW ROOM
    ==========================================
    Draws a square representing a room.
    Also draws a yellow node in the center.
    */
    private void DrawRoom(Vector3 roomCenter)
    {
        // Set color for the room outline
        Gizmos.color = roomColor;

        // Half size helps calculate corners
        float half = roomSize * 0.5f;

        // Calculate the four corners of the square
        Vector3 a = roomCenter + new Vector3(-half, yLevel, -half);
        Vector3 b = roomCenter + new Vector3(half, yLevel, -half);
        Vector3 c = roomCenter + new Vector3(half, yLevel, half);
        Vector3 d = roomCenter + new Vector3(-half, yLevel, half);

        // Draw the square edges
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);

        // Draw the center node of the room
        Gizmos.color = nodeColor;
        Gizmos.DrawSphere(roomCenter + new Vector3(0f, yLevel, 0f), roomNodeRadius);
    }


    /*
    ==========================================
    DRAW CONNECTION
    ==========================================
    Draws a red line between two room centers.

    This represents the dungeon graph edge
    connecting the two rooms.
    */
    private void DrawConnection(Vector3 from, Vector3 to)
    {
        Gizmos.color = edgeColor;
        Gizmos.DrawLine(from + new Vector3(0f, yLevel, 0f),
                        to + new Vector3(0f, yLevel, 0f));
    }
    private void SetCameraForDungeonView()
    {
        if (cam == null) return;

        // Position camera above the dungeon
        cam.transform.position = center + new Vector3(roomSize * 0.5f, 8f, 0f);

        // Rotate camera to look straight down
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}