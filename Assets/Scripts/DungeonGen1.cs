using System.Collections.Generic;
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

    [Header("Dungeon Settings")]
    [SerializeField] private int maxRooms = 8;
    [SerializeField] private int maxPlacementAttempts = 100;

    [Header("Colors")]
    [SerializeField] private Color roomColor = Color.green;
    [SerializeField] private Color nodeColor = Color.yellow;
    [SerializeField] private Color edgeColor = Color.red;

    // Controls whether the dungeon should currently be visible
    private bool dungeonVisible = false;

    // All generated room centers are stored here
    private List<Vector3> rooms = new List<Vector3>();

    // All connections between rooms are stored here
    private List<RoomConnection> connections = new List<RoomConnection>();

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

        // Draw every room
        for (int i = 0; i < rooms.Count; i++)
        {
            DrawRoom(rooms[i]);
        }

        // Draw every connection
        for (int i = 0; i < connections.Count; i++)
        {
            DrawConnection(connections[i].from, connections[i].to);
        }

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


        // Clear old dungeon data first
        rooms.Clear();
        connections.Clear();

        // First room always starts at the center
        rooms.Add(center);

        int attempts = 0;

        // Keep trying until we have enough rooms
        while (rooms.Count < maxRooms && attempts < maxPlacementAttempts)
        {
            attempts++;

            // Pick a random existing room as the base room
            Vector3 baseRoom = rooms[Random.Range(0, rooms.Count)];

            // Pick a random direction
            Vector3 direction = GetRandomDirection();

            // Candidate position for a new room
            Vector3 newRoom = baseRoom + direction * roomSize;

            // Only place the room if it does not already exist
            if (!RoomExists(newRoom))
            {
                rooms.Add(newRoom);
                connections.Add(new RoomConnection(baseRoom, newRoom));
            }
        }

        dungeonVisible = true;
        SetCameraForDungeonView();

        /*
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
        SetCameraForDungeonView();*/
    }

    public void ClearDungeon1()
    {
        dungeonVisible = false;
        rooms.Clear();
        connections.Clear();
    }

    private Vector3 GetRandomDirection()
    {
        int randomChoice = Random.Range(0, 4);

        if (randomChoice == 0)
            return Vector3.right;

        if (randomChoice == 1)
            return Vector3.left;

        if (randomChoice == 2)
            return Vector3.forward;

        return Vector3.back;
    }

    private bool RoomExists(Vector3 candidateRoom)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            // Because positions are placed exactly on roomSize steps,
            // direct comparison is okay here
            if (rooms[i] == candidateRoom)
            {
                return true;
            }
        }

        return false;
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
    private void DrawConnection(Vector3 point1, Vector3 point2)
    {
        Gizmos.color = edgeColor;
        Gizmos.DrawLine(point1 + new Vector3(0f, yLevel, 0f),
                        point2 + new Vector3(0f, yLevel, 0f));
    }

    private void SetCameraForDungeonView()
    {
        if (cam == null) return;

        // Slightly higher camera so 3 rooms fit comfortably in view
        cam.transform.position = center + new Vector3(0f, 10f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    // Simple helper type to store one edge in the dungeon graph
    private struct RoomConnection
    {
        public Vector3 from;
        public Vector3 to;

        public RoomConnection(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }
    }
}