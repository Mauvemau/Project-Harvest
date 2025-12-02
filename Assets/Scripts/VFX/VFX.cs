using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class VFX : MonoBehaviour, IVFX {
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float _clipLength;

    private void FinishAnimation() {
        gameObject.SetActive(false);
    }

    private void RestartAnimation() {
        _animator.Play(0, 0, 0f);
    }

    private void FinishLooping() {
        CancelInvoke(nameof(RestartAnimation));
        FinishAnimation();
    }
    
    public void Play(VFXPreset preset, float duration = 0f) {
        if (!preset.Controller) {
            FinishAnimation();
            return;
        }
        if (_spriteRenderer) {
            _spriteRenderer.color = preset.Color;
            _spriteRenderer.sortingOrder = preset.DrawOrder;
        }
        
        _animator.runtimeAnimatorController = preset.Controller;
        _animator.speed = preset.PlaybackSpeed;
        _animator.Play(0, 0, preset.AnimationStartOffset);

        AnimatorClipInfo[] info = _animator.GetCurrentAnimatorClipInfo(0);
        _clipLength = info.Length > 0 ? info[0].clip.length : 0.5f;

        CancelInvoke();

        if (duration > 0f) {
            InvokeRepeating(nameof(RestartAnimation), _clipLength / _animator.speed, _clipLength / _animator.speed);
            Invoke(nameof(FinishLooping), duration);
        } else {
            Invoke(nameof(FinishAnimation), _clipLength / _animator.speed);
        }
    }

    private void Awake() {
        if (!TryGetComponent(out _animator)) {
            Debug.LogError($"{name}: missing required component {nameof(Animator)}");
        }
        if (!TryGetComponent(out _spriteRenderer)) {
            Debug.LogError($"{name}: missing required component {nameof(SpriteRenderer)}");
        }
    }

    private void OnDisable() {
        CancelInvoke();
    }
}