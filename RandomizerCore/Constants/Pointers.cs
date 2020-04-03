using System.Collections.Generic;

namespace RandomizerCore.Constants
{
    public static class Pointers
    {
        public static int[] FireLocations = { 0x20850, 0x22850, 0x24850, 0x26850, 0x28850, 0x2a850, 0x2c850, 0x2e850, 0x36850, 0x32850, 0x34850, 0x38850 };
        
        public static int[] SpellTextPointers = { 0xEFEC, 0xEFFE, 0xF014, 0xF02A, 0xF05A, 0xf070, 0xf088, 0xf08e };
        
        public static int[] OutsidePalaceBricks = { 0x10485, 0x10495, 0x104A5, 0x104B5, 0x104C5, 0x104D5, 0x14023 };
        
        public static int[] InsidePalaceBricks = { 0x13F15, 0x13F25, 0x13F35, 0x13F45, 0x13F55, 0x13F65, 0x14033 };
        
        public static int[] InsidePalaceWindows = { 0x13F19, 0x13F29, 0x13F39, 0x13F49, 0x13F59, 0x13F69, 0x14027 };
        
        public static int[] InsidePalaceCurtains = { 0x13F1D, 0x13F2D, 0x13F3D, 0x13F4D, 0x13F5D, 0x13F6D, 0x1402B };
        
        public static int[] BrickSpritePointers = { 0x29650, 0x2B650, 0x2D650, 0x33650, 0x35650, 0x37650, 0x39650 };
        
        public static int[] InsidePalaceBrickSprites = { 0x29690, 0x2B690, 0x2D690, 0x33690, 0x35690, 0x37690, 0x39690 };
        
        public static int[] PalacePallates = { 0, 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60 };
        
        public static int[] PalaceGraphics = { 0, 0x04, 0x05, 0x09, 0x0A, 0x0B, 0x0C, 0x06 };
        
        public static int OverworldXOffset = 0x3F;
        
        public static int OverworldMapOffset = 0x7E;
        
        public static int OverworldWorldOffset = 0xBD;

        public static int[] Drops = { 0x8a, 0x8b, 0x8c, 0x8d, 0x90, 0x91, 0x92, 0x88 };

        public static SortedDictionary<int, int> PalaceConnectionLocations = new SortedDictionary<int, int>
        {
            {1, 0x1072B},
            {2, 0x1072B},
            {3, 0x12208},
            {4, 0x12208},
            {5, 0x1072B},
            {6, 0x12208},
            {7, 0x1472B},
        };

        public static Dictionary<int,int> PalaceAddresses = new Dictionary<int, int>
        {
            {1, 0x4663 },
            {2, 0x4664 },
            {3, 0x4665 },
            {4, 0xA140 },
            {5, 0x8663 },
            {6, 0x8664 },
            {7, 0x8665 }
        };

        public static int EnemyAddressOne = 0x108B0;
        
        public static int EnemyAddressTwo = 0x148B0;

        public static int EnemyPointerOne = 0x105B1;

        public static int EnemyPointerTwo = 0x1208E;

        public static int EnemyPointerThree = 0x145B1;

        public static List<int> EnemiesOne = new List<int> { 3, 4, 12, 17, 18, 24, 25, 26, 29, 0x1E, 0x1F, 0x23 };
        
        public static List<int> EnemiesTwo = new List<int> { 3, 4, 12, 17, 24, 25, 26, 29, 0x1F, 0x1E, 0x23 };
        
        public static List<int> EnemiesThree = new List<int> { 3, 4, 17, 18, 24, 25, 26, 0x1D };

        public static List<int> FlyingEnemiesOne = new List<int> { 0x06, 0x07, 0x0E };

        public static List<int> FlyingEnemiesTwo = new List<int> { 0x06, 0x07, 0x0E };

        public static List<int> FlyingEnemiesThree = new List<int> { 0x06, 0x14, 0x15, 0x17, 0x1E };

        public static List<int> GeneratorsOne = new List<int> { 0x0B, 0x0F, 0x1B, 0x0A };

        public static List<int> GeneratorsTwo = new List<int> { 0x1B };

        public static List<int> GeneratorsThree = new List<int> { 0x0B, 0x0C, 0x0F, 0x16 };

        public static List<int> ShortEnemiesOne = new List<int> { 0x03, 0x04, 0x11, 0x12 };
        
        public static List<int> ShortEnemiesTwo = new List<int> { 0x03, 0x04, 0x11 };

        public static List<int> ShortEnemiesThree = new List<int> { 0x03, 0x04, 0x11, 0x12 };

        public static List<int> TallEnemiesOne = new List<int> { 0x0C, 0x18, 0x19, 0x1A, 0x1D, 0x1E, 0x1F, 0x23 };
        
        public static List<int> TallEnemiesTwo = new List<int> { 0x0C, 0x18, 0x19, 0x1A, 0x1D, 0x1F, 0x1E, 0x23 };
        
        public static List<int> TallEnemiesThree = new List<int> { 0x18, 0x19, 0x1A, 0x1D };
    }
}