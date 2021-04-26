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
    public Text textLeaderboard;
    public static bool isPotionLife = false;
    public static bool isPotionDamage = false;
    public static bool isPotionTimerDamage = false;
    public static GameObject PotionLife;
    public static GameObject PotionDamage;
    public static GameObject PotionTimerDamage;
    public static float timerRemaining = 0;
    public static bool timerIsRunning = false;
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
        GameManager.isSeed = true;
        GameManager.isRandom = true;
        GameManager.isLost = false;

    }
    public void LoadGame()
    {
        InputField txt_Input = GameObject.Find("Player").GetComponent<InputField>();
        string pseudo = txt_Input.text;
        Player.namePlayer = pseudo;
        SceneManager.LoadScene(1);
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
        play.SetActive(false);
        quit.SetActive(false);
        seed.SetActive(true);
        checkSeed.SetActive(true);
        inputPlay.SetActive(true);
        button.SetActive(true);
        
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
