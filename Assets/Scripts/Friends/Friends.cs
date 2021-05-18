using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friends : MonoBehaviour {
    public OpStat profilePanel;

    // Same order: FL, AF, MC
    public GameObject LeftFL, LeftAF, LeftMC;
    public GameObject RightFL, RightAF, RightMC;
    public GameObject disclaimer;

    public Transform friendContainer;
    public GameObject friendPrefab;
    public Text friendNum;

    private void Start()
    {
        RightMC.transform.Find("Code").GetComponent<Text>().text = PlayerPrefs.GetString("Code");
    }
    private void OnEnable()
    {
        goTo1FL(true);
        GetList();
    }
    private void HighlightBut(GameObject go)
    {
        Color temp = go.GetComponent<Image>().color;
        temp.a = 1f;
        go.GetComponent<Image>().color = temp;
    }
    private void DehighlightBut(GameObject go)
    {
        Color temp = go.GetComponent<Image>().color;
        temp.a = 0f;
        go.GetComponent<Image>().color = temp;
    }
    public void goTo1FL(bool Dont = false)
    {
        disclaimer.SetActive(true);
        RightFL.gameObject.SetActive(true);
        RightAF.gameObject.SetActive(false);
        RightMC.gameObject.SetActive(false);
        HighlightBut(LeftFL);
        DehighlightBut(LeftAF);
        DehighlightBut(LeftMC);
        if (!Dont) { GetList(); }
    }
    public void goTo2AF()
    {
        disclaimer.SetActive(false);
        RightFL.gameObject.SetActive(false);
        RightAF.gameObject.SetActive(true);
        RightMC.gameObject.SetActive(false);
        HighlightBut(LeftAF);
        DehighlightBut(LeftFL);
        DehighlightBut(LeftMC);

        RightAF.transform.Find("Invalid").gameObject.SetActive(false);
        RightAF.transform.Find("Added").gameObject.SetActive(false);
    }
    public void goTo3MC()
    {
        disclaimer.SetActive(false);
        RightFL.gameObject.SetActive(false);
        RightAF.gameObject.SetActive(false);
        RightMC.gameObject.SetActive(true);
        HighlightBut(LeftMC);
        DehighlightBut(LeftAF);
        DehighlightBut(LeftFL);
    }

    //Make it update list, every time i open the friend panel
    public void GetList()
    {
        ClientTCP.PACKAGE_GetFriendsList();
        StartCoroutine(WaitforFL());
    }
     public IEnumerator WaitforFL()
    {
        yield return new WaitUntil(() => TempOpponent.Opponent.FLupdated == true);
        friendContainer.gameObject.SetActive(true);
        string[] Fr = TempOpponent.Opponent.Friends.Split(',');
        string[] Online = TempOpponent.Opponent.FriendsOnline.Split(',');
        string[] PP = TempOpponent.Opponent.FriendsPP.Split(',');
        string[] inGame = TempOpponent.Opponent.FriendsInGame.Split(',');
        string[] levels = TempOpponent.Opponent.FriendsLvl.Split(',');
        string[] shapes = TempOpponent.Opponent.FriendsShapeIDs.Split(',');

        foreach (Transform go in friendContainer) { Destroy(go.gameObject); }

        int numFriends = 0;
        if (Fr[0] != "")
        {
            numFriends = Fr.Length;
            for (int j = 0; j < numFriends; j++)
            {
                GameObject curF = Instantiate(friendPrefab, friendContainer);
                curF.GetComponent<FriendsButton>().Change(Fr[j], PP[j], Online[j], inGame[j], levels[j], shapes[j]);
            }
            TempOpponent.Opponent.FLupdated = false;
        }
        TempOpponent.Opponent.FLupdated = false;

        friendNum.text = numFriends.ToString() + " Friend" + (numFriends != 1 ? "s" : "");
    }
}
