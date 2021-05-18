using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfo : MonoBehaviour
{
    public GameObject AllAbilities;
    public GameObject AllSpecialAbilities;

    private string abLvl;
    private Text AbilityName;
    private Text EPNeeded;
    private Text Attack, Def, Pass;

    public Shape_Abilities curAb;
    private GameObject rarity;
    private int curRarity;
    private int curIdx;

    private bool ArrayContains(int[] Array, int i)
    {
        foreach (int j in Array)
        {
            if (i == j)
                return true;
        }
        return false;
    }


    public void updateInfo(int ID)
    {
        curIdx = ShopManager.selectedShapeIndex;
        if (ID < 100)
        {
            abLvl = ShopManager.AbLevelArray[ID];

            EPNeeded = transform.Find("EP Needed").Find("EP").GetComponent<Text>();
            EPNeeded.text = ShopManager.StatsArr[ID][6].ToString();

            Attack = transform.Find("Attack Power").transform.Find("Atck").GetComponent<Text>();
            Def = transform.Find("Defence Power").transform.Find("Def").GetComponent<Text>();
            Pass = transform.Find("Added Passive").transform.Find("P").GetComponent<Text>();

            int p1 = 0, p2 = 0, p3 = 0;
            int p2A = ShopManager.StatsArr[ID][0] / 2, p3A = ShopManager.StatsArr[ID][2] / 2;
            int p2D = ShopManager.StatsArr[ID][3] / 2, p3D = ShopManager.StatsArr[ID][5] / 2;
            if (curIdx == 0 && ArrayContains(Bot.CubeAtck, ID) && ShapeConstants.GroundAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 0 && ArrayContains(Bot.CubeDef, ID) && ShapeConstants.GroundAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 1 && ArrayContains(Bot.PyrAtck, ID) && ShapeConstants.WaterAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 1 && ArrayContains(Bot.PyrDef, ID) && ShapeConstants.WaterAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 2 && ArrayContains(Bot.StarAtck, ID) && ShapeConstants.FireAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 2 && ArrayContains(Bot.StarDef, ID) && ShapeConstants.FireAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 3 && ArrayContains(Bot.SphereAtck, ID) && ShapeConstants.AirAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 3 && ArrayContains(Bot.SphereDef, ID) && ShapeConstants.AirAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }

            curRarity = ShopManager.rarity[ID];
            rarity = transform.Find("Rarity").gameObject;
            rarity.GetComponent<Image>().color = ShapeConstants.rarityColors[curRarity];
            rarity.transform.Find("Rarity").GetComponent<Text>().text = ShapeConstants.rarityNames[curRarity];

            if (abLvl == "0")
            {
                Attack.text = "<color=black>" + ShopManager.StatsArr[ID][0].ToString() + " - " + ShopManager.StatsArr[ID][1].ToString() + " - " + ShopManager.StatsArr[ID][2].ToString() + "</color>";
                Def.text = "<color=black>" + ShopManager.StatsArr[ID][3].ToString() + " - " + ShopManager.StatsArr[ID][4].ToString() + " - " + ShopManager.StatsArr[ID][5].ToString() + "</color>";
                Pass.text = "<color=black>" + p1.ToString() + " - " + p2.ToString() + " - " + p3.ToString() + "</color>";
            }
            else if (abLvl == "1")
            {
                Attack.text = "<b><color=red>" + ShopManager.StatsArr[ID][0].ToString() + "</color></b><color=black> - " + ShopManager.StatsArr[ID][1].ToString() + " - " + ShopManager.StatsArr[ID][2].ToString() + "</color>";
                Def.text = "<b><color=green>" + ShopManager.StatsArr[ID][3].ToString() + "</color></b><color=black> - " + ShopManager.StatsArr[ID][4].ToString() + " - " + ShopManager.StatsArr[ID][5].ToString() + "</color>";
                Pass.text = "<b><color=blue>" + p1.ToString() + "</color></b><color=black> - " + p2.ToString() + " - " + p3.ToString() + "</color>";
            }
            else if (abLvl == "2")
            {
                Attack.text = "<color=black>" + ShopManager.StatsArr[ID][0].ToString() + " - </color><b><color=red>" + ShopManager.StatsArr[ID][1].ToString() + "</color></b><color=black> - " + ShopManager.StatsArr[ID][2].ToString() + "</color>";
                Def.text = "<color=black>" + ShopManager.StatsArr[ID][3].ToString() + " - </color><b><color=green>" + ShopManager.StatsArr[ID][4].ToString() + "</color></b><color=black> - " + ShopManager.StatsArr[ID][5].ToString() + "</color>";
                Pass.text = "<color=black>" + p1.ToString() + " - </color><b><color=blue>" + p2.ToString() + "</color></b><color=black> - " + p3.ToString() + "</color>";
            }
            else
            {
                Attack.text = "<color=black>" + ShopManager.StatsArr[ID][0].ToString() + " - " + ShopManager.StatsArr[ID][1].ToString() + " - </color><b><color=red>" + ShopManager.StatsArr[ID][2].ToString() + "</color></b>";
                Def.text = "<color=black>" + ShopManager.StatsArr[ID][3].ToString() + " - " + ShopManager.StatsArr[ID][4].ToString() + " - </color><b><color=green>" + ShopManager.StatsArr[ID][5].ToString() + "</color></b>";
                Pass.text = "<color=black>" + p1.ToString() + " - " + p2.ToString() + " - </color><b><color=blue>" + p3.ToString() + "</color></b>";
            }

            foreach (Transform go in AllAbilities.transform)
            {
                if (go.GetComponent<Shape_Abilities>().ID == ID)
                {
                    curAb = go.GetComponent<Shape_Abilities>();
                    break;
                }
            }

            int usedSpecs = 0;
            string aT = curAb.getAttackType(), dT = curAb.getDefenseType(), eT = curAb.getEscapeType();
            if (aT != "")
            {
                usedSpecs++;
                transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Attack " + aT;
            }
            if (dT != "")
            {
                for (int i = 0; i < 3; i++)
                {
                    if (curAb.defense[i] == 0) { continue; }
                    if (i == 0) { dT = "Above"; }
                    else if (i == 1) { dT = "Below"; }
                    else { dT = "Straight"; }

                    usedSpecs++;
                    transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                    transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Defense " + dT;
                }
            }
            if (eT != "")
            {
                usedSpecs++;
                transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Escape " + eT;
            }

            for (int r = usedSpecs + 1; r <= 2; r++)
                transform.Find("Spec" + r.ToString()).gameObject.SetActive(false);

            transform.Find("WT").gameObject.SetActive(false);
            transform.Find("Weakness").gameObject.SetActive(false);

            updatePic();

            AbilityName = transform.Find("Ability Name").transform.Find("Name").GetComponent<Text>();
            AbilityName.text = "Level " + abLvl + " " + curAb.name;

            transform.Find("Description").GetComponent<Text>().text = ShapeConstants.DescriptionArray[ID];
        }
        else
        {
            if (ID < 200)
                abLvl = ShopManager.Super100[ID - 101];
            else
                abLvl = ShopManager.Super200[ID - 201];

            int[] temp;
            if (ID < 200)
                temp = ShopManager.Super100StatsArr[ID - 101];
            else
                temp = ShopManager.Super200StatsArr[ID - 201];

            EPNeeded = transform.Find("EP Needed").transform.Find("EP").GetComponent<Text>();
            EPNeeded.text = "<b>" + temp[6].ToString() + "</b>";

            Attack = transform.Find("Attack Power").transform.Find("Atck").GetComponent<Text>();
            Def = transform.Find("Defence Power").transform.Find("Def").GetComponent<Text>();
            Pass = transform.Find("Added Passive").transform.Find("P").GetComponent<Text>();

            int p1 = 0, p2 = 0, p3 = 0, p2A = 0, p3A = 0, p2D = 0, p3D = 0;
            if (ID >= 100 && ID < 200)
            {
                p2A = ShopManager.Super100StatsArr[ID - 101][0] / 2;
                p3A = ShopManager.Super100StatsArr[ID - 101][2] / 2;
                p2D = ShopManager.Super100StatsArr[ID - 101][3] / 2;
                p3D = ShopManager.Super100StatsArr[ID - 101][5] / 2;
            }
            else if (ID >= 200)
            {
                p2A = ShopManager.Super200StatsArr[ID - 201][0] / 2;
                p3A = ShopManager.Super200StatsArr[ID - 201][2] / 2;
                p2D = ShopManager.Super200StatsArr[ID - 201][3] / 2;
                p3D = ShopManager.Super200StatsArr[ID - 201][5] / 2;
            }
            if (curIdx == 0 && ArrayContains(Bot.CubeAtck, ID) && ShapeConstants.GroundAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 0 && ArrayContains(Bot.CubeDef, ID) && ShapeConstants.GroundAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 1 && ArrayContains(Bot.PyrAtck, ID) && ShapeConstants.WaterAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 1 && ArrayContains(Bot.PyrDef, ID) && ShapeConstants.WaterAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 2 && ArrayContains(Bot.StarAtck, ID) && ShapeConstants.FireAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }
            else if (curIdx == 2 && ArrayContains(Bot.StarDef, ID) && ShapeConstants.FireAbilities.Contains(ID)) { p2 = p2D; p3 = p3D; }
            else if (curIdx == 3 && ArrayContains(Bot.SphereAtck, ID) && ShapeConstants.AirAbilities.Contains(ID)) { p2 = p2A; p3 = p3A; }

            curRarity = 3;
            rarity = transform.Find("Rarity").gameObject;
            rarity.GetComponent<Image>().color = ShapeConstants.rarityColors[curRarity];
            rarity.transform.Find("Rarity").GetComponent<Text>().text = ShapeConstants.rarityNames[curRarity];

            if (abLvl == "0")
            {
                Attack.text = "<color=black>" + temp[0].ToString() + " - " + temp[1].ToString() + " - " + temp[2].ToString() + "</color>";
                Pass.text = "<color=black>" + p1.ToString() + " - " + p2.ToString() + " - " + p3.ToString() + "</color>";
            }
            else if (abLvl == "1")
            {
                Attack.text = "<b><color=red>" + temp[0].ToString() + "</color></b><color=black> - " + temp[1].ToString() + " - " + temp[2].ToString() + "</color>";
                Pass.text = "<b><color=blue>" + p1.ToString() + "</color></b><color=black> - " + p2.ToString() + " - " + p3.ToString() + "</color>";
            }
            else if (abLvl == "2")
            {
                Attack.text = "<color=black>" + temp[0].ToString() + " - </color><b><color=red>" + temp[1].ToString() + "</color></b><color=black> - " + temp[2].ToString() + "</color>";
                Pass.text = "<color=black>" + p1.ToString() + " - </color><b><color=blue>" + p2.ToString() + "</color></b><color=black> - " + p3.ToString() + "</color>";
            }
            else
            {
                Attack.text = "<color=black>" + temp[0].ToString() + " - " + temp[1].ToString() + " - </color><b><color=red>" + temp[2].ToString() + "</color></b>";
                Pass.text = "<color=black>" + p1.ToString() + " - " + p2.ToString() + " - </color><b><color=blue>" + p3.ToString() + "</color></b>";
            }

            Def.text = "Infinity";

            foreach (Transform go in AllSpecialAbilities.transform)
            {
                if (go.GetComponent<Shape_Abilities>().ID == ID)
                {
                    curAb = go.GetComponent<Shape_Abilities>();
                    break;
                }
            }

            for (int r = 1; r <= 2; r++) 
                transform.Find("Spec" + r.ToString()).gameObject.SetActive(false);

            transform.Find("WT").gameObject.SetActive(true);
            transform.Find("Weakness").gameObject.SetActive(true);
            transform.Find("Weakness").GetChild(0).GetComponent<Text>().text = "Escape " + (ID < 200 ? ShapeConstants.Super100Weaknesses[ID - 101] : ShapeConstants.Super200Weaknesses[ID - 201]).ToString();

            AbilityName = transform.Find("Ability Name").transform.Find("Name").GetComponent<Text>();
            AbilityName.text = "Level " + abLvl + " " + curAb.name;

            if (ID < 200)
                transform.Find("Description").GetComponent<Text>().text = ShapeConstants.Super100Description[ID - 101];
            else
                transform.Find("Description").GetComponent<Text>().text = ShapeConstants.Super200Description[ID - 201];

            updatePic();
        }
    }

    private void updatePic()
    {

        curIdx = ShopManager.selectedShapeIndex;

        Transform temp = transform.Find("Pic");
        if (curAb.common)
            temp.GetChild(0).GetComponent<Image>().sprite = curAb.transform.GetChild(curIdx).GetComponent<Image>().sprite;
        else
            temp.GetChild(0).GetComponent<Image>().sprite = curAb.transform.GetChild(0).GetComponent<Image>().sprite;

        Transform EP = transform.Find("EP Needed");
        EP.GetComponent<Image>().color = ShapeConstants.bckdAbEPColor[curIdx];
    }
}
