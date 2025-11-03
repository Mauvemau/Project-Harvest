using UnityEngine;

/// <summary>
/// Base class for weapons
/// </summary>
public abstract class Weapon : MonoBehaviour, IWeapon {
    [Header("Stats")]
    [field: SerializeField] public WeaponStats BaseStats { get; protected set; }
    [SerializeField, ReadOnly] protected WeaponStats currentStats;

    [Header("Base Settings")]
    [SerializeField] protected LayerMask targetLayer;

    [SerializeField, ReadOnly] protected Vector2 aimDirection;
    protected float NextAttack;

    private void InitWeaponStats() {
        currentStats = BaseStats;
        aimDirection = Vector2.zero;
        NextAttack = 0;
    }

    public virtual void PerformManualAttack() { }
    
    public void AimWeapon(Vector2 direction) {
        aimDirection = direction;
    }

    protected virtual void OnAwake() { }

    private void Awake() {
        InitWeaponStats();

        OnAwake();
    }
}
