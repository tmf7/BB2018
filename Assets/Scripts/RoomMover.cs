using UnityEngine;

namespace GameJam.BB2018
{
    public class RoomMover : MonoBehaviour
    {
        public Transform cameraTransform;
        public float targetScale = 1;
        public float scaleChangeAlpha = .2f;

        private float _currentScale = 0;
        private Transform _roomContainer;
        private Vector3 _prevPosition = new Vector3(-999, 0, 0);

        private readonly Vector3 FLOOR_PROJECT = new Vector3(1, 0, 1);
        
        private void Start()
        {
            _prevPosition = cameraTransform.position;
            _roomContainer = transform.GetChild(0);
            _currentScale = targetScale;
        }

        private void Update()
        {
            Vector3 delta = (cameraTransform.position - _prevPosition);
            delta.Scale(FLOOR_PROJECT);

            transform.position += delta;
            _roomContainer.position -= delta;

            _prevPosition = cameraTransform.position;
            if (transform.localScale.x != targetScale)
            {
                _currentScale = Mathf.Lerp(_currentScale, targetScale, scaleChangeAlpha);
                if (Mathf.Abs(_currentScale - targetScale) < .001)
                {
                    _currentScale = targetScale;
                }
                transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
            }
        }

        public void Center()
        {
            Vector3 delta = (cameraTransform.position - transform.position);
            delta.Scale(FLOOR_PROJECT);
            transform.position += delta;
        }
    }
}