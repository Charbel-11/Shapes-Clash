using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ChestCode : MonoBehaviour
{
    //Used to deal with the case where we force open once the WS is over
    public static int[] OldRecentShapeIDs;      
    public static int OldWS;
    public static bool OpenIt = false;

    public Sprite[] Sprites = new Sprite[6];
    public ChestSlot[] ChestSlots;
    public GameObject ChestSlotsContainer, Queue;
    public GameObject InfoButton, OpenButton;
    public Image ChestImage;                //Image of the chest used for the animation (different than the button's pic)

    private Text WinstreakText;
    private GameObject ShowQueueButton;
    private Button ThisChestBut;
    private Animator Anim;
    private Chest TheChest;
    private bool ShowIt = true;

    //Returns the PP of the most used shape for this WS (in case of tie, use the shape with highest PP)
    public static int FindPPOfMostUsedShape(bool old = false)
    {
        int[] RecentShapeIDs = PlayerPrefsX.GetIntArray("RecentIDs");
        int[] shapePPs = PlayerPrefsX.GetIntArray("PP");

        int[] freq = new int[4];
        if (!old) {
            foreach (int c in RecentShapeIDs) {
                if (c == -1) { break; }
                freq[c]++;
            }
        }
        else {
            foreach (int c in OldRecentShapeIDs) {
                if (c == -1) { break; }
                freq[c]++;
            }
        }

        int maxIdx = 0;
        for (int i = 1; i < 4; i++)
        {
            if (freq[i] > freq[maxIdx]) { maxIdx = i; }
            else if (freq[i] == freq[maxIdx] && shapePPs[i] > shapePPs[maxIdx]) { maxIdx = i; }
        }

        return shapePPs[maxIdx];
    }

    public void init()
    {
        WinstreakText = transform.parent.Find("Winstreak").GetComponentInChildren<Text>();
        ShowQueueButton = transform.Find("ShowQueue").gameObject;
        ThisChestBut = gameObject.GetComponent<Button>();
        Anim = gameObject.GetComponent<Animator>();

        if (OpenIt)    //Force open in case the win streak is over
        {
            int Arena = (FindPPOfMostUsedShape(true) / ShapeConstants.ArenaPP) + 1;
            TheChest = new Chest(OldWS, Arena);
            OpenChest();

            OpenIt = false;
        }
        else { ChangeWinstreak(); }
    }

    public void ChangeWinstreak()
    {
        int WS = PlayerPrefs.GetInt("Winstreak");
        WinstreakText.text = WS.ToString();

        int Arena = (FindPPOfMostUsedShape() / ShapeConstants.ArenaPP) + 1;
        TheChest = new Chest(WS, Arena);
        ThisChestBut.image.sprite = Sprites[TheChest.SpriteNum];

        if (WS >= 12) { OpenChest(false); } //If WS >= 12 and queue + chest slots are full, nothing will happen 
    }

    //Called when we press on the chest
    public void ChangeState()
    {
        if (InfoButton.gameObject.activeSelf)
        {
            InfoButton.gameObject.SetActive(false);
            OpenButton.SetActive(false);
            ShowQueueButton.SetActive(true);
        }
        else
        {
            ShowQueueButton.SetActive(false);
            InfoButton.gameObject.SetActive(true);
            if (WinstreakText.text != "0") { OpenButton.SetActive(true); }
        }
    }

    public void OpenChest(bool msg = true)
    {
        int SlotNumber = -1;
        foreach (ChestSlot Slot in ChestSlots)
        {
            if (Slot.Unlocked && !Slot.OpeningChest)
            {
                SlotNumber = Slot.ChestNum;
                break;
            }
        }

        if (SlotNumber != -1)
        {
            ChestImage.gameObject.SetActive(true);
            ChestImage.sprite = Sprites[TheChest.SpriteNum];
            ThisChestBut.image.sprite = Sprites[5];
            WinstreakText.text = "0";

            Anim.SetInteger("ID", SlotNumber);
            Invoke("ResetIt", 1.3f);
            StartCoroutine(ChestSlots[SlotNumber].Initialize(TheChest));        

            PlayerPrefs.SetInt("Winstreak", 0);
            PlayerPrefsX.SetIntArray("RecentIDs", new int[] { -1 });
            ClientTCP.PACKAGE_DEBUG("ResetW", PlayerPrefs.GetString("Username")); //Resets recentID & WS in the server
            ChangeState();
        }
        else
        {
            //First 2 numbers are for WS/Arena of the 1st chest, Last 2 numbers are for the 2nd chest
            int[] Queue = PlayerPrefsX.GetIntArray("Queue");
            if (Queue.Length != 4) { Queue = new int[] { 0, 0, 0, 0 }; }

            int q = -1;
            if (Queue[0] == 0) { q = 0; }
            else if (Queue[2] == 0) { q = 2; }

            if (q != -1)  //Found place in queue
            {
                Queue[q] = TheChest.WS;
                Queue[q + 1] = TheChest.AR;
                ThisChestBut.image.sprite = Sprites[5];
                WinstreakText.text = "0";

                PlayerPrefs.SetInt("Winstreak", 0);
                PlayerPrefsX.SetIntArray("RecentIDs", new int[] { -1 });
                ClientTCP.PACKAGE_DEBUG("ResetW", PlayerPrefs.GetString("Username"));   //Resets recentID & WS in the server
                ChangeState();
            }
            else if (msg)   //No place, send the message if needed
            {
                GameObject.Find("Menu Manager").GetComponent<ValuesChange>().ShowMessage("You don't have enough space for this chest");
            }

            PlayerPrefsX.SetIntArray("Queue", Queue);
        }
    }

    private void ResetIt()
    {
        Anim.SetInteger("ID", -1);
    }

    public void ShowQueue()
    {
        if (ShowIt)
        {
            ChestSlotsContainer.SetActive(false);
            Queue.SetActive(true);
            ShowQueueButton.GetComponentInChildren<Text>().text = "Hide Queue";
            ShowIt = false;
        }
        else
        {
            Queue.SetActive(false);
            ChestSlotsContainer.SetActive(true);
            ShowQueueButton.GetComponentInChildren<Text>().text = "Show Queue";
            ShowIt = true;
        }
    }
}

