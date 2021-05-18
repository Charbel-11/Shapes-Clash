using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//To be attached on Canvas2
public class Bot : MonoBehaviour {
    public GameObject AbilitiesContainer, specialAbilitiesContainer;

    //escapes are included in def
    //If in practice, check and remove locked AbilitiesDic (make a new array for cur)
    //If online, check that the AbilitiesDic used are coherant with the level of the bot
    public static int[] CubeAtck = { 3, 4, 5, 6, 10, 12, 26, 39, 40 };
    public static int[] CubeDef = { 2, 7, 11, 14, 54 };
    public static int[] CubeOthers = { 8, 25, 34, 9 };
    public static int[] CubeSpecial = { 101, 202 };

    public static int[] PyrAtck = { 4, 13, 19, 21, 27, 28, 35, 37, 38 };
    public static int[] PyrDef = { 7, 11, 16, 36, 54 };
    public static int[] PyrOthers = { 8, 18, 25, 34, 9 };
    public static int[] PyrSpecial = { 103, 203 };

    public static int[] StarAtck = { 4, 15, 17, 29, 33, 30, 31, 32 };
    public static int[] StarDef = { 7, 11, 24, 54, 55 };
    public static int[] StarOthers = { 8, 25, 34, 9 };
    public static int[] StarSpecial = { 102, 201 };

    public static int[] SphereAtck = { 4, 43, 44, 45, 46, 47, 50, 51, 52 };
    public static int[] SphereDef = { 7, 11, 49, 54, 56 };
    public static int[] SphereOthers = { 8, 25, 34, 48, 9 };
    public static int[] SphereSpecial = { 104, 204 };

    private float OppState = 0.5f;
    public static List<int> BoostAbilities = new List<int> { 26, 33, 37, 44, 34 };
    public static List<int> EscapeAbilities = new List<int> { 3, 4, 7, 17, 21, 54 };
    private List<int> OppAbilities = new List<int>();
    private int[] OppAtts, OppDefs, OppEsc = new int[3];

    private List<int> pAtck, pDef, pOthers, pSpecial;
    private List<WeightedID> botAb = new List<WeightedID>();

    public static List<int> Above = new List<int> { 7, 10, 13, 30, 32, 48, 50, 52 };
    public static List<int> Straight = new List<int> { 1, 2, 3, 5, 9, 11, 15, 16, 17, 21, 22, 23, 24, 27, 28, 29, 31, 35, 36, 39, 42, 43, 45, 47, 49, 55, 56 };
    public static List<int> Below = new List<int> { 4, 6, 12, 14, 19, 38, 40, 46, 51, 54 };

    private int[] curAtck, curDef, curOthers, curSpecial;
    private int shapeIndex;
    private GameMaster GM;
    public int[] selectedIDs;
    public Dictionary<WeightedID, GameObject> AbilitiesDic = new Dictionary<WeightedID, GameObject>();
    public GameObject[] Abilities;
    private bool Done = false;


    System.Random R = new System.Random();

    private List<int> usedIDs = new List<int>();

    //Make the shape choice on the selection screen if the bot option is selected
    //Same for online, prepare a random shape
    public void StartFunction()
    {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        shapeIndex = GM.shapeID2;

        Abilities = new GameObject[9];
        selectedIDs = new int[9];

        if (shapeIndex == 0) {
            curAtck = CubeAtck;
            curDef = CubeDef;
            curOthers = CubeOthers;
            curSpecial = CubeSpecial;
            selectedIDs[8] = 1;
        }
        else if (shapeIndex == 1)
        {
            curAtck = PyrAtck;
            curDef = PyrDef;
            curOthers = PyrOthers;
            curSpecial = PyrSpecial;
            selectedIDs[8] = 22;
        }
        else if (shapeIndex == 2)
        {
            curAtck = StarAtck;
            curDef = StarDef;
            curOthers = StarOthers;
            curSpecial = StarSpecial;
            selectedIDs[8] = 23;
        }
        else if (shapeIndex == 3)
        {
            curAtck = SphereAtck;
            curDef = SphereDef;
            curOthers = SphereOthers;
            curSpecial = SphereSpecial;
            selectedIDs[8] = 42;
        }

        if (GM.shapeID1 == 0)
        { pAtck = new List<int>(CubeAtck); pDef = new List<int>(CubeDef); pOthers = new List<int>(CubeOthers); pSpecial = new List<int>(CubeSpecial); }
        else if (GM.shapeID1 == 1)
        { pAtck = new List<int>(PyrAtck); pDef = new List<int>(PyrDef); pOthers = new List<int>(PyrOthers); pSpecial = new List<int>(PyrSpecial); }
        else if (GM.shapeID1 == 2)
        { pAtck = new List<int>(StarAtck); pDef = new List<int>(StarDef); pOthers = new List<int>(StarOthers); pSpecial = new List<int>(StarSpecial); }
        else if (GM.shapeID1 == 3)
        { pAtck = new List<int>(SphereAtck); pDef = new List<int>(SphereDef); pOthers = new List<int>(SphereOthers); pSpecial = new List<int>(SphereSpecial); }

        for (int i = 0; i < 7; i++) { selectedIDs[i] = -1; } selectedIDs[7] = 0;
        initializeAbilities();
    }

