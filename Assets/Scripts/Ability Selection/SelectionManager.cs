using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System; 

public class SelectionManager : MonoBehaviour
{
    public bool P1;
    public static bool online;

    public ShapePPs selectedShapePP;
    public ShapeLvls selectedShapeLvl;

    public GameObject shapeContainer;

    public SelectedAbility[] arrayOfSelectedAbilities = new SelectedAbility[6];
    public GameObject Describer;

    public GameObject[] shapesAbilitiesPanels;
    public GameObject[] containersOfAllAbilities;       //All the parents of the abilities: common/cube/pyramid/...

    public GameObject selectedPassives;
    public GameObject PassivesInfo;

    public int IDOfSelectedAbility;
    public GameObject goOfSelectedAb;

    public int finalShapeIndex;
    public GameObject goOfPreviouslyChosenShape;        //Set it by default to ChooseSquare (or any other shape) to avoid bugs
    public GameObject[] ChooseButtons;

    public GameObject[] containersOfAllSpecialAbilities;     //Have to be in order 
    public int IDOfSpecialAbility;
    public GameObject goOfSpecialAbility;
    public string specialAbilityName;
    public GameObject[] selectedSpecial;

    public int[] finalIDs = new int[6];
    public string[] finalTexts = new string[6];

    public static string[] AbLevelArray;
    public static string[] Super100;
    public static string[] Super200;
    public static int[] PassivesArray;
    public static int[] rarity;

    public static int[] shapePPs;
    public static int[] shapeLvls;
    public static int[] shapeExp;

    private string levelStatsStr;
    public static int[][][] levelStats;

    public static int[] unlockedSkins;
    public int skin;
    public GameObject[] skinButtons;            //in order

    public static int[] unlockedBackgrounds;
    public int background;
    public GameObject[] bckgdButtons;           //Need to be in order of backgrds

    public string[] StatsArrStr;
    public string[] Super100StatsArrStr;
    public string[] Super200StatsArrStr;
    private string[] PassiveStatsArrStr;
    public static int[][] StatsArr;
    public static int[][] Super100StatsArr;
    public static int[][] Super200StatsArr;
    public static int[][] PassiveStatsArr;

    public GameObject EmotesContainer;
    public int[] EmotesID;
    public static int[] unlockedEmotes;
    public Text EmotesText;

    public GameObject Battle;
    public GameObject FriendlyBattle;
    public GameObject AcceptBattle;
    public GameObject Cancel;

    public GameObject MessagePanel;
    public GameObject ErrorPanel;

    private bool initialized = false;

    public bool OfflineCanvas;

    private void Awake()
    {
        Input.multiTouchEnabled = false;

        StatsArrStr = PlayerPrefsX.GetStringArray("StatsArray");
        Super100StatsArrStr = PlayerPrefsX.GetStringArray("Super100StatsArray");
        Super200StatsArrStr = PlayerPrefsX.GetStringArray("Super200StatsArray");
        PassiveStatsArrStr = PlayerPrefsX.GetStringArray("PassiveStatsArray");

        StatsArr = ClientHandleData.TransformStringArray(StatsArrStr);
        Super100StatsArr = ClientHandleData.TransformStringArray(Super100StatsArrStr);
        Super200StatsArr = ClientHandleData.TransformStringArray(Super200StatsArrStr);
        PassiveStatsArr = ClientHandleData.TransformStringArray(PassiveStatsArrStr);

        initialize();
    }

