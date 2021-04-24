using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ServiceStack.Redis;

public class ApiRedis : MonoBehaviour
{
    public static IRedisClientsManager manager = new RedisManagerPool("127.0.0.1:6379");
    public static IRedisClient client = manager.GetClient();
    // Start is called before the first frame update

    public static void SetStatus()
    {
        client.Set("PotionLoot", 0);
        client.Set("Money", 0);
    }

    public static void IncrPotionChance()
    {
        var val = client.Get<int>("PotionLoot");
        client.Set("PotionLoot",val+1);
        
    }
    public static void SetMoney(int value)
    {
        var val = client.Get<int>("Money");
        client.Set("Money", val + value);
    }
    public static int GetMoney()
    {
        return client.Get<int>("Money");
    }
    public static void SetPotionChance(int value)
    {
        client.Set("PotionLoot", value);
    }

    public static int GetChance()
    {
        return client.Get<int>("PotionLoot");
    }
}

   
