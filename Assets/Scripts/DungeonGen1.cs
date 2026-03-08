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

    // Controls whether the dungeon should currently be visible
    private bool dungeonVisible = false;

    // Store the center positions of the rooms
    private Vector3 roomA;
    private Vector3 roomB;
    private Vector3 roomC;

    private void Awake()
    {
        // If no camera was assigned manually, use the Main Camera
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void OnDrawGizmos()
    {
        // Only draw while playing and only if the dungeon is enabled
        if (!Application.isPlaying || !dungeonVisible)
            return;

        // Draw all three rooms
        DrawRoom(roomA);
        DrawRoom(roomB);
        DrawRoom(roomC);

        // Draw connection from room A to room B
        DrawConnection(roomA, roomB);

        // Draw connection from room A to room C
        DrawConnection(roomA, roomC);
    }

    public void GenerateDungeon1()
    {
        // Room A is always the starting room in the center
        roomA = center;

        // Room B is always placed to the right of room A
        roomB = center + new Vector3(roomSize, 0f, 0f);

        // Random.Range(0, 2) gives either 0 or 1
        // This creates a 50/50 chance
        int randomChoice = Random.Range(0, 2);

        // If randomChoice is 0, place room C above room A
        if (randomChoice == 0)
        {
            roomC = center + new Vector3(0f, 0f, roomSize);
        }
        // Otherwise place room C below room A
        else
        {
            roomC = center + new Vector3(0f, 0f, -roomSize);
        }

        // Enable drawing
        dungeonVisible = true;

        // Move the camera so the full layout is visible
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

        // Half of the room size is used to calculate corners from the center
        float half = roomSize * 0.5f;

        // Calculate the 4 corners of the room square
        Vector3 a = roomCenter + new Vector3(-half, yLevel, -half);
        Vector3 b = roomCenter + new Vector3(half, yLevel, -half);
        Vector3 c = roomCenter + new Vector3(half, yLevel, half);
        Vector3 d = roomCenter + new Vector3(-half, yLevel, half);

        // Draw the square room
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);

        // Draw a yellow node in the center of the room
        Gizmos.color = nodeColor;
        Gizmos.DrawSphere(roomCenter + new Vector3(0f, yLevel, 0f), roomNodeRadius);
    }

    /*
    ==========================================
    DRAW CONNECTION
    ==========================================
    Draws a red line between two room centers.

    This represents a graph connection
    between the two rooms.
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

        // Slightly higher camera so 3 rooms fit comfortably in view
        cam.transform.position = center + new Vector3(0f, 10f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}