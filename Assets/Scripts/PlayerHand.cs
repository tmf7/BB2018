﻿using System.Collections;
using UnityEngine;

namespace GameJam.BB2018
{
    public class PlayerHand : MonoBehaviour
    {
        public AudioClip pickupSound;
        public PlayerStats playerToAffect;
        public float pickupVibrationDuration;
        [Range(0, ushort.MaxValue)] public ushort pickupVibrationStrength;
        public float naturalSpring = 51;
        public float firmSpring = 300;
        public bool projectile = false;
        public Swing swing;

        private SteamVR_TrackedController _controller;
        private SteamVR_TrackedObject _controllerObject;
        private BlobSwinger _blob;
        private Transform _blobTransform;
        private AudioSource _swingAudioSource;

        private void Start()
        {
            _blob = GetComponentInChildren<BlobSwinger>();
            _blobTransform = _blob.gameObject.transform;
            swing.playerHand = this;
            _swingAudioSource = swing.gameObject.GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _controller = GetComponent<SteamVR_TrackedController>();
            _controllerObject = GetComponent<SteamVR_TrackedObject>();
        }

        private void Update()
        {
            if (_controller.triggerPressed)
            {
                _blob.springStrength = firmSpring;
            }
            else
            {
                _blob.springStrength = naturalSpring;
            }
            Vector3 deltaBlob = _blobTransform.position - transform.position;
        }

        private void OnTriggerStay(Collider collider)
        {
            Sludge sludge = collider.GetComponent<Sludge>();
            if (!_controller.triggerPressed || sludge == null)
            {
                return;
            }
        }

        public void SludgeAbsorb(Sludge sludge)
        {
            SoundManager.PlayLocalSoundFx(pickupSound, _swingAudioSource);
            playerToAffect.AddHealth(sludge.worth);
            StartCoroutine(VibrateController());
        }

        private IEnumerator VibrateController()
        {
            float timeRemaining = pickupVibrationDuration;

            while (timeRemaining > 0.0f)
            {
                SteamVR_Controller.Input((int)_controllerObject.index).TriggerHapticPulse(pickupVibrationStrength);
                timeRemaining -= Time.deltaTime;
                yield return null;              // TODO: yield return WaitForSeconds( pulseDelay ) for attennuated reverb
            }
        }
    }
}