using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public static string namePlayer = "";
    public static int scorePlayer = 0;
    public int pointsPerhealth = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public bool isMerchantActive = false;
    public static bool canAttackBoss = false;
    public Text HealthText;
    public Text TimerDamage;
    public Text ScoreText;
    public Text KeyText;
    public Text MoneyText;
    public GameObject AddDamage;
    public GameObject AddDefense;
    public GameObject AddHealth;
    public static Animator animator;
    private int health;
    public static bool firstLevel = false;
    public static bool secondLevel = false;
    public static bool isPotionLife = false;
    public static bool isPotionDamage = false;
    public static bool isPotionTimerDamage = false;
    public static GameObject PotionLife;
    public static GameObject PotionDamage;
    public static GameObject PotionTimerDamage;
    public static float timerRemaining = 0;
    public static bool timerIsRunning = false;

    private float[] noiseValues;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        health = GameManager.instance.playerHealthPoints;
        HealthText.text = "Health: " + health;
        KeyText.text = "Seed " + GameManager.seeds[10].ToString();
        MoneyText.text = "Money: " + ApiRedis.GetMoney();
        PotionLife = GameObject.Find("PotionLife");
        PotionDamage = GameObject.Find("PotionDamage");
        PotionTimerDamage = GameObject.Find("PotionTimerDamage");
        if (isPotionLife)
            PotionLife.SetActive(true);
        else
            PotionLife.SetActive(false);
        if (isPotionDamage)
            PotionDamage.SetActive(true);
        else
            PotionDamage.SetActive(false);
        if (isPotionTimerDamage)
            PotionTimerDamage.SetActive(true);
        else
            PotionTimerDamage.SetActive(false);
        base.Start();        
    }

    public static void SetScore(int plus)
    {
        scorePlayer += plus;
        
    }

    void Update()
    {
        if (!GameManager.instance.playersTurn) return;
        ScoreText.text = "Score: " + scorePlayer;
        if (Input.GetKeyDown("1"))  {
            if (isPotionLife)
            {                
                health += 20;
                HealthText.text = "Health: " + health;
                PotionLife.SetActive(false);
                isPotionLife = false; 
            }
        }
        if (Input.GetKeyDown("2"))
        {
            if (isPotionDamage)
            {
                PotionTimerDamage.SetActive(true);
                isPotionTimerDamage = true;
                timerIsRunning = true;
                timerRemaining = 15;
                ApiRedis.SetDamage(1);
                PotionDamage.SetActive(false);
                isPotionDamage = false;
            }
        }

        if (timerIsRunning)
        {
            if (timerRemaining >= 0)
            {
                TimerDamage.text = "Bonus damage: " + Mathf.FloorToInt(timerRemaining);
                timerRemaining -= Time.deltaTime;

            } else
            {
                PotionTimerDamage.SetActive(false);
                isPotionTimerDamage = false;
                ApiRedis.SetDamage(-1);
                timerRemaining = 0;
                timerIsRunning = false;
            }
        }
        
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }
    private void OnDisable()
    {
        GameManager.instance.playerHealthPoints = health;
    }

    public void AddDamageButton()
    {
        if (ApiRedis.GetMoney() >= 100)
        {
            ApiRedis.SetMoney(-100);
            ApiRedis.SetDamage(1);
            MoneyText.text = "Money: " + ApiRedis.GetMoney();
        }
    }

    public void AddDefenseButton()
    {
        if (ApiRedis.GetMoney() >= 50)
        {
            ApiRedis.SetMoney(-50);
            ApiRedis.SetDefense(1);
            MoneyText.text = "Money: " + ApiRedis.GetMoney();
        }
    }

    public void AddHealthButton()
    {
        if (ApiRedis.GetMoney() >= 20)
        {
            ApiRedis.SetMoney(-20);
            health += 5;
            MoneyText.text = "Money: " + ApiRedis.GetMoney();
        }
    }

    public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Exit")
        {
            GameManager.instance.level++;
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        } else if (other.tag == "Food")
        {
            health += pointsPerhealth;
            SetScore(pointsPerhealth);
            HealthText.text = "+" + pointsPerhealth + " Health: " + health;
            other.gameObject.SetActive(false);
        } else if (other.tag == "Soda")
        {
            health += pointsPerSoda;
            SetScore(pointsPerSoda);
            HealthText.text = "+" + pointsPerSoda + " Health: " + health;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Merchant")
        {
            if (!isMerchantActive)
            {
                isMerchantActive = true;
                AddDamage.SetActive(true);
                AddDefense.SetActive(true);
                AddHealth.SetActive(true);
            } else
            {
                isMerchantActive = false;
                AddDamage.SetActive(false);
                AddDefense.SetActive(false);
                AddHealth.SetActive(false);
            }           
        } else if (other.tag == "BossFloor")
        {
            if (firstLevel && secondLevel)
            {
                health -= 7;
                HealthText.text = "Health: " + health;
            } else if (firstLevel)
            {
                health -= 5;
                HealthText.text = "Health: " + health;
            } else
            {
                health -= 3;
                HealthText.text = "Health: " + health;
            }
        }
    }

    protected override void AttemptMove <T> (int xDir, int yDir, bool isPlayer = false)
    {
        MoneyText.text = "Money: " + ApiRedis.GetMoney();
        HealthText.text = "Health: " + health;
        base.AttemptMove<T>(xDir, yDir,true);
        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove <T> (T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseHealth (int loss)
    {
        animator.SetTrigger("playerHit");
        health -= loss;
        HealthText.text = "-" + loss + " Health: " + health;
        CheckIfGameOver();
    }


    private void CheckIfGameOver()
    {
        if (health <= 0)
            GameManager.instance.GameOver();
    }
}
