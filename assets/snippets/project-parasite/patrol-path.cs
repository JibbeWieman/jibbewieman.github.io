using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a patrol path with nodes and options for looping and direction.
/// </summary>
public class PatrolPath : MonoBehaviour
{
    [SerializeField, Tooltip("List of waypoints defining the patrol path.")]
    private List<Transform> waypoints = new List<Transform>();

    public int NodeCount => waypoints.Count;

    public Transform GetWaypoint(int index)
    {
        if (waypoints.Count == 0) return null;
        return waypoints[index % waypoints.Count];
    }

    public Vector3 GetPositionOfWaypoint(int index)
    {
        Transform waypoint = GetWaypoint(index);
        return waypoint ? waypoint.position : Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                if (i + 1 < waypoints.Count && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else if (i == waypoints.Count - 1)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
}