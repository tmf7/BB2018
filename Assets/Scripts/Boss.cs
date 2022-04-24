using UnityEngine;

namespace GameJam.BB2018
{
	public class Boss : MonoBehaviour
	{
		public enum BossStates
		{
			PATROL,
			FIGHT_INTRO,
			FIGHT,
			DEAD
		};

		[SerializeField] private AudioSource sfxAudioSource;
		[SerializeField] private AudioSource voiceAudioSource;
		[SerializeField] private AudioClip[] footstepSounds;
		[SerializeField] private AudioClip conversationSound;
		[SerializeField] private AudioClip[] fightIntroSounds;
		[SerializeField] private AudioClip[] tauntSounds;
		[SerializeField] private AudioClip[] hurtSounds;
		[SerializeField] private AudioClip deathSound;
		[SerializeField] private AudioClip flameThrowerSound;
		[SerializeField] private Transform playerTransform;

		[SerializeField] private Transform bossFightStart;
		[SerializeField] private float maxHealth;
		[SerializeField] private float vulnerability; // damage a virus hit does

		[SerializeField] private Transform[] patrolWaypoints;
		[SerializeField] private Transform[] fightWaypoints;
		[SerializeField] private float turnSpeed;
		[SerializeField] private float runSpeed;
		[SerializeField] private float walkSpeed;
		[SerializeField] private float currentHealth;

		private BossStates _state = BossStates.PATROL; // first seen patrolling
		private ParticleSystem[] _flameThowerParticles;
		private Animator _animator;

		private Transform _currentWaypointTarget;
		private float _moveSpeed;
		private bool _flamethrowerActive = false;
		private bool _atWaypoint = false;
		private bool _attacking = false;
		private bool _taunting = false;
		private bool _staggered = false;

		// animator triggers
		private readonly int _introTriggerParameter = Animator.StringToHash("Intro");
		private readonly int _attackTriggerParameter = Animator.StringToHash("Attack");
		private readonly int _staggerTriggerParameter = Animator.StringToHash("Stagger");
		private readonly int _tauntTriggerParameter = Animator.StringToHash("Taunt");
		private readonly int _dieTriggerParameter = Animator.StringToHash("Die");
		private readonly int _patrolTriggerParameter = Animator.StringToHash("Patrol");
		private readonly int _patrolIdleTriggerParameter = Animator.StringToHash("PatrolIdle");
		private readonly int _runTriggerParameter = Animator.StringToHash("Run");

		private readonly float FACING_THRESHOLD = Mathf.Cos(45.0f * Mathf.Deg2Rad);

		public BossStates State { get { return _state; } }
		public bool Attacking { get { return _flamethrowerActive; } }

		private void Start()
		{
			_flameThowerParticles = GetComponentsInChildren<ParticleSystem>();
			_animator = GetComponent<Animator>();
			currentHealth = maxHealth;
			StartPatrolWalk();
		}

		private void ClearAnimationTriggers()
		{
			_animator.ResetTrigger(_introTriggerParameter);
			_animator.ResetTrigger(_attackTriggerParameter);
			_animator.ResetTrigger(_staggerTriggerParameter);
			_animator.ResetTrigger(_tauntTriggerParameter);
			_animator.ResetTrigger(_dieTriggerParameter);
			_animator.ResetTrigger(_patrolIdleTriggerParameter);
			_animator.ResetTrigger(_runTriggerParameter);
		}

		private void ClearActions()
		{
			_attacking = false;
			_taunting = false;
			_staggered = false;
			StopFlames();
		}

		// primarily called by Fight Intro state animation events
		public void SetMoveSpeed(float moveSpeed)
		{
			_moveSpeed = moveSpeed;
		}

