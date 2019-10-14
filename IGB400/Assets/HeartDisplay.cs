using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public int health;
    public int NumOfHearts;
    
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public Image[] Hearts;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (health > NumOfHearts)
        {
            health = NumOfHearts;
        }

        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i < health)
            {
                Hearts[i].sprite = FullHeart;
            }
            else
            {
                Hearts[i].sprite = EmptyHeart;
            }

            if (i < NumOfHearts)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }
        }
    }

   
}