    public void initializeAbilities()
    {
        int attackNum, defNum, othersNum;

        int r = Random.Range(1, 11);
        if (r <= 5)
        {
            attackNum = 4; defNum = 1; othersNum = 1;
        }
        else
        {
            attackNum = 3; defNum = 2; othersNum = 1;
        }

        for (int i = 0; i < attackNum; i++)
        {
            bool newAb = false;
            while (!newAb)
            {
                r = Random.Range(0, curAtck.Length);
                newAb = true;
                for (int j = 0; j < 7; j++)
                {
                    if (selectedIDs[j] == curAtck[r]) { newAb = false; break; }
                }
            }
            selectedIDs[i] = curAtck[r];
            botAb.Add(new WeightedID(selectedIDs[i], 0));
        }

        for (int i = 0; i < defNum; i++)
        {
            bool newAb = false;
            while (!newAb)
            {
                r = Random.Range(0, curDef.Length);
                newAb = true;
                for (int j = 0; j < 7; j++)
                {
                    if (selectedIDs[j] == curDef[r]) { newAb = false; break; }
                }
            }
            selectedIDs[i + attackNum] = curDef[r];
            botAb.Add(new WeightedID(selectedIDs[i + attackNum], 1));
        }

        for (int i = 0; i < othersNum; i++)
        {
            bool newAb = false;
            while (!newAb)
            {
                r = Random.Range(0, curOthers.Length);
                newAb = true;
                for (int j = 0; j < 7; j++)
                {
                    if (selectedIDs[j] == curOthers[r]) { newAb = false; break; }
                }
            }
            selectedIDs[i + attackNum + defNum] = curOthers[r];
            botAb.Add(new WeightedID(selectedIDs[i + attackNum + defNum], 2));
        }

        r = Random.Range(0, curSpecial.Length);
        selectedIDs[6] = curSpecial[r];
        botAb.Add(new WeightedID(selectedIDs[6], 3));
        botAb.Add(new WeightedID(0, 2));
        botAb.Add(new WeightedID(selectedIDs[8], 0));

        for (int i = 0; i < 9; i++)
        {
            if (i == 6) { continue; }
            foreach (Transform go in AbilitiesContainer.transform)
            {
                if (go.GetComponent<Shape_Abilities>().ID == selectedIDs[i])
                {
                    Abilities[i] = go.gameObject;
                    AbilitiesDic.Add(botAb[i], go.gameObject);
                    go.GetComponent<Shape_Abilities>().Awake();
                    break;
                }
            }
        }

        foreach (Transform go in specialAbilitiesContainer.transform)
        {
            if (go.GetComponent<Shape_Abilities>().ID == selectedIDs[6])
            {
                Abilities[6] = go.gameObject;
                AbilitiesDic.Add(botAb[6], go.gameObject);
                go.GetComponent<Shape_Abilities>().Awake();
            }
        }

        if (GameMaster.botOnline)
        {
            int[] IDs = { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 6; i++) { IDs[i] = selectedIDs[i]; }
            int superID = selectedIDs[6];
            TempOpponent.Opponent.AbIDs = IDs;
            TempOpponent.Opponent.SuperID = superID;
        }
    }

