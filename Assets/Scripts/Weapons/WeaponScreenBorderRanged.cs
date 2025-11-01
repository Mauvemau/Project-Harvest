using UnityEngine;

public class WeaponScreenBorderRanged : WeaponRanged {
    [Header("ScreenBorder Weapon Settings")]
    [SerializeField] private float offset;
    [SerializeField] private bool left = true, right = true, top = true, bottom = true;

    private IControllableCamera mainCamera;

    private bool TryFindCameraReference() {
        if (mainCamera != null) return true;
        return ServiceLocator.TryGetService(out mainCamera);
    }

    /// <summary>
    /// Camera-aware mode: spawns at random positions on the rectangle edges of the camera view
    /// </summary>
    private Vector3 GetSpawnPositionFromCamera() {
        if (!TryFindCameraReference()) {
            Debug.LogWarning($"{name}: No {nameof(IControllableCamera)} found!");
            return transform.position;
        }

        Camera mainCameraReference = mainCamera.GetCameraReference();
        if (!mainCameraReference || !mainCameraReference.orthographic)
            return transform.position;

        float camHeight = mainCameraReference.orthographicSize * 2f;
        float camWidth = camHeight * mainCameraReference.aspect;

        Vector3 camCenter = mainCameraReference.transform.position;
        float halfWidth = camWidth / 2f;
        float halfHeight = camHeight / 2f;

        System.Collections.Generic.List<int> enabledSides = new();
        if (left) enabledSides.Add(0);
        if (right) enabledSides.Add(1);
        if (top) enabledSides.Add(2);
        if (bottom) enabledSides.Add(3);
        
        if (enabledSides.Count == 0) {
            enabledSides.AddRange(new int[] { 0, 1, 2, 3 });
        }

        int side = enabledSides[Random.Range(0, enabledSides.Count)];

        float x = 0f, y = 0f;
        switch (side) {
            case 0: // left
                x = camCenter.x - halfWidth - offset;
                y = Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight);
                break;
            case 1: // right
                x = camCenter.x + halfWidth + offset;
                y = Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight);
                break;
            case 2: // top
                y = camCenter.y + halfHeight + offset;
                x = Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth);
                break;
            case 3: // bottom
                y = camCenter.y - halfHeight - offset;
                x = Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth);
                break;
        }

        return new Vector3(x, y, 0f);
    }


    protected override void HandleAttack() {
        if (aimDirection.sqrMagnitude < 0.001f) return;
        if (Time.time < NextAttack) return;
        if (!preset) return;
        NextAttack = Time.time + currentStats.attackRateInSeconds;
        Vector3 spawnPos = GetSpawnPositionFromCamera();
        transform.position = spawnPos;

        Vector3 bulletScale = new Vector3(currentStats.attackSize, currentStats.attackSize, 1f);
        GameObject bulletObject = bulletFactory.Create(spawnPos, Quaternion.identity, bulletScale);
        if (!bulletObject.TryGetComponent(out IBullet bullet)) return;

        Shoot(bullet, preset, aimDirection, currentStats.attackDamage, bulletStats);
    }

    protected override void OnAwake() {
        if (!preset) {
            Debug.LogWarning($"{name}: No {nameof(BulletPresetSO)} set!");
        }
    }
}
