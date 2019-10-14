using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int WeaponDamage = 1; //default damage for all weapons
    public int RemainingUses = 1;
    public GameObject attackPrefab;
    public GameObject UseBar;

    private BoxCollider2D BC2D;
    // Start is called before the first frame update
    void Start()
    {
        BC2D = gameObject.GetComponent<BoxCollider2D>();
        UseBar = GameObject.FindWithTag("UseBar");

    }

    // Update is called once per frame

    public int DealDamage()
    {
        
        RemainingUses--;
        UpdateUses();
        return WeaponDamage;
        
    }

    public void UpdateUses()
    {
        UseBar.GetComponent<HeartDisplay>().health = RemainingUses;
        if (RemainingUses <= 0)
        {
            Destroy(gameObject);
        }
    }
}
