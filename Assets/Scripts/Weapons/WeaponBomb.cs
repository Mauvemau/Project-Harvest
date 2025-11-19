using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(MeshFilter), typeof(MeshRenderer))]
public class WeaponBomb : Weapon {
    [Header("References")] 
    [SerializeField] private Scanner scannerReference;
    
    [Header("Weapon Specific Settings")]
    [SerializeField] private bool killParentOnExplode = false;

    [Header("Visual Settings")]
    [SerializeField] private Color attackOutlineColor = Color.red;
    [SerializeField] private Color fillColor = Color.red;
    [SerializeField, Range(16, 128)] private int circleSegments = 16;
    [SerializeField, Min(0.1f)] private float bombBeepRate = 1f;
    [SerializeField, Min(0.1f)] private float alphaFadeDuration = 0.15f;
    [SerializeField] private AnimationCurve beepDetonationCurve =  AnimationCurve.Linear(0, 0, 1, 1);

    [Header("SFX Settings")] 
    [SerializeField] private AK.Wwise.Event beepingSfxEvent;
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private GameObject[] currentOverlaps;

    private LineRenderer _lr;
    private MeshFilter _mf;
    private MeshRenderer _mr;
    
    private bool _shouldDetonate = false;
    private float _lastBeepTime;
    private float _detonationStartTime;
    private float _detonationDuration;
    
    private bool _isBeeping = false;
    private float _beepProgress = 0f;
    private float _currentFadeDuration = 0.15f;
    
    public override void PerformManualAttack() => HandleAttack();
    
    private void Detonate() {
        _shouldDetonate = false;
        if (!scannerReference) return;
        
        currentOverlaps = scannerReference.GetAllOverlaps();
        foreach (GameObject other in currentOverlaps) {
            if (other.transform == transform.parent) continue;
            if (!other.TryGetComponent(out IDamageable damageable)) continue;
            damageable.TakeDamage(BaseStats.attackDamage);
        }

        if (!killParentOnExplode) return;
        GameObject parent = transform.parent.gameObject;
        if (!parent) return;
        if (!parent.TryGetComponent(out IDamageable parentDamageable)) return;
        parentDamageable.Kill();
    }
    
    [ContextMenu("Debug - Detonate Manually")]
    private void HandleAttack() {
        if (_shouldDetonate) return;
        NextAttack = Time.time + currentStats.attackRateInSeconds;
        _shouldDetonate = true;
        
        _detonationStartTime = Time.time;
        _detonationDuration = currentStats.attackRateInSeconds;
    }
    
    private void AdjustAttackRadius() {
        if (!scannerReference) return;
        scannerReference.SetRadius(BaseStats.attackSize);
    }
    
    private void UpdateBeep() {
        if (!_isBeeping || !_mr) return;

        _beepProgress += Time.deltaTime / _currentFadeDuration;
        float alpha = Mathf.Sin(Mathf.Clamp01(_beepProgress) * Mathf.PI);
        Color c = fillColor;
        c.a = alpha * fillColor.a;
        _mr.material.color = c;

        if (_beepProgress >= 1f) {
            _isBeeping = false;
            c.a = 0f;
            _mr.material.color = c;
        }
    }
    
    private void StartBeep(float duration) {
        _isBeeping = true;
        _beepProgress = 0f;
        _currentFadeDuration = Mathf.Min(alphaFadeDuration, duration);
    }

    
    private void HandleBeeping() {
        float elapsed = Time.time - _lastBeepTime;
        float interval = bombBeepRate;

        if (_shouldDetonate) {
            float t = Mathf.Clamp01((Time.time - _detonationStartTime) / _detonationDuration);
            float speedFactor = Mathf.Max(0.05f, beepDetonationCurve.Evaluate(1f - t));
            interval *= speedFactor;
        }

        if (elapsed >= interval) {
            _lastBeepTime = Time.time;
            StartBeep(interval);
            beepingSfxEvent?.Post(gameObject);
        }
    }
    
    private void DrawAttackFill() {
        if (!_mf) return;
        
        float fullCircleAngle = 360f;
        float startAngle = 0f;
        
        int segments = circleSegments;
        
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        
        vertices[0] = Vector3.zero;
        
        for (int i = 0; i <= segments; i++) {
            float currentAngle = startAngle + (fullCircleAngle / segments) * i;
            float rad = currentAngle * Mathf.Deg2Rad;
            
            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * BaseStats.attackSize;
        }
        
        for (int i = 0; i < segments; i++) {
            triangles[i * 3] = 0;        
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        
        Mesh mesh = new Mesh {
            vertices = vertices,
            triangles = triangles
        };
        mesh.RecalculateNormals();

        _mf.mesh = mesh;
    }
    
    private void DrawRangeCircle() {
        if (!_lr) return;
        int segments = circleSegments;
        _lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++) {
            float angle = (360f / segments) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * BaseStats.attackSize;
            _lr.SetPosition(i, point);
        }
    }
    
    private void Update() {
        DrawRangeCircle();
        DrawAttackFill();
        
        HandleBeeping();
        UpdateBeep();
        
        if (!_shouldDetonate) return;
        if (Time.time < NextAttack) return;
        Detonate();
    }
    
    protected override void OnAwake() {
        if (!scannerReference) {
            Debug.LogWarning($"{name}: Missing required {nameof(Scanner)} reference!");
        }
        if (!TryGetComponent<LineRenderer>(out var lr)) {
            Debug.LogWarning($"{name}: Missing required {nameof(LineRenderer)} reference!");
        }
        if (!TryGetComponent<MeshFilter>(out var mf)) {
            Debug.LogWarning($"{name}: Missing required {nameof(MeshFilter)} reference!");
        }
        if (!TryGetComponent<MeshRenderer>(out var mr)) {
            Debug.LogWarning($"{name}: Missing required {nameof(MeshRenderer)} reference!");
        }
        _lr = lr;
        _lr.positionCount = 0;
        _lr.startWidth = 0.05f;
        _lr.endWidth = 0.05f;
        _lr.useWorldSpace = false;
        _lr.material = new Material(Shader.Find("Sprites/Default"));
        _lr.startColor = attackOutlineColor;
        _lr.endColor = attackOutlineColor;

        _mf = mf;
        _mr = mr;
        _mr.material = new Material(Shader.Find("Sprites/Default")) {
            color = new Color(fillColor.r, fillColor.g, fillColor.b, 0f)
        };
    }

    private void OnValidate() {
        AdjustAttackRadius();
    }
    
    private void OnEnable() {
        AdjustAttackRadius();
        _shouldDetonate = false;
        _lastBeepTime = 0f;
        _detonationStartTime = 0f;
        _detonationDuration = 0f;
        _isBeeping = false;
        _beepProgress = 0f;
        _currentFadeDuration = 0.15f;

        if (!_mr) return;
        Color c = fillColor;
        c.a = 0f;
        _mr.material.color = c;
    }
}