    public void initialize()
    {
        AbLevelArray = PlayerPrefsX.GetStringArray("AbilitiesArray");
        Super100 = PlayerPrefsX.GetStringArray("Super100Array");
        Super200 = PlayerPrefsX.GetStringArray("Super200Array");
        PassivesArray = PlayerPrefsX.GetIntArray("PassivesArray");
        rarity = PlayerPrefsX.GetIntArray("AbilitiesRarety");

        shapePPs = PlayerPrefsX.GetIntArray("PP");
        shapeLvls = PlayerPrefsX.GetIntArray("Level");
        shapeExp = PlayerPrefsX.GetIntArray("XP");

        levelStatsStr = PlayerPrefs.GetString("LevelStats");
        levelStats = ClientHandleData.TransformToArrayShop(levelStatsStr);

        unlockedBackgrounds = PlayerPrefsX.GetIntArray("Backgrounds");
        background = PlayerPrefs.GetInt("BckgdID");

        unlockedSkins = PlayerPrefsX.GetIntArray("SkinsUnlockedAr");
        skin = PlayerPrefs.GetInt("SkinID");

        //adjust the size of default emoteID when having more than 4 emotes
        if (P1)
        {
            finalShapeIndex = PlayerPrefs.GetInt("ShapeSelectedID");    //0 for Cube, 1 for pyramid, ...
            EmotesID = PlayerPrefsX.GetIntArray("EmotesID");
            if (EmotesID.Length == 0)
            {
                EmotesID = new int[4] { -1, -1, -1, -1 };
                PlayerPrefsX.SetIntArray("EmotesID", EmotesID);               
            }
        }
        else
        {
            finalShapeIndex = PlayerPrefs.GetInt("2ShapeSelectedID");
            EmotesID = PlayerPrefsX.GetIntArray("EmotesID2");
            if (EmotesID.Length == 0)
            {
                EmotesID = new int[4] { -1, -1, -1, -1 };
                PlayerPrefsX.SetIntArray("EmotesID2", EmotesID);
            }
        }
        unlockedEmotes = PlayerPrefsX.GetIntArray("EmotesUnlockedAr");

        IDOfSelectedAbility = -1;

        initialized = true;

        //Happens if we are searching for an opponent, someone asks for a friendly battle and we say yes
        if (Cancel.transform.parent.gameObject.activeSelf) { Cancel.GetComponent<CancelSearch>().Cancel(false); }

        UpdatePoolOfAbilities();
        UpdatePoolOfSpecialAbilities();
        updatePassives();
        ResetSelectedShapeButton();
        resetSkinButton();
        resetSkinPic();
        ResetBckgdButtons();
        updateShapeStats();
        updateEmotes();

        foreach(Transform s in shapeContainer.transform)
        {
            s.GetComponentInChildren<ShapeLvls>().updateLevels();
            s.GetComponentInChildren<ShapePPs>().updateShapePPs();
        }

        //We just update the abilityToSelect, since the selectedAbilities take care of themselves
        ShouldHaveBeenOnEnable();   //Deals with case where it was already enabled
    }

    private void ShouldHaveBeenOnEnable()   
    {
        if (!online && !OfflineCanvas) { return; }

        if (TempOpponent.Opponent.FriendlyBattle && TempOpponent.Opponent.Accepting)
        {
            Battle.SetActive(false);
            FriendlyBattle.SetActive(false);
            AcceptBattle.SetActive(true);
        }
        else if (TempOpponent.Opponent.FriendlyBattle)
        {
            Battle.SetActive(false);
            FriendlyBattle.SetActive(true);
            AcceptBattle.SetActive(false);
        }
        else
        {
            Battle.SetActive(true);
            FriendlyBattle.SetActive(false);
            AcceptBattle.SetActive(false);
        }
    }

    public void HighlightUnselectedButtoons()
    {
        foreach (SelectedAbility curAbility in arrayOfSelectedAbilities)
        {
            curAbility.SetCanNowBeSelected(true);

            // Here we are highlighting it, we stop that animation somewhere else
            if (curAbility.GetSelected() == false)
            {
                curAbility.gameObject.GetComponent<Animator>().SetBool("Highlight", true);
            }
        }
    }

    public void StopTheHighlight()
    {
            foreach (SelectedAbility curAbility in arrayOfSelectedAbilities)
        {
            curAbility.gameObject.GetComponent<Animator>().SetBool("Highlight", false);
            curAbility.gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
            curAbility.SetCanNowBeSelected(false);
        }
    }

