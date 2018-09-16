using UnityEngine;

public class FaceCamera : MonoBehaviour {

    Camera cameraToLookAt;

    void Start() {
        cameraToLookAt = Camera.main;
    }

    void LateUpdate() {
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }

} // EnemyUI
