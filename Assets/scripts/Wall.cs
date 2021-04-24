using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp = 4;
    

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    public void DamageWall (int loss)
    {
        
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0)
        {
            Player.SetScore(5);
            gameObject.SetActive(false);
            ApiRedis.SetMoney(5);           
            RandomObject();
        }

    }

    public void RandomObject()
    {
        int loot = Random.Range(1, 3);
        if (loot == 1)
        {
            int chanceLoot = Random.Range(1, 100);
            if (chanceLoot < 90 - ApiRedis.GetChance())
            {
                ApiRedis.IncrPotionChance();
                Player.isPotionLife = true;
                Player.PotionLife.SetActive(true);
            }
            else
            {
                ApiRedis.SetPotionChance(0);
                Player.isPotionDamage = true;
                Player.PotionDamage.SetActive(true);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
