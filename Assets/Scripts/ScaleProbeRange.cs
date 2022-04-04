using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleProbeRange : MonoBehaviour {
    private Vector3 originalSize;
    private ReflectionProbe thisProbe;
    private float prevScale;
    private float originalScale;
	// Use this for initialization
	void Start () {
        thisProbe = GetComponent<ReflectionProbe>();
        originalSize = thisProbe.size;
        originalScale = transform.lossyScale.x;
        prevScale = originalScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (prevScale != transform.lossyScale.x) {
            prevScale = transform.lossyScale.x;
            thisProbe.size = originalSize * prevScale / originalScale;
        }
    }
}
