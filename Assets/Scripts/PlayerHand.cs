﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

	public AudioClip pickupSound;

	public PlayerStats playerToAffect;
	public float pickupVibrationDuration;
	public ushort pickupVibrationStrength;	// [0,65535]
    public float naturalSpring = 51, firmSpring = 300;
    public bool projectile = false;
    public Swing swing;

    private SteamVR_TrackedController _controller;
	private SteamVR_TrackedObject _controllerObject;
    private BlobSwinger blob;
    private Transform blobTransform;
    private AudioSource swingAudioSource;

    private void Start() {
        blob = GetComponentInChildren<BlobSwinger>();
        blobTransform = blob.gameObject.transform;
        swing.playerHand = this;
        swingAudioSource = swing.gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()	{
		_controller = GetComponent<SteamVR_TrackedController>();
		_controllerObject = GetComponent<SteamVR_TrackedObject> ();
    }
		
	void Update() {
        if (_controller.triggerPressed) {
            blob.springStrength = firmSpring;
        } else {
            blob.springStrength = naturalSpring;
        }
        Vector3 deltaBlob = blobTransform.position - transform.position;
	}

	void OnTriggerStay(Collider collider) {
		Sludge sludge = collider.GetComponent<Sludge> ();
		if ( !_controller.triggerPressed || sludge == null)
			return;
    }

    public void SludgeAbsorb(Sludge sludge) {
        SoundManager.instance.PlayLocalSoundFx(pickupSound, swingAudioSource);
        playerToAffect.AddHealth(sludge.worth);
        StartCoroutine(VibrateController());
    }

    IEnumerator VibrateController() {
		float timeRemaining = pickupVibrationDuration;

		while (timeRemaining > 0.0f) {
			SteamVR_Controller.Input ((int)_controllerObject.index).TriggerHapticPulse (pickupVibrationStrength);
			timeRemaining -= Time.deltaTime;
			yield return null;				// TODO: yield return WaitForSeconds( pulseDelay ) for attennuated reverb
		}
	}
} 