using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TMPLinkHandler : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Canvas canvasReference;
    private TMP_Text _tmpText;
    private Camera _cameraReference;

    public delegate void ClickOnEvent(string keyword);
    public static event ClickOnEvent OnClickedOnEvent;
    
    public void OnPointerClick(PointerEventData eventData) {
        if (!_cameraReference) return;

        int linkTaggedText = TMP_TextUtilities.FindIntersectingLink(_tmpText, eventData.position, _cameraReference);

        if (linkTaggedText == -1) return;
        TMP_LinkInfo linkInfo = _tmpText.textInfo.linkInfo[linkTaggedText];
        string linkID = linkInfo.GetLinkID();

        Debug.Log($"{name}: Clicked link: {linkID}");
        HandleURL(linkID);
        OnClickedOnEvent?.Invoke(linkInfo.GetLinkText());
    }

    private void HandleURL(string linkID) {
        if (!linkID.Contains("https://") && !linkID.Contains("www")) return;
        Application.OpenURL(linkID);
    }
    
    private void Start() {
        if (!TryGetComponent(out _tmpText)) {
            Debug.LogError($"{name}: No TMP Text attached");
        }
        if (!canvasReference) {
            Debug.LogError($"{name}: missing required component \"{nameof(Canvas)}\"");
            return;
        }
        
        if (!ServiceLocator.TryGetService(out IControllableCamera mainCamera)) {
            Debug.LogError($"{name}: no camera reference found!");
            return;
        }
        _cameraReference = mainCamera.GetCameraReference();
        canvasReference.renderMode = RenderMode.ScreenSpaceCamera;
        canvasReference.worldCamera = _cameraReference;
    }
}