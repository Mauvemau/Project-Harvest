using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for tracking internally a reference to a weapon and it's current level
/// </summary>
[System.Serializable]
public class EquippedWeaponTracker {
    public WeaponUpgradePlanSO upgradePlanReference;
    public GameObject weaponReference;
    public int currentLevel = 0;
}

/// <summary>
/// Contains and manages weapons and upgrades
/// </summary>
public class WeaponInventoryManager: MonoBehaviour {
    [Header("Inventory Settings")]
    [SerializeField, Min(0)] private int weaponInventoryLimit = 4;

    [Header("Weapon Settings")] 
    [SerializeField] private Vector3 weaponPositionOffset = Vector3.zero;

    [Header("Debug")]
    [SerializeField, ReadOnly] private List<EquippedWeaponTracker> equippedWeapons;

    private Factory _weaponFactory = new Factory();
    
    public WeaponUpgradePlanSO[] GetEquippedPlans() {
        WeaponUpgradePlanSO[] plans = new WeaponUpgradePlanSO[equippedWeapons.Count];
        for (int i = 0; i < equippedWeapons.Count; i++) {
            plans[i] = equippedWeapons[i].upgradePlanReference;
        }
        return plans;
    }
    
    public int GetPlanLevel(WeaponUpgradePlanSO upgradePlan) {
        foreach (EquippedWeaponTracker tracker in equippedWeapons) {
            if (tracker.upgradePlanReference == upgradePlan) {
                return tracker.currentLevel;
            }
        }

        Debug.LogWarning($"{name}: Requested level for unequipped plan.");
        return -1;
    }
    
    public int WeaponInventoryLimit => weaponInventoryLimit;
    
    /// <summary>
    /// Enacts a weapon upgrade plan, if there's inventory space for it, and spawns the first weapon (level 0) from that plan
    /// </summary>
    public void EquipPlan(WeaponUpgradePlanSO upgradePlan) {
        if (IsPlanEquipped(upgradePlan)) {
            Debug.LogWarning($"{name}: Plan already equipped.");
            return;
        }
        if (equippedWeapons.Count >= weaponInventoryLimit) {
            Debug.LogError($"{name}: Cannot equip upgrade plan; Inventory Limit Exceeded");
            return;
        }
        
        GameObject prefab = upgradePlan.GetWeaponOfLevel(0);
        if (!prefab) {
            Debug.LogError($"{name}: Cannot equip upgrade plan; There are no prefabs in this plan");
            return;
        }
        
        _weaponFactory.SetPrefabToCreate(prefab);
        GameObject weapon = _weaponFactory.Create(transform.position + weaponPositionOffset, Quaternion.identity, Vector3.one, transform);
        EquippedWeaponTracker weaponTracker = new EquippedWeaponTracker {
            upgradePlanReference = upgradePlan,
            weaponReference = weapon,
            currentLevel = 0
        };
        equippedWeapons.Add(weaponTracker);
    }

    /// <summary>
    /// If the weapon plan has more levels, it destroys the currently equipped weapon and spawns one 1 level higher
    /// </summary>
    public void UpgradeWeapon(WeaponUpgradePlanSO upgradePlan) {
        int index = GetEquippedPlanIndex(upgradePlan);
        if (index == -1) {
            Debug.LogError($"{name}: Cannot upgrade; Plan not equipped.");
            return;
        }

        EquippedWeaponTracker tracker = equippedWeapons[index];
        int nextLevel = tracker.currentLevel + 1;

        if (nextLevel >= upgradePlan.UpgradesCount) {
            Debug.LogError($"{name}: Cannot upgrade; Already at max level ({tracker.currentLevel}).");
            return;
        }

        GameObject nextWeapon = upgradePlan.GetWeaponOfLevel(nextLevel);
        _weaponFactory.SetPrefabToCreate(nextWeapon);

        _weaponFactory.Destroy(tracker.weaponReference);

        tracker.weaponReference = _weaponFactory.Create(transform.position + weaponPositionOffset, Quaternion.identity, Vector3.one, transform);
        tracker.currentLevel = nextLevel;
    }
    
    public void ClearInventory() {
        foreach (EquippedWeaponTracker tracker in equippedWeapons) {
            if (tracker.weaponReference != null) {
                _weaponFactory.Destroy(tracker.weaponReference);
            }
        }
        equippedWeapons.Clear();
    }
    
    private int GetEquippedPlanIndex(WeaponUpgradePlanSO plan) {
        for (int i = 0; i < equippedWeapons.Count; i++) {
            if (equippedWeapons[i].upgradePlanReference == plan) {
                return i;
            }
        }
        return -1;
    }
    
    private bool IsPlanEquipped(WeaponUpgradePlanSO plan) {
        return GetEquippedPlanIndex(plan) != -1;
    }
}
