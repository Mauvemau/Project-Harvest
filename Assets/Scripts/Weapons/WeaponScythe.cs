using System.Collections;
using UnityEngine;

/// <summary>
/// A weapon that attacks in a circular angle area of effect around the player.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WeaponScythe : Weapon {
    [Header("Weapon Specific Stats")]
    [SerializeField, Range(10f, 360f)] private float attackAngle = 90f;
    [SerializeField] private float pushForce = 12f;

    [Header("VFX Settings")]
    [SerializeField] private VFXPlayer vfxPlayer;
    [Tooltip("The vfx will start playing this many seconds before or after the attack logic")]
    [SerializeField] private float vfxOffsetInSeconds;

    [Header("SFX Settings")] 
    [SerializeField] private AK.Wwise.Event attackAudioEvent;

    [Header("Visual Settings")]
    [SerializeField, Min(12)] private int arcSegments = 40;
    [SerializeField] private float lineWidth = .05f;
    [SerializeField] private Color visualColor = Color.magenta;
    [SerializeField] private Color visualColorAttack = Color.white;
    [SerializeField] private AnimationCurve attackColorCrossFadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField, Min(0)] private float attackColorCrossFadeDuration = .3f;
    
    private LineRenderer _lineRenderer;
    private CircleCollider2D _collider;
    private Coroutine _colorCrossFadeRoutine;
    private float _nextVFX = 0f;
    
    private IEnumerator DoArcColorCrossFade() {
        _lineRenderer.startColor = visualColorAttack;
        _lineRenderer.endColor = visualColorAttack;

        float timer = 0f;

        while (timer < attackColorCrossFadeDuration) {
            float t = attackColorCrossFadeCurve.Evaluate(timer / attackColorCrossFadeDuration);
            
            Color current = Color.Lerp(visualColorAttack, visualColor, t);

            _lineRenderer.startColor = current;
            _lineRenderer.endColor = current;

            yield return null;
            timer += Time.deltaTime;
        }
        
        _lineRenderer.startColor = visualColor;
        _lineRenderer.endColor = visualColor;

        _colorCrossFadeRoutine = null;
    }
    
    /// <summary>
    /// Converts an angle in degrees into a direction vector
    /// </summary>
    private Vector3 DirFromAngle(float angle) {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }

    /// <summary>
    /// Draws an outline representing the area of effect of the attack.
    /// </summary>
    private void DrawAttackAreaOutline() {
        if (!_lineRenderer) return;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        float startAngle = aimAngle - attackAngle / 2f;

        bool fullCircle = Mathf.Approximately(attackAngle, 360f);
        
        Vector3[] positions = new Vector3[fullCircle ? arcSegments + 1 : arcSegments + 2];
        if (!fullCircle) {
            positions[0] = Vector3.zero;
        }
        
        int loopCount = fullCircle ? arcSegments : arcSegments + 1;
        for (int i = 0; i < loopCount; i++) {
            float currentAngle = startAngle + (fullCircle ? (360f / arcSegments) : (attackAngle / arcSegments)) * i;
            positions[(fullCircle ? i : i + 1)] = DirFromAngle(currentAngle) * currentStats.attackSize;
        }
        positions[fullCircle ? arcSegments : arcSegments + 1] = fullCircle ? positions[0] : Vector3.zero;
        
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.loop = fullCircle;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.SetPositions(positions);
    }

    private void HandleAttackVFX() {
        if (Time.time < _nextVFX) return;
        _nextVFX = float.MaxValue;

        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        vfxPlayer.PlayVFX(transform.position, Quaternion.Euler(0f, 0f, aimAngle));
    }

    private void HandleAttack() {
        if (Time.time < NextAttack) return;
        NextAttack = Time.time + currentStats.attackRateInSeconds;
        _nextVFX = NextAttack + vfxOffsetInSeconds;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            currentStats.attackSize * transform.lossyScale.x,
            targetLayer
        );

        bool fullCircle = Mathf.Approximately(attackAngle, 360f);

        foreach (var col in hits) {
            if (!col.TryGetComponent<IDamageable>(out var damageable))
                continue;

            Vector2 toTarget = (col.transform.position - transform.position);
            Vector2 pushDir = toTarget.normalized;

            if (!fullCircle) {
                float angleToEnemy = Vector2.Angle(aimDirection, pushDir);
                if (angleToEnemy > attackAngle / 2f)
                    continue;
            }
            
            damageable.TakeDamage(currentStats.attackDamage);
            if (col.TryGetComponent<IPushable>(out var pushable)) {
                if (pushForce < 0.1f) return; // Don't bother it if it's so small
                pushable.RequestPush(pushDir, pushForce);
            }
        }

        attackAudioEvent?.Post(gameObject);
        
        if (_colorCrossFadeRoutine != null)
            StopCoroutine(_colorCrossFadeRoutine);
        _colorCrossFadeRoutine = StartCoroutine(DoArcColorCrossFade());
    }

    private void Update() {
        DrawAttackAreaOutline();

        HandleAttackVFX();
        HandleAttack();
    }

    private void Start() {
        aimDirection = Vector2.right;
    }
    
    protected override void OnAwake() {
        if (!DebugTools.GetOptionalComponent(gameObject, out _lineRenderer)) {
            return;
        }
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = visualColor;
        _lineRenderer.endColor = visualColor;
        
        if (!DebugTools.GetRequiredComponent(gameObject, out _collider)) {
            return;
        }
        _collider.isTrigger = true;
        _collider.radius = currentStats.attackSize;
    }
}
