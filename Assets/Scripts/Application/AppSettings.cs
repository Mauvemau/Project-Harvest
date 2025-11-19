using UnityEngine;

[System.Serializable]
public class PlatformSettings {
    [SerializeField] private RuntimePlatform platform;
    [SerializeField, Range(0, 1)] private int vSync = 1;
    [SerializeField, Min(-1)] private int targetFrameRate = -1;

    public RuntimePlatform Platform => platform;
    public int VSync => vSync;
    public int TargetFrameRate => targetFrameRate;
}

public class AppSettings : MonoBehaviour {
    [Header("Per-platform Settings")]
    [SerializeField] private PlatformSettings[] platforms;

    private void InitializePlatform() {
        if (platforms.Length <= 0) return;
        foreach (PlatformSettings platform in platforms) {
            if (Application.platform == platform.Platform) {
                QualitySettings.vSyncCount = platform.VSync;
                Application.targetFrameRate = platform.TargetFrameRate;
            }
        }
    }

    private void Awake() {
        InitializePlatform();
    }
}
