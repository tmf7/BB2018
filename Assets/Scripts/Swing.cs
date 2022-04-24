using UnityEngine;

namespace GameJam.BB2018
{
    public class Swing : MonoBehaviour
    {
        public float damageDistance = 1;

        [HideInInspector] public PlayerHand playerHand;

        public bool CanDamage
        {
            get { return (transform.position - playerHand.transform.position).magnitude >= damageDistance; }
        }
    }
}