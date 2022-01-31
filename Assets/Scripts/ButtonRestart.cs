using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonRestart : MonoBehaviour
{
    PlayerMove player;
    Canvas canvas;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();        
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            PressButton();
        }
    }

    public void PressButton()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioClick, 1f);
        Destroy(canvas.gameObject);
        Destroy(GameManager.instance.gameObject);
        Destroy(player.gameObject);
        SceneManager.LoadScene("Scene1");
    }
}
