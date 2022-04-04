using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sludge : MonoBehaviour {

	public AudioClip[] sludgeIdleSounds;
	public AudioClip[] sludgeMoveSounds;
	public int worth;
	public float maxIdleSoundDelay;
	public float movementTolerace;		// amount of position change that constitutes movement (notwithstanding rigidbody velocity)
    public float minDistance = .3f, maxDistance = 1f;
    public float minSpeed = 1f, maxSpeed = 2f;
    public float maxRotation = 5f;
    protected AudioSource audioSource;
    protected Vector3 oldPosition;
    protected float nextIdleSoundTime;
    protected Material sludgeMat;
    protected Transform playerSwing;
    protected float scale;
    protected Transform[] waypoints;
    protected Vector4 DEFAULT_MORPH = new Vector4(.0001f, 0, 0, 0);
    protected int waypointIndex = 0;
    protected float speed;
    protected Quaternion rotation;

	public virtual void Start() {
		audioSource = GetComponent<AudioSource> ();
		nextIdleSoundTime = Time.time + Random.Range(maxIdleSoundDelay * 0.1f, maxIdleSoundDelay);
        sludgeMat = GetComponent<Renderer>().material;
        scale = transform.lossyScale.x;
        CustomStart();
    }

	public void Update() {
        /*if ((transform.position - oldPosition).magnitude > movementTolerace) {
			SoundManager.instance.PlayLocalSoundFx (sludgeMoveSounds [Random.Range (0, sludgeMoveSounds.Length)], audioSource);
		} else if (Time.time > nextIdleSoundTime) {
			SoundManager.instance.PlayLocalSoundFx (sludgeIdleSounds [Random.Range (0, sludgeIdleSounds.Length)], audioSource);
			nextIdleSoundTime = Time.time + Random.Range (maxIdleSoundDelay * 0.1f, maxIdleSoundDelay);
		}
		oldPosition = transform.position;*/
        if (playerSwing) {
            Vector3 delta = playerSwing.position - transform.position;
            float distance = delta.magnitude;
            if (distance > minDistance * scale) {
                sludgeMat.SetVector("_Morph", new Vector4(delta.x, delta.y, delta.z, 0));
                sludgeMat.SetFloat("_Transition", Mathf.Lerp(7, -7, (distance - minDistance * scale) / (maxDistance - minDistance) * scale));
            } else {
                Eaten();
            }
        } else {
            sludgeMat.SetVector("_Morph", DEFAULT_MORPH);
        }
        Movement();
    }

    private void OnTriggerEnter(Collider other) {
        Swing otherSwing = other.GetComponent<Swing>();
        if (otherSwing) {
            playerSwing = otherSwing.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        Swing otherSwing = other.GetComponent<Swing>();
        if (otherSwing) {
            playerSwing = null;
        }
    }

    public AudioSource AudioSource {
		get { return audioSource; }
	}

    protected virtual void CustomStart() {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("SludgeWaypoints");
        waypoints = new Transform[waypointObjects.Length];
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i] = waypointObjects[i].transform;
        }
        waypointIndex = (int)(Random.Range(0, waypoints.Length - 1));
        speed = Random.Range(minSpeed, maxSpeed);
        rotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * maxRotation);
    }

    protected virtual void Eaten() {
        Destroy(gameObject);
        playerSwing.GetComponent<Swing>().playerHand.SludgeAbsorb(this);
    }

    protected virtual void Movement() {
        Vector3 waypointDelta = waypoints[waypointIndex].position - transform.position;
        transform.position += speed * Time.deltaTime * waypointDelta.normalized;
        transform.rotation = rotation * transform.rotation;
        if (waypointDelta.magnitude < .06f) {
            waypointIndex = (int)(Random.Range(0, waypoints.Length - 1));
            speed = Random.Range(minSpeed, maxSpeed);
            rotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * maxRotation);
        }
    }
}
