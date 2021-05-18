using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FriendlyRequest : MonoBehaviour {

    private Text Username;
    private Button Accept;
    private Button Decline;
    private RectTransform Rect;
	private void Awake ()
    {
        Username = transform.Find("Text").GetComponent<Text>();
        Accept = transform.Find("Accept").GetComponent<Button>();
        Decline = transform.Find("Decline").GetComponent<Button>();
        Rect = gameObject.GetComponent<RectTransform>();
    }
	public void GetOn()
    {
        Username.text = Username.text + TempOpponent.Opponent.Username;
    }
	public void Accepting()
    {
        TempOpponent.Opponent.FriendlyBattle = true;
        TempOpponent.Opponent.Accepting = true;
        if (ValuesChange.SwitchScenes.TryGetValue("ChoosingScene", out UnityEngine.Events.UnityEvent Function))
        {
            Function.Invoke();
        }
        gameObject.SetActive(false);
    }
    public void Declining()
    {
        ClientTCP.PACKAGE_DEBUG("No",TempOpponent.Opponent.Username);
        gameObject.SetActive(false);
    }
}
