using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLightRange : MonoBehaviour {
    private float originalRange;
    private Light thisLight;
    private float prevScale;
    private float originalScale;
	// Use this for initialization
	void Start () {
        thisLight = GetComponent<Light>();
        originalRange = thisLight.range;
        originalScale = transform.lossyScale.x;
        prevScale = originalScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (prevScale != transform.lossyScale.x) {
            prevScale = transform.lossyScale.x;
            thisLight.range = originalRange * prevScale / originalScale;
        }
    }
}
