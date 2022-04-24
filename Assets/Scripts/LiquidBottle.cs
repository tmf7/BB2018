using UnityEngine;

namespace GameJam.BB2018
{
    //[ExecuteInEditMode]
    public class LiquidBottle : MonoBehaviour
    {
        [Range(0, 1)] public float fillLevel = .5f;
        public Transform liquidDirection;
        public string shaderName = "Custom/LiquidBottle";
        public float angleLimit = 90;
        public float angleCurveExponent = 2;

        [HideInInspector] public Material liquidMaterial;

        private Renderer _renderer;
        private Transform _liquidDirectionBottom;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();

            foreach (Material mat in _renderer.materials)
            {
                if (mat.shader == Shader.Find(shaderName))
                {
                    liquidMaterial = mat;
                    break;
                }
            }
            if (liquidDirection)
            {
                _liquidDirectionBottom = liquidDirection.GetChild(0);
                _liquidDirectionBottom.parent = transform.parent.parent;
            }
        }

        private void Update()
        {
            if (liquidMaterial)
            {
                Bounds bottleBounds = _renderer.bounds;
                liquidMaterial.SetVector("_Center", new Vector4(
                    bottleBounds.center.x, bottleBounds.center.y, bottleBounds.center.z));
                liquidMaterial.SetFloat("_FillOffset", bottleBounds.size.y * (fillLevel - .5f));
                if (liquidDirection)
                {
                    Vector3 direction = liquidDirection.position - _liquidDirectionBottom.position;
                    float newAngle = Vector3.Angle(Vector3.up, direction) / 180;
                    newAngle = Mathf.Pow(newAngle, angleCurveExponent);
                    newAngle *= angleLimit;

                    Vector3 y = Vector3.Cross(Vector3.up, direction).normalized;
                    Vector3 x = Vector3.Cross(y, Vector3.up).normalized;

                    direction = Vector3.up * Mathf.Cos(Mathf.Deg2Rad * newAngle)
                            + x * Mathf.Sin(Mathf.Deg2Rad * newAngle);

                    direction = direction.normalized;
                    liquidMaterial.SetVector("_FillVector", new Vector4(
                        direction.x, direction.y, direction.z));
                }
            }
        }
    }
}