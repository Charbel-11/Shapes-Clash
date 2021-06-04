using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;

public class GameMasterOffline : GameMaster
{
    public bool Tutorial = false;

    public Text[] TutorialTexts;

    private bool Continue = false;

    private Button[] ContinueButtons = new Button[2];

    private Button[] AbilitiesCanvas1 = new Button[6];
    private Button[] AbilitiesCanvas2 = new Button[6];
    private Button[] Attacks = new Button[2];
    private Button[] Get_EPs = new Button[2];
    private Button[] Supers = new Button[2];

    public Button[] SwitchSkip;

    public Animator[] Hands;

    private int[] TutorialShapeIDs = new int[2];

    private System.Random R = new System.Random();

    // Has to be awake to deactivate the unused player before a script calls them
    protected override void Awake()
    {
        Online = false;
        Replay = false;
        Spectate = false;
        if (PlayerPrefs.GetInt("Tutorial") == 1)
        {
            Tutorial = true;
            Shape_Abilities.Tutorial = true;
            PlayerPrefs.SetInt("Tutorial", 0);
            TutorialShapeIDs[0] = PlayerPrefs.GetInt("ShapeSelectedID");
            TutorialShapeIDs[1] = PlayerPrefs.GetInt("2ShapeSelectedID");
            PlayerPrefs.SetInt("ShapeSelectedID", 0);
            PlayerPrefs.SetInt("2ShapeSelectedID", 3);
            SwitchSkip[0].gameObject.SetActive(false);
            SwitchSkip[1].gameObject.SetActive(true);
            SwitchSkip[2].gameObject.SetActive(false);
            SwitchSkip[3].gameObject.SetActive(true);
        }
        base.Awake();


        skin2 = PlayerPrefs.GetInt("SkinID2");
        player2.transform.Find("Design").Find("Cube").gameObject.SetActive(0 == skin2);
        player2.transform.Find("Design").Find("Cube").GetComponent<MeshRenderer>().enabled = (0 == skin2);
        for (int i = 1; i < 2; i++)
        {
            player2.transform.Find("Design").Find("Cube" + i.ToString()).gameObject.SetActive(i == skin2);
        }

        if (!Tutorial)
        {
            finalIDs1 = PlayerPrefsX.GetIntArray("ActiveAbilityID" + shapeID1.ToString());
            specialAbilityID1 = PlayerPrefs.GetInt("SpecialAbilityID" + shapeID1.ToString());
            finalIDs2 = PlayerPrefsX.GetIntArray("2ActiveAbilityID" + shapeID2.ToString());
            specialAbilityID2 = PlayerPrefs.GetInt("2SpecialAbilityID" + shapeID2.ToString());

            EmotesID = PlayerPrefsX.GetIntArray("EmotesID");
            EmotesID2 = PlayerPrefsX.GetIntArray("EmotesID2");
        }
        else
        {
            finalIDs1 = new int[] { 2, 3, 41, 11, 39, 6 };
            specialAbilityID1 = 101;
            finalIDs2 = new int[] { 4, 44, 7, 52, 46, 49 };
            specialAbilityID2 = 201;

            EmotesID = new int[] { 0, 1, 2, -1 };
            EmotesID = new int[] { 0, 1, 2, -1 };
        }

        if(PlayerPrefs.GetInt("BotOnline") != 1)
            TempOpponent.Opponent.Passives = PassivesArray;
        else
        {
            TempOpponent.Opponent.SkinID = skin2 = 0;
        }
    }

