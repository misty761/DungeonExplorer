using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponThrow : MonoBehaviour
{
    public float speed = 2f;
    public int power = 10;
    public GameObject effectHit;
    
    PlayerMove player;
    Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        direction = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.Damaged(gameObject, collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.audioPunch, 0.5f);
            Instantiate(effectHit, collision.contacts[0].point, transform.rotation);
        }

        Destroy(gameObject);
    }
}
