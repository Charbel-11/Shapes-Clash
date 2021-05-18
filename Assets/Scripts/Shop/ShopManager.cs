using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ShopManager : MonoBehaviour
{
    public GameObject NotEnough;
    public GameObject ErrorOccured;

    // Bot Bar
    public Sprite[] ShapeXP;

    public static int coins;
    public static int redBolts;
    public static int diamonds;
    private GameObject botBar;

    // SHAPES
    public GameObject Shapes;
    public GameObject ShapeContainer;
    public static int[] shapePrices;    // Only coins
    public static int[] shapePPs;
    public static int[] shapeLvls;      // Level 0 means locked/not bought yet (depending on other shapes' level)
    public static int[] shapeExp;

    public static int selectedShapeIndex;

    private string levelStatsStr;
    public static int[][][] levelStats;

    // ABILITIES
    public Button goOfSelectedAbBut;
    public GameObject Describer, passiveDescription, CommonInfo;
    public GameObject preview;
    public GameObject[] abilities;  //common/cube/pyr/star/sphere           Show the selected one only;
    public GameObject[] specialAb;  //cube/pyr/star/sphere
    public GameObject[] passives;   //cube/pyr/star/sphere
    public bool canChange;
    public GameObject levelUpPanel;
    
    public static string[] AbLevelArray;
    public static string[] Super100;
    public static string[] Super200;
    public static int[] PassivesLevel;

    private string[] StatsArrStr;
    private string[] Super100StatsArrStr;
    private string[] Super200StatsArrStr;
    private string[] PassiveStatsArrStr;
    public static int[][] StatsArr;
    public static int[][] Super100StatsArr;
    public static int[][] Super200StatsArr;
    public static int[][] PassiveStatsArr;
    public static int[][][] ShopArray;
    public static int[][][] Super100Shop;
    public static int[][][] Super200Shop;
    public static int[][][] PassivesShop;
    public static int[] rarity;

    public int panelOpen;              //1 for common, 2 for specific, 3 for special, 4 for passives

    // EMOTES
    public GameObject goOfSelectedEmote;
    public BuyEmote buyEmote;
    public GameObject previews;
    public GameObject previewText;
    public GameObject commonInfoEmote;
    public GameObject emoteName;
    public GameObject commonEmoteContainer;

    private GameObject playerE;

    public static int[] EmotesUnlock;
    public static int[] EmotesPrice;

    //SKINS
    public GameObject SkinContainer;
    public static int[] SkinPrices;
    public static int[] unlockedSkins;
    public static int selectedSkin;

    // BACKGROUNDS
    public GameObject BckgdContainer;
    public static int[] BckgdPrices;    //If other than coins, modify it somehow
    public static int[] unlockedBckgd;
    public static int selectedBckgd;

    //RESOURCES
    public static int[] CoinPrices;
    public static int[] CoinRewards;
    public GameObject[] CoinPanels;
    public Purchaser IAPManager;
    public Text[] DiamondsPricesText;

    private void Awake()
    {
        canChange = false;

        // SHAPES
        shapePPs = PlayerPrefsX.GetIntArray("PP");
        shapeLvls = PlayerPrefsX.GetIntArray("Level");
        shapeExp = PlayerPrefsX.GetIntArray("XP");
        shapePrices = PlayerPrefsX.GetIntArray("ShapePrice");       
       
        selectedShapeIndex = PlayerPrefs.GetInt("ShapeSelectedID");    //0 for Cube, 1 for pyramid, ...

        levelStatsStr = PlayerPrefs.GetString("LevelStats");
        levelStats = ClientHandleData.TransformToArrayShop(levelStatsStr);


        // Bottom Bar
        botBar = GameObject.Find("Bot Bar");
        coins = PlayerPrefs.GetInt("Gold");
        redBolts = PlayerPrefs.GetInt("Redbolts");
        diamonds = PlayerPrefs.GetInt("Diamonds");


        // ABILITIES
        AbLevelArray = PlayerPrefsX.GetStringArray("AbilitiesArray");           //Contains ability levels of the current player
        Super100 = PlayerPrefsX.GetStringArray("Super100Array");
        Super200 = PlayerPrefsX.GetStringArray("Super200Array");
        PassivesLevel = PlayerPrefsX.GetIntArray("PassivesArray");

        StatsArrStr = PlayerPrefsX.GetStringArray("StatsArray");
        Super100StatsArrStr = PlayerPrefsX.GetStringArray("Super100StatsArray");
        Super200StatsArrStr = PlayerPrefsX.GetStringArray("Super200StatsArray");
        PassiveStatsArrStr = PlayerPrefsX.GetStringArray("PassiveStatsArray");
        StatsArr = ClientHandleData.TransformStringArray(StatsArrStr);          //Contains Game Stats
        Super100StatsArr = ClientHandleData.TransformStringArray(Super100StatsArrStr);
        Super200StatsArr = ClientHandleData.TransformStringArray(Super200StatsArrStr);
        PassiveStatsArr = ClientHandleData.TransformStringArray(PassiveStatsArrStr);
        ShopArray = ClientHandleData.TransformToArrayShop(PlayerPrefs.GetString("Shop"));   //contains prices
        Super100Shop = ClientHandleData.TransformToArrayShop(PlayerPrefs.GetString("Super100Shop"));
        Super200Shop = ClientHandleData.TransformToArrayShop(PlayerPrefs.GetString("Super200Shop"));
        PassivesShop = ClientHandleData.TransformToArrayShop(PlayerPrefs.GetString("PassivesShop"));
        rarity = PlayerPrefsX.GetIntArray("AbilitiesRarety");

        levelUpPanel.gameObject.SetActive(true);
//        foreach(Transform go in levelUpPanel.transform) { go.gameObject.SetActive(false); }

        // Emotes
        EmotesUnlock = PlayerPrefsX.GetIntArray("EmotesUnlockedAr");
        EmotesPrice = PlayerPrefsX.GetIntArray("EmotesPriceAr");
        initialEmoteState();

        resetSelected(true);

        //Skins
        SkinPrices = PlayerPrefsX.GetIntArray("SkinsPriceAr"); 
        unlockedSkins = PlayerPrefsX.GetIntArray("SkinsUnlockedAr");
        selectedSkin = PlayerPrefs.GetInt("SkinID");

        // Backgrounds
        BckgdPrices = PlayerPrefsX.GetIntArray("BgPriceAr");
        unlockedBckgd = PlayerPrefsX.GetIntArray("Backgrounds");
        selectedBckgd = PlayerPrefs.GetInt("BckgdID");

        //Resources
        CoinPrices = PlayerPrefsX.GetIntArray("CoinPriceAr");
        CoinRewards = PlayerPrefsX.GetIntArray("CoinRewardAr");

        canChange = true;
        //updateAll() is called by the button in the main menu
    }

    public void updateAll()
    {
        Awake();
        updateBotBar();
        updateShapes();
        updateShapeButtons();
        updateLockedButtons();
        updateLevels();
        updateSkinPic();
        updateSkinButtons();
        updateBckgdButtons();
        updateResourcesButtons();
    }

    public void updateBotBar()
    {
        coins = PlayerPrefs.GetInt("Gold");
        redBolts = PlayerPrefs.GetInt("Redbolts");
        diamonds = PlayerPrefs.GetInt("Diamonds");
        shapeLvls = PlayerPrefsX.GetIntArray("Level");
        shapeExp = PlayerPrefsX.GetIntArray("XP");

        Transform XP = botBar.transform.Find("XP");
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

        botBar.transform.Find("Coins").GetComponentInChildren<Text>().text = coins.ToString();
        botBar.transform.Find("Red Bolts").GetComponentInChildren<Text>().text = redBolts.ToString();
        botBar.transform.Find("Diamonds").GetComponentInChildren<Text>().text = diamonds.ToString();
    }

    //SHAPES
    public void updateShapes()
    {
        for (int i = 0; i < 4; i++)
        {
            Transform Cur = ShapeContainer.transform.Find(ShapeConstants.ShapeNames[i]);

            if (shapeLvls[i] < 1)
            {
                Cur.Find("XP").Find("Bar").GetComponent<Slider>().maxValue = 100000000;
                Cur.Find("XP").Find("Bar").GetComponent<Slider>().value = shapeExp[i];  //might be >0 due to bought common abs
                Cur.Find("XP").Find("Bar").transform.Find("Text").GetComponent<Text>().text = shapeExp[i].ToString();
                Cur.Find("XP").Find("Shape").Find("Level").GetComponent<Text>().text = "0";

                Cur.Find("PP").GetComponent<Text>().text = "0";
                Cur.Find("PP").GetComponent<Text>().color = ShapeConstants.PPColors[0];

                Cur.Find("Attack").GetComponent<Text>().text = "+0";
                Cur.Find("Defense").GetComponent<Text>().text = "+0";
                Cur.Find("Health").GetComponent<Text>().text = "100";
                continue;
            }

            Cur.Find("XP").Find("Bar").GetComponent<Slider>().maxValue = (shapeLvls[i] == 8 ? shapeExp[i] : levelStats[shapeLvls[i] - 1][4][0]);
            Cur.Find("XP").Find("Bar").GetComponent<Slider>().value = shapeExp[i];
            Cur.Find("XP").Find("Bar").transform.Find("Text").GetComponent<Text>().text = shapeExp[i].ToString() + (shapeLvls[i] == 8 ? "" : "/" + levelStats[shapeLvls[i] - 1][4][0].ToString());
            Cur.Find("XP").Find("Shape").Find("Level").GetComponent<Text>().text = shapeLvls[i].ToString();

            int PP = shapePPs[i], j = 0;
            while (j < ShapeConstants.PPRange.Length && ShapeConstants.PPRange[j] <= PP) { j++; }
            Cur.Find("PP").GetComponent<Text>().text = PP.ToString();
            Cur.Find("PP").GetComponent<Text>().color = ShapeConstants.PPColors[j];

            Cur.Find("Attack").GetComponent<Text>().text = "+" + levelStats[shapeLvls[i] - 1][i][0].ToString();
            Cur.Find("Defense").GetComponent<Text>().text = "+" + levelStats[shapeLvls[i] - 1][i][1].ToString();
            Cur.Find("Health").GetComponent<Text>().text = (100 + levelStats[shapeLvls[i] - 1][i][2]).ToString();
        }
    }
    public void updateShapeButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            ShapesButton cur = ShapeContainer.transform.Find(ShapeConstants.ShapeNames[i]).transform.Find("Choose").GetComponent<ShapesButton>();
            cur.updateAll();
        }
    }

    //ABILITIES
    void updateLockedButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject[] cur;
            if (i == 0) { cur = abilities; }
            else if (i == 1) { cur = specialAb; }
            else { cur = passives; }

            foreach (GameObject go in cur)
            {
                foreach (Transform a in go.transform.GetChild(0).transform.GetChild(0).transform)
                {
                    a.GetComponent<Ability>().updateButton();
                    a.GetComponent<Ability>().updatePanels();
                }
            }
            foreach (GameObject go in cur)
            {
                foreach (Transform a in go.transform.GetChild(0).transform.GetChild(0).transform)
                    a.GetComponent<Ability>().setOrder();
            }
        }
    }
    void updateLevels()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject[] cur;
            if (i == 0) { cur = abilities; }
            else if (i == 1) { cur = specialAb; }
            else { cur = passives; }

            foreach (GameObject go in cur)
            {
                foreach (Transform a in go.transform.GetChild(0).transform.GetChild(0).transform)
                {
                    a.Find("Levels").GetComponent<abilityLevel>().updateColor();
                }
            }
        }
    }

    public void openInfo(int i, bool direct3) 
    {
        if (panelOpen == 1)
            CommonInfo.gameObject.SetActive(true);
        else
            CommonInfo.gameObject.SetActive(false);

        Describer.SetActive(true);
        Describer.GetComponent<ShowInfo>().updateInfo(i);
        Describer.GetComponentInChildren<BuyAbility>().updateCost(direct3);
    }

    public void openPassive(int ID)
    {
        CommonInfo.gameObject.SetActive(false);
        passiveDescription.SetActive(true);

        for(int i = 0; i < passiveDescription.transform.childCount; i++)
        {
            passiveDescription.transform.GetChild(i).gameObject.SetActive(i == ID);
            if (i == ID)
            {
                passiveDescription.transform.GetChild(i).GetComponent<DescribePassive>().updateInfo();
                passiveDescription.transform.GetChild(i).GetComponentInChildren<BuyAbility>().updateCost(false);
            }
        }
    }

    public void resetSelected(bool changeOfPanel)       //if changeOfPanel is true, put preview back
    { 
        if (changeOfPanel == true) { preview.gameObject.SetActive(true); }
        else { preview.gameObject.SetActive(false); }

        if (goOfSelectedAbBut == null) { return; }

        goOfSelectedAbBut.gameObject.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        goOfSelectedAbBut.gameObject.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
        goOfSelectedAbBut.gameObject.transform.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
        goOfSelectedAbBut.interactable = true;
        Button[] buts = goOfSelectedAbBut.transform.parent.gameObject.GetComponentsInChildren<Button>();
        foreach (Button b in buts)
        {
            b.interactable = true;
        }

        if (goOfSelectedAbBut.name == "Button8")
            goOfSelectedAbBut.transform.parent.gameObject.GetComponentInChildren<abilityLevel>().stopInShopHighlight();
        else    //passive
            goOfSelectedAbBut.transform.parent.parent.gameObject.GetComponentInChildren<abilityLevel>().stopInShopHighlight();

        goOfSelectedAbBut = null;
    }

    //Has to be the last selected one
    public void levelUp(int newL, bool passive, bool super = false)
    {
        if (passive)
        {
            goOfSelectedAbBut.transform.parent.parent.gameObject.GetComponent<Ability>().updatePanels();

            goOfSelectedAbBut.transform.parent.parent.gameObject.GetComponentInChildren<abilityLevel>().updateLevel(newL);
            goOfSelectedAbBut.transform.parent.parent.gameObject.GetComponentInChildren<abilityLevel>().inShopHighlight();

        }
        else
        {
            goOfSelectedAbBut.transform.parent.gameObject.GetComponent<Ability>().updatePanels();

            goOfSelectedAbBut.transform.parent.gameObject.GetComponentInChildren<abilityLevel>().updateLevel(newL);
            goOfSelectedAbBut.transform.parent.gameObject.GetComponentInChildren<abilityLevel>().inShopHighlight();
        }

        if (!super && !passive) { Describer.GetComponent<ShowInfo>().updateInfo(goOfSelectedAbBut.transform.parent.GetComponent<Ability>().ID); }
        else if (passive)
        {
            int myID = goOfSelectedAbBut.transform.parent.parent.gameObject.GetComponent<Ability>().ID;
            foreach (Transform go in passiveDescription.transform)
            {
                if (go.GetComponent<DescribePassive>().ID == myID) { go.GetComponent<DescribePassive>().updateInfo(); break; }
            }
        }
        else { Describer.GetComponent<ShowInfo>().updateInfo(goOfSelectedAbBut.transform.parent.GetComponent<SpecialAbility>().ID); }

        Button temp = goOfSelectedAbBut;
        updateAll();
        goOfSelectedAbBut = temp;
        preview.SetActive(false);
        goOfSelectedAbBut.GetComponentInParent<Ability>().mimicClick();
    }

    public void openSpecific()
    {
        for(int i = 1; i < abilities.Length; i++)
        {
            abilities[i].gameObject.SetActive((selectedShapeIndex + 1) == i);
        }
    }

    public void openSpecial()
    {
        for (int i = 0; i < specialAb.Length; i++)
        {
            specialAb[i].gameObject.SetActive(i == selectedShapeIndex);
        }
    }

    public void openPassives()
    {
        for (int i = 0; i < passives.Length; i++)
        {
            passives[i].gameObject.SetActive(i == selectedShapeIndex);
        }
    }

    public void showWarning()
    {
        NotEnough.gameObject.SetActive(true);
    }

    public void showError()
    {
        ErrorOccured.gameObject.SetActive(true);
    }

    public bool shapeLevelUp()
    {
        bool changed = false;
        coins = PlayerPrefs.GetInt("Gold");
        diamonds = PlayerPrefs.GetInt("Diamonds");
        shapeLvls = PlayerPrefsX.GetIntArray("Level");      //In case we bought a new shape
        shapeExp = PlayerPrefsX.GetIntArray("XP");

        int iCoin = coins, iDiamonds = diamonds;
        int[] iShapeLvls = shapeLvls, iXP = shapeExp;
        Stack<int> s = new Stack<int>();

        for (int i = 0; i < 4; i++)
        {
            if (shapeLvls[i] == 0 || shapeLvls[i] == ShapeConstants.maxLevel) { continue; }        //Accumulate XP without leveling up

            int needed = levelStats[shapeLvls[i] - 1][4][0];

            while (shapeExp[i] >= needed && shapeLvls[i] != ShapeConstants.maxLevel)
            {
                changed = true;
                shapeExp[i] -= needed;
                shapeLvls[i]++;

                coins += levelStats[shapeLvls[i] - 1][4][1];
                diamonds += levelStats[shapeLvls[i] - 1][4][2];

                s.Push(i);

                needed = levelStats[shapeLvls[i] - 1][4][0];
            }
        }
        if (changed)
        {
            PlayerPrefsX.SetIntArray("Level", shapeLvls);
            PlayerPrefsX.SetIntArray("XP", shapeExp);
            PlayerPrefs.SetInt("Gold", coins);
            PlayerPrefs.SetInt("Diamonds", diamonds);
            PlayerPrefs.Save();

            updateAll();

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
            catch(Exception)
            {
                for(int i = 0; i < 4; i++)
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

                updateAll();

                throw new Exception();
            }
        }

        return changed;
    }


    //EMOTES
    public void ShowAnimation(int ID)
    {
        canChange = false;
        selectedShapeIndex = PlayerPrefs.GetInt("ShapeSelectedID");
         
        for (int i = 0; i < 4; i++)
        {
            previews.transform.GetChild(i).gameObject.SetActive(i == selectedShapeIndex);
            if (i == selectedShapeIndex)
            {
                playerE = previews.transform.GetChild(i).gameObject;
            }
        }

        playerE.transform.Find("Design").transform.position = new Vector3(0, 0, 0);
        playerE.transform.Find("Design").transform.rotation = new Quaternion(0, 0, 0, 0);
        playerE.GetComponent<Animator>().SetInteger("EID", -1);
        StartCoroutine(wait(ID));
    }

    IEnumerator wait(int ID)
    {
        yield return new WaitForSeconds(.1f);
        playerE.GetComponent<Animator>().SetInteger("EID", ID);
        yield return new WaitForSeconds(1f);
        canChange = true;
    }

    // TODO: could change parameters, i.e. get diamonds from a saved array
    public void updateEmoteButton(int cID, int cdiamonds)
    {
        buyEmote.ID = cID; buyEmote.diamonds = cdiamonds;
        buyEmote.updateText();
    }
    public void initialEmoteState()
    {
        if (goOfSelectedEmote != null) { goOfSelectedEmote.GetComponent<EmoteBut>().RemoveHighlight(); }
        goOfSelectedEmote = null;
        previewText.SetActive(true);
        commonInfoEmote.gameObject.SetActive(false);
        buyEmote.gameObject.SetActive(false);
        emoteName.gameObject.SetActive(false);
        previews.gameObject.SetActive(false);

        foreach (Transform go in commonEmoteContainer.transform) {
            go.GetComponentInChildren<EmoteBut>().updateState();
        }
    }


    //SKINS
    void updateSkinPic()
    {
        for (int i = 0; i < SkinContainer.transform.childCount; i++)
        {
            Transform cur = SkinContainer.transform.GetChild(i).transform.Find("Pics");
            for(int j = 0; j < 4; j++)
                cur.GetChild(j).gameObject.SetActive(j == selectedShapeIndex);
        }
    }
    void updateSkinButtons()
    {
        for (int i = 0; i < SkinContainer.transform.childCount; i++)
        {
            BuySkin cur = SkinContainer.transform.GetChild(i).transform.Find("Choose").GetComponent<BuySkin>();
            cur.updateAll();
        }
    }


    //BACKGROUND
    void updateBckgdButtons()
    {
        for (int i = 0; i < BckgdContainer.transform.childCount; i++)
        {
            BuyBackground cur = BckgdContainer.transform.GetChild(i).transform.Find("Choose").GetComponent<BuyBackground>();
            cur.updateAll();
        }
    }

    //RESOURCES
    void updateResourcesButtons()
    {
        for(int i = 0; i < CoinPrices.Length; i++)
        {
            CoinPanels[i].transform.Find("Name").GetComponent<Text>().text = CoinRewards[i].ToString() + " Coins";
            CoinPanels[i].transform.Find("Choose").Find("Price").GetComponent<Text>().text = CoinPrices[i].ToString();
            CoinPanels[i].GetComponentInChildren<BuyCoins>().reward = CoinRewards[i];
            CoinPanels[i].GetComponentInChildren<BuyCoins>().price = CoinPrices[i];
        }
    }
    public void setPrices()
    {
        DiamondsPricesText[0].text = IAPManager.GetProducePriceFromStore("d10");
        DiamondsPricesText[1].text = IAPManager.GetProducePriceFromStore("d25");
        DiamondsPricesText[2].text = IAPManager.GetProducePriceFromStore("d50");
    }
    public void addDiamonds(int addedD)
    {
        diamonds = PlayerPrefs.GetInt("Diamonds");
        diamonds += addedD;
        PlayerPrefs.SetInt("Diamonds", diamonds);
        updateBotBar();

        try { ClientTCP.PACKAGE_AdReward(-1); }
        catch (Exception) { showError(); }
    }
}