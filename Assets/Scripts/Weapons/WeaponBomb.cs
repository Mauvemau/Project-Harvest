using UnityEngine;

public class WeaponBomb : Weapon {
    [Header("References")] 
    [SerializeField] private Scanner scannerReference;
    
    [Header("Weapon Specific Settings")]
    [SerializeField] private bool killParentOnExplode = false;
    
    [SerializeField, ReadOnly] private GameObject[] currentOverlaps;

    private bool _shouldDetonate = false;
    
    public override void PerformManualAttack() {
        HandleAttack();
    }

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
    
    private void HandleAttack() {
        NextAttack = Time.time + currentStats.attackRateInSeconds;
        _shouldDetonate = true;
    }

    private void AdjustAttackRadius() {
        if (!scannerReference) return;
        scannerReference.SetRadius(BaseStats.attackSize);
    }
    
    private void Update() {
        if (!_shouldDetonate) return;
        if (Time.time < NextAttack) return;
        Detonate();
    }
    
    protected override void OnAwake() {
        if (!scannerReference) {
            Debug.LogWarning($"{name}: Missing required {nameof(Scanner)} reference!");
        }
    }

    private void OnEnable() {
        AdjustAttackRadius();
        _shouldDetonate = false;
    }

    private void OnValidate() {
        AdjustAttackRadius();
    }
}
