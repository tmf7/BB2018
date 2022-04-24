using UnityEngine;

namespace GameJam.BB2018
{
    [RequireComponent(typeof(ReflectionProbe))]
    public class ScaleProbeRange : MonoBehaviour
    {
        private ReflectionProbe _targetProbe;
        private Vector3 _originalSize;
        private float _prevScale;
        private float _originalScale;

        private void Start()
        {
            _targetProbe = GetComponent<ReflectionProbe>();
            _originalSize = _targetProbe.size;
            _originalScale = transform.lossyScale.x;
            _prevScale = _originalScale;
        }

        private void Update()
        {
            if (_prevScale != transform.lossyScale.x)
            {
                _prevScale = transform.lossyScale.x;
                _targetProbe.size = _originalSize * _prevScale / _originalScale;
            }
        }
    }
}
