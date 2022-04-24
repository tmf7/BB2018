using UnityEngine;
using UnityEngine.UI;

namespace GameJam.BB2018
{
	public class PlayerHUD : MonoBehaviour
	{
		public PlayerStats playerToWatch;

		[HideInInspector] public GameManager gameManager;

		private Slider _timeSlider;
		private Text[] _timeTexts;
		private Text[] _healthTexts;
		private int _oldSecondsValue;
		private bool _timeIsRed = false;

		private void Start()
		{
			_timeSlider = GetComponentInChildren<Slider>();
			_timeTexts = _timeSlider.GetComponentsInChildren<Text>();
			_healthTexts = GetComponentsInChildren<Text>();
		}

		private void Update()
		{
			// time
			_timeSlider.normalizedValue = 1.0f - gameManager.NomalizedTime;
			int minutes = gameManager.MinutesRemaining();
			int seconds = gameManager.SecondsRemaining();
			string timeString = minutes.ToString() + ":" + (seconds >= 10 ? "" : "0") + seconds.ToString();
			_timeTexts[0].text = timeString;
			_timeTexts[1].text = timeString;

			// blink red
			if (minutes == 0 && seconds != _oldSecondsValue)
			{
				if (!_timeIsRed || seconds == 0)
				{
					_timeIsRed = true;
					_timeTexts[1].color = Color.red;
				}
				else
				{
					_timeIsRed = false;
					_timeTexts[1].color = Color.white;
				}
			}

			_oldSecondsValue = seconds;

			// health
			string healthString = "Strength: " + Mathf.Round(playerToWatch.Health);
			_healthTexts[0].text = healthString;
			_healthTexts[1].text = healthString;
		}
	}
}