using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAttack : MonoBehaviour
{
    bool bitAttack;
    PlayerMove player;

    // Start is called before the first frame update
    void Start()
    {
        bitAttack = false;
        player = FindObjectOfType<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bitAttack)
        {
            player.Attack();
        }

        // Å°º¸µå
        if (Input.GetKeyDown(KeyCode.Space)) PressDown();
        else if (Input.GetKeyUp(KeyCode.Space)) PressUp();
    }

    public void PressDown()
    {
        bitAttack = true;
    }

    public void PressUp()
    {
        bitAttack = false;
    }
}
