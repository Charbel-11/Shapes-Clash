using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayOffline : MonoBehaviour
{

    void Start()
    {
        if (CloudOnce.CloudVariables.Username == "N")
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }

    public void HandleOfflineLogin()
    {
        int i = 0;
        bool firsttime = true;
        string Username = CloudOnce.CloudVariables.Username;
        int Gold = CloudOnce.CloudVariables.Gold;
        string Level = CloudOnce.CloudVariables.ShapeLvl;
        string[] Lvl1 = Level.Split(',');
        int[] LevelArray = new int[Lvl1.Length];
        foreach (string c in Lvl1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out LevelArray[i]);
            i++;
        }
        firsttime = true;
        string XP = CloudOnce.CloudVariables.XP;
        string[] XP1 = XP.Split(',');
        int[] XPArray = new int[XP1.Length];
        foreach (string c in XP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out XPArray[i]);
            i++;
        }
        firsttime = true;
        string PP = CloudOnce.CloudVariables.PP;
        string[] PP1 = PP.Split(',');
        int[] PPArray = new int[PP1.Length];
        foreach (string c in PP1)
        {
            if (firsttime)
            {
                i = 0;
                firsttime = false;
            }
            Int32.TryParse(c, out PPArray[i]);
            i++;
        }
        firsttime = true;
        string Abilities = CloudOnce.CloudVariables.Abilities;
        string[] AbilitiesArray = Abilities.Split(',');
        string Super100 = CloudOnce.CloudVariables.Super100;
        string[] Super100Array = Super100.Split(',');
        string Super200 = CloudOnce.CloudVariables.Super200;
        string[] Super200Array = Super100.Split(',');
        string Stats = CloudOnce.CloudVariables.Stats;
        string[] StatsArraytemp = Stats.Split('-');
        string Super100Stats = CloudOnce.CloudVariables.Super100Stats;
        string[] Super100StatsArraytemp = Super100Stats.Split('-');
        string Super200Stats = CloudOnce.CloudVariables.Super200Stats;
        string[] Super200StatsArraytemp = Super200Stats.Split('-');
        string Code = CloudOnce.CloudVariables.Code;
        i = 0;
        string Pas = CloudOnce.CloudVariables.Passives;
        string[] PassivesStr = Pas.Split(',');
        int[] Passives = new int[PassivesStr.Length];
        foreach (string c in PassivesStr)
        {
            Int32.TryParse(c, out Passives[i]);
            i++;
        }
        /*string Replay = CloudOnce.CloudVariables.Replay;
        string[] Replays = Replay.Split('|');
        string[] ReplayNew;
        if (Replays[0] != "N")
        {
            string[] Ar = Replays[0].Split('/');
            ReplayNew = new string[Ar.Length * Replays.Length];
            i = 0;
            foreach (string c in Replays)
            {
                string[] d = c.Split('/');
                foreach (string e in d)
                {
                    ReplayNew[i] = e;
                    i++;
                }
            }
        }
        else
        {
            ReplayNew = new string[] { "N" };
        }*/
        string LevelStats = CloudOnce.CloudVariables.LevelStats;
        int Redbolts = CloudOnce.CloudVariables.Redbolts;
        int Diamonds = CloudOnce.CloudVariables.Diamonds;
        string Bg = CloudOnce.CloudVariables.Backgrounds;
        string[] BackgroundsStr = Bg.Split(',');
        int[] Backgrounds = new int[BackgroundsStr.Length];
        i = 0;
        foreach (string s in BackgroundsStr)
        {
            Int32.TryParse(s, out Backgrounds[i]);
            i++;
        }
        string SkinsUnlocked = CloudOnce.CloudVariables.Skins;
        string[] SkinsUnlockedStr = SkinsUnlocked.Split(',');
        int[] SkinsUnlockedAr = new int[SkinsUnlockedStr.Length];
        i = 0;
        foreach (string s in SkinsUnlockedStr)
        {
            Int32.TryParse(s, out SkinsUnlockedAr[i]);
            i++;
        }
        string EmotesUnlocked = CloudOnce.CloudVariables.Emotes;
        string[] EmotesUnlockedStr = EmotesUnlocked.Split(',');
        int[] EmotesUnlockedAr = new int[EmotesUnlockedStr.Length];
        i = 0;
        foreach (string s in EmotesUnlockedStr)
        {
            Int32.TryParse(s, out EmotesUnlockedAr[i]);
            i++;
        }
        string maxPPstr = CloudOnce.CloudVariables.MaxPP;
        string[] maxPPstrAr = maxPPstr.Split(',');
        int[] maxPP = new int[maxPPstrAr.Length];
        i = 0;
        foreach (string s in maxPPstrAr)
        {
            Int32.TryParse(s, out maxPP[i]);
            i++;
        }
        SelectionManager.online = false;
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Redbolts", Redbolts);
        PlayerPrefs.SetInt("Diamonds", Diamonds);
        //PlayerPrefsX.SetStringArray("Replay", ReplayNew);
        PlayerPrefsX.SetIntArray("Level", LevelArray);
        PlayerPrefsX.SetIntArray("XP", XPArray);
        PlayerPrefs.SetString("Username", Username);
        PlayerPrefs.SetString("Code", Code);
        PlayerPrefsX.SetIntArray("PP", PPArray);
        PlayerPrefsX.SetStringArray("AbilitiesArray", AbilitiesArray);
        PlayerPrefsX.SetStringArray("Super100Array", Super100Array);
        PlayerPrefsX.SetStringArray("Super200Array", Super200Array);
        PlayerPrefsX.SetStringArray("StatsArray", StatsArraytemp);
        PlayerPrefsX.SetStringArray("Super100StatsArray", Super100StatsArraytemp);
        PlayerPrefsX.SetStringArray("Super200StatsArray", Super200StatsArraytemp);
        PlayerPrefsX.SetIntArray("PassivesArray", Passives);
        PlayerPrefs.SetInt("Offline", 0);
        PlayerPrefs.SetString("LevelStats", LevelStats);
        PlayerPrefsX.SetIntArray("Backgrounds", Backgrounds);
        PlayerPrefsX.SetIntArray("SkinsUnlockedAr", SkinsUnlockedAr);
        PlayerPrefsX.SetIntArray("EmotesUnlockedAr", EmotesUnlockedAr);
        PlayerPrefsX.SetIntArray("MaxPP", maxPP);
        // REMOVE LATER
        PlayerPrefsX.SetIntArray("Level", new int[] { 1, 1, 1, 1 });
        PlayerPrefsX.SetStringArray("AbilitiesArray", "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1".Split(','));
        //
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenuScene");
    }
}
