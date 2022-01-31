using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public GameObject imageMusicOn;
    public GameObject imageMusicOff;

    public bool isMusicOn;
    AudioSource music;
    
    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();   
        int musicState = PlayerPrefs.GetInt("MusicState", 1);
        if (musicState != 0) 
        {
            isMusicOn = true;
        } 
        else 
        {
            isMusicOn = false;
        }
        SetMusic();

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry_TouchUp = new EventTrigger.Entry();
        entry_TouchUp.eventID = EventTriggerType.PointerUp;
        entry_TouchUp.callback.AddListener((data) => { TouchUp(); });
        eventTrigger.triggers.Add(entry_TouchUp);
    }

    private void TouchUp()
    {
        isMusicOn = !isMusicOn;
        SetMusic();
    }

    void SetMusic()
    {
        if (isMusicOn)
        {
            music.Play();
            imageMusicOn.SetActive(true);
            imageMusicOff.SetActive(false);
            PlayerPrefs.SetInt("MusicState", 1);    // 0: music off, 1: music on
        }
        else{
            music.Stop();
            imageMusicOn.SetActive(false);
            imageMusicOff.SetActive(true);
            PlayerPrefs.SetInt("MusicState", 0);    // 0: music off, 1: music on
        }
    }

}
