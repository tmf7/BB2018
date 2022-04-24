using UnityEngine;

namespace GameJam.BB2018
{
    public class MenuController : MonoBehaviour
    {
        public void TurnOn()
        {
            gameObject.SetActive(true);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}