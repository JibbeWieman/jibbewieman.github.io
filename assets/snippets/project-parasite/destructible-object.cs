using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class DestructibleObject : MonoBehaviour
{
    #region VARIABLES
    [Header("Destructible Settings")]
    [SerializeField]
    private GameObject destroyedVersion;

    [SerializeField]
    private AudioClip[] destructionSfx;
    private AudioSource m_AudioSource;

    [Header("Self-Destruct Settings")]
    [SerializeField]
    private bool enableSelfDestruct = false;
    public bool EnableSelfDestruct { get => enableSelfDestruct; set => enableSelfDestruct = value; }

    [SerializeField, Tooltip("Destroy the main object after all children are destroyed"), HideInInspector]
    private bool destroyRoot = false;
    public bool DestroyRoot { get => destroyRoot; set => destroyRoot = value; }

    [SerializeField, Range(1f, 20f), HideInInspector]
    private float minDestroyTime = 3f;
    public float MinDestroyTime { get => minDestroyTime; set => minDestroyTime = value; }

    [SerializeField, Range(1f, 20f), HideInInspector]
    private float maxDestroyTime = 10f;
    public float MaxDestroyTime { get => maxDestroyTime; set => maxDestroyTime = value; }
    #endregion

    #region UNITY METHODS
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            DestroyObject();
        }
    }
    #endregion

    #region DESTRUCTION METHODS
    public void DestroyObject()
    {
        // Find the scene
        Scene currentScene = gameObject.scene;

        if (!currentScene.isLoaded || destroyedVersion == null) return;

        // Instantiate the prefab in the target scene
        GameObject instantiatedObject = Instantiate(destroyedVersion, transform.position, transform.rotation);

        // Play Destruction Sound
        m_AudioSource = instantiatedObject.GetComponent<AudioSource>();
        DebugUtility.HandleErrorIfNullGetComponent<AudioSource, DestructibleObject>(m_AudioSource, instantiatedObject.transform, instantiatedObject);

        Game_Manager.PlayRandomSfx(m_AudioSource, destructionSfx);

        // Set the parent of the instantiated object to ensure it's in the correct scene hierarchy
        SceneManager.MoveGameObjectToScene(instantiatedObject, currentScene);
        instantiatedObject.transform.SetParent(transform.parent);

        // Optionally destroy the current object
        Destroy(gameObject);

        if (EnableSelfDestruct)
        {
            StartCoroutine(SelfDestructRoutine(instantiatedObject));
        }
    }

    private IEnumerator SelfDestructRoutine(GameObject targetObject)
    {
        DestructionTarget[] destructionTargets = targetObject.GetComponentsInChildren<DestructionTarget>();

        foreach (DestructionTarget target in destructionTargets)
        {
            Destroy(target.gameObject, Random.Range(minDestroyTime, maxDestroyTime));
        }

        if (destroyRoot)
        {
            yield return new WaitForSeconds(maxDestroyTime);
            Destroy(targetObject);
        }
    }

    #endregion
}

#if UNITY_EDITOR
#region Custom Inspector for DestructibleObject
[CustomEditor(typeof(DestructibleObject))]
public class DestructibleObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DestructibleObject destructibleObject = (DestructibleObject)target;

        // Draw default inspector fields
        DrawDefaultInspector();

        // Only show self-destruct fields if enableSelfDestruct is true
        if (destructibleObject.EnableSelfDestruct)
        {
            destructibleObject.DestroyRoot = EditorGUILayout.Toggle("Destroy Root", destructibleObject.DestroyRoot);
            destructibleObject.MinDestroyTime = EditorGUILayout.FloatField("Min Destroy Time", destructibleObject.MinDestroyTime);
            destructibleObject.MaxDestroyTime = EditorGUILayout.FloatField("Max Destroy Time", destructibleObject.MaxDestroyTime);
        }

        // Apply changes made in the custom inspector to the target object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endregion
#endif
