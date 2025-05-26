using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves an agent along a patrol path with optional looping and random fallback movement.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PatrolAgent : MonoBehaviour
{
	#region Variables

	[Header("Values")]
	[SerializeField, Tooltip("Should the agent loop the path?")]
	private bool loop = true;

	[SerializeField, Tooltip("Fallback random pacing range.")]
	private float randomPaceRange = 5f;

	[Space(5)]
	[Tooltip("The patrol path this agent will follow.")]
	public PatrolPath patrolPath;

	//Private Variables
	private int currentWaypointIndex = 0;
	private bool isReversing = false;
	private NavMeshAgent m_NavMeshAgent;

	//Random pacing variables
	private bool isRandomDestinationSet = false;
	private Vector3 currentRandomDestination;

	private float timeSinceLastMove;
	private const float maxTimeToReachDestination = 5f; // Max time to reach destination in seconds

	#endregion

	#region Unity Methods

	private void Start()
	{
		m_NavMeshAgent = GetComponent<NavMeshAgent>();

		if (patrolPath == null || patrolPath.NodeCount == 0)
		{
			Debug.LogWarning($"{name} has no patrol path assigned. Switching to random pacing.");
		}
	}

	private void Update()
	{
		if (patrolPath != null && patrolPath.NodeCount > 0)
		{
			Patrol();
		}
		else
		{
			RandomPacing();
		}
	}

	#endregion

	#region Patrol Logic

	private void Patrol()
	{
		Vector3 targetPosition = patrolPath.GetPositionOfWaypoint(currentWaypointIndex);
		SetAgentDestination(targetPosition);

		if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
		{
			AdvanceWaypoint();
		}
	}

	private void AdvanceWaypoint()
	{
		if (loop)
		{
			currentWaypointIndex = (currentWaypointIndex + 1) % patrolPath.NodeCount;
		}
		else
		{
			if (isReversing)
			{
				currentWaypointIndex--;
				if (currentWaypointIndex < 0)
				{
					currentWaypointIndex = 1;
					isReversing = false;
				}
			}
			else
			{
				currentWaypointIndex++;
				if (currentWaypointIndex >= patrolPath.NodeCount)
				{
					currentWaypointIndex = patrolPath.NodeCount - 2;
					isReversing = true;
				}
			}
		}
	}

	private void SetAgentDestination(Vector3 destination)
	{
		if (m_NavMeshAgent.isOnNavMesh)
		{
			m_NavMeshAgent.SetDestination(destination);
		}
	}

	#endregion

	#region Random Pacing

	private void RandomPacing()
	{
		// Check if the agent has reached the current random destination
		if (isRandomDestinationSet && Vector3.Distance(transform.position, currentRandomDestination) < 0.5f)
		{
			isRandomDestinationSet = false; // Mark the destination as reached
			timeSinceLastMove = 0f; // Reset the timer
		}

		// Handle stuck agent
		if (isRandomDestinationSet)
		{
			timeSinceLastMove += Time.deltaTime;
			if (timeSinceLastMove >= maxTimeToReachDestination)
			{
				isRandomDestinationSet = false; // Force pick a new destination
			}
		}

		// If no random destination is set, pick a new one
		if (!isRandomDestinationSet)
		{
			Vector3 randomDirection = Random.insideUnitSphere * randomPaceRange;
			randomDirection += transform.position;

			if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomPaceRange, NavMesh.AllAreas))
			{
				currentRandomDestination = hit.position; // Store the new random destination
				SetAgentDestination(currentRandomDestination);
				isRandomDestinationSet = true; // Mark the destination as set
			}
		}
	}

	#endregion
}