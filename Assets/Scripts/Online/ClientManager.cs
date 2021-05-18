using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour {
    public static string serverIP = "90.186.35.142";
    public static int serverPort = 8080;
    public static bool Done = false;
    public static bool lostFocus = false;
    public static int New = 0;
    public GameObject LoginPanel;
    public GameObject Offline;
    public GameObject UpdatePanel;

    string gameId = "3547393";
    bool testMode = false;


    private void Awake()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();

        Screen.SetResolution(1920, 1080, true);

        ClientHandleData.InitializePacketListener();
        ShapeConstants.Initialize();
        Advertisement.Initialize(gameId, testMode);

        StartCoroutine(Connect());
    }
    IEnumerator Connect()
    {
        yield return new WaitUntil(() => Done);     //Connect once CloudOnce is set
        Done = false;      
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        ClientTCP.Username = CloudOnce.CloudVariables.Username;
        ClientTCP.InitializeClientSocket(serverIP, serverPort);   
        StartCoroutine(NewAcc());
        StartCoroutine(OfflineEnum());
    }

    private void OnApplicationQuit()
    {
        ClientTCP.CloseConnection();
    }

    IEnumerator NewAcc()
    {
        yield return new WaitUntil(() => New != 0);
        if(New == 1)
        {
            PlayerPrefs.SetInt("SpecialAbilityID" + "0",-1);
            PlayerPrefs.SetInt("SpecialAbilityID" + "1", -1);
            PlayerPrefs.SetInt("SpecialAbilityID" + "2", -1);
            PlayerPrefs.SetInt("SpecialAbilityID" + "3", -1);
            PlayerPrefs.SetFloat("Volume", 0.5f);
            PlayerPrefs.SetFloat("AbVolume", 0.5f);
            PlayerPrefs.SetInt("Tutorial", 1);
            PlayerPrefs.SetInt("DN", 0);
            int[] Array = new int[4] { 0, 0, 0, 0 };
            PlayerPrefsX.SetIntArray("OpeningChest", Array);
            PlayerPrefsX.SetIntArray("TimerAr", Array);
            PlayerPrefsX.SetIntArray("Queue", Array);
            PlayerPrefs.SetInt("bot", 0);
            PlayerPrefsX.SetStringArray("BotProfiles", new string[0]);
            PlayerPrefsX.SetStringArray("Replay", new string[1] { "N" });
            MainMenuController.JustRegistered = true;
            LoginPanel.SetActive(true);
        }
        else if(New == 3)
        {
            Offline.SetActive(true);
        }
        New = 0;        
    }
    IEnumerator OfflineEnum()
    {
        yield return new WaitForSeconds(5f);
        if (New == 0) { New = 3; }
    }
    public void TryAgain()
    {
        Done = true;
        StartCoroutine(Connect());
    }

    //Note: When keyboard is on, onappliccationfocus(false) is called
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            ClientTCP.PACKAGE_DEBUG("Offline", PlayerPrefs.GetString("Username"));
            PlayerPrefs.SetInt("DN", 0);
            string s = SceneManager.GetActiveScene().name;
            if (s == "OnlineBattleScene" || s == "SpectateScene") { ClientTCP.CloseConnection(); lostFocus = true; }
        }
        else
        {
            if(ClientTCP.ClientSocket != null)
                ClientTCP.PACKAGE_DEBUG("Online", PlayerPrefs.GetString("Username"));

            if (lostFocus || PlayerPrefs.GetInt("DN") == 1)
            {
                lostFocus = false;
                PlayerPrefs.SetInt("DN", 0);
                TempOpponent.Opponent.Reset();
                SceneManager.LoadScene("LoginOrRegisterScene");
            }
        }
    }
}
