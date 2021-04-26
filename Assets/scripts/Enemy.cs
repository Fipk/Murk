using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public int hp = 5;
    private Animator animator;
    private Transform target;
    private bool skipMove;
    public GameObject[] test;
    public bool canMove = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (ApiRedis.GetParangon() > 0 && MainMenu.isParangon)
        {
            playerDamage += 10;
        }
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
        test = GameObject.FindGameObjectsWithTag("Wall");
    }
    void Update()
    {
        checkEnemyHp();
    }

    public void loseEnemyHp(int loss)
    {
        if (ApiRedis.GetLevel() == 10)
        {
            if (Player.canAttackBoss)
            {
                hp -= loss;
            }
        } else
        {
            hp = hp - loss;
        }       
    }

    public void checkEnemyHp()
    {
        if (hp <= 0)
        {
            gameObject.SetActive(false);
            ApiRedis.SetMoney(10);
            canMove = false;
            if (ApiRedis.GetLevel() == 10)
            {
                GameManager.instance.GameWon();
            }
        }
    }

    protected override void AttemptMove <T> (int xDir, int yDir, bool isPlayer = false)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        } else if (canMove)
        {
            base.AttemptMove<T>(xDir, yDir);
            skipMove = true;
        }        
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
        {
            //Debug.Log("x: " + target.position.x + " " + transform.position.x + "\n L'ennemi bouge en y");
            yDir = target.position.y > transform.position.y ? 1 : -1;
            /*for (int i = 0; i < test.Length; i++) {
                if ((test[i].transform.position.y == transform.position.y - 1 || test[i].transform.position.y == transform.position.y + 1) && test[i].transform.position.x == transform.position.x)
                {
                    Debug.Log("\n Je suis dedans.");
                    if (test[i].transform.position.y == transform.position.y && test[i].transform.position.x == transform.position.x+1)
                    {
                        Debug.Log("\nIl bouge � gauche.");
                        yDir = 0;
                        xDir = -1;
                        return;
                    } else if (test[i].transform.position.y == transform.position.y && test[i].transform.position.x == transform.position.x - 1)
                    {
                        Debug.Log("\nIl bouge � droite.");
                        yDir = 0;
                        xDir = 1;
                        return;
                    }                   
                } 
            }*/
        } else
        {
            //Debug.Log("y: " + target.position.x + " " + transform.position.x + "\n L'ennemi bouge en x");
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove <T> (T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("EnemyAttack");
        if (ApiRedis.GetLevel() == 10)
        {
            hitPlayer.LoseHealth((20+playerDamage) - ApiRedis.GetDefense());
        } else
        {
            hitPlayer.LoseHealth(playerDamage - ApiRedis.GetDefense());
        }
    }
}
