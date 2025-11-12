using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Light2D))]
public class FlickeringLight : MonoBehaviour {
    [SerializeField, Range(0f, 3f)] private float minIntensity = 0.75f;
    [SerializeField, Range(0f, 3f)] private float maxIntensity = 1.2f;
    [SerializeField, Min(0f)] private float flickerInterval = 0.1f;
    
    private Light2D _lightReference;
    private float _timer;
    
    private void HandleFlickering() {
        if (!_lightReference) return;
        
        _timer += Time.deltaTime;
        if (_timer < flickerInterval) return;
        _timer = 0;

        _lightReference.intensity = Random.Range(minIntensity, maxIntensity);
    }
    
    private void ValidateIntensityLevels() {
        if (!(maxIntensity < minIntensity)) return;
        
        (minIntensity, maxIntensity) = (maxIntensity, minIntensity);
        Debug.LogWarning($"{name}: max intensity must be greater than or equal to min intensity!");
    }
    
    private void Update() {
        HandleFlickering();
    }

    private void Awake() {
        if (!TryGetComponent(out _lightReference)) {
            Debug.LogError($"{name}: missing required component \"{nameof(Light2D)}\"!");
        }
    }

    private void OnValidate() {
        ValidateIntensityLevels();
    }
}
