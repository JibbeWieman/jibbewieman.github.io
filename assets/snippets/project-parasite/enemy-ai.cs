using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
	#region VARIABLES
	...
	#endregion

	private void Start()
	{
		...
	}

	private void OnEnable()
	{
		onDetectedTarget += HandleDetectedTarget;
		onAttackTarget -= Attack;
		onLostTarget += Patrol;
	}
	private void OnDisable()
	{
		onDetectedTarget -= HandleDetectedTarget;
		onAttackTarget -= Attack;
		onLostTarget -= Patrol;
	}

	void OnDetectedTarget()
	{
		onDetectedTarget.Invoke();
	}
	void OnAttackTarget()
	{
		onAttackTarget.Invoke();
	}
	void OnLostTarget()
	{
		onLostTarget.Invoke();
	}

	private void HandleDetectedTarget()
	{
		if (!IsScared())
		{
			Chase();
		}
		else
		{
			Flee();
		}
	}
	private bool IsScared()
	{
		if (GetComponentInChildren<ActorWeaponsManager>().GetActiveWeapon() == false 
			|| m_Health.CurrentHealth <= 30)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Regroup(OnBodyFoundEvent evt)
	{
		state = EnemyState.regrouping;

		SetAgentDestination(evt.Body.transform.position);

		if (transform.position == evt.Body.transform.position)
		{
			state = EnemyState.patrolling;
		}
	}

	void EnsureIsWithinLevelBounds()
	{
		// at every frame, this tests for conditions to kill the enemy
		if (transform.position.y < SelfDestructYHeight)
		{
			Destroy(gameObject);
			return;
		}
	}

	void OnDamaged(float damage, GameObject damageSource)
	{
		// test if the damage source is the player
		if (damageSource && !damageSource.GetComponent<EnemyController>())
		{
			// pursue the player
			DetectionModule.OnDamaged(damageSource);

			onDamaged?.Invoke();

			// play the damage tick sound
			if (DamageTick && !m_WasDamagedThisFrame)
				AudioUtility.CreateSFX(DamageTick, transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

			m_WasDamagedThisFrame = true;
		}
	}

	void OnDie()
	{
		// spawn a particle system when dying
		GameObject vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
		Destroy(vfx, 5f);

		Game_Manager.PlayRandomSfx(m_AudioSource, dieSfx);

		m_EnemyManager.UnregisterEnemy(this);

		NavMeshAgent = null;
		m_PatrolAgent = null;
		DetectionModule = null;
	}

	private void Update()
	{
		EnsureIsWithinLevelBounds();

		DetectionModule?.HandleTargetDetection(m_Actor, m_SelfColliders);
		if (m_Health.CurrentHealth <= 0f)
		{
			SetAgentState(0f, false, false);
			return;
		}

		if (!IsStunned && !GetComponent<Actor>().IsActive())
		{
			if (DetectionModule.IsSeeingTarget)
			{
				HandleTargetDetection();
			}

			StateHandler();
		}
	}
	private void HandleTargetDetection()
	{
		if (DetectionModule.KnownDetectedTarget.CompareTag("Host") &&
			DetectionModule.KnownDetectedTarget.GetComponentInParent<ActorCharacterController>().IsDead &&
			DetectionModule.KnownDetectedTarget.GetComponentInParent<Actor>().IsActive())
		{
			enemyTarget = DetectionModule.KnownDetectedTarget;
		}
		else if (DetectionModule.KnownDetectedTarget.transform.root.CompareTag("Player"))
		{
			enemyTarget = player;
		}
		else if (!DetectionModule.KnownDetectedTarget.CompareTag("Host") &&
			!DetectionModule.KnownDetectedTarget.GetComponentInParent<ActorCharacterController>().IsDead &&
			!DetectionModule.KnownDetectedTarget.GetComponentInParent<Actor>().IsActive() &&
			!DetectionModule.KnownDetectedTarget.transform.root.CompareTag("Player"))
		{
			enemyTarget = null;
			OnLostTarget();
		}
	}

	#region STATE FUNCTIONS
	private void StateHandler()
	{
		switch (state)
		{
			case EnemyState.patrolling:
				SetAgentState(m_PatrolSpeed, true, true);
				Patrol();
				break;
			case EnemyState.chasing:
				SetAgentState(m_ChaseSpeed, true, false);
				Chase();
				break;
			case EnemyState.attacking:
				SetAgentState(m_AttackSpeed, true, false);
				Attack();
				break;
			case EnemyState.fleeing:
				SetAgentState(m_FleeSpeed, true, false);
				Flee();
				break;
			case EnemyState.regrouping:
				SetAgentState(m_PatrolSpeed, true, false);
				Regroup(Events.OnBodyFoundEvent);
				break;
			default:
				Debug.LogWarning($"Unhandled state: {state}");
				break;
		}
	}
	private void SetAgentState(float speed, bool navEnabled, bool patrolEnabled)
	{
		NavMeshAgent.enabled = navEnabled;
		NavMeshAgent.speed = speed;
		m_PatrolAgent.enabled = patrolEnabled;
	}

	private void Patrol()
	{
		if (state != EnemyState.patrolling)
			Debug.Log("Patrolling.");

		state = EnemyState.patrolling;
	}

	private void Chase()
	{
		if (state != EnemyState.chasing)
			Debug.Log("Chasing Player");

		state = EnemyState.chasing;

		float distanceToTarget = Vector3.Distance(transform.position, enemyTarget.transform.position);

		if (distanceToTarget <= DetectionModule.AttackRange)
		{
			state = EnemyState.attacking;
			return;
		}

		SetAgentDestination(enemyTarget.transform.position);
	}

	private void Attack()
	{
		if (state != EnemyState.attacking)
			Debug.Log("Attacking player");

		state = EnemyState.attacking;

		//Make sure enemy doesn't move
		SetAgentDestination(transform.position);

		Vector3 targetPosition = new Vector3(enemyTarget.transform.position.x, transform.position.y, enemyTarget.transform.position.z); //Make sure the AI only rotates on the y axis to prevent Neo Dodges
		transform.LookAt(targetPosition);


		if (!alreadyAttacked)
		{
			WeaponController weapon = m_ActorWeaponsManager.GetActiveWeapon();

			weapon.TryShoot(enemyTarget);
			Debug.Log(enemyTarget);

			alreadyAttacked = true;
		}
		Invoke(nameof(ResetAttack), timeBetweenAttacks);
	}
	private void ResetAttack()
	{
		alreadyAttacked = false;
	}

	private void Flee()
	{
		if (state != EnemyState.fleeing)
			Debug.Log("Fleeing from player");

		state = EnemyState.fleeing;

		float distance = Vector3.Distance(transform.position, enemyTarget.transform.position);

		if (distance < DetectionModule.DetectionRange)
		{
			Vector3 dirToPlayer = transform.position - enemyTarget.transform.position;          //Vector player to me

			Vector3 newPos = transform.position + dirToPlayer;

			SetAgentDestination(newPos);
		}
	}
	private void SetAgentDestination(Vector3 destination)
	{
		if (NavMeshAgent && NavMeshAgent.isOnNavMesh)
		{
			NavMeshAgent.SetDestination(destination);
		}
	}
	#endregion

	#region GIZMOS
	void OnDrawGizmosSelected()
	{
		// Path reaching range
		Gizmos.color = PathReachingRangeColor;

		if (DetectionModule != null)
		{
			// Detection range
			Gizmos.color = DetectionRangeColor;
			Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

			// Attack range
			Gizmos.color = AttackRangeColor;
			Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
		}
	}
	#endregion
}