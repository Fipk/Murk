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
        client.Set("PlayerDamage", 1);
        client.Set("PlayerDefense", 0);
        client.Set("level", 1);
        client.Set("WallDestroy", 0);
    }

    public static void SetWallDestroy(int value)
    {
        var val = client.Get<int>("WallDestroy");
        client.Set("WallDestroy", val + value);
    }

    public static int GetWallDestroy()
    {
        return client.Get<int>("WallDestroy");
    }

    public static void SetLevel(int value)
    {
        client.Set("level", value);
    }

    public static int GetLevel()
    {
        return client.Get<int>("level");
    }
    public static void SetDefense(int value)
    {
        var val = client.Get<int>("PlayerDefense");
        client.Set("PlayerDefense", val + value);
    }

    public static int GetDefense()
    {
        return client.Get<int>("PlayerDefense");
    }

    public static void SetDamage(int value)
    {
        var val = client.Get<int>("PlayerDamage");
        client.Set("PlayerDamage", val + value);
    }

    public static int GetDamage()
    {
        return client.Get<int>("PlayerDamage");
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

   