    //Consider cases where an ability can't be chosen in the next rounds, maybe check if button is interactible instead
    public void makeChoice()
    {
        bool attack = Random.Range(0f, 1.25f) >= OppState;
        if (!attack)
            attack = CheckDefWeights(botAb);
        List<WeightedID> filteredList = FilterTypes(botAb, attack ? 0 : 1);
        int r = GetRandomIndex(filteredList.Count);
        GameObject G;
        AbilitiesDic.TryGetValue(filteredList[r], out G);
        G.GetComponent<Shape_Abilities>().updateState();
        int i = 0;
        while (i < filteredList.Count && (CheckEP(r, filteredList) || !G.GetComponent<Button>().interactable || (filteredList[r].type == 0) != attack) || checkifused(filteredList[r].ID))
        {
            i++; if(i >= filteredList.Count) { break; }
            r = GetRandomIndex(filteredList.Count - i) + i;
            AbilitiesDic.TryGetValue(filteredList[r], out G);
            G.GetComponent<Shape_Abilities>().updateState();
        }
        if (i >= filteredList.Count && !Abilities[8].GetComponent<Button>().interactable)
        {
            usedIDs.Add(0);
            Abilities[7].GetComponent<Shape_Abilities>().UseAbility();
        }
        else if (i >= filteredList.Count)
        {
            int ran = R.Next(7, 9);

            if (ran == 7)
                usedIDs.Add(0);
            else
                usedIDs.Add(Abilities[8].GetComponent<Shape_Abilities>().ID);

            Abilities[ran].GetComponent<Shape_Abilities>().UseAbility();
            
        }
        else
        {
            usedIDs.Add(filteredList[r].ID);
            G.GetComponent<Shape_Abilities>().UseAbility();
        }
    }

    private bool checkifused(int ID)
    {
        if (usedIDs.Count < 2)
            return false;

        return (usedIDs[usedIDs.Count - 1] == ID && usedIDs[usedIDs.Count - 2] == ID);
    }

    private int GetRandomIndex(int listsize)
    {
        int i = R.Next(0, 101);
        int j = 0;
        while(listsize - j > 0)
        {
            if (i < (10 + 15 * j))
                return listsize - (j + 1);
            j++;
        }
        return 0;
    }

    private bool CheckDefWeights(List<WeightedID> ListToCheck)
    {
        foreach(WeightedID i in ListToCheck)
        {
            if (i.Weight >= 1 && i.type == 1)
                return false;
        }
        return true;
    }

    private List<WeightedID> FilterTypes(List<WeightedID> IDs, int type)
    {
        List<WeightedID> newList = new List<WeightedID>();
        foreach(WeightedID i in IDs)
        {
            if (i.type == type || (i.type!= 1 && type == 0))
                newList.Add(i);
        }
        return newList;
    }

    public void UpdateBot()
    {
        if (0.5 <= OppState && OppState <= 1)
            OppState = 0.6f;
        else
            OppState = 0.4f;
        int ID = GM.player1.GetIdOfAnimUsed();
        if (!OppAbilities.Contains(ID))
            OppAbilities.Add(ID);
        OppState += (float)(-2.5 + GM.player1.GetEP()) / 20; OppState -= (float)(-5 + GM.player1.GetEP());
        foreach (int i in OppAbilities)
            if (!BoostAbilities.Contains(i))
                OppState += GetWeight(i);
        OppState += GetWeight(ID);
        if (GM.player2Life <= 50) { OppState += (float)(-GM.player2Life + 50) / 40; }
        if (GM.player1Life <= 50) { OppState -= (float)(-GM.player1Life + 50) / 40; }
        if (BoostAbilities.Contains(GM.player2.GetIdOfAnimUsed())) { OppState -= 1;}
        if(OppState < 0) { OppState = 0; }
        else if(OppState > 1) { OppState = 1; }

        foreach(WeightedID i in botAb)
        {
            i.Weight = 0;
            WeightID(i,OppAbilities);
        }
        botAb.Sort(WeightedID.CompareWeightedIDs);
    }

    private float GetWeight(int ID)
    {
        if (BoostAbilities.Contains(ID))
            return 0.5f;
        else if (pAtck.Contains(ID))
            return 0.1f;
        else if (pDef.Contains(ID))
            return -0.1f;
        else
            return 0;       
    }

