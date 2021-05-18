using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayController : MonoBehaviour
{
    public OpStat profilePanel;
    public GameObject replayPrefab;
    public Transform replayContainer;
    public int limit = 20;

    private GameObject Text;

    private void Awake()
    {
        Text = transform.Find("NoReplaysAvailable").gameObject;
        Text.SetActive(false);
    }

    public void Initialize()
    {
        foreach (Transform go in replayContainer)
        {
            Destroy(go.gameObject);
        }

        //bool Updated = PlayerPrefs.GetInt("Updated") == 1;
        string[] Rs = PlayerPrefsX.GetStringArray("Replay");
        List<string> AllUsernames = new List<string>();
        if (Rs[0] == "N")
        {
            Text.SetActive(true);
        }
        else
        {
            int i = 0;
            int j = 0;
            string[] ReplaysNew = new string[21];
            foreach (string R in Rs)
            {
                ReplaysNew[j] = R;
                if (j == 20)
                {
                    j = -1;
                    AllUsernames.Add(ReplaysNew[12]);
                    string RNew = string.Join("/", ReplaysNew);
                    ReplaysNew = new string[21];
                    GameObject curR = Instantiate(replayPrefab, replayContainer);
                    curR.GetComponent<Replay>().Initialize(RNew);
                    i++;
                    if (i == limit) { break; }
                }
                j++;
            }
            string[] botProfiles = PlayerPrefsX.GetStringArray("BotProfiles");
            List<string> NewProfiles = new List<string>();
            foreach (string s in botProfiles)
            {
                if (AllUsernames.Contains(s.Split('|')[1]))
                    NewProfiles.Add(s);
            }
            PlayerPrefsX.SetStringArray("BotProfiles", NewProfiles.ToArray());
        }
        //PlayerPrefs.SetInt("Updated", 0);
    }
}
