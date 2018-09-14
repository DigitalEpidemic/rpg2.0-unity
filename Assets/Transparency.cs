using UnityEngine;

public class Transparency : MonoBehaviour {

    [SerializeField] Material originalMaterial;
    [SerializeField] Material fadeMaterial;

    void OnTriggerStay(Collider collider) {
        if (collider.GetComponent<MeshRenderer>() && collider.CompareTag("Wall")) {
            collider.GetComponent<MeshRenderer>().material = fadeMaterial;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.GetComponent<MeshRenderer>() && collider.CompareTag("Wall")) {
            collider.GetComponent<MeshRenderer>().material = originalMaterial;
        }
    }

} // Transparency
