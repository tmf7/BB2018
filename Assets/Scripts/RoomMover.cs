using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMover : MonoBehaviour {
    public Transform cameraTransform;
    public float targetScale = 1;
    public float scaleChangeAlpha = .2f;
    private float currentScale = 0;
    private Transform roomContainer;
    private Vector3 prevPosition = new Vector3(-999, 0, 0);

    private Vector3 FLOOR_PROJECT = new Vector3(1, 0, 1);
	// Use this for initialization
	void Start () {
        prevPosition = cameraTransform.position;
        roomContainer = transform.GetChild(0);
        currentScale = targetScale;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 delta = (cameraTransform.position - prevPosition);
        delta.Scale(FLOOR_PROJECT);

        transform.position += delta;
        roomContainer.position -= delta;

        prevPosition = cameraTransform.position;
        if (transform.localScale.x != targetScale) {
            currentScale = Mathf.Lerp(currentScale, targetScale, scaleChangeAlpha);
            if (Mathf.Abs(currentScale - targetScale) < .001) {
                currentScale = targetScale;
            }
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        }
	}

    public void Center() {
        Vector3 delta = (cameraTransform.position - transform.position);
        delta.Scale(FLOOR_PROJECT);
        transform.position += delta;
    }
}
