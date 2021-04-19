using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class PostMethod : MonoBehaviour
{
    InputField outputArea;
    // Start is called before the first frame update
    void Start()
    {
        outputArea = GameObject.Find("OutputArea").GetComponent<InputField>();
        GameObject.Find("PostButton").GetComponent<Button>().onClick.AddListener(PostData);
    }

    void PostData() => StartCoroutine(PostData_Coroutine());

    IEnumerator PostData_Coroutine()
    {
        string url = "https://my-json-server.typicode.com/typicode/demo/posts";
        WWWForm form = new WWWForm();
        form.AddField("title", "testdata");
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                outputArea.text = request.error;
            else
                outputArea.text = request.downloadHandler.text;
        }
    }
}
