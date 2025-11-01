using System;
using UnityEngine;

[System.Serializable]
public abstract class GameplayEvent {
    [Header("Timing Settings")]
    [SerializeField] private float timestampInSeconds = 0f;
    [SerializeField, ReadOnly] private string timestampFormatted;
    
    [Header("Bonus Events")]
    [SerializeField] private CustomCallback eventCallbacks;

    private bool _triggered = false;
    
    public float TimestampInSeconds => timestampInSeconds;

    protected virtual void OnEventTriggered() {}

    public void TriggerEvent() {
        if (_triggered) return;
        _triggered = true;
        
        eventCallbacks.Invoke();
        OnEventTriggered();
    }
    
    public void Reset() {
        _triggered = false;
    }
    
    public void OnValidate() {
        TimeSpan t = TimeSpan.FromSeconds(timestampInSeconds);
        timestampFormatted = $"{(int)t.Minutes:D2}:{(int)t.Seconds:D2}";
    }
}
