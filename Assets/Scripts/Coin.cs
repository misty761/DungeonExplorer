using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int price = 1;
    public GameObject effectCollected;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            player.AddCoin(price);
            Instantiate(effectCollected, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
