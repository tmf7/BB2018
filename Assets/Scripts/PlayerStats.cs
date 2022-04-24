using UnityEngine;

namespace GameJam.BB2018
{
	public class PlayerStats : MonoBehaviour
	{
		private float _currentHealth = 1000;

		private void Update()
		{
			if (_currentHealth <= 0)
			{
				_currentHealth = 0;
				// TODO: kill player (reset scene/show static worldspace mainmenu)
			}
		}

		public void AddHealth(float amount)
		{
			_currentHealth += amount;
		}

		public void RemoveHealth(float amount)
		{
			_currentHealth -= amount;
		}

		public float Health
		{
			get { return _currentHealth; }
			set { _currentHealth = value; }
		}
	}
}