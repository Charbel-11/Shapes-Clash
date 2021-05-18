using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsButton : MonoBehaviour
{
    public GameObject FriendlyBattle;
    public GameObject Profile;
    public GameObject Spectate;
    public GameObject Remove;

    public Sprite[] XPSprite;

    private int profileVersion;
    private string profile;
    private string[] info;

    private Text Username;
    private Text PP;
    private Text XP;
    private Transform Crown;
    private Image OnlineCircle;
    private int state;
    private bool init = false;

    private void Awake()
    {
        setup();
    }
    
    public void setup()
    {
        if (init) { return; }
        init = true;
        state = 0; profileVersion = -2;

        Username = transform.Find("Username").GetComponent<Text>();
        PP = transform.Find("PP").GetComponent<Text>();
        XP = transform.Find("XP").GetComponentInChildren<Text>();
        Crown = transform.Find("Crown");
        OnlineCircle = transform.Find("Onlinecircle").GetComponent<Image>();
        FriendlyBattle = transform.Find("FriendlyBattle").gameObject;
        Profile = transform.Find("Profile").gameObject;
        Spectate = transform.Find("Spectate").gameObject;
        Remove = transform.Find("Remove").gameObject;
        FriendlyBattle.SetActive(false);
        Profile.SetActive(false);
        Spectate.SetActive(false);
        Remove.SetActive(false);
        Crown.gameObject.SetActive(true);
        PP.gameObject.SetActive(true);
    }

    public string GetUsername()
    {
        return Username.text;
    }
    public void Change(string User, string PPval, string Online, string inGame, string lvl, string shapeID)
    {
        setup();

        Int32.TryParse(shapeID, out int ID);

        Username.text = User;
        PP.text = PPval;
        XP.text = lvl;
        transform.Find("XP").GetComponent<Image>().sprite = XPSprite[ID];

        if (Online.Equals("0"))
        {
            OnlineCircle.color = new Color(1f, 0, 0);
            FriendlyBattle.GetComponent<Button>().interactable = false;
            Spectate.GetComponent<Button>().interactable = false;
            this.transform.SetAsLastSibling();
            return;
        }

        OnlineCircle.color = new Color(0, 1f, 0);
        this.transform.SetAsFirstSibling();

        if (inGame.Equals("1"))
        {
            FriendlyBattle.GetComponent<Button>().interactable = false;
            Spectate.GetComponent<Button>().interactable = true;
        }
        else if (inGame.Equals("0"))
        {
            FriendlyBattle.GetComponent<Button>().interactable = true;
            Spectate.GetComponent<Button>().interactable = false;
        }
    }
    public void OnClick()
    {
        if (state == 0)
        {
            Crown.gameObject.SetActive(false);
            PP.gameObject.SetActive(false);
            FriendlyBattle.SetActive(true);
            Profile.SetActive(true);
            Spectate.SetActive(true);
        }
        else if (state == 1)
        {
            FriendlyBattle.SetActive(false);
            Profile.SetActive(false);
            Spectate.SetActive(false);
            Remove.SetActive(true);
        }
        else
        {
            Crown.gameObject.SetActive(true);
            PP.gameObject.SetActive(true);
            Remove.SetActive(false);
        }

        state++; state %= 3;
    }

    public void openProfile()
    {
        ClientTCP.PACKAGE_DEBUG("Profile", Username.text, profileVersion);
        StartCoroutine(open());    
    }

    IEnumerator open()
    {
        yield return new WaitUntil(() => (TempOpponent.Opponent.GotProfile));
        TempOpponent.Opponent.GotProfile = false;

        if (TempOpponent.Opponent.changed)
        {
            profile = TempOpponent.Opponent.profile;
            info = profile.Split('|');
            profileVersion = Int32.Parse(info[0]);
        }
        Friends temp = transform.parent.parent.parent.parent.parent.Find("FriendsManager").GetComponent<Friends>();
        temp.profilePanel.transform.parent.gameObject.SetActive(true);
        temp.profilePanel.updateProfilePanel(info);
    }
}
