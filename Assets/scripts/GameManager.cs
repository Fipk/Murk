using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerHealthPoints = 100;
    [HideInInspector] public bool playersTurn = true;
    public int level = 1;

    private Text levelText;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    public static bool isSeed = true;
    public static bool isRandom = true;
    private static int[] seeds = new int[10];
    System.DateTime foo;

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
            for (int i = 0; i < 10; i++)
            {            
                foo = System.DateTime.Now;
                long unixTime = ((System.DateTimeOffset)foo).ToUnixTimeSeconds();
                seeds[i] = (int)unixTime + i;
            }
            isSeed = false;
        }
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();       
        InitGame();
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
        levelText.text = "You cleared " + level + " levels";
        levelImage.SetActive(true);
        enabled = false;
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
