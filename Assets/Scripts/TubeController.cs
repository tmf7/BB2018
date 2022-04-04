using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour {
    public Transform originalTranform;
    private Transform originalParent;
    private SteamVR_TrackedController controller = null;

	// Use this for initialization
	void Start () {
        originalParent = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
		if (controller && controller.triggerPressed && transform.parent == originalParent) {
            transform.parent = controller.gameObject.transform;
        } else if ((!controller || controller && !controller.triggerPressed) && transform.parent != originalParent) {
            transform.position = originalTranform.position;
            transform.rotation = originalTranform.rotation;
            transform.parent = originalParent;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<BlobSwinger>()) {
            controller = other.gameObject.transform.parent.gameObject.GetComponent<SteamVR_TrackedController>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<BlobSwinger>()) {
            controller = null;
        }
    }
}
