using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeConstants : MonoBehaviour
{
    public static Color[] lifeBarColors = new Color[] { new Color(.669f, .516f, 0f), new Color(0f, 0f, 1f), new Color(1f, 0f, 0f), new Color(0f, 1f, 0f) };

    public static Color[] levelColors = new Color[] { new Color(.65f, .46f, 0f), new Color(0f, 0.6367367f, 0.8207547f), new Color(1f, 0f, 0f), new Color(0f, 0.8490566f, 0.1322492f) };
    public static Color[] levelMaxColors = new Color[] { new Color(.425f, .278f, 0f), new Color(0f, 0f, 0.8584906f), new Color(0.6603774f, .1f, 0f), new Color(0f, 0.5283019f, 0f) };

    public static Color[] bckgNameColor = new Color[] { new Color(0.4433962f, 0.2632665f, 0f), new Color(0.2f, 0.2f, 0.42f), new Color(0.372549f, 0f, 0f), new Color(0f, 0.4f, 0f) };
    public static Color[] bckdAbEPColor = new Color[] { new Color(0.575f, .36f, 0.23f), new Color(0f, 0.224f, 1f), new Color(.75f, 0f, 0f), new Color(0f, 0.5f, 0f) };

    public static int[] PPRange = new int[] { 20, 50, 100 };
    public static Color[] PPColors = new Color[] { new Color(0.5f, 0.5f, 0.5f), new Color(0f, 0.6f, 0f), new Color(1f, 0f, 1f), new Color(0.9882354f, 0.7137255f, 0.3215686f) };

    public static string[] rarityNames = new string[] { "Basic", "Rare", "Mythic", "Majestic" };
    public static Color[] rarityColors = new Color[] { new Color(0.5f, 0.5f, 0.5f), new Color(0.7830189f, 0.2105287f, 0.6896338f), new Color(1f, .5f, 0f), new Color(1f, 1f, 0f) };

    public static Color selectedColor = new Color(0f, 0f, 255f);

    //Unavailable, to claim, claimed
    public static Color[] checkPointsStates = new Color[] { new Color(0.6784314f, 0.6784314f, 0.6784314f), new Color(0f, .9f, 0f), new Color(0.04705883f, 0.5294118f, 0.7490196f) };

    public static int maxLevel = 8;
    public static int ArenaPP = 50;

    public static string[] NamesArray = new string[57];
    public static string[] PassiveNames = new string[8];
    public static string[] Super100Names = new string[] { "Cube Crash", "Supernova", "Portal Attack", "Tornado" };
    public static string[] Super200Names = new string[] { "BlackHole", "Ground Destruction", "Water Transfer", "Toxic Worm" };
    public static string[] Super100Weaknesses = new string[] { "Below", "Below", "Below", "Below" };
    public static string[] Super200Weaknesses = new string[] { "Above", "Above", "Above", "Above" };
    public static string[] ShapeNames = new string[] { "Cube", "Pyramid", "Star", "Sphere" };
    public static string[] DescriptionArray = new string[57];
    public static string[] PassiveDescription = new string[8];
    public static string[] Super100Description = new string[4];
    public static string[] Super200Description = new string[4];
    public static List<int> FireAbilities = new List<int>();
    public static List<int> WaterAbilities = new List<int>();
    public static List<int> GroundAbilities = new List<int>();
    public static List<int> AirAbilities = new List<int>();
    public static List<int> CubeOnly, PyrOnly, StarOnly, SphereOnly;

    public static void Initialize()
    {
        NamesArray[0] = "Get EP";
        NamesArray[1] = "AttackCube";
        NamesArray[2] = "Shield";                   DescriptionArray[2] = "The Cube uses his earth mastery to create a shield of rocks in front and above him";
        NamesArray[3] = "Tackle";                   DescriptionArray[3] = "The Cube rushes straight to attack his opponent, destroying any shield in its way";
        NamesArray[4] = "Shadow Attack";            DescriptionArray[4] = "The Shape gets below the ground and pops behind the opponent, dealing damage from behind. He also escapes any attack above the ground";
        NamesArray[5] = "Mini Cubes Barrage";       DescriptionArray[5] = "The Cube launches a serie of deadly attacks at his opponent";
        NamesArray[6] = "Cube Sealing";             DescriptionArray[6] = "The Cube summons a cube cage around the opponent and closes it down to suffocate him while being protected from any straight bullet";
        NamesArray[7] = "Elevation";                DescriptionArray[7] = "Allows the shape to elevate and escape any attacks that are either frontal or from below";                                                                                                       
        NamesArray[8] = "Devil's Deal";             DescriptionArray[8] = "You temporarly give 2 Energy Points to the devil in order to get an extra EP for the next 3 rounds. After it was used, the player has to wait 4 rounds before being able to use it again";
        NamesArray[9] = "Energy Drain";             DescriptionArray[9] = "Your hands attack the opponent, going through any shield, and steal the opponent's Energy Points. If he doesn't have any, than his life is drained";
        NamesArray[10] = "Aerial Strike";           DescriptionArray[10] = "The Cube launches a projectile in the air and hurts the player from his head";
        NamesArray[11] = "Mirror Defense";          DescriptionArray[11] = "The Shape creates two portals, one in front of him and one behind the opponent. Anything that enters the first gets out from the second. It can be quite useful against projectiles";
        NamesArray[12] = "Earthquake";              DescriptionArray[12] = "The Cube misbalances the tectonic plates thanks to his ground mastery, hereby creating a powerful earthquake";
        NamesArray[13] = "Eye Of Horus";            DescriptionArray[13] = "The Eye of Horus assists its follower the Pyramid by taking down its enemies";
        NamesArray[14] = "Ground Of Steel";         DescriptionArray[14] = "The Cube uses his ground mastery to harden it into a material similar to steel and form parts of it into a wall in front of him, therefore protecting himself from straight and below attacks";
        NamesArray[15] = "FireBlast";               DescriptionArray[15] = "The Star gives its opponents a first hand taste of what a sun eruption feels like";
        NamesArray[16] = "WaterFall";               DescriptionArray[16] = "The Pyramid utilizes its water mastery to create a protective Waterfall, protecting it from above and straight attacks.";
        NamesArray[17] = "Spin Little Star";        DescriptionArray[17] = "The Star spins to gain momentum and charge full force into its opponent, dealing massive damage";
        NamesArray[18] = "Curse Of The Cloud";      DescriptionArray[18] = "The Pyramid uses its water mastery to create a raining cloud over its opponent’s head which deals damage continuously for 3 turns";
        NamesArray[19] = "Fountain Attack";         DescriptionArray[19] = "The Pyramid collects water underground and makes it emerge at an extremely high speed, severly damaging its opponent";
        NamesArray[20] = "NOTHING";
        NamesArray[21] = "Rotating Snow Spear";     DescriptionArray[21] = "The Pyramid spins to gain momentum and charge full force into its opponent, dealing massive damage";
        NamesArray[22] = "AttackPyr";
        NamesArray[23] = "AttackStar";
        NamesArray[24] = "Fire Shield";             DescriptionArray[24] = "The Star uses its fire mastery to create a fire shield in front and below him burning incoming attacks to shreds";
        NamesArray[25] = "Mate";                    DescriptionArray[25] = "The Shape cuts off part of his soul and uses his respective mastery to create a vessel for it, which will fight for him and protect him no matter the cost";
        NamesArray[26] = "Assist";                  DescriptionArray[26] = "The Cube summons help from the earth spirits and therefore increases his attack for the next round, leaving himself defenseless";
        NamesArray[27] = "Ice Disk";                DescriptionArray[27] = "The Pyramid freezes water and shapes it into a deadly disk which rotates at a frightening velocity, attacking his opponent";
        NamesArray[28] = "Ice Shards";              DescriptionArray[28] = "The Pyramid uses its water mastery to shape underground water into ice shards gradually emerging and ripping anything in their path to shreds";
        NamesArray[29] = "Fireball";                DescriptionArray[29] = "The Star uses its fire mastery to create a massive fire ball which hurls at the opponent causing massive damage";
        NamesArray[30] = "Fire Bridge";             DescriptionArray[30] = "The Star releases an enormous amount of heat that ignites the air around it and shapes it into a bridge aiming for the opponent";
        NamesArray[31] = "Fire Laser";              DescriptionArray[31] = "The Star focuses all of its fire into one single point and unleashes an extremely speedy and deadly burst at its opponent";
        NamesArray[32] = "Fire Bomb";               DescriptionArray[32] = "The Star bombards its opponent with a fire bomb dealing massive damage in the process";
        NamesArray[33] = "Fire Boost";              DescriptionArray[33] = "The Star amasses fire energy from its environment, boosting its attack for the next round while leaving itself defenseless this turn";
        NamesArray[34] = "DoubleEdged Swords";      DescriptionArray[34] = "The Shape sacrifices defense for attack by letting a sword pierce his armor in exchange for getting one to strengthen his attack";
        NamesArray[35] = "Bubble Attack";           DescriptionArray[35] = "The Pyramid gathers water into a bubble and launches it, drowning his enemy in the process";
        NamesArray[36] = "Bubble Shield";           DescriptionArray[36] = "The Pyramid gathers water into a bubble surrounding and protecting it from incoming straight and below attacks";
        NamesArray[37] = "BluePlanetBackup";        DescriptionArray[37] = "The Pyramid amasses water energy from Earth’s environment, boosting its attack for the next round while leaving itself defenseless this turn";
        NamesArray[38] = "Water Cage";              DescriptionArray[38] = "The Pyramid envelops its opponent in a water cage, suffocating it in the process";
        NamesArray[39] = "Mud Attack";              DescriptionArray[39] = "The Cube launches mud at high velocity towards his opponent, causing massive damage in the process";
        NamesArray[40] = "Mud Eruption";            DescriptionArray[40] = "The Cube summons mud from underground and surrounds the opponent with it, suffocating and damaging him in the process";
        NamesArray[41] = "Healing Circle";          DescriptionArray[41] = "The Shape gathers the life energy around himself and lets it gradually heal him for 3 rounds";
        NamesArray[42] = "AttackSphere";            
        NamesArray[43] = "Toxic Shot";              DescriptionArray[43] = "The Sphere launches a poisonous liquid at its opponent at an extremely high velocity, dealing massive damage in the process";
        NamesArray[44] = "Poisonous Air";           DescriptionArray[44] = "The Sphere amasses poisonous gases from Earth’s environment, boosting its attack for the next round while leaving itself defenseless this turn";
        NamesArray[45] = "Poisonous Bubble";        DescriptionArray[45] = "The Sphere gathers poison into a bubble and launches it in its enemy's direction";
        NamesArray[46] = "Toxic Ring";              DescriptionArray[46] = "The Sphere lets poisonous gas emerge from underground and surround its opponent, suffocating it";
        NamesArray[47] = "Air Cannon";              DescriptionArray[47] = "The Sphere sucks in the surrounding air and launches it at a blinding speed destroying everything in its path";
        NamesArray[48] = "Poison Cloud";            DescriptionArray[48] = "The Sphere manipulates the planet’s toxic weather to form a poisonous cloud over its opponent and drown him with toxic rain";
        NamesArray[49] = "Upwards Spiral";          DescriptionArray[49] = "The Sphere manipulates toxic gas into a spinning spiral, therefore defending against incoming straight and above attacks";
        NamesArray[50] = "Wind Arrow";              DescriptionArray[50] = "The Sphere uses its air mastery to form it into an arrow and launch it from above at its opponent";
        NamesArray[51] = "Mini Nado";               DescriptionArray[51] = "The Sphere manipulates the air directly below its opponent into a miniature tornado, therefore dealing massive damage";
        NamesArray[52] = "Toxic Hammer";            DescriptionArray[52] = "The Sphere manipulates the toxic gas in the Earth’s atmosphere into a hammer of doom, judging everyone it strikes on for their sins against mother nature";
        NamesArray[53] = "NOTHING";                 
        NamesArray[54] = "Shadow Dissolve";         DescriptionArray[54] = "The Player dissolves into his own shadow, therefore escaping any attack from above or coming at him directly";
        NamesArray[55] = "Flame Shield";            DescriptionArray[55] = "The Star manipulates flames into a shield defending it from incoming above and straight attacks";
        NamesArray[56] = "Downwards Spiral";        DescriptionArray[56] = "The Sphere manipulates toxic gas into a spinning spiral, therefore defending against incoming straight and below attacks";

        PassiveNames[0] = "Stuck in Place";         PassiveDescription[0] = "If the opponent gets hit, he has a chance to be stuck in place for the next round, disallowing him to do any ability which requires leaving his place";
        PassiveNames[1] = "Protective Earth";       PassiveDescription[1] = "If the player gets hit, he has a chance to summon extra cubes which will grant him additional defense for the next round";
        PassiveNames[2] = "Freeze";                 PassiveDescription[2] = "If the opponent gets hit, he has a chance to be frozen for the next round, disallowing him to use any ability";
        PassiveNames[3] = "Snow";                   PassiveDescription[3] = "The snow comes down, giving an attack boost to the pyramid for the next round";
        PassiveNames[4] = "Burn";                   PassiveDescription[4] = "The opponent has a chance to get burned when he gets hit by the star's attack; for 2 rounds, he will suffer some damage";
        PassiveNames[5] = "Fiery Eyes";             PassiveDescription[5] = "The eyes of the star are lit by a fire, making its next attack more dangerous than ever";
        PassiveNames[6] = "Helping Spheres";        PassiveDescription[6] = "Some spheres are summoned in circle with the original sphere in the middle, yielding support to its next attack";
        PassiveNames[7] = "Fog";                    PassiveDescription[7] = "When surrounded by the frog, if the sphere doesn't use any ability for the next round, he can escape any attack for sure";

        Super100Description[0] = "The Cube uses his ground mastery in order to gather hundreds of mini cubes from the ground and form one gigantic cube over the enemy and hurls it at them, leaving him stunned and awaiting his certain doom unless he somehow escaped underground before the process began";
        Super100Description[1] = "The Star increases his size voluntarily in order to produce a mighty explosion using the clashing forces of gravity and the nuclear interactions inside of him. He leaves the enemy stunned, awaiting his certain doom unless he somehow escaped underground before the process began";
        Super100Description[2] = "The Pyramid summons portals of doom around its opponent, which bombard it with ice shards at an unthinkable speed, shattering its opponent to pieces and freezing it in the process. The only chance his opponent has involves escaping underground";
        Super100Description[3] = "The Sphere focuses all of the surrounding air into one point and swirls it an extremely high velocity creating a vicious tornado aimed at its opponent and destroying everything in its path. The only way to evade this majestic attack is by escaping from below";

        Super200Description[0] = "The Star voluntarily increases his size so much that he succumbs to the force of Gravity to form a Black Hole: the most certain form of doom where not even light can escape once it has entered it. He leaves the enemy stunned awaiting to be pulled in and ripped to shreds unless he somehow evaded the doom radius in an aerial fashion before the black hole fully set in";
        Super200Description[1] = "The Cube turns into a gigantic version of himself and starts to roll onto what appears to be only small insects daring to defy his might. Unless the enemy escaped somehow in the air, he leaves him stunned awaiting his certain doom";
        Super200Description[2] = "The Pyramid collects all of the surrounding water and funnels it towards its opponent in a never ending stream leaving him gasping for air as he drowns in the water unless the opponenet escapes somehow from above";
        Super200Description[3] = "The Sphere channels all of the environment’s toxicity into a worm-like object which then races towards its opponent with the aim of suffocating and poisoning it badly. The only way to evade this majestic attack is by escaping from above";


        GroundAbilities.Add(2);            WaterAbilities.Add(13);                  FireAbilities.Add(15);              AirAbilities.Add(43);
        GroundAbilities.Add(5);            WaterAbilities.Add(16);                  FireAbilities.Add(24);              AirAbilities.Add(45);
        GroundAbilities.Add(6);            WaterAbilities.Add(18);                  FireAbilities.Add(29);              AirAbilities.Add(46);
        GroundAbilities.Add(10);           WaterAbilities.Add(19);                  FireAbilities.Add(30);              AirAbilities.Add(47);
        GroundAbilities.Add(12);           WaterAbilities.Add(27);                  FireAbilities.Add(31);              AirAbilities.Add(48);
        GroundAbilities.Add(14);           WaterAbilities.Add(35);                  FireAbilities.Add(32);              AirAbilities.Add(49);
        GroundAbilities.Add(39);           WaterAbilities.Add(36);                  FireAbilities.Add(102);             AirAbilities.Add(50);
        GroundAbilities.Add(40);           WaterAbilities.Add(38);                  FireAbilities.Add(201);             AirAbilities.Add(51);
        GroundAbilities.Add(101);          WaterAbilities.Add(103);                 FireAbilities.Add(55);              AirAbilities.Add(52);
        GroundAbilities.Add(202);          WaterAbilities.Add(203);                                                     AirAbilities.Add(104);
                                                                                                                        AirAbilities.Add(204);
                                                                                                                        AirAbilities.Add(56);

        CubeOnly = new List<int>(GroundAbilities);CubeOnly.Add(3);CubeOnly.Add(26);
        PyrOnly = new List<int>(WaterAbilities);PyrOnly.Add(21);PyrOnly.Add(37);
        StarOnly = new List<int>(FireAbilities);StarOnly.Add(17);StarOnly.Add(33);
        SphereOnly = new List<int>(AirAbilities);SphereOnly.Add(44);
    }
}
