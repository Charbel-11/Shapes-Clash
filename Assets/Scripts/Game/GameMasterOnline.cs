using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
//using System.Diagnostics;

public class GameMasterOnline : GameMaster
{
    public bool v2, p1Selected, p1Selected2;
    private int leftAlive, leftAlive2;

    public GameObject endGame2;
    public int previousPP2;

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
    private int Wait = 0;

    private int[] LastID = new int[2] { 0, 0 };

    public GameObject Disc;

    // Has to be awake to deactivate the unused player before a script calls them
    protected override void Awake()
    {
        Online = true;
        Replay = Spectate = false;
        p1Selected = p1Selected2 = true; //needs to be really early because of Reconnect
        base.Awake();

        finalIDs1 = PlayerPrefsX.GetIntArray("ActiveAbilityID" + shapeID1.ToString());
        specialAbilityID1 = PlayerPrefs.GetInt("SpecialAbilityID" + shapeID1.ToString());

        EmotesID = PlayerPrefsX.GetIntArray("EmotesID");

        v2 = PlayerPrefs.GetInt("GameMode") == 2;

        player2.transform.Find("Design").Find("Cube").gameObject.SetActive(0 == TempOpponent.Opponent.SkinID);
        player2.transform.Find("Design").Find("Cube").GetComponent<MeshRenderer>().enabled = (0 == TempOpponent.Opponent.SkinID);
        for (int i = 1; i < 2; i++)
        {
            player2.transform.Find("Design").Find("Cube" + i.ToString()).gameObject.SetActive(i == TempOpponent.Opponent.SkinID);
        }

        if (v2)
        {
            finalIDs2 = PlayerPrefsX.GetIntArray("2ActiveAbilityID" + shapeID12.ToString());
            specialAbilityID2 = PlayerPrefs.GetInt("2SpecialAbilityID" + shapeID12.ToString());

            if (shapeID12 == 0)
                player12 = cubeP1;
            else if (shapeID12 == 1)
                player12 = pyramidP1;
            else if (shapeID12 == 2)
                player12 = starP1;
            else if (shapeID12 == 3)
                player12 = sphereP1;

            //Set up the correct skin
            player12.transform.Find("Design").Find("Cube").gameObject.SetActive(0 == skin);
            player12.transform.Find("Design").Find("Cube").GetComponent<MeshRenderer>().enabled = (0 == skin);
            for (int i = 1; i < 2; i++)
            {
                player12.transform.Find("Design").Find("Cube" + i.ToString()).gameObject.SetActive(i == skin);
            }

            if (shapeID22 == 0)
                player22 = cubeP2;
            else if (shapeID22 == 1)
                player22 = pyramidP2;
            else if (shapeID22 == 2)
                player22 = starP2;
            else if (shapeID22 == 3)
                player22 = sphereP2;

            player22.transform.Find("Design").Find("Cube").gameObject.SetActive(0 == TempOpponent.Opponent.SkinID2);
            player22.transform.Find("Design").Find("Cube").GetComponent<MeshRenderer>().enabled = (0 == TempOpponent.Opponent.SkinID2);
            for (int i = 1; i < 2; i++)
            {
                player22.transform.Find("Design").Find("Cube" + i.ToString()).gameObject.SetActive(i == TempOpponent.Opponent.SkinID2);
            }

            leftAlive = leftAlive2 = 2;

            previousPP2 = PlayerPrefsX.GetIntArray("PP")[shapeID12];
        }
        else { leftAlive = leftAlive2 = 1; }
    }

