using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionInGame : MonoBehaviour {
    public Shape_Abilities curAb;

    private int abLvl;
    private Text AbilityName;
    private Text Attack;
    private Text Def;

    private int curRarity;
    private bool p1, v2;
    private GameObject rarity;
    private GameMaster GM;

    public void setUpDescription(int ID) {
        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        v2 = (GM is GameMasterOnline && ((GameMasterOnline)GM).v2);
        p1 = transform.root.name == "Canvas1";

        if (ID <= 0)
            return;
        if (ID < 100) {
            Int32.TryParse(GameMaster.AbLevelArray[ID], out abLvl);

            Attack = transform.Find("Atck").GetComponent<Text>();
            Def = transform.Find("Def").GetComponent<Text>();
            int att = GameMaster.StatsArr[ID][abLvl - 1], def = GameMaster.StatsArr[ID][abLvl + 2];
            int addAtt = GM.player1.getAddLvlAttack(), addDef = GM.player1.getAddLvlDef();
            if (!p1 && !v2) { addAtt = GM.player2.getAddLvlAttack(); addDef = GM.player2.getAddLvlDef(); }
            else if (!p1 && v2) { addAtt = GM.player12.getAddLvlAttack(); addDef = GM.player12.getAddLvlDef(); }

            Attack.text = att.ToString() + (att > 0 ? "+" + addAtt.ToString() : "");
            Def.text = def.ToString() + (def > 0 ? "+" + addDef.ToString() : "");

            curRarity = GameMaster.rarity[ID];
            rarity = transform.Find("Rarity").gameObject;
            rarity.GetComponent<Image>().color = ShapeConstants.rarityColors[curRarity];

            foreach (Transform go in transform.parent.transform) {
                if (go.GetComponent<Shape_Abilities>() == null) { continue; }
                if (go.GetComponent<Shape_Abilities>().ID == ID) {
                    curAb = go.GetComponent<Shape_Abilities>();
                    break;
                }
            }

            int usedSpecs = 0;
            string aT = curAb.getAttackType(), dT = curAb.getDefenseType(), eT = curAb.getEscapeType();
            if (aT != "") {
                usedSpecs++;
                transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Attack " + aT;
            }
            if (dT != "") {
                for (int i = 0; i < 3; i++) {
                    if (curAb.defense[i] == 0) { continue; }
                    if (i == 0) { dT = "Above"; }
                    else if (i == 1) { dT = "Below"; }
                    else { dT = "Straight"; }

                    usedSpecs++;
                    transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                    transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Defense " + dT;
                }
            }
            if (eT != "") {
                usedSpecs++;
                transform.Find("Spec" + usedSpecs.ToString()).gameObject.SetActive(true);
                transform.Find("Spec" + usedSpecs.ToString()).GetComponentInChildren<Text>().text = "Escape " + eT;
            }

            if (usedSpecs == 1) {
                transform.Find("Spec").gameObject.SetActive(true);
                transform.Find("Spec").GetComponentInChildren<Text>().text = transform.Find("Spec1").GetComponentInChildren<Text>().text;
                transform.Find("Spec1").gameObject.SetActive(false);
            }
            else {
                transform.Find("Spec").gameObject.SetActive(false);
            }

            for (int r = usedSpecs + 1; r <= 2; r++)
                transform.Find("Spec" + r.ToString()).gameObject.SetActive(false);

            transform.Find("WT").gameObject.SetActive(false);
            transform.Find("Weakness").gameObject.SetActive(false);

            AbilityName = transform.Find("Rarity").GetChild(0).GetComponent<Text>();
            AbilityName.text = "Level " + abLvl + " " + curAb.name;
            AbilityName.color = new Color(1f, 1f, 1f);
        }
        else {
            if (ID < 200)
                Int32.TryParse(GameMaster.Super100[ID - 101], out abLvl);
            else
                Int32.TryParse(GameMaster.Super200[ID - 201], out abLvl);

            int[] temp;
            if (ID < 200)
                temp = GameMaster.Super100StatsArr[ID - 101];
            else
                temp = GameMaster.Super200StatsArr[ID - 201];

            Attack = transform.Find("Atck").GetComponent<Text>();
            int addAtt = GM.player1.getAddLvlAttack();
            if (!p1 && !v2) { addAtt = GM.player2.getAddLvlAttack(); }
            else if (!p1 && v2) { addAtt = GM.player12.getAddLvlAttack(); }

            curRarity = 3;
            rarity = transform.Find("Rarity").gameObject;
            rarity.GetComponent<Image>().color = ShapeConstants.rarityColors[curRarity];

            Attack.text = temp[abLvl - 1].ToString() + "+" + addAtt.ToString();

            foreach (Transform go in transform.parent.transform) {
                if (go.GetComponent<Shape_Abilities>() == null) { continue; }
                if (go.GetComponent<Shape_Abilities>().ID == ID) {
                    curAb = go.GetComponent<Shape_Abilities>();
                    break;
                }
            }

            transform.Find("Spec").gameObject.SetActive(false);
            for (int r = 1; r <= 2; r++)
                transform.Find("Spec" + r.ToString()).gameObject.SetActive(false);

            transform.Find("WT").gameObject.SetActive(true);
            transform.Find("Weakness").gameObject.SetActive(true);
            transform.Find("Weakness").GetChild(0).GetComponent<Text>().text = "Escape " + (ID < 200 ? ShapeConstants.Super100Weaknesses[ID - 101] : ShapeConstants.Super200Weaknesses[ID - 201]).ToString();

            AbilityName = transform.Find("Rarity").GetChild(0).GetComponent<Text>();
            AbilityName.text = "Level " + abLvl + " " + curAb.name;
            AbilityName.color = new Color(0f, 0f, 0f);
        }
    }
}