    public void OpenExplanatoryPanel(int i)
    {
        if (i < 0) { return; }
        Describer.gameObject.SetActive(true);
        Describer.GetComponent<DescribeMe>().showMe(i);
    }

    public void OpenPassivePanel(int ID)
    {
        PassivesInfo.gameObject.SetActive(true);
        for(int i = 0; i < PassivesInfo.transform.childCount; i++)
        {
            PassivesInfo.transform.GetChild(i).gameObject.SetActive(i == ID);
            if (i == ID)
                PassivesInfo.transform.GetChild(i).GetComponent<ShowPassive>().updateInfo();
        }
    }

    public void ReActivateRemovedAbility(int ID)
    {
        foreach (GameObject container in containersOfAllAbilities)
        {
            foreach (Transform ability in container.transform)
            {
                if (ability.gameObject.GetComponentInChildren<AbilityToSelect>().ID == ID)      //modified with levels
                {
                    ability.gameObject.GetComponentInChildren<AbilityToSelect>().SetAlreadySelected(false);
                    break;
                }
            }
        }
    }

    public void ResetAllAbilities()
    {
        if (goOfSelectedAb != null)
        {
            goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
            goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
            goOfSelectedAb.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            goOfSelectedAb.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        }
        StopTheHighlight();

        foreach (GameObject container in containersOfAllAbilities)
        {
            foreach (Transform ability in container.transform)
            {
                if (finalIDs.Contains(ability.gameObject.GetComponentInChildren<AbilityToSelect>().ID))
                    ability.gameObject.GetComponentInChildren<AbilityToSelect>().SetAlreadySelected(true);  //modified with levels
                else
                    ability.gameObject.GetComponentInChildren<AbilityToSelect>().SetAlreadySelected(false);
            }
        }
        IDOfSelectedAbility = -1;
    }

    public void ResetSelectedShapeButton()
    {
        for(int i = 0; i < 4; i++)
        {
            Transform go = ChooseButtons[i].transform;

            if (i == finalShapeIndex)
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = ShapeConstants.selectedColor;
                go.transform.GetChild(0).GetComponent<Text>().text = "Chosen";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
            else if (shapeLvls[go.GetComponent<ChooseShape>().ID] > 0)
            {
                go.GetComponent<Button>().interactable = true;
                go.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Choose";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(.196f, .196f, .196f);
            }
            else
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Locked";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
        }
    }

    public void ResetBckgdButtons()
    {
        for(int i = 0; i < bckgdButtons.Length; i++)
        {
            Transform go = bckgdButtons[i].transform;

            if (i == background)
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = ShapeConstants.selectedColor;
                go.transform.GetChild(0).GetComponent<Text>().text = "Selected";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
            else if (unlockedBackgrounds[go.GetComponent<ChooseBackground>().ID] == 1)
            {
                go.GetComponent<Button>().interactable = true;
                go.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Select";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(.196f, .196f, .196f);
            }
            else
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Locked";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
        }
    }

