using UnityEngine;

namespace GameJam.BB2018
{
    public class TubeController : MonoBehaviour
    {
        public Transform originalTranform;

        private Transform _originalParent;
        private SteamVR_TrackedController _controller = null;

        private void Start()
        {
            _originalParent = transform.parent;
        }

        private void Update()
        {
            if (_controller && _controller.triggerPressed && transform.parent == _originalParent)
            {
                transform.parent = _controller.gameObject.transform;
            }
            else if ((!_controller || _controller && !_controller.triggerPressed) && transform.parent != _originalParent)
            {
                transform.position = originalTranform.position;
                transform.rotation = originalTranform.rotation;
                transform.parent = _originalParent;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BlobSwinger>())
            {
                _controller = other.gameObject.transform.parent.gameObject.GetComponent<SteamVR_TrackedController>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<BlobSwinger>())
            {
                _controller = null;
            }
        }
    }
}