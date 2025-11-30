using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EnableOnButtonSelected : MonoBehaviour {
    [Header("References")]
    [SerializeField] private GameObject objectToEnable;

    private Button _button;

    private void Update() {
        UpdateVisibility();
    }

    private void UpdateVisibility() {
        objectToEnable.SetActive(_button.interactable);
    }
    
    private void Awake() {
        if (!TryGetComponent(out _button)) {
            Debug.LogError($"{name}: missing required component; {nameof(Button)}!");
        }
    }
    
    private void OnEnable() {
        UpdateVisibility();
    }
}