		private void Update()
		{
			switch (_state)
			{
				case BossStates.PATROL:
				{
					if (!_atWaypoint)
					{
						MoveTowardWaypoint();
					}
					break;
				}
				case BossStates.FIGHT_INTRO:
				{
					// fightIntroSpeed is controlled on an animation curve
					// no waypoint needed, just move straight ahead
					transform.position += transform.forward * _moveSpeed * Time.deltaTime;
					break;
				}
				case BossStates.FIGHT:
				{
					if (!_atWaypoint && !_staggered)
					{
						MoveTowardWaypoint();
					}
					else if (MoveToFacePlayer())
					{
						ChooseAttack();
					}
					break;
				}
			}
		}

		public void ShootFlames()
		{
			if (!_flamethrowerActive)
			{
				SoundManager.PlayLocalSoundFx(flameThrowerSound, sfxAudioSource, true);
				foreach (ParticleSystem p in _flameThowerParticles)
				{
					p.gameObject.SetActive(true);
					p.Play();
				}
				_flamethrowerActive = true;
			}
		}

		public void StopFlames()
		{
			if (_flamethrowerActive)
			{
				SoundManager.StopLocalSoundFx(sfxAudioSource);
				foreach (ParticleSystem p in _flameThowerParticles)
				{
					ParticleSystem.MainModule main = p.main;
					main.stopAction = ParticleSystemStopAction.Disable;
					p.Stop(true);
				}
				_flamethrowerActive = false;
			}
		}

		private void StartPatrolWalk()
		{
			ClearAnimationTriggers();
			ClearActions();
			_state = BossStates.PATROL;
			FindNewWaypoint();
		}

		private void StartPatrolIdle()
		{
			ClearAnimationTriggers();
			ClearActions();
			_animator.SetTrigger(_patrolIdleTriggerParameter);
		}

		// animation event in Patrol_Idle
		public void FinishPatrolIdle()
		{
			FindNewWaypoint();
		}

		// called by GameManager
		public void StartFightIntro()
		{
			// "teleport" into the elevator
			ClearAnimationTriggers();
			ClearActions();
			currentHealth = maxHealth;
			transform.position = bossFightStart.position;
			transform.rotation = bossFightStart.rotation;
			PlayFightIntroSound();
			_state = BossStates.FIGHT_INTRO;
			_animator.SetTrigger(_introTriggerParameter);
		}

		// animation event in Intro
		public void StartFight()
		{
			ClearAnimationTriggers();
			ClearActions();
			_state = BossStates.FIGHT;
			turnSpeed = 100;
			FindNewWaypoint();
		}

		private void StartTaunt()
		{
			ClearAnimationTriggers();
			ClearActions();
			PlayTauntSound();
			_animator.SetTrigger(_tauntTriggerParameter);
			_taunting = true;
		}

		// animation event in Taunt
		public void FinishTaunt()
		{
			_taunting = false;
			FindNewWaypoint();
		}

		private void StartAttack()
        {
			ClearAnimationTriggers();
			ClearActions();
			_animator.SetTrigger(_attackTriggerParameter);
			_attacking = true;
		}

		// animation event in Attack
		public void FinishAttack()
		{
			_attacking = false;
			// 20% chance of double-attack from same waypoint
			if (Random.Range(0, 1) <= 0.8f)
			{
				FindNewWaypoint();
			}
		}

		private void StartStagger()
		{
			ClearAnimationTriggers();
			ClearActions();
			PlayHurtSound();
			_staggered = true;
			_animator.SetTrigger(_staggerTriggerParameter);
		}

		// animation event in Stagger
		public void FinishStagger()
		{
			_staggered = false;

			if (currentHealth <= 0.0f)
			{
				StartDeath();
			}
			else
			{
				FindNewWaypoint();
			}
		}

		private void StartDeath()
		{
			ClearAnimationTriggers();
			ClearActions();
			PlayDeathSound();
			_state = BossStates.DEAD;
			_animator.SetTrigger(_dieTriggerParameter);
		}

		private void ChooseAttack()
		{
			if (!_taunting && !_attacking && !_staggered)
			{
				if (Random.value > 0.5f)
				{
					StartTaunt();
				}
				else
				{
					StartAttack();
				}
			}
		}