    protected override void Start()
    {
        base.Start();
        p1Name = PlayerPrefs.GetString("Username");
        p2Name = TempOpponent.Opponent.Username;

        canvasPlayer1.transform.Find("Switch").gameObject.SetActive(v2);
        canvasPlayer2.transform.Find("Switch").gameObject.SetActive(v2);
        if (v2)
        {
            canvasPlayer2.gameObject.SetActive(true);

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
                                child.transform.Find(namesS[i]).gameObject.SetActive(i == shapeID12);
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
                Ab.GetChild(i).gameObject.SetActive(i == shapeID12);
            }

            //For the special ability
            Ab = canvasPlayer2.transform.Find("AllSpecialAbilities");
            foreach (Transform child in Ab)
            {
                if (child.name == "AbDescription") { continue; }
                child.gameObject.SetActive(specialAbilityID2 == child.GetComponent<Shape_Abilities>().ID);
            }

            //For the energy point
            Transform ep = canvasPlayer2.transform.Find("EnergyPoints");
            for (int i = 0; i < 4; i++)
            {
                ep.GetChild(i).gameObject.SetActive(i == shapeID12);
            }

            for (int j = 1; j <= 6; j++)
                canvasPlayer2.transform.Find("Ability" + j.ToString()).Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(finalIDs2[j - 1]);

            canvasPlayer2.transform.Find("AllSpecialAbilities").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(specialAbilityID2);

            int[] attackID = { 1, 22, 23, 42 };
            canvasPlayer2.transform.Find("Attack").Find("AbDescription").GetComponent<DescriptionInGame>().setUpDescription(attackID[shapeID12]);

            canvasPlayer2.transform.Find("Get_EP").GetComponent<Shape_Abilities>().Awake();
            canvasPlayer2.gameObject.SetActive(false);
        }
        for (int i = 1; i < EmotesID.Length + 1; i++)
        {
            GameObject.Find("Emotes" + i).GetComponent<EmotesMotherClass>().ID = EmotesID[i - 1];
        }
        StartCoroutine(waitRound(round, Reconnect));
        Reconnect = false;
    }

    public void switchShape(bool send = true)
    {
        turnOffPassives(true);

        int tmp = shapeID1;
        shapeID1 = shapeID12;
        shapeID12 = tmp;

        player1.GetComponent<Animator>().SetInteger("ID", 53);
        p1Selected = !p1Selected;
        canvasPlayer1.gameObject.SetActive(false);
        if (shapeID1 == 2) { player12.transform.Find("Design").localPosition = new Vector3(-0.002948284f, -0.15f, 0.01100063f); }
        else { player12.transform.Find("Design").localPosition = new Vector3(0, 0, 0); }
        player12.gameObject.SetActive(true);
        player12.GetComponent<Animator>().SetInteger("ID", -1);
        if (shapeID1 == 1 || shapeID1 == 2 || shapeID1 == 3)
            player12.GetComponent<Animator>().SetBool("Player1", true);
        player1.SetEP(player1.GetEP() - 2);          //2 for the switch shape cost
        player12.SetEP(player1.GetEP());

        Transform MiniEnemy1 = player12.transform.Find("MiniEnemy");
        Transform MiniEnemy2 = player2.transform.Find("MiniEnemy");
        for (int i = 0; i < 4; i++)
        {
            MiniEnemy1.GetChild(i).gameObject.SetActive(i == shapeID2);
            MiniEnemy2.GetChild(i).gameObject.SetActive(i == shapeID1);
        }

        Shape_Player tempPlayer = player1;
        player1 = player12;
        player12 = tempPlayer;

        Canvas tempCanvas = canvasPlayer1;
        canvasPlayer1 = canvasPlayer2;
        canvasPlayer2 = tempCanvas;

        player1.SetIdOfAnimUSed(53);
        animatorPlayer1 = player1.GetComponent<Animator>();
        canvasPlayer1.transform.Find("EnergyPoints").GetChild(shapeID1).GetComponentInChildren<Show_EP>().UpdateEP();
        canvasPlayer1.transform.Find("EmotesController").GetComponent<Button>().GetComponent<Image>().color = ShapeConstants.bckdAbEPColor[shapeID1];

        updateLifeBars();

        if (send && GameMaster.Online && !GameMaster.Spectate && !GameMaster.Replay && gameObject.transform.root.name != "Player1" && gameObject.transform.root.name != "Player2")
            ClientTCP.PACKAGE_GiveChoice();

        StartCoroutine("SwitchTimer");
    }

    public void switchShape2()
    {
        turnOffPassives(false);

        int tmp = shapeID2;
        shapeID2 = shapeID22;
        shapeID22 = tmp;

        player2.GetComponent<Animator>().SetInteger("ID", 53);
        p1Selected2 = !p1Selected2;
        if (shapeID2 == 2) { player22.transform.Find("Design").localPosition = new Vector3(-0.002948284f, -0.15f, 0.01100063f); }
        else { player22.transform.Find("Design").localPosition = new Vector3(0, 0, 0); }
        player22.gameObject.SetActive(true);
        player22.GetComponent<Animator>().SetInteger("ID", -1);
        player2.SetEP(player2.GetEP() - 2);             //2 for the switch shape cost
        player22.SetEP(player2.GetEP());
        player22.SetConnectionID(player2.GetConnectionID());

        Transform MiniEnemy1 = player1.transform.Find("MiniEnemy");
        Transform MiniEnemy2 = player22.transform.Find("MiniEnemy");
        for (int i = 0; i < 4; i++)
        {
            MiniEnemy1.GetChild(i).gameObject.SetActive(i == shapeID2);
            MiniEnemy2.GetChild(i).gameObject.SetActive(i == shapeID1);
        }

        Shape_Player tempPlayer = player2;
        player2 = player22;
        player22 = tempPlayer;

        canvasPlayer1.transform.Find("Player2Life").GetComponent<LifeBar_ValueChange>().Start();
        canvasPlayer2.transform.Find("Player2Life").GetComponent<LifeBar_ValueChange>().Start();

        animatorPlayer2 = player2.GetComponent<Animator>();

        StartCoroutine("SwitchTimer2");
    }

    IEnumerator SwitchTimer()
    {
        yield return new WaitForSeconds(1f);
        player12.gameObject.SetActive(false);
        canvasPlayer1.gameObject.SetActive(true);

        if (!Spectate)
        {
            for(int k = 0; k < 6; k++)
            {
                Shape_Abilities cur = canvasPlayer1.transform.Find("Ability" + (k + 1).ToString()).GetComponentInChildren<Shape_Abilities>();
                if (cur == null) { continue; }
                cur.Disabled(); cur.updateState();
            }
            canvasPlayer1.transform.Find("Switch").GetComponent<Button>().interactable = false;
            if (canvasPlayer1.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>())
                canvasPlayer1.transform.Find("AllSpecialAbilities").GetComponentInChildren<Shape_Abilities>().Disabled();
        }
    }

    IEnumerator SwitchTimer2()
    {
        yield return new WaitForSeconds(1f);
        player22.gameObject.SetActive(false);
    }

    public override void TryEvaluating()
    {
        player1ChoiceDone = player1.GetChoiceDone();
        player2ChoiceDone = player2.GetChoiceDone();

        if (player1ChoiceDone && player2ChoiceDone)
        {
            if (StopEvaluating) { return; }
            StopEvaluating = true;
            StartCoroutine(EvaluateOutput());
            return;
        }

        if (player1ChoiceDone) { ChangeCameraAndCanvas(); }
    }

    void ChangeCameraAndCanvas()
    {
        canvasPlayer1.gameObject.SetActive(false);

        if (botActive)
        {
            bot.makeChoice();
            cameraPlayer1.gameObject.SetActive(false);
            cameraFight.gameObject.SetActive(true);
        }
    }

    protected override IEnumerator EvaluateOutput()
    {
        Wait = 0;
        DisconnectedFirstTime = false;

        ChoicesP1.Add(player1.GetIdOfAnimUsed());
        ChoicesP2.Add(player2.GetIdOfAnimUsed());

        canvasPlayer1.gameObject.SetActive(false);
        cameraPlayer1.gameObject.SetActive(false);

        yield return StartCoroutine(base.EvaluateOutput());

        //PASSIVES

        //For the star burn passive
        if (player1 is Star_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Star_Player>().CanBurn == true || player1.ProbPlusAtt > 0))
            {
                Stop1 = true;
                ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                StartCoroutine(WaitForProb(1, 1));
            }
        }
        if (player2 is Star_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Star_Player>().CanBurn == true || player2.ProbPlusAtt > 0))
            {
                Stop2 = true;
                if(Disconnected)
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"),true);

                StartCoroutine(WaitForProb(2, 1));
            }
        }
        //For turning off the FieryEyes
        if (player1.GetComponent<Shape_Player>().AdditionalDamage > 0)
        {
            player1.GetComponent<Shape_Player>().AdditionalDamage = 0;
            player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
            AddText(FightText,"The Attack Boost faded.");
        }
        if (player2.GetComponent<Shape_Player>().AdditionalDamage > 0)
        {
            player2.GetComponent<Shape_Player>().AdditionalDamage = 0;
            player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(false);
            AddText(FightText, "The Attack Boost faded.");
        }
        //For the Star Additional Attack passive
        if (player1 is Star_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Star_Player>().CanFieryEyes == true || player1.ProbPlusDef > 0))
            {
                if (Stop1)
                {
                    StartCoroutine(Fuckwhileloops(1));
                }
                else
                {
                    Stop3 = true;
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                    StartCoroutine(WaitForProb(1, 2));
                }
            }
        }
        if (player2 is Star_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Star_Player>().CanFieryEyes == true || player2.ProbPlusDef > 0))
            {
                if (Stop2)
                {
                    StartCoroutine(Fuckwhileloops(2));
                }
                else
                {
                    Stop4 = true;
                    if(Disconnected)
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);

                    StartCoroutine(WaitForProb(2, 2));
                }
            }
        }

        //For the cube stuck in place passive
        if (player1 is Cube_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Cube_Player>().CanStuckInPlace == true || player1.ProbPlusAtt > 0))
            {
                Stop5 = true;
                ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                StartCoroutine(WaitForProb(1, 3));
            }
        }
        if (player2 is Cube_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Cube_Player>().CanStuckInPlace == true || player2.ProbPlusAtt > 0))
            {
                Stop6 = true;
                if(Disconnected)
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);

                StartCoroutine(WaitForProb(2, 3));
            }
        }

        //For the cube protective earth passive; it stays enabled until the player is attacked
        if (player1 is Cube_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player1.ProbPlusDef > 0) && !protectiveEarthEffectP1)
            {
                if (Stop5)
                {
                    StartCoroutine(Fuckwhileloops(3));
                }
                else
                {
                    Stop7 = true;
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                    StartCoroutine(WaitForProb(1, 4));
                }
            }
        }
        if (player2 is Cube_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Cube_Player>().CanHaveProtectiveEarth == true || player2.ProbPlusDef > 0) && !protectiveEarthEffectP2)
            {
                if (Stop6)
                {
                    StartCoroutine(Fuckwhileloops(4));
                }
                else
                {
                    Stop8 = true;
                    if(Disconnected)
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);

                    StartCoroutine(WaitForProb(2, 4));
                }
            }
        }

        //For the pyramid freeze passive

        if (player1 is Pyramid_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Pyramid_Player>().CanFreeze == true || player1.ProbPlusAtt > 0))
            {
                Stop9 = true;
                ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                StartCoroutine(WaitForProb(1, 5));
            }
        }
        if (player2 is Pyramid_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Pyramid_Player>().CanFreeze == true || player2.ProbPlusAtt > 0))
            {
                Stop10 = true;
                if (Disconnected)
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                StartCoroutine(WaitForProb(2, 5));
            }
        }

        //for the Pyramid Snow Passive
        if (player1 is Pyramid_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Pyramid_Player>().CanSnow == true || player1.ProbPlusDef > 0))
            {
                if (Stop9)
                {
                    StartCoroutine(Fuckwhileloops(5));
                }
                else
                {
                    Stop11 = true;
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                    StartCoroutine(WaitForProb(1, 6));
                }
            }
        }
        if (player2 is Pyramid_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Pyramid_Player>().CanSnow == true || player2.ProbPlusDef > 0))
            {
                if (Stop10)
                {
                    StartCoroutine(Fuckwhileloops(6));
                }
                else
                {
                    Stop12 = true;
                    if (Disconnected)
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                    StartCoroutine(WaitForProb(2, 6));
                }
            }
        }

        //for the Sphere HelperSpheres Passive
        if (player1 is Sphere_Player)
        {
            if (netDamageTakenP2 > 0 && (player1.GetComponent<Sphere_Player>().CanHelperSpheres || player1.ProbPlusAtt > 0))
            {
                Stop13 = true;
                ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                StartCoroutine(WaitForProb(1, 7));
            }
        }
        if (player2 is Sphere_Player)
        {
            if (netDamageTakenP1 > 0 && (player2.GetComponent<Sphere_Player>().CanHelperSpheres || player2.ProbPlusAtt > 0))
            {
                Stop14 = true;
                if (Disconnected)
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                StartCoroutine(WaitForProb(2, 7));
            }
        }

        //For The Smoke Passive
        if (player1 is Sphere_Player)
        {
            if (netDamageTakenP1 > 0 && (player1.GetComponent<Sphere_Player>().CanSmoke || player1.ProbPlusDef > 0))
            {
                if (Stop13)
                {
                    StartCoroutine(Fuckwhileloops(7));
                }
                else
                {
                    Stop15 = true;
                    ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
                    StartCoroutine(WaitForProb(1, 8));
                }
            }
        }
        if (player2 is Sphere_Player)
        {
            if (netDamageTakenP2 > 0 && (player2.GetComponent<Sphere_Player>().CanSmoke || player2.ProbPlusDef > 0))
            {
                if (Stop14)
                {
                    StartCoroutine(Fuckwhileloops(8));
                }
                else
                {
                    Stop16 = true;
                    if (Disconnected)
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"),true);
                    StartCoroutine(WaitForProb(2, 8));
                }
            }
        }

        player1Life = player1.GetLife();
        player2Life = player2.GetLife();

        if (v2)
        {
            if (!v2CheckEndRound())
            {
                if (Stop1 || Stop2 || Stop3 || Stop4 || Stop5 || Stop6 || Stop7 || Stop8 || Stop9 || Stop10 || Stop11 || Stop12 || Stop13 || Stop14 || Stop15 || Stop16)
                    StartCoroutine(ResetV2());
                else
                    ResetRound();
            }
        }
        else
        {
            if (player1Life <= 0 || player2Life <= 0)
                GameOver();
            else if (Stop1 || Stop2 || Stop3 || Stop4 || Stop5 || Stop6 || Stop7 || Stop8 || Stop9 || Stop10 || Stop11 || Stop12 || Stop13 || Stop14 || Stop15 || Stop16)
                StartCoroutine(Reset());
            else
                ResetRound();
        }
    }

    bool v2CheckEndRound()
    {
        Debug.Log("Check: " + player1.GetLife() + " " + player2.GetLife());
        if (player1.GetLife() <= 0 && player12.GetLife() <= 0)
        {
            leftAlive = 0;
            GameOver();
            return true;
        }
        if (player2.GetLife() <= 0 && player22.GetLife() <= 0)
        {
            leftAlive2 = 0;
            GameOver();
            return true;
        }

        if (player1.GetLife() <= 0 || player12.GetLife() <= 0)
        {
            if (leftAlive == 2)
            {
                switchShape(false);
                canvasPlayer1.transform.Find("Switch").gameObject.SetActive(false);
                canvasPlayer2.transform.Find("Switch").gameObject.SetActive(false);
                leftAlive = 1;
            }
        }
        if (player2.GetLife() <= 0 || player22.GetLife() <= 0)
        {
            if (leftAlive2 == 2)
            {
                leftAlive2 = 1;
                switchShape2();
            }
        }
        return false;
    }

    private void forceEndRound()
    {
        if (player1.GetChoiceDone() == false)
        {
            canvasPlayer1.transform.Find("Get_EP").GetComponent<Get_EP>().UseAbility();
        }

        if(player2.GetChoiceDone() == false && Disconnected)
        {
            if (TempOpponent.Opponent.Abilities.TryGetValue(0, out TempOpponent.Method method))
                method.Invoke();
        }
    }

    IEnumerator waitRound(int r, bool reconnect = false)
    {
        if (reconnect) { yield return new WaitForSeconds(1); }
        else { yield return new WaitForSeconds(4); }
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
        if (v2) { canvasPlayer1.transform.Find("Switch").GetComponent<Button>().interactable = false; }
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
        foreach (Text T in SecondsLeft)
            T.transform.parent.gameObject.SetActive(false);

        base.ResetRound(Start);

        if (v2)
        {
            canvasPlayer1.transform.Find("Switch").GetComponent<Button>().interactable = player1.GetEP() >= 2;
            canvasPlayer2.transform.Find("Switch").GetComponent<Button>().interactable = player1.GetEP() >= 2;
        }
        canvasPlayer1.transform.Find("EmotesController").GetComponent<Button>().interactable = true;
        EmotesMotherClass.OK = true;

        //ClientTCP.PACKAGE_DEBUG("Resetting for Player 1 with shapeID" + shapeID1);

        if (!Start)
            round++;
        StopEvaluating = false;
        DisconnectedFirstTime = false;
        LastID = new int[2] { 0, 0 };
        StartCoroutine(waitRound(round));
    }

    public override void GameOver()
    {
        /*
        StackTrace stackTrace = new StackTrace();
        print(stackTrace.GetFrame(0).GetMethod().Name + " called GameOver");
        print(stackTrace.GetFrame(1).GetMethod().Name + " called GameOver");
        if (stackTrace.GetFrame(2) != null)
            print(stackTrace.GetFrame(2).GetMethod().Name + " called GameOver");
        if (stackTrace.GetFrame(3) != null)
            print(stackTrace.GetFrame(3).GetMethod().Name + " called GameOver");
            */

        FightCanvas.SetActive(false);
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

        int shapeID = PlayerPrefs.GetInt("ShapeSelectedID");
        string lastDeck = PlayerPrefs.GetString("Username") + "," + shapeID.ToString() + "," + String.Join(",", PlayerPrefsX.GetIntArray("ActiveAbilityID" + shapeID.ToString())) + "," + PlayerPrefs.GetInt("SpecialAbilityID" + shapeID.ToString()).ToString();
        ClientTCP.PACKAGE_DEBUG("LastDeck", lastDeck);

        if (player1Life <= 0 && player2Life <= 0)
        {
            playerWon = 0;
            cameraFight.gameObject.SetActive(true);
            animatorPlayer1.SetBool("Win", true);
            animatorPlayer2.SetBool("Win", true);
            int GameMode = v2 ? 2 : 1;
            if (TempOpponent.Opponent.ConnectionID2 > TempOpponent.Opponent.ConnectionID)
            {
                ClientTCP.PACKAGE_ENDGAME(GameMode, PlayerPrefs.GetString("Username"), TempOpponent.Opponent.Username, TempOpponent.Opponent.ConnectionID, shapeID1, shapeID2, Mode==1, true);
            }
            AddReplay(TempOpponent.Opponent.Username, shapeID1, shapeID2, string.Join(",", ChoicesP1), string.Join(",", ChoicesP2), string.Join(",", ProbsP1), string.Join(",", ProbsP2), string.Join(",", AbLevelArray), string.Join(",", TempOpponent.Opponent.AbLevelArray), string.Join(",", Super100), string.Join(",", TempOpponent.Opponent.Super100), string.Join(",", Super200), string.Join(",", TempOpponent.Opponent.Super200), string.Join(",", PassivesArray), string.Join(",", TempOpponent.Opponent.Passives),false, Mode == 1, true);
        }
        else if (player1Life <= 0)
        {
            playerWon = 2;
            AddReplay(TempOpponent.Opponent.Username, shapeID1, shapeID2, string.Join(",", ChoicesP1), string.Join(",", ChoicesP2), string.Join(",", ProbsP1), string.Join(",", ProbsP2), string.Join(",", AbLevelArray), string.Join(",", TempOpponent.Opponent.AbLevelArray), string.Join(",", Super100), string.Join(",", TempOpponent.Opponent.Super100), string.Join(",", Super200), string.Join(",", TempOpponent.Opponent.Super200), string.Join(",", PassivesArray), string.Join(",", TempOpponent.Opponent.Passives), false,Mode == 1, false);
            animatorPlayer1.SetBool("Hit", true);
            animatorPlayer2.SetBool("Win", true);
        }
        else
        {
            int GameMode = v2 ? 2 : 1;

            ClientTCP.PACKAGE_ENDGAME(GameMode, PlayerPrefs.GetString("Username"), TempOpponent.Opponent.Username, TempOpponent.Opponent.ConnectionID, shapeID1, shapeID2, Mode == 1, false);
            AddReplay(TempOpponent.Opponent.Username, shapeID1, shapeID2, string.Join(",", ChoicesP1), string.Join(",", ChoicesP2), string.Join(",", ProbsP1), string.Join(",", ProbsP2), string.Join(",", AbLevelArray), string.Join(",", TempOpponent.Opponent.AbLevelArray), string.Join(",", Super100), string.Join(",", TempOpponent.Opponent.Super100), string.Join(",", Super200), string.Join(",", TempOpponent.Opponent.Super200), string.Join(",", PassivesArray), string.Join(",", TempOpponent.Opponent.Passives),true, Mode == 1, false);
            playerWon = 1;
            animatorPlayer2.SetBool("Hit", true);
            animatorPlayer1.SetBool("Win", true);
        }
        cameraFight.gameObject.SetActive(false);
        cameraPlayer1.gameObject.SetActive(true);
        cameraPlayer1.GetComponent<Animator>().SetBool("End", true);

        if (Mode == 1)
        {
            TempOpponent.Opponent.FriendlyBattle = false;
            TempOpponent.Opponent.Accepting = false;
        }
        //TempOpponent.Opponent.Reset();

        StartCoroutine(endC());
    }

    IEnumerator endC()
    {
        yield return new WaitForSeconds(1.5f);
        endGame.gameObject.SetActive(true);

        if (Mode != 1)  
        {
            if (v2) endGame2.gameObject.SetActive(true);
            int goldGained = 0, rbGained = 0, xpGained = 0;
            if (playerWon == 1) { goldGained = 50; rbGained = 5; xpGained = 50; }
            else if (playerWon == 0) { goldGained = 25; rbGained = 2; xpGained = 25; }
            else if (playerWon == 2) { goldGained = 15; xpGained = 5; }

            int curGold = PlayerPrefs.GetInt("Gold");
            int curRB = PlayerPrefs.GetInt("Redbolts");
            int[] curXP = PlayerPrefsX.GetIntArray("XP");

            curXP[shapeID1] += xpGained;
            if (v2) curXP[shapeID12] += xpGained;
            curGold += goldGained;
            curRB += rbGained;
            if (v2) { curGold += goldGained; curRB += rbGained; }
            PlayerPrefs.SetInt("Gold", curGold);
            PlayerPrefs.SetInt("Redbolts", curRB);
            PlayerPrefsX.SetIntArray("XP", curXP);
            PlayerPrefs.Save();

            try
            {
                bool b = endGame.GetComponent<Endgame>().shapeLevelUp();
                if (!b) ClientTCP.PACKAGE_ChestOpening();
            }
            catch (Exception e)
            {
                endGame.GetComponent<Endgame>().ErrorOccured.gameObject.SetActive(true);
                print(e);
            }

            endGame.GetComponent<Endgame>().UpdateEnd(goldGained, rbGained, xpGained);
            if (v2) endGame2.GetComponent<Endgame>().UpdateEnd(goldGained, rbGained, xpGained);   
            if (v2) endGame2.gameObject.SetActive(false);
        }
        else  //For friendly games
        {
            endGame.GetComponent<Endgame>().UpdateEnd(0, 0, 0);
        }
    }

    IEnumerator WaitForProb(int Player, int ID)
    {
        if (Player == 1)
        {
            yield return new WaitUntil(() => TempOpponent.Opponent.GotProb1 == true);
            TempOpponent.Opponent.GotProb1 = false;
            ProbsP1.Add(TempOpponent.Opponent.Probability);
            if (ID == 1)
            {
                if (TempOpponent.Opponent.Probability <= (((Star_Player)player1).CanBurn ? PassiveStats[4][PassivesArray[4] + 2]+player1.ProbPlusAtt : player1.ProbPlusAtt))
                {
                    StartRoundCountBurn2 = round;
                    player2.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop1 = false;
            }
            else if (ID == 2)
            {
                if (TempOpponent.Opponent.Probability <= (((Star_Player)player1).CanFieryEyes ? PassiveStats[5][PassivesArray[5] + 2] + player1.ProbPlusDef : player1.ProbPlusDef))
                {
                    player1.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player1.AdditionalDamage = PassiveStats[5][PassivesArray[5] - 1];
                    AddText(FightText, "Player 1 got angry because he took damage and gained a power boost.");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop3 = false;
            }
            else if (ID == 3)
            {
                if (TempOpponent.Opponent.Probability <= (((Cube_Player)player1).CanStuckInPlace ? PassiveStats[0][PassivesArray[0] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt))
                {
                    StartRoundCountStuckInPlace2 = round;
                    player2.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 is stuck!");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop5 = false;
            }
            else if (ID == 4)
            {
                if (TempOpponent.Opponent.Probability <= (((Cube_Player)player1).CanHaveProtectiveEarth ? PassiveStats[1][PassivesArray[1] + 2] + player1.ProbPlusDef : player1.ProbPlusDef))
                {
                    player1.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP1 = true;
                    HardeningPow1 = PassiveStats[1][PassivesArray[1] - 1];
                    AddText(FightText, "Player 1 hardened himself");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop7 = false;
            }
            else if (ID == 5)
            {
                if (TempOpponent.Opponent.Probability <= (((Pyramid_Player)player1).CanFreeze ? PassiveStats[2][PassivesArray[2] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt))
                {
                    StartRoundCountFreeze2 = round;
                    animatorPlayer2.SetBool("Frozen", true);
                    AddText(FightText, "Player 2 is frozen for " + PassiveStats[2][PassivesArray[2] - 1]+" round(s)");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop9 = false;
            }
            else if (ID == 6)
            {
                if(TempOpponent.Opponent.Probability <= (((Pyramid_Player)player1).CanSnow ? PassiveStats[3][PassivesArray[3] + 2] + player1.ProbPlusDef : player1.ProbPlusDef))
                {
                    player1.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing1 = true;
                    AddText(FightText, "Player 1's call for help from the Weather Gods has been answered!");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop11 = false;
            }
            else if(ID == 7)
            {
                if (TempOpponent.Opponent.Probability <= (((Sphere_Player)player1).CanHelperSpheres ? PassiveStats[6][PassivesArray[6] + 2] + player1.ProbPlusAtt : player1.ProbPlusAtt))
                {
                    player1.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres1 = 1;
                    AddText(FightText, "Poison Spheres rushed to Player1's help!");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop13 = false;
            }
            else if(ID == 8)
            {
                if (TempOpponent.Opponent.Probability <= (((Sphere_Player)player1).CanSmoke ? PassiveStats[7][PassivesArray[7] + 2] + player1.ProbPlusDef : player1.ProbPlusDef))
                {
                    player1.GetComponent<Sphere_Player>().Smoke.SetActive(true);
                    DoitSmoke1 = true;
                    AddText(FightText, "Player 1 was surrounded by Fog!");
                }
                TempOpponent.Opponent.Probability = 0;
                Stop15 = false;
            }
        }
        else
        {
            yield return new WaitUntil(() => TempOpponent.Opponent.GotProb2 == true || TempOpponent.Opponent.GotProb21 == true || DisconnectedFirstTime);
            if (DisconnectedFirstTime)
            {
                bool GoGo = true;
                if (LastID[0] == 0)
                    LastID[0] = ID;
                else if (LastID[1] == 0 && LastID[0] != ID)
                    LastID[1] = ID;
                else
                    if (LastID[0] == ID || LastID[1] == ID)
                        GoGo = false;
                if (GoGo)
                {
                    if (ID == 1)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 1));
                    }
                    else if (ID == 2)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 2));
                    }
                    else if (ID == 3)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 3));
                    }
                    else if (ID == 4)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 4));
                    }
                    else if (ID == 5)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 5));
                    }
                    else if (ID == 6)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 6));
                    }
                    else if (ID == 7)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 7));
                    }
                    else if (ID == 8)
                    {
                        ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"), true);
                        StartCoroutine(WaitForProb(2, 8));
                    }
                    yield break;
                }                
            }
            else if (TempOpponent.Opponent.GotProb2)
            {
                TempOpponent.Opponent.GotProb2 = false;
                ProbsP2.Add(TempOpponent.Opponent.Probability2);
            }
            else
            {               
                TempOpponent.Opponent.GotProb21 = false;
                ProbsP2.Add(TempOpponent.Opponent.Probability21);
            }
            if (ID == 1)
            {
                if (TempOpponent.Opponent.Probability2 <= (((Star_Player)player2).CanBurn ? PassiveStats[4][TempOpponent.Opponent.Passives[4] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt))
                {
                    StartRoundCountBurn1 = round;
                    player1.transform.Find("BurnedFlame").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop2 = false;
            }
            else if (ID == 2)
            {
                int Range = ((Star_Player)player2).CanBurn ? PassiveStats[5][TempOpponent.Opponent.Passives[5] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range)
                {
                    player2.transform.Find("Design").transform.Find("FieryEyes").gameObject.SetActive(true);
                    player2.AdditionalDamage = PassiveStats[5][TempOpponent.Opponent.Passives[5] - 1];
                    AddText(FightText, "Player 2 got angry because he took damage and gained a power boost.");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop4 = false;
            }
            else if (ID == 3)
            {
                if (TempOpponent.Opponent.Probability2 <= (((Cube_Player)player2).CanStuckInPlace ? PassiveStats[0][TempOpponent.Opponent.Passives[0] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt))
                {
                    StartRoundCountStuckInPlace1 = round;
                    player1.transform.Find("GroundStuck").gameObject.SetActive(true);
                    AddText(FightText, "Player 2 was Burned by his Opponent's Fire Attack");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop6 = false;
            }
            else if (ID == 4)
            {
                int Range = ((Cube_Player)player2).CanHaveProtectiveEarth ? PassiveStats[1][TempOpponent.Opponent.Passives[1] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range)
                {
                    player2.GetComponent<Cube_Player>().ShieldAppearance.SetActive(true);
                    protectiveEarthEffectP2 = true;
                    HardeningPow2 = PassiveStats[1][TempOpponent.Opponent.Passives[1] - 1];
                    AddText(FightText, "Player 2 hardened himself");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop8 = false;
            }
            else if (ID == 5)
            {
                if (TempOpponent.Opponent.Probability2 <= (((Pyramid_Player)player2).CanFreeze ? PassiveStats[2][TempOpponent.Opponent.Passives[2] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt))
                {
                    StartRoundCountFreeze1 = round;
                    animatorPlayer1.SetBool("Frozen", true);
                    AddText(FightText, "Player 1 is frozen for " + PassiveStats[2][TempOpponent.Opponent.Passives[2] - 1]+" round(s)");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop10 = false;
            }
            else if (ID == 6)
            {
                int Range = ((Pyramid_Player)player2).CanSnow ? PassiveStats[3][TempOpponent.Opponent.Passives[3] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range)
                {
                    player2.GetComponent<Pyramid_Player>().Snow.SetActive(true);
                    Snowing2 = true;
                    AddText(FightText, "Player 2's call for help from the Weather Gods has been answered!");
                }
                TempOpponent.Opponent.Probability2 = 0;
                TempOpponent.Opponent.Probability21 = 0;
                Stop12 = false;
            }
            else if (ID == 7)
            {
                if (TempOpponent.Opponent.Probability2 <= (((Sphere_Player)player2).CanHelperSpheres ? PassiveStats[6][TempOpponent.Opponent.Passives[6] + 2] + player2.ProbPlusAtt : player2.ProbPlusAtt))
                {
                    player2.GetComponent<Sphere_Player>().HelperSpheres.SetActive(true);
                    HelperSpheres2 = 1;
                    AddText(FightText, "Poison Spheres rushed to Player2's help!");
                }
                TempOpponent.Opponent.Probability2 = 0;
                Stop14 = false;
            }
            else if (ID == 8)
            {
                int Range = ((Sphere_Player)player2).CanSmoke ? PassiveStats[7][TempOpponent.Opponent.Passives[7] + 2] + player2.ProbPlusDef : player2.ProbPlusDef;
                if (TempOpponent.Opponent.Probability2 <= Range || TempOpponent.Opponent.Probability21 <= Range)
                {
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
    IEnumerator Reset()
    {
        yield return new WaitUntil(() => (!Stop1 && !Stop2 && !Stop3 && !Stop4 && !Stop5 && !Stop6 && !Stop7 && !Stop8 && !Stop9 && !Stop10 && !Stop11 && !Stop12 && !Stop13 && !Stop14 && !Stop15 && !Stop16 && Wait == 0));
        if ((v2 && ((player1.GetLife() <= 0 && player12.GetLife() <= 0) || (player2.GetLife() <= 0 && player22.GetLife() <= 0))) || (!v2 && (player1Life <= 0 || player2Life <= 0)))
            GameOver();
        else
            ResetRound();
    }
    IEnumerator ResetV2()
    {
        yield return new WaitUntil(() => (!Stop1 && !Stop2 && !Stop3 && !Stop4 && !Stop5 && !Stop6 && !Stop7 && !Stop8 && !Stop9 && !Stop10 && !Stop11 && !Stop12 && !Stop13 && !Stop14 && !Stop15 && !Stop16 && Wait == 0));
        if (!v2CheckEndRound()) { ResetRound(); }
    }
    IEnumerator Fuckwhileloops(int ID)
    {
        Wait += 1;
        if (ID == 1)
        {
            yield return new WaitUntil(() => (!Stop1));
            Stop3 = true;
            ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
            StartCoroutine(WaitForProb(1, 2));
            Wait -= 1;
        }
        else if (ID == 2)
        {
            yield return new WaitUntil(() => (!Stop2));
            Stop4 = true;
            StartCoroutine(WaitForProb(2, 2));
            Wait -= 1;
        }
        else if (ID == 3)
        {
            yield return new WaitUntil(() => (!Stop5));
            Stop7 = true;
            ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
            StartCoroutine(WaitForProb(1, 4));
            Wait -= 1;
        }
        else if (ID == 4)
        {
            yield return new WaitUntil(() => (!Stop6));
            Stop8 = true;
            StartCoroutine(WaitForProb(2, 4));
            Wait -= 1;
        }
        else if (ID == 5)
        {
            yield return new WaitUntil(() => (!Stop9));
            Stop11 = true;
            ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
            StartCoroutine(WaitForProb(1, 6));
            Wait -= 1;
        }
        else if (ID == 6)
        {
            yield return new WaitUntil(() => (!Stop10));
            Stop12 = true;
            StartCoroutine(WaitForProb(2, 6));
            Wait -= 1;
        }
        else if(ID == 7)
        {
            yield return new WaitUntil(() => (!Stop13));
            Stop15 = true;
            ClientTCP.PACKAGE_GetProbability(player2.GetConnectionID(), PlayerPrefs.GetString("Username"));
            StartCoroutine(WaitForProb(1, 8));
            Wait -= 1;
        }
        else if (ID == 8)
        {
            yield return new WaitUntil(() => (!Stop14));
            Stop16 = true;
            StartCoroutine(WaitForProb(2, 8));
            Wait -= 1;
        }
    }

    private IEnumerator ReconnectIt(int OtherConnID)
    {
        yield return new WaitUntil(() => (!StopEvaluating));
        ClientTCP.PACKAGE_Reconnect(OtherConnID);
        Disconnected = false;
    }
    public void ReconnectFunction(int OtherConnID)
    {
        StartCoroutine(ReconnectIt(OtherConnID));
    }
}