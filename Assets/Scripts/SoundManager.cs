using UnityEngine;
using UnityEngine.Audio;

namespace GameJam.BB2018
{
	public class SoundManager : MonoBehaviour
	{
		private static SoundManager _instance;

		public AudioSource musicSource;
		public AudioSource globalSFxSource;
		public AudioMixer sfxMixer;         // control the volume only

		private float _savedSfxAttenuation = 0.0f;
		private float _savedMusicVolume = 1.0f;

		private const string ATTENUTATION = "Attenuation";

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
		}

		public static void PlayLocalSoundFx(AudioClip soundFx, AudioSource source, bool loop = false)
		{
			source.loop = loop;
			source.clip = soundFx;
			source.Play();
		}

		public static void StopLocalSoundFx(AudioSource source)
		{
			source.loop = false;
			source.clip = null;
			source.Stop();
		}

		public static void PlayGlobalSoundFx(AudioClip soundFx, bool loop = false)
		{
			_instance.globalSFxSource.loop = loop;
			_instance.globalSFxSource.clip = soundFx;
			_instance.globalSFxSource.Play();
		}

		public static void PlayMusic(AudioClip music, bool loop = true)
		{
			_instance.musicSource.loop = loop;
			_instance.musicSource.clip = music;
			_instance.musicSource.Play();
		}

		public static void ToggleMasterMute()
		{
			_instance.musicSource.mute = !_instance.musicSource.mute;
			if (!_instance.musicSource.mute)
			{
				_instance.musicSource.volume = _instance._savedMusicVolume;
				_instance.sfxMixer.SetFloat(ATTENUTATION, _instance._savedSfxAttenuation);
			}
			else
			{
				_instance.sfxMixer.SetFloat(ATTENUTATION, -80.0f);
			}
		}

		public static bool IsMuted
		{
			get
			{
				return _instance.musicSource.mute;
			}
		}

		public static float MusicVolume
		{
			get
			{
				return _instance._savedMusicVolume;
			}
		}

		public static float SfxVolume
		{
			get
			{
				return _instance._savedSfxAttenuation;
			}
		}

		// min 0, max 1 range set in UI slider
		public static void SetMusicVolume(float volume)
		{
			_instance._savedMusicVolume = volume;
			if (!IsMuted)
			{
				_instance.musicSource.volume = _instance._savedMusicVolume;
			}
		}

		// min -80, max 0 range set in UI slider and mixer properties
		public static void SetSFxVolume(float attenuation)
		{
			_instance._savedSfxAttenuation = attenuation;
			if (!IsMuted)
			{
				_instance.sfxMixer.SetFloat(ATTENUTATION, _instance._savedSfxAttenuation);
			}
		}
	}
}