using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCursor : MonoBehaviour {

    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D enemyCursor;

    [SerializeField] LayerMask enemyLayer;

    CameraRaycaster cameraRaycaster;

    CursorMode cursorMode = CursorMode.Auto;

    void Awake() {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
    }

    void LateUpdate() {
        switch (cameraRaycaster.layerHit) {
            case Layer.Enemy:
                Cursor.SetCursor(enemyCursor, Vector2.zero, cursorMode);
                break;
            default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
                return;
        }
    }

} // HandleCursor
