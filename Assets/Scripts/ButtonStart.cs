using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            PressButton();
        }
    }

    public void PressButton()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioClick, 1f);
        GameManager.instance.StartGame();
    }
}
