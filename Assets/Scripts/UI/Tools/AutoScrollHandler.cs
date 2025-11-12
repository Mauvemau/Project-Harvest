using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class AutoScroll : MonoBehaviour, IBeginDragHandler, IScrollHandler {
    [Header("Auto Scroll Settings")]
    [SerializeField] private float scrollSpeed = 0.1f;
    [SerializeField] private bool vertical = true;

    private ScrollRect _scrollRect;
    private bool _isUserInteracting = false;

    public void OnBeginDrag(PointerEventData eventData) => _isUserInteracting = true;
    public void OnScroll(PointerEventData eventData) => _isUserInteracting = true;
    private void ResumeNow() => _isUserInteracting = false;
    
    private void Update() {
        if (_isUserInteracting || !_scrollRect) return;

        float delta = scrollSpeed * Time.deltaTime;

        if (vertical)
            delta *= -1f;

        if (vertical) {
            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                _scrollRect.verticalNormalizedPosition + delta
            );
        } else {
            _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(
                _scrollRect.horizontalNormalizedPosition + delta
            );
        }
    }

    private void Awake() {
        if (!TryGetComponent(out _scrollRect)) {
            Debug.LogError($"{name}: missing required component \"{nameof(ScrollRect)}\"");
        }
    }

    private void OnEnable() {
        if (!_scrollRect) return;
        _scrollRect.verticalNormalizedPosition = 1;
        ResumeNow();
    }
}

