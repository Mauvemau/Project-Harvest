using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GlobalVariableManager {
    [Header("Global Game Variables")] 
    [SerializeField] private GameVariables gameBaseVariables;
    [SerializeField] private GameVariables gameCurrentVariables;
    
    [Header("Game Variables Settings")]
    [Tooltip("Each level earned, the experience needed to level up is multiplied by this amount")]
    [SerializeField] private float experienceNeededIncrease = 1.15f;

    [Header("Controllers")]
    [SerializeField] private ProgressBarController xpBarController;
    
    [Header("Event Invokers")]
    [SerializeField] private VoidEventChannelSO onLevelUp;
    [SerializeField] private StringEventChannelSo onUpdateLevelValue;

    private void UpdateXpBarUI() {
        xpBarController.UpdateValues(gameCurrentVariables.CurrentExperience, gameCurrentVariables.ExperienceNeeded);
    }

    private void UpdateCurrentLevelUI() {
        if (onUpdateLevelValue) {
            onUpdateLevelValue.RaiseEvent($"{gameCurrentVariables.CurrentLevel}");
        }
    }
    
    private void CheckIfLevelUp() {
        while (gameCurrentVariables.CurrentExperience >= gameCurrentVariables.ExperienceNeeded) {
            gameCurrentVariables.CurrentExperience -= gameCurrentVariables.ExperienceNeeded;
            gameCurrentVariables.CurrentLevel++;
            gameCurrentVariables.ExperienceNeeded *= experienceNeededIncrease;

            UpdateCurrentLevelUI();
            if (onLevelUp) {
                onLevelUp.RaiseEvent();
            }
        }
    }
    
    public void AddCurrentExperience(float amount) {
        gameCurrentVariables.CurrentExperience += amount;
        CheckIfLevelUp();
        UpdateXpBarUI();
    }
    
    //

    public void DebugLevelUp() {
        AddCurrentExperience(gameCurrentVariables.ExperienceNeeded);
    }
    
    [ContextMenu("Debug GlobalVars - Reset XP")]
    public void ResetGameVariables() {
        gameCurrentVariables = gameBaseVariables.Copy();
    }
    
    [ContextMenu("Debug GlobalVars - Reset All")]
    public void ResetAll() {
        ResetGameVariables();
        UpdateXpBarUI();
        UpdateCurrentLevelUI();
    }
    
    public void Init() {
        ResetAll();
    }
}
