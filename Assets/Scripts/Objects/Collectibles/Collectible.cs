using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public abstract class Collectible : MonoBehaviour, ICollectable {
    [Header("Collect Animation Settings")]
    [SerializeField] private float collectAnimationDuration = .2f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("SFX Settings")] 
    [SerializeField] private AK.Wwise.Event collectSfxEvent;

    private bool _collected = false;

    IEnumerator PerformCollectAnimation(Vector3 startPoint, Vector3 endPoint) {
        float timeElapsed = 0f;

        while (timeElapsed < collectAnimationDuration) {
            float t = timeElapsed / collectAnimationDuration;
            float curvedT = animationCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPoint, endPoint, curvedT);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPoint;
        collectSfxEvent?.Post(gameObject);
        OnCollect();
        gameObject.SetActive(false);
    }

    protected virtual void OnCollect() { }

    public void Collect(GameObject collector) {
        if (_collected) return;
        _collected = true;
        
        if (!collector) return;
        StartCoroutine(PerformCollectAnimation(transform.position, collector.transform.position));
    }

    private void OnEnable() {
        _collected = false;
    }

    private void OnValidate() {
        if (!TryGetComponent(out CircleCollider2D col)) {
            Debug.LogError($"{name}: missing reference \"{nameof(col)}\"");
            return;
        }
        col.isTrigger = true;
    }
}
