using UnityEngine;

[System.Serializable]
public class VFXPreset {
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField, Min(0.1f)] private float playbackSpeed = 1f;
    [SerializeField, Range(0f, 1f)] private float animationStartOffset = 0f;
    [SerializeField] private int drawOrder = 2;
    [SerializeField] private Color color = Color.white;

    public RuntimeAnimatorController Controller => controller;
    public float PlaybackSpeed => playbackSpeed;
    public float AnimationStartOffset => animationStartOffset;
    public int DrawOrder => drawOrder;
    public Color Color => color;
}
