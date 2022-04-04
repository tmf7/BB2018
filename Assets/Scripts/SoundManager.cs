using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

	public AudioSource musicSource;
	public AudioSource globalSFxSource;
	public AudioMixer sfxMixer;			// control the volume only

	public static SoundManager instance = null;

	private float savedSfxAttenuation = 0.0f;
	private float savedMusicVolume = 1.0f;

	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else if (instance != this) {
			Destroy (gameObject);	
		}
	}

	public void PlayLocalSoundFx (AudioClip soundFx, AudioSource source, bool loop = false) {
		source.loop = loop;
		source.clip = soundFx;
		source.Play ();
	}

	public void PlayGlobalSoundFx (AudioClip soundFx, bool loop = false) {
		instance.globalSFxSource.loop = loop;
		instance.globalSFxSource.clip = soundFx;
		instance.globalSFxSource.Play ();
	}

	public void PlayMusic(AudioClip music, bool loop = true) {
		instance.musicSource.loop = loop;
		instance.musicSource.clip = music;
		instance.musicSource.Play ();
	}

	public void ToggleMasterMute() {
		instance.musicSource.mute = !instance.musicSource.mute;
		if (!instance.musicSource.mute) {
			instance.musicSource.volume = instance.savedMusicVolume;
			instance.sfxMixer.SetFloat ("Attenuation", instance.savedSfxAttenuation);
		} else {
			instance.sfxMixer.SetFloat ("Attenuation", -80.0f);
		}
	}

	public bool isMuted {
		get {
			return instance.musicSource.mute;
		}
	}
		
	public float musicVolume {
		get { 
			return instance.savedMusicVolume;
		}
	}

	public float sfxVolume {
		get { 
			return instance.savedSfxAttenuation;
		}
	}
			
	// min 0, max 1 range set in UI slider
	public void SetMusicVolume(float volume) {
		instance.savedMusicVolume = volume;
		if (!isMuted)
			instance.musicSource.volume = instance.savedMusicVolume;
	}

	// min -80, max 0 range set in UI slider and mixer properties
	public void SetSFxVolume(float attenuation) {
		instance.savedSfxAttenuation = attenuation;
		if (!isMuted)
			instance.sfxMixer.SetFloat ("Attenuation", instance.savedSfxAttenuation);
	}
}
