using UnityEngine;

/// <summary>
/// Contains a preset that will define the behaviour and appareance of a bullet
/// </summary>
[CreateAssetMenu(menuName = "Containers/Bullet Preset")]
public class BulletPresetSO : ScriptableObject {
    [Header("Behaviour Settings")]
    [SerializeReference, SubclassSelector] private IBulletStrategy behaviour = new LinearShotStrategy();

    [Header("Audio Settings")] 
    [SerializeField] private AK.Wwise.Event shotAudioEvent;
    [SerializeField] private AK.Wwise.Event hitAudioEvent;
    [SerializeField] private AK.Wwise.Event destroyAudioEvent;
    
    [Header("Visual Settings")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color tint = Color.white;
    
    public IBulletStrategy Behaviour => behaviour;
    public Sprite Sprite => sprite;
    public Color Tint => tint;
    public AK.Wwise.Event ShotAudioEvent => shotAudioEvent;
    public AK.Wwise.Event HitAudioEvent => hitAudioEvent;
    public AK.Wwise.Event DestroyAudioEvent => destroyAudioEvent;
}
