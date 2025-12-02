using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAlphaPulseEffect : MonoBehaviour {
    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;

    private Image _image;
    private float _timer;

    private void UnscaledUpdate() {
        _timer += Time.unscaledDeltaTime * fadeSpeed;

        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            Mathf.PingPong(_timer, 1f)
        );

        Color c = _image.color;
        c.a = alpha;
        _image.color = c;
    }

    private void Update() {
        UnscaledUpdate();
    }

    private void Awake() {
        if (!TryGetComponent(out _image)) {
            Debug.LogError($"{name}: missing required component; {nameof(Image)}!");
        }
    }

    private void OnEnable() {
        _timer = 0f;
        
        Color color = _image.color;
        color.a = maxAlpha;
        _image.color = color;
    }
}