using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameplayEventManager {
    [SerializeField] private ChangeSpawnRateEvent[] changeSpawnRateEvents;
    [SerializeField] private SpawnEnemyEvent[] spawnEnemyEvents;

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
    }
    
    public void Update(float currentGameTime) {
        PollEvents(changeSpawnRateEvents, currentGameTime);
        PollEvents(spawnEnemyEvents,  currentGameTime);
    }

    public void OnValidate() {
        ValidateEvents(changeSpawnRateEvents);
        ValidateEvents(spawnEnemyEvents);
    }
}
