using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;
    public bool randomPitch;
    public bool canPlayWhilePlaying;

    public Coroutine fadeInCoroutine;
    public Coroutine fadeAwayCoroutine;

    public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    private static SoundManager _instance;
    public static SoundManager Instance 
    { 
        get
        {
            return _instance;
        }
    }

    public static void CreateInstance(Transform soundManagerPf)
    {
        _instance = Instantiate(soundManagerPf, Vector3.zero, Quaternion.identity).GetComponent<SoundManager>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string soundName)
    {
        Sound currentSound = FindSound(soundName);

        if (!currentSound.source.isPlaying || currentSound.canPlayWhilePlaying)
        {
            if (currentSound.randomPitch)
            {
                //currentSound.source.volume = UnityEngine.Random.Range(.8f, 1f);
                currentSound.source.pitch = UnityEngine.Random.Range(.8f, 1.1f);
            }
            currentSound.source.Play();
        }
        /*else if (currentSound.source.isPlaying && currentSound.canPlayWhilePlaying)
        {
            if (currentSound.randomPitch)
            {
                currentSound.source.volume = UnityEngine.Random.Range(.8f, 1f);
                currentSound.source.pitch = UnityEngine.Random.Range(.8f, 1.1f);
            }
            currentSound.source.Play();
        }*/
    }

    public void Play(Sound currentSound)
    {
        if (!currentSound.source.isPlaying)
            currentSound.source.Play();
        else if (currentSound.source.isPlaying && currentSound.canPlayWhilePlaying)
            currentSound.source.Play();
    }

    public void Stop(string soundName)
    {
        Sound currentSound = FindSound(soundName);
        currentSound.source.Stop();
    }

    public void ChangeSoundPitch(string soundName, float pitch)
    {
        pitch = Mathf.Clamp(pitch, .1f, 3f);

        Sound currentSound = FindSound(soundName);
        currentSound.source.pitch = pitch;
    }

    public void ChangeSoundVolume(string soundName, float volume)
    {
        volume = Mathf.Clamp(volume, .1f, 1f);

        Sound currentSound = FindSound(soundName);
        currentSound.source.volume = volume;
    }

    public void FadeAwayVolume(string soundName, float time)
    {
        Sound currentSound = FindSound(soundName);
        float currentVolume = currentSound.source.volume;

        if (currentSound.fadeInCoroutine != null)
            StopCoroutine(currentSound.fadeInCoroutine);

        if (currentSound.source.volume == 0)
        {
            currentSound.source.Stop();
            return;
        }

        currentSound.fadeAwayCoroutine = StartCoroutine(FadeAway(currentSound, currentVolume, time));
    }
    IEnumerator FadeAway(Sound currentSound, float startVolume, float time)
    {
        float timeElapsed = 0f;
        do
        {
            //Debug.Log(currentSound.source.volume);
            currentSound.source.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / time);
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        } while (timeElapsed < time);
        currentSound.source.volume = 0f;
        currentSound.source.Stop();
    }

    public void FadeInVolume(string soundName,float targetVolume, float time)
    {
        Sound currentSound = FindSound(soundName);
        float currentVolume = currentSound.source.volume;

        if (currentSound.fadeAwayCoroutine != null)
            StopCoroutine(currentSound.fadeAwayCoroutine);

        if (currentSound.source.volume == targetVolume)
        {
            Play(currentSound);
            return;
        }

        currentSound.fadeInCoroutine = StartCoroutine(FadeIn(currentSound, currentVolume, targetVolume, time));
    }

    IEnumerator FadeIn(Sound currentSound, float startVolume, float targetVolume, float time)
    {
        Play(currentSound);
        //Debug.Log(currentSound.source.volume);
        float timeElapsed = 0f;
        do
        {
            //Debug.Log(currentSound.volume);
            currentSound.source.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / time);
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        } while (timeElapsed < time);
        currentSound.source.volume = targetVolume;
    }

    private Sound FindSound(string soundName)
    {
        Sound currentSound = Array.Find(sounds, sound => sound.name == soundName);
        if (currentSound == null)
        {
            Debug.LogError("I don't have \"" + soundName + "\" sound!");
            return null;
        }
        else
            return currentSound;
    }
}
