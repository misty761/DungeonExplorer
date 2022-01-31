using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSpikes : MonoBehaviour
{
    public int power = 10;

    float timeStay;
    readonly float durationStay = 1f;
    bool isStaying;

    private void Start()
    {
        timeStay = 0f;
        isStaying = false;
    }

    private void Update()
    {
        if (GameManager.instance.state != GameManager.State.Play) return;

        if (isStaying)
        {
            if (timeStay == 0f)
            {
                PlayerMove player = FindObjectOfType<PlayerMove>();
                player.Damaged(gameObject);
            }

            timeStay += Time.deltaTime;
            if (timeStay > durationStay) timeStay = 0f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            timeStay = 0f;
            isStaying = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isStaying = false;
        }
    }
}
