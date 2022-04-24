using UnityEngine;

namespace GameJam.BB2018
{
    public class BlobSwinger : MonoBehaviour
    {
        public Transform swing;
        public float springStrength = 51;

        private SpringJoint _joint;
        private Material _blobMaterial;
        private Color _originalColor = new Color(1, 1, 1, 0.1764706f);

        private readonly int MAIN_COLOR_PROPERTY = Shader.PropertyToID("_Color");
        private readonly int MORPH_PROPERTY = Shader.PropertyToID("_Morph");
        private readonly int DAMPEN_PROPERTY = Shader.PropertyToID("_Dampen");
        private readonly int TRANSITION_PROPERTY = Shader.PropertyToID("_Transition");

        private Material BlobMaterial
        {
            get
            {
                if (_blobMaterial == null)
                {
                    _blobMaterial = GetComponent<Renderer>().material;
                }
                return _blobMaterial;
            }
        }

        private void Start()
        {
            swing.parent = transform.parent.parent;
            
            _originalColor = BlobMaterial.GetColor(MAIN_COLOR_PROPERTY);
            _joint = swing.gameObject.GetComponent<SpringJoint>();
        }

        private void Update()
        {
            Vector3 delta = swing.position - transform.position;
            BlobMaterial.SetVector(MORPH_PROPERTY, new Vector4(delta.x, delta.y, delta.z, 0));
            _joint.spring = springStrength;
        }

        public void ResetColor()
        {
            BlobMaterial.SetColor(MAIN_COLOR_PROPERTY, _originalColor);
        }

        public void SetColor(Color color)
        {
            BlobMaterial.SetColor(MAIN_COLOR_PROPERTY, color);
        }

        public void SetShrink(bool shrink)
        {
            if (shrink)
            {
                BlobMaterial.SetFloat(DAMPEN_PROPERTY, 1);
                BlobMaterial.SetFloat(TRANSITION_PROPERTY, -7);
                transform.localScale = new Vector3(0.3435692f, 0.3435692f, 0.3435692f) * .4f;
            }
            else
            {
                BlobMaterial.SetFloat(DAMPEN_PROPERTY, .1f);
                BlobMaterial.SetFloat(TRANSITION_PROPERTY, 0);
                transform.localScale = new Vector3(0.3435692f, 0.3435692f, 0.3435692f);
            }
        }
    }
}