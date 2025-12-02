using UnityEngine;

public class DevelopmentOnlyElement : MonoBehaviour {
    private void Awake() {
        gameObject.SetActive(Debug.isDebugBuild);
    }
}