		private bool MoveToFacePlayer()
		{
			Vector3 playerDirection = (playerTransform.position - transform.position).normalized;
			Quaternion targetRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
			Quaternion outputRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
			outputRotation.x = 0.0f;
			outputRotation.z = 0.0f;
			transform.rotation = outputRotation;

			return Vector3.Dot(playerDirection, transform.forward) > FACING_THRESHOLD;
		}

		private void MoveTowardWaypoint()
		{
			_moveSpeed = _state == BossStates.FIGHT ? runSpeed : walkSpeed;

			Vector3 towards = (_currentWaypointTarget.position - transform.position).normalized;
			Quaternion targetRotation = Quaternion.LookRotation(towards);
			Quaternion outputRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
			outputRotation.x = 0.0f;
			outputRotation.z = 0.0f;
			transform.rotation = outputRotation;
			Vector3 delta = transform.forward * _moveSpeed * Time.deltaTime * Mathf.Pow(Mathf.Clamp01(Vector3.Dot(transform.forward, towards)), 0.2f);
			delta.Scale(new Vector3(1, 0, 1));
			transform.position += delta; // always move forward, the rotation takes care of tracking to the waypoint
		}

		private void FindNewWaypoint()
		{
			Transform[] waypoints = null;
			Transform oldWaypointTarget = _currentWaypointTarget;
			_atWaypoint = false;
			ClearActions();

			if (_state == BossStates.FIGHT)
			{
				_animator.SetTrigger(_runTriggerParameter);
				waypoints = fightWaypoints;
			}
			else
			{
				_animator.SetTrigger(_patrolTriggerParameter);
				waypoints = patrolWaypoints;
			}

			do
			{
				_currentWaypointTarget = waypoints[Random.Range(0, waypoints.Length)];
			} while (_currentWaypointTarget == oldWaypointTarget);
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (_state != BossStates.DEAD)
			{
				CheckWaypointHit(collider);
				CheckTakeDamage(collider);
			}
		}

		private void CheckWaypointHit(Collider collider)
		{
			if (collider.transform == _currentWaypointTarget)
			{
				_atWaypoint = true;
				_moveSpeed = 0.0f;

				if (_state == BossStates.PATROL)
				{
					StartPatrolIdle();
				}
				//else if (_state == BossStates.FIGHT)
				//{
				//	ClearAnimationTriggers();
				//	ClearActions();
				//	_animator.SetTrigger(_patrolTriggerParameter);
				//}
			}
		}

		private void CheckTakeDamage(Collider collider)
		{
			Swing swingObject = collider.GetComponent<Swing>();

			if (_state == BossStates.FIGHT && 
				swingObject != null && 
				swingObject.CanDamage)
			{
				currentHealth -= vulnerability;
				StartStagger();
			}
		}

		public void PlayConversation()
		{
			SoundManager.PlayLocalSoundFx(conversationSound, voiceAudioSource);
		}

		public void PlayFightIntroSound()
		{
			SoundManager.PlayLocalSoundFx(fightIntroSounds[Random.Range(0, fightIntroSounds.Length)], voiceAudioSource);
		}

		public void PlayHurtSound()
		{
			SoundManager.PlayLocalSoundFx(hurtSounds[Random.Range(0, hurtSounds.Length)], voiceAudioSource);
		}

		public void PlayFootStepSound()
		{
			SoundManager.PlayLocalSoundFx(footstepSounds[Random.Range(0, footstepSounds.Length)], sfxAudioSource);
		}

		public void PlayTauntSound()
		{
			SoundManager.PlayLocalSoundFx(tauntSounds[Random.Range(0, tauntSounds.Length)], voiceAudioSource);
		}

		public void PlayDeathSound()
		{
			SoundManager.PlayLocalSoundFx(deathSound, voiceAudioSource);
		}
	}
}