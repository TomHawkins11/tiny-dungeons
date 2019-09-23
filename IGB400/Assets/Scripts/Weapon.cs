using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int WeaponDamage = 1; //default damage for all weapons

    private BoxCollider2D BC2D;
    // Start is called before the first frame update
    void Start()
    {
        BC2D = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(int dmg)
    {
        
    }
}
