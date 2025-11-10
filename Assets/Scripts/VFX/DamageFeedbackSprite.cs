using UnityEngine;

[System.Serializable]
public class DamageFeedbackSprite {
    [SerializeField] private float feedbackDuration = .5f;

    [Header("Color Settings")]
    [SerializeField] private Color damageTint = Color.red;
    [SerializeField] private Color healingTint = Color.green;
    [SerializeField] private AnimationCurve colorSwitchCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Death Fade Settings")]
    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Color _defaultColor;
    private Color _targetColor;
    private float _elapsed;
    private bool _isPlaying;
    private bool _isDead;
    private float _deadElapsed;

    private SpriteRenderer _spriteRenderer;

    public void Init(SpriteRenderer spriteRenderer) {
        if (!spriteRenderer) return;
        _spriteRenderer = spriteRenderer;
        _defaultColor = spriteRenderer.color;
        _elapsed = 0;
        _isPlaying = false;
        _isDead = false;
        _deadElapsed = 0;
    }

    public void Reset() {
        _isPlaying = false;
        _elapsed = 0;
        _isDead = false;
        _deadElapsed = 0;

        if (!_spriteRenderer) return;
        _spriteRenderer.color = _defaultColor;
    }

    public void SetDead() {
        if (_isDead) return;
        _isDead = true;
        _deadElapsed = 0f;
        _isPlaying = false;
    }

    public void PlayDamageFeedback(float damageReceived) {
        if (_isDead) return;
        _targetColor = damageReceived > 0 ? damageTint : healingTint;
        _elapsed = 0f;
        _isPlaying = true;
    }

    public void Update() {
        if (!_spriteRenderer) return;

        if (_isDead) {
            _deadElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_deadElapsed / fadeOutDuration);
            float curveT = fadeOutCurve.Evaluate(t);

            Color c = _defaultColor;
            c.a = Mathf.Lerp(_defaultColor.a, 0f, curveT);
            _spriteRenderer.color = c;
            return;
        }

        if (!_isPlaying) return;

        _elapsed += Time.deltaTime;
        float tFeedback = _elapsed / feedbackDuration;

        if (tFeedback >= 1f) {
            _spriteRenderer.color = _defaultColor;
            _isPlaying = false;
            return;
        }

        float curveValue = tFeedback < 0.5f
            ? colorSwitchCurve.Evaluate(tFeedback * 2f)
            : colorSwitchCurve.Evaluate((1f - tFeedback) * 2f);

        _spriteRenderer.color = Color.Lerp(_defaultColor, _targetColor, curveValue);
    }
}
