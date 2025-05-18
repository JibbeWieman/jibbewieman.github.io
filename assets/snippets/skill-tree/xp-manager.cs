using Unity.FPS.Game;
using UnityEngine;

public class XpManager : Manager
{
    #region VARIABLES
    public int PlayerLevel { get; private set; }
    public int PlayerXP { get; private set; }

    [Tooltip("Previous XP Threshold required to level up")]
    public int PrevThreshold { get; private set; }

    [Tooltip("XP required to level up")]
    public int CurThreshold { get; private set; }
    #endregion

    #region UNITY METHODS
    public override void Start()
    {
        base.Start();

        EventManager.AddListener<XpUpdateEvent>(UpdateXP);

        PlayerLevel = 1;        // Start at level 1
        PlayerXP = 0;           // Start with 0 XP
        PrevThreshold = 0;
        CalculateXPThreshold(); // Initialize XP threshold for level-up
    }
    #endregion

    #region XP METHODS
    public void UpdateXP(XpUpdateEvent evt)
    {
        PlayerXP = evt.XP;      // Add XP to player's total

        // Check if the player has enough XP to level up
        if (PlayerXP >= CurThreshold)
        {
            LevelUpEvent lvlUpEvt = Events.LevelUpEvent;
            LevelUp(lvlUpEvt);
        }
    }

    // Handles level-up logic
    private void LevelUp(LevelUpEvent evt)
    {
        evt.Level++;

        PlayerLevel = evt.Level;

        SkillPointsChangedEvent skillPointEvent = Events.SkillPointsChangedEvent;
        skillPointEvent.SkillPoints++;

        CalculateXPThreshold();  // Recalculate XP needed for the next level
    }

    // Calculate the XP threshold for the next level
    private void CalculateXPThreshold()
    {
        PrevThreshold = CurThreshold;
        CurThreshold = PlayerLevel * 150;
    }

    void OnDestroy()
    {
        EventManager.RemoveListener<XpUpdateEvent>(UpdateXP);
    }
    #endregion
}