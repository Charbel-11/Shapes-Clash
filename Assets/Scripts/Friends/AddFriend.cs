using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriend : MonoBehaviour {
    public const int friendLimit = 30;
    public GameObject invalid, added, friendPrefab;
    public Transform friendContainer;
    public Text friendNum;
    [SerializeField] InputField Code;

    public void Add()
    {
        string[] Fr = TempOpponent.Opponent.Friends.Split(',');
        if (Fr.Length == friendLimit)
        {
            invalid.gameObject.SetActive(true);
            invalid.GetComponent<Text>().text = "Max number of friend reached (30)";
            return;
        }

        if (Code.text == "" || Code.text == PlayerPrefs.GetString("Code") || Code.text.Length > 15)
        {
            added.gameObject.SetActive(false);
            invalid.gameObject.SetActive(true);
            invalid.GetComponent<Text>().text = "Invalid Code";
            return;
        }

        ClientTCP.PACKAGE_AddFriend(Code.text);
    }
    public void Added()
    {
        StartCoroutine(WaitforFL());
        added.gameObject.SetActive(true);
        invalid.gameObject.SetActive(false);
    }
    IEnumerator WaitforFL()
    {
        yield return new WaitUntil(() => TempOpponent.Opponent.FLupdated == true);
        string[] Fr = TempOpponent.Opponent.Friends.Split(',');
        string[] Online = TempOpponent.Opponent.FriendsOnline.Split(',');
        string[] PP = TempOpponent.Opponent.FriendsPP.Split(',');
        string[] inGame = TempOpponent.Opponent.FriendsInGame.Split(',');
        string[] levels = TempOpponent.Opponent.FriendsLvl.Split(',');
        string[] shapes = TempOpponent.Opponent.FriendsShapeIDs.Split(',');

        foreach (Transform go in friendContainer) { Destroy(go.gameObject); }

        int i = Fr.Length;
        for (int j = 0; j < i; j++)
        {
            GameObject curF = Instantiate(friendPrefab, friendContainer);
            curF.GetComponent<FriendsButton>().Change(Fr[j], PP[j], Online[j], inGame[j], levels[j], shapes[j]);
        }
        TempOpponent.Opponent.FLupdated = false;

        friendNum.text = i.ToString() + " Friend" + (i != 1 ? "s" : "");
    }
}
