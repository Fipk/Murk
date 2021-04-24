using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject play;
    public GameObject quit;
    public GameObject inputPlay;
    public GameObject button;

    public void LoadGame()
    {
        InputField txt_Input = GameObject.Find("Player").GetComponent<InputField>();
        string pseudo = txt_Input.text;
        Player.namePlayer = pseudo;
        SceneManager.LoadScene(1);
    }

    public void SetupGame()
    {
        play.SetActive(false);
        quit.SetActive(false);
        inputPlay.SetActive(true);
        button.SetActive(true);
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
