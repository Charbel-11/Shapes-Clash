using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameMaster : MonoBehaviour {
    public static bool doneInit = false;
    public Sprite[] ShapeXP;

    public static bool botActive;
    public static bool botOnline;
    public Bot bot;

    public Shape_Player player1, player12;
    public Shape_Player player2, player22;

    public Camera cameraPlayer1, cameraPlayer2, cameraFight;
    public Canvas canvasPlayer1, canvasPlayer2, endGame;

    public Cube_Player cubeP1, cubeP2;
    public Pyramid_Player pyramidP1, pyramidP2;
    public Star_Player starP1, starP2;
    public Sphere_Player sphereP1, sphereP2;

    public GameObject miniCubeEarth, miniCubeFire, miniCubeWater, miniCubeWind, MiddleRubble;
    public FallInPieces[] Rubbles;

    protected bool player1TriedToAtck, player1EscapeFromAtck;
    protected int player1OverallAttack, player1OverallDefense, player1DamageDone;
    protected int player1AtckDir, player1DefDir, player1EscapeCount;
    protected int[] player1Attack = { 0, 0, 0 };
    protected int[] player1Defense = { 0, 0, 0 };
    protected bool[] player1EscapeFrom = { false, false, false };
    protected int boostP1 = 0;
    public int player1Life;
    protected bool player1ChoiceDone;
    protected int player1ID;
    protected int player1selfdmg;
    public static Animator animatorPlayer1;
    public int skin;

    protected bool player2TriedToAtck, player2EscapeFromAtck;
    protected int player2OverallAttack, player2OverallDefense, player2DamageDone;
    protected int player2AtckDir, player2DefDir, player2EscapeCount;
    protected int[] player2Attack = { 0, 0, 0 };
    protected int[] player2Defense = { 0, 0, 0 };
    protected bool[] player2EscapeFrom = { false, false, false };
    protected int boostP2 = 0;
    public int player2Life;
    protected bool player2ChoiceDone;
    protected int player2ID;
    protected int player2selfdmg;
    public static Animator animatorPlayer2;
    public int skin2;
    protected bool sameEscape;

    //For the ability selection process
    protected int[] finalIDs1;
    public int shapeID1;
    public int shapeID12;   //second shape of p1
    protected int specialAbilityID1;
    protected int[] finalIDs2;
    public int shapeID2, shapeID22;
    protected int specialAbilityID2;
    protected Transform Ab;

    protected int netDamageTakenP1;
    protected int netDamageTakenP2;

    protected bool call;
    public int round;
    public int StartRoundCountCurseP1, StartRoundCountCurseP2;
    public int StartRoundCountP1, StartRoundCountP2;
    public int StartRoundCountBurn1, StartRoundCountBurn2;
    public int StartRoundCountFreeze1, StartRoundCountFreeze2;
    public int StartRoundCountStuckInPlace1, StartRoundCountStuckInPlace2;

    public bool protectiveEarthEffectP1 = false;
    public bool protectiveEarthEffectP2 = false;

    public static string[] AbLevelArray;
    public static string[] Super100;
    public static string[] Super200;

    public string[] StatsArrStr;
    public string[] Super100StatsArrStr;
    public string[] Super200StatsArrStr;
    public static int[][] StatsArr;
    public static int[][] Super100StatsArr;
    public static int[][] Super200StatsArr;
    public static int[] rarity;

    private string levelStatsStr;
    public static int[][][] levelStats;

    public static int[] EmotesID = new int[4];
    public static int[] EmotesID2 = new int[4];

    public int playerWon;
    public static bool Online;
    public static int Mode;
    public static bool Spectate = false;

    public GameObject[] Backgrounds;            //Set as in the order in the shop
    public int background;

    public bool MateOn1 = false;
    public bool MateOn2 = false;

    public int MatePow1 = 0;
    public int MatePow2 = 0;
    public int MateLP1 = 0;
    public int MateLP2 = 0;
    public GameObject MateLife1, MateLife2;
    public GameObject MateLife12, MateLife22;

    private bool StopMate1 = false;
    private bool StopMate2 = false;

    private bool First1 = true;
    private bool First2 = true;

    public bool AssistCube1 = false;
    public bool AssistCube2 = false;

    public bool FireEnergy1 = false;
    public bool FireEnergy2 = false;

    public bool BluePlanet1 = false;
    public bool BluePlanet2 = false;

    private bool Doit1 = false;
    private bool Doit2 = false;
    public int AssistPow1 = 0;
    public int AssistPow2 = 0;

    private bool DoitFire1 = false;
    private bool DoitFire2 = false;
    public int FirePow1 = 0;
    public int FirePow2 = 0;

    private bool DoitWater1 = false;
    private bool DoitWater2 = false;
    public int WaterPow1 = 0;
    public int WaterPow2 = 0;

    public bool DoitDoubleEdge1 = false;
    public bool DoitDoubleEdge2 = false;
    public bool DoubleEdge1 = false;
    public bool DoubleEdge2 = false;
    public int DoubleEdgeAtt1 = 0;
    public int DoubleEdgeDef1 = 0;
    public int DoubleEdgeAtt2 = 0;
    public int DoubleEdgeDef2 = 0;

    public bool Snowing1 = false;
    public bool Snowing2 = false;

    public bool Healing1 = false;
    public bool Healing2 = false;

    public int Heal1;
    public int Heal2;
    public int HealCount1 = 0;
    public int HealCount2 = 0;

    public bool PoisonAir1 = false;
    public bool PoisonAir2 = false;
    public int PoisonPow1 = 0;
    public int PoisonPow2 = 0;

    public int HelperSpheres1 = 0;
    public int HelperSpheres2 = 0;

    public bool DoitPoison1 = false;
    public bool DoitPoison2 = false;

    public bool DoitSmoke1 = false;
    public bool DoitSmoke2 = false;

    public static bool Replay = false;

    public int[][] PassiveStats;
    public int[] PassivesArray;

    public int previousPP;

    public LifeBar_ValueChange[] LifeBars;

    public static List<GameObject> ObjectsToDestroy = new List<GameObject>();

    public int PPDrain1 = 0;
    public int PPDrain2 = 0;

    public int HardeningPow1 = 0;
    public int HardeningPow2 = 0;

    public List<int> ChoicesP1 = new List<int>();
    public List<int> ChoicesP2 = new List<int>();
    public List<int> ProbsP1 = new List<int>();
    public List<int> ProbsP2 = new List<int>();

    public static bool Reconnect = false;
    public static bool Disconnected = false;
    public static bool StopEvaluating = false;

    public GameObject roundDisplay1, roundDisplay2;
    public int cameraState1, cameraState2;

    public Text[] SecondsLeft;

    public bool HitFace1 = false;
    public bool HitFace2 = false;

    public GameObject FightCanvas;
    public Text FightText;

    public String p1Name, p2Name;

    public static bool DisconnectedFirstTime = false;


    // Has to be awake to deactivate the unused player before a script calls them
    protected virtual void Awake() {
        if (!(this is GameMasterOffline))
            PlayerPrefs.SetInt("BotOnline", 0);
        doneInit = false;
        Input.multiTouchEnabled = false;


        ObjectsToDestroy = new List<GameObject>();

        Screen.SetResolution(1920, 1080, true);

        botActive = PlayerPrefs.GetInt("bot") == 1 || PlayerPrefs.GetInt("BotOnline") == 1;
        botOnline = PlayerPrefs.GetInt("BotOnline") == 1;
        bot.gameObject.SetActive(botActive);

        background = PlayerPrefs.GetInt("BckgdID");
        for (int i = 0; i < Backgrounds.Length; i++) {
            Backgrounds[i].SetActive(i == background);
        }

        if (background == 0) {
            cameraPlayer1.backgroundColor = new Color(0, 0, 0);
            cameraPlayer2.backgroundColor = new Color(0, 0, 0);
            cameraFight.backgroundColor = new Color(0, 0, 0);
        }
        else if (background == 1) {
            cameraPlayer1.backgroundColor = new Color(0.5568628f, 0.09019608f, 0.09803922f);
            cameraPlayer2.backgroundColor = new Color(0.5568628f, 0.09019608f, 0.09803922f);
            cameraFight.backgroundColor = new Color(0.5568628f, 0.09019608f, 0.09803922f);

            Transform temp = Backgrounds[1].transform.Find("UP");
            temp.Find("Front").gameObject.SetActive(false);
            temp.Find("P1").gameObject.SetActive(true);
            temp.Find("P2").gameObject.SetActive(false);
            temp.Find("BackEnd1").gameObject.SetActive(false);
            temp.Find("BackEnd2").gameObject.SetActive(false);
        }

        round = 1;
        StartRoundCountP1 = 0;
        StartRoundCountP2 = 0;
        playerWon = 0;

        protectiveEarthEffectP1 = false;
        protectiveEarthEffectP2 = false;

        cameraPlayer1.gameObject.SetActive(true);
        cameraPlayer2.gameObject.SetActive(false);
        cameraFight.gameObject.SetActive(false);
        endGame.gameObject.SetActive(false);

        skin = PlayerPrefs.GetInt("SkinID");

        if (this is GameMasterOffline && ((GameMasterOffline)this).Tutorial) {
            AbLevelArray = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1".Split(',');
            Super100 = Super200 = "1,1,1,1".Split(',');
        }
        else {
            AbLevelArray = PlayerPrefsX.GetStringArray("AbilitiesArray");
            Super100 = PlayerPrefsX.GetStringArray("Super100Array");
            Super200 = PlayerPrefsX.GetStringArray("Super200Array");
        }
        rarity = PlayerPrefsX.GetIntArray("AbilitiesRarety");

        StatsArrStr = PlayerPrefsX.GetStringArray("StatsArray");
        Super100StatsArrStr = PlayerPrefsX.GetStringArray("Super100StatsArray");
        Super200StatsArrStr = PlayerPrefsX.GetStringArray("Super200StatsArray");

        StatsArr = ClientHandleData.TransformStringArray(StatsArrStr);
        Super100StatsArr = ClientHandleData.TransformStringArray(Super100StatsArrStr);
        Super200StatsArr = ClientHandleData.TransformStringArray(Super200StatsArrStr);

        levelStatsStr = PlayerPrefs.GetString("LevelStats");
        levelStats = ClientHandleData.TransformToArrayShop(levelStatsStr);

        string[] PassiveStatsArray = PlayerPrefsX.GetStringArray("PassiveStatsArray");
        PassiveStats = ClientHandleData.TransformStringArray(PassiveStatsArray); // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog
        PassivesArray = PlayerPrefsX.GetIntArray("PassivesArray"); // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog

        if (Online) {
            shapeID1 = TempOpponent.Opponent.ShapeIDUser;
            shapeID12 = TempOpponent.Opponent.ShapeID2User;
            shapeID2 = TempOpponent.Opponent.ShapeID;
            shapeID22 = TempOpponent.Opponent.ShapeID12;
        }
        else {
            shapeID1 = PlayerPrefs.GetInt("ShapeSelectedID");

            if (PlayerPrefs.GetInt("BotOnline") == 1)
                shapeID2 = TempOpponent.Opponent.ShapeID;
            else
                shapeID2 = PlayerPrefs.GetInt("2ShapeSelectedID");
        }

        cubeP1.gameObject.SetActive(shapeID1 == 0);
        pyramidP1.gameObject.SetActive(shapeID1 == 1);
        starP1.gameObject.SetActive(shapeID1 == 2);
        sphereP1.gameObject.SetActive(shapeID1 == 3);
        if (shapeID1 == 0)
            player1 = cubeP1;
        else if (shapeID1 == 1)
            player1 = pyramidP1;
        else if (shapeID1 == 2)
            player1 = starP1;
        else if (shapeID1 == 3)
            player1 = sphereP1;

        //Set up the correct skin
        player1.transform.Find("Design").Find("Cube").gameObject.SetActive(0 == skin);
        player1.transform.Find("Design").Find("Cube").GetComponent<MeshRenderer>().enabled = (0 == skin);
        for (int i = 1; i < 2; i++) {
            player1.transform.Find("Design").Find("Cube" + i.ToString()).gameObject.SetActive(i == skin);
        }

        cubeP2.gameObject.SetActive(shapeID2 == 0);
        pyramidP2.gameObject.SetActive(shapeID2 == 1);
        starP2.gameObject.SetActive(shapeID2 == 2);
        sphereP2.gameObject.SetActive(shapeID2 == 3);
        if (shapeID2 == 0)
            player2 = cubeP2;
        else if (shapeID2 == 1)
            player2 = pyramidP2;
        else if (shapeID2 == 2)
            player2 = starP2;
        else if (shapeID2 == 3)
            player2 = sphereP2;

        doneInit = true;

        animatorPlayer1 = player1.GetComponent<Animator>();
        animatorPlayer2 = player2.GetComponent<Animator>();
        animatorPlayer1.SetInteger("ID", -1);
        animatorPlayer2.SetInteger("ID", -1);

        if (shapeID1 == 1 || shapeID1 == 2 || shapeID1 == 3) {
            animatorPlayer1.SetBool("Player1", true);
        }

        LifeBars = new LifeBar_ValueChange[10];
        LifeBars[0] = canvasPlayer1.transform.Find("Player1Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[1] = canvasPlayer1.transform.Find("Player2Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[2] = canvasPlayer2.transform.Find("Player1Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[3] = canvasPlayer2.transform.Find("Player2Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[4] = canvasPlayer1.transform.Find("MateLife").GetComponent<LifeBar_ValueChange>();
        LifeBars[5] = canvasPlayer1.transform.Find("MateLife2").GetComponent<LifeBar_ValueChange>();
        LifeBars[6] = canvasPlayer2.transform.Find("MateLife").GetComponent<LifeBar_ValueChange>();
        LifeBars[7] = canvasPlayer2.transform.Find("MateLife2").GetComponent<LifeBar_ValueChange>();
        LifeBars[8] = canvasPlayer1.transform.Find("Switch").GetComponentInChildren<LifeBar_ValueChange>();
        LifeBars[9] = canvasPlayer2.transform.Find("Switch").GetComponentInChildren<LifeBar_ValueChange>();


        MateLife1 = canvasPlayer1.transform.Find("MateLife").gameObject;
        MateLife2 = canvasPlayer1.transform.Find("MateLife2").gameObject;
        MateLife1.SetActive(false);
        MateLife2.SetActive(false);

        FightCanvas = GameObject.Find("FightCanvas");
        FightText = FightCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        FightCanvas.SetActive(false);

        if (Reconnect) {
            player1.Reconnect = true;
            player2.Reconnect = true;
            player1.SetLife(TempOpponent.Opponent.LP1);
            player2.SetLife(TempOpponent.Opponent.LP2);
            player1.SetEP(TempOpponent.Opponent.EP1);
            player2.SetEP(TempOpponent.Opponent.EP2);
            round = TempOpponent.Opponent.Round;
            foreach (int C in TempOpponent.Opponent.Choices1)
                ChoicesP1.Add(C);

            foreach (int C in TempOpponent.Opponent.Choices2)
                ChoicesP2.Add(C);

            foreach (int C in TempOpponent.Opponent.Probs1)
                ProbsP1.Add(C);

            foreach (int C in TempOpponent.Opponent.Probs2)
                ProbsP2.Add(C);

            ReadReconnectString(ClientHandleData.ReconnectString, PlayerPrefs.GetInt("BotOnline") == 1);
        }

        if (botActive) { bot.StartFunction(); }
    }

    //Has to be in start for the ability button to get the correct IDs in Awake
    protected virtual void Start() {
        cameraState1 = cameraState2 = 1;

        ResetRound(true);
        canvasPlayer1.gameObject.SetActive(true);
        if (canvasPlayer2 != null)
            canvasPlayer2.gameObject.SetActive(false);

        string[] namesS = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
        Ab = canvasPlayer1.transform.Find("AllUsualAbilities");
        for (int j = 0; j < Ab.childCount; j++) {
            Transform child = Ab.GetChild(j);
            bool used = false;
            for (int k = 0; k < 6; k++) {
                if (child.GetComponent<Shape_Abilities>().ID == finalIDs1[k]) {
                    if (child.GetComponent<Shape_Abilities>().common == true) {
                        for (int i = 0; i < namesS.Length; i++)
                            child.transform.Find(namesS[i]).gameObject.SetActive(i == shapeID1);
                    }
                    used = true;
                    child.parent = canvasPlayer1.transform.Find("Ability" + (k + 1).ToString());
                    child.SetSiblingIndex(0);
                    child.localScale = new Vector3(1f, 1f, 1f);
                    child.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                    child.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
                    child.GetComponent<Shape_Abilities>().Awake();
                    child.GetComponent<Shape_Abilities>().Start();
                    break;
                }
            }
            if (used) {
                j--; continue;
            }
            child.gameObject.SetActive(false);
        }

        //For the shape's specific attack
        Ab = canvasPlayer1.transform.Find("Attack");
        for (int i = 0; i < 4; i++) {
            Ab.GetChild(i).gameObject.SetActive(i == shapeID1);
            if (Ab.GetChild(i).gameObject.activeSelf)
                Ab.GetChild(i).GetComponent<Shape_Abilities>().Awake();
        }

        //For the special ability
        Ab = canvasPlayer1.transform.Find("AllSpecialAbilities");
        foreach (Transform child in Ab) {
            if (child.name == "AbDescription") { continue; }
            child.gameObject.SetActive(specialAbilityID1 == child.GetComponent<Shape_Abilities>().ID);
            if (child.gameObject.activeSelf)
                child.GetComponent<Shape_Abilities>().Awake();
        }

        //For the energy point
        Transform ep = canvasPlayer1.transform.Find("EnergyPoints");
        for (int i = 0; i < 4; i++) {
            ep.GetChild(i).gameObject.SetActive(i == shapeID1);
        }

        for (int j = 1; j <= 6; j++)
            canvasPlayer1.transform.Find("Ability" + j.ToString()).Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(finalIDs1[j - 1]);

        canvasPlayer1.transform.Find("AllSpecialAbilities").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(specialAbilityID1);

        int[] attackID = { 1, 22, 23, 42 };
        canvasPlayer1.transform.Find("Attack").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(attackID[shapeID1]);
        //Player 2 is done in offline mode only

        canvasPlayer1.transform.Find("Get_EP").GetComponent<Shape_Abilities>().Awake();

        //Check shou elon 3aze hole l rubbles
        GameObject[] Rubble = new GameObject[MiddleRubble.transform.childCount];
        Rubbles = new FallInPieces[MiddleRubble.transform.childCount];
        for (int i = 0; i < MiddleRubble.transform.childCount; i++) {
            Rubble[i] = MiddleRubble.transform.GetChild(i).gameObject;
            Rubbles[i] = Rubble[i].GetComponent<FallInPieces>();
        }

        Transform MiniEnemy1 = player1.transform.Find("MiniEnemy");
        Transform MiniEnemy2 = player2.transform.Find("MiniEnemy");
        for (int i = 0; i < 4; i++) {
            MiniEnemy1.GetChild(i).gameObject.SetActive(i == shapeID2);
            MiniEnemy2.GetChild(i).gameObject.SetActive(i == shapeID1);
        }

        previousPP = PlayerPrefsX.GetIntArray("PP")[shapeID1];

        //To display the correct round
        if (!Reconnect) {
            roundDisplay1 = canvasPlayer1.transform.Find("Round").gameObject;
            roundDisplay2 = canvasPlayer2.transform.Find("Round").gameObject;

            roundDisplay1.transform.Find("Text").GetComponent<Text>().text = "1";
            roundDisplay1.transform.Find("X2").gameObject.SetActive(false);
            roundDisplay2.transform.Find("Text").GetComponent<Text>().text = "1";
            roundDisplay2.transform.Find("X2").gameObject.SetActive(false);
        }
        else {
            Debug.Log("The opponent already chose " + TempOpponent.Opponent.ChoiceID);
            if (TempOpponent.Opponent.ChoiceID != -1) {
                if (TempOpponent.Opponent.Abilities.TryGetValue(TempOpponent.Opponent.ChoiceID, out TempOpponent.Method method)) {
                    method.Invoke();
                }
                else {
                    Shape_Abilities[] AbilitiesP2 = player2.GetComponentsInChildren<Shape_Abilities>();
                    foreach (Shape_Abilities Ability in AbilitiesP2) {
                        if (Ability.ID == TempOpponent.Opponent.ChoiceID) {
                            Ability.Awake();
                            Ability.UseAbility();
                            break;
                        }
                    }
                }
            }
            roundDisplay1 = canvasPlayer1.transform.Find("Round").gameObject;
            roundDisplay2 = canvasPlayer2.transform.Find("Round").gameObject;
            if (round > 5 && round < 10) {
                roundDisplay1.transform.Find("X2").gameObject.SetActive(true);
                if (!(Replay || Spectate))
                    roundDisplay2.transform.Find("X2").gameObject.SetActive(true);
            }
            else if (round > 10 && round < 15) {
                roundDisplay1.transform.Find("X2").GetComponent<Text>().text = "x3";
                if (!(Replay || Spectate))
                    roundDisplay2.transform.Find("X2").GetComponent<Text>().text = "x3";
            }
            else if (round > 15) {
                roundDisplay1.transform.Find("X2").GetComponent<Text>().text = "x4";
                if (!(Replay || Spectate))
                    roundDisplay2.transform.Find("X2").GetComponent<Text>().text = "x4";
            }
        }
    }

    public void changeCameraState() {
        if (canvasPlayer1.gameObject.activeSelf == true) {
            cameraState1++;
            cameraState1 %= 2;

            if (cameraState1 == 0) {
                cameraPlayer1.gameObject.SetActive(false);
                cameraFight.gameObject.SetActive(true);
            }
            else {
                cameraFight.gameObject.SetActive(false);
                cameraPlayer1.gameObject.SetActive(true);
            }

            if (background == 1) {
                Transform temp = Backgrounds[1].transform.Find("UP");
                temp.Find("Front").gameObject.SetActive(cameraState1 == 0);
                temp.Find("P1").gameObject.SetActive(cameraState1 == 1);
            }

            string[] leftHandSide = { "Player1Life", "Player2Life", "MateLife", "MateLife2" };
            string[] rightHandSide = { "Player1Life0", "Player2Life0", "MateLife0", "MateLife20" };

            for (int i = 0; i < leftHandSide.Length; i++) {
                Vector2 temp = canvasPlayer1.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMax;
                canvasPlayer1.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMax = canvasPlayer1.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMax;
                canvasPlayer1.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMax = temp;

                temp = canvasPlayer1.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMin;
                canvasPlayer1.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMin = canvasPlayer1.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMin;
                canvasPlayer1.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMin = temp;
            }

            if (Online) {
                if (this.GetComponent<GameMasterOnline>().v2 == true)
                    cameraState2 = cameraState1;
            }
        }
        else {
            cameraState2++;
            cameraState2 %= 2;

            print(cameraState2);
            if (cameraState2 == 0) {
                cameraPlayer2.gameObject.SetActive(false);
                cameraFight.gameObject.SetActive(true);
            }
            else {
                cameraFight.gameObject.SetActive(false);
                cameraPlayer2.gameObject.SetActive(true);
            }

            if (background == 1) {
                Transform temp = Backgrounds[1].transform.Find("UP");
                temp.Find("Front").gameObject.SetActive(cameraState2 == 0);
                temp.Find("P2").gameObject.SetActive(cameraState2 == 1);
            }

            string[] leftHandSide = { "Player1Life", "Player2Life", "MateLife", "MateLife2" };
            string[] rightHandSide = { "Player1Life0", "Player2Life0", "MateLife0", "MateLife20" };

            for (int i = 0; i < leftHandSide.Length; i++) {
                Vector2 temp = canvasPlayer2.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMax;
                canvasPlayer2.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMax = canvasPlayer2.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMax;
                canvasPlayer2.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMax = temp;

                temp = canvasPlayer2.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMin;
                canvasPlayer2.transform.Find(leftHandSide[i]).GetComponent<RectTransform>().anchorMin = canvasPlayer2.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMin;
                canvasPlayer2.transform.Find(rightHandSide[i]).GetComponent<RectTransform>().anchorMin = temp;
            }

            if (Online) {
                if (this.GetComponent<GameMasterOnline>().v2 == true)
                    cameraState1 = cameraState2;
            }
        }
    }

    public virtual void TryEvaluating() { }

    //states: {0:assist}, {1:fire energy}, {2:double edge}, {3:snow}, {4:blue planet}, {5:poison air}, {6:helper spheres}
    void BoostAttack(int state, bool p1) {
        if (p1 && (player1OverallAttack > 0 || MateOn1)) {
            int add = 0;
            if (state == 0) { add = player1.CubeAssist.GetAttPow(); }
            else if (state == 1) { add = FirePow1; }
            else if (state == 2) { add = DoubleEdgeAtt1; }
            else if (state == 3) { add = PassiveStats[3][PassivesArray[3] - 1]; }
            else if (state == 4) { add = WaterPow1; }
            else if (state == 5) { add = PoisonPow1; }
            else if (state == 6) { add = PassiveStats[6][PassivesArray[6] - 1]; }

            boostP1 += add;
            player1.setAdditionalDamage(boostP1);
            player1OverallAttack = player1.getOverallAttack();
            player1Attack = player1.getAttackArr();
        }
        else if (!p1 && (player2OverallAttack > 0 || MateOn2)) {
            int add = 0;
            if (state == 0) { add = player2.CubeAssist.GetAttPow(); }
            else if (state == 1) { add = FirePow2; }
            else if (state == 2) { add = DoubleEdgeAtt2; }
            else if (state == 3) { add = PassiveStats[3][TempOpponent.Opponent.Passives[3] - 1]; }
            else if (state == 4) { add = WaterPow2; }
            else if (state == 5) { add = PoisonPow2; }
            else if (state == 6) { add = PassiveStats[6][TempOpponent.Opponent.Passives[6] - 1]; }


            boostP2 += add;
            player2.setAdditionalDamage(boostP2);
            player2OverallAttack = player2.getOverallAttack();
            player2Attack = player2.getAttackArr();
        }

        if (p1) {
            if (player1OverallAttack > 0) {
                if (state == 3) { AddText(FightText, p1Name + "'s attack was strenghtened by the snow"); }
            }
            else {
                if (state == 3) { AddText(FightText, p1Name + "'s snow power boost wore off because he didn't attack"); }
            }
        }
        else {
            if (player2OverallAttack > 0) {
                if (state == 3) { AddText(FightText, p2Name + "'s attack was strenghtened by the snow"); }
            }
            else {
                if (state == 3) { AddText(FightText, p2Name + "'s snow power boost wore off because he didn't attack"); }
            }

        }
    }

    protected virtual IEnumerator EvaluateOutput() {
        FightCanvas.SetActive(true);
        cameraFight.gameObject.SetActive(true);

        player1Attack = player1.getAttackArr();
        player1Defense = player1.getDefenseArr();
        player1EscapeFrom = player1.getEscapeFromArr();
        player1Life = player1.GetLife();
        player1ID = player1.GetIdOfAnimUsed();
        player1selfdmg = player1.GetSelfdmg();

        player2Attack = player2.getAttackArr();
        player2Defense = player2.getDefenseArr();
        player2EscapeFrom = player2.getEscapeFromArr();
        player2Life = player2.GetLife();
        player2ID = player2.GetIdOfAnimUsed();
        player2selfdmg = player2.GetSelfdmg();

        player1OverallAttack = player1OverallDefense = player1AtckDir = player1DefDir = player1EscapeCount = 0;
        player2OverallAttack = player2OverallDefense = player2AtckDir = player2DefDir = player2EscapeCount = 0;
        sameEscape = true; player1EscapeFromAtck = player2EscapeFromAtck = false;
        for (int i = 0; i < 3; i++) {
            player1OverallAttack += player1Attack[i];
            player1OverallDefense += player1Defense[i];
            if (player1Attack[i] > 0) { player1AtckDir++; }
            if (player1Defense[i] > 0) { player1DefDir++; }
            if (player1EscapeFrom[i]) { player1EscapeCount++; }

            player2OverallAttack += player2Attack[i];
            player2OverallDefense += player2Defense[i];
            if (player2Attack[i] > 0) { player2AtckDir++; }
            if (player2Defense[i] > 0) { player2DefDir++; }
            if (player2EscapeFrom[i]) { player2EscapeCount++; }

            if (player1EscapeFrom[i] ^ player2EscapeFrom[i]) { sameEscape = false; }
            if (player1EscapeFrom[i] && player2Attack[i] > 0) { player1EscapeFromAtck = true; }
            if (player2EscapeFrom[i] && player1Attack[i] > 0) { player2EscapeFromAtck = true; }
        }
        player1TriedToAtck = (player1OverallAttack > 0);
        player2TriedToAtck = (player2OverallAttack > 0);

        if (player1Attack[1] > 0)
            animatorPlayer2.SetBool("AttBelow", true);
        if (player2Attack[1] > 0)
            animatorPlayer1.SetBool("AttBelow", true);

        if (player1ID < 100 && player2ID < 100) {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips[player1ID]);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips[player2ID]);
        }
        else if (player1ID > 100 && player2ID < 100) {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips[player2ID]);
            if (player1ID < 200) { SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips100[player1ID - 101]); }
            else { SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips200[player1ID - 201]); }
        }
        else if (player1ID < 100 && player2ID > 100) {
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips[player1ID]);
            if (player2ID < 200) { SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips100[player2ID - 101]); }
            else { SoundManager.Instance.PlayOneShot(SoundManager.Instance.Clips200[player2ID - 201]); }
        }
        // No sound if both use super

        if (player1ID == 54)
            player1.transform.Find("Design").gameObject.layer = 14;
        if (player2ID == 54)
            player2.transform.Find("Design").gameObject.layer = 17;

        if (background == 1) {
            Transform temp = Backgrounds[1].transform.Find("UP");
            temp.Find("Front").gameObject.SetActive(true);
            temp.Find("P1").gameObject.SetActive(false);
            temp.Find("P2").gameObject.SetActive(false);
        }

        //To see the ability better from a wider view
        if (player1ID == 7 || player2ID == 7 || player1ID == 10 || player2ID == 10 || player1ID >= 100 || player2ID >= 100) {
            if (background == 1) {
                Backgrounds[1].transform.Find("UP").Find("Front").gameObject.SetActive(false);
                Backgrounds[1].transform.Find("UP").Find("FrontSpare").gameObject.SetActive(true);
            }
            cameraFight.fieldOfView = 80;
        }
        else {
            cameraFight.fieldOfView = 65;
        }


        //Remove the ability's PP
        player1.SetEP(player1.GetEP() - player1.GetEPToRemove());
        player2.SetEP(player2.GetEP() - player2.GetEPToRemove());

        if (player1ID == 8)
            StartRoundCountP1 = round;
        if (player2ID == 8)
            StartRoundCountP2 = round;

        //For the bullet instantiation
        if ((player1ID == 7 || player1ID == 21) && shapeID1 == 2) {
            animatorPlayer1.SetBool("Player1", true);
        }

        #region AssistAbilities
        //Cube Assist
        if (AssistCube1) {
            if (Doit1) {
                BoostAttack(0, true);
                if (player1OverallAttack > 0) {
                    if (player1Attack[2] > 0) { player1.CubeAssist.Anim.SetInteger("ID", 0); }
                    else if (player1Attack[0] > 0) { player1.CubeAssist.Anim.SetInteger("ID", 1); }
                    else if (player1Attack[1] > 0) { player1.CubeAssist.Anim.SetInteger("ID", 2); }
                    else if (MateOn1) { player1.CubeAssist.Anim.SetInteger("ID", 0); }

                }
                Doit1 = AssistCube1 = false;
                StartCoroutine(player1.CubeAssist.SetFalse());
            }
            else {
                player1.CubeAssist.gameObject.SetActive(true);
                player1.CubeAssist.Initialize(AssistPow1);
                Doit1 = true;
            }
        }
        if (AssistCube2) {
            if (Doit2) {
                BoostAttack(0, false);
                if (player2OverallAttack != 0) {
                    if (player2Attack[2] > 0) { player2.CubeAssist.Anim.SetInteger("ID", 0); }
                    else if (player2Attack[0] > 0) { player2.CubeAssist.Anim.SetInteger("ID", 1); }
                    else if (player2Attack[1] > 0) { player2.CubeAssist.Anim.SetInteger("ID", 2); }
                    else if (MateOn2) { player2.CubeAssist.Anim.SetInteger("ID", 0); }
                }
                Doit2 = AssistCube2 = false;
                StartCoroutine(player2.CubeAssist.SetFalse());
            }
            else {
                player2.CubeAssist.gameObject.SetActive(true);
                player2.CubeAssist.Initialize(AssistPow2);
                Doit2 = true;
            }
        }

        //Fire Energy
        if (FireEnergy1) {
            if (DoitFire1) {
                BoostAttack(1, true);
                FirePow1 = 0;
                DoitFire1 = FireEnergy1 = false;
            }
            else {
                DoitFire1 = true;
            }
        }
        if (FireEnergy2) {
            if (DoitFire2) {
                BoostAttack(1, false);
                FirePow2 = 0;
                DoitFire2 = FireEnergy2 = false;
            }
            else {
                DoitFire2 = true;
            }
        }

        //DoubleEdged Sword
        if (DoubleEdge1) {
            if (DoitDoubleEdge1) {
                BoostAttack(2, true);
                if (player1OverallAttack > 0) { DoubleEdgeAtt1 = 0; }
                if (player1OverallDefense > 0) {
                    player1OverallDefense -= DoubleEdgeDef1;
                    if (player1OverallDefense < 0) {
                        DoubleEdgeDef1 = -player1OverallDefense;
                        player1OverallDefense = 0;
                    }
                    else {
                        DoubleEdgeDef1 = 0;
                    }
                    for (int i = 0; i < 3; i++) {
                        if (player1Defense[i] > 0) { player1Defense[i] = player1OverallDefense / player1DefDir; }
                    }
                    player1.SetShieldPower(player1Defense);
                }
                DoitDoubleEdge1 = false;
            }
            else {
                DoitDoubleEdge1 = true;
            }
        }
        if (DoubleEdge2) {
            if (DoitDoubleEdge2) {
                BoostAttack(2, false);
                if (player2OverallAttack > 0) { DoubleEdgeAtt2 = 0; }
                if (player2OverallDefense != 0) {
                    player2OverallDefense -= DoubleEdgeDef2;
                    if (player2OverallDefense < 0) {
                        DoubleEdgeDef2 = -player2OverallDefense;
                        player2OverallDefense = 0;
                    }
                    else {
                        DoubleEdgeDef2 = 0;
                    }
                    for (int i = 0; i < 3; i++) {
                        if (player2Defense[i] > 0) { player2Defense[i] = player2OverallDefense / player2DefDir; }
                    }
                    player2.SetShieldPower(player2Defense);
                }
                DoitDoubleEdge2 = false;
            }
            else {
                DoitDoubleEdge2 = true;
            }
        }

        //Snow Passive
        if (Snowing1) {
            BoostAttack(3, true);
            player1.GetComponent<Pyramid_Player>().Snow.SetActive(false);
            Snowing1 = false;
        }
        if (Snowing2) {
            BoostAttack(3, false);
            player2.GetComponent<Pyramid_Player>().Snow.SetActive(false);
            Snowing2 = false;
        }

        //Blue Planet
        if (BluePlanet1) {
            if (DoitWater1) {
                BoostAttack(4, true);
                WaterPow1 = 0;
                DoitWater1 = BluePlanet1 = false;
            }
            else {
                DoitWater1 = true;
            }
        }
        if (BluePlanet2) {
            if (DoitWater2) {
                BoostAttack(4, false);
                WaterPow2 = 0;
                DoitWater2 = BluePlanet2 = false;
            }
            else {
                DoitWater2 = true;
            }
        }

        //Healing
        if (Healing1) {
            if (HealCount1 < 3) {
                player1Life += Heal1;
                if (player1Life > player1.MaxLP) { player1Life = player1.MaxLP; }
                AddText(FightText, "Player 1 healed " + (player1Life - player1.GetLife()) + " HP");
                player1.SetLife(player1Life);
                HealCount1 += 1;
            }
        }
        if (Healing2) {
            if (HealCount2 < 3) {
                player2Life += Heal2;
                if (player2Life > player2.MaxLP) { player2Life = player2.MaxLP; }
                AddText(FightText, "Player 2 healed " + (player2Life - player2.GetLife()) + " HP");
                player2.SetLife(player2Life);
                HealCount2 += 1;
            }
        }

        //PoisonousAir
        if (PoisonAir1) {
            if (DoitPoison1) {
                BoostAttack(5, true);
                PoisonPow1 = 0;
                DoitPoison1 = PoisonAir1 = false;
            }
            else {
                DoitPoison1 = true;
            }
        }
        if (PoisonAir2) {
            if (DoitPoison2) {
                BoostAttack(5, false);
                PoisonPow2 = 0;
                DoitPoison2 = PoisonAir2 = false;
            }
            else {
                DoitPoison2 = true;
            }
        }

        //HelperSpheresPassive
        if (HelperSpheres1 == 1) {
            if (player1OverallAttack > 0 && player2ID < 100) {
                BoostAttack(6, true);
                if (player1Attack[2] > 0 || player1ID > 100)
                    player1.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 0);
                else if (player1Attack[0] > 0)
                    player1.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 1);
                else if (player1Attack[1] > 0)
                    player1.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 2);
            }
            HelperSpheres1 = 2;
        }
        if (HelperSpheres2 == 1) {
            if (player2OverallAttack != 0 && player1ID < 100) {
                BoostAttack(6, false);
                if (player2Attack[2] > 0 || player2ID > 100)
                    player2.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 0);
                else if (player2Attack[0] > 0)
                    player2.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 1);
                else if (player2Attack[1] > 0)
                    player2.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", 2);
            }
            HelperSpheres2 = 2;
        }

        //SmokePassive
        if (DoitSmoke1) {
            player1EscapeFrom[0] = player1EscapeFrom[1] = player1EscapeFrom[2] = true;
            player1EscapeCount = 3;
            DoitSmoke1 = false;
        }
        if (DoitSmoke2) {
            player2EscapeFrom[0] = player2EscapeFrom[1] = player2EscapeFrom[2] = true;
            player2EscapeCount = 3;
            DoitSmoke2 = false;
        }
        #endregion

        if (player1ID == 7 && player2Attack[0] > 0)
            animatorPlayer1.SetBool("ElevationStop", true);
        if (player2ID == 7 && player1Attack[0] > 0)
            animatorPlayer2.SetBool("ElevationStop", true);

        animatorPlayer1.SetInteger("ID", player1ID);
        animatorPlayer2.SetInteger("ID", player2ID);

        player1.Choice(player1ID);
        player2.Choice(player2ID);

        //Add EP to the corresponding player
        if (player1ID == 0)
            player1.SetEP(player1.GetEP() + 1);
        if (player2ID == 0)
            player2.SetEP(player2.GetEP() + 1);

        //For Curse Of The Cloud
        if (player1ID == 18 && player2EscapeFrom[0] == false) {
            StartRoundCountCurseP2 = round;
            player1.transform.Find("StormCloud").gameObject.SetActive(true);
        }
        if (player2ID == 18 && player1EscapeFrom[0] == false) {
            StartRoundCountCurseP1 = round;
            player2.transform.Find("StormCloud").gameObject.SetActive(true);
        }

        //for tackle&Co with CubeSealing&Co
        if (player1Attack[1] > 0 && player1Defense[2] > 0 && player2Attack[2] > 0 && (player2EscapeFrom[0] && player2EscapeFrom[1] && !player2EscapeFrom[2])) {
            if (player1OverallDefense >= player2OverallAttack)
                player2EscapeFrom[0] = player2EscapeFrom[1] = false;
        }
        if (player2Attack[1] > 0 && player2Defense[2] > 0 && player1Attack[2] > 0 && (player1EscapeFrom[0] && player1EscapeFrom[1] && !player1EscapeFrom[2])) {
            if (player2OverallDefense >= player1OverallAttack)
                player1EscapeFrom[0] = player1EscapeFrom[1] = false;
        }

        //To disable smoke
        if (player1EscapeCount == 3 && player1ID < 100 && ((player1OverallAttack > 0 || player1OverallDefense > 0) || player2ID > 100)) {
            player1EscapeFrom[0] = player1EscapeFrom[1] = player1EscapeFrom[2] = false;
            player1EscapeCount = 0;
            player1.GetComponent<Sphere_Player>().Smoke.SetActive(false);
        }
        if (player2EscapeCount == 3 && player2ID < 100 && ((player2OverallAttack > 0 || player2OverallDefense > 0) || player1ID > 100)) {
            player2EscapeFrom[0] = player2EscapeFrom[1] = player2EscapeFrom[2] = false;
            player2EscapeCount = 0;
            player2.GetComponent<Sphere_Player>().Smoke.SetActive(false);
        }

        if (player1EscapeCount == 3 && player1OverallAttack == 0 && player1OverallDefense == 0 && player2OverallAttack > 0 && player2ID < 100)
            AddText(FightText, p1Name + " escaped thanks to the smoke!");
        if (player2EscapeCount == 3 && player2OverallAttack == 0 && player2OverallDefense == 0 && player1OverallAttack > 0 && player1ID < 100)
            AddText(FightText, p2Name + " escaped thanks to the smoke!");

        //Handle Mate contribution (after boost)
        if (MateOn1) {
            bool defended = false;
            if (First1) {
                player1.Mate.gameObject.SetActive(true);
                player1.Mate.SetAdditionals(player1.getAddLvlAttack(), player1.getAddLvlDef());
                player1.Mate.IncreaseBulletPow(boostP1);
                player1.Mate.Initialize(MateLP1, MatePow1);
                MateLife1.SetActive(true);
                MateLife1.GetComponent<Slider>().maxValue = MateLP1;
                if (this is GameMasterOffline) {
                    MateLife12.SetActive(true);
                    MateLife12.GetComponent<Slider>().maxValue = MateLP1;
                }
                if (player1 is Star_Player) { player1.Mate.Anim.SetBool("Star", true); }
                First1 = false;
            }
            player1Attack[2] += player1.Mate.GetBulletPow();
            player1OverallAttack += player1.Mate.GetBulletPow();

            if (player2ID < 100) {
                player1.Mate.BulletInstantiation();
                MatePow1 = player1.Mate.GetBulletPow();
                MateLP1 = player1.Mate.GetLP();

                int[] diff = new int[] { 0, 0, 0 }; int maxDif = 0, idx = -1;
                for (int i = 0; i < 3; i++) {
                    if (i == 2 && player1ID == 11) { continue; }          //if atck straight and we have mirror, skip
                    diff[i] = player2Attack[i] - (player1Attack[i] + player1Defense[i]);
                    if (diff[i] > maxDif) { maxDif = diff[i]; idx = i; }
                    if (player1EscapeFrom[i] && i != 2) {
                        maxDif = 0; idx = -1;
                    }
                }
                if (idx != -1) {
                    player1.Mate.Anim.SetInteger("ID", (1 + idx) % 3);
                    player2Attack[idx] -= player1Attack[idx]; player1Attack[idx] = 0;
                    int delta = Math.Min(player2Attack[idx], MateLP1);
                    player2Attack[idx] -= delta; MateLP1 -= delta;
                    defended = true;
                }
            }
            else {
                player1.Mate.Anim.SetInteger("ID", (player2ID < 200 ? 1 : 2));
                int delta = Math.Min(player2Attack[2], MateLP1);
                player2Attack[2] -= delta; MateLP1 -= delta;
                defended = true;
            }
            if (defended) AddText(FightText, "The mate of " + p1Name + " defended him bravely!");
        }
        if (MateOn2) {
            bool defended = false;
            if (First2) {
                player2.Mate.gameObject.SetActive(true);
                player2.Mate.SetAdditionals(player2.getAddLvlAttack(), player2.getAddLvlDef());
                player2.Mate.IncreaseBulletPow(boostP2);
                player2.Mate.Initialize(MateLP2, MatePow2);
                MateLife2.SetActive(true);
                MateLife2.GetComponent<Slider>().maxValue = MateLP2;
                if (this is GameMasterOffline) {
                    MateLife22.SetActive(true);
                    MateLife22.GetComponent<Slider>().maxValue = MateLP2;
                }
                if (player2 is Star_Player) { player2.Mate.Anim.SetBool("Star", true); }
                First2 = false;
            }
            player2Attack[2] += player2.Mate.GetBulletPow();
            player2OverallAttack += player2.Mate.GetBulletPow();

            if (player1ID < 100) {
                player2.Mate.BulletInstantiation();
                MatePow2 = player2.Mate.GetBulletPow();
                MateLP2 = player2.Mate.GetLP();

                int[] diff = new int[] { 0, 0, 0 }; int maxDif = 0, idx = -1;
                for (int i = 0; i < 3; i++) {
                    if (i == 2 && player2ID == 11) { continue; }        //if atck straight and we have mirror, skip
                    diff[i] = player1Attack[i] - (player2Attack[i] + player2Defense[i]);
                    if (diff[i] > maxDif) { maxDif = diff[i]; idx = i; }
                    if (player2EscapeFrom[i] && i != 2) {
                        maxDif = 0; idx = -1;
                    }
                }
                if (idx != -1) {
                    player2.Mate.Anim.SetInteger("ID", (1 + idx) % 3);
                    player1Attack[idx] -= player2Attack[idx]; player2Attack[idx] = 0;
                    int delta = Math.Min(player1Attack[idx], MateLP2);
                    player1Attack[idx] -= delta; MateLP2 -= delta;
                    defended = true;
                }
            }
            else {
                player2.Mate.Anim.SetInteger("ID", (player1ID < 200 ? 1 : 2));
                int delta = Math.Min(player1Attack[2], MateLP2);
                player1Attack[2] -= delta; MateLP2 -= delta;
                defended = true;
            }
            if (defended) AddText(FightText, "The mate of " + p2Name + " defended him bravely!");
        }

        //Handle escapes
        if (player2ID < 100) {
            if (player1EscapeFrom[0]) { player2OverallAttack -= player2Attack[0]; player2Attack[0] = 0; }
            if (player1EscapeFrom[1]) { player2OverallAttack -= player2Attack[1]; player2Attack[1] = 0; }
            if (player1EscapeFrom[2]) { player2OverallAttack -= player2Attack[2]; player2Attack[2] = 0; }
        }
        if (player1ID < 100) {
            if (player2EscapeFrom[0]) { player1OverallAttack -= player1Attack[0]; player1Attack[0] = 0; }
            if (player2EscapeFrom[1]) { player1OverallAttack -= player1Attack[1]; player1Attack[1] = 0; }
            if (player2EscapeFrom[2]) { player1OverallAttack -= player1Attack[2]; player1Attack[2] = 0; }
        }

        if (MateOn2 && MateOn1 && player1ID == 9 && player2ID == 9 && !(MatePow1 == MatePow2)) {
            if (MatePow1 > MatePow2) {
                int EP = player2.GetEP();
                if (EP >= PPDrain1) {
                    player2.SetEP(EP - PPDrain1);
                    player1.SetEP(player1.GetEP() + PPDrain1);
                    AddText(FightText, p1Name + " drained " + PPDrain1 + " EP from " + p2Name);
                }
                else {
                    int Diff = PPDrain1 - EP;
                    player1.SetEP(player1.GetEP() + EP);
                    player2.SetEP(0);
                    player1Life += Diff * 5;
                    player2Life -= Diff * 5;
                    AddText(FightText, p1Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p2Name);
                }
            }
            else {
                int EP = player1.GetEP();
                if (EP >= PPDrain2) {
                    player1.SetEP(EP - PPDrain2);
                    player2.SetEP(player2.GetEP() + PPDrain2);
                    AddText(FightText, p2Name + " drained " + PPDrain2 + " EP from " + p1Name);
                }
                else {
                    int Diff = PPDrain2 - EP;
                    player2.SetEP(player2.GetEP() + EP);
                    player1.SetEP(0);
                    player2Life += Diff * 5;
                    player1Life -= Diff * 5;
                    AddText(FightText, p2Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p1Name);
                }
            }
        }
        else if (player1ID == 9 && player2ID == 9) {
            if (PPDrain1 > PPDrain2) {
                AddText(FightText, p1Name + "'s Draining Hands overpowered " + p2Name + "'s");
                int EP = player2.GetEP();
                int PPDrain = PPDrain1 - PPDrain2;
                if (EP >= PPDrain) {
                    player2.SetEP(EP - PPDrain);
                    player1.SetEP(player1.GetEP() + PPDrain);
                    AddText(FightText, p1Name + " drained " + PPDrain + " EP from " + p2Name);
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain - EP;
                    player1.SetEP(player1.GetEP() + EP);
                    player2.SetEP(0);
                    player1Life += Diff * 5;
                    player2Life -= Diff * 5;
                    AddText(FightText, p1Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p2Name);
                }
            }
            else if (PPDrain2 > PPDrain1) {
                AddText(FightText, p2Name + "'s Draining Hands overpowered " + p1Name + "'s");
                int EP = player1.GetEP();
                int PPDrain = PPDrain2 - PPDrain1;
                if (EP >= PPDrain) {
                    player1.SetEP(EP - PPDrain);
                    player2.SetEP(player1.GetEP() + PPDrain);
                    AddText(FightText, p2Name + " drained " + PPDrain + " EP from " + p1Name);
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain - EP;
                    player2.SetEP(player2.GetEP() + EP);
                    player1.SetEP(0);
                    player2Life += Diff * 5;
                    player1Life -= Diff * 5;
                    AddText(FightText, p2Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p1Name);
                }
            }
            else {
                AddText(FightText, "Draining Hands neutralized one another");
            }
        }
        else if (player1ID == 9 && player2Attack[2] == 0 && !player2EscapeFrom[2] && !MateOn2) {
            int EP = player2.GetEP();
            if (EP >= PPDrain1) {
                player2.SetEP(EP - PPDrain1);
                player1.SetEP(player1.GetEP() + PPDrain1);
                AddText(FightText, p1Name + " drained " + PPDrain1 + " EP from " + p2Name);
            }
            //If the opponent doesn't have EPs, we take 10 of his life
            else {
                int Diff = PPDrain1 - EP;
                player1.SetEP(player1.GetEP() + EP);
                player2.SetEP(0);
                player1Life += Diff * 5;
                player2Life -= Diff * 5;
                AddText(FightText, p1Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p2Name);
            }
        }
        else if (player2ID == 9 && player1Attack[2] == 0 && !player1EscapeFrom[2] && !MateOn1) {
            int EP = player1.GetEP();
            if (EP >= PPDrain2) {
                player1.SetEP(EP - PPDrain2);
                player2.SetEP(player2.GetEP() + PPDrain2);
                AddText(FightText, p2Name + " drained " + PPDrain2 + " EP from " + p1Name);
            }
            //If the opponent doesn't have EPs, we take 10 of his life
            else {
                int Diff = PPDrain2 - EP;
                player2.SetEP(player2.GetEP() + EP);
                player1.SetEP(0);
                player2Life += Diff * 5;
                player1Life -= Diff * 5;
                AddText(FightText, p2Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p1Name);
            }
        }
        else if (player1ID == 9 && player2Attack[2] > 0 && MateOn1 && !MateOn2) {
            int Dmg = MatePow1 - player2Attack[2];
            if (Dmg > 0) {
                player2Life -= Dmg;
                AddText(FightText, "Player 1's mate does " + Dmg + " damage to player 2");

                int EP = player2.GetEP();
                if (EP >= PPDrain1) {
                    player2.SetEP(EP - PPDrain1);
                    player1.SetEP(player1.GetEP() + PPDrain1);
                    AddText(FightText, "Player 1 drained " + PPDrain1 + " EP from player2");
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain1 - EP;
                    player1.SetEP(player1.GetEP() + EP);
                    player2.SetEP(0);
                    player1Life += Diff * 5;
                    player2Life -= Diff * 5;
                    AddText(FightText, "Player 1 drained " + Diff * 5 + " HP and " + EP + " EP from player2");
                }
            }
            else if (Dmg < 0) {
                player1Life += Dmg;
                AddText(FightText, "Player 2 does " + -Dmg + " to Player 1");
            }
            else {
                int EP = player2.GetEP();
                if (EP >= PPDrain1) {
                    player2.SetEP(EP - PPDrain1);
                    player1.SetEP(player1.GetEP() + PPDrain1);
                    AddText(FightText, "Player 1 drained " + PPDrain1 + " EP from player2");
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain1 - EP;
                    player1.SetEP(player1.GetEP() + EP);
                    player2.SetEP(0);
                    player1Life += Diff * 5;
                    player2Life -= Diff * 5;
                    AddText(FightText, "Player 1 drained " + Diff * 5 + " HP and " + EP + " EP from player2");
                }
            }
        }
        else if (player2ID == 9 && player1Attack[2] > 0 && MateOn2 && !MateOn1) {
            int Dmg = MatePow2 - player1Attack[2];
            if (Dmg > 0) {
                player1Life -= Dmg;
                AddText(FightText, p2Name + "'s mate does " + Dmg + " damage to " + p1Name);

                int EP = player1.GetEP();
                if (EP >= PPDrain2) {
                    player1.SetEP(EP - PPDrain2);
                    player2.SetEP(player2.GetEP() + PPDrain2);
                    AddText(FightText, p2Name + " drained " + PPDrain2 + " EP from " + p1Name);
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain2 - EP;
                    player2.SetEP(player2.GetEP() + EP);
                    player1.SetEP(0);
                    player2Life += Diff * 5;
                    player1Life -= Diff * 5;
                    AddText(FightText, p2Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p1Name);
                }
            }
            else if (Dmg < 0) {
                player2Life += Dmg;
                AddText(FightText, p1Name + " does " + -Dmg + " to " + p2Name);
            }
            else {
                int EP = player1.GetEP();
                if (EP >= PPDrain2) {
                    player1.SetEP(EP - PPDrain2);
                    player2.SetEP(player2.GetEP() + PPDrain2);
                    AddText(FightText, p2Name + " drained " + PPDrain2 + " EP from " + p1Name);
                }
                //If the opponent doesn't have EPs, we take 10 of his life
                else {
                    int Diff = PPDrain2 - EP;
                    player2.SetEP(player2.GetEP() + EP);
                    player1.SetEP(0);
                    player2Life += Diff * 5;
                    player1Life -= Diff * 5;
                    AddText(FightText, p2Name + " drained " + Diff * 5 + " HP and " + EP + " EP from " + p1Name);
                }
            }
        }
        //For The mirror Defense
        if (player1ID == 11 && player2Attack[2] > 0 && player2ID < 100) {
            int diff = player1Attack[2] - player2Attack[2];

            if (diff > 0)    //happens for ex if we have mate
            {
                player2Life -= diff;
                AddText(FightText, p1Name + " does " + diff + " damage to " + p2Name);
            }
            else if (diff == 0) {
                AddText(FightText, "The attacks got stopped!");
            }
            else {
                player2Attack[2] = -diff;
                if (player2ID != 3 && player2ID != 17 && player2ID != 21) {
                    if (MateOn2) {
                        player2.Mate.Anim.SetInteger("ID", 3);
                        MateLP2 -= player2Attack[2];
                        AddText(FightText, p2Name + " does " + player2OverallAttack + " damage to his mate due to the portal");
                        if (MateLP2 < 0) { player2Life += MateLP2; AddText(FightText, p2Name + " does " + -MateLP2 + " damage to himself due to the portal"); MateLP2 = 0; }

                    }
                    else if (!player2EscapeFrom[2]) {
                        player2Life -= player2Attack[2];
                        AddText(FightText, p2Name + " does " + player2OverallAttack + " damage to himself due to the portal");
                    }
                }
            }
            //Note: this doesn't deal with multi directional attacks
        }
        else if (player2ID == 11 && player1Attack[2] > 0 && player1ID < 100) {
            int diff = player2Attack[2] - player1Attack[2];

            if (diff > 0)    //happens for ex if we have mate
            {
                player1Life -= diff;
                AddText(FightText, p2Name + " does " + diff + " damage to " + p1Name);
            }
            else if (diff == 0) {
                AddText(FightText, "The attacks got stopped!");
            }
            else {
                player1Attack[2] = -diff;
                if (player1ID != 3 && player1ID != 17 && player1ID != 21) {
                    if (MateOn1) {
                        player1.Mate.Anim.SetInteger("ID", 3);
                        MateLP1 -= player1Attack[2];
                        AddText(FightText, p1Name + " does " + player1OverallAttack + " damage to his mate due to the portal");
                        if (MateLP1 < 0) { player1Life += MateLP1; AddText(FightText, p1Name + " does " + -MateLP1 + " damage to himself due to the portal"); MateLP1 = 0; }

                    }
                    else if (!player1EscapeFrom[2]) {
                        player1Life -= player1Attack[2];
                        AddText(FightText, p1Name + " does " + player1OverallAttack + " damage to himself due to the portal");
                    }
                }
            }
        }
        else if (player1ID == 19 && player2ID == 6) {
            if (player1Attack[1] > player2Attack[1]) {
                player2Life -= (player1Attack[1] - player2Attack[1]);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(); }
            }
            else if (player1Attack[1] < player2Attack[1]) {
                player1Life -= (player2Attack[1] - player1Attack[1]);
                animatorPlayer1.SetBool("Sealed", true);
                animatorPlayer2.SetBool("FountainStop", true);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(true); }
            }
            else if (player1Attack == player2Attack) {
                animatorPlayer1.SetBool("Sealed", true);
                animatorPlayer2.SetBool("Fountained", true);
                animatorPlayer2.SetBool("FountainStop", true);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(true); }
            }
        }
        else if (player2ID == 19 && player1ID == 6) {
            if (player2Attack[1] > player1Attack[1]) {
                player1Life -= (player2Attack[1] - player1Attack[1]);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(); }
            }
            else if (player2Attack[1] < player1Attack[1]) {
                player2Life -= (player1Attack[1] - player2Attack[1]);
                animatorPlayer2.SetBool("Sealed", true);
                animatorPlayer1.SetBool("FountainStop", true);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(true); }
            }
            else if (player2Attack == player1Attack) {
                animatorPlayer2.SetBool("Sealed", true);
                animatorPlayer1.SetBool("Fountained", true);
                animatorPlayer1.SetBool("FountainStop", true);
                foreach (FallInPieces c in Rubbles) { c.SpecialFall(true); }
            }
        }
        //Case of Special Abilities Escapable by Air.
        else if (player1ID > 200 && player2ID < 101 && !(player2EscapeFrom[1] && player2EscapeFrom[2])) {
            //For the unstoppable abilities in the layers
            StopMate2 = true;

            animatorPlayer2.SetInteger("ID", -1);

            animatorPlayer2.SetBool("Scared", true);
            player2Life -= player1Attack[2];
            AddText(FightText, p1Name + " dazzled " + p2Name + " with his magnificient attack and dealt " + player1Attack[2] + " damage to him");
        }
        else if (player2ID > 200 && player1ID < 101 && !(player1EscapeFrom[1] && player1EscapeFrom[2])) {
            StopMate1 = true;

            animatorPlayer1.SetInteger("ID", -1);

            animatorPlayer1.SetBool("Scared", true);
            player1Life -= player2Attack[2];
            AddText(FightText, p2Name + " Dazzled " + p1Name + " with his magnificient attack and dealt " + player2Attack[2] + " damage to him");
        }
        //Only case of escape from above
        else if (player1ID > 200 && player2ID < 101 && (player2EscapeFrom[1] && player2EscapeFrom[2])) {
            StopMate2 = true;
            AddText(FightText, p2Name + " somehow escaped!");
        }
        else if (player1ID < 101 && player2ID > 200 && (player1EscapeFrom[1] && player1EscapeFrom[2])) {
            StopMate1 = true;
            AddText(FightText, p1Name + " somehow escaped!");
        }
        //Case of both Special Abilties.
        else if (player1ID > 100 && player2ID > 100) {
            StopMate1 = StopMate2 = true;
            animatorPlayer1.SetTrigger("Scared2");
            animatorPlayer2.SetTrigger("Scared2");
            AddText(FightText, "Both Players were amazed by their opponent's attack's mightiness and therefore couldn't follow up through with their own");
        }
        //Case of Special Abilities Escapable from Below.
        else if ((player1ID > 100 && player1ID < 200) && player2ID < 101 && !(player2EscapeFrom[0] && player2EscapeFrom[2])) {
            //For the unstoppable abilities in the layers
            StopMate2 = true;
            animatorPlayer2.SetInteger("ID", -1);
            animatorPlayer2.SetBool("Scared", true);
            player2Life -= player1Attack[2];
            AddText(FightText, p1Name + " dazzled " + p2Name + " with their magnificient attack and dealt " + player1Attack[2] + " damage to them");
        }
        else if ((player2ID > 100 && player2ID < 200) && player1ID < 101 && !(player1EscapeFrom[0] && player1EscapeFrom[2])) {
            StopMate1 = true;
            animatorPlayer1.SetInteger("ID", -1);
            animatorPlayer1.SetBool("Scared", true);
            player1Life -= player2Attack[2];
            AddText(FightText, p2Name + " Dazzled " + p1Name + " with their magnificient attack and dealt " + player2Attack[2] + " damage to them");
        }
        //Only case of escape from below
        else if ((player1ID > 100 && player1ID < 200) && player2ID < 101 && (player2EscapeFrom[0] && player2EscapeFrom[2])) {
            StopMate2 = true;
            animatorPlayer2.SetTrigger("Scared1");
            AddText(FightText, p2Name + " somehow escaped!");
        }
        else if (player1ID < 101 && (player2ID > 100 && player2ID < 200) && (player1EscapeFrom[0] && player1EscapeFrom[2])) {
            StopMate1 = true;
            animatorPlayer1.SetTrigger("Scared1");
            AddText(FightText, p1Name + " somehow escaped!");
        }
        else {
            int damageDoneTo2 = 0; int damageDoneTo1 = 0;

            for (int i = 0; i < 3; i++) {
                int diff = Math.Min(player1Attack[i], player2Attack[i]);
                player1Attack[i] -= diff; player2Attack[i] -= diff;
                player1OverallAttack -= diff; player2OverallAttack -= diff;

                int defended = Math.Min(player1Attack[i], player2Defense[i]);
                player1Attack[i] -= defended; player1OverallAttack -= defended;
                defended = Math.Min(player2Attack[i], player1Defense[i]);
                player2Attack[i] -= defended; player2OverallAttack -= defended;

                damageDoneTo2 += player1Attack[i];
                damageDoneTo1 += player2Attack[i];
            }

            /*
                        print("Player 1: [Escape Count, " + player1EscapeCount + "], [TriedToAttack, " + player1TriedToAtck + "], " +
                "[OverallAttack " + player1OverallAttack + "]");
                        print("Player 2: [Escape Count, " + player2EscapeCount + "], [TriedToAttack, " + player2TriedToAtck + "], " +
                 "[OverallAttack " + player2OverallAttack + "]");
                 */

            player2Life -= damageDoneTo2;
            player1Life -= damageDoneTo1;


            if (!player1TriedToAtck && !player2TriedToAtck && damageDoneTo1 == 0 && damageDoneTo2 == 0)
                AddText(FightText, "No Attacks Recorded");
            else if (player1TriedToAtck && player2TriedToAtck && player1OverallAttack == 0 && player2OverallAttack == 0 && sameEscape)
                AddText(FightText, "Attacks were neutralized");
            else {
                if (player1TriedToAtck && damageDoneTo2 > 0)
                    AddText(FightText, p1Name + " does " + damageDoneTo2 + " damage to " + p2Name);
                else if (damageDoneTo2 > 0 && MateOn1)
                    AddText(FightText, "The mate of " + p1Name + " does " + damageDoneTo2 + " damage to " + p2Name);

                if (player2OverallAttack == 0 && player2TriedToAtck && player1EscapeFromAtck)
                    AddText(FightText, p1Name + " escaped!");
                if (player2OverallAttack == 0 && player2TriedToAtck && !player1EscapeFromAtck && damageDoneTo1 == 0)
                    AddText(FightText, p1Name + " defended well.");

                if (player2TriedToAtck && damageDoneTo1 > 0)
                    AddText(FightText, p2Name + " does " + damageDoneTo1 + " damage to " + p1Name);
                else if (damageDoneTo1 > 0 && MateOn2)
                    AddText(FightText, "The mate of " + p2Name + " does " + damageDoneTo1 + " damage to " + p1Name);

                if (player1OverallAttack == 0 && player1TriedToAtck && player2EscapeFromAtck)
                    AddText(FightText, p2Name + " escaped!");
                if (player1OverallAttack == 0 && player1TriedToAtck && !player2EscapeFromAtck && damageDoneTo2 == 0)
                    AddText(FightText, p2Name + " defended well.");
            }
        }

        //Aerial Strike with Eye Of Horus
        if (player1ID == 10 && player2ID == 13)
            animatorPlayer2.SetTrigger("SpecialEye");
        else if (player1ID == 13 && player2ID == 10)
            animatorPlayer1.SetTrigger("SpecialEye");
        //Double Eye of Horus
        else if (player1ID == 13 && player2ID == 13) {
            animatorPlayer1.SetTrigger("SpecialEye");
            animatorPlayer2.SetTrigger("SpecialEye");
        }

        player1.transform.Find("Design").GetComponent<PlayerCollision>().updateState();
        player2.transform.Find("Design").GetComponent<PlayerCollision>().updateState();

        netDamageTakenP1 = player1.GetLife() - player1Life;
        netDamageTakenP2 = player2.GetLife() - player2Life;
        if (netDamageTakenP1 > 0 && protectiveEarthEffectP1) {
            if (netDamageTakenP1 >= HardeningPow1) {
                netDamageTakenP1 -= HardeningPow1;
                player1Life += HardeningPow1;
                protectiveEarthEffectP1 = false;
                HardeningPow1 = 0;
                player1.GetComponent<Cube_Player>().ShieldDisappearance.SetActive(true);
                AddText(FightText, "However, " + p1Name + "'s Hardening absorbed some of the Damage");
            }
            else {
                player1Life += netDamageTakenP1;
                HardeningPow1 -= netDamageTakenP1;
                netDamageTakenP1 = 0;
                player1.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                AddText(FightText, "However, " + p1Name + "'s Hardening absorbed all of the Damage");
            }

        }
        if (netDamageTakenP2 > 0 && protectiveEarthEffectP2) {
            if (netDamageTakenP2 >= HardeningPow2) {
                netDamageTakenP2 -= HardeningPow2;
                player2Life += HardeningPow2;
                protectiveEarthEffectP2 = false;
                HardeningPow2 = 0;
                player2.GetComponent<Cube_Player>().ShieldDisappearance.SetActive(true);
                AddText(FightText, "However, " + p2Name + "'s Hardening absorbed some of the Damage");
            }
            else {
                player2Life += netDamageTakenP2;
                HardeningPow2 -= netDamageTakenP2;
                netDamageTakenP2 = 0;
                player2.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                AddText(FightText, "However, " + p2Name + "'s Hardening absorbed all of the Damage");
            }
        }

        if (netDamageTakenP1 > 0 && player2ID < 100)
            StartCoroutine(HitFace(1, round));
        if (netDamageTakenP2 > 0 && player1ID < 100)
            StartCoroutine(HitFace(2, round));

        if (netDamageTakenP1 > 0 && DoubleEdgeDef1 > 0 && !DoitDoubleEdge1) {
            netDamageTakenP1 += DoubleEdgeDef1;
            player1Life -= DoubleEdgeDef1;
            AddText(FightText, p1Name + " takes " + DoubleEdgeDef1 + " additional damage due to the swords.");
            DoubleEdgeDef1 = 0;
        }
        if (netDamageTakenP2 > 0 && DoubleEdgeDef2 > 0 && !DoitDoubleEdge2) {
            netDamageTakenP2 += DoubleEdgeDef2;
            player2Life -= DoubleEdgeDef2;
            AddText(FightText, p2Name + " takes " + DoubleEdgeDef2 + " additional damage due to the swords.");
            DoubleEdgeDef2 = 0;
        }

        if (player1ID == -1 && player2ID == -1)
            yield return new WaitForSeconds(0.75f);
        else
            yield return new WaitForSeconds(2.5f);

        if (MateOn1) player1.Mate.SetLife(MateLP1);
        if (MateOn2) player2.Mate.SetLife(MateLP2);

        //Setting the smoke false
        if (player1EscapeCount == 3 && player1 is Sphere_Player)
            player1.GetComponent<Sphere_Player>().Smoke.SetActive(false);
        if (player2EscapeCount == 3 && player2 is Sphere_Player)
            player2.GetComponent<Sphere_Player>().Smoke.SetActive(false);

        //Setting the swords false
        if (DoubleEdge1 && !DoitDoubleEdge1) {
            player1.DoubleEdgedSword.SetActive(false);
            DoubleEdge1 = false;
        }
        if (DoubleEdge2 && !DoitDoubleEdge2) {
            player2.DoubleEdgedSword.SetActive(false);
            DoubleEdge2 = false;
        }
        if (HealCount1 == 3) {
            HealCount1 = Heal1 = 0;
            Healing1 = false;
            player1.transform.Find("HealingCircle").gameObject.SetActive(false);
        }
        if (HealCount2 == 3) {
            HealCount2 = Heal2 = 0;
            Healing2 = false;
            player2.transform.Find("HealingCircle").gameObject.SetActive(false);
        }

        if (MateOn1 && MateLP1 == 0) {
            MateOn1 = false;
            MateLife1.SetActive(false);
            if (this is GameMasterOffline) { MateLife12.SetActive(false); }
            First1 = true;
            AddText(FightText, "The mate of " + p1Name + " is dead");
        }
        if (MateOn2 && MateLP2 == 0) {
            MateOn2 = false;
            MateLife2.SetActive(false);
            if (this is GameMasterOffline) { MateLife22.SetActive(false); }
            First2 = true;
            AddText(FightText, "The mate of " + p2Name + " is dead");
        }

        if (player1selfdmg > 0) {
            player1Life -= player1selfdmg;
            AddText(FightText, p1Name + " inflicted " + player1selfdmg + " damage to himself with his attack");
        }
        if (player2selfdmg > 0) {
            player2Life -= player2selfdmg;
            AddText(FightText, p2Name + " inflicted " + player2selfdmg + " damage to himself with his attack");
        }

        player1.SetLife(player1Life);
        player2.SetLife(player2Life);

        //Passives and end are done separatly
    }
    public virtual IEnumerator Choice() {
        yield return new WaitForSeconds(0.1f);
    }

    //Turn off passives, mates, boosts...
    public void turnOffPassives(bool p) {
        if (p) {
            animatorPlayer1.SetInteger("ID", -1);
            animatorPlayer1.SetBool("Sealed", false);
            animatorPlayer1.SetBool("Fountained", false);
            animatorPlayer1.SetBool("FountainStop", false);
            animatorPlayer1.SetBool("Hit", false);
            animatorPlayer1.SetBool("Scared", false);
            animatorPlayer1.SetBool("OnFire", false);
            animatorPlayer1.SetBool("ElevationStop", false);
            animatorPlayer1.SetBool("AttBelow", false);
            player1.setAdditionalDamage(0);

            if (player1 is Star_Player || player1 is Pyramid_Player)
                player1.GetComponent<Animator>().SetBool("Portal", false);

            player1.GetComponent<Animator>().SetBool("Scared1", false);

            if (MateOn1) {
                player1.Mate.Anim.SetInteger("ID", -1);
                player1.Mate.SetLife(0);
                MateLife1.SetActive(false);
                MateOn1 = false;
            }
            StopMate1 = false;
            if (player1 is Cube_Player) {
                protectiveEarthEffectP1 = false;
                player1.GetComponent<Cube_Player>().ShieldAppearance.SetActive(false);
            }
            else if (player1 is Pyramid_Player) {
                Snowing1 = false;
                player1.GetComponent<Pyramid_Player>().Snow.SetActive(false);
            }
            else if (player1 is Star_Player) {
                player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
            }
            else if (player1 is Sphere_Player) {
                HelperSpheres1 = 0; DoitSmoke1 = false;
                player1.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", -1);
                player1.GetComponent<Sphere_Player>().HelperSpheres.SetActive(false);
                player1.GetComponent<Sphere_Player>().Smoke.SetActive(false);
            }

            StartRoundCountP1 = 0;
            if (StartRoundCountCurseP1 != 0) {
                StartRoundCountCurseP1 = 0;
                player2.transform.Find("StormCloud").gameObject.SetActive(false);
            }
            if (StartRoundCountBurn1 != 0) {
                StartRoundCountBurn1 = 0;
                player1.transform.Find("BurnedFlame").gameObject.SetActive(false);
            }
            if (StartRoundCountFreeze1 != 0) {
                StartRoundCountFreeze1 = 0;
                animatorPlayer1.SetBool("Frozen", false);
            }
            if (StartRoundCountStuckInPlace1 != 0) {
                StartRoundCountStuckInPlace1 = 0;
                player1.transform.Find("GroundStuck").gameObject.SetActive(false);
            }
            if (HealCount1 != 0) {
                HealCount1 = Heal1 = 0;
                Healing1 = false;
                player1.transform.Find("HealingCircle").gameObject.SetActive(false);
            }
        }
        else {
            animatorPlayer2.SetInteger("ID", -1);
            animatorPlayer2.SetBool("Sealed", false);
            animatorPlayer2.SetBool("Fountained", false);
            animatorPlayer2.SetBool("FountainStop", false);
            animatorPlayer2.SetBool("Hit", false);
            animatorPlayer2.SetBool("Scared", false);
            animatorPlayer2.SetBool("OnFire", false);
            animatorPlayer2.SetBool("ElevationStop", false);
            animatorPlayer2.SetBool("AttBelow", false);
            player2.setAdditionalDamage(0);

            if (player2 is Star_Player || player2 is Pyramid_Player)
                player2.GetComponent<Animator>().SetBool("Portal", false);

            if (MateOn2) {
                player2.Mate.Anim.SetInteger("ID", -1);
                player2.Mate.SetLife(0);
                MateLife2.SetActive(false);
                MateOn2 = false;
            }
            StopMate2 = false;
            if (player2 is Cube_Player) {
                protectiveEarthEffectP2 = false;
                player2.GetComponent<Cube_Player>().ShieldAppearance.SetActive(false);
            }
            else if (player2 is Pyramid_Player) {
                Snowing2 = false;
                player2.GetComponent<Pyramid_Player>().Snow.SetActive(false);
            }
            else if (player2 is Star_Player) {
                player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
            }
            else if (player2 is Sphere_Player) {
                HelperSpheres2 = 0; DoitSmoke2 = false;
                player2.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", -1);
                player2.GetComponent<Sphere_Player>().HelperSpheres.SetActive(false);
                player2.GetComponent<Sphere_Player>().Smoke.SetActive(false);
            }

            StartRoundCountP2 = 0;
            if (StartRoundCountCurseP2 != 0) {
                StartRoundCountCurseP2 = 0;
                player1.transform.Find("StormCloud").gameObject.SetActive(false);
            }
            if (StartRoundCountBurn2 != 0) {
                StartRoundCountBurn2 = 0;
                player2.transform.Find("BurnedFlame").gameObject.SetActive(false);
            }
            if (StartRoundCountFreeze2 != 0) {
                StartRoundCountFreeze2 = 0;
                animatorPlayer2.SetBool("Frozen", false);
            }
            if (StartRoundCountStuckInPlace2 != 0) {
                StartRoundCountStuckInPlace2 = 0;
                player2.transform.Find("GroundStuck").gameObject.SetActive(false);
            }
            if (HealCount2 != 0) {
                HealCount2 = Heal2 = 0;
                Healing2 = false;
                player2.transform.Find("HealingCircle").gameObject.SetActive(false);
            }
        }
    }

    protected virtual void ResetRound(bool Start = false) {
        updateLifeBars();

        foreach (GameObject G in ObjectsToDestroy) {
            if (G != null && G.name != "AboveBullet")
                Destroy(G);
        }
        ObjectsToDestroy.Clear();

        FightText.text = "";
        FightCanvas.SetActive(false);
        canvasPlayer1.gameObject.SetActive(true);

        if (player1ID == 54)
            player1.transform.Find("Design").gameObject.layer = 10;
        if (player2ID == 54)
            player2.transform.Find("Design").gameObject.layer = 11;

        HitFace1 = false;
        HitFace2 = false;
        if (HelperSpheres1 == 2) {
            player1.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", -1);
            player1.GetComponent<Sphere_Player>().HelperSpheres.SetActive(false);
        }
        if (HelperSpheres2 == 2) {
            player2.GetComponent<Sphere_Player>().HelperSpheres.GetComponent<Animator>().SetInteger("ID", -1);
            player2.GetComponent<Sphere_Player>().HelperSpheres.SetActive(false);
        }
        if (MateOn1) {
            player1.Mate.Anim.SetInteger("ID", -1);
            player1.Mate.ResetBulletPow();
        }
        if (MateOn2) {
            player2.Mate.Anim.SetInteger("ID", -1);
            player2.Mate.ResetBulletPow();
        }
        if (player1 is Sphere_Player) {
            ((Sphere_Player)player1).ToxicRing.SetActive(true);
        }
        if (player2 is Sphere_Player) {
            ((Sphere_Player)player2).ToxicRing.SetActive(true);
        }
        DoubleEdgeAtt1 = DoubleEdgeAtt2 = DoubleEdgeDef1 = DoubleEdgeDef2 = 0;
        StopMate1 = false;
        StopMate2 = false;
        player1.GetComponent<Animator>().SetBool("Sealed", false);
        player2.GetComponent<Animator>().SetBool("Sealed", false);
        player1.GetComponent<Animator>().SetBool("Fountained", false);
        player2.GetComponent<Animator>().SetBool("Fountained", false);
        player1.GetComponent<Animator>().SetBool("FountainStop", false);
        player2.GetComponent<Animator>().SetBool("FountainStop", false);
        player1.GetComponent<Animator>().SetBool("Hit", false);
        player2.GetComponent<Animator>().SetBool("Hit", false);
        player1.GetComponent<Animator>().SetBool("Scared", false);
        player2.GetComponent<Animator>().SetBool("Scared", false);
        player1.GetComponent<Animator>().SetBool("OnFire", false);
        player2.GetComponent<Animator>().SetBool("OnFire", false);
        animatorPlayer1.SetBool("ElevationStop", false);
        animatorPlayer2.SetBool("ElevationStop", false);
        animatorPlayer1.SetBool("AttBelow", false);
        animatorPlayer2.SetBool("AttBelow", false);
        if (player1 is Star_Player || player1 is Pyramid_Player)
            player1.GetComponent<Animator>().SetBool("Portal", false);
        if (player2 is Star_Player || player2 is Pyramid_Player)
            player2.GetComponent<Animator>().SetBool("Portal", false);

        if (shapeID1 == 2) {
            player1.GetComponent<Animator>().SetBool("Player1", false);
        }
        PlayerCollision.isfirsttime = true;
        PlayerCollision.firstime = true;
        PlayerCollision.firstTimeWaterP1 = true;
        PlayerCollision.firstTimeWaterP2 = true;
        if (!((player2ID > 100 && player2ID < 200) && player1ID == 4))
            player1.GetComponentInChildren<PlayerCollision>().LastCol = null;
        if (!((player1ID > 100 && player1ID < 200) && player2ID == 4))
            player2.GetComponentInChildren<PlayerCollision>().LastCol = null;
        player1.GetComponent<Animator>().SetBool("Scared1", false);
        player1.EarthquakeOngoing = false;
        player2.EarthquakeOngoing = false;

        //Get back to idle
        player1.ProbPlusAtt = player1.ProbPlusDef = player2.ProbPlusAtt = player2.ProbPlusDef = 0;
        player1.SetIdOfAnimUSed(-1);
        player1.GetComponent<Animator>().SetInteger("ID", -1);
        player2.SetIdOfAnimUSed(-1);
        player2.GetComponent<Animator>().SetInteger("ID", -1);

        player1ChoiceDone = false;
        player1.SetAttackPower(new int[] { 0, 0, 0 });
        player1.SetShieldPower(new int[] { 0, 0, 0 });
        player1.setEscapeFrom(new bool[] { false, false, false });
        player1.SetSelfdmg(0);
        player1.SetBulletPower(0);
        player1.SetEPToRemove(0);
        player1.SetChoiceDone(false);

        player2ChoiceDone = false;
        player2.SetAttackPower(new int[] { 0, 0, 0 });
        player2.SetShieldPower(new int[] { 0, 0, 0 });
        player2.setEscapeFrom(new bool[] { false, false, false });
        player2.SetSelfdmg(0);
        player2.SetBulletPower(0);
        player2.SetEPToRemove(0);
        player2.SetChoiceDone(false);

        call = false;

        cameraFight.fieldOfView = 65;
        if (Replay) {
            StartCoroutine(Choice());
        }
        if (!Spectate && !Replay) {
            if (cameraState1 == 1) {
                cameraFight.gameObject.SetActive(false);
                cameraPlayer1.gameObject.SetActive(true);
            }
            else {
                cameraFight.gameObject.SetActive(true);
                cameraPlayer1.gameObject.SetActive(false);
            }

            canvasPlayer1.gameObject.SetActive(true);
            if (background == 1) {
                Transform temp = Backgrounds[1].transform.Find("UP");
                temp.Find("Front").gameObject.SetActive(cameraState1 == 0);
                temp.Find("FrontSpare").gameObject.SetActive(false);
                temp.Find("P1").gameObject.SetActive(cameraState1 == 1);
            }
        }


        //When devil's deal's effect should be over
        if (round - StartRoundCountP1 == 3)
            StartRoundCountP1 = 0;
        if (round - StartRoundCountP2 == 3)
            StartRoundCountP2 = 0;

        //For devil's deal since the end of this round = start of the next
        if (StartRoundCountP1 != 0) {
            player1.SetEP(player1.GetEP() + 1);
            //Add anim?
        }
        if (StartRoundCountP2 != 0) {
            player2.SetEP(player2.GetEP() + 1);
        }

        //For curse of the cloud to last 3 rounds
        if (StartRoundCountCurseP1 != 0) {
            player1.SetLife(player1.GetLife() - 5);

            if (round - StartRoundCountCurseP1 == 2) {
                StartRoundCountCurseP1 = 0;
                player2.transform.Find("StormCloud").gameObject.SetActive(false);
            }
        }
        if (StartRoundCountCurseP2 != 0) {
            player2.SetLife(player2.GetLife() - 5);

            if (round - StartRoundCountCurseP2 == 2) {
                StartRoundCountCurseP2 = 0;
                player1.transform.Find("StormCloud").gameObject.SetActive(false);
            }
        }

        //Disable/Enable the buttons that the player can/can't use due to available PP at the end of each round
        if (!Start) {
            int EPToAdd = 0;
            //Here, we still have the round number of the last round (the one which just ended)
            if (round < 4)
                EPToAdd = 1;
            else if (round < 9) {
                EPToAdd = 2;
                if (round == 4) {
                    roundDisplay1.transform.Find("X2").gameObject.SetActive(true);
                    if (!(Replay || Spectate))
                        roundDisplay2.transform.Find("X2").gameObject.SetActive(true);
                }
            }
            else if (round < 14) {
                EPToAdd = 3;
                if (round == 9) {
                    roundDisplay1.transform.Find("X2").GetComponent<Text>().text = "x3";
                    if (!(Replay || Spectate))
                        roundDisplay2.transform.Find("X2").GetComponent<Text>().text = "x3";
                }
            }
            else {
                EPToAdd = 4;
                if (round == 14) {
                    roundDisplay1.transform.Find("X2").GetComponent<Text>().text = "x4";
                    if (!(Replay || Spectate))
                        roundDisplay2.transform.Find("X2").GetComponent<Text>().text = "x4";
                }
            }

            roundDisplay1.transform.Find("Text").GetComponent<Text>().text = (round + 1).ToString();
            if (!(Replay || Spectate))
                roundDisplay2.transform.Find("Text").GetComponent<Text>().text = (round + 1).ToString();

            player1.SetEP(player1.GetEP() + EPToAdd);
            player2.SetEP(player2.GetEP() + EPToAdd);

            if (!Spectate && !Replay) {
                for (int k = 0; k < 6; k++) {
                    Shape_Abilities cur = canvasPlayer1.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                    if (cur == null) { continue; }  //If we have < 6 abilities chosen
                    cur.Disabled(); cur.updateState();
                }
                if (canvasPlayer1.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>())
                    canvasPlayer1.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>().Disabled();
                canvasPlayer1.transform.Find("Attack").GetComponentInChildren<Shape_Abilities>().Disabled();
            }

            //P2 done in offline only

            //PASSIVES

            //protectiveEarthEffectP1 = false;
            //protectiveEarthEffectP2 = false;

            //For the Burned Effect; effective 2 rounds
            if (StartRoundCountBurn1 != 0) {
                player1.SetLife(player1.GetLife() - PassiveStats[4][TempOpponent.Opponent.Passives[4] - 1]);
                AddText(FightText, p1Name + " lost " + PassiveStats[4][TempOpponent.Opponent.Passives[4] - 1] + " HP due to being Burned");

                if (round - StartRoundCountBurn1 == 1) {
                    StartRoundCountBurn1 = 0;
                    player1.transform.Find("BurnedFlame").gameObject.SetActive(false);
                    AddText(FightText, p1Name + " is no longer burned");
                }
            }
            if (StartRoundCountBurn2 != 0) {
                player2.SetLife(player2.GetLife() - PassiveStats[4][PassivesArray[4] - 1]);
                AddText(FightText, p2Name + " lost " + PassiveStats[4][PassivesArray[4] - 1] + " HP due to being Burned");

                if (round - StartRoundCountBurn2 == 1) {
                    StartRoundCountBurn2 = 0;
                    player2.transform.Find("BurnedFlame").gameObject.SetActive(false);
                    AddText(FightText, p2Name + " is no longer burned");
                }
            }

            //For the Freeze Effect; effective 1 round
            if (StartRoundCountFreeze1 != 0) {
                if (round - StartRoundCountFreeze1 == PassiveStats[2][TempOpponent.Opponent.Passives[2] - 1]) {
                    StartRoundCountFreeze1 = 0;
                    animatorPlayer1.SetBool("Frozen", false);
                    AddText(FightText, p1Name + " is no longer frozen");
                }
                else {
                    for (int k = 0; k < 6; k++) {
                        if (!Replay && !Spectate) {
                            Shape_Abilities cur = canvasPlayer1.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                            if (cur == null) { continue; }
                            cur.DisableAll();
                        }
                    }
                    if (!Replay && !Spectate)
                        canvasPlayer1.transform.Find("Attack").GetComponentInChildren<Shape_Abilities>().DisableAll();
                }
            }
            if (StartRoundCountFreeze2 != 0) {
                if (round - StartRoundCountFreeze2 == PassiveStats[2][PassivesArray[2] - 1]) {
                    StartRoundCountFreeze2 = 0;
                    animatorPlayer2.SetBool("Frozen", false);
                    AddText(FightText, p2Name + " is no longer frozen");
                }
                //else for p2 only
            }

            //For the stuck in place effect; effective one round
            if (StartRoundCountStuckInPlace1 != 0) {
                if (round - StartRoundCountStuckInPlace1 == PassiveStats[0][TempOpponent.Opponent.Passives[0] - 1]) {
                    StartRoundCountStuckInPlace1 = 0;
                    player1.transform.Find("GroundStuck").gameObject.SetActive(false);
                    AddText(FightText, p1Name + " is no longer stuck in place");
                }
                else {
                    for (int k = 0; k < 6; k++) {
                        if (!Replay && !Spectate) {
                            Shape_Abilities cur = canvasPlayer1.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                            if (cur == null) { continue; }
                            cur.DisableEscapes();
                        }
                    }
                }
            }
            if (StartRoundCountStuckInPlace2 != 0) {
                if (round - StartRoundCountStuckInPlace2 == PassiveStats[0][PassivesArray[0] - 1]) {
                    StartRoundCountStuckInPlace2 = 0;
                    player2.transform.Find("GroundStuck").gameObject.SetActive(false);
                    AddText(FightText, p2Name + " is no longer stuck in place");
                }
                //else for p2 only
            }

            //P2 done in offline only
            //round++ in offline/online too (after dealing with P2)
        }

        canvasPlayer1.transform.Find("EnergyPoints").GetChild(shapeID1).GetComponentInChildren<Show_EP>().UpdateEP();
        if (this is GameMasterOffline && PlayerPrefs.GetInt("BotOnline") == 1) {
            string[] botRec = new string[] { player1.GetLife().ToString(), player2.GetLife().ToString(), round.ToString(), String.Join("/", ChoicesP1), String.Join("/", ChoicesP2), String.Join("/", ProbsP1), String.Join("/", ProbsP2), TempOpponent.Opponent.Username, shapeID2.ToString(), String.Join("|", TempOpponent.Opponent.AbLevelArray), TempOpponent.Opponent.OpPP.ToString() };
            ClientTCP.PACKAGE_DEBUG("BotReconnectString", ReconnectString() + "," + String.Join(",", botRec), TempOpponent.Opponent.BotRoomNum);
        }
    }
    public void SetAnim(int Anim) {
        if (Anim == 1)
            Invoke("SetIt1", 1.5f);
        else
            Invoke("SetIt2", 1.5f);
    }
    private void SetIt() {
        animatorPlayer1.SetInteger("EID", -1);
    }
    private void SetIt2() {
        animatorPlayer2.SetInteger("EID", -1);
    }

    public void updateLifeBars() {
        for (int i = 0; i < LifeBars.Length; i++) {
            if (LifeBars[i] == null || LifeBars[i].enabled == false) { continue; }
            LifeBars[i].updateState();
        }
    }

    public IEnumerator HitFace(int Player, int Round) {
        if (Player == 1) {
            yield return new WaitUntil(() => (HitFace1));
            if (Round == round)
                animatorPlayer1.SetBool("Hit", true);
            else
                HitFace1 = false;
        }
        else {
            yield return new WaitUntil(() => (HitFace2));
            if (Round == round)
                animatorPlayer2.SetBool("Hit", true);
            else
                HitFace2 = false;
        }
    }
    //If damage is 1, damage was taken otherwise health was taken
    public void ShowDamage(int player, int value, bool damage) {
        canvasPlayer1.transform.Find("P" + player.ToString() + "Damage").gameObject.SetActive(true);
        Text t = canvasPlayer1.transform.Find("P" + player.ToString() + "Damage").GetComponent<Text>();
        t.color = damage ? new Color(1f, 0f, 0f) : new Color(0f, 1f, 0f);
        t.text = value.ToString();

        if (!GameMaster.Replay && !GameMaster.Spectate) {
            t = canvasPlayer2.transform.Find("P" + player.ToString() + "Damage").GetComponent<Text>();
            t.color = damage ? new Color(1f, 0f, 0f) : new Color(0f, 1f, 0f);
            t.text = value.ToString();

        }
        Invoke("HideDamage", 1f);
    }

    public void HideDamage() {
        canvasPlayer1.transform.Find("P1Damage").gameObject.SetActive(false);
        canvasPlayer1.transform.Find("P2Damage").gameObject.SetActive(false);
        if (GameMaster.Replay || GameMaster.Spectate)
            return;
        canvasPlayer2.transform.Find("P1Damage").gameObject.SetActive(false);
        canvasPlayer2.transform.Find("P2Damage").gameObject.SetActive(false);
    }

    public void ReadReconnectString(string ReconnectString, bool Spectate = false) {
        string[] S = ReconnectString.Split(',');
        int[] Ar = new int[S.Length];
        int i = 0;
        foreach (string s in S) {
            System.Int32.TryParse(s, out Ar[i]);
            i++;
        }
        if (!Spectate) {
            #region Setting Stuff
            StartRoundCountCurseP2 = Ar[0];
            StartRoundCountCurseP1 = Ar[1];
            StartRoundCountP2 = Ar[2];
            StartRoundCountP1 = Ar[3];
            StartRoundCountBurn2 = Ar[4];
            StartRoundCountBurn1 = Ar[5];
            StartRoundCountFreeze2 = Ar[6];
            StartRoundCountFreeze1 = Ar[7];
            StartRoundCountStuckInPlace2 = Ar[8];
            StartRoundCountStuckInPlace1 = Ar[9];
            protectiveEarthEffectP2 = Ar[10] == 1;
            protectiveEarthEffectP1 = Ar[11] == 1;
            MateOn2 = Ar[12] == 1;
            MateOn1 = Ar[13] == 1;
            MatePow2 = Ar[14];
            MatePow1 = Ar[15];
            MateLP2 = Ar[16];
            MateLP1 = Ar[17];
            AssistCube2 = Ar[18] == 1;
            AssistCube1 = Ar[19] == 1;
            FireEnergy2 = Ar[20] == 1;
            FireEnergy1 = Ar[21] == 1;
            BluePlanet2 = Ar[22] == 1;
            BluePlanet1 = Ar[23] == 1;
            Doit2 = Ar[24] == 1;
            Doit1 = Ar[25] == 1;
            AssistPow2 = Ar[26];
            AssistPow1 = Ar[27];
            DoitFire2 = Ar[28] == 1;
            DoitFire1 = Ar[29] == 1;
            FirePow2 = Ar[30];
            FirePow1 = Ar[31];
            DoitWater2 = Ar[32] == 1;
            DoitWater1 = Ar[33] == 1;
            WaterPow2 = Ar[34];
            WaterPow1 = Ar[35];
            DoitDoubleEdge2 = Ar[36] == 1;
            DoitDoubleEdge1 = Ar[37] == 1;
            DoubleEdge2 = Ar[38] == 1;
            DoubleEdge1 = Ar[39] == 1;
            DoubleEdgeAtt2 = Ar[40];
            DoubleEdgeDef2 = Ar[41];
            DoubleEdgeAtt1 = Ar[42];
            DoubleEdgeDef1 = Ar[43];
            Snowing2 = Ar[44] == 1;
            Snowing1 = Ar[45] == 1;
            Healing2 = Ar[46] == 1;
            Healing1 = Ar[47] == 1;
            Heal2 = Ar[48];
            Heal1 = Ar[49];
            HealCount2 = Ar[50];
            HealCount1 = Ar[51];
            PoisonAir2 = Ar[52] == 1;
            PoisonAir1 = Ar[53] == 1;
            PoisonPow2 = Ar[54];
            PoisonPow1 = Ar[55];
            HelperSpheres2 = Ar[56];
            HelperSpheres1 = Ar[57];
            DoitPoison2 = Ar[58] == 1;
            DoitPoison1 = Ar[59] == 1;
            DoitSmoke2 = Ar[60] == 1;
            DoitSmoke1 = Ar[61] == 1;
            PPDrain2 = Ar[62];
            PPDrain1 = Ar[63];
            HardeningPow2 = Ar[64];
            HardeningPow1 = Ar[65];
            player2.SetEP(Ar[66]);
            player1.SetEP(Ar[67]);
            #endregion
        }
        else {
            #region Setting Stuff
            StartRoundCountCurseP1 = Ar[0];
            StartRoundCountCurseP2 = Ar[1];
            StartRoundCountP1 = Ar[2];
            StartRoundCountP2 = Ar[3];
            StartRoundCountBurn1 = Ar[4];
            StartRoundCountBurn2 = Ar[5];
            StartRoundCountFreeze1 = Ar[6];
            StartRoundCountFreeze2 = Ar[7];
            StartRoundCountStuckInPlace1 = Ar[8];
            StartRoundCountStuckInPlace2 = Ar[9];
            protectiveEarthEffectP1 = Ar[10] == 1;
            protectiveEarthEffectP2 = Ar[11] == 1;
            MateOn1 = Ar[12] == 1;
            MateOn2 = Ar[13] == 1;
            MatePow1 = Ar[14];
            MatePow2 = Ar[15];
            MateLP1 = Ar[16];
            MateLP2 = Ar[17];
            AssistCube1 = Ar[18] == 1;
            AssistCube2 = Ar[19] == 1;
            FireEnergy1 = Ar[20] == 1;
            FireEnergy2 = Ar[21] == 1;
            BluePlanet1 = Ar[22] == 1;
            BluePlanet2 = Ar[23] == 1;
            Doit1 = Ar[24] == 1;
            Doit2 = Ar[25] == 1;
            AssistPow1 = Ar[26];
            AssistPow2 = Ar[27];
            DoitFire1 = Ar[28] == 1;
            DoitFire2 = Ar[29] == 1;
            FirePow1 = Ar[30];
            FirePow2 = Ar[31];
            DoitWater1 = Ar[32] == 1;
            DoitWater2 = Ar[33] == 1;
            WaterPow1 = Ar[34];
            WaterPow2 = Ar[35];
            DoitDoubleEdge1 = Ar[36] == 1;
            DoitDoubleEdge2 = Ar[37] == 1;
            DoubleEdge1 = Ar[38] == 1;
            DoubleEdge2 = Ar[39] == 1;
            DoubleEdgeAtt1 = Ar[40];
            DoubleEdgeDef1 = Ar[41];
            DoubleEdgeAtt2 = Ar[42];
            DoubleEdgeDef2 = Ar[43];
            Snowing1 = Ar[44] == 1;
            Snowing2 = Ar[45] == 1;
            Healing1 = Ar[46] == 1;
            Healing2 = Ar[47] == 1;
            Heal1 = Ar[48];
            Heal2 = Ar[49];
            HealCount1 = Ar[50];
            HealCount2 = Ar[51];
            PoisonAir1 = Ar[52] == 1;
            PoisonAir2 = Ar[53] == 1;
            PoisonPow1 = Ar[54];
            PoisonPow2 = Ar[55];
            HelperSpheres1 = Ar[56];
            HelperSpheres2 = Ar[57];
            DoitPoison1 = Ar[58] == 1;
            DoitPoison2 = Ar[59] == 1;
            DoitSmoke1 = Ar[60] == 1;
            DoitSmoke2 = Ar[61] == 1;
            PPDrain1 = Ar[62];
            PPDrain2 = Ar[63];
            HardeningPow1 = Ar[64];
            HardeningPow2 = Ar[65];
            player1.SetEP(Ar[66]);
            player2.SetEP(Ar[67]);
            #endregion
        }

        if (MateOn1) {
            if (player1.Mate == null)
                player1.Mate = player1.transform.Find("Mate").GetComponent<MateCode>();
            player1.Mate.Reconnect = true;
            player1.Mate.gameObject.SetActive(true);
            player1.Mate.Initialize(MateLP1, MatePow1);
            MateLife1.SetActive(true);
            int AbLevel;
            Int32.TryParse(AbLevelArray[25], out AbLevel);
            MateLife1.GetComponent<Slider>().maxValue = StatsArr[25][AbLevel + 2] + levelStats[PlayerPrefsX.GetIntArray("Level")[shapeID1] - 1][shapeID1][1];
            MateLife1.GetComponent<Slider>().value = MateLP1;
            if (player1 is Star_Player) {
                if (player1.Mate.Anim == null)
                    player1.Mate.Anim = player1.Mate.GetComponent<Animator>();
                player1.Mate.Anim.SetBool("Star", true);
            }
            First1 = false;
        }
        if (MateOn2) {
            if (player2.Mate == null)
                player2.Mate = player2.transform.Find("Mate").GetComponent<MateCode>();
            player2.Mate.Reconnect = true;
            player2.Mate.gameObject.SetActive(true);
            player2.Mate.Initialize(MateLP2, MatePow2);
            MateLife2.SetActive(true);
            int AbLevel;
            Int32.TryParse(TempOpponent.Opponent.AbLevelArray[25], out AbLevel);
            MateLife2.GetComponent<Slider>().maxValue = StatsArr[25][AbLevel + 2] + levelStats[TempOpponent.Opponent.OpLvl - 1][shapeID2][1];
            MateLife2.GetComponent<Slider>().value = MateLP2;
            if (player2 is Star_Player) {
                if (player2.Mate.Anim == null)
                    player2.Mate.Anim = player2.Mate.GetComponent<Animator>();
                player2.Mate.Anim.SetBool("Star", true);
            }
            First2 = false;
        }
    }
    public string ReconnectString() {
        List<int> L = new List<int>();
        #region Adding Stuff
        L.Add(StartRoundCountCurseP1);
        L.Add(StartRoundCountCurseP2);
        L.Add(StartRoundCountP1);
        L.Add(StartRoundCountP2);
        L.Add(StartRoundCountBurn1);
        L.Add(StartRoundCountBurn2);
        L.Add(StartRoundCountFreeze1);
        L.Add(StartRoundCountFreeze2);
        L.Add(StartRoundCountStuckInPlace1);
        L.Add(StartRoundCountStuckInPlace2);
        L.Add(protectiveEarthEffectP1 ? 1 : 0);
        L.Add(protectiveEarthEffectP2 ? 1 : 0);
        L.Add(MateOn1 ? 1 : 0);
        L.Add(MateOn2 ? 1 : 0);
        L.Add(MatePow1);
        L.Add(MatePow2);
        L.Add(MateLP1);
        L.Add(MateLP2);
        L.Add(AssistCube1 ? 1 : 0);
        L.Add(AssistCube2 ? 1 : 0);
        L.Add(FireEnergy1 ? 1 : 0);
        L.Add(FireEnergy2 ? 1 : 0);
        L.Add(BluePlanet1 ? 1 : 0);
        L.Add(BluePlanet2 ? 1 : 0);
        L.Add(Doit1 ? 1 : 0);
        L.Add(Doit2 ? 1 : 0);
        L.Add(AssistPow1);
        L.Add(AssistPow2);
        L.Add(DoitFire1 ? 1 : 0);
        L.Add(DoitFire2 ? 1 : 0);
        L.Add(FirePow1);
        L.Add(FirePow2);
        L.Add(DoitWater1 ? 1 : 0);
        L.Add(DoitWater2 ? 1 : 0);
        L.Add(WaterPow1);
        L.Add(WaterPow2);
        L.Add(DoitDoubleEdge1 ? 1 : 0);
        L.Add(DoitDoubleEdge2 ? 1 : 0);
        L.Add(DoubleEdge1 ? 1 : 0);
        L.Add(DoubleEdge2 ? 1 : 0);
        L.Add(DoubleEdgeAtt1);
        L.Add(DoubleEdgeDef1);
        L.Add(DoubleEdgeAtt2);
        L.Add(DoubleEdgeDef2);
        L.Add(Snowing1 ? 1 : 0);
        L.Add(Snowing2 ? 1 : 0);
        L.Add(Healing1 ? 1 : 0);
        L.Add(Healing2 ? 1 : 0);
        L.Add(Heal1);
        L.Add(Heal2);
        L.Add(HealCount1);
        L.Add(HealCount2);
        L.Add(PoisonAir1 ? 1 : 0);
        L.Add(PoisonAir2 ? 1 : 0);
        L.Add(PoisonPow1);
        L.Add(PoisonPow2);
        L.Add(HelperSpheres1);
        L.Add(HelperSpheres2);
        L.Add(DoitPoison1 ? 1 : 0);
        L.Add(DoitPoison2 ? 1 : 0);
        L.Add(DoitSmoke1 ? 1 : 0);
        L.Add(DoitSmoke2 ? 1 : 0);
        L.Add(PPDrain1);
        L.Add(PPDrain2);
        L.Add(HardeningPow1);
        L.Add(HardeningPow2);
        L.Add(player1.GetEP());
        L.Add(player2.GetEP());
        #endregion
        return string.Join(",", L);
    }
    public virtual void GameOver() { }

    public void AddText(Text TheText, string TextToAdd) {
        if (TheText.text == "")
            TheText.text = TextToAdd;
        else
            TheText.text = TheText.text + Environment.NewLine + TextToAdd;
    }

    protected void AddReplay(string OtherPlayerUsername, int ShapeID, int OtherplayerShapeID, string ChoicesP1,
        string ChoicesP2, string ProbsP1, string ProbsP2, string Level1, string Level2, string Super100Level1,
        string Super100Level2, string Super200Level1, string Super200Level2, string PassivesLevel1, string PassivesLevel2,
        bool Won, bool Friendly = false, bool Draw = false) {
        string[] Replay = { ChoicesP1, ChoicesP2, ProbsP1, ProbsP2, Level1, Level2, Super100Level1, Super100Level2,
            Super200Level1, Super200Level2, PassivesLevel1, PassivesLevel2, OtherPlayerUsername, ShapeID.ToString(),
            OtherplayerShapeID.ToString(), PlayerPrefs.GetInt("SkinID").ToString(), TempOpponent.Opponent.SkinID.ToString(),
            PlayerPrefsX.GetIntArray("Level")[ShapeID].ToString(), TempOpponent.Opponent.OpLvl.ToString(), "", "" };
        int PPAdded;
        if (Friendly || Draw) { PPAdded = 0; }
        else {
            #region PPCalculations
            int PPdiff = (PlayerPrefsX.GetIntArray("PP")[shapeID1] - TempOpponent.Opponent.OpPP);                 //TODO: Fix for 2v2 game mode
            PPdiff = Won ? PPdiff : -PPdiff;
            PPAdded = 0;
            if (PPdiff >= 10) { PPAdded = 1; }
            if (PPdiff < 10 && PPdiff >= 5) { PPAdded = 2; }
            if (PPdiff < 5 && PPdiff >= 0) { PPAdded = 3; }
            if (PPdiff < 0 && PPdiff >= -5) { PPAdded = 4; }
            if (PPdiff < -5 && PPdiff >= -10) { PPAdded = 5; }
            if (PPdiff < -10) { PPAdded = 6; }
            #endregion
            if (PlayerPrefs.GetInt("BotOnline") == 1)
                TempOpponent.Opponent.OpPP += Won ? -PPAdded : PPAdded;
            PPAdded = Won ? PPAdded : -PPAdded;
        }
        DateTime CurrentTime = DateTime.Now;
        int[] Time = { CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second };
        string TheTime = string.Join(",", Time);
        Replay[Replay.Length - 2] = PPAdded.ToString(); Replay[Replay.Length - 1] = TheTime;

        string[] OldReplay = PlayerPrefsX.GetStringArray("Replay");
        string[] ReplayAr;
        if (OldReplay == null || OldReplay.Length == 0) { ReplayAr = Replay; }
        else {
            ReplayAr = new string[(OldReplay.Length < 9 * 21) ? (OldReplay.Length + 21) : (9 * 21)];
            int i = 0;
            foreach (string s in Replay) { ReplayAr[i] = s; i++; }
            foreach (string s in OldReplay) {
                ReplayAr[i] = s; i++;
                if (i >= 9 * 21) { break; }
            }
        }

        for (int i = 0; i < ReplayAr.Length; i++)
            if (ReplayAr[i] == null) { ReplayAr[i] = ""; }

        bool b = PlayerPrefsX.SetStringArray("Replay", ReplayAr);
        PlayerPrefs.Save();
    }
}
