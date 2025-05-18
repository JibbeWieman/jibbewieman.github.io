using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ActorFacade : MonoBehaviour
{
    #region VARIABLES
    private ActorsManager actorManager;
    private ActorCharacterController actorController;
    private ActorWeaponsManager actorWeaponsManager;

    private Actor currentActor;

    [Tooltip("The parasite.")]
    private GameObject player;

    private HostThirdPersonCam hostCam;
    private InfectAbility infectAbility;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        actorManager = GetComponent<ActorsManager>();
        player = actorManager?.Player;

        EventManager.AddListener<ActorPossesedEvent>(SwitchActor);
        EventManager.AddListener<AimEvent>(HandleCameraSwitching);
        Events.ActorPossesedEvent.CurrentActor = 0;                 // Player actor ID is 0

        hostCam = FindOrLogError<HostThirdPersonCam>();
        infectAbility = FindOrLogError<InfectAbility>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Events.ActorPossesedEvent.InHost)
            LeaveHost();
    }

    private void FixedUpdate()
    {
        actorController?.AdjustAnimationSpeed();
        actorController?.HandleCharacterMovement();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<ActorPossesedEvent>(SwitchActor);
        EventManager.RemoveListener<AimEvent>(HandleCameraSwitching);
    }
    #endregion

    #region ACTOR MANAGEMENT
    public void SwitchActor(ActorPossesedEvent evt)
    {
        var targetActor = ActorsManager.FindActorById(evt.CurrentActor);
        if (targetActor == null)
        {
            Debug.LogError($"Actor with ID {evt.CurrentActor} not found!");
            return;
        }

        EnterActor(targetActor);
    }

    private void EnterActor(Actor targetActor)
    {
        SetHostLogic(targetActor);

        currentActor = targetActor;
        actorController = FindActorController(currentActor.gameObject);

        if (actorController == null)
        {
            Debug.LogError($"ActorCharacterController not found for {currentActor.gameObject.name}!");
            return;
        }

        if (!currentActor.IsPlayer())
        {
            currentActor.Affiliation = 0;
        }

        if (!currentActor.GetComponent<AudioListener>())
        {
            currentActor.AddComponent<AudioListener>();
        }

        ReviveActor(currentActor);
        DisableAIComponents(currentActor);
        EnableWeaponsManager(currentActor);

        actorController.transform.SetPositionAndRotation(currentActor.transform.position, currentActor.transform.rotation);

        hostCam?.SetupHostVariables(currentActor);
        CameraSwitcher.RegisterCameras(currentActor);

        //Debug.Log($"Switching to actor: {targetActor.name}, camera: {targetActor.BasicCam}");
        CameraSwitcher.SwitchCamera(currentActor.BasicCam);
    }

    private void LeaveHost()
    {
        currentActor.GetComponent<Health>().TakeDamage(250, player);
        currentActor.GetComponentInChildren<Animator>().SetBool("isWalking", false);

        player.transform.SetPositionAndRotation(currentActor.transform.position + Vector3.left, currentActor.transform.rotation);
        player.SetActive(true);

        if (!currentActor.IsPlayer())
        {
            currentActor.Affiliation = 1;
        }

        if (currentActor.TryGetComponent<AudioListener>(out var audioListener))
        {
            Destroy(audioListener);
        }

        DisableWeaponsManager();
        MoveHostToRefuge();

        CameraSwitcher.UnregisterCameras(currentActor);
        Events.ActorPossesedEvent.CurrentActor = 0; // Player actor ID is 0
    }

    private void HandleCameraSwitching(AimEvent evt)
    {
        if (evt.IsAiming)
        {
            CameraSwitcher.SwitchCamera(currentActor.CombatCam);
        }
        else
        {
            CameraSwitcher.SwitchCamera(currentActor.BasicCam);
        }
    }
    #endregion

    #region HELPER METHODS
    private T FindOrLogError<T>() where T : MonoBehaviour
    {
        var component = FindObjectOfType<T>();
        DebugUtility.HandleErrorIfNullFindObject<T, ActorFacade>(component, this);
        return component;
    }

    private ActorCharacterController FindActorController(GameObject actor)
    {
        return actor.GetComponent<ActorCharacterController>() ?? actor.GetComponentInParent<ActorCharacterController>();
    }

    private void SetHostLogic(Actor targetActor)
    {
        if (targetActor.IsPlayer())
        {
            Events.ActorPossesedEvent.InHost = false;
            return;
        }

        player.SetActive(false);
        infectAbility.isLeeching = false;
        Events.ActorPossesedEvent.InHost = true;
    }

    private void ReviveActor(Actor actor)
    {
        var health = actor.GetComponent<Health>();
        if (health?.CurrentHealth <= 0)
        {
            health.Heal(1);
        }
    }

    private void DisableAIComponents(Actor actor)
    {
        actor.TryGetComponent(out PatrolAgent _patrolAgent);
        actor.TryGetComponent(out NavMeshAgent _navMeshAgent);

        if (_patrolAgent != null)
            _patrolAgent.enabled = false;
        if (_navMeshAgent != null)
            _navMeshAgent.enabled = false;
    }

    private void EnableWeaponsManager(Actor actor)
    {
        if (actor.TryGetComponent(out actorWeaponsManager))
        {
            actorWeaponsManager.enabled = true;
        }
    }

    private void DisableWeaponsManager()
    {
        if (actorWeaponsManager != null)
        {
            actorWeaponsManager.enabled = false;
        }
    }

    private void MoveHostToRefuge()
    {
        Transform refugeCamp = GameObject.FindWithTag("HostRefugeCamp")?.transform;
        if (refugeCamp != null)
        {
            currentActor.transform.SetParent(refugeCamp);
        }
    }
    #endregion
}
