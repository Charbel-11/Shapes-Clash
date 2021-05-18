using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Shape_Abilities : MonoBehaviour
{
    public static bool choiceDoneP1;
    public static bool choiceDoneP2; 
    public static bool Tutorial = false;

    //Set by subclasses
    public int EP_Needed, ID;

    public bool P1, onlineP2, common;
    public int availableEP;
    public int AbLevel;
    public int[] AttArr, DefArr;    //stats
    public int AttPow, DefPow;      

    //0 above, 1 below, 2 straight
    public int[] attack, defense;       //practical values
    public bool[] escapeFrom;   

    public Button thisButt;
    public Image EPBgrd;
    public Text EPT;

    public Shape_Player player;

    public static List<int> notAttacksIDs = new List<int>() {0,2,7,8,9,11,14,16,18,24,25,26,33,34,36,37,41,44,49,54};

    public virtual void Awake()
    {
        if (transform.root.gameObject.name == "Abilities") { return; }
        AttArr = new int[3] { 0, 0, 0 };
        DefArr = new int[3] { 0, 0, 0 };

        //Gets the ability level for the online player
        if (transform.root.gameObject.name == "Player2")
        {
            onlineP2 = true;

            if (GameMaster.Online && !GameMaster.Replay && !GameMaster.Spectate)
            {
                if (GameObject.Find("Game Manager").GetComponent<GameMasterOnline>().p1Selected2 && !TempOpponent.Opponent.Abilities.ContainsKey(ID))
                    TempOpponent.Opponent.Abilities.Add(ID, UseAbility);
                else if (!TempOpponent.Opponent.Abilities12.ContainsKey(ID))
                    TempOpponent.Opponent.Abilities12.Add(ID, UseAbility);
            }
            else if(!TempOpponent.Opponent.Abilities.ContainsKey(ID))
                TempOpponent.Opponent.Abilities.Add(ID, UseAbility);

            if (ID < 100)
                Int32.TryParse(TempOpponent.Opponent.AbLevelArray[ID], out AbLevel);
            else if ((ID > 100) && (ID < 200))
                Int32.TryParse(TempOpponent.Opponent.Super100[ID - 101], out AbLevel);
            else if (ID > 200)
                Int32.TryParse(TempOpponent.Opponent.Super200[ID - 201], out AbLevel);
        }
        else   //Gets the ability level for the local player
        {
            if (!GameMaster.doneInit) { return; }
            onlineP2 = false;
            if (!(PlayerPrefs.GetInt("BotOnline") == 1))
            {
                if (ID < 100)
                    Int32.TryParse(GameMaster.AbLevelArray[ID], out AbLevel);
                else if ((ID > 100) && (ID < 200))
                    Int32.TryParse(GameMaster.Super100[ID - 101], out AbLevel);
                else if (ID > 200)
                    Int32.TryParse(GameMaster.Super200[ID - 201], out AbLevel);

            }
            else
            {
                if (ID < 100)
                    Int32.TryParse(TempOpponent.Opponent.AbLevelArray[ID], out AbLevel);
                else if ((ID > 100) && (ID < 200))
                    Int32.TryParse(TempOpponent.Opponent.Super100[ID - 101], out AbLevel);
                else if (ID > 200)
                    Int32.TryParse(TempOpponent.Opponent.Super200[ID - 201], out AbLevel);

            }
            //Needed?
            if (transform.root.gameObject.name == "Player1" && (GameMaster.Spectate || GameMaster.Replay) && !TempOpponent.Opponent.Abilities2.ContainsKey(ID))
                TempOpponent.Opponent.Abilities2.Add(ID, UseAbility);
            
            if (GameMaster.Spectate && transform.root.gameObject.name == "Canvas1")
            {
                //Still looks good?
                gameObject.GetComponent<Button>().interactable = false;

                //Purpose? what abt pics?
                string Name;
                if (!TempOpponent.Opponent.AbNames.TryGetValue(ID, out Name) && this.name != "Shape_Abilities")
                    TempOpponent.Opponent.AbNames.Add(ID, gameObject.name);
            }
        }

        //Gets the atck/def stat of the ability
        //When implemented and tested, remove all start functions from all the abilities.
        if (ID > 0 && ID < 100) {
            for (int i = 0; i < 3; i++)
            {
                AttArr[i] = GameMaster.StatsArr[ID][i];
                DefArr[i] = GameMaster.StatsArr[ID][i + 3];
            }
            EP_Needed = GameMaster.StatsArr[ID][6];
            if (Bot.EscapeAbilities.Contains(ID) && player is Sphere_Player)
                EP_Needed -= 1;
        }
        else if (ID >= 100 && ID < 200)
        {
            for(int i = 0; i < 3; i++)
            {
                AttArr[i] = GameMaster.Super100StatsArr[ID - 101][i];
                DefArr[i] = GameMaster.Super100StatsArr[ID - 101][i + 3];
            }
            EP_Needed = GameMaster.Super100StatsArr[ID - 101][6];
        }
        else if (ID >= 200)
        {
            for (int i = 0; i < 3; i++)
            {
                AttArr[i] = GameMaster.Super200StatsArr[ID - 201][i];
                DefArr[i] = GameMaster.Super200StatsArr[ID - 201][i + 3];
            }
            EP_Needed = GameMaster.Super200StatsArr[ID - 201][6];
        }
  
        if (AbLevel > 0)
        {
            AttPow = AttArr[AbLevel - 1];
            DefPow = DefArr[AbLevel - 1];
        }

        //Finds the player
        if (transform.root.name == "Canvas2" && GameMaster.Online)
        {
            P1 = true;
            player = GameObject.Find("Game Manager").GetComponent<GameMaster>().player12;
        }
        else if (transform.root.name == "Canvas1" && GameMaster.Online)
        {
            P1 = true;
            player = GameObject.Find("Game Manager").GetComponent<GameMaster>().player1;
        }
        else if (transform.root.gameObject.name == "Player2")
        {
            P1 = false;
            player = GetComponentInParent<Shape_Player>();
        }
        else if (transform.root.gameObject.name == "Canvas2" || transform.root.gameObject.name == "Bot")
        {
            P1 = false;
            player = GameObject.Find("Player2").GetComponent<Shape_Player>();
        }
        else
        {
            P1 = true;
            player = GameObject.Find("Player1").GetComponent<Shape_Player>();
        }

        //Fix the colors and interactibility
        if (transform.root.gameObject.name == "Canvas2" || transform.root.gameObject.name == "Canvas1")
        {
            thisButt = gameObject.GetComponent<Button>();
            thisButt.GetComponent<Image>().color = new Color(1f, 1f, 1f);

            Disabled();
        }
        if(transform.root.name == "Bot")
            thisButt = gameObject.GetComponent<Button>();

        if (ID > 0)     //0 is for Get_EP
        {
            if ((transform.root.gameObject.name == "Canvas1") || (transform.root.gameObject.name == "Canvas2"))
            {
                EPT = transform.Find("EP").GetComponent<Text>();
                EPT.text = EP_Needed.ToString();

                int idx = 0;
                if (player is Pyramid_Player)
                    idx = 1;
                else if (player is Star_Player)
                    idx = 2;
                else if (player is Sphere_Player)
                    idx = 3;

                if(ID < 100)
                {
                    EPBgrd = transform.Find("EPB").GetComponent<Image>();
                    EPBgrd.color = ShapeConstants.bckdAbEPColor[idx];
                }
            }
        }

        SetSpecs();
    }

    public virtual void Start() { }

    public virtual void SetSpecs() { }

    public virtual void UseAbility()
    {
        if ((P1 && choiceDoneP1) || (!P1 && choiceDoneP2)) { return; }

        player.SetIdOfAnimUSed(ID);
        player.SetEPToRemove(EP_Needed);
        if (player.escapeFrom.Length != 3)
            player.escapeFrom = new bool[3];
        escapeFrom.CopyTo(player.escapeFrom, 0);
        player.SetAttackPower(attack);
        player.SetShieldPower(defense);
        if(!(notAttacksIDs.Contains(ID)))
            player.SetBulletPower(AttPow,1);

        player.SetChoiceDone(true);

        if (((GameMaster.Online && !GameMaster.Spectate && !GameMaster.Replay) || PlayerPrefs.GetInt("BotOnline") == 1) && gameObject.transform.root.name != "Player1" && gameObject.transform.root.name != "Player2")
        {
            ClientTCP.PACKAGE_GiveChoice();
        }
    }

    public void Disabled()
    {
        if ((transform.root.gameObject.name == "Canvas1" || transform.root.gameObject.name == "Canvas2" || transform.root.name == "Bot") && !Tutorial && !GameMaster.Spectate)
        {
            availableEP = player.GetEP();
            if (availableEP < EP_Needed)
                thisButt.interactable = false;
            else
                thisButt.interactable = true;
        }
    }

    public virtual void updateState() { }

    public void DisableEscapes()
    {
        if (escapeFrom[0] || escapeFrom[1] || escapeFrom[2])
            thisButt.interactable = false;
    }

    public void DisableAll()
    {
        if (gameObject.name != "Get_EP" && !GameMaster.Spectate)
            thisButt.interactable = false;
    }

    public string getAttackType()
    {
        if (attack.Length == 0) { SetSpecs(); }
        if (attack[0] > 0) { return "Above"; }
        else if (attack[1] > 0) { return "Below"; }
        else if (attack[2] > 0) { return "Straight"; }
        return "";
    }
    public string getDefenseType()
    {
        if (defense.Length == 0) { SetSpecs(); }
        if (defense[0] > 0) { return "Above"; }
        else if (defense[1] > 0) { return "Below"; }
        else if (defense[2] > 0) { return "Straight"; }
        return "";
    }
    public string getEscapeType()
    {
        if (escapeFrom.Length == 0) { SetSpecs(); }
        if (escapeFrom[0] && escapeFrom[1] && escapeFrom[2]) { return "Ultimate"; }
        if (escapeFrom[1] && escapeFrom[2]) { return "Above"; }
        else if (escapeFrom[0] && escapeFrom[2]) { return "Below"; }
        else if (escapeFrom[0] && escapeFrom[1]) { return "Straight"; }
        return "";
    }
}