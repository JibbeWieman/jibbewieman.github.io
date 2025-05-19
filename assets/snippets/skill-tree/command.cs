using Unity.FPS.Game;
using UnityEngine;

public class Command
{
    public virtual void Execute()
    {
    }
}

public class UnlockCommand : Command
{
    private Node node;
    private int cost;
    private SkillPointsChangedEvent pointsChangedEvt = Events.SkillPointsChangedEvent;

    public UnlockCommand(Node node, int cost)
    {
        this.node = node;
        this.cost = cost;
    }

    public override void Execute()
    {
        Debug.Log($"Trying to unlock: <color=#42ff98>{node.name}</color>");

        // Check if the node can be unlocked
        if (!node.CanUnlock() || node.IsUnlocked)
        {
            Debug.Log($"<color=#ff0000>Unable to unlock: {node.name}</color>");
            return;
        }

        // Deduct skill points and unlock the skill
        node.IsUnlocked = true;
        pointsChangedEvt.SkillPoints -= cost;
        node.UpdateButtonColour();

        // Set conflicting nodes to !interactable
        if (node.conflictingNodes != null)
        {
            foreach (Node conflictingNode in node.conflictingNodes)
            {
                conflictingNode.skillButton.interactable = false;
            }
        }

        // Broadcast unlock event
        SkillUnlockedEvent skillUnlockedEvent = Events.SkillUnlockedEvent;
        skillUnlockedEvent.skillType = node.skillType;
        skillUnlockedEvent.node = node.gameObject;
        EventManager.Broadcast(skillUnlockedEvent);

        Debug.Log($"Unlocked: <color=#42ff98>{node.name}</color>");
    }
}

public class ShowTooltipCommand : Command
{
    private readonly SkillTooltip _tooltip;

    public ShowTooltipCommand(SkillTooltip tooltip)
    {
        _tooltip = tooltip;
    }

    public override void Execute()
    {
        _tooltip.ShowTooltip();
        Debug.Log($"Showing ToolTip: <color=#42ff98>{_tooltip}</color>");
    }
}

public class HideTooltipCommand : Command
{
    private readonly SkillTooltip _tooltip;

    public HideTooltipCommand(SkillTooltip tooltip)
    {
        _tooltip = tooltip;
    }

    public override void Execute()
    {
        _tooltip.HideTooltip();
        Debug.Log($"Hiding ToolTip: <color=#42ff98>{_tooltip}</color>");
    }
}
