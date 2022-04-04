using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    public PlayerStats playerToWatch;

    [HideInInspector]
    public GameManager gameManager;

    private Slider timeSlider;
	private Text[] timeTexts;
	private Text[] healthTexts;
	private int oldSecondsValue;
	private bool timeIsRed = false;

	void Start() {
		timeSlider = GetComponentInChildren<Slider> ();
		timeTexts = timeSlider.GetComponentsInChildren<Text> ();
		healthTexts = GetComponentsInChildren<Text> ();
	}

	void Update() {
		// time
		timeSlider.normalizedValue = 1.0f - gameManager.NomalizedTime;
		int minutes = gameManager.MinutesRemaining ();
		int seconds = gameManager.SecondsRemaining();
		string timeString = minutes.ToString () + ":" + (seconds >= 10 ? "" : "0" ) + seconds.ToString ();
		timeTexts [0].text = timeString;
		timeTexts [1].text = timeString;

		// blink red
		if (minutes == 0 && seconds != oldSecondsValue) {
			if (!timeIsRed || seconds == 0) {
				timeIsRed = true;
				timeTexts [1].color = Color.red;
			} else {
				timeIsRed = false;
				timeTexts [1].color = Color.white;
			}
		}

		oldSecondsValue = seconds;

		// health
		string healthString = "Strength: " + Mathf.Round(playerToWatch.Health);
		healthTexts [0].text = healthString;
		healthTexts [1].text = healthString;
	}
}
