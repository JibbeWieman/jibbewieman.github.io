using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    #region VARIABLES
    public int id;
    public int cost;
    public bool IsUnlocked;
    public SkillType skillType;
    public SkillTooltip skillTooltip;
    [HideInInspector]
    public Button skillButton;

    [Tooltip("List of nodes that conflict with this node. If any of these are unlocked, this node cannot be unlocked.")]
    [Space(5)]
    public List<Node> conflictingNodes;

    private SkillPointsChangedEvent SkillPointsChanged = Events.SkillPointsChangedEvent;

    [Space(10), SerializeField]
    private SceneTypeObject ST_GameManager;
    private GraphManager graphManager;
    #endregion

    private void Start()
    {
        // Find the GraphManager in the scene
        if (ST_GameManager.Objects.Count > 0 && ST_GameManager.Objects[0].TryGetComponent<GraphManager>(out graphManager))
            graphManager.AddSkillNode(id, this);
        else
            Debug.LogWarning("No GraphManager found in the scene!");

        // Add Unlock as an onClick listener to the button component
        skillButton = GetComponent<Button>();
        if (skillButton != null && skillButton.onClick.GetPersistentEventCount() == 0)
            //skillButton.onClick.AddListener(Unlock);
            skillButton.onClick.AddListener(ExecuteUnlockCommand);
        else if (skillButton == null)
            Debug.LogWarning("No Button component found on this GameObject!");
        else if (skillButton.onClick.GetPersistentEventCount() == 0)
            Debug.Log("Skill Button already has a function assigned to it!");
    }

    private void ExecuteUnlockCommand()
    {
        // Create and execute the command
        UnlockCommand unlockCommand = new(this, cost);
        unlockCommand.Execute();
    }

    public bool CanUnlock()
    {
        // First, check if the player has enough skill points to unlock this skill
        if (SkillPointsChanged.SkillPoints < cost)
        {
            //Debug.Log("L broke boy.");
            return false;
        }

        // Then, check if all prerequisite nodes are unlocked
        List<Node> prerequisites = graphManager.GetPrerequisites(id);
        foreach (Node prerequisite in prerequisites)
        {
            if (!prerequisite.IsUnlocked)
            {
                //Debug.Log("Prerequisite nodes are not unlocked.");
                return false;
            }
        }

        // Lastly, check if any conflicting nodes are unlocked
        foreach (Node conflictingNode in conflictingNodes)
        {
            if (conflictingNode != null && conflictingNode.IsUnlocked)
            {
                Debug.Log($"Conflict detected with Node ID: {conflictingNode}. Cannot unlock this node.");
                return false;
            }
        }
        return true;
    }

    public void UpdateButtonColour()
    {
        if (ColorUtility.TryParseHtmlString("#DBB448", out Color newColor))
        {
            ColorBlock colors = skillButton.colors;
            colors.normalColor = newColor;
            skillButton.colors = colors;
        }
    }
}
