using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damageMin = 1;
    public int damageMax = 5;
    public float attackSpeed = 1f;

    PlayerMove player;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerMove>();
        player.attackSpeedWeapon = attackSpeed;
    }
}
