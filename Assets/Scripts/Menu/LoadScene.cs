using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public void load()
    {
        ClientManager.New = 0;
        SceneManager.LoadScene("LoginOrRegisterScene");
    }
    
    public void MainMenu()
    {
        if(SceneManager.GetActiveScene().name == "MainMenuScene") { return; }
        TempOpponent.Opponent.Reset();
        SceneManager.LoadScene("MainMenuScene");
    }


    private void OnEnable()
    {
        if (transform.parent.name == "Disconnect")
            Invoke("MainMenu", 5f);
    }
}
