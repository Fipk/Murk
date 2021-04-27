using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Declarations
    public GameObject play;
    public GameObject quit;
    public GameObject inputPlay;
    public GameObject button;
    public GameObject seed;
    public GameObject checkSeed;
    public GameObject leaderboardButton;
    public InputField outputArea;
    public GameObject HealthMalus;
    public GameObject HealthBonus;
    public GameObject DefenseMalus;
    public GameObject DefenseBonus;
    public GameObject AttackMalus;
    public GameObject AttackBonus;
    public GameObject Validate;
    public GameObject Parangon;
    public GameObject ParangonText;
    public Text textLeaderboard;
    private static bool isInit;
    public static bool isPotionLife = false;
    public static bool isPotionDamage = false;
    public static bool isPotionTimerDamage = false;
    public static GameObject PotionLife;
    public static GameObject PotionDamage;
    public static GameObject PotionTimerDamage;
    public static float timerRemaining = 0;
    public static bool timerIsRunning = false;
    public static bool isParangon = false;
    public Image m_SpriteRenderer;
    #endregion

    public void Start()
    {       
        Player.scorePlayer = 0;
        Player.namePlayer = "";
        Player.isPotionLife = false;
        Player.isPotionDamage = false;
        Player.isPotionTimerDamage = false;
        Player.timerRemaining = 0;
        Player.timerIsRunning = false;
        Player.firstLevel = false;
        Player.secondLevel = false;
        Player.canAttackBoss = false;
        Player.isStarted = false;
        GameManager.isSeed = true;
        GameManager.isRandom = true;
        GameManager.isLost = false;
        GameManager.isHealthMalus = false;
        GameManager.isDefenseMalus = false;
        GameManager.isAttackMalus = false;
        GameManager.HealthMalus = 0;
        GameManager.DefenseMalus = 0;
        GameManager.AttackMalus = 0;
        if (!isInit)
        {
            isInit = true;
            ApiRedis.client.Set("Parangon", 0);
        }
    }
    public void LoadGame()
    {
        InputField txt_Input = GameObject.Find("Player").GetComponent<InputField>();
        string pseudo = txt_Input.text;
        Player.namePlayer = pseudo;
        SceneManager.LoadScene(1);
    }

    public void SetParangonBool()
    {
        isParangon = !isParangon;
        if (isParangon)
        {
            ParangonText.SetActive(true);
        } else
        {
            ParangonText.SetActive(false);
        }        
    }
    void ColorGreen(GameObject test)
    {
        m_SpriteRenderer = test.GetComponent<Image>();
        m_SpriteRenderer.color = new Color(0f, 1f, 1f, 1f);
    }

    void ColorRed(GameObject test)
    {         
        m_SpriteRenderer = test.GetComponent<Image>();
        m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);        
    }

    public void Options()
    {
        play.SetActive(false);
        quit.SetActive(false);
        HealthMalus.SetActive(true);
        HealthBonus.SetActive(true);
        DefenseMalus.SetActive(true);
        DefenseBonus.SetActive(true);
        AttackMalus.SetActive(true);
        AttackBonus.SetActive(true);
        Validate.SetActive(true);
        Parangon.SetActive(true);
    }

    public void HealthUpdateMalus()
    {
        ColorGreen(HealthMalus);
        GameManager.isHealthMalus = true;
        GameManager.HealthMalus = -50;
        ColorRed(HealthBonus);
    }
    public void HealthUpdateBonus()
    {
        ColorGreen(HealthBonus);
        GameManager.isHealthMalus = true;
        GameManager.HealthMalus = 50;
        ColorRed(HealthMalus);
    }
    public void DefenseUpdateMalus()
    {
        ColorGreen(DefenseMalus);
        GameManager.isDefenseMalus = true;
        GameManager.DefenseMalus = -5;
        ColorRed(DefenseBonus);
    }
    public void DefenseUpdateBonus()
    {
        ColorGreen(DefenseBonus);
        GameManager.isDefenseMalus = true;
        GameManager.DefenseMalus = 5;
        ColorRed(DefenseMalus);
    }
    public void AttackUpdateMalus()
    {
        ColorGreen(AttackMalus);
        GameManager.isAttackMalus = true;
        GameManager.AttackMalus = -1;
        ColorRed(AttackBonus);
    }
    public void AttackUpdateBonus()
    {
        ColorGreen(AttackBonus);
        GameManager.isAttackMalus = true;
        GameManager.AttackMalus = 1;
        ColorRed(AttackMalus);
    }

    /*public void leaderboard()
    {
        var client = new MongoClient("mongodb://root:root@127.0.0.1:27017");
        var database = client.GetDatabase("murk");
        var seedsCollection = database.GetCollection<BsonDocument>("seeds");
        var leaderboardCollection = database.GetCollection<BsonDocument>("leaderboard");
        play.SetActive(false);
        quit.SetActive(false);
        leaderboardButton.SetActive(false);
        
        var documents = GameManager.leaderboardCollection.Find(new BsonDocument()).ToList();
        foreach (BsonDocument doc in documents)
        {
            Debug.Log("test");
            var filter = Builders<BsonDocument>.Filter.Eq("keySeed", doc[1]);
            var sort = Builders<BsonDocument>.Sort.Descending("score");
            //var lastDocument = leaderboardCollection.Find(filter).Sort(sort).First();
            var lastDocument = leaderboardCollection.Find(filter).ToList();
            outputArea.text += lastDocument[1].ToString() + " Meilleur score; " + lastDocument[2] + "\n";
        }
    }*/
    public void SetupGame()
    {
        HealthMalus.SetActive(false);
        HealthBonus.SetActive(false);
        DefenseMalus.SetActive(false);
        DefenseBonus.SetActive(false);
        AttackMalus.SetActive(false);
        AttackBonus.SetActive(false);
        Validate.SetActive(false);
        Parangon.SetActive(false);
        seed.SetActive(true);
        checkSeed.SetActive(true);
        inputPlay.SetActive(true);
        button.SetActive(true);
        ParangonText.SetActive(false);

    }

    public void CheckIfSeedExist()
    {
        InputField txt_Input = GameObject.Find("Seed").GetComponent<InputField>();
        string keySeed = txt_Input.text;
        txt_Input.text = "";
        var documents = GameManager.seedsCollection.Find(new BsonDocument()).ToList();
        foreach (BsonDocument doc in documents)
        {
            if (doc[11].ToString() == keySeed)
            {
                for (int i = 1; i <= 11; i++)
                {
                    GameManager.seeds[i - 1] = (int)doc[i];
                }
                GameManager.isRandom = false;
                seed.SetActive(false);
                checkSeed.SetActive(false);
                break;
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
