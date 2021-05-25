using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMasterSpectate : GameMaster {
    private bool SendPackage = false;
    private bool Go = true;

    private bool Stop1 = false;
    private bool Stop2 = false;
    private bool Stop3 = false;
    private bool Stop4 = false;
    private bool Stop5 = false;
    private bool Stop6 = false;
    private bool Stop7 = false;
    private bool Stop8 = false;
    private bool Stop9 = false;
    private bool Stop10 = false;
    private bool Stop11 = false;
    private bool Stop12 = false;
    private bool Stop13 = false;
    private bool Stop14 = false;
    private bool Stop15 = false;
    private bool Stop16 = false;

    private bool otherCall;

    public static GameObject[] Abilities;
    public GameObject Abs;
    public GameObject Attack;
    public static GameObject SpecialAb;
    public GameObject Special;

    System.Random R = new System.Random();

    public GameObject Disc;

    protected override void Awake() {
        doneInit = true;
        Spectate = true;
        Online = true;
        Replay = false;

        otherCall = false;


        //Every ability button has ALL Ids so no need for multiple canvases for each shape
        shapeID1 = TempOpponent.Opponent.ShapeID2;
        shapeID2 = TempOpponent.Opponent.ShapeID;

        background = PlayerPrefs.GetInt("BckgdID");
        for (int i = 0; i < Backgrounds.Length; i++) {
            Backgrounds[i].SetActive(i == background);
        }

        if (background == 0) {
            cameraFight.backgroundColor = new Color(0, 0, 0);
        }
        else if (background == 1) {
            cameraFight.backgroundColor = new Color(0.5568628f, 0.09019608f, 0.09803922f);

            Transform temp = Backgrounds[1].transform.Find("UP");
            temp.Find("Front").gameObject.SetActive(true);
            temp.Find("P1").gameObject.SetActive(false);
            temp.Find("P2").gameObject.SetActive(false);
        }

        AbLevelArray = TempOpponent.Opponent.AbLevelArray2;
        Super100 = TempOpponent.Opponent.Super1002;
        Super200 = TempOpponent.Opponent.Super2002;

        StatsArrStr = PlayerPrefsX.GetStringArray("StatsArray");
        Super100StatsArrStr = PlayerPrefsX.GetStringArray("Super100StatsArray");
        Super200StatsArrStr = PlayerPrefsX.GetStringArray("Super200StatsArray");

        StatsArr = ClientHandleData.TransformStringArray(StatsArrStr);
        Super100StatsArr = ClientHandleData.TransformStringArray(Super100StatsArrStr);
        Super200StatsArr = ClientHandleData.TransformStringArray(Super200StatsArrStr);

        string[] PassiveStatsArray = PlayerPrefsX.GetStringArray("PassiveStatsArray");
        PassiveStats = ClientHandleData.TransformStringArray(PassiveStatsArray); // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog
        PassivesArray = TempOpponent.Opponent.Passives2; // 0 = StuckinPlace, 1 = Hardening, 2 = Freeze, 3 = Snowing, 4 = Burn, 5 = FieryEyes, 6 = HelperSpheres, 7 =Fog

        cubeP1.gameObject.SetActive(shapeID1 == 0);
        pyramidP1.gameObject.SetActive(shapeID1 == 1);
        starP1.gameObject.SetActive(shapeID1 == 2);
        sphereP1.gameObject.SetActive(shapeID1 == 3);
        if (shapeID1 == 0)
            player1 = GameObject.Find("Player1").GetComponent<Cube_Player>();
        else if (shapeID1 == 1)
            player1 = GameObject.Find("Player1").GetComponent<Pyramid_Player>();
        else if (shapeID1 == 2)
            player1 = GameObject.Find("Player1").GetComponent<Star_Player>();
        else if (shapeID1 == 3)
            player1 = GameObject.Find("Player1").GetComponent<Sphere_Player>();

        cubeP2.gameObject.SetActive(shapeID2 == 0);
        pyramidP2.gameObject.SetActive(shapeID2 == 1);
        starP2.gameObject.SetActive(shapeID2 == 2);
        sphereP2.gameObject.SetActive(shapeID2 == 3);
        if (shapeID2 == 0)
            player2 = GameObject.Find("Player2").GetComponent<Cube_Player>();
        else if (shapeID2 == 1)
            player2 = GameObject.Find("Player2").GetComponent<Pyramid_Player>();
        else if (shapeID2 == 2)
            player2 = GameObject.Find("Player2").GetComponent<Star_Player>();
        else if (shapeID2 == 3)
            player2 = GameObject.Find("Player2").GetComponent<Sphere_Player>();


        animatorPlayer1 = player1.GetComponent<Animator>();
        animatorPlayer2 = player2.GetComponent<Animator>();
        animatorPlayer1.SetInteger("ID", -1);
        animatorPlayer2.SetInteger("ID", -1);

        if (shapeID1 == 1 || shapeID1 == 2 || shapeID1 == 3) {
            animatorPlayer1.SetBool("Player1", true);
        }

        round = 1;
        StartRoundCountP1 = 0;
        StartRoundCountP2 = 0;
        playerWon = 0;

        protectiveEarthEffectP1 = false;
        protectiveEarthEffectP2 = false;

        //endGame.gameObject.SetActive(false);

        canvasPlayer1.gameObject.SetActive(true);
        Abilities = new GameObject[6];
        string[] namesS = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
        for (int i = 1; i < 7; i++) {
            Abilities[i - 1] = GameObject.Find("Ability" + i);
            foreach (Transform Child in Abilities[i - 1].transform) {
                if (Child.name == "QMark") {
                    Child.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
                else {
                    Child.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
                    if (Child.GetComponent<Shape_Abilities>().common == true) {
                        for (int k = 0; k < namesS.Length; k++)
                            Child.transform.Find(namesS[k]).gameObject.SetActive(k == shapeID1);
                    }
                }
            }
        }
        Ab = canvasPlayer1.transform.Find("Abs").Find("Attack");
        for (int i = 0; i < 4; i++) {
            Ab.GetChild(i).gameObject.SetActive(i == shapeID1);
            if (Ab.GetChild(i).gameObject.activeSelf)
                Ab.GetChild(i).GetComponent<Button>().interactable = false;
        }
        Transform ep = canvasPlayer1.transform.Find("EnergyPoints");
        for (int i = 0; i < 4; i++) {
            ep.GetChild(i).gameObject.SetActive(i == shapeID1);
        }

        FightCanvas = GameObject.Find("FightCanvas");
        FightText = FightCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        FightCanvas.SetActive(false);

        p1Name = TempOpponent.Opponent.Username2;
        p2Name = TempOpponent.Opponent.Username;

        ClientTCP.LPID = R.Next(1000000000);
        ClientTCP.PACKAGE_DEBUG("GiveLP", "", TempOpponent.Opponent.ConnectionID2);
    }

    protected override void Start() {
        GameObject[] Rubble = new GameObject[MiddleRubble.transform.childCount];
        Rubbles = new FallInPieces[MiddleRubble.transform.childCount];
        for (int i = 0; i < MiddleRubble.transform.childCount; i++) {
            Rubble[i] = MiddleRubble.transform.GetChild(i).gameObject;
            Rubbles[i] = Rubble[i].GetComponent<FallInPieces>();
        }
        SpecialAb = Special;
        foreach (Transform Child in SpecialAb.transform) {
            if (Child.name == "QMark") {
                Child.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            else {
                Child.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            }
        }

        roundDisplay1 = canvasPlayer1.transform.Find("Round").gameObject;

        roundDisplay1.transform.Find("Text").GetComponent<Text>().text = "1";
        roundDisplay1.transform.Find("X2").gameObject.SetActive(false);

        LifeBars = new LifeBar_ValueChange[4];
        LifeBars[0] = canvasPlayer1.transform.Find("Player1Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[1] = canvasPlayer1.transform.Find("Player2Life").GetComponent<LifeBar_ValueChange>();
        LifeBars[2] = canvasPlayer1.transform.Find("MateLife").GetComponent<LifeBar_ValueChange>();
        LifeBars[3] = canvasPlayer1.transform.Find("MateLife2").GetComponent<LifeBar_ValueChange>();

    }

    //Update??? Call???
    public void CheckChoices() {
        player1ChoiceDone = player1.GetChoiceDone();
        player2ChoiceDone = player2.GetChoiceDone();

        if (player1ChoiceDone && player2ChoiceDone && call == false) {
            call = true;
            StartCoroutine(EvaluateOutput());
            //To avoid calling this multiple times while waiting for 2 sec  
        }
    }

    protected override IEnumerator EvaluateOutput() {
        //To avoid multiple evaluation
        if (otherCall == false) {
            Abs.SetActive(false);
            yield return StartCoroutine(base.EvaluateOutput());
            otherCall = true;

            // PASSIVES
            //For the star burn passive
            if (player1 is Star_Player) {
                if (netDamageTakenP2 > 0 && (player1.GetComponent<Star_Player>().CanBurn == true || player1.ProbPlusAtt > 0)) {
                    Stop1 = true;
                    StartCoroutine(WaitForProb(1, 1));
                }
            }
            if (player2 is Star_Player) {
                if (netDamageTakenP1 > 0 && (player2.GetComponent<Star_Player>().CanBurn == true || player2.ProbPlusAtt > 0)) {
                    Stop2 = true;
                    StartCoroutine(WaitForProb(2, 1));
                }
            }
            //For turning off the FieryEyes or others
            if (player1.GetComponent<Shape_Player>().getAdditionalDamage() > 0) {
                player1.GetComponent<Shape_Player>().setAdditionalDamage(0); boostP1 = 0;
                player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
                AddText(FightText, "The Attack Boost faded.");
            }
            if (player2.GetComponent<Shape_Player>().getAdditionalDamage() > 0) {
                player2.GetComponent<Shape_Player>().setAdditionalDamage(0); boostP2 = 0;
                player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
                AddText(FightText, "The Attack Boost faded.");
            }
            //For the Star Additional Attack passive
            if (player1 is Star_Player) {
                if (netDamageTakenP1 > 0 && (player1.GetComponent<Star_Player>().CanFieryEyes == true || player1.ProbPlusDef > 0)) {
                    if (Stop1) {
                        StartCoroutine(Fuckwhileloops(1));
                    }
                    else {
                        Stop3 = true;
                        StartCoroutine(WaitForProb(1, 2));
                    }
                }
            }
            if (player2 is Star_Player) {
                if (netDamageTakenP2 > 0 && (player2.GetComponent<Star_Player>().CanFieryEyes == true || player2.ProbPlusDef > 0)) {
                    if (Stop2) {
                        StartCoroutine(Fuckwhileloops(2));
                    }
                    else {
                        Stop4 = true;
                        StartCoroutine(WaitForProb(2, 2));
                    }
                }
            }

            //For the cube stuck in place passive
            if (player1 is Cube_Player) {
                if (netDamageTakenP2 > 0 && (player1.GetComponent<Cube_Player>().CanStuckInPlace == true || player1.ProbPlusAtt > 0)) {
                    Stop5 = true;
                    StartCoroutine(WaitForProb(1, 3));
                }
            }
            if (player2 is Cube_Player) {
                if (netDamageTakenP1 > 0 && (player2.GetComponent<Cube_Player>().CanStuckInPlace == true || player2.ProbPlusAtt > 0)) {
                    Stop6 = true;
                    StartCoroutine(WaitForProb(2, 3));
                }
            }

            //For the cube protective earth passive; it stays enabled until the player is attacked
            if (player1 is Cube_Player) {
                if (netDamageTakenP1 > 0 && (player1.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player1.ProbPlusDef > 0) && !protectiveEarthEffectP1) {
                    if (Stop5) {
                        StartCoroutine(Fuckwhileloops(3));
                    }
                    else {
                        Stop7 = true;
                        StartCoroutine(WaitForProb(1, 4));
                    }
                }
            }
            if (player2 is Cube_Player) {
                if (netDamageTakenP2 > 0 && (player2.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player2.ProbPlusDef > 0) && !protectiveEarthEffectP2) {
                    if (Stop6) {
                        StartCoroutine(Fuckwhileloops(4));
                    }
                    else {
                        Stop8 = true;
                        StartCoroutine(WaitForProb(2, 4));
                    }
                }
            }

            //For the pyramid freeze passive

            if (player1 is Pyramid_Player) {
                if (netDamageTakenP2 > 0 && (player1.GetComponent<Pyramid_Player>().CanFreeze == true || player1.ProbPlusAtt > 0)) {
                    Stop9 = true;
                    StartCoroutine(WaitForProb(1, 5));
                }
            }
            if (player2 is Pyramid_Player) {
                if (netDamageTakenP1 > 0 && (player2.GetComponent<Pyramid_Player>().CanFreeze == true || player2.ProbPlusAtt > 0)) {
                    Stop10 = true;
                    StartCoroutine(WaitForProb(2, 5));
                }
            }

            //for the Pyramid Snow Passive
            if (player1 is Pyramid_Player) {
                if (netDamageTakenP1 > 0 && (player1.GetComponent<Pyramid_Player>().CanSnow == true || player1.ProbPlusDef > 0)) {
                    if (Stop9) {
                        StartCoroutine(Fuckwhileloops(5));
                    }
                    else {
                        Stop11 = true;
                        StartCoroutine(WaitForProb(1, 6));
                    }
                }
            }
            if (player2 is Pyramid_Player) {
                if (netDamageTakenP2 > 0 && (player2.GetComponent<Pyramid_Player>().CanSnow == true || player2.ProbPlusDef > 0)) {
                    if (Stop10) {
                        StartCoroutine(Fuckwhileloops(6));
                    }
                    else {
                        Stop12 = true;
                        StartCoroutine(WaitForProb(2, 6));
                    }
                }
            }

            //for the Sphere HelperSpheres Passive
            if (player1 is Sphere_Player) {
                if (netDamageTakenP2 > 0 && (player1.GetComponent<Sphere_Player>().CanHelperSpheres || player1.ProbPlusAtt > 0)) {
                    Stop13 = true;
                    StartCoroutine(WaitForProb(1, 7));
                }
            }
            if (player2 is Sphere_Player) {
                if (netDamageTakenP1 > 0 && (player2.GetComponent<Sphere_Player>().CanHelperSpheres || player2.ProbPlusAtt > 0)) {
                    Stop14 = true;
                    StartCoroutine(WaitForProb(2, 7));
                }
            }

            //For The Smoke Passive
            if (player1 is Sphere_Player) {
                if (netDamageTakenP1 > 0 && (player1.GetComponent<Sphere_Player>().CanSmoke || player1.ProbPlusDef > 0)) {
                    if (Stop13) {
                        StartCoroutine(Fuckwhileloops(7));
                    }
                    else {
                        Stop15 = true;
                        StartCoroutine(WaitForProb(1, 8));
                    }
                }
            }
            if (player2 is Sphere_Player) {
                if (netDamageTakenP2 > 0 && (player2.GetComponent<Sphere_Player>().CanSmoke || player2.ProbPlusDef > 0)) {
                    if (Stop14) {
                        StartCoroutine(Fuckwhileloops(8));
                    }
                    else {
                        Stop16 = true;
                        StartCoroutine(WaitForProb(2, 8));
                    }
                }
            }

            player1Life = player1.GetLife();
            player2Life = player2.GetLife();

            if (player1Life <= 0 || player2Life <= 0) {
                AddText(FightText, "Game Over!");
                GameOver();
            }
            else if (Stop1 || Stop2 || Stop3 || Stop4 || Stop5 || Stop6 || Stop7 || Stop8 || Stop9 || Stop10 || Stop11 || Stop12 || Stop13 || Stop14 || Stop15 || Stop16) {
                StartCoroutine(Reset());
            }
            else {
                ResetRound();
            }
        }
    }

    protected override void ResetRound(bool Start = false) {
        if (!Start)
            Abs.SetActive(true);
        base.ResetRound(Start);

        ClientTCP.LPID = R.Next(1000000000);
        ClientTCP.PACKAGE_DEBUG("GiveLP", "", TempOpponent.Opponent.ConnectionID2);

        SendPackage = false;
        otherCall = false;

        SetValueBack();

        round++;
    }

    public override void GameOver() {
        animatorPlayer1.SetInteger("ID", -1);
        animatorPlayer2.SetInteger("ID", -1);
        if (player1Life <= 0) {
            playerWon = 2;
            //cameraPlayer2.gameObject.SetActive(true);
            //cameraPlayer2.GetComponent<Animator>().SetBool("End", true);
        }
        else {
            /*if (Mode == 0)
            {
                ClientTCP.PACKAGE_ENDGAME(PlayerPrefs.GetString("Username"), TempOpponent.Opponent.Username, TempOpponent.Opponent.ConnectionID, shapeID1, shapeID2);
            }
            else if (Mode == 1)
            {
                ClientTCP.PACKAGE_ENDGAME(PlayerPrefs.GetString("Username"), TempOpponent.Opponent.Username, TempOpponent.Opponent.ConnectionID, shapeID1, shapeID2, true);
            }*/
            playerWon = 1;
            //cameraFight.gameObject.SetActive(false);
            //cameraPlayer1.gameObject.SetActive(true);
            //cameraPlayer1.GetComponent<Animator>().SetBool("End", true);
        }
        /*if (Mode == 1)
        {
            TempOpponent.Opponent.FriendlyBattle = false;
            TempOpponent.Opponent.Accepting = false;
        }*/
        //TempOpponent.Opponent.Reset();
        //SceneManager.LoadScene("Choosing Scene");
        //StartCoroutine(endC());
    }
    IEnumerator endC() {
        yield return new WaitForSeconds(1.5f);
        endGame.gameObject.SetActive(true);
        endGame.GetComponent<Endgame>().UpdateEnd(0, 0, 0);
    }
    void SetValueBack() {
        Go = true;
        call = false;
    }
    IEnumerator WaitForProb(int Player, int ID) {
        if (Player == 1) {
            yield return new WaitUntil(() => TempOpponent.Opponent.GotProb1 == true || TempOpponent.Opponent.GotProb11);
            if (TempOpponent.Opponent.GotProb1) {
                TempOpponent.Opponent.GotProb1 = false;
            }
            else if (TempOpponent.Opponent.GotProb11) {
                TempOpponent.Opponent.GotProb11 = false;
            }
            if (ID == 1) {
                if (TempOpponent.Opponent.Probability <= (((Star_Player)player1).CanBurn ? PassiveStats[4][PassivesArray[4] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt)) {
                    StartRoundCountBurn2 = round;
                    player2.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop1 = false;
            }
            else if (ID == 2) {
                int Range = ((Star_Player)player1).CanFieryEyes ? PassiveStats[5][PassivesArray[5] + 2] + player1.ProbPlusDef : player1.ProbPlusDef;
                if (TempOpponent.Opponent.Probability <= Range || TempOpponent.Opponent.Probability1 <= Range) {
                    player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player1.GetComponent<Shape_Player>().setAdditionalDamage(PassiveStats[5][PassivesArray[5] - 1]);
                    AddText(FightText, "Player 1 got angry because he took damage and gained a power boost.");
                }
                TempOpponent.Opponent.Probability = 0;
                TempOpponent.Opponent.Probability1 = 0;
                Stop3 = false;
            }
            else if (ID == 3) {
                if (TempOpponent.Opponent.Probability <= (((Cube_Player)player1).CanStuckInPlace ? PassiveStats[0][PassivesArray[0] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt)) {
                    StartRoundCountStuckInPlace2 = round;
                    player2.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop5 = false;
            }
            else if (ID == 4) {
                int Range = ((Cube_Player)player1).CanHaveProtectiveEarth ? PassiveStats[1][PassivesArray[1] + 2] + player1.ProbPlusDef : player1.ProbPlusDef;
                if (TempOpponent.Opponent.Probability <= Range || TempOpponent.Opponent.Probability1 <= Range) {
                    player1.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP1 = true;
                    HardeningPow1 = PassiveStats[1][PassivesArray[1] - 1];
                    AddText(FightText, "Player 1 hardened himself");
                }
                TempOpponent.Opponent.Probability = 0;
                TempOpponent.Opponent.Probability1 = 0;
                Stop7 = false;
            }
            else if (ID == 5) {
                if (TempOpponent.Opponent.Probability <= (((Pyramid_Player)player1).CanFreeze ? PassiveStats[2][PassivesArray[2] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt)) {
                    StartRoundCountFreeze2 = round;
                    animatorPlayer2.SetBool("Frozen", true);
                    AddText(FightText, "Player 2 is frozen for " + PassiveStats[2][PassivesArray[2] - 1] + " round(s)");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop9 = false;
            }
            else if (ID == 6) {
                int Range = ((Pyramid_Player)player1).CanSnow ? PassiveStats[3][PassivesArray[3] + 2] + player1.ProbPlusDef : player1.ProbPlusDef;
                if (TempOpponent.Opponent.Probability <= Range || TempOpponent.Opponent.Probability1 <= Range) {
                    player1.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing1 = true;
                    AddText(FightText, "Player 1's call for help from the Weather Gods has been answered!");
                }
                TempOpponent.Opponent.Probability = 0;
                TempOpponent.Opponent.Probability1 = 0;
                Stop11 = false;
            }
            else if (ID == 7) {
                if (TempOpponent.Opponent.Probability <= (((Sphere_Player)player1).CanHelperSpheres ? PassiveStats[6][PassivesArray[6] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt)) {
                    player1.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres1 = 1;
                    AddText(FightText, "Poison Spheres rushed to Player1's help!");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop13 = false;
            }
            else if (ID == 8) {
                int Range = ((Sphere_Player)player1).CanSmoke ? PassiveStats[7][PassivesArray[7] + 2] + player1.ProbPlusDef : player1.ProbPlusDef;
                if (TempOpponent.Opponent.Probability <= Range || TempOpponent.Opponent.Probability1 <= Range) {
                    player1.GetComponent<Sphere_Player>().Smoke.SetActive(true);
                    DoitSmoke1 = true;
                    AddText(FightText, "Player 1 was surrounded by Fog!");
                }
                TempOpponent.Opponent.Probability = 0;
                TempOpponent.Opponent.Probability1 = 0;
                Stop15 = false;
            }
        }
        else {
            yield return new WaitUntil(() => TempOpponent.Opponent.GotProb2 == true || TempOpponent.Opponent.GotProb21 == true);

            if (TempOpponent.Opponent.GotProb2) {
                TempOpponent.Opponent.GotProb2 = false;
            }
            else {
                TempOpponent.Opponent.GotProb21 = false;
            }
            if (ID == 1) {
                if (TempOpponent.Opponent.Probability2 <= (((Star_Player)player2).CanBurn ? PassiveStats[4][TempOpponent.Opponent.Passives[4] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt)) {
                    StartRoundCountBurn1 = round;
                    player1.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop2 = false;
            }
            else if (ID == 2) {
                int Range = ((Star_Player)player2).CanBurn ? PassiveStats[5][TempOpponent.Opponent.Passives[5] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range) {
                    player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player2.GetComponent<Shape_Player>().setAdditionalDamage(PassiveStats[5][TempOpponent.Opponent.Passives[5] - 1]);
                    AddText(FightText, "Player 2 got angry because he took damage and gained a power boost.");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop4 = false;
            }
            else if (ID == 3) {
                if (TempOpponent.Opponent.Probability2 <= (((Cube_Player)player2).CanStuckInPlace ? PassiveStats[0][TempOpponent.Opponent.Passives[0] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt)) {
                    StartRoundCountStuckInPlace1 = round;
                    player1.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop6 = false;
            }
            else if (ID == 4) {
                int Range = ((Cube_Player)player2).CanHaveProtectiveEarth ? PassiveStats[1][TempOpponent.Opponent.Passives[1] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range) {
                    player2.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP2 = true;
                    HardeningPow2 = PassiveStats[1][TempOpponent.Opponent.Passives[1] - 1];
                    AddText(FightText, "Player 2 hardened himself");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop8 = false;
            }
            else if (ID == 5) {
                if (TempOpponent.Opponent.Probability2 <= (((Pyramid_Player)player2).CanFreeze ? PassiveStats[2][TempOpponent.Opponent.Passives[2] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt)) {
                    StartRoundCountFreeze1 = round;
                    animatorPlayer1.SetBool("Frozen", true);
                    AddText(FightText, "Player 1 is frozen for " + PassiveStats[2][TempOpponent.Opponent.Passives[2] - 1] + " round(s)");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop10 = false;
            }
            else if (ID == 6) {
                int Range = ((Pyramid_Player)player2).CanSnow ? PassiveStats[3][TempOpponent.Opponent.Passives[3] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range) {
                    player2.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing2 = true;
                    AddText(FightText, "Player 2's call for help from the Weather Gods has been answered!");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop12 = false;
            }
            else if (ID == 7) {
                if (TempOpponent.Opponent.Probability2 <= (((Sphere_Player)player2).CanHelperSpheres ? PassiveStats[6][TempOpponent.Opponent.Passives[6] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt)) {
                    player2.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres2 = 1;
                    AddText(FightText, "Poison Spheres rushed to Player2's help!");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop14 = false;
            }
            else if (ID == 8) {
                int Range = ((Sphere_Player)player2).CanSmoke ? PassiveStats[7][TempOpponent.Opponent.Passives[7] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range) {
                    player2.GetComponent<Sphere_Player>().Smoke.SetActive(true);
                    DoitSmoke2 = true;
                    AddText(FightText, "Player 2 was surrounded by Fog!");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop16 = false;
            }
        }
    }
    IEnumerator Reset() {
        yield return new WaitUntil(() => (!Stop1 && !Stop2 && !Stop3 && !Stop4 && !Stop5 && !Stop6 && !Stop7 && !Stop8 && !Stop9 && !Stop10 && !Stop11 && !Stop12 && !Stop13 && !Stop14 && !Stop15 && !Stop16));
        if (player1Life <= 0 || player2Life <= 0)
            GameOver();
        else
            ResetRound();
    }
    IEnumerator Fuckwhileloops(int ID) {
        if (ID == 1) {
            yield return new WaitUntil(() => (!Stop1));
            Stop3 = true;
            StartCoroutine(WaitForProb(1, 2));

        }
        else if (ID == 2) {
            yield return new WaitUntil(() => (!Stop2));
            Stop4 = true;
            StartCoroutine(WaitForProb(2, 2));
        }
        else if (ID == 3) {
            yield return new WaitUntil(() => (!Stop5));
            Stop7 = true;
            StartCoroutine(WaitForProb(1, 4));
        }
        else if (ID == 4) {
            yield return new WaitUntil(() => (!Stop6));
            Stop8 = true;
            StartCoroutine(WaitForProb(2, 4));
        }
        else if (ID == 5) {
            yield return new WaitUntil(() => (!Stop9));
            Stop11 = true;
            StartCoroutine(WaitForProb(1, 6));
        }
        else if (ID == 6) {
            yield return new WaitUntil(() => (!Stop10));
            Stop12 = true;
            StartCoroutine(WaitForProb(2, 6));
        }
        else if (ID == 7) {
            yield return new WaitUntil(() => (!Stop13));
            Stop15 = true;
            StartCoroutine(WaitForProb(1, 8));
        }
        else if (ID == 6) {
            yield return new WaitUntil(() => (!Stop14));
            Stop16 = true;
            StartCoroutine(WaitForProb(2, 8));
        }
    }
}