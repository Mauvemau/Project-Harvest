using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFadeInEffect : MonoBehaviour {
    [SerializeField] private float fadeDurationInSeconds = 1f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup _canvasGroup;
    private Coroutine _fadeRoutine;

    private void StartFadeIn() {
        if (!_canvasGroup) return;
        if (_fadeRoutine != null) {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine() {
        _canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDurationInSeconds) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDurationInSeconds);
            _canvasGroup.alpha = fadeCurve.Evaluate(t);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _fadeRoutine = null;
    }
    
    private void Awake() {
        if (!TryGetComponent(out _canvasGroup)) {
            Debug.LogError($"{name}: missing required component {nameof(CanvasGroup)}!");
        }
    }

    private void OnEnable() {
        StartFadeIn();
    }

    private void OnDisable() {
        if (_fadeRoutine != null) {
            StopCoroutine(_fadeRoutine);
        }
        
        if (!_canvasGroup) return;
        _canvasGroup.alpha = 0f;
    }
}