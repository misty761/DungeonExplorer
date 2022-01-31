using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : MonoBehaviour
{
    public GameObject prefChest;
    public float probChest = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        float random = Random.Range(0f, 1f);
        if (random < probChest)
        {
            Instantiate(prefChest, transform.position, transform.rotation);
        }
        
    }

}
