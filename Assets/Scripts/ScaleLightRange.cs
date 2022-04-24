using UnityEngine;

namespace GameJam.BB2018
{
    [RequireComponent(typeof(Light))]
    public class ScaleLightRange : MonoBehaviour
    {
        private Light _targetLight;
        private float _originalRange;
        private float _prevScale;
        private float _originalScale;

        private void Start()
        {
            _targetLight = GetComponent<Light>();
            _originalRange = _targetLight.range;
            _originalScale = transform.lossyScale.x;
            _prevScale = _originalScale;
        }

        private void Update()
        {
            if (_prevScale != transform.lossyScale.x)
            {
                _prevScale = transform.lossyScale.x;
                _targetLight.range = _originalRange * _prevScale / _originalScale;
            }
        }
    }
}