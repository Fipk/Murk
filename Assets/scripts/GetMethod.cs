using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ServiceStack.Redis;

public class GetMethod : MonoBehaviour
{
    InputField outputArea;
    // Start is called before the first frame update
    void Start()
    {
        var manager = new RedisManagerPool("127.0.0.1:6379");
        using (var client = manager.GetClient())
        {
            client.Set("foo", "bar");
            Debug.Log(client.Get<string>("foo"));
        }
        outputArea = GameObject.Find("OutputArea").GetComponent<InputField>();
        GameObject.Find("GetButton").GetComponent<Button>().onClick.AddListener(GetData);
    }

    void GetData() => StartCoroutine(GetData_Coroutine());

    IEnumerator GetData_Coroutine()
    {
        string url = "https://my-json-server.typicode.com/typicode/demo/posts";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                outputArea.text = request.error;
            else
                outputArea.text = request.downloadHandler.text;
        }
    }
}


