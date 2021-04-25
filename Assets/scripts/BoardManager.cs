using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] merchantTiles;
    public GameObject[] bossTiles;
    public GameObject[] bossEnemyTiles;


    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList(int level)
    {
        gridPositions.Clear();
        if (level == 10)
        {
            for (int x = 0; x < columns - 1; x++)
            {
                for (int y = 0; y < rows - 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    } else
                    {
                        gridPositions.Add(new Vector3(x, y, 0f));                       
                    }
                }
            }
        } else
        {
            for (int x = 1; x < columns - 1; x++)
            {
                for (int y = 1; y < rows - 1; y++)
                {
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }
        
        
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns +1 ; x++)
        {
            for (int y = -1; y < columns +1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition(int seed)
    {
        Random.InitState(seed);
        int randomIndex = Random.Range(0, gridPositions.Count) + (int)(Random.value * 5);
        if (randomIndex >= gridPositions.Count)
        {
            randomIndex = Mathf.Abs(randomIndex - gridPositions.Count);        
        }       
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum, int seed)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {           
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];          
            Vector3 randomPosition = RandomPosition(seed);
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }

    }

    public void SetupScene(int level, int seed)
    {
        BoardSetup();
        InitialiseList(level);
        if (level == 5)
        {
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum, seed);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum, seed);
            LayoutObjectAtRandom(merchantTiles, 1, 1, seed);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        } else if (level == 10)
        {          
            LayoutObjectAtRandom(bossTiles, 10, 15, seed);
            LayoutObjectAtRandom(wallTiles, 2, 1, seed);
            LayoutObjectAtRandom(bossEnemyTiles, 1, 0, seed);
        } else
        {
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum, seed);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum, seed);
            int enemyCount = (int)(level / 3);
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount, seed);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }
       
    }
}
