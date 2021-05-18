using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour
{
    private int profileVersion;
    private string profile;
    private string[] info;
   
    private Text Username;
    private Text PPDifference;
    private int[] Choices1;
    private int[] Choices2;
    private int[] Probs1;
    private int[] Probs2;
    private string[] Levels1;
    private string[] Levels2;
    private string[] Super100Levels1;
    private string[] Super100Levels2;
    private string[] Super200Levels1;
    private string[] Super200Levels2;
    private int[] Passives1;
    private int[] Passives2;
    private int ShapeID1;
    private int ShapeID2;
    private int SkinID;
    private int SkinID2;
    private int Level1;
    private int Level2;
    private Text TimeText;
    private TimeSpan Time;
    //private DateTime TimeNow;
    bool Offline = false;
    void Awake()
    {
        Username = transform.Find("Username").GetComponent<Text>();
        PPDifference = transform.Find("PP").GetComponent<Text>();
        TimeText = transform.Find("Time").GetComponent<Text>();
        profileVersion = -2;
    }
    public void Initialize(string Array)
    {
        if (ValuesChange.offline) { transform.Find("Profile").GetComponent<Button>().interactable = false; }
        string Rep = Array;
        string[] Reps = Rep.Split('/');
        int i = 0;
        foreach (string c in Reps)
        {
            if (i <= 11)
            {
                string[] PlaceHolder = c.Split(',');
                int[] Holder = new int[PlaceHolder.Length];
                int j = 0;
                foreach (string d in PlaceHolder)
                {
                    Int32.TryParse(d, out Holder[j]);
                    j++;
                }
                if (i == 0)
                {
                    Choices1 = Holder;
                }
                else if (i == 1)
                {
                    Choices2 = Holder;
                }
                else if (i == 2)
                {
                    Probs1 = Holder;
                }
                else if (i == 3)
                {
                    Probs2 = Holder;
                }
                else if (i == 4)
                {
                    Levels1 = PlaceHolder;
                }
                else if (i == 5)
                {
                    Levels2 = PlaceHolder;
                }
                else if (i == 6)
                {
                    Super100Levels1 = PlaceHolder;
                }
                else if (i == 7)
                {
                    Super100Levels2 = PlaceHolder;
                }
                else if (i == 8)
                {
                    Super200Levels1 = PlaceHolder;
                }
                else if (i == 9)
                {
                    Super200Levels2 = PlaceHolder;
                }
                else if(i == 10)
                {
                    string[] Passives1Str = PlaceHolder;
                    Passives1 = new int[Passives1Str.Length];
                    int h = 0;
                    foreach(string s in Passives1Str)
                    {
                        Int32.TryParse(s,out Passives1[h]);
                        h++;
                    }
                }
                else if(i == 11)
                {
                    string[] Passives2Str = PlaceHolder;
                    Passives2 = new int[Passives2Str.Length];
                    int h = 0;
                    foreach (string s in Passives2Str)
                    {
                        Int32.TryParse(s, out Passives2[h]);
                        h++;
                    }
                }
            }
            else
            {
                if (i == 12)
                {
                    Username.text = c;
                }
                else if (i == 13)
                {
                    Int32.TryParse(c, out ShapeID1);
                }
                else if (i == 14)
                {
                    Int32.TryParse(c, out ShapeID2);
                }
                else if(i == 15)
                {
                    Int32.TryParse(c, out SkinID);
                }
                else if(i == 16)
                {
                    Int32.TryParse(c, out SkinID2);
                }
                else if(i == 17)
                {
                    Int32.TryParse(c, out Level1);
                }
                else if(i == 18)
                {
                    Int32.TryParse(c, out Level2);
                }
                else if (i == 19)
                {
                    int PP;
                    Int32.TryParse(c, out PP);
                    if (PP > 0)
                    {
                        PPDifference.text = "+" + PP;
                        PPDifference.color = new Color(0, 1f, 0);
                    }
                    else if (PP < 0)
                    {
                        PPDifference.text = PP.ToString();
                        PPDifference.color = new Color(1f, 0, 0);
                    }
                    else
                    {
                        PPDifference.text = PP.ToString();
                        PPDifference.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                }
                else if(i == 20)
                {
                    string[] PlaceHolder = c.Split(',');
                    int[] Holder = new int[PlaceHolder.Length];
                    int j = 0;
                    foreach (string d in PlaceHolder)
                    {
                        Int32.TryParse(d, out Holder[j]);
                        j++;
                    }
                    DateTime TimeThen = new DateTime(Holder[0], Holder[1], Holder[2], Holder[3], Holder[4], Holder[5]);
                    Time = DateTime.Now - TimeThen;

                    //TimeNow = DateTime.Now;
                    if (Time.Days != 0)
                    {
                        TimeText.text = Time.Days + "d " + Time.Hours + "h " + Time.Minutes + "m Ago";
                    }
                    else if (Time.Hours != 0)
                    {
                        TimeText.text = Time.Hours + "h " + Time.Minutes + "m Ago";
                    }
                    else if (Time.Minutes != 0)
                    {
                        TimeText.text = Time.Minutes + "m Ago";
                    }
                    else if (Time.Seconds != 0)
                    {
                        TimeText.text =Time.Seconds + "s Ago";
                    }
                }
                #region OldCode
                /*
                else if (!Updated)
                {
                    if(TimeNow.Millisecond == 0)
                    {
                        string[] PlaceHolder = c.Split(',');
                        int[] Holder = new int[PlaceHolder.Length];
                        int j = 0;
                        foreach (string d in PlaceHolder)
                        {
                            Int32.TryParse(d, out Holder[j]);
                            j++;
                        }
                        Time = new TimeSpan(Holder[0], Holder[1], Holder[2], Holder[3]);
                        TimeNow = DateTime.Now;
                        Offline = true;
                    }
                    TimeSpan TimeGoneBy = DateTime.Now - TimeNow;
                    TimeNow = DateTime.Now;
                    Time = Time.Add(TimeGoneBy);
                    if (Time.Days != 0)
                    {
                        if(Offline)
                            TimeText.text = Time.Days + "d " + Time.Hours + "h " + Time.Minutes + "m Ago (Inaccurate)";
                        else
                            TimeText.text = Time.Days + "d " + Time.Hours + "h " + Time.Minutes + "m Ago";
                    }
                    else if (Time.Hours != 0)
                    {
                        if(Offline)
                            TimeText.text = Time.Hours + "h " + Time.Minutes + "m Ago (Inaccurate)";
                        else
                            TimeText.text = Time.Hours + "h " + Time.Minutes + "m Ago";
                    }
                    else if (Time.Minutes != 0)
                    {
                        if(Offline)
                            TimeText.text = Time.Minutes + "m Ago (Inaccurate)";
                        else
                            TimeText.text = Time.Minutes + "m Ago";
                    }
                    else if (Time.Seconds != 0)
                    {
                        if(Offline)
                        {
                            TimeText.text = Time.Seconds + "s Ago (Inaccurate)";
                        }
                        else
                        {
                            TimeText.text = Time.Seconds + "s Ago";
                        }
                    }
                }
                */
                #endregion
            }
            i++;
        }
    }
    public void ReplayIt()
    {
        TempOpponent.Opponent.Choices1 = Choices1;
        TempOpponent.Opponent.Choices2 = Choices2;
        TempOpponent.Opponent.Probs1 = Probs1;
        TempOpponent.Opponent.Probs2 = Probs2;
        TempOpponent.Opponent.AbLevelArray2 = Levels1;
        TempOpponent.Opponent.AbLevelArray = Levels2;
        TempOpponent.Opponent.Super1002 = Super100Levels1;
        TempOpponent.Opponent.Super100 = Super100Levels2;
        TempOpponent.Opponent.Super2002 = Super200Levels1;
        TempOpponent.Opponent.Super200 = Super200Levels2;
        TempOpponent.Opponent.ShapeID2 = ShapeID1;
        TempOpponent.Opponent.ShapeID = ShapeID2;
        TempOpponent.Opponent.Username = Username.text;
        TempOpponent.Opponent.Replay = true;
        TempOpponent.Opponent.Passives2 = Passives1;
        TempOpponent.Opponent.Passives = Passives2;
        TempOpponent.Opponent.SkinID = SkinID;
        TempOpponent.Opponent.SkinID2 = SkinID2;
        TempOpponent.Opponent.OpLvl = Level2;
        TempOpponent.Opponent.OpLvl2 = Level1;
        SceneManager.LoadScene("SpectateScene");
    }

    public void openProfile()
    {
        bool bot = false;
        if (profileVersion < 0)  //either a bot or uninitialized yet
        {
            string[] botProfiles = PlayerPrefsX.GetStringArray("BotProfiles");
            foreach (String s in botProfiles)
            {
                if (s.Split('|')[1] == Username.text)
                {
                    TempOpponent.Opponent.profile = s;
                    bot = true; break;
                }
            }
        }

        if (bot)
        {
            TempOpponent.Opponent.changed = true;
            TempOpponent.Opponent.GotProfile = true;
        }
        else
        {
            try
            {
                ClientTCP.PACKAGE_DEBUG("Profile", Username.text, profileVersion);
            }
            catch (Exception) { return; }
        }
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
        Transform temp = transform.parent.parent.parent;
        temp.GetComponent<ReplayController>().profilePanel.transform.parent.gameObject.SetActive(true);
        temp.GetComponent<ReplayController>().profilePanel.updateProfilePanel(info);
    }
}
