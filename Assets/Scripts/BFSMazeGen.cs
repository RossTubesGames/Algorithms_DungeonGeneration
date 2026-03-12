using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSMazeGen : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Maze Settings")]
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private int width = 15;
    [SerializeField] private int height = 15;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float yLevel = 0f;
    [SerializeField] private float nodeRadius = 0.12f;

    [Header("BFS Generation")]
    [SerializeField] private Vector2Int startCell = new Vector2Int(7, 7);
    [SerializeField] private int targetOpenCells = 30;
    [SerializeField, Range(0f, 1f)] private float branchChance = 0.45f;

    [Header("Generation Speed")]
    [SerializeField] private float stepDelay = 0.05f;
    [SerializeField] private float dequeueDelay = 0.01f;

    [Header("Draw Settings")]
    [SerializeField] private bool drawBlockedGrid = true;
    [SerializeField] private bool drawConnectionLines = true;
    [SerializeField] private bool drawNodeDots = false;
    [SerializeField] private bool drawCurrentCell = true;

    private bool[,] openCells;
    private bool drawMaze = false;
    private Vector2Int farthestCell;
    private Vector2Int currentCell;
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

        if (drawMaze)
        {
            DrawMazeGizmo();
        }
    }

    private void DrawMazeGizmo()
    {
        if (openCells == null)
            return;

        float offsetX = (width - 1) * cellSize * 0.5f;
        float offsetZ = (height - 1) * cellSize * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = GetWorldPosition(x, z, offsetX, offsetZ);

                if (openCells[x, z])
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(worldPos, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));

                    if (drawNodeDots)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(worldPos + Vector3.up * 0.05f, nodeRadius);
                    }

                    if (drawConnectionLines)
                    {
                        DrawConnections(x, z, worldPos, offsetX, offsetZ);
                    }
                }
                else if (drawBlockedGrid)
                {
                    Gizmos.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    Gizmos.DrawWireCube(worldPos, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));
                }
            }
        }

        DrawSpecialCells(offsetX, offsetZ);
    }

    private void DrawConnections(int x, int z, Vector3 worldPos, float offsetX, float offsetZ)
    {
        Gizmos.color = Color.white;

        if (x + 1 < width && openCells[x + 1, z])
        {
            Vector3 rightPos = GetWorldPosition(x + 1, z, offsetX, offsetZ);
            Gizmos.DrawLine(worldPos, rightPos);
        }

        if (z + 1 < height && openCells[x, z + 1])
        {
            Vector3 topPos = GetWorldPosition(x, z + 1, offsetX, offsetZ);
            Gizmos.DrawLine(worldPos, topPos);
        }
    }

    private void DrawSpecialCells(float offsetX, float offsetZ)
    {
        if (IsInside(startCell))
        {
            Vector3 startPos = GetWorldPosition(startCell.x, startCell.y, offsetX, offsetZ);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(startPos + Vector3.up * 0.06f, new Vector3(cellSize * 0.55f, 0.06f, cellSize * 0.55f));
        }

        if (IsInside(farthestCell))
        {
            Vector3 endPos = GetWorldPosition(farthestCell.x, farthestCell.y, offsetX, offsetZ);
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(endPos + Vector3.up * 0.08f, new Vector3(cellSize * 0.45f, 0.06f, cellSize * 0.45f));
        }

        if (drawCurrentCell && IsInside(currentCell))
        {
            Vector3 currentPos = GetWorldPosition(currentCell.x, currentCell.y, offsetX, offsetZ);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(currentPos + Vector3.up * 0.1f, new Vector3(cellSize * 0.35f, 0.06f, cellSize * 0.35f));
        }
    }

    private Vector3 GetWorldPosition(int x, int z, float offsetX, float offsetZ)
    {
        float xPos = x * cellSize - offsetX;
        float zPos = z * cellSize - offsetZ;
        return center + new Vector3(xPos, yLevel, zPos);
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

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

            if (IsInside(next))
            {
                neighbors.Add(next);
            }
        }

        return neighbors;
    }

    private bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    private void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Vector2Int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private int CountOpenNeighbors(Vector2Int cell)
    {
        int count = 0;

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

            if (IsInside(next) && openCells[next.x, next.y])
            {
                count++;
            }
        }

        return count;
    }

    private void SetCameraForMazeView()
    {
        if (cam == null)
            return;

        cam.transform.position = center + new Vector3(0f, 24f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void DrawMaze()
    {
        if (generationRoutine != null)
        {
            StopCoroutine(generationRoutine);
        }

        openCells = new bool[width, height];

        if (!IsInside(startCell))
        {
            startCell = new Vector2Int(width / 2, height / 2);
        }

        farthestCell = startCell;
        currentCell = startCell;
        drawMaze = true;
        SetCameraForMazeView();

        generationRoutine = StartCoroutine(GenerateMazeSlowly());
    }

    public void ClearMaze()
    {
        if (generationRoutine != null)
        {
            StopCoroutine(generationRoutine);
            generationRoutine = null;
        }

        drawMaze = false;
        openCells = null;
    }

    private IEnumerator GenerateMazeSlowly()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startCell);
        visited.Add(startCell);
        openCells[startCell.x, startCell.y] = true;

        int openCount = 1;

        if (stepDelay > 0f)
            yield return new WaitForSeconds(stepDelay);

        while (queue.Count > 0 && openCount < targetOpenCells)
        {
            Vector2Int current = queue.Dequeue();
            currentCell = current;
            farthestCell = current;

            if (dequeueDelay > 0f)
                yield return new WaitForSeconds(dequeueDelay);

            List<Vector2Int> neighbors = GetNeighbors(current);
            Shuffle(neighbors);

            for (int i = 0; i < neighbors.Count; i++)
            {
                Vector2Int next = neighbors[i];

                if (visited.Contains(next))
                    continue;

                visited.Add(next);

                if (CountOpenNeighbors(next) <= 1 && Random.value <= branchChance)
                {
                    openCells[next.x, next.y] = true;
                    queue.Enqueue(next);
                    openCount++;
                    currentCell = next;

                    if (stepDelay > 0f)
                        yield return new WaitForSeconds(stepDelay);

                    if (openCount >= targetOpenCells)
                        break;
                }
            }
        }

        currentCell = farthestCell;
        generationRoutine = null;
        Debug.Log("BFS maze drawn.");
    }
}