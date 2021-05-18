using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPassThePhone : MonoBehaviour {
    public GameObject YouNeedToChooseMore;
    public bool bot;

    private SelectionManager SM;

    private void Start()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    public void StartGame()
    {
        int[] IDs = new int[6];
        IDs = SM.finalIDs;

        int chosen = 0;
        for (int i = 0; i < 6; i++)
        {
            chosen += (IDs[i] != -1 ? 1 : 0);
        }

        if (chosen < 4 && !bot)
        {
            YouNeedToChooseMore.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("GameMode", 0);
            PlayerPrefs.SetInt("bot", bot ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("PassThePhone");
        }
    }
}
