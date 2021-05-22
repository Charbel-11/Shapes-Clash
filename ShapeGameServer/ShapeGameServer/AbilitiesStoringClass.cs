using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AbilitiesStoringClass
{
    public static int[][] AbilitiesArray = new int[57][];
    public static int[][] Super100Array = new int[4][];
    public static int[][] Super200Array = new int[4][];
    public static int[][] PassivesArray = new int[8][];
    public static int[][][] AbilitiesShopArray = new int[AbilitiesArray.Length][][];
    public static int[][][] Super100ShopArray = new int[Super100Array.Length][][];
    public static int[][][] Super200ShopArray = new int[Super200Array.Length][][];
    public static int[] ShapePrice = { 2500, 2500, 2500, 2500 };
    public static int[] ChestSlotPrices = { 0, 1000, 10000, 500};      
    public static int[] AbilitiesRarety = new int[AbilitiesArray.Length];
    public static int[][][] LevelStatsArray = new int[8][][];
    public static string[] NamesArray = new string[AbilitiesArray.Length];
    public static int[] EmotesPrice = new int[5] { 100, 500, 500, 1000, 100 };
    public static int[] BgPrice = new int[2] { 1000, 1000 };
    public static int[] SkinPrice = new int[2] { 0, 2000 };
    public static int[] CoinPrice = new int[5] { 10, 20, 30, 40, 50 };
    public static int[] CoinReward = new int[5] { 250, 500, 800, 1100, 1400 };
    public static int[][][] PassivesShopArray = new int[PassivesArray.Length][][];
    public static int[][][] TrophyRoadArray = new int[ShapePrice.Length + 1][][];
    public static int[] AdditionalRewardsArray = new int[9];
    public static int GameVersion = 2; // Needs to be updated everytime we update the Game.
    #region BotNames
    public static string[] botNames = new string[] { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Mia", "Amelia", "Harper", "Evelyn", "Abigail", "Emily", "Elizabeth", "Mila", "Ella", "Avery", "Sofia", "Camila", "Aria", "Scarlett", "Victoria", "Madison", "Luna", "Grace", "Chloe", "Penelope", "Layla", "Riley", "Zoey", "Nora", "Lily", "Eleanor", "Hannah", "Lillian", "Addison", "Aubrey", "Ellie", "Stella", "Natalie", "Zoe", "Leah", "Hazel", "Violet", "Aurora", "Savannah", "Audrey", "Brooklyn", "Bella", "Claire", "Skylar", "Lucy", "Paisley", "Everly", "Anna", "Caroline", "Nova", "Genesis", "Emilia", "Kennedy", "Samantha", "Maya", "Willow", "Kinsley", "Naomi", "Aaliyah", "Elena", "Sarah", "Ariana", "Allison", "Gabriella", "Alice", "Madelyn", "Cora", "Ruby", "Eva", "Serenity", "Autumn", "Adeline", "Hailey", "Gianna", "Valentina", "Isla", "Eliana", "Quinn", "Nevaeh", "Ivy", "Sadie", "Piper", "Lydia", "Alexa", "Josephine", "Emery", "Julia", "Delilah", "Arianna", "Vivian", "Kaylee", "Sophie", "Brielle", "Madeline", "Peyton", "Rylee", "Clara", "Hadley", "Melanie", "Mackenzie", "Reagan", "Adalynn", "Liliana", "Aubree", "Jade", "Katherine", "Isabelle", "Natalia", "Raelynn", "Maria", "Athena", "Ximena", "Arya", "Leilani", "Taylor", "Faith", "Rose", "Kylie", "Alexandra", "Mary", "Margaret", "Lyla", "Ashley", "Amaya", "Eliza", "Brianna", "Bailey", "Andrea", "Khloe", "Jasmine", "Melody", "Iris", "Isabel", "Norah", "Annabelle", "Valeria", "Emerson", "Adalyn", "Ryleigh", "Eden", "Emersyn", "Anastasia", "Kayla", "Alyssa", "Juliana", "Charlie", "Esther", "Ariel", "Cecilia", "Valerie", "Alina", "Molly", "Reese", "Aliyah", "Lilly", "Parker", "Finley", "Morgan", "Sydney", "Jordyn", "Eloise", "Trinity", "Daisy", "Kimberly", "Lauren", "Genevieve", "Sara", "Arabella", "Harmony", "Elise", "Remi", "Teagan", "Alexis", "London", "Sloane", "Laila", "Lucia", "Diana", "Juliette", "Sienna", "Elliana", "Londyn", "Ayla", "Callie", "Gracie", "Josie", "Amara", "Jocelyn", "Daniela", "Everleigh", "Mya", "Rachel", "Summer", "AlanaLiam", "Noah", "William", "James", "Logan", "Benjamin", "Mason", "Elijah", "Oliver", "Jacob", "Lucas", "Michael", "Alexander", "Ethan", "Daniel", "Matthew", "Aiden", "Henry", "Joseph", "Jackson", "Samuel", "Sebastian", "David", "Carter", "Wyatt", "Jayden", "John", "Owen", "Dylan", "Luke", "Gabriel", "Anthony", "Isaac", "Grayson", "Jack", "Julian", "Levi", "Christopher", "Joshua", "Andrew", "Lincoln", "Mateo", "Ryan", "Jaxon", "Nathan", "Aaron", "Isaiah", "Thomas", "Charles", "Caleb", "Josiah", "Christian", "Hunter", "Eli", "Jonathan", "Connor", "Landon", "Adrian", "Asher", "Cameron", "Leo", "Theodore", "Jeremiah", "Hudson", "Robert", "Easton", "Nolan", "Nicholas", "Ezra", "Colton", "Angel", "Brayden", "Jordan", "Dominic", "Austin", "Ian", "Adam", "Elias", "Jaxson", "Greyson", "Jose", "Ezekiel", "Carson", "Evan", "Maverick", "Bryson", "Jace", "Cooper", "Xavier", "Parker", "Roman", "Jason", "Santiago", "Chase", "Sawyer", "Gavin", "Leonardo", "Kayden", "Ayden", "Jameson", "Kevin", "Bentley", "Zachary", "Everett", "Axel", "Tyler", "Micah", "Vincent", "Weston", "Miles", "Wesley", "Nathaniel", "Harrison", "Brandon", "Cole", "Declan", "Luis", "Braxton", "Damian", "Silas", "Tristan", "Ryder", "Bennett", "George", "Emmett", "Justin", "Kai", "Max", "Diego", "Luca", "Ryker", "Carlos", "Maxwell", "Kingston", "Ivan", "Maddox", "Juan", "Ashton", "Jayce", "Rowan", "Kaiden", "Giovanni", "Eric", "Jesus", "Calvin", "Abel", "King", "Camden", "Amir", "Blake", "Alex", "Brody", "Malachi", "Emmanuel", "Jonah", "Beau", "Jude", "Antonio", "Alan", "Elliott", "Elliot", "Waylon", "Xander", "Timothy", "Victor", "Bryce", "Finn", "Brantley", "Edward", "Abraham", "Patrick", "Grant", "Karter", "Hayden", "Richard", "Miguel", "Joel", "Gael", "Tucker", "Rhett", "Avery", "Steven", "Graham", "Kaleb", "Jasper", "Jesse", "Matteo", "Dean", "Zayden", "Preston", "August", "Oscar", "Jeremy", "Alejandro", "Marcus", "Dawson", "Lorenzo", "Messiah", "Zion", "Maximus" };
    #endregion
    public static void InitializeAbilitiesArray()
    {
        #region AbilitiesArray
        AbilitiesArray[0] = new int[7] { 0, 0, 0, 0, 0, 0 , 0};
        AbilitiesArray[1] = new int[7] { 5, 10, 15, 0, 0, 0, 0 };
        AbilitiesArray[2] = new int[7] { 0, 0, 0, 15, 20, 25, 2 };
        AbilitiesArray[3] = new int[7] { 20, 25, 30, 0, 0, 0, 6 };
        AbilitiesArray[4] = new int[7] { 15, 20, 25, 0, 0, 0, 6 };
        AbilitiesArray[5] = new int[7] { 20, 25, 30, 0, 0, 0, 4 };
        AbilitiesArray[6] = new int[7] { 20, 25, 30, 20, 25, 30, 8 };
        AbilitiesArray[7] = new int[7] { 0, 0, 0, 0, 0, 0, 3 };
        AbilitiesArray[8] = new int[7] { 0, 0, 0, 0, 0, 0, 1 }; 
        AbilitiesArray[9] = new int[7] { 1, 2, 3, 0, 0, 0, 0 };
        AbilitiesArray[10] = new int[7] { 10, 15, 20, 0, 0, 0, 2 };
        AbilitiesArray[11] = new int[7] { 0, 0, 0, 0, 0, 0, 4 };
        AbilitiesArray[12] = new int[7] { 15, 20, 25, 0, 0, 0, 3 };
        AbilitiesArray[13] = new int[7] { 15, 20, 25, 0, 0, 0, 3 };
        AbilitiesArray[14] = new int[7] { 0, 0, 0, 15, 20, 25, 2 };
        AbilitiesArray[15] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[16] = new int[7] { 0, 0, 0, 20, 25, 30, 2 };
        AbilitiesArray[17] = new int[7] { 20, 25, 30, 0, 0, 0, 6 };
        AbilitiesArray[18] = new int[7] { 0, 0, 0, 0, 0, 0, 2 };
        AbilitiesArray[19] = new int[7] { 10, 15, 20, 0, 0, 0, 3 };
        AbilitiesArray[20] = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        AbilitiesArray[21] = new int[7] { 20, 25, 30, 0, 0, 0, 6 };
        AbilitiesArray[22] = new int[7] { 5, 10, 15, 0, 0, 0, 0 };
        AbilitiesArray[23] = new int[7] { 5, 10, 15, 0, 0, 0, 0 };
        AbilitiesArray[24] = new int[7] { 0, 0, 0, 20, 25, 30, 2 };
        AbilitiesArray[25] = new int[7] { 5, 10, 15, 30, 35, 40, 13 };
        AbilitiesArray[26] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[27] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[28] = new int[7] { 25, 30, 35, 0, 0, 0, 6 };
        AbilitiesArray[29] = new int[7] { 25, 30, 35, 0, 0, 0, 6 };
        AbilitiesArray[30] = new int[7] { 20, 25, 30, 0, 0, 0, 6 };
        AbilitiesArray[31] = new int[7] { 20, 25, 30, 0, 0, 0, 4 };
        AbilitiesArray[32] = new int[7] { 10, 15, 20, 0, 0, 0, 2 };
        AbilitiesArray[33] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[34] = new int[7] { 10, 15, 20, 10, 10, 10, 1};
        AbilitiesArray[35] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[36] = new int[7] { 0, 0, 0, 10, 15, 20, 1 };
        AbilitiesArray[37] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[38] = new int[7] { 20, 25, 30, 20, 25, 30, 8};
        AbilitiesArray[39] = new int[7] { 25, 30, 35, 0, 0, 0, 6 };
        AbilitiesArray[40] = new int[7] { 15, 20, 25, 0, 0, 0, 4 };
        AbilitiesArray[41] = new int[7] { 5, 7, 10, 0, 0, 0, 3 };
        AbilitiesArray[42] = new int[7] { 5, 10, 15, 0, 0, 0, 0 };
        AbilitiesArray[43] = new int[7] { 15, 20, 25, 0, 0, 0, 2};
        AbilitiesArray[44] = new int[7] { 15, 20, 25, 0, 0, 0, 2 };
        AbilitiesArray[45] = new int[7] { 20, 25, 30, 0, 0, 0, 4 };
        AbilitiesArray[46] = new int[7] { 20, 25, 30, 20, 25, 30, 8 };
        AbilitiesArray[47] = new int[7] { 25, 30, 35, 0, 0, 0, 6 };
        AbilitiesArray[48] = new int[7] { 20, 25, 30, 0, 0, 0, 6 };
        AbilitiesArray[49] = new int[7] { 0, 0, 0, 20, 25, 30, 2 };
        AbilitiesArray[50] = new int[7] { 10, 15, 20, 0, 0, 0, 2 };
        AbilitiesArray[51] = new int[7] { 15, 20, 25, 0, 0, 0, 4 };
        AbilitiesArray[52] = new int[7] { 25, 30, 35, 0, 0, 0, 8 };
        AbilitiesArray[53] = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        AbilitiesArray[54] = new int[7] { 0, 0, 0, 0, 0, 0, 3 };
        AbilitiesArray[55] = new int[7] { 0, 0, 0, 20, 25, 30, 3 };
        AbilitiesArray[56] = new int[7] { 0, 0, 0, 20, 25, 30, 3 };
        #endregion
        #region Super100Array
        Super100Array[0] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super100Array[1] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super100Array[2] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super100Array[3] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        #endregion
        #region Super200Array
        Super200Array[0] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super200Array[1] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super200Array[2] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        Super200Array[3] = new int[7] { 40, 45, 50, 0, 0, 0, 15 };
        #endregion
        #region PassivesArray
        PassivesArray[0] = new int[6] { 1 ,1 , 2, 10, 20, 20 };
        PassivesArray[1] = new int[6] { 5, 10, 15, 10, 10, 10 };
        PassivesArray[2] = new int[6] { 1, 1, 2, 5, 10, 10 };
        PassivesArray[3] = new int[6] { 5, 10, 15, 10, 10, 10 };
        PassivesArray[4] = new int[6] { 5, 5, 10, 10, 20, 20 };
        PassivesArray[5] = new int[6] { 5, 10, 15, 10, 10, 10 };
        PassivesArray[6] = new int[6] { 5, 10, 15, 10, 10, 10 };
        PassivesArray[7] = new int[6] { 1, 1, 1, 5, 10, 20 };
        #endregion
        #region AbilitiesShopArray
        AbilitiesShopArray[0] = new int[3][];
        AbilitiesShopArray[0][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[0][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[0][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[1] = new int[3][];
        AbilitiesShopArray[1][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[1][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[1][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[2] = new int[3][];
        AbilitiesShopArray[2][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[2][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[2][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[3] = new int[3][];
        AbilitiesShopArray[3][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[3][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[3][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[4] = new int[3][];
        AbilitiesShopArray[4][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[4][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[4][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[5] = new int[3][];
        AbilitiesShopArray[5][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[5][1] = new int[3] { 650, 10, 2 };
        AbilitiesShopArray[5][2] = new int[3] { 800, 20, 3 };
        AbilitiesShopArray[6] = new int[3][];
        AbilitiesShopArray[6][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[6][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[6][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[7] = new int[3][];
        AbilitiesShopArray[7][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[7][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[7][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[8] = new int[3][];
        AbilitiesShopArray[8][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[8][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[8][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[9] = new int[3][];
        AbilitiesShopArray[9][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[9][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[9][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[10] = new int[3][];
        AbilitiesShopArray[10][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[10][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[10][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[11] = new int[3][];
        AbilitiesShopArray[11][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[11][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[11][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[12] = new int[3][];
        AbilitiesShopArray[12][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[12][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[12][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[13] = new int[3][];
        AbilitiesShopArray[13][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[13][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[13][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[14] = new int[3][];
        AbilitiesShopArray[14][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[14][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[14][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[15] = new int[3][];
        AbilitiesShopArray[15][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[15][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[15][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[16] = new int[3][];
        AbilitiesShopArray[16][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[16][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[16][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[17] = new int[3][];
        AbilitiesShopArray[17][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[17][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[17][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[18] = new int[3][];
        AbilitiesShopArray[18][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[18][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[18][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[19] = new int[3][];
        AbilitiesShopArray[19][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[19][1] = new int[3] { 650, 10, 2 };
        AbilitiesShopArray[19][2] = new int[3] { 800, 20, 3 };
        AbilitiesShopArray[20] = new int[3][];
        AbilitiesShopArray[20][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[20][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[20][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[21] = new int[3][];
        AbilitiesShopArray[21][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[21][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[21][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[22] = new int[3][];
        AbilitiesShopArray[22][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[22][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[22][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[23] = new int[3][];
        AbilitiesShopArray[23][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[23][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[23][2] = new int[3] { 450, 10, 3 };
        // l/RFuFNjdJ2c$T(
        AbilitiesShopArray[24] = new int[3][];
        AbilitiesShopArray[24][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[24][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[24][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[25] = new int[3][];
        AbilitiesShopArray[25][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[25][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[25][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[26] = new int[3][];
        AbilitiesShopArray[26][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[26][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[26][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[27] = new int[3][];
        AbilitiesShopArray[27][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[27][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[27][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[28] = new int[3][];
        AbilitiesShopArray[28][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[28][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[28][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[29] = new int[3][];
        AbilitiesShopArray[29][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[29][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[29][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[30] = new int[3][];
        AbilitiesShopArray[30][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[30][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[30][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[31] = new int[3][];
        AbilitiesShopArray[31][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[31][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[31][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[32] = new int[3][];
        AbilitiesShopArray[32][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[32][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[32][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[33] = new int[3][];
        AbilitiesShopArray[33][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[33][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[33][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[34] = new int[3][];
        AbilitiesShopArray[34][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[34][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[34][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[35] = new int[3][];
        AbilitiesShopArray[35][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[35][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[35][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[36] = new int[3][];
        AbilitiesShopArray[36][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[36][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[36][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[37] = new int[3][];
        AbilitiesShopArray[37][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[37][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[37][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[38] = new int[3][];
        AbilitiesShopArray[38][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[38][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[38][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[39] = new int[3][];
        AbilitiesShopArray[39][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[39][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[39][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[40] = new int[3][];
        AbilitiesShopArray[40][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[40][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[40][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[41] = new int[3][];
        AbilitiesShopArray[41][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[41][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[41][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[42] = new int[3][];
        AbilitiesShopArray[42][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[42][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[42][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[43] = new int[3][];
        AbilitiesShopArray[43][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[43][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[43][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[44] = new int[3][];
        AbilitiesShopArray[44][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[44][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[44][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[45] = new int[3][];
        AbilitiesShopArray[45][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[45][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[45][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[46] = new int[3][];
        AbilitiesShopArray[46][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[46][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[46][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[47] = new int[3][];
        AbilitiesShopArray[47][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[47][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[47][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[48] = new int[3][];
        AbilitiesShopArray[48][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[48][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[48][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[49] = new int[3][];
        AbilitiesShopArray[49][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[49][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[49][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[50] = new int[3][];
        AbilitiesShopArray[50][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[50][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[50][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[51] = new int[3][];
        AbilitiesShopArray[51][0] = new int[3] { 300, 0, 1 };
        AbilitiesShopArray[51][1] = new int[3] { 450, 10, 2 };
        AbilitiesShopArray[51][2] = new int[3] { 600, 20, 3 };
        AbilitiesShopArray[52] = new int[3][];
        AbilitiesShopArray[52][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[52][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[52][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[53] = new int[3][];
        AbilitiesShopArray[53][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[53][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[53][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[54] = new int[3][];
        AbilitiesShopArray[54][0] = new int[3] { 500, 0, 1 };
        AbilitiesShopArray[54][1] = new int[3] { 650, 15, 2 };
        AbilitiesShopArray[54][2] = new int[3] { 800, 30, 3 };
        AbilitiesShopArray[55] = new int[3][];
        AbilitiesShopArray[55][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[55][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[55][2] = new int[3] { 450, 10, 3 };
        AbilitiesShopArray[56] = new int[3][];
        AbilitiesShopArray[56][0] = new int[3] { 150, 0, 1 };
        AbilitiesShopArray[56][1] = new int[3] { 300, 5, 2 };
        AbilitiesShopArray[56][2] = new int[3] { 450, 10, 3 };
        #endregion
        #region Super100ShopArray
        Super100ShopArray[0] = new int[3][];
        Super100ShopArray[0][0] = new int[3] { 2000, 500, 4 };
        Super100ShopArray[0][1] = new int[3] { 4000, 1000, 6 };
        Super100ShopArray[0][2] = new int[3] { 8000, 2000, 8 };
        Super100ShopArray[1] = new int[3][];
        Super100ShopArray[1][0] = new int[3] { 2000, 500, 4 };
        Super100ShopArray[1][1] = new int[3] { 4000, 1000, 6 };
        Super100ShopArray[1][2] = new int[3] { 8000, 2000, 8 };
        Super100ShopArray[2] = new int[3][];
        Super100ShopArray[2][0] = new int[3] { 2000, 500, 4 };
        Super100ShopArray[2][1] = new int[3] { 4000, 1000, 6 };
        Super100ShopArray[2][2] = new int[3] { 8000, 2000, 8 };
        Super100ShopArray[3] = new int[3][];
        Super100ShopArray[3][0] = new int[3] { 2000, 500, 4 };
        Super100ShopArray[3][1] = new int[3] { 4000, 1000, 6 };
        Super100ShopArray[3][2] = new int[3] { 8000, 2000, 8 };
        #endregion
        #region Super200ShopArray
        Super200ShopArray[0] = new int[3][];
        Super200ShopArray[0][0] = new int[3] { 2000, 500, 4 };
        Super200ShopArray[0][1] = new int[3] { 4000, 1000, 6 };
        Super200ShopArray[0][2] = new int[3] { 8000, 2000, 8 };
        Super200ShopArray[1] = new int[3][];
        Super200ShopArray[1][0] = new int[3] { 2000, 500, 4 };
        Super200ShopArray[1][1] = new int[3] { 4000, 1000, 6 };
        Super200ShopArray[1][2] = new int[3] { 8000, 2000, 8 };
        Super200ShopArray[2] = new int[3][];
        Super200ShopArray[2][0] = new int[3] { 2000, 500, 4 };
        Super200ShopArray[2][1] = new int[3] { 4000, 1000, 6 };
        Super200ShopArray[2][2] = new int[3] { 8000, 2000, 8 };
        Super200ShopArray[3] = new int[3][];
        Super200ShopArray[3][0] = new int[3] { 2000, 500, 4 };
        Super200ShopArray[3][1] = new int[3] { 4000, 1000, 6 };
        Super200ShopArray[3][2] = new int[3] { 8000, 2000, 8 };
        #endregion
        #region PassivesShopArray
        PassivesShopArray[0] = new int[3][];
        PassivesShopArray[0][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[0][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[0][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[1] = new int[3][];
        PassivesShopArray[1][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[1][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[1][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[2] = new int[3][];
        PassivesShopArray[2][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[2][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[2][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[3] = new int[3][];
        PassivesShopArray[3][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[3][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[3][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[4] = new int[3][];
        PassivesShopArray[4][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[4][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[4][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[4] = new int[3][];
        PassivesShopArray[4][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[4][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[4][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[5] = new int[3][];
        PassivesShopArray[5][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[5][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[5][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[6] = new int[3][];
        PassivesShopArray[6][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[6][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[6][2] = new int[3] { 1000, 500, 8 };
        PassivesShopArray[7] = new int[3][];
        PassivesShopArray[7][0] = new int[3] { 100, 50, 2 };
        PassivesShopArray[7][1] = new int[3] { 400, 200, 4 };
        PassivesShopArray[7][2] = new int[3] { 1000, 500, 8 };
        #endregion
        #region LevelStatsArray
        LevelStatsArray[0] = new int[5][];
        LevelStatsArray[0][0] = new int[3] { 0, 0, 0};
        LevelStatsArray[0][1] = new int[3] { 0, 0, 0};
        LevelStatsArray[0][2] = new int[3] { 0, 0, 0 };
        LevelStatsArray[0][3] = new int[3] { 0, 0, 0 };
        LevelStatsArray[0][4] = new int[3] { 500, 200, 5 };
        LevelStatsArray[1] = new int[5][];
        LevelStatsArray[1][0] = new int[3] { 0, 3, 10 };
        LevelStatsArray[1][1] = new int[3] { 2, 2, 10 };
        LevelStatsArray[1][2] = new int[3] { 3, 0, 10 };
        LevelStatsArray[1][3] = new int[3] { 1, 1, 10 };
        LevelStatsArray[1][4] = new int[3] { 1000, 500, 10 };
        LevelStatsArray[2] = new int[5][];
        LevelStatsArray[2][0] = new int[3] { 2, 5, 25 };
        LevelStatsArray[2][1] = new int[3] { 4, 4, 25 };
        LevelStatsArray[2][2] = new int[3] { 5, 2, 25 };
        LevelStatsArray[2][3] = new int[3] { 3, 3, 25 };
        LevelStatsArray[2][4] = new int[3] { 2000, 1000, 20 };
        LevelStatsArray[3] = new int[5][];
        LevelStatsArray[3][0] = new int[3] { 4, 7, 40 };
        LevelStatsArray[3][1] = new int[3] { 6, 6, 40 };
        LevelStatsArray[3][2] = new int[3] { 7, 4, 40 };
        LevelStatsArray[3][3] = new int[3] { 5, 5, 40 };
        LevelStatsArray[3][4] = new int[3] { 4000, 2000, 35 };
        LevelStatsArray[4] = new int[5][];
        LevelStatsArray[4][0] = new int[3] { 6, 9, 55 };
        LevelStatsArray[4][1] = new int[3] { 8, 8, 55 };
        LevelStatsArray[4][2] = new int[3] { 9, 6, 55 };
        LevelStatsArray[4][3] = new int[3] { 7, 7, 55 };
        LevelStatsArray[4][4] = new int[3] { 6000, 4000, 45 };
        LevelStatsArray[5] = new int[5][];
        LevelStatsArray[5][0] = new int[3] { 8, 11, 70 };
        LevelStatsArray[5][1] = new int[3] { 10, 10, 70 };
        LevelStatsArray[5][2] = new int[3] { 11, 8, 70 };
        LevelStatsArray[5][3] = new int[3] { 9, 9, 70 };
        LevelStatsArray[5][4] = new int[3] { 8000, 6000, 60 };
        LevelStatsArray[6] = new int[5][];
        LevelStatsArray[6][0] = new int[3] { 10, 13, 85 };
        LevelStatsArray[6][1] = new int[3] { 12, 12, 85 };
        LevelStatsArray[6][2] = new int[3] { 13, 10, 85 };
        LevelStatsArray[6][3] = new int[3] { 11, 11, 85 };
        LevelStatsArray[6][4] = new int[3] { 10000, 8500, 70 };
        LevelStatsArray[7] = new int[5][];
        LevelStatsArray[7][0] = new int[3] { 12, 15, 100 };
        LevelStatsArray[7][1] = new int[3] { 14, 14, 100 };
        LevelStatsArray[7][2] = new int[3] { 15, 12, 100 };
        LevelStatsArray[7][3] = new int[3] { 13, 13, 100 };
        LevelStatsArray[7][4] = new int[3] { 15000, 12000, 100 };
        #endregion
        #region AbilitiesRarety
        AbilitiesRarety[0] = 0;
        AbilitiesRarety[1] = 0;
        AbilitiesRarety[2] = 0;
        AbilitiesRarety[3] = 2;
        AbilitiesRarety[4] = 2;
        AbilitiesRarety[5] = 1;
        AbilitiesRarety[6] = 2;
        AbilitiesRarety[7] = 2;
        AbilitiesRarety[8] = 1;
        AbilitiesRarety[9] = 2;
        AbilitiesRarety[10] = 0;
        AbilitiesRarety[11] = 2;
        AbilitiesRarety[12] = 1;
        AbilitiesRarety[13] = 0;
        AbilitiesRarety[14] = 0;
        AbilitiesRarety[15] = 1;
        AbilitiesRarety[16] = 0;
        AbilitiesRarety[17] = 2;
        AbilitiesRarety[18] = 2;
        AbilitiesRarety[19] = 1;
        AbilitiesRarety[20] = -1;
        AbilitiesRarety[21] = 2;
        AbilitiesRarety[22] = 0;
        AbilitiesRarety[23] = 0;
        AbilitiesRarety[24] = 0;
        AbilitiesRarety[25] = 2;
        AbilitiesRarety[26] = 2;
        AbilitiesRarety[27] = 1;
        AbilitiesRarety[28] = 2;
        AbilitiesRarety[29] = 1;
        AbilitiesRarety[30] = 2;
        AbilitiesRarety[31] = 1;
        AbilitiesRarety[32] = 0;
        AbilitiesRarety[33] = 2;
        AbilitiesRarety[34] = 0;
        AbilitiesRarety[35] = 1;
        AbilitiesRarety[36] = 0;
        AbilitiesRarety[37] = 2;
        AbilitiesRarety[38] = 2;
        AbilitiesRarety[39] = 1;
        AbilitiesRarety[40] = 1;
        AbilitiesRarety[41] = 2;
        AbilitiesRarety[42] = 0;
        AbilitiesRarety[43] = 1;
        AbilitiesRarety[44] = 2;
        AbilitiesRarety[45] = 1;
        AbilitiesRarety[46] = 2;
        AbilitiesRarety[47] = 1;
        AbilitiesRarety[48] = 1;
        AbilitiesRarety[49] = 0;
        AbilitiesRarety[50] = 0;
        AbilitiesRarety[51] = 1;
        AbilitiesRarety[52] = 2;
        AbilitiesRarety[53] = -1;
        AbilitiesRarety[54] = 2;
        AbilitiesRarety[55] = 0;
        AbilitiesRarety[56] = 0;
        #endregion
        #region NamesArray
        NamesArray[0] = "Get PP";
        NamesArray[1] = "AttackCube";
        NamesArray[2] = "Shield";
        NamesArray[3] = "Tackle";
        NamesArray[4] = "Shadow Attack";
        NamesArray[5] = "Mini Cubes Barrage";
        NamesArray[6] = "Cube Sealing";
        NamesArray[7] = "Elevation";
        NamesArray[8] = "Devil's Deal";
        NamesArray[9] = "Energy Drain";
        NamesArray[10] = "Aerial Strike";
        NamesArray[11] = "Mirror Defense";
        NamesArray[12] = "Earthquake";
        NamesArray[13] = "Eye Of Horus";
        NamesArray[14] = "Ground Of Steel";
        NamesArray[15] = "FireBlast";
        NamesArray[16] = "WaterFall";
        NamesArray[17] = "Spin Little Star";
        NamesArray[18] = "Curse Of The Cloud";
        NamesArray[19] = "Fountain Attack";
        NamesArray[20] = "NOTHING";
        NamesArray[21] = "Rotating Snow Spear";
        NamesArray[22] = "AttackPyr";
        NamesArray[23] = "AttackStar";
        NamesArray[24] = "Fire Shield";
        NamesArray[25] = "Mate";
        NamesArray[26] = "Assist";
        NamesArray[27] = "Ice Disk";
        NamesArray[28] = "Ice Shards";
        NamesArray[29] = "Fireball";
        NamesArray[30] = "Fire Bridge";
        NamesArray[31] = "Fire Laser";
        NamesArray[32] = "Fire Bomb";
        NamesArray[33] = "Fire Boost";
        NamesArray[34] = "DoubleEdged Swords";
        NamesArray[35] = "Bubble Attack";
        NamesArray[36] = "Bubble Shield";
        NamesArray[37] = "BluePlanetBackup";
        NamesArray[38] = "Water Cage";
        NamesArray[39] = "Mud Attack";
        NamesArray[40] = "Mud Eruption";
        NamesArray[41] = "Healing Circle";
        NamesArray[42] = "AttackSphere";
        NamesArray[43] = "Toxic Shot";
        NamesArray[44] = "Poisonous Air";
        NamesArray[45] = "Poisonous Bubble";
        NamesArray[46] = "Toxic Ring";
        NamesArray[47] = "Air Cannon";
        NamesArray[48] = "Poison Cloud";
        NamesArray[49] = "Upwards Spiral";
        NamesArray[50] = "Wind Arrow";
        NamesArray[51] = "Mini Nado";
        NamesArray[52] = "Toxic Hammer";
        NamesArray[53] = "NOTHING";
        NamesArray[54] = "Shadow Dissolve";
        NamesArray[55] = "Flame Shield";
        NamesArray[56] = "Downwards Spiral";
        #endregion
        #region TrophyRoadArray
        TrophyRoadArray[0] = new int[7][];
        TrophyRoadArray[0][0] = new int[6] { 50, 10, 0, -1, -1, -1 };
        TrophyRoadArray[0][1] = new int[6] { 75, 15, 0, -1, -1, -1 };
        TrophyRoadArray[0][2] = new int[6] { 100, 20, 0, -1, -1, -1 };
        TrophyRoadArray[0][3] = new int[6] { 0, 0, 0, -1, 12, -1 };
        TrophyRoadArray[0][4] = new int[6] { 0, 0, 5, -1, -1, -1 };
        TrophyRoadArray[0][5] = new int[6] { 125, 20, 0, -1, -1, -1 };
        TrophyRoadArray[0][6] = new int[6] { 150, 25, 0, -1, -1, -1 };
        TrophyRoadArray[1] = new int[7][];
        TrophyRoadArray[1][0] = new int[6] { 50, 10, 0, -1, -1, -1 };
        TrophyRoadArray[1][1] = new int[6] { 75, 15, 0, -1, -1, -1 };
        TrophyRoadArray[1][2] = new int[6] { 100, 20, 0, -1, -1, -1 };
        TrophyRoadArray[1][3] = new int[6] { 0, 0, 0, 13, -1, -1 };
        TrophyRoadArray[1][4] = new int[6] { 0, 0, 5, -1, -1, -1 };
        TrophyRoadArray[1][5] = new int[6] { 125, 20, 0, -1, -1, -1 };
        TrophyRoadArray[1][6] = new int[6] { 150, 25, 0, -1, -1, -1 };
        TrophyRoadArray[2] = new int[7][];
        TrophyRoadArray[2][0] = new int[6] { 50, 10, 0, -1, -1, -1 };
        TrophyRoadArray[2][1] = new int[6] { 75, 15, 0, -1, -1, -1 };
        TrophyRoadArray[2][2] = new int[6] { 100, 20, 0, -1, -1, -1 };
        TrophyRoadArray[2][3] = new int[6] { 0, 0, 0, -1, -1, 30 };
        TrophyRoadArray[2][4] = new int[6] { 0, 0, 5, -1, -1, -1 };
        TrophyRoadArray[2][5] = new int[6] { 125, 20, 0, -1, -1, -1 };
        TrophyRoadArray[2][6] = new int[6] { 150, 25, 0, -1, -1, -1 };
        TrophyRoadArray[3] = new int[7][];
        TrophyRoadArray[3][0] = new int[6] { 50, 10, 0, -1, -1, -1 };
        TrophyRoadArray[3][1] = new int[6] { 75, 15, 0, -1, -1, -1 };
        TrophyRoadArray[3][2] = new int[6] { 100, 20, 0, -1, -1, -1 };
        TrophyRoadArray[3][3] = new int[6] { 0, 0, 0, 50, -1, -1 };
        TrophyRoadArray[3][4] = new int[6] { 0, 0, 5, -1, -1, -1 };
        TrophyRoadArray[3][5] = new int[6] { 125, 20, 0, -1, -1, -1 };
        TrophyRoadArray[3][6] = new int[6] { 150, 25, 0, -1, -1, -1 };
        TrophyRoadArray[4] = new int[7][];
        TrophyRoadArray[4][0] = new int[6] { 150, 20, 0, -1, -1, -1 };
        TrophyRoadArray[4][1] = new int[6] { 200, 30, 0, -1, -1, -1 };
        TrophyRoadArray[4][2] = new int[6] { 250, 40, 0, -1, -1, -1 };
        TrophyRoadArray[4][3] = new int[6] { 0, 0, 15, -1, -1, -1 };
        TrophyRoadArray[4][4] = new int[6] { 300, 50, 0, -1, -1, -1 };
        TrophyRoadArray[4][5] = new int[6] { 350, 60, 0, -1, -1, -1 };
        TrophyRoadArray[4][6] = new int[6] { 0, 0, 0, -1, -1, 7 };
        #endregion
        #region AdditionalRewardsArray
        AdditionalRewardsArray[0] = 10;
        AdditionalRewardsArray[1] = 20;
        AdditionalRewardsArray[2] = 30;
        AdditionalRewardsArray[3] = 50;
        AdditionalRewardsArray[4] = 70;
        AdditionalRewardsArray[5] = 90;
        AdditionalRewardsArray[6] = 120;
        AdditionalRewardsArray[7] = 150;
        AdditionalRewardsArray[8] = 200;
        #endregion

    }
    public static string TransformToString(int[][] AbilitiesAr)
    {
        string[][] AbilitiesArraystr = new string[AbilitiesAr.Length][];
        string Abilities ="";
        bool firsttime = true;
        try
        {
            int i = 0;
            foreach (int[] c in AbilitiesAr)
            {
                AbilitiesArraystr[i] = new string[c.Length];
                int j = 0;
                foreach(int d in c)
                {
                    AbilitiesArraystr[i][j] = d.ToString();
                    j++;
                }
                i++;
            }
            foreach (string[] d in AbilitiesArraystr)
            {
                if (firsttime)
                {
                    Abilities = String.Join(",", d);
                    firsttime = false;
                }
                else
                {
                    Abilities = Abilities + "-" + String.Join(",", d);
                }
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
        return Abilities;
    }
    public static string TransformToStringSuper100()
    {
        string[][] Super100str = new string[Super100Array.Length][];
        string Super100 = "";
        bool firsttime = true;
        int i = 0;
        try
        {
            foreach (int[] c in Super100Array)
            {
                Super100str[i] = new string[7];
                for (int j = 0; j < 7; j++)
                {
                    Super100str[i][j] = c[j].ToString();
                }
                i++;
            }
            foreach (string[] d in Super100str)
            {
                if (firsttime)
                {
                    Super100 = String.Join(",", d);
                    firsttime = false;
                }
                else
                {
                    Super100 = Super100 + "-" + String.Join(",", d);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        };
        return Super100;
    }
    public static string TransformToStringSuper200()
    {
        string[][] Super200str = new string[Super200Array.Length][];
        string Super200 = "";
        int i = 0;
        bool firsttime = true;
        try
        {
            foreach (int[] c in Super200Array)
            {
                Super200str[i] = new string[7];
                for (int j = 0; j < 7; j++)
                {
                    Super200str[i][j] = c[j].ToString();
                }
                i++;
            }
            foreach (string[] d in Super200str)
            {
                if (firsttime)
                {
                    Super200 = String.Join(",", d);
                    firsttime = false;
                }
                else
                {
                    Super200 = Super200 + "-" + String.Join(",", d);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
        return Super200;
    }
    public static string TransformToStringShop(int[][][] Array)
    {
        string[] Strings2 = new string[Array.Length];
        string Abilities = "";
        try
        {
            int Count3 = 0;
            foreach(int[][] c in Array)
            {
                int Count2 = 0;
                string[] Strings = new string[c.Length];
                foreach (int[] d in c)
                {
                    string[] Str = new string[d.Length];
                    int Count = 0;
                    foreach (int i in d)
                    {
                        Str[Count] = i.ToString();
                        Count++;
                    }
                    Strings[Count2] = String.Join(",", Str);
                    Count2++;
                }
                Strings2[Count3] = String.Join("/", Strings);
                Count3++;
            }
            Abilities = String.Join("|", Strings2);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
        return Abilities;
    }
    public static string TransformToStringShape()
    {
        string[] ShapePriceStr = new string[ShapePrice.Length];
        int i = 0;
        foreach(int c in ShapePrice)
        {
            ShapePriceStr[i] = c.ToString();
            i++;
        }
        return String.Join(",", ShapePriceStr);
    }
    public static string TransformToStringChestSlot()
    {
        string[] ChestSlotStr = new string[ChestSlotPrices.Length];
        int i = 0;
        foreach (int c in ChestSlotPrices)
        {
            ChestSlotStr[i] = c.ToString();
            i++;
        }
        return String.Join(",", ChestSlotStr);
    }
    public static string TransformToStringLevel()
    {
        string[][][] StringAr = new string[LevelStatsArray.Length][][];
        int i = 0;
        string Finalstring = "";
        foreach(int[][] c in LevelStatsArray)
        {
            StringAr[i] = new string[c.Length][];
            int j = 0;
            foreach(int[] d in c)
            {
                StringAr[i][j] = new string[d.Length];
                int h = 0;
                foreach(int e in d)
                {
                    StringAr[i][j][h] = e.ToString();
                    h++;
                }
                if(j == 0)
                {
                    Finalstring = Finalstring + String.Join(",", StringAr[i][j]);
                }
                else
                {
                    Finalstring = Finalstring + "/" + String.Join(",", StringAr[i][j]);
                }
                j++;
            }
            if(i != 7)
                Finalstring = Finalstring + "|";
            i++;
        }
        return Finalstring;
    }
}

