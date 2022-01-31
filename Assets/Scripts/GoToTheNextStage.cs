using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToTheNextStage : MonoBehaviour
{
    public GameObject boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (boss == null)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GameManager.instance.LoadTheNextStage();
            }
        }
    }
}
