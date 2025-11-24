using UnityEngine;

[System.Serializable]
public class BehaviourModifier {
    [SerializeField, SubclassSelector] private ICharacterBehaviourStrategy behaviour = new StandbyStrategy();
    [SerializeField] private GameObject[] activeWeapons;
    [SerializeField, Min(0f)] private float duration = 10f;
    
    public ICharacterBehaviourStrategy Behaviour => behaviour;
    public float Duration => duration;

    public void ToggleWeapons(bool toggle) {
        if (activeWeapons.Length <= 0) return;
        
        foreach (GameObject weaponObject in activeWeapons) {
            weaponObject.SetActive(toggle);
        }
    }

    public void Reset() {
        ToggleWeapons(false);
    }
}

public class EnemyBehaviourManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Enemy enemyReference;
    
    [Header("Behaviour Settings")]
    [SerializeField] private BehaviourModifier[] behaviourQueue;
    [SerializeField] private bool loopQueue = true;
    
    private int _currentBehaviourIndex = 0;
    private float _nextBehaviourTimestamp = 0f;

    private void ResetBehaviours() {
        foreach (BehaviourModifier behaviourModifier in behaviourQueue) {
            behaviourModifier.Reset();
        }
    }
    
    private void SkipCurrentBehaviour() {
        if (_currentBehaviourIndex >= behaviourQueue.Length) {
            if (loopQueue) {
                _currentBehaviourIndex = 0;   
            }
            return;
        }
        _currentBehaviourIndex++;
        
        SetBehaviour(behaviourQueue[_currentBehaviourIndex]);
    }

    private void SetBehaviour(BehaviourModifier behaviourModifier) {
        if (!enemyReference) return;
        enemyReference.SetBehaviourStrategy(behaviourModifier.Behaviour);
        _nextBehaviourTimestamp = Time.time + behaviourModifier.Duration;
    }
    
    private void Update() {
        if (!enemyReference) return;
        if (_nextBehaviourTimestamp <= 0 || Time.time < _nextBehaviourTimestamp) return;
        
        SkipCurrentBehaviour();
    }
    
    private void Awake() {
        if (!enemyReference) {
            Debug.LogError($"{name}: No enemy reference assigned!");
        }
        if (behaviourQueue.Length <= 0) {
            Debug.LogWarning($"{name}: No behaviours configured!");
        }
    }

    private void OnEnable() {
        if (behaviourQueue.Length <= 0) {
            Debug.LogWarning($"{name}: No behaviours configured!");
            return;
        }
        ResetBehaviours();

        _currentBehaviourIndex = 0;
        
        SetBehaviour(behaviourQueue[_currentBehaviourIndex]);
    }
}
