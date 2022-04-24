using UnityEngine;

namespace GameJam.BB2018
{
    public class BeakerController : MonoBehaviour
    {
        public GameManager gameManager;
        public float cooldown = 0.8f;

        private float _cooldownTimeRemaining = 0;

        private void Update()
        {
            if (_cooldownTimeRemaining > 0)
            {
                _cooldownTimeRemaining -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "BottleTip" && _cooldownTimeRemaining <= 0)
            {
                _cooldownTimeRemaining = cooldown;
                gameManager.TouchBottle(other.gameObject.transform.parent.name);
            }
        }
    }
}