using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

	public AudioClip[] footstepSounds;
	public AudioClip[] tauntSounds;
	public AudioClip[] hurtSounds;
	public AudioClip flameThrowerSound;

	public Transform bossFightStart;

	public float maxHealth;
	public float vulnerability;		// damage a virus hit does
	public float staggerChance;

	public Transform[] patrolWaypoints;
	public Transform[] fightWaypoints;
	public float turnSpeed;
	public float runSpeed;
	public float walkSpeed;

	private enum BossStates {
		BOSS_PATROL,
		BOSS_INTRO,
		BOSS_FIGHT,
		BOSS_DEAD
	};
	private BossStates bossState = BossStates.BOSS_PATROL;					// first seen patrolling

	private ParticleSystem[] flameThowerParticles;
	private AudioSource audioSource;
	private Animator animator;

	// animator triggers
	private int introHash 		= Animator.StringToHash( "Intro" );
	private int attackHash		= Animator.StringToHash( "Attack" );
	private int staggerHash 	= Animator.StringToHash( "Stagger" );
	private int tauntHash 		= Animator.StringToHash ("Taunt");
	private int dieHash 		= Animator.StringToHash ("Die");
	private int patrolHash		= Animator.StringToHash( "Patrol");
	private int idleHash		= Animator.StringToHash( "Idle");

	private Transform playerTransform;
	private Transform currentWaypointTarget;
	private Transform oldWaypointTarget;
	private float moveSpeed;
	private bool atWaypoint = false;										// positioned off its first waypoint
	private bool taunting = false;
    [HideInInspector]
	public bool attacking = false;
	private bool zeroingIn = false;

	public float currentHealth;

	void Start () {
		flameThowerParticles = GetComponentsInChildren<ParticleSystem> ();
		audioSource = GetComponent<AudioSource> ();
		animator = GetComponent<Animator> ();
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;				// find and track the player
		currentHealth = maxHealth;
		moveSpeed = walkSpeed;
		currentWaypointTarget = patrolWaypoints [Random.Range (0, patrolWaypoints.Length)];		// start off patrolling
	}

	// state handling
	private bool derp = false;
	void Update () {
		/*if (!derp && Input.GetKeyDown (KeyCode.Space)) {
			print ("space!");
			MoveToIntro ();
			derp = true;
			return;
		}*/

		/*if (Input.GetKeyDown (KeyCode.H)) {
			currentHealth -= vulnerability;

			if (currentHealth <= 0) {
				animator.SetTrigger (dieHash);
				bossState = BossStates.BOSS_DEAD;
			} else if (Random.Range (0.0f, 1.0f) >= 0.5) {
				animator.SetTrigger (staggerHash);
			}
		}*/
			
		switch (bossState) {
			case BossStates.BOSS_PATROL: {
				if (!atWaypoint)
					MoveToWaypoint ();
				else
					moveSpeed = 0.0f;
				break;
			}

			case BossStates.BOSS_FIGHT: {
				if (!atWaypoint) {
					MoveToWaypoint ();		// This happens right after intro
				} else { 
					Vector3 playerDirection = (playerTransform.position - transform.position).normalized;

					bool facingPlayer = false;
					// only try the raycast if the forward is w/in 45 degrees of directly looking at the player
					if (Vector3.Dot(playerDirection, transform.forward) > 0.707f) {
                        //RaycastHit hit;
                        //facingPlayer = (Physics.Raycast (transform.position, playerDirection, out hit, 1000.0f) && hit.collider.transform == playerTransform);
                        facingPlayer = true;
					}
                    //Debug.Log(facingPlayer);
					if (!attacking && facingPlayer) {	
						print ("FACING");
						animator.SetTrigger (attackHash);
						attacking = true;
						zeroingIn = false;
					} else {
						if (!zeroingIn) {
							animator.SetTrigger (patrolHash);
							zeroingIn = true;
						}

						moveSpeed = walkSpeed;
						Quaternion targetRotation = Quaternion.LookRotation (playerDirection, Vector3.up);			// TODO: possible issue with up direction
						Quaternion outputRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
						outputRotation.x = 0.0f;
						outputRotation.z = 0.0f;
						transform.rotation = outputRotation;
					}
				}
				break;
			}

			case BossStates.BOSS_INTRO: { 
				// animation events in Intro set the moveSpeed
				transform.position += transform.forward * moveSpeed * Time.deltaTime;
				break;
			}
				
			case BossStates.BOSS_DEAD: { 
				// just be dead
				break;
			}
		}
	}

	// animation event in intro
	public void IntroHold() {
		moveSpeed = 0.0f;
	}

	// animation event in intro
	public void IntroMove() {
		moveSpeed = walkSpeed;
	}

	public void ShootFlames() {
		SoundManager.instance.PlayLocalSoundFx (flameThrowerSound, audioSource, true);
		foreach (ParticleSystem p in flameThowerParticles) {
			p.gameObject.SetActive (true);
			p.Play ();
		}
	}

	public void StopFlames () {
		SoundManager.instance.PlayLocalSoundFx (null, audioSource, false);
		foreach (ParticleSystem p in flameThowerParticles) {
			ParticleSystem.MainModule main = p.main;
			main.stopAction = ParticleSystemStopAction.Disable;
			p.Stop (true);
		}
	}

	public void PlayHurtSound() {
		SoundManager.instance.PlayLocalSoundFx (hurtSounds [Random.Range (0, hurtSounds.Length)], audioSource);
	}


	public void PlayFootStepSound () {
		SoundManager.instance.PlayLocalSoundFx (footstepSounds [Random.Range (0, footstepSounds.Length)], audioSource);
	}

	public void PlayTauntSound () {
		SoundManager.instance.PlayLocalSoundFx (tauntSounds [Random.Range (0, tauntSounds.Length)], audioSource);
	}

	// animation event in Taunt
	public void FinishTaunt() {
		print ("NO TAUNT.");
		taunting = false;
	}

	// animation event in Attack
	public void FinishAttack() {
		attacking = false;
		atWaypoint = Random.Range (0, 1) > 0.8f;	// 20% chance of double-attack from same waypoint
		if (!atWaypoint)
			FindNewWaypoint ();
	}

	public void AllowPatrol() {
		atWaypoint = false;
		animator.SetTrigger (patrolHash);			// only start moving again once the idle->patrol transition finishes (atWaypoint = false allow move)
		FindNewWaypoint ();
	}

	// TODO: gamemanager calls this
	public void MoveToIntro() {
		// "teleport" into the elevator
		transform.position = bossFightStart.position;
		transform.rotation = bossFightStart.rotation;
		animator.SetTrigger (introHash);
		bossState = BossStates.BOSS_INTRO;
	}

	// animation event in intro
	public void StartFight() {
		currentWaypointTarget = fightWaypoints [Random.Range (0, fightWaypoints.Length)];
		bossState = BossStates.BOSS_FIGHT;
		atWaypoint = false;
	}

	void MoveToWaypoint() {
		if (bossState == BossStates.BOSS_FIGHT) {
			moveSpeed = runSpeed;
			turnSpeed = 100;
		} else {
			moveSpeed = walkSpeed;
			turnSpeed = 50;
		}

		Vector3 towards = (currentWaypointTarget.position - transform.position).normalized;
		Quaternion targetRotation = Quaternion.LookRotation (towards);
		Quaternion outputRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
		outputRotation.x = 0.0f;
		outputRotation.z = 0.0f;
		transform.rotation = outputRotation;
		Vector3 delta = transform.forward * moveSpeed * Time.deltaTime * Mathf.Pow( Mathf.Clamp01 (Vector3.Dot (transform.forward, towards)), 0.2f);
		delta.Scale (new Vector3 (1, 0, 1));
		transform.position += delta;		// always move forward, the rotation takes care of tracking to the waypoint
	}

	void FindNewWaypoint() {
		oldWaypointTarget = currentWaypointTarget;
		do {
			if (bossState == BossStates.BOSS_FIGHT) {
				currentWaypointTarget = fightWaypoints [Random.Range (0, fightWaypoints.Length)];
			} else {
				currentWaypointTarget = patrolWaypoints [Random.Range (0, patrolWaypoints.Length)];
			}
		} while (currentWaypointTarget == oldWaypointTarget);
	}

	void OnCollisionEnter(Collision collision) {
	}

	// waypoint traversal
	void OnTriggerEnter(Collider collider) {
		if (collider.transform == currentWaypointTarget) {
			atWaypoint = true;
			animator.SetTrigger (idleHash);
		}
        Swing swingObject = collider.gameObject.GetComponent<Swing>();

        if (swingObject && swingObject.canDamage) {
            currentHealth -= vulnerability;

            if (currentHealth <= 0) {
                animator.SetTrigger(dieHash);
                bossState = BossStates.BOSS_DEAD;
            } else if (Random.Range(0.0f, 1.0f) >= staggerChance) {
                animator.SetTrigger(staggerHash);
            }
        }
    }
}
