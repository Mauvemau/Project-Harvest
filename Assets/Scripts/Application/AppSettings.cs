using UnityEngine;

public class AppSettings : MonoBehaviour {
    [Header("Desktop Settings")] 
    [SerializeField, Range(0, 1)] private int vSyncSetting = 1; 
    [SerializeField, Min(-1)] private int targetFrameRateDesktop = -1;
    
    [Header("Android Settings")] 
    [SerializeField] private int targetFrameRateAndroid = 60;

    private void Awake() {
#if UNITY_STANDALONE || UNITY_EDITOR
        QualitySettings.vSyncCount = vSyncSetting;
        Application.targetFrameRate = targetFrameRateDesktop;
#elif UNITY_ANDROID
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = targetFrameRateAndroid;
#endif
    }
}
