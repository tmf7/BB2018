using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSwinger : MonoBehaviour {
    public Transform swing;
    public float springStrength = 51;
    [HideInInspector]
    public Material blobMat;
    private SpringJoint joint;

    // Use this for initialization
	void Start () {
        swing.parent = transform.parent.parent;
        blobMat = GetComponent<Renderer>().material;
        joint = swing.gameObject.GetComponent<SpringJoint>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 delta = swing.position - transform.position;
        blobMat.SetVector("_Morph", new Vector4(delta.x, delta.y, delta.z, 0));
        joint.spring = springStrength;
    }

    public void SetShrink(bool shrink) {
        if (shrink) {
            blobMat.SetFloat("_Dampen", 1);
            blobMat.SetFloat("_Transition", -7);
            transform.localScale = new Vector3(0.3435692f, 0.3435692f, 0.3435692f) * .4f;
        } else {
            blobMat.SetFloat("_Dampen", .1f);
            blobMat.SetFloat("_Transition", 0);
            transform.localScale = new Vector3(0.3435692f, 0.3435692f, 0.3435692f);
        }
    }
}
