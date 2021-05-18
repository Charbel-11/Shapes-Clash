using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Endgame : MonoBehaviour {
    public GameObject OpAb;
    public bool firstTimeOp;
    public GameObject levelUpPanel;
    public GameObject ErrorOccured;
    public bool BotOnline = false;

    private GameMaster GM;
    private bool p1;

    private int goldGained, rbGained, XPGained;

    private void Start()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        p1 = transform.root.name == "EndGame";
        firstTimeOp = true;
    }

    public void UpdateEnd(int _goldGained, int _rbGained, int _XPGained)
    {
        goldGained = _goldGained;
        rbGained = _rbGained;
        XPGained = _XPGained;

        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        p1 = transform.root.name == "EndGame";

        if (GameMaster.Online)
        {
            transform.Find("Panel").Find("Button").gameObject.SetActive(GM.GetComponent<GameMasterOnline>().v2);
            transform.Find("Panel").Find("Double").gameObject.SetActive(!GM.GetComponent<GameMasterOnline>().v2 && GameMaster.Mode != 1);
        }
        else
        {
            transform.Find("Panel").Find("Button").gameObject.SetActive(false);
            transform.Find("Panel").Find("Double").gameObject.SetActive(PlayerPrefs.GetInt("BotOnline") == 1);
        }

        GetComponentInChildren<GetGold>().UpdateGold(goldGained);
        GetComponentInChildren<GetRB>().UpdateRB(rbGained);
        GetComponentInChildren<MyLevel>().UpdateLevel(XPGained);
        transform.Find("Panel").Find("PP Gained").GetComponent<MyPP>().UpdatePP(p1);
        transform.Find("Panel").Find("PP").GetComponent<MyPP>().UpdatePP(p1);

        transform.Find("Panel").transform.Find("Draw").gameObject.SetActive(GM.playerWon == 0);
        transform.Find("Panel").transform.Find("Victory").gameObject.SetActive(GM.playerWon == 1);
        transform.Find("Panel").transform.Find("Defeat").gameObject.SetActive(GM.playerWon == 2);

        if (!GameMaster.Online && !BotOnline) { transform.Find("Opponent's Abilities").GetComponent<Button>().interactable = false; }
        else { transform.Find("Opponent's Abilities").GetComponent<Button>().interactable = true; }
    }

    public void DoubleRewards()
    {
        PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + goldGained);
        PlayerPrefs.SetInt("Redbolts", PlayerPrefs.GetInt("Redbolts") + rbGained);
        PlayerPrefs.Save();

        GetComponentInChildren<GetGold>().UpdateGold(goldGained * 2);
        GetComponentInChildren<GetRB>().UpdateRB(rbGained * 2);

        transform.Find("Panel").Find("Double").gameObject.SetActive(false);

        ClientTCP.PACKAGE_ChestOpening();
    }

    public void MainMenu()
    {
        if (GameMaster.Online || GameMaster.botOnline)
        {
            TempOpponent.Opponent.Reset();
        }
        BotOnline = false;
        GameMaster.Reconnect = false;
        SceneManager.LoadScene("MainMenuScene");
    }
    public void PlayAgain()
    {
        GameMaster.Reconnect = false;
        if (GameMaster.Online)
        {
            TempOpponent.Opponent.Reset();
            ValuesChange.SceneNb = (GM.GetComponent<GameMasterOnline>().v2 ? 3 : 1);
        }
        else if (BotOnline)
        {
            TempOpponent.Opponent.Reset();
            ValuesChange.SceneNb = 1;
            BotOnline = false;
        }
        else
        {
            ValuesChange.SceneNb = 2;
        }
        SceneManager.LoadScene("MainMenuScene");
    }
    public void SeeAbilities()
    {
        OpAb.SetActive(true);
        if (firstTimeOp)
        {
            firstTimeOp = false;
            if (p1) GetComponentInChildren<OpStat>().UpdateOp();
            else GetComponentInChildren<OpStat>().UpdateOp2();
        }
    }

    public bool shapeLevelUp()
    {
        bool changed = false;
        int coins = PlayerPrefs.GetInt("Gold");
        int diamonds = PlayerPrefs.GetInt("Diamonds");
        int[] shapeLvls = PlayerPrefsX.GetIntArray("Level");      //In case we bought a new shape
        int[] shapeExp = PlayerPrefsX.GetIntArray("XP");

        int iCoin = coins, iDiamonds = diamonds;
        int[] iShapeLvls = shapeLvls, iXP = shapeExp;
        Stack<int> s = new Stack<int>();

        for (int i = 0; i < 4; i++)
        {
            if (shapeLvls[i] == 0 || shapeLvls[i] == ShapeConstants.maxLevel) { continue; }        //Accumulate XP without leveling up

            int needed = GameMaster.levelStats[shapeLvls[i] - 1][4][0];

            while (shapeExp[i] >= needed && shapeLvls[i] != ShapeConstants.maxLevel)
            {
                changed = true;
                shapeExp[i] -= needed;
                shapeLvls[i]++;

                coins += GameMaster.levelStats[shapeLvls[i] - 1][4][1];
                diamonds += GameMaster.levelStats[shapeLvls[i] - 1][4][2];

                s.Push(i);

                needed = GameMaster.levelStats[shapeLvls[i] - 1][4][0];
            }
        }
        if (changed)
        {         
            PlayerPrefsX.SetIntArray("Level", shapeLvls);
            PlayerPrefsX.SetIntArray("XP", shapeExp);
            PlayerPrefs.SetInt("Gold", coins);
            PlayerPrefs.SetInt("Diamonds", diamonds);
            PlayerPrefs.Save();

            while (s.Count > 0)
            {
                int cur = s.Pop();
                levelUpPanel.transform.GetChild(cur).gameObject.SetActive(true);
                levelUpPanel.transform.GetChild(cur).gameObject.GetComponent<LevelUp>().UpdateInfo();
            }

            try
            {
                ClientTCP.PACKAGE_ChestOpening();
            }
            catch (Exception)
            {
                for (int i = 0; i < 4; i++)
                {
                    levelUpPanel.transform.GetChild(i).gameObject.SetActive(false);
                }

                coins = iCoin; diamonds = iDiamonds;
                shapeLvls = iShapeLvls; shapeExp = iXP;

                PlayerPrefsX.SetIntArray("Level", shapeLvls);
                PlayerPrefsX.SetIntArray("XP", shapeExp);
                PlayerPrefs.SetInt("Gold", coins);
                PlayerPrefs.SetInt("Diamonds", diamonds);
                PlayerPrefs.Save();

                throw new Exception();
            }
        }

        return changed;
    }
}
