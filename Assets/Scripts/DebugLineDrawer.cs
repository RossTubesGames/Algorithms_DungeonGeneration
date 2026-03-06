using UnityEngine;

public class DebugLineDrawer : MonoBehaviour
{
    // Start point of the line in the unity world space
    [SerializeField]
    private Vector3 start = new Vector3(0f, 0f, 0f);

    //end point of the line in world space
    [SerializeField]
    private Vector3 end = new Vector3(5f, 0f, 0f); //moving forward
    private void Start()
    {
        OnDrawGizmos();
    }
    //function to draw debug visual
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Draw a line from start position to end position
        Gizmos.DrawLine(start, end);

        // Draw small spheres at both ends so we can see the points
        Gizmos.DrawSphere(start, 0.1f);
        Gizmos.DrawSphere(end, 0.1f);
    }
}
