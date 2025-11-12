using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour {
    [Header("Optional References")]
    [SerializeField] protected Image fillImage;
    [SerializeField] protected TMP_Text percentageText;
    
    [Header("Event Listeners")]
    [SerializeField] private FloatEventChannelSO setMaxValueListener;
    [SerializeField] private FloatEventChannelSO setCurrentValueListener;

    protected float MaxValue;
    protected float CurrentValue;

    protected virtual void OnValueUpdated() {
        if (!fillImage) return;
        fillImage.fillAmount = Mathf.Clamp01(CurrentValue / MaxValue);
        
        if (percentageText) {
            percentageText.text = $"{Mathf.Round((CurrentValue / MaxValue) * 100f)}%";
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void SetMaxValue(float amount) {
        if (amount <= 0) {
            Debug.LogWarning($"{name}: Trying to set invalid max value: {amount}");
            return;
        }
        MaxValue = amount;
        SetCurrentValue(amount);
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void SetCurrentValue(float amount) {
        if (MaxValue <= 0) {
            Debug.LogWarning($"{name}: Trying to update value of progress bar with unset max value!");
            return;
        }

        if (amount < 0) {
            amount = 0;
        }

        if (amount > MaxValue) {
            amount = MaxValue;
        }
        
        CurrentValue = amount;
        OnValueUpdated();
    }

    protected virtual void OnValidated() { }
    
    private void OnValidate() {
        if(fillImage)
            fillImage.type = Image.Type.Filled;
        OnValidated();
    }

    private void Awake() {
        if (setMaxValueListener) {
            setMaxValueListener.OnEventRaised += SetMaxValue;
        }
        if (setCurrentValueListener) {
            setCurrentValueListener.OnEventRaised += SetCurrentValue;
        }
    }

    private void OnDestroy() {
        if (setMaxValueListener) {
            setMaxValueListener.OnEventRaised -= SetMaxValue;
        }
        if (setCurrentValueListener) {
            setCurrentValueListener.OnEventRaised -= SetCurrentValue;
        }
    }
}
