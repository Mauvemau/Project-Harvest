using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class IntervalAnimationController : MonoBehaviour {
    [Header("Animation Settings")]
    [SerializeField, Min(0f)] private float minInterval = 2f;
    [SerializeField, Min(0f)] private float maxInterval = 5f;

    private Animator _animator;
    private float _clipLength;
    private float _nextClipEnd;
    private float _nextClipStart;

    private void ScheduleNextPlay() {
        _nextClipEnd = 0;
        float interval = Random.Range(minInterval, maxInterval);
        _nextClipStart = Time.time + interval;
    }

    private void PlayAnimation() {
        if (!enabled) return;
        _animator.Play(0, 0, 0f);

        _nextClipEnd = Time.time + _clipLength;
    }

    private void Update() {
        if (!_animator) return;
        if(_nextClipEnd > 0 && Time.time > _nextClipEnd) {
            ScheduleNextPlay();
        }
        if(_nextClipEnd == 0 && Time.time > _nextClipStart) {
            PlayAnimation();
        }
    }

    private void Awake() {
        if (!TryGetComponent(out _animator)) {
            Debug.LogError($"{name}: missing required component \"{nameof(Animator)}\"");
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