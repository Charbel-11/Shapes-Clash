using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using Random = System.Random;
using System;
using System.Diagnostics;

public class ChestSlot : MonoBehaviour {
    public static bool[] GotChest = new bool[] { false, false, false, false };
    private static int Counter = 0;

    public Sprite[] Sprites = new Sprite[6];
    public Chest TheChest;
    public bool Unlocked;
    public bool OpeningChest = false;
    public int ChestNum;
    public GameObject curChest;

    private int[] ChestSlotPrices;
    private int[] ChestObjects = new int[10];
    private int price;
    private int Timeleft = 0;
    public bool Go = false;
    private GameObject OpenButton, UnlockButton, OpenNowButton;
    private GameObject Lock;
    private Image img;
    private Text Timer, OpenNowCost;
    private Stopwatch Watch = new Stopwatch();
    private ValuesChange MenuManager;

    public void Start()
    {
        Go = false;
        img = GetComponent<Image>();
        Lock = transform.Find("Lock").gameObject;
        MenuManager = GameObject.Find("Menu Manager").GetComponent<ValuesChange>();

        OpenButton = transform.Find("Open").gameObject;
        UnlockButton = transform.Find("Unlock").gameObject;
        OpenButton.SetActive(false);

        OpenNowButton = transform.Find("OpenNow").gameObject;
        OpenNowCost = OpenNowButton.transform.Find("Cost").GetComponent<Text>();

        int[] LockedArray = PlayerPrefsX.GetIntArray("ChestSlots");
        if (LockedArray.Length == 0) { LockedArray = new int[4] { 0, 0, 0, 0 }; }
        Unlocked = LockedArray[ChestNum] == 1;
        Lock.SetActive(!Unlocked);
        UnlockButton.SetActive(!Unlocked);

        Timer = transform.Find("Timer").GetComponent<Text>();
        Timer.gameObject.SetActive(false);

        int[] curOpening = PlayerPrefsX.GetIntArray("OpeningChest");
        if (curOpening.Length == 4)
            OpeningChest = curOpening[ChestNum] == 1;
        else
        {
            int[] Array = new int[] { 0, 0, 0, 0 };
            PlayerPrefsX.SetIntArray("OpeningChest", Array);
            OpeningChest = false;
        }

        ChestSlotPrices = PlayerPrefsX.GetIntArray("ChestSlotPrices");
        price = ChestSlotPrices[ChestNum];
        UnlockButton.transform.Find("Cost").GetComponent<Text>().text = price.ToString();

        if (OpeningChest)
        {
            ClientTCP.PACKAGE_DEBUG("SendChest", PlayerPrefs.GetString("Username"), ChestNum);  //Gets us the time remaining
            string[] ChestArrayStr = PlayerPrefsX.GetStringArray("ChestContent");
            int[][] ChestAr = ClientHandleData.TransformStringArray(ChestArrayStr);
            ChestObjects = ChestAr[ChestNum];
            TheChest = new Chest(ChestObjects[8], ChestObjects[9]);
            img.sprite = Sprites[TheChest.SpriteNum];
            StartCoroutine(Wait());
        }

        Counter++;
        if (Counter == 4)
        {
            curChest.GetComponentInChildren<ChestCode>().init();
            Counter = 0;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitUntil(() => GotChest[ChestNum]);
        GotChest[ChestNum] = false;
        int[] TimeleftAr = PlayerPrefsX.GetIntArray("TimerAr");
        Timeleft = TimeleftAr[ChestNum] * 1000;
        Watch.Start();
        Go = true;
    }
    public IEnumerator Initialize(Chest ChestToOpen, bool Q = false)
    {
        TheChest = new Chest(ChestToOpen);  //Copy constructor
        if (!Q) { yield return new WaitForSeconds(1.3f); }  //Wait for the animation to end

        img.sprite = Sprites[TheChest.SpriteNum];
        Timeleft = TheChest.TimeToOpen * 1000;
        int[] TimerAr = PlayerPrefsX.GetIntArray("TimerAr");
        TimerAr[ChestNum] = Timeleft / 1000;
        PlayerPrefsX.SetIntArray("TimerAr", TimerAr);
        Watch.Start();

        SetUpChestComponent();  //Need this to store WS/AR for this chest
        OpeningChest = true; Go = true;

        int[] Opening = PlayerPrefsX.GetIntArray("OpeningChest");
        Opening[ChestNum] = 1;
        PlayerPrefsX.SetIntArray("OpeningChest", Opening);
        PlayerPrefs.Save();

        ClientTCP.PACKAGE_Chest(Timeleft / 1000, ChestNum);
    }

    void Update()
    {
        if (OpeningChest && Go)
        {
            Watch.Stop();
            Timeleft -= Convert.ToInt32(Watch.Elapsed.TotalMilliseconds);
            if (Timeleft < 0) { Timeleft = 0; }
            Watch.Restart();

            TimeSpan Ts = TimeSpan.FromSeconds(Timeleft / 1000);
            string str = Ts.ToString(@"hh\:mm\:ss");
            if (!Timer.gameObject.activeSelf && Timeleft > 0) {
                Timer.gameObject.SetActive(true);
                OpenNowButton.gameObject.SetActive(true);
            }
            Timer.text = str;
            OpenNowCost.text = (Timeleft / 120000).ToString();

            if (Timeleft == 0 && !OpenButton.activeSelf)
            {
                Timer.gameObject.SetActive(false);
                OpenNowButton.gameObject.SetActive(false);
                OpenButton.SetActive(true);
            }
        }
    }

    public void SetUpChestComponent()
    {
        ChestObjects[0] = TheChest.Gold;
        ChestObjects[1] = TheChest.RedBolts;
        ChestObjects[2] = TheChest.Diamonds;

        string[] Levels = PlayerPrefsX.GetStringArray("AbilitiesArray");
        int[] AbilitiesRarety = PlayerPrefsX.GetIntArray("AbilitiesRarety");
        int[] ShapeLevelArray = PlayerPrefsX.GetIntArray("Level");
        string[] Super100Levels = PlayerPrefsX.GetStringArray("Super100Array");
        string[] Super200Levels = PlayerPrefsX.GetStringArray("Super200Array");
        int[] LevelsArray = new int[Levels.Length];
        int[] Super100Array = new int[Super100Levels.Length];
        int[] Super200Array = new int[Super200Levels.Length];

        List<int> UnlockableCommonAbs = new List<int>();
        List<int> UnlockableRareAbs = new List<int>();
        List<int> UnlockableMythicAbs = new List<int>();
        List<int> UnlockableMajesticAbs = new List<int>();
        List<int> UnlockableShapes = new List<int>();

        int i = 0;
        foreach (string s in Levels)
        {
            Int32.TryParse(s, out LevelsArray[i]);
            if (LevelsArray[i] == 0 && (GetShape(i) == -1 || ShapeLevelArray[GetShape(i)] > 0))
            {
                if (AbilitiesRarety[i] == 0) { UnlockableCommonAbs.Add(i); }
                else if (AbilitiesRarety[i] == 1) { UnlockableRareAbs.Add(i); }
                else if (AbilitiesRarety[i] == 2) { UnlockableMythicAbs.Add(i); }
            }
            i++;
        }

        i = 0;
        foreach (int c in ShapeLevelArray)
        {
            if (c == 0) { UnlockableShapes.Add(i); }
            i++;
        }

        i = 0;
        foreach (string s in Super100Levels)
        {
            Int32.TryParse(s, out Super100Array[i]);
            if (Super100Array[i] == 0 && (GetShape(i + 101) == -1 || ShapeLevelArray[GetShape(i + 101)] > 0))
            {
                UnlockableMajesticAbs.Add(i + 101);
            }
            i++;
        }

        i = 0;
        foreach (string s in Super200Levels)
        {
            Int32.TryParse(s, out Super200Array[i]);
            if (Super200Array[i] == 0 && (GetShape(i + 201) == -1 || ShapeLevelArray[GetShape(i + 201)] > 0))
            {
                UnlockableMajesticAbs.Add(i + 201);
            }
            i++;
        }

        Random rand = new Random();
        if (Percentage(TheChest.CommonChance) && UnlockableCommonAbs.Count != 0)
        {
            int index = rand.Next(UnlockableCommonAbs.Count);
            ChestObjects[3] = UnlockableCommonAbs[index];
        }
        else { ChestObjects[3] = -1; }

        if (Percentage(TheChest.RareChance) && UnlockableRareAbs.Count != 0)
        {
            int index = rand.Next(UnlockableRareAbs.Count);
            ChestObjects[4] = UnlockableRareAbs[index];
        }
        else { ChestObjects[4] = -1; }

        if (Percentage(TheChest.MythicChance) && UnlockableMythicAbs.Count != 0)
        {
            int index = rand.Next(UnlockableMythicAbs.Count);
            ChestObjects[5] = UnlockableMythicAbs[index];
        }
        else { ChestObjects[5] = -1; }

        if (Percentage(TheChest.MajesticChance) && UnlockableMajesticAbs.Count != 0)
        {
            int index = rand.Next(UnlockableMajesticAbs.Count);
            ChestObjects[6] = UnlockableMajesticAbs[index];
        }
        else { ChestObjects[6] = -1; }

        if (Percentage(TheChest.NewShapeChance) && UnlockableShapes.Count != 0)
        {
            int index = rand.Next(UnlockableShapes.Count);
            ChestObjects[7] = UnlockableShapes[index];
        }
        else { ChestObjects[7] = -1; }

        ChestObjects[8] = TheChest.WS;
        ChestObjects[9] = TheChest.AR;

        string[] ChestInfoStr = PlayerPrefsX.GetStringArray("ChestContent");
        if (ChestInfoStr.Length == 0)
            ChestInfoStr = new string[] { String.Join(",", ChestObjects), "", "", "" };
        else
            ChestInfoStr[ChestNum] = String.Join(",", ChestObjects);

        PlayerPrefsX.SetStringArray("ChestContent", ChestInfoStr);
    }

    public void Open()
    {
        int[] Opening = PlayerPrefsX.GetIntArray("OpeningChest");
        Opening[ChestNum] = 0;
        OpeningChest = false;
        PlayerPrefsX.SetIntArray("OpeningChest", Opening);
        Chest C = new Chest(TheChest);

        SetUpChestComponent();

        int[] Queue = PlayerPrefsX.GetIntArray("Queue");
        if (Queue[0] != 0)
        {
            TheChest = new Chest(Queue[0], Queue[1]);
            StartCoroutine(Initialize(TheChest, true));
            if (Queue[2] != 0)
            {
                Queue[0] = Queue[2];
                Queue[1] = Queue[3];
                Queue[2] = 0;
                Queue[3] = 0;
            }
            else
            {
                Queue[0] = 0;
                Queue[1] = 0;
            }
        }
        else
        {
            TheChest = new Chest(0, 0);
            img.sprite = Sprites[5];
            Timer.gameObject.SetActive(false);
            OpenNowButton.gameObject.SetActive(false);
        }

        PlayerPrefsX.SetIntArray("Queue", Queue);
        OpenButton.SetActive(false);
        MenuManager.OpenIt(C, ChestObjects);
        MenuManager.Awake();
    }

    //Open with diamonds without waiting
    public void OpenNow()
    {
        int dCost = Timeleft / 120000;
        int curDiamonds = ValuesChange.diamonds;

        if (curDiamonds < dCost)
        {
            MenuManager.ShowMessage("You don't have enough resources");
            return;
        }

        curDiamonds -= dCost; ValuesChange.diamonds = curDiamonds;
        PlayerPrefs.SetInt("Diamonds", curDiamonds);

        try { Open(); }
        catch (Exception)
        {
            curDiamonds += dCost;
            ValuesChange.diamonds = curDiamonds;
            PlayerPrefs.SetInt("Diamonds", curDiamonds);
        }

        PlayerPrefs.Save();
    }

    public void BuyChestSlot()
    {
        int curCoins = ValuesChange.coins;
        int curDiamonds = ValuesChange.diamonds;
        int[] LockedArray = PlayerPrefsX.GetIntArray("ChestSlots");

        if (ChestNum < 3 && curCoins >= price)
        {
            curCoins -= price; ValuesChange.coins = curCoins;
            PlayerPrefs.SetInt("Gold", curCoins);
        }
        else if (ChestNum == 3 && curDiamonds >= price)
        {
            curDiamonds -= price; ValuesChange.diamonds = curDiamonds;
            PlayerPrefs.SetInt("Diamonds", curDiamonds);
        }
        else
        {
            MenuManager.ShowMessage("You don't have enough resources");
            return;
        }

        LockedArray[ChestNum] = 1;
        PlayerPrefsX.SetIntArray("ChestSlots", LockedArray);
        PlayerPrefs.Save();

        try { ClientTCP.PACKAGE_ChestOpening(); }
        catch (Exception)
        {
            revertChanges1();
            MenuManager.showError();
            return;
        }

        Start();
        MenuManager.Awake();
    }
    public void revertChanges1()
    {
        int curCoins = ValuesChange.coins;
        int curDiamonds = ValuesChange.diamonds;
        int[] LockedArray = PlayerPrefsX.GetIntArray("ChestSlots");

        if (ChestNum < 3 && curCoins >= price)
        {
            curCoins += price; ValuesChange.coins = curCoins;
            PlayerPrefs.SetInt("Gold", curCoins);
        }
        else if (ChestNum == 3 && curDiamonds >= price)
        {
            curDiamonds += price; ValuesChange.diamonds = curDiamonds;
            PlayerPrefs.SetInt("Diamonds", curDiamonds);
        }

        PlayerPrefsX.SetIntArray("ChestSlots", LockedArray);
        PlayerPrefs.Save();
    }

    private bool Percentage(int Percent)
    {
        if (Percent == 0) { return false; }
        Random rand = new Random();
        int chance = rand.Next(1, 101);
        return chance <= Percent;
    }

    private int GetShape(int abID)
    {
        if (ShapeConstants.CubeOnly.Contains(abID))
            return 0;
        else if (ShapeConstants.PyrOnly.Contains(abID))
            return 1;
        else if (ShapeConstants.StarOnly.Contains(abID))
            return 2;
        else if (ShapeConstants.SphereOnly.Contains(abID))
            return 3;
        else
            return -1;
    }
}
