using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public static int damageEnemy = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text HealthText;
    public Text TimerDamage;
    public static Animator animator;
    private int food;
    public static bool isPotionLife = false;
    public static bool isPotionDamage = false;
    public static bool isPotionTimerDamage = false;
    public static GameObject PotionLife;
    public static GameObject PotionDamage;
    public static GameObject PotionTimerDamage;
    public static float timerRemaining = 0;
    public static bool timerIsRunning = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        HealthText.text = "Health: " + food;
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

    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        if (Input.GetKeyDown("1"))  {
            if (isPotionLife)
            {                
                food += 20;
                HealthText.text = "Health: " + food;
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

        if (timerIsRunning)
        {
            if (timerRemaining > 0)
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
        GameManager.instance.playerFoodPoints = food;
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
            food += pointsPerFood;
            HealthText.text = "+" + pointsPerFood + " Health: " + food;
            other.gameObject.SetActive(false);
        } else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            HealthText.text = "+" + pointsPerSoda + " Health: " + food;
            other.gameObject.SetActive(false);
        } 
    }

    protected override void AttemptMove <T> (int xDir, int yDir, bool isPlayer = false)
    {
        HealthText.text = "Health: " + food;
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

    public void LoseFood (int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        HealthText.text = "-" + loss + " Health: " + food;
        CheckIfGameOver();
    }


    private void CheckIfGameOver()
    {
        if (food <= 0)
            GameManager.instance.GameOver();
    }
}
