using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AOE {
    public Vector2 TargetPoint { get; private set; }
    public float AttackRadius {get; private set;}
    public float DeathTime { get; private set; }

    private readonly float _damage = 0f;
    private readonly float _tickRate = 0f;
    private float _nextTick = 0f;
    
    public AOE(Vector2 targetPoint, float lifeTime, float damage, float attackRadius, float tickRate) {
        TargetPoint = targetPoint;
        if (lifeTime > 0f) {
            DeathTime = Time.time + lifeTime;
        }
        else {
            DeathTime = 0f;
        }
        _damage = damage;
        AttackRadius = attackRadius;
        _tickRate = tickRate;
        _nextTick = Time.time;
    }

    public void Update(Scanner scanner) {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + _tickRate;
        
        scanner.SetFollowParent(false);
        scanner.SetRadius(AttackRadius);
        scanner.SetPosition(TargetPoint, true);

        GameObject[] targets = scanner.GetAllOverlaps();
        foreach (GameObject target in targets) {
            if (!target.TryGetComponent(out IDamageable damageable)) continue;
            damageable.TakeDamage(_damage);
        }
    }
}

public class WeaponAOE : Weapon {
    [Header("References")] 
    [SerializeField] private Scanner scannerReference;

    [Header("Weapon Specific Settings")] 
    [SerializeField] private Vector2 targetAreaSize = Vector2.zero;
    [SerializeField] private float aoeLifeTime = 1f;
    [SerializeField] private float aoeTickRate = 1f;
    
    [Header("Vfx Settings")]
    [SerializeField] private VFXPlayer vfxPlayer;

    [Header("Sfx Settings")] 
    [SerializeField] private AK.Wwise.Event attackSfxEvent;

#if UNITY_EDITOR
    [Header("Visual Settings")] 
    [SerializeField] private Color targetAreaGizmoColor = Color.magenta;

    private const int GizmoCircleSegments = 16;
#endif
    
    private List<AOE> _targetPoints = new List<AOE>();

    private Vector2 GetRandomPointInTargetArea() {
        if (targetAreaSize is { x: < 1, y: < 1 }) {
            return transform.position;
        }

        float halfX = targetAreaSize.x * 0.5f;
        float halfY = targetAreaSize.y * 0.5f;

        float randomX = Random.Range(-halfX, halfX);
        float randomY = Random.Range(-halfY, halfY);
        
        Vector2 localPoint = new Vector2(randomX, randomY);
        
        Vector2 worldPoint = transform.TransformPoint(localPoint);

        return worldPoint;
    }
    
    private void UpdateActiveTargetPoints() {
        foreach (AOE targetPoint in _targetPoints.ToList()) {
            if (Time.time >= targetPoint.DeathTime && targetPoint.DeathTime > 0f) {
                _targetPoints.Remove(targetPoint);
                continue;
            }
            
            targetPoint.Update(scannerReference);
        }
    }

    private void CreateAoePoint() {
        AOE newTargetPoint = new AOE(GetRandomPointInTargetArea(), 
            aoeLifeTime, BaseStats.attackDamage, BaseStats.attackSize, aoeTickRate);
        _targetPoints.Add(newTargetPoint);
        vfxPlayer.PlayVFX(newTargetPoint.TargetPoint, Quaternion.identity, aoeLifeTime);
        attackSfxEvent?.Post(gameObject);
    }
    
    private void HandleAoeSpawning() {
        // If attack rate is 0 we will only create 1 attack
        if (Mathf.Approximately(currentStats.attackRateInSeconds, 0f)) {
            if (_targetPoints.Count == 0) {
             CreateAoePoint();   
            }
            return;
        }
        
        if (Time.time < NextAttack) return;
        NextAttack = Time.time + currentStats.attackRateInSeconds;
        
        CreateAoePoint();
    }
    
    private void Update() {
        HandleAoeSpawning();
        UpdateActiveTargetPoints();
    }

    private void OnDisable() {
        _targetPoints.Clear();
    }
        
#if UNITY_EDITOR
    private void OnDrawTargetCircleGizmos(Vector2 center, float radius) {
        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= GizmoCircleSegments; i++) {
            float angle = i * Mathf.PI * 2f / GizmoCircleSegments;
            Vector3 newPoint = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                0f
            );

            if (i > 0) {
                Gizmos.DrawLine(prevPoint, newPoint);
            }

            prevPoint = newPoint;
        }
    }

    
    private void OnDrawGizmos() {
        Gizmos.color = targetAreaGizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(targetAreaSize.x, targetAreaSize.y, 0));

        foreach (AOE targetPoint in _targetPoints) {
            OnDrawTargetCircleGizmos(targetPoint.TargetPoint, targetPoint.AttackRadius);
        }
    }
#endif
}
