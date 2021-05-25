using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Shape_Player : MonoBehaviour {
    protected GameMaster GM;
    public bool EarthquakeOngoing;

    //Variables common to ALL shapes
    private int ConnectionID;
    private string Username;

    private bool P1;
    private bool Initialized = false;

    private int EP;
    private int life = 100;
    private int EPToRemove;

    private bool choiceDone;
    private int idOfAnimUsed;

    private int selfdmg;
    private int bulletPower;

    public GameObject Bullet;

    private Light glowLight;

    //0 above, 1 below, 2 straight
    private int[] attack, defense;
    private bool[] escapeFrom;

    private int AdditionalDamage;

    public MateCode Mate;
    public Assist CubeAssist;
    public GameObject EPEffect;
    public GameObject DoubleEdgedSword;
    public GameObject HealingCircle;

    public bool Reconnect = false;

    //From levels
    private int AddAtt = 0;
    private int AddDef = 0;
    public int MaxLP = 0;

    private bool TriggerCollision;

    public int ProbPlusAtt = 0;
    public int ProbPlusDef = 0;

    private void Awake() {
        P1 = gameObject.name == "Player1";

        if (gameObject.name == "Player2") {
            ConnectionID = TempOpponent.Opponent.ConnectionID;
            Username = TempOpponent.Opponent.Username;
        }
        else if (gameObject.name == "Player1" && GameMaster.Spectate) {
            ConnectionID = TempOpponent.Opponent.ConnectionID2;
            Username = TempOpponent.Opponent.Username2;
        }
        Mate = gameObject.transform.Find("Mate").GetComponent<MateCode>();
    }

    protected void OnEnable() {
        if (Initialized) { return; }
        Initialized = true;

        GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();
        int Level;
        if (GameMaster.levelStats == null) {
            string levelStatsStr = PlayerPrefs.GetString("LevelStats");
            GameMaster.levelStats = ClientHandleData.TransformToArrayShop(levelStatsStr);
        }
        if (P1) {
            if (!(GameMaster.Replay || GameMaster.Spectate || GameMaster.Online || GameMaster.botOnline) && ((GameMasterOffline)GM).Tutorial)
                Level = 1;
            else if (!(GameMaster.Replay || GameMaster.Spectate))
                Level = PlayerPrefsX.GetIntArray("Level")[GM.shapeID1];
            else if ((GameMaster.Online || GameMaster.botOnline) && (GameMaster.Replay || GameMaster.Spectate))
                Level = TempOpponent.Opponent.OpLvl2;
            else
                Level = 1;
            AddAtt = GameMaster.levelStats[Level - 1][GM.shapeID1][0];
            AddDef = GameMaster.levelStats[Level - 1][GM.shapeID1][1];
            MaxLP = 100 + GameMaster.levelStats[Level - 1][GM.shapeID1][2];
        }
        else {
            if (GameMaster.Online || GameMaster.botOnline) {
                Level = TempOpponent.Opponent.OpLvl;
            }
            else if (!(GameMaster.Replay || GameMaster.Spectate || GameMaster.Online || GameMaster.botOnline) && ((GameMasterOffline)GM).Tutorial)
                Level = 1;
            else {
                Level = PlayerPrefsX.GetIntArray("Level")[GM.shapeID2];
            }
            AddAtt = GameMaster.levelStats[Level - 1][GM.shapeID2][0];
            AddDef = GameMaster.levelStats[Level - 1][GM.shapeID2][1];
            MaxLP = 100 + GameMaster.levelStats[Level - 1][GM.shapeID2][2];
        }

        if (!Reconnect && !GameMaster.Spectate) {
            life = MaxLP;
            EP = 100;

            //choiceDone gets reset to false by the GM
            choiceDone = false;
            idOfAnimUsed = -1;

            // bulletPower represents the attack of a single bullet but attackPower represents the overall attack
            bulletPower = 0;
        }
        attack = new int[] { 0, 0, 0 };
        defense = new int[] { 0, 0, 0 };
        escapeFrom = new bool[] { false, false, false };

        glowLight = gameObject.GetComponentInChildren<Light>();
        glowLight.intensity = 0;

        Reconnect = false;
    }

    public int GetConnectionID() {
        return this.ConnectionID;
    }
    public void SetConnectionID(int connectionID) {
        this.ConnectionID = connectionID;
    }
    public string GetUsername() {
        return this.Username;
    }
    public void SetUsername(string username) {
        this.Username = username;
    }
    public int GetEP() {
        return this.EP;
    }
    public void SetEP(int newEP) {
        this.EP = newEP;
        if (glowLight == null)
            glowLight = gameObject.GetComponentInChildren<Light>();

        if (newEP < 10)
            glowLight.intensity = newEP;
        else
            glowLight.intensity = 10;
    }

    public bool GetTriggerCollision() {
        return TriggerCollision;
    }

    public int GetLife() {
        return this.life;
    }
    public void SetLife(int newLife) {
        //Show how much life was lost/gained
        if (newLife > MaxLP && MaxLP != 0)
            newLife = MaxLP;

        if (GM == null)
            GM = GameObject.Find("Game Manager").GetComponent<GameMaster>();

        if (life > newLife)
            GM.ShowDamage(P1 ? 1 : 2, life - newLife, true);
        else if (life < newLife)
            GM.ShowDamage(P1 ? 1 : 2, newLife - life, false);

        life = newLife;
        GM.updateLifeBars();
    }

    public int GetEPToRemove() {
        return this.EPToRemove;
    }
    public void SetEPToRemove(int newVal) {
        this.EPToRemove = newVal;
    }

    public bool GetChoiceDone() {
        return this.choiceDone;
    }
    public void SetChoiceDone(bool newChoice) {
        if (P1) { Shape_Abilities.choiceDoneP1 = newChoice; }
        else { Shape_Abilities.choiceDoneP2 = newChoice; }

        choiceDone = newChoice;
        if (choiceDone) { GM.TryEvaluating(); }
    }

    public int GetIdOfAnimUsed() {
        return this.idOfAnimUsed;
    }
    public void SetIdOfAnimUSed(int newID) {
        List<int> IDs = new List<int> { 1, 2, 3, 5, 6, 11, 14, 15, 17, 21, 22, 23, 25, 27, 29, 42 };
        if (IDs.Contains(newID))
            TriggerCollision = true;
        else
            TriggerCollision = false;

        int Level = GetLevel(newID);
        idOfAnimUsed = newID;
        if (Level < 2 || (GM is GameMasterOffline && ((GameMasterOffline)GM).Tutorial)) { ProbPlusAtt = ProbPlusDef = 0; return; }
        int[] Stats = GetStats(newID, Level);
        int s0 = (int)(Stats[0] / 2);
        int s1 = (int)(Stats[1] / 2);

        ProbPlusAtt = (ArrayContains(Bot.CubeAtck, newID) && ShapeConstants.GroundAbilities.Contains(newID)) ? s0 : 0;
        ProbPlusAtt = (ArrayContains(Bot.PyrAtck, newID) && ShapeConstants.WaterAbilities.Contains(newID)) ? s0 : 0;
        ProbPlusAtt = (ArrayContains(Bot.StarAtck, newID) && ShapeConstants.FireAbilities.Contains(newID)) ? s0 : 0;
        ProbPlusAtt = (ArrayContains(Bot.SphereAtck, newID) && ShapeConstants.AirAbilities.Contains(newID)) ? s0 : 0;

        ProbPlusDef = (ArrayContains(Bot.CubeDef, newID) && ShapeConstants.GroundAbilities.Contains(newID)) ? s1 : 0;
        ProbPlusDef = (ArrayContains(Bot.PyrDef, newID) && ShapeConstants.WaterAbilities.Contains(newID)) ? s1 : 0;
        ProbPlusDef = (ArrayContains(Bot.StarDef, newID) && ShapeConstants.FireAbilities.Contains(newID)) ? s1 : 0;
        ProbPlusDef = (ArrayContains(Bot.SphereDef, newID) && ShapeConstants.AirAbilities.Contains(newID)) ? s1 : 0;
    }

    private int[] GetStats(int ID, int Level) {
        int idx = (Level == 3 ? 2 : 0);
        int[] Ar = new int[2];
        if (ID > 0 && ID < 100) {
            Ar[0] = GameMaster.StatsArr[ID][idx];
            Ar[1] = GameMaster.StatsArr[ID][idx + 3];
        }
        else if (ID >= 100 && ID < 200) {
            Ar[0] = GameMaster.Super100StatsArr[ID - 101][idx];
            Ar[1] = GameMaster.Super100StatsArr[ID - 101][idx + 3];
        }
        else if (ID >= 200) {
            Ar[0] = GameMaster.Super200StatsArr[ID - 201][idx];
            Ar[1] = GameMaster.Super200StatsArr[ID - 201][idx + 3];
        }
        return Ar;
    }

    private int GetLevel(int ID) {
        int AbLevel = -1;
        if (ID < 0) { return AbLevel; }
        if (gameObject.name == "Player1") {
            if (ID < 100)
                Int32.TryParse(GameMaster.AbLevelArray[ID], out AbLevel);
            else if ((ID > 100) && (ID < 200))
                Int32.TryParse(GameMaster.Super100[ID - 101], out AbLevel);
            else if (ID > 200)
                Int32.TryParse(GameMaster.Super200[ID - 201], out AbLevel);
        }
        else {
            if (GameMaster.Online || GameMaster.botOnline) {
                if (ID < 100)
                    Int32.TryParse(TempOpponent.Opponent.AbLevelArray[ID], out AbLevel);
                else if ((ID > 100) && (ID < 200))
                    Int32.TryParse(TempOpponent.Opponent.Super100[ID - 101], out AbLevel);
                else if (ID > 200)
                    Int32.TryParse(TempOpponent.Opponent.Super200[ID - 201], out AbLevel);
            }
            else {
                if (ID < 100)
                    Int32.TryParse(GameMaster.AbLevelArray[ID], out AbLevel);
                else if ((ID > 100) && (ID < 200))
                    Int32.TryParse(GameMaster.Super100[ID - 101], out AbLevel);
                else if (ID > 200)
                    Int32.TryParse(GameMaster.Super200[ID - 201], out AbLevel);
            }
        }
        return AbLevel;
    }

    private bool ArrayContains(int[] Array, int i) {
        foreach (int j in Array) {
            if (i == j)
                return true;
        }
        return false;
    }

    public int getAddLvlAttack() {
        return AddAtt;
    }

    public int[] getAttackArr() {
        int[] ans = new int[3];
        attack.CopyTo(ans, 0);
        return ans;
    }
    public int getOverallAttack() {
        int ans = 0;
        for (int i = 0; i < 3; i++) { ans += attack[i]; }
        return ans;
    }
    public int getAttack(int dir) {
        return attack[dir];
    }
    public int GetDefensePower() {
        int ans = 0;
        for (int i = 0; i < 3; i++) {
            if (defense[i] > 0)                //Defense is always the same and the total amount for each direction
            { ans = defense[i]; break; }
        }
        return ans;
    }
    public int getDefense(int dir) {
        return defense[dir];
    }
    public int getAddLvlDef() {
        return AddDef;
    }
    public int[] getDefenseArr() {
        int[] ans = new int[3];
        defense.CopyTo(ans, 0);
        return ans;
    }
    public int GetBulletPower() {
        return bulletPower;
    }

    public void setAddLvlAtt(int n) { AddAtt = n; }
    public void setAddLvlDef(int n) { AddDef = n; }

    public void SetBulletPower(int newPower, int Ratio = 1) {
        bulletPower = newPower + (AdditionalDamage / Ratio) + (AddAtt / Ratio);
    }

    public void SetAttackPower(int[] newPower) {
        newPower.CopyTo(attack, 0);
        int attackDir = 0;
        for (int i = 0; i < 3; i++) {
            if (attack[i] > 0) { attackDir++; }
        }
        for (int i = 0; i < 3; i++) {
            if (attack[i] > 0) { attack[i] += (AdditionalDamage + AddAtt) / attackDir; }
        }
    }
    public void SetShieldPower(int[] newPower) {
        newPower.CopyTo(defense, 0);
        for (int i = 0; i < 3; i++) {
            if (defense[i] > 0) { defense[i] += AddDef; }
        }
    }

    public bool escapesFrom(int dir) {
        return escapeFrom[dir];
    }
    public bool[] getEscapeFromArr() {
        bool[] ans = new bool[3];
        escapeFrom.CopyTo(ans, 0);
        return ans;
    }
    public void setEscapeFrom(bool[] newEscape) {
        newEscape.CopyTo(escapeFrom, 0);
    }

    public int getAdditionalDamage() {
        return AdditionalDamage;
    }
    public void setAdditionalDamage(int newD) {
        AdditionalDamage = -AdditionalDamage + newD;
        SetAttackPower(attack);
        if (bulletPower > 0) {
            int Ratio = getOverallAttack() / bulletPower;
            SetBulletPower(bulletPower, Ratio);
        }
        AdditionalDamage = newD;
    }


    public void SetSelfdmg(int dmg) {
        selfdmg = dmg;
    }
    public int GetSelfdmg() {
        return selfdmg;
    }

    public virtual void Choice(int ID) {
        if (ID == 0) {
            EPEffect.SetActive(true);
            Invoke("ResetEffect", 2f);
        }
        if (ID == 34) {
            DoubleEdgedSword.SetActive(true);
        }
        if (ID == 41) {
            HealingCircle.SetActive(true);
        }
    }
    private void ResetEffect() {
        EPEffect.SetActive(false);
    }

    public virtual void SetFalse() { }


    public string GetAttackType() {
        if (attack[0] > 0) { return "Above"; }
        else if (attack[1] > 0) { return "Below"; }
        else if (attack[2] > 0) { return "Straight"; }
        return "";
    }
    public string GetEscapeType() {
        if (escapeFrom[0] && escapeFrom[1] && escapeFrom[2]) { return "Ultimate"; }
        if (escapeFrom[1] && escapeFrom[2]) { return "Above"; }
        else if (escapeFrom[0] && escapeFrom[2]) { return "Below"; }
        else if (escapeFrom[0] && escapeFrom[1]) { return "Straight"; }
        return "";
    }
}