    private void WeightID(WeightedID ID, List<int> Abilities)
    {
        if (ID.type == 3) { ID.Weight = 100; return; }
        if(ID.type == 2) { ID.Weight = 0; return; }
        if (BoostAbilities.Contains(ID.ID))
        {
            ID.Weight += (GM.player2Life - 40) / 10;
            return;
        }
        List<int> relevantIDs = FilterIDs(Abilities, ID);

        if(ID.type == 0)
        {
            int EscapePotential = 6;
            foreach(int i in relevantIDs)
            {
                int dir = FindDirection(i);
                if (dir == ID.direction && EscapeAbilities.Contains(i) && !pAtck.Contains(i))
                    ID.Weight += 3;
                else if (dir != ID.direction && EscapeAbilities.Contains(i) && !pAtck.Contains(i))
                { ID.Weight -= 2; EscapePotential -= 6; }
                else if (dir == ID.direction && EscapeAbilities.Contains(i) && pAtck.Contains(i))
                    { ID.Weight += 1; EscapePotential -= 2; }
                else if (dir != ID.direction && EscapeAbilities.Contains(i) && pAtck.Contains(i))
                    ID.Weight -= 1;
                else if (dir == ID.direction && pDef.Contains(i))
                    ID.Weight -= 2;
                else if (dir == ID.direction && pAtck.Contains(i))
                    ID.Weight -= 1;
                else
                    ID.Weight += 1;
            }
            AbilitiesDic.TryGetValue(ID, out GameObject G);
            ID.Weight += Mathf.CeilToInt((float)G.GetComponent<Shape_Abilities>().AttPow / 5);
            ID.Weight += EscapePotential;
        }
        else if(ID.type == 1 && !EscapeAbilities.Contains(ID.ID))
        {
            foreach(int i in relevantIDs)
            {
                int dir = FindDirection(i);
                if (dir == ID.direction)
                    ID.Weight += 2;
                else
                    ID.Weight -= 3;
            }
            AbilitiesDic.TryGetValue(ID, out GameObject G);
            ID.Weight += Mathf.CeilToInt((float)G.GetComponent<Shape_Abilities>().DefPow / 10);
        }
        else if(ID.type == 1 && EscapeAbilities.Contains(ID.ID))
        {
            foreach(int i in relevantIDs)
            {
                int dir = FindDirection(i);
                if (dir == ID.direction)
                    ID.Weight -= 1;
                else
                    ID.Weight += 2;
            }
        }

    }

    private List<int> FilterIDs(List<int> AbilitiesDic, WeightedID ID)
    {
        List<int> newList = new List<int>();
        foreach(int i in AbilitiesDic)
        {
            if (ID.type == 0 && (pAtck.Contains(i) || pDef.Contains(i) || EscapeAbilities.Contains(i)))
                newList.Add(i);
            else if (ID.type == 1  && (pAtck.Contains(i)))
                newList.Add(i);
        }
        return newList;
    }

    private bool CheckEP(int r, List<WeightedID> list)
    {
        if (list[r].ID < 100)
            return GameMaster.StatsArr[list[r].ID][6] > GM.player2.GetEP();
        else if (list[r].ID < 200)
            return GameMaster.Super100StatsArr[list[r].ID - 101][6] > GM.player2.GetEP();
        else
            return GameMaster.Super200StatsArr[list[r].ID - 201][6] > GM.player2.GetEP();
    }

    public static int FindDirection(int ID)
    {
        if (Bot.Above.Contains(ID))
            return 0;
        else if (Bot.Below.Contains(ID))
            return 1;
        else if (Bot.Straight.Contains(ID))
            return 2;
        else
            return -1;
    }
}
public class WeightedID
{
    public int ID,Weight,type,direction;
    //type: 0 Attack,1 Defense,2 Other, 3 Super
    public WeightedID(int ID,int type)
    {
        this.ID = ID;
        Weight = 0;
        this.type = type;
        direction = Bot.FindDirection(ID);
    }

    public static int CompareWeightedIDs(WeightedID ID1, WeightedID ID2)
    {
        if (ID1.Weight > ID2.Weight)
            return -1;
        else if (ID1.Weight < ID2.Weight)
            return 1;
        else
            return 0;
    }

    public override string ToString()
    {
        string name;
        if (ID >= 0 && ID <= 100)
            name = ShapeConstants.NamesArray[ID];
        else if (ID <= 200)
            name = ShapeConstants.Super100Names[ID - 101];
        else
            name = ShapeConstants.Super200Names[ID - 201];

        return name + " : " + Weight;
    }

}
