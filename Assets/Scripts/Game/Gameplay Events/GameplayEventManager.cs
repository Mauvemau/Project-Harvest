using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameplayEventManager {
    [SerializeField] private ChangeSpawnRateEvent[] changeSpawnRateEvents;
    [SerializeField] private SpawnEnemyEvent[] spawnEnemyEvents;

    [Header("Debug")] 
    [SerializeField] private SpawnEnemyEvent[] spawnDebugEnemyEvents;

    private void PollEvents(IEnumerable<GameplayEvent> gameplayEvents, float currentGameTime) {
        foreach (GameplayEvent gameplayEvent in gameplayEvents) {
            if(currentGameTime < gameplayEvent.TimestampInSeconds) return;
            gameplayEvent.TriggerEvent();
        }
    }

    private void ValidateEvents(IEnumerable<GameplayEvent> gameplayEvents) {
        foreach (GameplayEvent gameplayEvent in gameplayEvents) {
            gameplayEvent.OnValidate();
        }
    }

    private void ResetEvents(IEnumerable<GameplayEvent> gameplayEvents) {
        foreach (GameplayEvent gameplayEvent in gameplayEvents) {
            gameplayEvent.Reset();
        }
    }
    
    public void Reset() {
        ResetEvents(changeSpawnRateEvents);
        ResetEvents(spawnEnemyEvents);
        ResetEvents(spawnDebugEnemyEvents);
    }
    
    public void Update(float currentGameTime, bool debug = false) {
        if (!debug) {
            PollEvents(changeSpawnRateEvents, currentGameTime);
            PollEvents(spawnEnemyEvents, currentGameTime);
        }
        else {
            PollEvents(spawnDebugEnemyEvents, currentGameTime);
        }
    }

    public void OnValidate() {
        if (changeSpawnRateEvents.Length > 0) {
            ValidateEvents(changeSpawnRateEvents);
        }
        if (spawnEnemyEvents.Length > 0) {
            ValidateEvents(spawnEnemyEvents);
        }
        if (spawnDebugEnemyEvents.Length > 0) {
            ValidateEvents(spawnDebugEnemyEvents);
        }
    }
}
