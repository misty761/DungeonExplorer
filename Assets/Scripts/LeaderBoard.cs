using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class LeaderBoard : MonoBehaviour
{
    string LEADER_BOARD_ID = "CgkIoenYo7sdEAIQAQ";

    // Start is called before the first frame update
    void Start()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_TouchDown = new EventTrigger.Entry();
        entry_TouchDown.eventID = EventTriggerType.PointerDown;
        entry_TouchDown.callback.AddListener((data) => { TouchDown(); });
        eventTrigger.triggers.Add(entry_TouchDown);

        EventTrigger.Entry entry_TouchUp = new EventTrigger.Entry();
        entry_TouchUp.eventID = EventTriggerType.PointerUp;
        entry_TouchUp.callback.AddListener((data) => { TouchUp(); });
        eventTrigger.triggers.Add(entry_TouchUp);

        // authenticate user:
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) =>
        {
            // handle results
            Debug.Log("handle results : " + result);
        });

        /*
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        // requests the email address of the player be available.
        // Will bring up a prompt for consent.
        .RequestEmail()
        // requests a server auth code be generated so it can be passed to an
        //  associated back end server application and exchanged for an OAuth token.
        .RequestServerAuthCode(false)
        // requests an ID token be generated.  This OAuth token can be used to
        //  identify the player to other services such as Firebase.
        .RequestIdToken()
        .Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        */

        PlayGamesPlatform.Activate();
    }

    private void TouchDown()
    {
        
    }

    private void TouchUp()
    {
        RankButtonClick();
    }

    public void RankButtonClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioClick, 1f);
        Social.localUser.Authenticate(AuthenticateHandler);
    }

    void AuthenticateHandler(bool isSuccess)
    {
        if (isSuccess)
        {
            int highScore = PlayerPrefs.GetInt("ScoreTop", 0);
            Social.ReportScore((long) highScore, LEADER_BOARD_ID, (bool success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADER_BOARD_ID);
                    //Debug.Log("Show Leader Board UI : " + success);
                    //Debug.Log("highScore : " + highScore);
                }
                else
                {
                    //Debug.Log("Show Leader Board UI : " + success);
                }
            });
        }
        else
        {
            // login failed
            Debug.Log("Login failed to Google Play Games : " + isSuccess);
        }
    }
}
