using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class ValuesChange : MonoBehaviour
{
    public static bool offline;

    public GameObject topBar;
    public Sprite[] ShapeXP;
    public SelectedShape curShape;

    public GameObject PlayButton, RoadButton, RewardButton;

    public static int coins;
    public static int redBolts;
    public static int diamonds;

    public static int[] maxShapePPs;
    public static int[] shapePPs;
    public static int[] shapeLvls;
    public static int[] shapeExp;

    private string levelStatsStr;
    public static int[][][] levelStats;     //[i][j][k]: i for levels; j from 0 to 3 for shapes (where k for health,atck,def or other subgrp), 4 for level up (where k xp/gold/diamond)

    public static int selectedShapeIndex;

    //Glory Road
    public static int maxPP, toClaim;

    private string trophyRoadStr;
    private string[] trophyRoadUnlockedStr;
    public static int[][][] trophyRoadStats;
    public static int[][] trophyRoadUnlocked;
    public static string[] AbLevelArray;

    public GameObject mainRoad, shapeRoad;
    public GameObject[] checkpointContainer;
    public GameObject notification;
    public GameObject AllAbilities;
    public TrophyRoadSwitchShape SwitchShape;

    public GameObject ResourcesAnim;

    public GameObject ErrorPanel;

    //Chest System
    public GameObject Canvas;
    public GameObject Cam;
    public GameObject CamChest;
    public OpenChest Chest;
    private bool Go = true;
    private int i = 0;
    private bool First = true;
    public ChestSlot[] Slots;

    //Main Menu
    public Button Shop;
    public Button Practice;
    public Button BattleOnline;
    public Button Friends;
    public Button Leaderboard;
    public Button Battle2v2;
    public GameObject ChestGrp;
    public GameObject ChestCur;
    public Button MainMenu;

    public GameObject[] BattleOnlineObj;

    public GameObject Choose2Players, ChooseScreen;
    public static int SceneNb = 0;

    public static Dictionary<string, UnityEngine.Events.UnityEvent> SwitchScenes = new Dictionary<string, UnityEngine.Events.UnityEvent>();

    private List<int> ResetList = new List<int>();

    //Ads
    public bool usedP1 = false;
    public GameObject AdsBut1, AdsBut2; //1 is for diamonds, 2 is for c/rb
    public Text remaining1Text, remaining2Text, timeLeftText;
    private int remaining1, remaining2;
    private int timeLeftBeforeReset;
    private Stopwatch Watch = new Stopwatch();

    //For offline design
    public GameObject ShapeGO, GameModes;
    public GameObject Message;

    public GameObject dailyrew;
    public GameObject addrew;

    public void Awake()
    {
        Screen.SetResolution(1920, 1080, true);

        ErrorPanel.SetActive(false);

        SwitchScenes.Clear();
        SwitchScenes.Add("ChoosingScene", BattleOnline.onClick);
        SwitchScenes.Add("Choose2PlayersScreen", Battle2v2.onClick);
        SwitchScenes.Add("MainMenu", MainMenu.onClick);

        if (PlayerPrefs.GetInt("Offline") == 0)
        {
            offline = true;

            Shop.interactable = false;
            BattleOnline.interactable = false;
            Friends.interactable = false;
            Leaderboard.interactable = false;
            Battle2v2.interactable = false;
            ChestGrp.SetActive(false);
            ChestCur.SetActive(false);
            foreach (GameObject G in BattleOnlineObj)
                G.SetActive(false);

            ShapeGO.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.15f, 0.26f);
            ShapeGO.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.375f, 0.635f);
            GameModes.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.39f, 0.26f);
            GameModes.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.875f, 0.635f);
        }
        else
        {
            offline = false;

            Shop.interactable = true;
            BattleOnline.interactable = true;
            Friends.interactable = true;
            Leaderboard.interactable = true;
            //Battle2v2.interactable = true;
            ChestGrp.SetActive(true);
            //            ChestCur.SetActive(true);

            if (PlayerPrefs.GetInt("DailyReward") == 1)
                dailyrew.SetActive(true);

            int ConsDays = PlayerPrefs.GetInt("ConsDays");
            int[] addrewards = PlayerPrefsX.GetIntArray("AdditionalRewards");
            for(int j = 0; j <= 80; j += 10)
            {
                if (ConsDays >= (20 + j) && addrewards[(int)(ConsDays / 10) - 2] == 1)
                    addrew.SetActive(true);
            }

            ShapeGO.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.15f, 0.45f);
            ShapeGO.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.375f, 0.825f);
            GameModes.transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.39f, 0.45f);
            GameModes.transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.875f, 0.825f);
        }

        if (SceneNb == 1)
        {
            BattleOnline.onClick.Invoke();
            transform.parent.gameObject.SetActive(false);
        }
        else if (SceneNb == 2)
        {
            Practice.onClick.Invoke();
            transform.parent.gameObject.SetActive(false);
        }
        else if (SceneNb == 3)
        {
            Battle2v2.onClick.Invoke();
            transform.parent.gameObject.SetActive(false);
        }
        SceneNb = 0;

        selectedShapeIndex = PlayerPrefs.GetInt("ShapeSelectedID");    //0 for Cube, 1 for pyramid, ...
        curShape.UpdateShape();

        coins = PlayerPrefs.GetInt("Gold");
        redBolts = PlayerPrefs.GetInt("Redbolts");
        diamonds = PlayerPrefs.GetInt("Diamonds");
        shapeLvls = PlayerPrefsX.GetIntArray("Level");
        shapeExp = PlayerPrefsX.GetIntArray("XP");
        shapePPs = PlayerPrefsX.GetIntArray("PP");
        maxShapePPs = PlayerPrefsX.GetIntArray("MaxPP");

        AbLevelArray = PlayerPrefsX.GetStringArray("AbilitiesArray");

        levelStatsStr = PlayerPrefs.GetString("LevelStats");
        levelStats = ClientHandleData.TransformToArrayShop(levelStatsStr);

        trophyRoadStr = PlayerPrefs.GetString("TrophyRoad");
        trophyRoadUnlockedStr = PlayerPrefsX.GetStringArray("TrophyRoadUnlocked");
        trophyRoadStats = ClientHandleData.TransformToArrayShop(trophyRoadStr);
        trophyRoadUnlocked = ClientHandleData.TransformStringArray(trophyRoadUnlockedStr);
        

        Transform XP = topBar.transform.Find("XP");
        Transform Shape = XP.transform.Find("Shape");
        Shape.Find("Level").GetComponent<Text>().text = shapeLvls[selectedShapeIndex].ToString();
        Shape.GetComponent<Image>().sprite = ShapeXP[selectedShapeIndex];
        if (selectedShapeIndex == 0)
        {
            Shape.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.2f, .3f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.8f, .7f);
        }
        else if (selectedShapeIndex == 1)
        {
            Shape.transform.localScale = new Vector3(1f, 1f, 1f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.1f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.225f, .15f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.775f, .65f);
        }
        else if (selectedShapeIndex == 2)
        {
            Shape.transform.localScale = new Vector3(1f, 1f, 1f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.1f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.3f, .25f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.7f, .7f);
        }
        else if (selectedShapeIndex == 3)
        {
            Shape.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            Shape.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMin = new Vector2(.2f, .3f);
            Shape.Find("Level").GetComponent<RectTransform>().anchorMax = new Vector2(.8f, .7f);
        }

        XP.Find("Bar").GetComponent<Slider>().maxValue = (shapeLvls[selectedShapeIndex] == ShapeConstants.maxLevel ? shapeExp[selectedShapeIndex] : levelStats[shapeLvls[selectedShapeIndex] - 1][4][0]);
        XP.Find("Bar").GetComponent<Slider>().value = shapeExp[selectedShapeIndex];
        XP.Find("Bar").transform.Find("Text").GetComponent<Text>().text = shapeExp[selectedShapeIndex].ToString() + (shapeLvls[selectedShapeIndex] == ShapeConstants.maxLevel ? "" : "/" + levelStats[shapeLvls[selectedShapeIndex] - 1][4][0].ToString());

        topBar.transform.Find("Coins").GetComponentInChildren<Text>().text = coins.ToString();
        topBar.transform.Find("Red Bolts").GetComponentInChildren<Text>().text = redBolts.ToString();
        topBar.transform.Find("Diamonds").GetComponentInChildren<Text>().text = diamonds.ToString();

        int PP = shapePPs[selectedShapeIndex];
        int i = 0;
        while (i < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[i] <= PP) { i++; }
        topBar.transform.Find("PP").GetComponent<Text>().text = PP.ToString();
        topBar.transform.Find("PP").GetComponent<Text>().color = ShapeConstants.PPColors[i];

        int[] remaining = PlayerPrefsX.GetIntArray("AdsRem");
        remaining1 = remaining[0]; remaining2 = remaining[1];
        if (remaining1 <= 0) { remaining1 = 0; }
        AdsBut1.GetComponent<Button>().interactable = (remaining1 > 0);
        remaining1Text.text = remaining1.ToString() + " remaining";
        if (remaining2 <= 0) { remaining2 = 0; }
        AdsBut2.GetComponent<Button>().interactable = (remaining2 > 0);
        remaining2Text.text = remaining2.ToString() + " remaining";
        timeLeftBeforeReset = PlayerPrefs.GetInt("AdTimeBeforeReset") * 1000;   //*1000 to make it in ms
        Watch.Start();

        updateRoadStats();
    }

    private void lightenButton(GameObject go)
    {
        go.GetComponent<Image>().color = new Color(0.3128782f, 0.5212605f, 0.8396226f);
    }
    private void darkenButton(GameObject go)
    {
        go.GetComponent<Image>().color = new Color(0.2196079f, 0.3607843f, 0.5764706f);
    }

    public void goToRoad()
    {
        lightenButton(RoadButton);
        darkenButton(PlayButton);
        darkenButton(RewardButton);
    }
    public void goToPlay()
    {
        lightenButton(PlayButton);
        darkenButton(RoadButton);
        darkenButton(RewardButton);
    }
    public void goToRewards()
    {
        lightenButton(RewardButton);
        darkenButton(PlayButton);
        darkenButton(RoadButton);
    }

    //Chest system
    public void OpenIt(Chest ChestToOpen, int[] Contents)
    {
        Canvas.SetActive(false);
        CamChest.SetActive(true);
        Cam.SetActive(false);
        Chest.gameObject.SetActive(true);
        Chest.Initialize(ChestToOpen);
        i = 0;
        StartCoroutine(FuckIEnum(Contents));
    }

    private IEnumerator FuckIEnum(int[] Contents)
    {
        if (!Go) { yield return new WaitUntil(() => Go); }

        //Add The Contents To PlayerPrefs
        if (First)
        {
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + Contents[0]);
            PlayerPrefs.SetInt("Redbolts", PlayerPrefs.GetInt("Redbolts") + Contents[1]);
            PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + Contents[2]);
            string[] AbilitiesStr = PlayerPrefsX.GetStringArray("AbilitiesArray");
            for (int j = 3; j < 6; j++)
            {
                if (Contents[j] != 0 && Contents[j] != -1)
                {
                    AbilitiesStr[Contents[j]] = "1";
                    if (j == 3) { Chest.CommonAb.sprite = Chest.Sprites[Contents[j]]; }
                    else if (j == 4) { Chest.RareAb.sprite = Chest.Sprites[Contents[j]]; }
                    else if (j == 5) { Chest.MythicAb.sprite = Chest.Sprites[Contents[j]]; }
                }
            }
            PlayerPrefsX.SetStringArray("AbilitiesArray", AbilitiesStr);
            if (Contents[6] != 0 && Contents[6] != -1)
            {
                if (Contents[6] < 200)
                {
                    string[] Super100 = PlayerPrefsX.GetStringArray("Super100Array");
                    Super100[Contents[6] - 101] = "1";
                    PlayerPrefsX.SetStringArray("Super100Array", Super100);
                    Chest.MajesticAb.sprite = Chest.Super100s[Contents[6] - 101];
                }
                else
                {
                    string[] Super200 = PlayerPrefsX.GetStringArray("Super200Array");
                    Super200[Contents[6] - 201] = "1";
                    PlayerPrefsX.SetStringArray("Super200Array", Super200);
                    Chest.MajesticAb.sprite = Chest.Super200s[Contents[6] - 201];
                }
            }
            if (Contents[7] != -1)
            {
                int[] ShapeLevels = PlayerPrefsX.GetIntArray("Level");
                ShapeLevels[Contents[7]] = 1;
                PlayerPrefsX.SetIntArray("Level", ShapeLevels);
                Chest.Shapes.sprite = Chest.ShapesSpr[Contents[7]];
            }
            PlayerPrefs.Save();
            ClientTCP.PACKAGE_ChestOpening();
        }

        if ((i == 7 || Contents[i] != 0) && Contents[i] != -1 && i < 8)
        {
            Chest.Anim.SetInteger("ID", 0);
            if (First)
            {
                yield return new WaitForSeconds(1f);
                First = false;
            }
            Chest.Children[i].SetActive(true);
            if (i <= 2)
            {
                Chest.Children[i].transform.GetChild(0).GetComponent<Text>().text = Contents[i].ToString();
            }
            else if (i <= 5)
            {
                Chest.Children[i].transform.GetChild(0).GetComponent<Text>().text = ShapeConstants.NamesArray[Contents[i]];
                Chest.Children[i].transform.GetChild(0).GetComponent<Text>().color = ShapeConstants.rarityColors[i-3];
            }
            else if (i == 6)
            {
                if (Contents[i] < 200)
                {
                    Chest.Children[i].transform.GetChild(0).GetComponent<Text>().text = ShapeConstants.Super100Names[Contents[i] - 101];
                    Chest.Children[i].transform.GetChild(0).GetComponent<Text>().color = ShapeConstants.rarityColors[3];
                }
                else
                {
                    Chest.Children[i].transform.GetChild(0).GetComponent<Text>().text = ShapeConstants.Super200Names[Contents[i] - 201];
                    Chest.Children[i].transform.GetChild(0).GetComponent<Text>().color = ShapeConstants.rarityColors[3];
                }
            }
            else
            {
                Chest.Children[i].transform.GetChild(0).GetComponent<Text>().text = ShapeConstants.ShapeNames[Contents[i]];
            }
            Chest.Children[i].GetComponent<Animation>().Play();
            Invoke("FuckEnumerators", 2f);
            Go = false;
        }
        else
        {
            i++;
        }
        if (i != 8)
        {
            yield return StartCoroutine(FuckIEnum(Contents));
        }
        else if (i >= 8)
        {
            First = true;
            Go = true;
            Chest.Anim.SetInteger("ID", -1);
            Chest.gameObject.SetActive(false);
            CamChest.SetActive(false);
            Cam.SetActive(true);
            Canvas.SetActive(true);

            playResourcesAnim(0);
            playResourcesAnim(1);
            if (Contents[2] > 0) playResourcesAnim(2);

            foreach (ChestSlot Slot in Slots) { Slot.Start(); }
        }

    }
    private void FuckEnumerators()
    {
        Chest.Children[i].SetActive(false);
        i++;
        Go = true;
    }

    //Glory Road
    public void updateRoadStats()
    {
        maxPP = toClaim = 0;
        for (int i = 0; i < 4; i++) { maxPP = (maxShapePPs[i] > maxPP ? maxShapePPs[i] : maxPP); }

        mainRoad.transform.Find("Max PP").GetComponent<Text>().text = maxPP.ToString();

        for (int i = 0; i < checkpointContainer.Length; i++)
        {
            foreach (Transform go in checkpointContainer[i].transform)
            {
                if (go.GetComponent<Checkpoint>())
                    toClaim += go.GetComponent<Checkpoint>().updateState();
            }
        }

        SwitchShape.GoToShape(selectedShapeIndex);

        setRoadNotification();
    }

    public void setRoadNotification()
    {
        if (toClaim == 0)
        {
            notification.SetActive(false);
            return;
        }

        notification.SetActive(true);
        notification.transform.GetChild(0).GetComponent<Text>().text = toClaim.ToString();
    }

    public void changeRoadNotification(int change)
    {
        if (notification.transform.GetChild(0).GetComponent<Text>().text == "0")
            notification.SetActive(true);

        int temp;
        Int32.TryParse(notification.transform.GetChild(0).GetComponent<Text>().text, out temp);

        notification.transform.GetChild(0).GetComponent<Text>().text = (temp + change).ToString();

        if (notification.transform.GetChild(0).GetComponent<Text>().text == "0")
            notification.SetActive(false);
    }

    //Ads
    private void Update()
    {
        Watch.Stop();
        timeLeftBeforeReset -= Convert.ToInt32(Watch.Elapsed.TotalMilliseconds);
        if (timeLeftBeforeReset < 0) { timeLeftBeforeReset = 0; }
        Watch.Restart();

        TimeSpan Ts = TimeSpan.FromSeconds(timeLeftBeforeReset / 1000);
        string str = Ts.ToString(@"hh\:mm\:ss");
        timeLeftText.text = "Resets in " + str;

        if (timeLeftBeforeReset == 0)
        {
            remaining1 = 1; remaining2 = 10;
            remaining1Text.text = remaining1.ToString() + " remaining";
            remaining2Text.text = remaining2.ToString() + " remaining";
            timeLeftBeforeReset = 86400 * 1000;
            PlayerPrefs.SetInt("AdTimeBeforeReset", timeLeftBeforeReset);
            PlayerPrefsX.SetIntArray("AdsRem", new int[] { remaining1, remaining2 });
        }

        AdsBut1.GetComponent<Button>().interactable = remaining1 > 0;
        AdsBut2.GetComponent<Button>().interactable = remaining2 > 0;
    }

    public void SetUsedP1() { usedP1 = true; }
    public void watchedFirstAd()
    {
        PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + 10);
        playResourcesAnim(2);
        topBar.transform.Find("Diamonds").GetComponentInChildren<Text>().text = PlayerPrefs.GetInt("Diamonds").ToString();

        int[] remaining = PlayerPrefsX.GetIntArray("AdsRem");
        remaining1 = remaining[0];
        remaining1--;
        if (remaining1 < 0) { remaining1 = 0; }
        remaining1Text.text = remaining1.ToString() + " remaining";

        remaining[0] = remaining1;
        PlayerPrefsX.SetIntArray("AdsRem", remaining);

        usedP1 = false;

        PlayerPrefs.Save();
        try { ClientTCP.PACKAGE_AdReward(timeLeftBeforeReset / 1000); }
        catch(Exception) { showError(); }
    }

    public void watchedSecondAd()
    {
        PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 50);
        PlayerPrefs.SetInt("Redbolts", PlayerPrefs.GetInt("Redbolts") + 5);
        playResourcesAnim(0); playResourcesAnim(1);
        topBar.transform.Find("Coins").GetComponentInChildren<Text>().text = PlayerPrefs.GetInt("Gold").ToString();
        topBar.transform.Find("Red Bolts").GetComponentInChildren<Text>().text = PlayerPrefs.GetInt("Redbolts").ToString();

        int[] remaining = PlayerPrefsX.GetIntArray("AdsRem");
        remaining2 = remaining[1];
        remaining2--;
        if (remaining2 < 0) { remaining2 = 0; }
        remaining2Text.text = remaining2.ToString() + " remaining";

        remaining[1] = remaining2;
        PlayerPrefsX.SetIntArray("AdsRem", remaining);

        PlayerPrefs.Save();
        try { ClientTCP.PACKAGE_AdReward(timeLeftBeforeReset / 1000); }
        catch (Exception) { showError(); }
    }

    //0 for Gold, 1 for RB, 2 for Diamonds
    public void playResourcesAnim(int ID)
    {
        if (ResourcesAnim == null)
            ResourcesAnim = GameObject.Find("Resources Anim");
        ResourcesAnim.transform.GetChild(ID).GetComponent<Animator>().SetTrigger("Trig");
    }

    public void showError()
    {
        ErrorPanel.SetActive(true);
    }

    public void SkipAnimation()
    {
        if (ResetList.Contains(i))
            return;

        Chest.Children[i].GetComponent<Animation>()["ChestComponents"].speed = 5f;
        ResetList.Add(i);
        Invoke("ResetAnim", 1.4f);
    }
    private void ResetAnim()
    {
        if (ResetList.Count == 0)
            return;

        foreach(int j in ResetList)
            Chest.Children[j].GetComponent<Animation>()["ChestComponents"].speed = 1f;
        ResetList.Clear();
    }

    public void ShowMessage(string msg)
    {
        Message.SetActive(true);
        Message.transform.Find("Text").GetComponent<Text>().text = msg;
    }

    public void Tutorial()
    {
        PlayerPrefs.SetInt("Tutorial", 1);
        PlayerPrefs.SetInt("bot", 0);
        SceneManager.LoadScene("PassThePhone");
    }
}