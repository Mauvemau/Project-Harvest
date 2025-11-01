using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IControllableCamera {
    [Header("Movement Settings")] 
    [SerializeField] private GameObject targetReference;

    [Header("Collision Settings")]
    [SerializeField] private GameObject mapBoundsReference;

    private Bounds _mapBounds;
    private Camera _cam;

    public Vector3 GetScreenToWorldPoint(Vector2 mousePosition) => _cam.ScreenToWorldPoint(mousePosition);
    public Camera GetCameraReference() => _cam;

    private void LateUpdate() {
        if (!targetReference || !_cam) return;

        Vector3 targetPos = targetReference.transform.position;
        Vector3 newPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        
        if (mapBoundsReference) {
            float verticalExtent = _cam.orthographicSize;
            float horizontalExtent = verticalExtent * _cam.aspect;

            float minX = _mapBounds.min.x + horizontalExtent;
            float maxX = _mapBounds.max.x - horizontalExtent;
            float minY = _mapBounds.min.y + verticalExtent;
            float maxY = _mapBounds.max.y - verticalExtent;

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        }

        transform.position = newPos;
    }
    
    private void Awake() {
        if (!TryGetComponent(out _cam)) return;
        ServiceLocator.SetService<IControllableCamera>(this);
        
        if (!mapBoundsReference) return;
        Renderer r = mapBoundsReference.GetComponent<Renderer>();
        if (r != null) {
            _mapBounds = r.bounds;
        } else {
            Debug.LogError("Map bounds reference must have a Renderer component!");
        }
    }
}