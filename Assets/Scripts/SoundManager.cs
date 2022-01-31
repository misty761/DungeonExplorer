using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static SoundManager instance;
    public GameObject imageSoundOn;
    public GameObject imageSoundOff;

    public AudioClip audioAlert;
    public AudioClip audioAttack;
    public AudioClip audioClick;
    public AudioClip audioCoin;
    public AudioClip audioDamaged;
    public AudioClip audioDeath;
    public AudioClip audioDenied;
    public AudioClip audioDoor;
    public AudioClip audioFanfare;
    public AudioClip audioFire;
    public AudioClip audioGameOver;
    public AudioClip audioJump;
    public AudioClip audioLevelUp;
    public AudioClip audioLifeUp;
    public AudioClip audioNegative;
    public AudioClip audioPunch;
    public AudioClip audioScore;
    public AudioClip audioSpawnItem;
    public AudioClip audioSword;

    public bool isSoundOn;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("SoundManager is already existed!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        int soundState = PlayerPrefs.GetInt("SoundState", 1);
        if (soundState != 0) 
        {
            isSoundOn = true;
        } 
        else 
        {
            isSoundOn = false;
        }
        SetSound();

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry_TouchUp = new EventTrigger.Entry();
        entry_TouchUp.eventID = EventTriggerType.PointerUp;
        entry_TouchUp.callback.AddListener((data) => { TouchUp(); });
        eventTrigger.triggers.Add(entry_TouchUp);
    }

    private void SetSound()
    {
        if (isSoundOn)
        {
            imageSoundOn.SetActive(true);
            imageSoundOff.SetActive(false);
            PlayerPrefs.SetInt("SoundState", 1);    // 0: sound off, 1: sound on
        }
        else{
            imageSoundOn.SetActive(false);
            imageSoundOff.SetActive(true);
            PlayerPrefs.SetInt("SoundState", 0);    // 0: sound off, 1: sound on
        }
    }

    private void TouchUp()
    {
        isSoundOn = !isSoundOn;
        PlaySound(audioClick);
        SetSound();
    }

    public void PlaySound(AudioClip audioClip)
    {
        if (isSoundOn) audioSource.PlayOneShot(audioClip);
    }

    public void PlaySound(AudioSource audio, AudioClip audioClip)
    {
        if (isSoundOn) audio.PlayOneShot(audioClip);
    }

    public void PlaySound(AudioClip audioClip, float v = 1f)
    {
        if (isSoundOn)
        {
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero, v);
        }
    }

    public void PlaySound(AudioClip audioClip, Vector3 position, float v = 1f)
    {
        if (isSoundOn)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, v);
        }
    }
}
