using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public static int scorePlayer = 0;
    public static int damageEnemy = 1;
    public int pointsPerhealth = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text HealthText;
    public Text TimerDamage;
    public Text ScoreText;
    public static Animator animator;
    private int health;
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
                damageEnemy++;
                PotionDamage.SetActive(false);
                isPotionDamage = false;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            System.DateTime foo = System.DateTime.Now;
            long unixTime = ((System.DateTimeOffset)foo).ToUnixTimeSeconds();
            Debug.Log((int)unixTime);
            
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
                damageEnemy--;
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
    }

    protected override void AttemptMove <T> (int xDir, int yDir, bool isPlayer = false)
    {
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
