using UnityEngine;

namespace GameJam.BB2018
{
    public class Sludge : MonoBehaviour
    {
        public AudioClip[] sludgeIdleSounds;
        public AudioClip[] sludgeMoveSounds;
        public int worth;
        public float maxIdleSoundDelay;
        public float movementTolerace;      // amount of position change that constitutes movement (notwithstanding rigidbody velocity)
        public float minDistance = .3f, maxDistance = 1f;
        public float minSpeed = 1f, maxSpeed = 2f;
        public float maxRotation = 5f;

        protected AudioSource _audioSource;
        protected Vector3 _oldPosition;
        protected float _nextIdleSoundTime;
        protected Material _sludgeMat;
        protected Transform _playerSwing;
        protected float _scale;
        protected Transform[] _waypoints;
        protected int _waypointIndex = 0;
        protected float _speed;
        protected Quaternion _rotation;

        protected readonly Vector4 DEFAULT_MORPH = new Vector4(.0001f, 0, 0, 0);

        public virtual void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _nextIdleSoundTime = Time.time + Random.Range(maxIdleSoundDelay * 0.1f, maxIdleSoundDelay);
            _sludgeMat = GetComponent<Renderer>().material;
            _scale = transform.lossyScale.x;
            CustomStart();
        }

        public void Update()
        {
            /*
            if ((transform.position - oldPosition).magnitude > movementTolerace) 
            {
                SoundManager.instance.PlayLocalSoundFx (sludgeMoveSounds [Random.Range (0, sludgeMoveSounds.Length)], audioSource);
            } 
            else if (Time.time > nextIdleSoundTime) 
            {
                SoundManager.instance.PlayLocalSoundFx (sludgeIdleSounds [Random.Range (0, sludgeIdleSounds.Length)], audioSource);
                nextIdleSoundTime = Time.time + Random.Range (maxIdleSoundDelay * 0.1f, maxIdleSoundDelay);
            }
            oldPosition = transform.position;
            */

            if (_playerSwing)
            {
                Vector3 delta = _playerSwing.position - transform.position;
                float distance = delta.magnitude;
                if (distance > minDistance * _scale)
                {
                    _sludgeMat.SetVector("_Morph", new Vector4(delta.x, delta.y, delta.z, 0));
                    _sludgeMat.SetFloat("_Transition", Mathf.Lerp(7, -7, (distance - minDistance * _scale) / (maxDistance - minDistance) * _scale));
                }
                else
                {
                    Eaten();
                }
            }
            else
            {
                _sludgeMat.SetVector("_Morph", DEFAULT_MORPH);
            }
            Movement();
        }

        private void OnTriggerEnter(Collider other)
        {
            Swing otherSwing = other.GetComponent<Swing>();
            if (otherSwing)
            {
                _playerSwing = otherSwing.gameObject.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Swing otherSwing = other.GetComponent<Swing>();
            if (otherSwing)
            {
                _playerSwing = null;
            }
        }

        public AudioSource AudioSource
        {
            get { return _audioSource; }
        }

        protected virtual void CustomStart()
        {
            GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("SludgeWaypoints");
            _waypoints = new Transform[waypointObjects.Length];

            for (int i = 0; i < _waypoints.Length; i++)
            {
                _waypoints[i] = waypointObjects[i].transform;
            }

            _waypointIndex = (int)(Random.Range(0, _waypoints.Length - 1));
            _speed = Random.Range(minSpeed, maxSpeed);
            _rotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * maxRotation);
        }

        protected virtual void Eaten()
        {
            Destroy(gameObject);
            _playerSwing.GetComponent<Swing>().playerHand.SludgeAbsorb(this);
        }

        protected virtual void Movement()
        {
            Vector3 waypointDelta = _waypoints[_waypointIndex].position - transform.position;
            transform.position += _speed * Time.deltaTime * waypointDelta.normalized;
            transform.rotation = _rotation * transform.rotation;

            if (waypointDelta.magnitude < .06f)
            {
                _waypointIndex = (int)(Random.Range(0, _waypoints.Length - 1));
                _speed = Random.Range(minSpeed, maxSpeed);
                _rotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * maxRotation);
            }
        }
    }
}