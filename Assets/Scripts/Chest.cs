using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    Animator animator;

    public GameObject prefCoin;
    public GameObject prefPotionSmall;
    public GameObject prefPotionBig;
    public GameObject prefMimic;
    public float probCoin = 0.35f;
    public float probPotionSmall = 0.35f;
    public float probPotionBig = 0.1f;
    public float probPotionMimic = 0.1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Opened"))
        {
            SpawnItem();
            
            Destroy(gameObject);
        }
    }

    void SpawnItem()
    {
        float prob = Random.Range(0f, 1f);
        if (prob < probPotionBig)
        {
            Instantiate(prefPotionBig, transform.position, transform.rotation);
        }
        else if (prob < probPotionBig + probPotionSmall)
        {
            Instantiate(prefPotionSmall, transform.position, transform.rotation);
        }
        else if (prob < probPotionBig + probPotionSmall + probPotionMimic)
        {
            Instantiate(prefMimic, transform.position, transform.rotation);
        }
        else if (prob < probPotionBig + probPotionSmall + probPotionMimic + probCoin)
        {
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(transform.position.x + 0.03f * i, transform.position.y);
                Instantiate(prefCoin, pos, transform.rotation);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.audioDoor, 1f);
            animator.SetTrigger("Open");
        }
    }
}
