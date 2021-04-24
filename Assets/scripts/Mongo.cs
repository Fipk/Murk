using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;

public class Mongo : MonoBehaviour
{
    void Start()
    {
        /*var client = new MongoClient("mongodb://root:root@127.0.0.1:27017");
        var database = client.GetDatabase("murk");
        var seedsDocument = database.GetCollection<BsonDocument>("seeds");
        var leaderboardDocument = database.GetCollection<BsonDocument>("leaderboard");
        var test = new BsonDocument
        {
            { "name", "MongoDb" },
            { "type", "Database" },
            { "firstname", "calvin" }
        };
        collection.InsertOne(test);*/
        
    }
}