    //Has to be in start for the ability button to get the correct IDs in Awake
    protected override void Start()
    {
        p1Name = "Player 1"; p2Name = "Player 2";
        base.Start();
        //Player 1 is done in both (mother class)
        canvasPlayer2.gameObject.SetActive(true);

        canvasPlayer1.transform.Find("Switch").gameObject.SetActive(false);
        canvasPlayer2.transform.Find("Switch").gameObject.SetActive(false);

        MateLife12 = canvasPlayer2.transform.Find("MateLife").gameObject;
        MateLife22 = canvasPlayer2.transform.Find("MateLife2").gameObject;
        MateLife12.SetActive(false);
        MateLife22.SetActive(false);

        if (PlayerPrefs.GetInt("BotOnline") == 1)
        {
            for (int i = 1; i < EmotesID.Length + 1; i++)
            {
                GameObject.Find("Emotes" + i).GetComponent<EmotesMotherClass>().ID = EmotesID[i - 1];
            }
            p1Name = PlayerPrefs.GetString("Username"); p2Name = TempOpponent.Opponent.Username;
        }

        if (player1.GetLife() <= 0 || player2.GetLife() <= 0)
        {
            player1Life = player1.GetLife(); player2Life = player2.GetLife();
            GameOver();
            return;
        }

        if (botActive && !Tutorial) { canvasPlayer2.gameObject.SetActive(false); return; }

        string[] namesS = { "ImageCube", "ImagePyramid", "ImageStar", "ImageSphere" };
        Ab = canvasPlayer2.transform.Find("AllUsualAbilities");
        for (int j = 0; j < Ab.childCount; j++)
        {
            Transform child = Ab.GetChild(j);
            bool used = false;
            for (int k = 0; k < 6; k++)
            {
                if (child.GetComponent<Shape_Abilities>().ID == finalIDs2[k])
                {
                    if (child.GetComponent<Shape_Abilities>().common == true)
                    {
                        for (int i = 0; i < namesS.Length; i++)
                            child.transform.Find(namesS[i]).gameObject.SetActive(i == shapeID2);
                    }
                    used = true;
                    child.parent = canvasPlayer2.transform.Find("Ability" + (k + 1).ToString());
                    child.SetSiblingIndex(0);
                    child.localScale = new Vector3(1f, 1f, 1f);
                    child.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
                    child.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
                    child.GetComponent<Shape_Abilities>().Awake();
                    child.GetComponent<Shape_Abilities>().Start();
                    break;
                }
            }
            if (used)
            {
                j--; continue;
            }
            child.gameObject.SetActive(false);
        }
        //For the shape's specific attack
        Ab = canvasPlayer2.transform.Find("Attack");
        for (int i = 0; i < 4; i++)
        {
            Ab.GetChild(i).gameObject.SetActive(i == shapeID2);
            if (Ab.GetChild(i).gameObject.activeSelf)
                Ab.GetChild(i).GetComponent<Shape_Abilities>().Awake();
        }

        //For the special ability
        Ab = canvasPlayer2.transform.Find("AllSpecialAbilities");
        foreach (Transform child in Ab)
        {
            if (child.name == "AbDescription") { continue; }
            child.gameObject.SetActive(specialAbilityID2 == child.GetComponent<Shape_Abilities>().ID);
            if (child.gameObject.activeSelf)
                child.GetComponent<Shape_Abilities>().Awake();
        }

        //For the energy point
        Transform ep = canvasPlayer2.transform.Find("EnergyPoints");
        for (int i = 0; i < 4; i++)
        {
            ep.GetChild(i).gameObject.SetActive(i == shapeID2);
        }

        for (int j = 1; j <= 6; j++)
            canvasPlayer2.transform.Find("Ability" + j.ToString()).Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(finalIDs2[j - 1]);

        canvasPlayer2.transform.Find("AllSpecialAbilities").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(specialAbilityID2);

        int[] attackID = { 1, 22, 23, 42 };
        canvasPlayer2.transform.Find("Attack").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(attackID[shapeID2]);

        for (int i = 1; i < EmotesID.Length + 1; i++)
        {
            GameObject.Find("Emotes" + i).GetComponent<EmotesMotherClass>().ID = EmotesID[i - 1];
            GameObject.Find("Emotes2" + i).GetComponent<EmotesMotherClass>().ID = EmotesID2[i - 1];
        }

        canvasPlayer2.transform.Find("Get_EP").GetComponent<Shape_Abilities>().Awake();

        canvasPlayer2.gameObject.SetActive(false);

        foreach (Animator H in Hands)
            H.gameObject.SetActive(false);

        if (Tutorial)
        {
            player1.setAddLvlAtt(0); player1.setAddLvlDef(0);
            player2.setAddLvlAtt(0); player2.setAddLvlDef(0);
            player1.MaxLP = 100; player1.SetLife(100);
            StartCoroutine(TutorialFunction(1));
        }


        if (PlayerPrefs.GetInt("BotOnline") == 1)
            StartCoroutine(waitRound(1));

        Reconnect = false;
       
    }

    public override void TryEvaluating()
    {
        player1ChoiceDone = player1.GetChoiceDone();
        player2ChoiceDone = player2.GetChoiceDone();

        if (player1ChoiceDone && player2ChoiceDone)
        {
            StartCoroutine(EvaluateOutput());
            return;
        }

        if (player1ChoiceDone)
        {
            ChangeCameraAndCanvas();
        }
    }

    //Turn off camera 1 and on camera 2 and change canvases
    void ChangeCameraAndCanvas()
    {
        cameraFight.gameObject.SetActive(false);

        cameraPlayer1.gameObject.SetActive(false);
        canvasPlayer1.gameObject.SetActive(false);

        if (botActive)
        {
            bot.makeChoice();
            cameraFight.gameObject.SetActive(true);
        }
        else
        {
            canvasPlayer2.gameObject.SetActive(true);
            if (cameraState2 == 1)
                cameraPlayer2.gameObject.SetActive(true);
            else
                cameraFight.gameObject.SetActive(true);
        }

        if (background == 1)
        {
            Transform temp = Backgrounds[1].transform.Find("UP");
            temp.Find("P1").gameObject.SetActive(false);
            if (botActive)
            {
                temp.Find("Front").gameObject.SetActive(true);
                temp.Find("P2").gameObject.SetActive(false);
            }
            else
            {
                temp.Find("Front").gameObject.SetActive(cameraState2 == 0);
                temp.Find("P2").gameObject.SetActive(cameraState2 == 1);
            }
        }
    }

