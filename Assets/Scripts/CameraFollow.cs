using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    GameObject player;
    float speed = 100.0f;

    Vector3 hitLocation;

    // Scrollwheel
    [SerializeField] float minFov = 15f;
    [SerializeField] float maxFov = 90f;
    [SerializeField] float sensitivity = 10f;
    float defaultFov;
    float previousFov;
    bool hasScrolled = false;

    ProtectCameraFromWallClip protectCameraFromWallClip;

    // Use this for initialization
    void Start() {
        defaultFov = Camera.main.fieldOfView;
        player = GameObject.FindGameObjectWithTag("Player");
        protectCameraFromWallClip = GetComponent<ProtectCameraFromWallClip>();
    }

    // Called after all Update functions have been called
    void LateUpdate() {
        transform.position = player.transform.position;

        var cameraRotation = new Vector3(0f, Input.GetAxis("Horizontal"), 0f);
        transform.Rotate(cameraRotation * speed * Time.deltaTime);

        
        PlayerZoom();
    }

    void PlayerZoom() {
        float fov = Camera.main.fieldOfView;
        if (!protectCameraFromWallClip.protecting) {
            if ((Input.GetAxis("Mouse ScrollWheel") > 0f) || Input.GetAxis("Mouse ScrollWheel") < 0f) {
                fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
                fov = Mathf.Clamp(fov, minFov, maxFov);
                previousFov = fov;
                hasScrolled = true;
                Camera.main.fieldOfView = fov;
            }
        }

        if (protectCameraFromWallClip.protecting) {
            fov = defaultFov;
            Camera.main.fieldOfView = fov;
        }

        if (!protectCameraFromWallClip.protecting) {
            if (hasScrolled) {
                fov = previousFov;
                //Camera.main.fieldOfView = fov;
                //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, previousFov, Time.deltaTime*3f);
                Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, previousFov, Time.deltaTime*85f);
            }
        }

        print("HasScrolled: " + hasScrolled);
        
        // TODO Fix camera FoV when turning away from wall if previously zoomed
    }

    void CompensateForWalls() {
        var playerTransform = player.transform.position;
        var cameraTransform = Camera.main.transform.position;

        Debug.DrawLine(playerTransform, cameraTransform, Color.cyan);

        RaycastHit wallHit;
        if (Physics.Linecast(playerTransform, cameraTransform, out wallHit)) {
            wallHit.transform.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 0.2f);
        }
    }

} // CameraFollow
