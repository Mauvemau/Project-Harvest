using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class IntervalAnimationController : MonoBehaviour {
    [Header("Animation Settings")]
    [SerializeField, Min(0f)] private float minInterval = 2f;
    [SerializeField, Min(0f)] private float maxInterval = 5f;

    private Animator _animator;
    private float _clipLength;

    private void ScheduleNextPlay() {
        float interval = Random.Range(minInterval, maxInterval);
        Invoke(nameof(PlayEntryAnimation), interval);
    }

    private void PlayEntryAnimation() {
        _animator.enabled = true;
        _animator.Play(0, 0, 0f);
        
        Invoke(nameof(ScheduleNextPlay), _clipLength / _animator.speed);
    }
    
    private void Awake() {
        if (!TryGetComponent(out _animator)) {
            Debug.LogError($"{name}: missing required component \"{nameof(Animator)}\"");
            enabled = false;
            return;
        }
        
        if (_animator.runtimeAnimatorController != null &&
            _animator.runtimeAnimatorController.animationClips.Length > 0) {
            _clipLength = _animator.runtimeAnimatorController.animationClips[0].length;
        } else {
            Debug.LogWarning($"{name}: no animation clips found on animator.");
            _clipLength = 1f;
        }

        ScheduleNextPlay();
    }
}