public class Chest
{
    public int LowerBoundGold, UpperBoundGold;
    public int LowerBoundRedBolts, UpperBoundRedBolts;
    public int LowerBoundDiamonds, UpperBoundDiamonds;
    public int Gold, RedBolts, Diamonds;
    public int CommonChance, RareChance, MythicChance, MajesticChance;
    public int NewShapeChance;
    public int SpriteNum;
    public int TimeToOpen;
    public int WS;
    public int AR;

    //Takes Winstreak and Arena as parameters and sets the specs of the corresponding chest
    public Chest(int Winstreak, int Arena)
    {
        WS = Winstreak; AR = Arena;
        Random Rand = new Random();

        if (Winstreak == 0)     //No chest
        {
            LowerBoundGold = UpperBoundGold = 0;
            LowerBoundRedBolts = UpperBoundRedBolts = 0;
            LowerBoundDiamonds = UpperBoundDiamonds = 0;
            Gold = RedBolts = Diamonds = 0;
            CommonChance = RareChance = MythicChance = MajesticChance = 0;
            NewShapeChance = 0;
            SpriteNum = 5;
        }
        else if (Winstreak >= 1 && Winstreak < 3)
        {
            LowerBoundGold = 50 + (Arena - 1) * 50;
            UpperBoundGold = 100 + (Arena - 1) * 50;
            LowerBoundRedBolts = 2 + (Arena - 1) * 4;
            UpperBoundRedBolts = 6 + (Arena - 1) * 4;
            LowerBoundDiamonds = 0;
            UpperBoundDiamonds = 2 + (Arena - 1) * 2;
            Gold = Rand.Next(LowerBoundGold, UpperBoundGold + 1);
            RedBolts = Rand.Next(LowerBoundRedBolts, UpperBoundRedBolts + 1);
            Diamonds = Rand.Next(LowerBoundDiamonds, UpperBoundDiamonds + 1);
            CommonChance = 0;
            RareChance = 0;
            MythicChance = 0;
            MajesticChance = 0;
            NewShapeChance = 0;
            SpriteNum = 0;
            TimeToOpen = 1200;
        }
        else if (Winstreak < 6 && Winstreak >= 3)
        {
            LowerBoundGold = 100 + (Arena - 1) * 50;
            UpperBoundGold = 150 + (Arena - 1) * 50;
            LowerBoundRedBolts = 6 + (Arena - 1) * 4;
            UpperBoundRedBolts = 10 + (Arena - 1) * 4;
            LowerBoundDiamonds = 4;
            UpperBoundDiamonds = 8 + (Arena - 1) * 2;
            Gold = Rand.Next(LowerBoundGold, UpperBoundGold + 1);
            RedBolts = Rand.Next(LowerBoundRedBolts, UpperBoundRedBolts + 1);
            Diamonds = Rand.Next(LowerBoundDiamonds, UpperBoundDiamonds + 1);
            CommonChance = 15 + (Arena - 1) * 10;
            RareChance = 5 + (Arena - 1) * 5;
            MythicChance = 0;
            MajesticChance = 0;
            NewShapeChance = 0;
            SpriteNum = 1;
            TimeToOpen = 2400;
        }
        else if (Winstreak < 9 && Winstreak >= 6)
        {
            LowerBoundGold = 200 + (Arena - 1) * 50;
            UpperBoundGold = 300 + (Arena - 1) * 50;
            LowerBoundRedBolts = 14 + (Arena - 1) * 4;
            UpperBoundRedBolts = 20 + (Arena - 1) * 4;
            LowerBoundDiamonds = 8;
            UpperBoundDiamonds = 16 + (Arena - 1) * 2;
            Gold = Rand.Next(LowerBoundGold, UpperBoundGold + 1);
            RedBolts = Rand.Next(LowerBoundRedBolts, UpperBoundRedBolts + 1);
            Diamonds = Rand.Next(LowerBoundDiamonds, UpperBoundDiamonds + 1);
            CommonChance = 30 + (Arena - 1) * 10;
            RareChance = 15 + (Arena - 1) * 5;
            MythicChance = 10 + (Arena - 1) * 3;
            MajesticChance = 0;
            NewShapeChance = 0;
            SpriteNum = 2;
            TimeToOpen = 5400;
        }
        else if (Winstreak < 12 && Winstreak >= 9)
        {
            LowerBoundGold = 350 + (Arena - 1) * 50;
            UpperBoundGold = 500 + (Arena - 1) * 50;
            LowerBoundRedBolts = 24 + (Arena - 1) * 4;
            UpperBoundRedBolts = 34 + (Arena - 1) * 4;
            LowerBoundDiamonds = 14;
            UpperBoundDiamonds = 20 + (Arena - 1) * 2;
            Gold = Rand.Next(LowerBoundGold, UpperBoundGold + 1);
            RedBolts = Rand.Next(LowerBoundRedBolts, UpperBoundRedBolts + 1);
            Diamonds = Rand.Next(LowerBoundDiamonds, UpperBoundDiamonds + 1);
            CommonChance = 60 + (Arena - 1) * 10;
            RareChance = 30 + (Arena - 1) * 5;
            MythicChance = 20 + (Arena - 1) * 3;
            MajesticChance = 10 + (Arena - 1) * 1;
            NewShapeChance = 0;
            SpriteNum = 3;
            TimeToOpen = 9000;
        }
        else if (Winstreak == 12)
        {
            LowerBoundGold = 500 + (Arena - 1) * 50;
            UpperBoundGold = 800 + (Arena - 1) * 50;
            LowerBoundRedBolts = 32 + (Arena - 1) * 4;
            UpperBoundRedBolts = 44 + (Arena - 1) * 4;
            LowerBoundDiamonds = 20;
            UpperBoundDiamonds = 30 + (Arena - 1) * 2;
            Gold = Rand.Next(LowerBoundGold, UpperBoundGold + 1);
            RedBolts = Rand.Next(LowerBoundRedBolts, UpperBoundRedBolts + 1);
            Diamonds = Rand.Next(LowerBoundDiamonds, UpperBoundDiamonds + 1);
            CommonChance = 100;
            RareChance = 75 + (Arena - 1) * 5;
            MythicChance = 50 + (Arena - 1) * 3;
            MajesticChance = 20 + (Arena - 1) * 1;
            NewShapeChance = 10 + (Arena - 1) * 1;
            SpriteNum = 4;
            TimeToOpen = 14400;
        }
    }
    public Chest(Chest otherChest)
    {
        LowerBoundGold = otherChest.LowerBoundGold;
        UpperBoundGold = otherChest.UpperBoundGold;
        LowerBoundRedBolts = otherChest.LowerBoundRedBolts;
        UpperBoundRedBolts = otherChest.UpperBoundRedBolts;
        LowerBoundDiamonds = otherChest.LowerBoundDiamonds;
        UpperBoundDiamonds = otherChest.UpperBoundDiamonds;
        Gold = otherChest.Gold;
        RedBolts = otherChest.RedBolts;
        Diamonds = otherChest.Diamonds;
        CommonChance = otherChest.CommonChance;
        RareChance = otherChest.RareChance;
        MythicChance = otherChest.MythicChance;
        MajesticChance = otherChest.MajesticChance;
        NewShapeChance = otherChest.NewShapeChance;
        SpriteNum = otherChest.SpriteNum;
        TimeToOpen = otherChest.TimeToOpen;
        WS = otherChest.WS;
        AR = otherChest.AR;
    }
}
