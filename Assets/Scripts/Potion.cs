using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float healthUpPercent = 10;
    public GameObject effectCollected;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            player.AddHealth(healthUpPercent);
            Instantiate(effectCollected, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
