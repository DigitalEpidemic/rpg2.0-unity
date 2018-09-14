using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    //[SerializeField] float minDistance = 1.0f;
    //[SerializeField] float maxDistance = 4.0f;
    //[SerializeField] float smooth = 10.0f;
    //Vector3 dollyDir;
    //[SerializeField] Vector3 dollyDirAdjusted;
    //[SerializeField] float distance;

    void Awake () {
        //dollyDir = transform.localPosition.normalized;
        //distance = transform.localPosition.magnitude;

    }

    // Update is called once per frame
    void Update() {
        Debug.DrawRay(transform.position, Vector3.back * 10f);
        ////Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 10f)) {
            float distance = Vector3.Distance(transform.position, hit.point);
            //print(distance);
            //transform.position = new Vector3();
        }
    }
}