    protected override IEnumerator EvaluateOutput()
    {
        if (Tutorial)
            canvasPlayer1.gameObject.SetActive(false);

        ChoicesP1.Add(player1.GetIdOfAnimUsed());
        ChoicesP2.Add(player2.GetIdOfAnimUsed());

        cameraPlayer2.gameObject.SetActive(false);
        canvasPlayer2.gameObject.SetActive(false);

        yield return StartCoroutine(base.EvaluateOutput());

        #region Passives
        //For the star burn passive
        if (player1 is Star_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Star_Player>().CanBurn == true || player1.ProbPlusAtt > 0))
            {
                int Range = player1.ProbPlusAtt;
                if (player1.GetComponent<Star_Player>().CanBurn)
                    Range += PassiveStats[4][PassivesArray[4] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountBurn2 = round;
                    player2.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
            }
        }
        if (player2 is Star_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Star_Player>().CanBurn == true || player2.ProbPlusAtt > 0))
            {
                int Range = player2.ProbPlusAtt;
                if (player2.GetComponent<Star_Player>().CanBurn)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[4][TempOpponent.Opponent.Passives[4] + 2] : PassiveStats[4][PassivesArray[4] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountBurn1 = round;
                    player1.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
            }
        }

        //For the pyramid freeze passive
        if (player1 is Pyramid_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Pyramid_Player>().CanFreeze == true || player1.ProbPlusAtt > 0))
            {
                int Range = player1.ProbPlusAtt;
                if (player1.GetComponent<Pyramid_Player>().CanFreeze)
                    Range += PassiveStats[2][PassivesArray[2] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountFreeze2 = round;
                    animatorPlayer2.SetBool("Frozen", true);
                    AddText(FightText,"Player 2 is frozen for " + PassiveStats[2][PassivesArray[2] - 1] + " round(s)");
                }
            }
        }
        if (player2 is Pyramid_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Pyramid_Player>().CanFreeze == true || player2.ProbPlusAtt > 0))
            {
                int Range = player2.ProbPlusAtt;
                if (player2.GetComponent<Pyramid_Player>().CanFreeze)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[2][TempOpponent.Opponent.Passives[2] + 2] : PassiveStats[2][PassivesArray[2] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountFreeze1 = round;
                    animatorPlayer1.SetBool("Frozen", true);
                    AddText(FightText,"Player 1 is frozen for " + PassiveStats[2][PassivesArray[2] - 1] + " round(s)");
                }
            }
        }

        //For turning off the FieryEyes or others
        if (player1.GetComponent<Shape_Player>().getAdditionalDamage() > 0)
        {
            player1.GetComponent<Shape_Player>().setAdditionalDamage(0); boostP1 = 0;
            if (player1.transform.Find("Design").transform.Find("FieryEyes") != null) { player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false); }
            AddText(FightText,"The Attack Boost faded.");
        }
        if (player2.GetComponent<Shape_Player>().getAdditionalDamage() > 0)
        {
            player2.GetComponent<Shape_Player>().setAdditionalDamage(0); boostP2 = 0;
            if (player2.transform.Find("Design").transform.Find("FieryEyes") != null) { player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false); }
            AddText(FightText,"The Attack Boost faded.");
        }

        //For the Star Additional Attack passive
        if (player1 is Star_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Star_Player>().CanFieryEyes == true || player1.ProbPlusDef > 0))
            {
                int Range = player1.ProbPlusDef;
                if (player1.GetComponent<Star_Player>().CanFieryEyes)
                    Range += PassiveStats[5][PassivesArray[5] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player1.GetComponent<Shape_Player>().setAdditionalDamage(PassiveStats[5][PassivesArray[5] - 1]);
                    AddText(FightText,"Player 1 got angry because he took damage and gained a power boost.");
                }
            }
        }
        if (player2 is Star_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Star_Player>().CanFieryEyes == true || player2.ProbPlusDef > 0))
            {
                int Range = player2.ProbPlusDef;
                if (player2.GetComponent<Star_Player>().CanFieryEyes)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[5][TempOpponent.Opponent.Passives[5] + 2] : PassiveStats[5][PassivesArray[5] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player2.GetComponent<Shape_Player>().setAdditionalDamage(PassiveStats[5][PassivesArray[5] - 1]);
                    AddText(FightText,"Player 2 got angry because he took damage and gained a power boost.");
                }
            }
        }

        //For the cube stuck in place passive
        if (player1 is Cube_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Cube_Player>().CanStuckInPlace == true || player1.ProbPlusAtt > 0))
            {
                int Range = player1.ProbPlusAtt;
                if (player1.GetComponent<Cube_Player>().CanStuckInPlace)
                    Range += PassiveStats[0][PassivesArray[0] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountStuckInPlace2 = round;
                    player2.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText,"Player 2 is stuck in place");
                }
            }
        }
        if (player2 is Cube_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Cube_Player>().CanStuckInPlace == true || player2.ProbPlusAtt > 0))
            {
                int Range = player2.ProbPlusAtt;
                if (player2.GetComponent<Cube_Player>().CanStuckInPlace)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[0][TempOpponent.Opponent.Passives[0] + 2] : PassiveStats[0][PassivesArray[0] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    StartRoundCountStuckInPlace1 = round;
                    player1.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText,"Player 1 is stuck in place");
                }
            }
        }

        //For the cube protective earth passive; it stays enabled until the player is attacked
        if (player1 is Cube_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player1.ProbPlusDef > 0) && !protectiveEarthEffectP1)
            {
                int Range = player1.ProbPlusDef;
                if (player1.GetComponent<Cube_Player>().CanHaveProtectiveEarth)
                    Range += PassiveStats[1][PassivesArray[1] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {                   
                    player1.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP1 = true;
                    HardeningPow1 = PassiveStats[1][PassivesArray[1]-1];
                    AddText(FightText,"Player 1 hardened himself");
                }
            }
        }
        if (player2 is Cube_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player2.ProbPlusDef > 0) && !protectiveEarthEffectP2)
            {
                int Range = player2.ProbPlusDef;
                if (player2.GetComponent<Cube_Player>().CanHaveProtectiveEarth)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[1][TempOpponent.Opponent.Passives[1] + 2] : PassiveStats[1][PassivesArray[1] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    player2.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP2 = true;
                    HardeningPow2 = PassiveStats[1][PassivesArray[1] - 1];
                    AddText(FightText,"Player 2 hardened himself");
                }
            }
        }

        //for the Pyramid Snow Passive
        if (player1 is Pyramid_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Pyramid_Player>().CanSnow || player1.ProbPlusDef > 0))
            {
                int Range = player1.ProbPlusDef;
                if (player1.GetComponent<Pyramid_Player>().CanSnow)
                    Range += PassiveStats[3][PassivesArray[3] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    player1.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing1 = true;
                    AddText(FightText,"Player 1's call for help from the Weather Gods has been answered!");
                }
            }
        }
        if (player2 is Pyramid_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Pyramid_Player>().CanSnow || player2.ProbPlusDef > 0))
            {
                int Range = player2.ProbPlusDef;
                if (player2.GetComponent<Pyramid_Player>().CanSnow)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[3][TempOpponent.Opponent.Passives[3] + 2] : PassiveStats[3][PassivesArray[3] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    player2.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing2 = true;
                    AddText(FightText,"Player 2's call for help from the Weather Gods has been answered!");
                }
            }
        }
        //For The HelperSpheres Passive
        if (player1 is Sphere_Player)
        {
            if(netDamageTakenP2>0 && (player1.GetComponent<Sphere_Player>().CanHelperSpheres || player1.ProbPlusAtt > 0))
            {
                int Range = player1.ProbPlusAtt;
                if (player1.GetComponent<Sphere_Player>().CanHelperSpheres)
                    Range += PassiveStats[6][PassivesArray[6] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    player1.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres1 = 1;
                    AddText(FightText,"Poison Spheres rushed to Player1's help!");
                }
            }
        }
        if (player2 is Sphere_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Sphere_Player>().CanHelperSpheres || player2.ProbPlusAtt > 0))
            {
                int Range = player2.ProbPlusAtt;
                if (player2.GetComponent<Sphere_Player>().CanHelperSpheres)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[6][TempOpponent.Opponent.Passives[6] + 2] : PassiveStats[6][PassivesArray[6] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    player2.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres2 = 1;
                    AddText(FightText,"Poison Spheres rushed to Player2's help!");
                }
            }
        }
        //For The Smoke Passive
        if (player1 is Sphere_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Sphere_Player>().CanSmoke || player1.ProbPlusDef > 0))
            {
                int Range = player1.ProbPlusDef;
                if (player1.GetComponent<Sphere_Player>().CanSmoke)
                    Range += PassiveStats[7][PassivesArray[7] + 2];
                int Probability = R.Next(0, 101);
                ProbsP1.Add(Probability);
                if (Probability <= Range)
                {
                    player1.GetComponent<Sphere_Player>().Smoke.SetActive(true);
                    DoitSmoke1 = true;
                    AddText(FightText,"Player 1 is surrounded by Fog!");
                }
            }
        }
        if (player2 is Sphere_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Sphere_Player>().CanSmoke || player2.ProbPlusDef > 0))
            {
                int Range = player2.ProbPlusDef;
                if (player2.GetComponent<Sphere_Player>().CanSmoke)
                    Range += (PlayerPrefs.GetInt("BotOnline") == 1) ? PassiveStats[7][TempOpponent.Opponent.Passives[7] + 2] : PassiveStats[7][PassivesArray[7] + 2];
                int Probability = R.Next(0, 101);
                ProbsP2.Add(Probability);
                if (Probability <= Range)
                {
                    player2.GetComponent<Sphere_Player>().Smoke.SetActive(true);
                    DoitSmoke2 = true;
                    AddText(FightText,"Player 2 is surrounded by Fog!");
                }
            }
        }
        #endregion 

        player1Life = player1.GetLife();
        player2Life = player2.GetLife();

        if (player1Life <= 0 || player2Life <= 0)
        {
            GameOver();
        }
        else
        {
            ResetRound();
        }
    }

    private void forceEndRound()
    {
        if (player1.GetChoiceDone() == false)
        {
            canvasPlayer1.transform.Find("Get_EP").GetComponent<Get_EP>().UseAbility();
        }
        else if (player2.GetChoiceDone() == false)
        {
            canvasPlayer2.transform.Find("Get_EP").GetComponent<Get_EP>().UseAbility();
        }
    }

    IEnumerator waitRound(int r)
    {
        yield return new WaitForSeconds(4);
        if (round != r) { yield break; }
        foreach (Text T in SecondsLeft)
        {
            if (T.transform.root.gameObject.activeSelf)
            {
                T.transform.parent.gameObject.SetActive(true);
                T.text = "3 Seconds Left";
            }
        }
        yield return new WaitForSeconds(1);
        if (round != r) { yield break; }
        foreach (Text T in SecondsLeft)
        {
            if (T.transform.root.gameObject.activeSelf)
            {
                T.transform.parent.gameObject.SetActive(true);
                T.text = "2 Seconds Left";
            }
        }
        canvasPlayer1.transform.Find("Emotes").GetComponent<Emotes>().Back();
        canvasPlayer1.transform.Find("EmotesController").GetComponent<Button>().interactable = false;
        EmotesMotherClass.OK = false;
        yield return new WaitForSeconds(1);
        if (round != r) { yield break; }
        foreach (Text T in SecondsLeft)
        {
            if (T.transform.root.gameObject.activeSelf)
            {
                T.transform.parent.gameObject.SetActive(true);
                T.text = "1 Seconds Left";
            }
        }
        yield return new WaitForSeconds(1);
        if (round == r) forceEndRound();
    }

    protected override void ResetRound(bool Start = false)
    {
        if ((Tutorial || PlayerPrefs.GetInt("BotOnline") == 1)  && !Start)
        {
            foreach (Text T in SecondsLeft)
                T.transform.parent.gameObject.SetActive(false);
        }
        if (!Start && botActive)
        {
            bot.UpdateBot(); canvasPlayer1.transform.Find("EmotesController").GetComponent<Button>().interactable = true;
            EmotesMotherClass.OK = true;
        }

            base.ResetRound(Start);

        if (!Start)
        {         
            if (botActive)
            {
                foreach (GameObject but in bot.Abilities)
                {
                    but.GetComponent<Shape_Abilities>().Disabled();
                    but.GetComponent<Shape_Abilities>().updateState();
                }
            }
            else{
                for (int k = 0; k < 6; k++)
                {
                    Shape_Abilities cur = canvasPlayer2.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                    if (cur == null) { continue; }
                    cur.Disabled(); cur.updateState();
                }
                if (canvasPlayer2.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>())
                    canvasPlayer2.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>().Disabled();
                canvasPlayer2.transform.Find("Attack").GetComponentInChildren<Shape_Abilities>().Disabled();
            }

            //PASSIVES

            if (StartRoundCountFreeze2 != 0)
            {
                if (round - StartRoundCountFreeze2 < PassiveStats[2][PassivesArray[2] - 1])
                {
                    if (botActive)
                    {
                        foreach (GameObject but in bot.Abilities)
                            but.GetComponent<Shape_Abilities>().DisableAll();
                    }
                    else
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            Shape_Abilities cur = canvasPlayer2.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                            if (cur == null) { continue; }
                            cur.DisableAll();
                        }
                        canvasPlayer2.transform.Find("Attack").GetComponentInChildren<Shape_Abilities>().DisableAll();
                    }
                }
            }

            if (StartRoundCountStuckInPlace2 != 0)
            {
                if (round - StartRoundCountStuckInPlace2 < PassiveStats[0][PassivesArray[0] - 1])
                {
                    if (botActive)
                    {
                        foreach (GameObject but in bot.Abilities)
                            but.GetComponent<Shape_Abilities>().DisableEscapes();
                    }
                    else
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            Shape_Abilities cur = canvasPlayer2.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                            if (cur == null) { continue; }
                            cur.DisableEscapes();
                        }
                    }
                }
            }
        }

        if (!Start)
            round++;

        if (Tutorial && !Start)
        {
            StartCoroutine(TutorialFunction(round));
        }

        canvasPlayer2.transform.Find("EnergyPoints").GetChild(shapeID2).GetComponentInChildren<Show_EP>().UpdateEP();

        if (PlayerPrefs.GetInt("BotOnline") == 1)
            StartCoroutine(waitRound(round));
    }

    public override void GameOver()
    {
        FightCanvas.SetActive(false);
        canvasPlayer1.gameObject.SetActive(false);
        canvasPlayer2.gameObject.SetActive(false);
        if (background == 1)
        {
            Backgrounds[1].transform.Find("UP").Find("BackEnd1").gameObject.SetActive(true);
            Backgrounds[1].transform.Find("UP").Find("BackEnd2").gameObject.SetActive(true);
        }

        turnOffPassives(true);
        turnOffPassives(false);

        //To remove the glow light
        player1.SetEP(0);
        player2.SetEP(0);

        if(player1Life <= 0 && player2Life <= 0)
        {
            playerWon = 0;
            cameraFight.gameObject.SetActive(true);
            animatorPlayer1.SetBool("Win", true);
            animatorPlayer2.SetBool("Win", true);
        }
        else if (player1Life <= 0)
        {
            playerWon = 2;
            cameraFight.gameObject.SetActive(false);
            cameraPlayer2.gameObject.SetActive(true);
            cameraPlayer2.GetComponent<Animator>().SetBool("End", true);
            animatorPlayer1.SetBool("Hit", true);
            animatorPlayer2.SetBool("Win", true);
        }
        else
        {
            playerWon = 1;
            cameraFight.gameObject.SetActive(false);
            cameraPlayer1.gameObject.SetActive(true);
            cameraPlayer1.GetComponent<Animator>().SetBool("End", true);
            animatorPlayer2.SetBool("Hit", true);
            animatorPlayer1.SetBool("Win", true);
        }

        if (PlayerPrefs.GetInt("BotOnline") == 1)
        {
            int shapeID = PlayerPrefs.GetInt("ShapeSelectedID");
            string lastDeck = PlayerPrefs.GetString("Username") + "," + shapeID.ToString() + "," + String.Join(",", PlayerPrefsX.GetIntArray("ActiveAbilityID" + shapeID.ToString())) + "," + PlayerPrefs.GetInt("SpecialAbilityID" + shapeID.ToString()).ToString();
            ClientTCP.PACKAGE_DEBUG("LastDeck", lastDeck);

            ClientTCP.PACKAGE_ENDGAME(1, PlayerPrefs.GetString("Username"), TempOpponent.Opponent.Username, TempOpponent.Opponent.ConnectionID, shapeID1, shapeID2, Mode == 1, playerWon == 0, playerWon,TempOpponent.Opponent.OpPP);
            AddReplay(TempOpponent.Opponent.Username, shapeID1, shapeID2, string.Join(",", ChoicesP1), string.Join(",", ChoicesP2), string.Join(",", ProbsP1), string.Join(",", ProbsP2), string.Join(",", AbLevelArray), string.Join(",", TempOpponent.Opponent.AbLevelArray), string.Join(",", Super100), string.Join(",", TempOpponent.Opponent.Super100), string.Join(",", Super200), string.Join(",", TempOpponent.Opponent.Super200), string.Join(",", PassivesArray), string.Join(",", TempOpponent.Opponent.Passives), playerWon == 1, false, playerWon == 0);
            #region BotProfiles
            string[] PP = new string[4] { "0", "0", "0", "0" };
            if (TempOpponent.Opponent.OpPP < 0)
            {
                TempOpponent.Opponent.OpPP = 0;
            }
            PP[shapeID2] = TempOpponent.Opponent.OpPP.ToString();
            string[] Lvl = new string[4] { "0", "0", "0", "0" };
            Lvl[shapeID2] = TempOpponent.Opponent.OpLvl.ToString();
            int[] botAb = new int[6] { bot.selectedIDs[0], bot.selectedIDs[1], bot.selectedIDs[2], bot.selectedIDs[3], bot.selectedIDs[4], bot.selectedIDs[5] };
            lastDeck = shapeID2.ToString() + "," + string.Join(",", botAb) + "," + bot.selectedIDs[6].ToString();
            List<string> buffer = new List<string>();
            buffer.Add((-1).ToString());
            buffer.Add(TempOpponent.Opponent.Username);
            buffer.Add(string.Join(",",PP));
            buffer.Add(lastDeck);
            buffer.Add((TempOpponent.Opponent.OpPP + R.Next(0,10)).ToString());
            buffer.Add(string.Join(",", TempOpponent.Opponent.AbLevelArray));
            buffer.Add(string.Join(",", TempOpponent.Opponent.Super100));
            buffer.Add(string.Join(",", TempOpponent.Opponent.Super200));
            buffer.Add(string.Join(",", TempOpponent.Opponent.Passives));
            buffer.Add(string.Join(",", Lvl));
            buffer.Add("0");
            buffer.Add(playerWon == 1 ? "0" : R.Next(1,5).ToString());
            buffer.Add(R.Next(4,9).ToString());
            string[] OldProfiles = PlayerPrefsX.GetStringArray("BotProfiles");
            if (OldProfiles.Length == 0)
                PlayerPrefsX.SetStringArray("BotProfiles", new string[1] { string.Join("|", buffer) });
            else
            {
                string[] newProfiles = new string[OldProfiles.Length + 1];
                int i = 0;
                foreach(string s in OldProfiles)
                {
                    newProfiles[i] = s;
                    i++;
                }
                newProfiles[newProfiles.Length - 1] = string.Join("|", buffer);
                PlayerPrefsX.SetStringArray("BotProfiles", newProfiles);
            }
            #endregion

        }

        StartCoroutine(endC());
    }

    IEnumerator endC()
    {
        yield return new WaitForSeconds(1.5f);
        if (!Tutorial)
        {
            if (PlayerPrefs.GetInt("BotOnline") == 1)
            {
                int goldGained = 0, rbGained = 0, xpGained = 0;
                if (playerWon == 1) { goldGained = 50; rbGained = 5; xpGained = 50; }
                else if (playerWon == 0) { goldGained = 25; rbGained = 2; xpGained = 25; }
                else if (playerWon == 2) { goldGained = 15; xpGained = 5; }

                int pp = PlayerPrefsX.GetIntArray("PP")[shapeID1];
                float add = UnityEngine.Random.Range(0, 1f) * pp / 2.0f;
                goldGained += (int)add; rbGained += (int)(add / 10);

                int curGold = PlayerPrefs.GetInt("Gold");
                int curRB = PlayerPrefs.GetInt("Redbolts");
                int[] curXP = PlayerPrefsX.GetIntArray("XP");

                curXP[shapeID1] += xpGained;
                curRB += rbGained;
                curGold += goldGained;
                PlayerPrefs.SetInt("Gold", curGold);
                PlayerPrefs.SetInt("Redbolts", curRB);
                PlayerPrefsX.SetIntArray("XP", curXP);
                PlayerPrefs.Save();

                try
                {
                    bool b = endGame.GetComponent<Endgame>().shapeLevelUp();
                    if (!b) ClientTCP.PACKAGE_ChestOpening();
                }
                catch (System.Exception e)
                {
                    endGame.GetComponent<Endgame>().ErrorOccured.gameObject.SetActive(true);
                    print(e);
                }
                endGame.gameObject.SetActive(true);
                endGame.GetComponent<Endgame>().BotOnline = true;
                endGame.GetComponent<Endgame>().UpdateEnd(goldGained, rbGained, xpGained);
            }
            else
            {
                endGame.gameObject.SetActive(true);
                endGame.GetComponent<Endgame>().UpdateEnd(0, 0, 0);
            }
        }
        else
        {
            StartCoroutine(TutorialFunction(11));
        }
        PlayerPrefs.SetInt("BotOnline", 0);
    }

    private IEnumerator TutorialFunction(int RoundNum)
    {
        if(RoundNum == 1)
        {
            player2.SetLife(70);
            #region GettingCanvas1Stuff
            for (int i = 1; i< 7; i++)
            {
                AbilitiesCanvas1[i-1] = canvasPlayer1.transform.Find("Ability" + i.ToString()).GetComponentInChildren<Button>();
                AbilitiesCanvas1[i-1].interactable = false;
            }
            Get_EPs[0] = canvasPlayer1.transform.Find("Get_EP").GetComponent<Button>();
            Get_EPs[0].interactable = false;
            Attacks[0] = canvasPlayer1.transform.Find("Attack").Find("AttackCube").GetComponent<Button>();
            Attacks[0].interactable = false;
            Supers[0] = canvasPlayer1.transform.Find("AllSpecialAbilities").Find("Cube Crash").GetComponent<Button>();
            Supers[0].interactable = false;
            ContinueButtons[0] = TutorialTexts[0].transform.parent.GetComponentInChildren<Button>();
            #endregion

            #region Getting Canvas2 Stuff
            for (int i = 1; i < 7; i++)
            {
                AbilitiesCanvas2[i - 1] = canvasPlayer2.transform.Find("Ability" + i.ToString()).GetComponentInChildren<Button>();
                AbilitiesCanvas2[i - 1].interactable = false;
                AbilitiesCanvas2[i - 1].GetComponent<Shape_Abilities>().player = player2;
            }
            Get_EPs[1] = canvasPlayer2.transform.Find("Get_EP").GetComponent<Button>();
            Get_EPs[1].interactable = false;
            Get_EPs[1].GetComponent<Shape_Abilities>().player = player2;
            Attacks[1] = canvasPlayer2.transform.Find("Attack").Find("AttackSphere").GetComponent<Button>();
            Attacks[1].interactable = false;
            Attacks[1].GetComponent<Shape_Abilities>().player = player2;
            Supers[1] = canvasPlayer2.transform.Find("AllSpecialAbilities").Find("Black Hole").GetComponent<Button>();
            Supers[1].interactable = false;
            Supers[1].GetComponent<Shape_Abilities>().player = player2;
            ContinueButtons[1] = TutorialTexts[1].transform.parent.GetComponentInChildren<Button>();
            #endregion

            #region TutorialStuff
            TutorialTexts[0].text = "Welcome To Shapes Clash ! We are thankful that you decided to give our game a try, and we hope you'll enjoy it!\n" +
                "Here's a little tutorial to introduce you to our game, you can skip ahead anytime by clicking the 'Skip Tutorial' Button on your top right.";
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].text = "Shapes Clash's main gamemode is 1v1, where 2 Players battle it out for glory!\n"+
                "Each player chooses a Shape to represent them in battle and then chooses the Shape's abilities.\n" +
                "Shapes use many abilities to attack , defend or put themselves in an advantageous position";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 0);
            Get_EPs[0].interactable = true;
            Get_EPs[1].GetComponent<Get_EP>().UseAbility();
            #endregion
        }
        else if(RoundNum == 2)
        {
            Hands[0].gameObject.SetActive(false);
            Get_EPs[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "During this round, both players used the GetEP ability.\n" +
                "This ability is common to all Shapes and grants a shape an additional Energy Point the round it uses it.\n" +
                "Energy Points are needed to use most abilities. The cost varies per Ability.\n" +
                "Moreover, a player automatically gains 1EP per round from round 1 to 5, 2EP from round 5 to 10, 3EP from round 10 to 15 and 4EP from round 15 onwards.";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", -1);
            Attacks[0].interactable = true;
            Get_EPs[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 3)
        {
            Hands[0].gameObject.SetActive(false);
            Attacks[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "The ability you used this prior round is simply called Attack.\n" +
                "It's an ability common to all shapes and costs 0 EP. This Attack Ability has the direction 'Straight'.\n" +
                "There are 3 different directions in Shapes Clash : Straight, Above and Below.\n" +
                "Each one is independant from the other, therefore they won't ever collide.";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Attacks[0].interactable = true;
            Attacks[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 4)
        {
            Hands[0].gameObject.SetActive(false);
            Attacks[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "Both Shapes used Attack last round. Both attacks had the same direction (Straight) so they collided.\n" +
                "Moreover, they both had the same Attack Power (10), nullifying each other.\n" +
                "In a collision, the strongest attack will always win, it will however be weakened by the other attack.";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Attacks[0].interactable = true;
            AbilitiesCanvas2[3].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 5)
        {
            Hands[0].gameObject.SetActive(false);
            Attacks[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "That was quite an attack by your opponent! You launched a straight attack, and your opponent launched an attack " +
                "from Above, resulting in both of you dealing damage independently!\n" +
                "However, their attack was stronger, dealing more damage.\n" +
                "The attack they used is called Toxic Hammer and is a specific ability for the sphere!\n" +
                "Let's fight back now, shall we?";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 6);
            AbilitiesCanvas1[5].interactable = true;
            AbilitiesCanvas2[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 6)
        {
            Hands[0].gameObject.SetActive(false);
            AbilitiesCanvas1[5].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "Well that was quite an attack! You just used \"Cube Sealing\"; it is an attack from below specific only to the " +
                "Cube.\nHowever what makes this attack so special is that it also has a defensive component!\nIt defends you against straight attacks from your opponent " +
                "while also dishing damage from Below!\nThe ability your opponent used is called Poisonous Air and boosts their attack for next round!";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", -1);
            Attacks[0].interactable = true;
            Attacks[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 7)
        {
            Hands[0].gameObject.SetActive(false);
            Attacks[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "And here's why boosting abilities are so useful.\nThey allowed your opponent to power up their own weak attack by 15 " +
                "and therefore overpower your own attack which would have normally nullified theirs.\n" +
                "This is all you need to know about attacking! Let's focus on defense now!";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 1);
            AbilitiesCanvas1[0].interactable = true;
            Attacks[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 8)
        {
            Hands[0].SetInteger("ID", -1);
            Hands[0].gameObject.SetActive(false);
            AbilitiesCanvas1[0].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "The Ability you just used is called Shield and is specific to the Cube!\n" +
                "It defends you from Straight and above Attacks with a defense power of 15 at level 1. However, it leaves you wide open to attacks from below!\n" +
                "All Shields only shield you from 2 Directions,that's why planning in advance and predicting your opponent is crucial in Shapes Clash.\n" +
                "Now let's tackle the escaping component of the Game!";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 2);
            AbilitiesCanvas1[1].interactable = true;
            AbilitiesCanvas2[2].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 9)
        {
            Hands[0].SetInteger("ID", -1);
            Hands[0].gameObject.SetActive(false);
            AbilitiesCanvas1[1].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "Well that was a peaceful round! No damage was dealt, because your opponent used the ability Elevation " +
                "to escape in an upwards direction! Your attack being straight had no influence on him.\nHowever your attack, named Tackle and specific to the cube, " +
                "also has an escape component to it!\nIt allows you to escape in the straight direction, and therefore evade any attacks from other directions!";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 3);
            AbilitiesCanvas1[2].interactable = true;
            AbilitiesCanvas2[0].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 10)
        {
            Hands[0].SetInteger("ID", -1);
            Hands[0].gameObject.SetActive(false);
            AbilitiesCanvas1[2].interactable = false;
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "Wise choice to heal! The ability you just used is called healing circle and is common to all shapes.\n" +
                "It's a support ability that spawns a healing circle lasting 3 rounds and healing you each round a bit!\n" +
                "Your opponent used Shadow attack, also common to all Shapes!\n" +
                "Shadow Attack allows a shape to attack and escape from below at the same time!\n" +
                "Now, Let's finish our opponent once and for all.";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            player1.SetEP(15);
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            Hands[0].gameObject.SetActive(true);
            Hands[0].SetInteger("ID", 7);
            Supers[0].interactable = true;
            Attacks[1].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if(RoundNum == 11)
        {
            canvasPlayer1.gameObject.SetActive(true);
            Hands[0].gameObject.SetActive(false);
            Supers[0].gameObject.SetActive(false);
            Get_EPs[0].gameObject.SetActive(false);
            Attacks[0].gameObject.SetActive(false);
            foreach(Button B in AbilitiesCanvas1)
            {
                B.gameObject.SetActive(false);
            }
            TutorialTexts[0].transform.parent.gameObject.SetActive(true);
            TutorialTexts[0].text = "WOW, that was quite a finish. The Ability you used just now is your majestic ability.\nEach Shape has two of them and must " +
                "choose which one to bring to battle. The difference between the two is that one can be escaped from above and one from below, so choose wisely!\nYour majestic ability will make your opponent unable to move and deal massive damage unless he escapes or uses his own majestic ability." +
                "It however costs a lot of EP.\n" +
                "Don't forget to check out the shop and buy some new abilities! Always try to have at least one attack for each direction.\n" +
                "Finally, in case you ever delete the game and want to recover your account, you're going to need to following recovery Code : " + PlayerPrefs.GetString("SecretCode") + "\n" +
                "Save the code in a sure place, because if you forget it, there is no other way to recover your account. You can also find the code in the settings tab in the main menu.\n" +
                "Congratulations on completing the Tutorial, we look forward to seeing you dominate in Shapes Clash!";
            ContinueButtons[0].interactable = true;
            yield return new WaitUntil(() => Continue);
            Continue = false;
            PlayerPrefs.SetInt("ShapeSelectedID", TutorialShapeIDs[0]);
            PlayerPrefs.SetInt("2ShapeSelectedID", TutorialShapeIDs[1]);
            TutorialTexts[0].transform.parent.gameObject.SetActive(false);
            LoadMainMenu();
        }
    }
    
    public void SetContinue()
    {
        Continue = true;
        ContinueButtons[0].interactable = false;
    }
    public void LoadMainMenu()
    {
        Tutorial = false;
        Shape_Abilities.Tutorial = false;
        PlayerPrefs.SetInt("ShapeSelectedID", TutorialShapeIDs[0]);
        PlayerPrefs.SetInt("2ShapeSelectedID", TutorialShapeIDs[1]);
        SceneManager.LoadScene("MainMenuScene");
    }
}
 