using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MongoDB.Bson;
using MongoDB.Driver;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerHealthPoints = 100;
    [HideInInspector] public bool playersTurn = true;
    public int level = 1;
    public static bool isLost = false;

    private Text levelText;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    public static bool isSeed = true;
    public static bool isRandom = true;
    private static int[] seeds = new int[11];
    System.DateTime foo;
    public static MongoClient client = new MongoClient("mongodb://root:root@127.0.0.1:27017");
    public static IMongoDatabase database = client.GetDatabase("murk");
    public static IMongoCollection<BsonDocument> seedsCollection = database.GetCollection<BsonDocument>("seeds");
    public static IMongoCollection<BsonDocument> leaderboardCollection = database.GetCollection<BsonDocument>("leaderboard");

    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if ( instance != this)
        {
            Destroy(gameObject);
        }
        if (isSeed && isRandom)
        {         
            for (int i = 0; i <= 10; i++)
            {            
                foo = System.DateTime.Now;
                long unixTime = ((System.DateTimeOffset)foo).ToUnixTimeSeconds();
                seeds[i] = (int)unixTime + i;
            }
            isSeed = false;
            InsertSeedDatabase();
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();       
        InitGame();
    }

    void InsertSeedDatabase()
    {
        var seedDB = new BsonDocument
            {               
                { "seed1", seeds[0] },
                { "seed2", seeds[1] },
                { "seed3", seeds[2] },
                { "seed4", seeds[3] },
                { "seed5", seeds[4] },
                { "seed6", seeds[5] },
                { "seed7", seeds[6] },
                { "seed8", seeds[7] },
                { "seed9", seeds[8] },
                { "seed10", seeds[9] },
                { "keySeed", seeds[10] }
            };
        seedsCollection.InsertOne(seedDB);
        //var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("6083d85afacf2129cca301ab"));
        //var firstDocument = seedsDocument.Find(new BsonDocument()).FirstOrDefault();
        //Debug.Log(firstDocument.ToString());

    }
    
    

    private void OnLevelWasLoaded (int index)
    {
        if (level == 1)
        {
            return;
        }           
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level, seeds[level]);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    public void GameOver()
    {
        if (!isLost)
        {
            isLost = true;
            levelText.text = "You cleared " + level + " levels";
            levelImage.SetActive(true);
            var sort = Builders<BsonDocument>.Sort.Descending("keySeed");
            var lastDocument = seedsCollection.Find(new BsonDocument()).Sort(sort).First();
            var scoreDocument = new BsonDocument
            {
                { "keySeed", lastDocument[11] },
                { "player", Player.namePlayer },
                { "score", Player.scorePlayer },
                { "isComplete", false },
            };
            leaderboardCollection.InsertOne(scoreDocument);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i =0; i < enemies.Count; i++)
        {
            
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
