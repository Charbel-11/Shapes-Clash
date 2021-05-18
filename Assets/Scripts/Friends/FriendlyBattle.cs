using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FriendlyBattle : MonoBehaviour
{
    public void Battle()
    {
        ClientTCP.PACKAGE_CheckFriend(transform.GetComponentInParent<FriendsButton>().GetUsername(),PlayerPrefs.GetString("Username"));
        StartCoroutine(WaitForCheck());
    }
	IEnumerator WaitForCheck()
    {
        yield return new WaitUntil(() => TempOpponent.Opponent.CheckDone == true);
        TempOpponent.Opponent.CheckDone = false;
        if (TempOpponent.Opponent.FriendlyBattle == true)
        {
            TempOpponent.Opponent.Username = transform.GetComponentInParent<FriendsButton>().GetUsername();
            if (ValuesChange.SwitchScenes.TryGetValue("ChoosingScene", out UnityEngine.Events.UnityEvent Function))
            {
                Function.Invoke();
            }
        }
        else
        {
            GameObject.Find("Menu Manager").GetComponent<ValuesChange>().ShowMessage("You are not in his Friends List");
        }
    }
}
