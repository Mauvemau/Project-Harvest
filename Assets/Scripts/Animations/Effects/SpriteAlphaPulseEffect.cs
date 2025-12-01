using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaPulse : MonoBehaviour {
    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;

    private SpriteRenderer _spriteRenderer;
    private float _timer;

    private void Update() {
        _timer += Time.deltaTime * fadeSpeed;

        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            Mathf.PingPong(_timer, 1f)
        );

        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }
    
    private void Awake() {
        if (!TryGetComponent(out _spriteRenderer)) {
            Debug.LogError($"{name}: missing required component; {nameof(SpriteRenderer)}!");
        }
    }
    
    private void OnEnable() {
        _timer = 0f;
        
        Color color = _spriteRenderer.color;
        color.a = maxAlpha;
        _spriteRenderer.color = color;
    }
}