    public void resetSkinButton()
    {
        for (int i = 0; i < skinButtons.Length; i++)
        {
            Transform go = skinButtons[i].transform;

            if (i == skin)
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = ShapeConstants.selectedColor;
                go.transform.GetChild(0).GetComponent<Text>().text = "Selected";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
            else if (unlockedSkins[go.GetComponent<ChooseSkin>().ID] == 1)
            {
                go.GetComponent<Button>().interactable = true;
                go.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Select";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(.196f, .196f, .196f);
            }
            else
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
                go.transform.GetChild(0).GetComponent<Text>().text = "Locked";
                go.transform.GetChild(0).GetComponent<Text>().color = new Color(255f, 255f, 255f);
            }
        }
    }

    public void resetSkinPic()
    {
        for (int i = 0; i < bckgdButtons.Length; i++)
        {
            Transform go = skinButtons[i].transform.parent.Find("Pics");
            for(int j = 0; j < 4; j++)
            {
                go.GetChild(j).gameObject.SetActive(j == finalShapeIndex);
            }
        }
    }

    public void UpdatePoolOfAbilities()
    {
        if (!initialized) { return; }

        foreach (GameObject go in containersOfAllAbilities)
        {
            foreach (Transform child in go.transform)
            {
                child.GetComponentInChildren<AbilityToSelect>().setButton();
                child.GetComponentInChildren<abilityLevel>().updateColor();
            }
            //After initializing them, arange their order
            foreach (Transform child in go.transform)
            {
                child.GetComponentInChildren<AbilityToSelect>().setOrder();
            }
        }

        if (P1)
        {
            finalIDs = PlayerPrefsX.GetIntArray("ActiveAbilityID" + finalShapeIndex.ToString());
            finalTexts = PlayerPrefsX.GetStringArray("ActiveAbilityText" + finalShapeIndex.ToString());
        }
        else
        {
            finalIDs = PlayerPrefsX.GetIntArray("2ActiveAbilityID" + finalShapeIndex.ToString());
            finalTexts = PlayerPrefsX.GetStringArray("2ActiveAbilityText" + finalShapeIndex.ToString());
        }

        if (finalIDs.Length == 0)
            finalIDs = new int[6] { -1, -1, -1, -1, -1, -1 };
        if (finalTexts.Length == 0)
            finalTexts = new string[6] { "", "", "", "", "", "" };

        foreach (SelectedAbility selecAb in arrayOfSelectedAbilities)
            selecAb.UpdateContent();

        for (int i = 0; i < shapesAbilitiesPanels.Length; i++)
        {
            if (i == finalShapeIndex)
                shapesAbilitiesPanels[i].SetActive(true);
            else
                shapesAbilitiesPanels[i].SetActive(false);
        }

        ResetAllAbilities();
    }

    public void UpdatePoolOfSpecialAbilities()
    {
        if (P1)
            IDOfSpecialAbility = PlayerPrefs.GetInt("SpecialAbilityID" + finalShapeIndex.ToString());
        else
            IDOfSpecialAbility = PlayerPrefs.GetInt("2SpecialAbilityID" + finalShapeIndex.ToString());

        goOfSpecialAbility = null;
        specialAbilityName = "";
        for (int i = 0; i < containersOfAllSpecialAbilities.Length; i++)
        {
            if (i == finalShapeIndex)
            {
                foreach (Transform but1 in containersOfAllSpecialAbilities[i].transform)
                {
                    Button but = but1.GetComponent<Button>();
                    but.GetComponent<SpecialAbility>().setButton();

                    //Safety Check
                    if (but.GetComponent<SpecialAbility>().ID == IDOfSpecialAbility && but.GetComponent<SpecialAbility>().level <= 0)
                    {
                        IDOfSelectedAbility = -1;
                        goOfSpecialAbility = null;
                        if (P1)
                            PlayerPrefs.SetInt("SpecialAbilityID" + finalShapeIndex.ToString(), -1);
                        else
                            PlayerPrefs.SetInt("2SpecialAbilityID" + finalShapeIndex.ToString(), -1);
                    }

                    if (but.GetComponent<SpecialAbility>().ID == IDOfSpecialAbility)
                    {
                        but.transform.Find("Panel").gameObject.GetComponent<Image>().color = new Color(1f, .9f, 0);
                        but.GetComponent<SpecialAbility>().selected = true;
                        goOfSpecialAbility = but.gameObject;
                        specialAbilityName = but.GetComponentInChildren<Text>().text;
                    }
                    else
                    {
                        but.transform.Find("Panel").gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                        but.GetComponent<SpecialAbility>().selected = false;
                    }
                }
                foreach (Transform but1 in containersOfAllSpecialAbilities[i].transform) {
                    Button but = but1.GetComponent<Button>();
                    but.GetComponent<SpecialAbility>().setOrder();
                }
            }
            else
            {
                foreach (Transform but1 in containersOfAllSpecialAbilities[i].transform)
                {
                    Button but = but1.GetComponent<Button>();
                    but.transform.Find("Panel").gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                    but.GetComponent<SpecialAbility>().selected = false;
                }
            }
        }

        Transform par = containersOfAllSpecialAbilities[0].transform.parent.transform.parent.transform.parent;
        for(int i = 0; i < 4; i++)
        {
            par.GetChild(i).gameObject.SetActive(i == finalShapeIndex); 
        }

        updateSpecial();
    }

    public void updateSpecial()
    {
        for (int i = 0; i < 2; i++)
        {
            selectedSpecial[i].GetComponent<SelectedSpecialAbility>().ID = IDOfSpecialAbility;
            selectedSpecial[i].GetComponent<SelectedSpecialAbility>().curName = specialAbilityName;

            if (IDOfSpecialAbility > 200)
            {
                Int32.TryParse(Super200[IDOfSpecialAbility - 201], out selectedSpecial[i].GetComponent<SelectedSpecialAbility>().level);
            }
            else if (IDOfSpecialAbility > 100)
            {
                Int32.TryParse(Super100[IDOfSpecialAbility - 101], out selectedSpecial[i].GetComponent<SelectedSpecialAbility>().level);
            }

            selectedSpecial[i].GetComponent<SelectedSpecialAbility>().updateSpecial();
        }
    }

    public void updateShapeStats()
    {
        selectedShapePP.updateShapePPs();
        selectedShapeLvl.updateLevels();

        int i = 0;
        foreach(GameObject go in ChooseButtons)
        {
            if (shapeLvls[i] < 1)
            {
                go.transform.parent.Find("Attack").GetComponent<Text>().text = "+0";
                go.transform.parent.Find("Defense").GetComponent<Text>().text = "+0";
                go.transform.parent.Find("Health").GetComponent<Text>().text = "100";
                i++;
                continue;
            }
            go.transform.parent.Find("Attack").GetComponent<Text>().text = "+" + levelStats[shapeLvls[i] - 1][i][0].ToString();
            go.transform.parent.Find("Defense").GetComponent<Text>().text = "+" + levelStats[shapeLvls[i] - 1][i][1].ToString();
            go.transform.parent.Find("Health").GetComponent<Text>().text = (100 + levelStats[shapeLvls[i] - 1][i][2]).ToString();
            i++;
        }
    }

    public void updatePassives()
    {
        selectedPassives.GetComponent<Passive>().updateInfo();
    }

    public void updateEmotes()
    {
        foreach(Transform go in EmotesContainer.transform)
        {
            go.Find("Name").GetComponent<Image>().color = ShapeConstants.bckgNameColor[finalShapeIndex];
            for (int i = 0; i < 4; i++)
            {
                go.Find("Emote").GetChild(0).GetChild(i).gameObject.SetActive(i == finalShapeIndex);
            }
        }

        EmoteToSelect.Chosen = 0;
        for (int i = 0; i < EmotesID.Length; i++)
        {
            if (EmotesID[i] != -1)
                EmoteToSelect.Chosen++;
        }

        EmotesText.text = EmoteToSelect.Chosen.ToString() + "/4 Emotes selected";
    }

    public void rejectedFriendly()
    {
        Cancel.GetComponent<CancelSearch>().Cancel();
        FriendlyBattle.SetActive(false);
        showMessage("Your friend rejected your request");
    }

    //In red
    public void showMessage(string msg)
    {
        MessagePanel.SetActive(true);
        MessagePanel.transform.Find("Text").GetComponent<Text>().text = msg;
    }
    
    public void showError()
    {
        ErrorPanel.SetActive(true);
    }
}