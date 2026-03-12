using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSCarvedMazeGen : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Maze Size")]
    [SerializeField] private int logicalWidth = 12;
    [SerializeField] private int logicalHeight = 12;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private float yLevel = 0f;

    [Header("Generation Speed")]
    [SerializeField] private float nodeStepDelay = 0.04f;
    [SerializeField] private float blueStepDelay = 0.04f;
    [SerializeField] private float yellowStepDelay = 0.04f;
    [SerializeField] private float backtrackDelay = 0.02f;

    [Header("Draw")]
    [SerializeField] private bool drawWhiteGrid = true;
    [SerializeField] private bool drawBlueWalls = true;
    [SerializeField] private bool drawYellowPath = true;
    [SerializeField] private bool drawVisitOrderDots = true;
    [SerializeField] private float nodeRadius = 0.08f;
    private int currentOrder;

    private bool[,] revealedCells;
    private bool[,] blueCells;
    private bool[,] yellowCells;
    private int[,] visitOrder;

    private bool drawMaze = false;
    private int gridWidth;
    private int gridHeight;
    private Coroutine generationRoutine;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!drawMaze || revealedCells == null)
            return;

        DrawMazeGizmos();
    }

    public void DrawMaze()
    {
        if (generationRoutine != null)
        {
            StopCoroutine(generationRoutine);
        }

        SetupMazeData();
        drawMaze = true;
        SetCameraForMazeView();
        generationRoutine = StartCoroutine(GenerateCornerMazeSlowly());
    }

    public void ClearMaze()
    {
        if (generationRoutine != null)
        {
            StopCoroutine(generationRoutine);
            generationRoutine = null;
        }

        drawMaze = false;
        revealedCells = null;
        blueCells = null;
        yellowCells = null;
        visitOrder = null;
    }

    private void SetupMazeData()
    {
        gridWidth = logicalWidth * 2 + 1;
        gridHeight = logicalHeight * 2 + 1;

        revealedCells = new bool[gridWidth, gridHeight];
        blueCells = new bool[gridWidth, gridHeight];
        yellowCells = new bool[gridWidth, gridHeight];
        visitOrder = new int[gridWidth, gridHeight];

        currentOrder = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                visitOrder[x, y] = -1;
            }
        }
    }

    private IEnumerator GenerateCornerMazeSlowly()
    {
        bool[,] visitedLogical = new bool[logicalWidth, logicalHeight];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int start = new Vector2Int(0, logicalHeight - 1);
        stack.Push(start);
        visitedLogical[start.x, start.y] = true;

        Vector2Int startGrid = LogicalToGrid(start);

        yield return StartCoroutine(RevealNodeAsYellow(startGrid));
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedLogicalNeighbors(current, visitedLogical);

            if (unvisitedNeighbors.Count == 0)
            {
                stack.Pop();

                if (backtrackDelay > 0f)
                    yield return new WaitForSeconds(backtrackDelay);

                continue;
            }

            Vector2Int next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
            visitedLogical[next.x, next.y] = true;

            Vector2Int currentGrid = LogicalToGrid(current);
            Vector2Int nextGrid = LogicalToGrid(next);
            Vector2Int wallBetween = (currentGrid + nextGrid) / 2;

            // Step 1: reveal the next logical node
            yield return StartCoroutine(RevealNodeAsYellow(startGrid));
            yield return StartCoroutine(RevealNodeStep(nextGrid));
            // Step 2: reveal/check the wall cell in blue
            yield return StartCoroutine(RevealBlueStep(wallBetween));
            // Step 3: convert that wall/pass-through to yellow path
            yield return StartCoroutine(RevealYellowStep(wallBetween));
            // Step 4: convert next node to yellow path
            yield return StartCoroutine(RevealYellowStep(nextGrid));




            // Keep current node yellow too
            revealedCells[currentGrid.x, currentGrid.y] = true;
            yellowCells[currentGrid.x, currentGrid.y] = true;

            stack.Push(next);
        }

        generationRoutine = null;
        Debug.Log("Maze generated.");
    }

    private IEnumerator RevealNodeAsYellow(Vector2Int gridCell)
    {
        revealedCells[gridCell.x, gridCell.y] = true;
        yellowCells[gridCell.x, gridCell.y] = true;

        if (visitOrder[gridCell.x, gridCell.y] == -1)
        {
            visitOrder[gridCell.x, gridCell.y] = currentOrder;
            currentOrder++;
        }

        if (yellowStepDelay > 0f)
            yield return new WaitForSeconds(yellowStepDelay);
    }

    private IEnumerator RevealNodeStep(Vector2Int gridCell)
    {
        revealedCells[gridCell.x, gridCell.y] = true;
        blueCells[gridCell.x, gridCell.y] = true;

        if (visitOrder[gridCell.x, gridCell.y] == -1)
        {
            visitOrder[gridCell.x, gridCell.y] = currentOrder;
            currentOrder++;
        }

        if (nodeStepDelay > 0f)
            yield return new WaitForSeconds(nodeStepDelay);
    }

    private IEnumerator RevealBlueStep(Vector2Int gridCell)
    {
        revealedCells[gridCell.x, gridCell.y] = true;
        blueCells[gridCell.x, gridCell.y] = true;

        if (visitOrder[gridCell.x, gridCell.y] == -1)
        {
            visitOrder[gridCell.x, gridCell.y] = currentOrder       ;
            currentOrder++;
        }

        if (blueStepDelay > 0f)
            yield return new WaitForSeconds(blueStepDelay);
    }

    private IEnumerator RevealYellowStep(Vector2Int gridCell)
    {
        revealedCells[gridCell.x, gridCell.y] = true;
        blueCells[gridCell.x, gridCell.y] = false;
        yellowCells[gridCell.x, gridCell.y] = true;

        if (visitOrder[gridCell.x, gridCell.y] == -1)
        {
            visitOrder[gridCell.x, gridCell.y] = currentOrder;
            currentOrder++;
        }

        if (yellowStepDelay > 0f)
            yield return new WaitForSeconds(yellowStepDelay);
    }

    private Vector2Int LogicalToGrid(Vector2Int logicalCell)
    {
        return new Vector2Int(logicalCell.x * 2 + 1, logicalCell.y * 2 + 1);
    }

    private List<Vector2Int> GetUnvisitedLogicalNeighbors(Vector2Int cell, bool[,] visited)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int next = cell + directions[i];

            if (next.x < 0 || next.x >= logicalWidth || next.y < 0 || next.y >= logicalHeight)
                continue;

            if (!visited[next.x, next.y])
            {
                result.Add(next);
            }
        }

        return result;
    }

    private void DrawMazeGizmos()
    {
        float offsetX = (gridWidth - 1) * cellSize * 0.5f;
        float offsetZ = (gridHeight - 1) * cellSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = GridToWorld(x, y, offsetX, offsetZ);

                if (drawWhiteGrid)
                {
                    Gizmos.color = new Color(0.85f, 0.85f, 0.85f, 1f);
                    Gizmos.DrawWireCube(pos, new Vector3(cellSize * 0.95f, 0.03f, cellSize * 0.95f));
                }

                if (!revealedCells[x, y])
                    continue;

                if (drawBlueWalls && blueCells[x, y])
                {
                    Gizmos.color = new Color(0.2f, 0.3f, 1f, 1f);
                    Gizmos.DrawCube(pos, new Vector3(cellSize * 0.7f, 0.04f, cellSize * 0.7f));
                }

                if (drawYellowPath && yellowCells[x, y])
                {
                    Gizmos.color = new Color(1f, 0.9f, 0.15f, 1f);
                    Gizmos.DrawCube(pos + Vector3.up * 0.01f, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));
                }

                if (drawVisitOrderDots && visitOrder[x, y] >= 0)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(pos + Vector3.up * 0.04f, nodeRadius);
                }
            }
        }
    }

    private Vector3 GridToWorld(int x, int y, float offsetX, float offsetZ)
    {
        float xPos = x * cellSize - offsetX;
        float zPos = y * cellSize - offsetZ;
        return center + new Vector3(xPos, yLevel, zPos);
    }

    private void SetCameraForMazeView()
    {
        if (cam == null)
            return;

        float biggest = Mathf.Max(gridWidth, gridHeight) * cellSize;
        cam.transform.position = center + new Vector3(0f, biggest * 1.2f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}