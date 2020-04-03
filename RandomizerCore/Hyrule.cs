using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RandomizerCore.Constants;
using RandomizerCore.Constants.Enums;
using RandomizerCore.Constants.Places;
using RandomizerCore.Constants.Towns;
using RandomizerCore.Sprites;
using RandomizerCore.Text;

namespace Z2Randomizer
{
    /*
     * 
    Change List:
        * Fixed a bug with random hidden palace/new kasuto
        * Fixed a bug that should hopefully speed up Seed generation a bit
        * Added an option to allow users to select their beam sprite
        * Added a whole bunch of new playable character sprites (thanks Knightcrawler, Lufia, and Plan!)
        * Added two forms of helpful hints
        * Items in the drop pool are now guaranteed to appear in the pool at least once
        * Added an option to standardize drops
        * Added an option to randomize which items are in the item pool
        * Added some additional community hints (too many people to thank for this one)
        * Various UI tweaks and fixes


    Menu to magic: location E15? Three NOPs?

    To remove wizard, upstab, riverman text change the following to 0xFF:
        0xE563 - shield (pointer a553, at 0xEFEC)
        0xE68A - jump (pointer a67a, at 0xEFFE)
        0xE7E9 - life (pointer a7d9, at 0xF014)
        0xE833 - You know bagu
        0xE989 - fairy (pointer a979, at 0xF02A)
        0xE9AE - when you jump ....
        0xEC23 - fire (pointer ac13, at 0xF05A)
        0xEDAA - reflect (pointer ad9a, at 0xf070)
        0xEF1F - spell (pointer af0f, at 0xf088)
        0xEF81 - thunder (pointer af71, at 0xf08e)

    Bug List:
        Palace 6 fallthrough bug
        Ceiling stalfos 
        Shorten remaining wizards and such

        funky grass encounter in east hyrule
        death mountain encounters

    Feature List:

        Text Changes
            More community hints
            Helpful hints
        Item shuffling
            More extreme item shuffling
        Move towns accross continents
        Shuffle palace entrance statues / columns
        boss shuffle
        Experience Slider
            random/gems only/low/normal/high
        Shorten Towns
        Random%
        More extreme enemy shuffle
            Slider? (easy, medium, crazy)
        Overworld Generation improvements
            Allow continent entraces to be swapped
            Continent sizes
        Remove few remaining extra rooms
        Swap rooms across dungeons
        Mirror Rooms
        Change pbag amounts
        Swap up/downstab
        Put in sprites
        Fix small item shuffle
        Add Seed #, hash to title/file select

    Spell swap notes:
        Shield exit: 0xC7BB, 0xC1; enter: 0xC7EC, 0x90 //change map 48 pointer to map 40 pointer
        Jump exit: 0xC7BF, 0xC5; enter: 0xC7F0, 0x94 //change map 49 pointer to map 41 pointer
        Life exit: 0xC7C3, 0xC9; enter 0xC7F4, 0x98 //change map 50 pointer to map 42 pointer
        Fairy exit: 0xC7C7, 0xCD; enter 0xC7F8, 0x9C //change map 51 pointer to map 43 pointer
        Fire exit: 0xC7Cb, 0xD1; enter 0xC7FC, 0xA0 //change map 52 pointer to map 44 pointer
        Reflect exit: 0xC7Cf, 0xD5; enter 0xC800, 0xA4 //change map 53 pointer to map 45 pointer
        Spell exit: 0xC7D3, 0x6A; enter 0xC795, 0xC796, 0x4D //new kasuto item?
        Thunder exit: 0xC7D7, 0xDD; enter 0xC808, 0xAC
        Downstab exit: 0xC7DB, 0xE1; enter 0xC80C, 0xB0
        Upstab exit: 0xC7DF, 0xE5; enter 0xC810, 0xB4
    */

    public class Hyrule
    {
        private Dictionary<Spells, Spells> _spellMap;
        private List<Location> _itemLocs;
        private List<Location> _pbagHearts;
        private int _magContainers;
        private int _heartContainers;
        private int _startHearts;
        private int _maxHearts;
        private int _numHContainers;
        private int _kasutoJars;
        public bool[] _itemGet;
        public bool[] _spellGet;
        public bool _hiddenPalace;
        public bool _hiddenKasuto;
        private List<int> _visitedEnemies;

        private WestHyrule _westHyrule;
        private EastHyrule _eastHyrule;
        private MazeIsland _mazeIsland;
        private DeathMountain _deathMountain;
        private List<World> _worlds;
        private List<Palace> _palaces;

        public Rom RomData { get; set; }

        public Random R { get; set; }

        public RandomizerProperties Props { get; set; }

        public Hyrule(RandomizerProperties p)
        {
            Props = p;

            R = new Random(Props.Seed);
            RomData = Props.InputFileStream != null ? new Rom(Props.InputFileStream) : new Rom(Props.FileName);

            _palaces = new List<Palace>();
            _itemGet = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            _spellGet = new bool[] { false, false, false, false, false, false, false, false, false, false };

            UpdateRomData();
        }

        public string GenerateRom()
        {
            var newFileName = GenerateFileName();

            RomData.Dump(newFileName);

            return newFileName;
        }

        public (Stream OutputStream, string FileName) GenerateRomStream()
        {
            string newFileName = GenerateFileName();

            return (RomData.DumpStream(), newFileName);
        }

        private string GenerateFileName() => Props.FileName.Substring(0, Props.FileName.LastIndexOf("\\") + 1) + "Z2_" +
        Props.Seed + "_" + Props.Flags + ".nes";

        private void UpdateRomData()
        {
            //Hacky fix for palace connections
            RomData.Put(0x1074A, 0xFC);
            RomData.Put(0x1477D, 0xFC);

            //Hacky fix for new kasuto

            //NOTES:
            //Set 1DF77 to map number:
            //2D - fire town
            //2F - reflect town
            //31 - hidden town
            //33 - thunder town
            //Set 1DF79 to YPos of town

            RomData.Put(0x8660, 0x51);
            RomData.Put(0x924D, 0x00);

            //Hack fix for palace 6
            RomData.Put(0x8664, 0xE6);
            RomData.Put(0x935E, 0x02);

            if (Props.RandomizeNewKasutoJarRequirements)
            {
                _kasutoJars = R.Next(5, 8);
                RomData.Put(0xEEC9, (byte)(0xD0 + _kasutoJars));
                RomData.Put(0x1E7E8, (byte)_kasutoJars);
            }
            else
            {
                _kasutoJars = 7;
            }

            //text testing
            //Allows casting magic without requeueing a spell
            if (Props.FastSpellCasting)
            {
                RomData.Put(0xE15, 0xEA);
                RomData.Put(0xE16, 0xEA);
                RomData.Put(0xE17, 0xEA);
            }

            if (Props.DisableMusic)
            {
                for (int i = 0; i < 4; i++)
                {
                    RomData.Put(0x1a010 + i, 08);
                    RomData.Put(0x1a3da + i, 08);
                    RomData.Put(0x1a63f + i, 08);
                }

                RomData.Put(0x1a946, 08);
                RomData.Put(0x1a947, 08);
                RomData.Put(0x1a94c, 08);

                RomData.Put(0x1a02f, 0);
                RomData.Put(0x1a030, 0x44);
                RomData.Put(0x1a031, 0xA3);
                RomData.Put(0x1a032, 0);
                RomData.Put(0x1a033, 0);
                RomData.Put(0x1a034, 0);

                RomData.Put(0x1a3f4, 0);
                RomData.Put(0x1a3f5, 0x44);
                RomData.Put(0x1a3f6, 0xA3);
                RomData.Put(0x1a3f7, 0);
                RomData.Put(0x1a3f8, 0);
                RomData.Put(0x1a3f9, 0);

                RomData.Put(0x1a66e, 0);
                RomData.Put(0x1a66f, 0x44);
                RomData.Put(0x1a670, 0xA3);
                RomData.Put(0x1a671, 0);
                RomData.Put(0x1a672, 0);
                RomData.Put(0x1a673, 0);

                RomData.Put(0x1a970, 0);
                RomData.Put(0x1a971, 0x44);
                RomData.Put(0x1a972, 0xA3);
                RomData.Put(0x1a973, 0);
                RomData.Put(0x1a974, 0);
                RomData.Put(0x1a975, 0);
            }

            if (Props.HiddenPalace.Equals("Random"))
            {
                _hiddenPalace = R.NextDouble() > .5;
            }
            else
            {
                _hiddenPalace = Props.HiddenPalace.Equals("On");
            }

            if (Props.HiddenKasuto.Equals("Random"))
            {
                _hiddenKasuto = R.NextDouble() > .5;
            }
            else
            {
                _hiddenKasuto = Props.HiddenKasuto.Equals("On");
            }

            List<int> small = new List<int>();
            List<int> large = new List<int>();

            if (Props.RandomizeDrops)
            {
                while (small.Count() == 0)
                {
                    for (int i = 0; i < Pointers.Drops.Length; i++)
                    {
                        if (R.NextDouble() > .5)
                        {
                            small.Add(Pointers.Drops[i]);
                        }
                    }
                }

                while (large.Count == 0)
                {
                    for (int i = 0; i < Pointers.Drops.Length; i++)
                    {
                        if (R.NextDouble() > .5)
                        {
                            large.Add(Pointers.Drops[i]);
                        }
                    }
                }
            }

            if (Props.ManuallySelectDrops)
            {
                if (Props.SmallEnemyBlueJar)
                {
                    small.Add(0x90);
                }
                if (Props.SmallEnemyRedJar)
                {
                    small.Add(0x91);
                }
                if (Props.SmallEnemyFiftyPbag)
                {
                    small.Add(0x8a);
                }
                if (Props.SmallEnemyOneHundredPbag)
                {
                    small.Add(0x8b);
                }
                if (Props.SmallEnemyTwoHundredPbag)
                {
                    small.Add(0x8c);
                }
                if (Props.SmallEnemyFiveHundredPbag)
                {
                    small.Add(0x8d);
                }
                if (Props.SmallEnemyOneUp)
                {
                    small.Add(0x92);
                }
                if (Props.SmallEnemyKey)
                {
                    small.Add(0x88);
                }
                if (Props.LargeEnemyBlueJar)
                {
                    large.Add(0x90);
                }
                if (Props.LargeEnemyRedJar)
                {
                    large.Add(0x91);
                }
                if (Props.LargeEnemyFiftyPbag)
                {
                    large.Add(0x8a);
                }
                if (Props.LargeEnemyOneHundredPbag)
                {
                    large.Add(0x8b);
                }
                if (Props.LargeEnemyTwoHundredPbag)
                {
                    large.Add(0x8c);
                }
                if (Props.LargeEnemyFiveHundredPbag)
                {
                    large.Add(0x8d);
                }
                if (Props.LargeEnemyOneUp)
                {
                    large.Add(0x92);
                }
                if (Props.LargeEnemyKey)
                {
                    large.Add(0x88);
                }
            }

            if (Props.RandomizeDrops || Props.ManuallySelectDrops)
            {
                for (int i = 0; i < small.Count(); i++)
                {
                    int swap = R.Next(small.Count());
                    int temp = small[i];
                    small[i] = small[swap];
                    small[swap] = temp;
                }

                for (int i = 0; i < large.Count(); i++)
                {
                    int swap = R.Next(large.Count());
                    int temp = large[i];
                    large[i] = large[swap];
                    large[swap] = temp;
                }
                for (int i = 0; i < 8; i++)
                {
                    if (i < small.Count())
                    {
                        RomData.Put(0x1E880 + i, (byte)small[i]);
                    }
                    else
                    {
                        RomData.Put(0x1E880 + i, (byte)small[R.Next(small.Count())]);
                    }
                    if (i < large.Count())
                    {
                        RomData.Put(0x1E888 + i, (byte)large[i]);
                    }
                    else
                    {
                        RomData.Put(0x1E888 + i, (byte)large[R.Next(large.Count())]);
                    }
                }
            }

            //Fix for extra battle scene
            RomData.Put(0x8645, 0x00);

            //Disable hold over head animation
            RomData.Put(0x1E54C, 0);

            //Make text go fast
            RomData.Put(0xF75E, 0x00);
            RomData.Put(0xF625, 0x00);
            RomData.Put(0xF667, 0x00);

            //Fix for softlock
            //1E19A: 20AAFE
            //1FEBA: EE2607A2008E4C0760

            RomData.Put(0x1E19A, 0x20);
            RomData.Put(0x1E19B, 0xAA);
            RomData.Put(0x1E19C, 0xFE);

            RomData.Put(0x1FEBA, 0xEE);
            RomData.Put(0x1FEBB, 0x26);
            RomData.Put(0x1FEBC, 0x07);
            RomData.Put(0x1FEBD, 0xA2);
            RomData.Put(0x1FEBE, 0x00);
            RomData.Put(0x1FEBF, 0x8E);
            RomData.Put(0x1FEC0, 0x4C);
            RomData.Put(0x1FEC1, 0x07);
            RomData.Put(0x1FEC2, 0x60);

            /*
            Up + A:
        1cbba(cbaa): insert jump to d39a (1d3aa) (209ad3)

        1d3aa(d39a): store 707(8D0707) compare to 3(c903) less than 2 jump(3012) Load FB1 (ADb10f)compare with zero(c901) branch if zero(f00B) Load 561(AD6105) store accumulator into side memory(8Db00f) load accumulator with 1(a901) store to fb1(8db10f) return (60)

        d3bc(1d3cc): Load accumulator with fbo (adb00f)store to 561(8d6105) load 707(AD0707) return (60)

        feb3(1fec3): Store y into 707(8c0707) load 0(a900) stor into fb1(8db10f) return (60)

        CAD0(1CAE0): (20bcd3) c902 10

        CAE3(1CAF3): NOP NOP NOP(EAEAEA)

        CF92: (1CFA2): Jump to feb3(20b3fe)

            */
            if (Props.RestartAtPalacesIfGameOver)
            {
                RomData.Put(0x1cbba, 0x20);
                RomData.Put(0x1cbbb, 0x9a);
                RomData.Put(0x1cbbc, 0xd3);

                RomData.Put(0x1d3aa, 0x8d);
                RomData.Put(0x1d3ab, 0x07);
                RomData.Put(0x1d3ac, 0x07);
                RomData.Put(0x1d3ad, 0xad);
                RomData.Put(0x1d3ae, 0x07);
                RomData.Put(0x1d3af, 0x07);
                RomData.Put(0x1d3b0, 0xc9);
                RomData.Put(0x1d3b1, 0x03);
                RomData.Put(0x1d3b2, 0x30);
                RomData.Put(0x1d3b3, 0x12);
                RomData.Put(0x1d3b4, 0xad);
                RomData.Put(0x1d3b5, 0xb0);
                RomData.Put(0x1d3b6, 0x0f);
                RomData.Put(0x1d3b7, 0xc9);
                RomData.Put(0x1d3b8, 0x01);
                RomData.Put(0x1d3b9, 0xf0);
                RomData.Put(0x1d3ba, 0x0b);
                RomData.Put(0x1d3bb, 0xad);
                RomData.Put(0x1d3bc, 0x61);
                RomData.Put(0x1d3bd, 0x05);
                RomData.Put(0x1d3be, 0x8d);
                RomData.Put(0x1d3bf, 0xb1);
                RomData.Put(0x1d3c0, 0x0f);
                RomData.Put(0x1d3c1, 0xa9);
                RomData.Put(0x1d3c2, 0x01);
                RomData.Put(0x1d3c3, 0x8D);
                RomData.Put(0x1d3c4, 0xB0);
                RomData.Put(0x1d3c5, 0x0F);
                RomData.Put(0x1d3c6, 0xad);
                RomData.Put(0x1d3c7, 0x07);
                RomData.Put(0x1d3c8, 0x07);
                RomData.Put(0x1d3c9, 0x29);
                RomData.Put(0x1d3ca, 0x07);
                RomData.Put(0x1d3cb, 0x60);
                RomData.Put(0x1d3cc, 0xad);
                RomData.Put(0x1d3cd, 0xb1);
                RomData.Put(0x1d3ce, 0x0f);
                RomData.Put(0x1d3cf, 0x8d);
                RomData.Put(0x1d3d0, 0x61);
                RomData.Put(0x1d3d1, 0x05);
                RomData.Put(0x1d3d2, 0x20);
                RomData.Put(0x1d3d3, 0x57);
                RomData.Put(0x1d3d4, 0xa0);
                RomData.Put(0x1d3d5, 0xad);
                RomData.Put(0x1d3d6, 0x07);
                RomData.Put(0x1d3d7, 0x07);
                RomData.Put(0x1d3d8, 0x60);

                //feb3(1fec3): Store y into 707(8c0707) load 0(a900) stor into fb1(8db10f) return (60)
                RomData.Put(0x1fec3, 0x8c);
                RomData.Put(0x1fec4, 0x07);
                RomData.Put(0x1fec5, 0x07);
                RomData.Put(0x1fec6, 0xa9);
                RomData.Put(0x1fec7, 0x00);
                RomData.Put(0x1fec8, 0x8d);
                RomData.Put(0x1fec9, 0xb0);
                RomData.Put(0x1feca, 0x0f);
                RomData.Put(0x1fecb, 0x60);

                //CAD0(1CAE0): (20b7d3) c902 10
                RomData.Put(0x1cae0, 0x20);
                RomData.Put(0x1cae1, 0xbc);
                RomData.Put(0x1cae2, 0xd3);
                RomData.Put(0x1cae3, 0xc9);
                RomData.Put(0x1cae4, 0x03);
                RomData.Put(0x1cae5, 0x10);

                //CAE3(1CAF3): NOP NOP NOP(EAEAEA)
                RomData.Put(0x1caf3, 0xea);
                RomData.Put(0x1caf4, 0xea);
                RomData.Put(0x1caf5, 0xea);

                //CF92: (1CFA2): Jump to feb3(20b3fe)
                RomData.Put(0x1cfa2, 0x20);
                RomData.Put(0x1cfa3, 0xb3);
                RomData.Put(0x1cfa4, 0xfe);
            }

            if (Props.PermanentBeamSword)
            {
                RomData.Put(0x186c, 0xEA);
                RomData.Put(0x186d, 0xEA);
            }

            if (Props.StandardizeDrops)
            {
                RomData.Put(0x1e8bd, 0x20);
                RomData.Put(0x1e8be, 0x4c);
                RomData.Put(0x1e8bf, 0xff);

                RomData.Put(0x1ff5c, 0xc0);
                RomData.Put(0x1ff5d, 0x02);
                RomData.Put(0x1ff5e, 0xd0);
                RomData.Put(0x1ff5f, 0x07);
                RomData.Put(0x1ff60, 0xad);
                RomData.Put(0x1ff61, 0xfe);
                RomData.Put(0x1ff62, 0x06);
                RomData.Put(0x1ff63, 0xee);
                RomData.Put(0x1ff64, 0xfe);
                RomData.Put(0x1ff65, 0x06);
                RomData.Put(0x1ff66, 0x60);
                RomData.Put(0x1ff67, 0xad);
                RomData.Put(0x1ff68, 0xff);
                RomData.Put(0x1ff69, 0x06);
                RomData.Put(0x1ff6a, 0xee);
                RomData.Put(0x1ff6b, 0xff);
                RomData.Put(0x1ff6c, 0x06);
                RomData.Put(0x1ff6d, 0x60);
            }

            ShortenWizards();
            _magContainers = 4;
            _visitedEnemies = new List<int>();

            RandomizeStartingValues();
            RandomizeEnemies();
            LoadPalaces();
            ProcessOverworld();
            DumpText();

            UpdateRom();
        }



        /*
            Text Notes:
            
            Community Text Changes
            ----------------------
            Shield Spell    15  43
            Cannot Help     16  35
            Jump Spell      24  34
            Life Spell      35  37
            You know..?     37  42
            Fairy           46  37
            Downstab        47  38
            Bagu            48  44
            Fire            70  43
            You know        71  34
            Reflect         81  37
            Upstab          82  32
            Spell           93  25
            Thunder         96  36 
        */
        public static byte ReverseByte(byte b)
        {
            return (byte)(((b * 0x80200802ul) & 0x0884422110ul) * 0x0101010101ul >> 32);
        }

        private void DumpText()
        {
            int totalBytes = 0;
            int totalText = 0;
            List<List<char>> texts = new List<List<char>>();
            for (int i = 0xEFCE; i <= 0xF090; i += 2)
            {
                List<char> t = new List<char>();
                int addr = RomData.GetByte(i);
                addr += (RomData.GetByte(i + 1) << 8);
                addr += 0x4010;
                int c = RomData.GetByte(addr);
                int bytes = 0;
                while (c != 0xFF)
                {
                    if (c >= 0xD0 && c <= 0xD9)
                        _ = (char)(c - (0xd0 - '0'));
                    else if (c >= 0xDA && c <= 0xF3)
                        _ = (char)(c - (0xda - 'A'));
                    else if (c == 0xcf)
                        _ = '.';
                    else if (c == 0xce)
                        _ = '/';
                    else if (c == 0x9c)
                        _ = ',';
                    else if (c == 0x36)
                        _ = '!';
                    else if (c == 0x34)
                        _ = '?';
                    else if (c == 0x32)
                        _ = '*';
                    else if (c == 0xf4)
                        _ = ' ';
                    else if (c == 0xfd || c == 0xfe)
                        _ = '\n';

                    addr++;
                    t.Add((char)c);
                    c = RomData.GetByte(addr);
                    bytes++;
                }

                bytes++;
                t.Add((char)0xFF);
                texts.Add(t);
                totalBytes += bytes;
                totalText++;
            }

            if (Props.CommunityHints)
            {
                var wizardTexts = Text.WizardText;
                do
                {
                    for (int i = 0; i < 8; i++)
                    {
                        List<int> used = new List<int>();
                        int thisone = R.Next(wizardTexts.Count());
                        while (used.Contains(thisone))
                        {
                            thisone = R.Next(wizardTexts.Count());
                        }
                        List<char> c2 = ToGameText(wizardTexts[thisone]).ToList();
                        c2.Add((char)0xFF);
                        texts[IndexValues.WizardIndex[i]] = c2;
                        used.Add(thisone);
                    }

                    List<char> c3 = ToGameText(Text.BaguText[R.Next(Text.BaguText.Length)]).ToList();
                    c3.Add((char)0xFF);
                    texts[48] = c3;

                    c3 = ToGameText(Text.BridgeText[R.Next(Text.BridgeText.Length)]).ToList();
                    c3.Add((char)0xFF);
                    texts[37] = c3;

                    c3 = ToGameText(Text.DownstabText[R.Next(Text.DownstabText.Length)]).ToList();
                    c3.Add((char)0xFF);
                    texts[47] = c3;

                    c3 = ToGameText(Text.UpstabText[R.Next(Text.UpstabText.Length)]).ToList();
                    c3.Add((char)0xFF);
                    texts[82] = c3;
                } while (TextLength(texts) > 3134);
            }
            LoadItemLocs();


            if ((Props.HintType.Equals("Spell Item") || Props.HintType.Equals("Spell + Helpful")))
            {
                int i = 0;
                while (i < _itemLocs.Count() && _itemLocs[i]._item != Items.Trophy)
                {
                    i++;
                }
                if (i < _itemLocs.Count())
                {
                    texts[IndexValues.TrophyIndex] = CreateHint(_itemLocs[i], Items.Trophy);
                }

                i = 0;
                while (i < _itemLocs.Count() && _itemLocs[i]._item != Items.Medicine)
                {
                    i++;
                }
                if (i < _itemLocs.Count())
                {
                    texts[IndexValues.MedIndex] = CreateHint(_itemLocs[i], Items.Medicine);
                }

                i = 0;
                while (i < _itemLocs.Count() && _itemLocs[i]._item != Items.Kid)
                {
                    i++;
                }
                if (i < _itemLocs.Count())
                {
                    texts[IndexValues.KidIndex] = CreateHint(_itemLocs[i], Items.Kid);
                }
            }

            List<Items> it = new List<Items>();
            List<Items> placedItems = new List<Items>();
            List<int> placedIndex = new List<int>();

            for (int i = 0; i < _itemLocs.Count(); i++)
            {
                it.Add(_itemLocs[i]._item);
            }

            if (Props.HintType.Equals("Spell + Helpful"))
            {
                it.Remove(Items.Trophy);
                it.Remove(Items.Kid);
                it.Remove(Items.Medicine);
            }

            if (Props.HintType.Equals("Helpful") || Props.HintType.Equals("Spell + Helpful"))
            {
                bool placedSmall = false;
                List<Items> smallItems = new List<Items> { Items.Bluejar, Items.Fivehundobag, Items.Key, Items.Hundobag, Items.Magiccontainer, Items.Heartcontainer, Items.Oneup, Items.Redjar, Items.Smallbag, Items.Twohundobag };
                List<int> placedTowns = new List<int>();

                for (int i = 0; i < 4; i++)
                {
                    Items doThis = it[R.Next(it.Count())];
                    while ((placedSmall && smallItems.Contains(doThis)) || placedItems.Contains(doThis))
                    {
                        doThis = it[R.Next(it.Count())];
                    }
                    int j = 0;
                    while (_itemLocs[j]._item != doThis)
                    {
                        j++;
                    }
                    List<char> hint = CreateHint(_itemLocs[j], doThis);
                    int town = R.Next(9);
                    while (placedTowns.Contains(town))
                    {
                        town = R.Next(9);
                    }

                    if (town == 0)
                    {
                        int index = Rauru.Hints[R.Next(Rauru.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 1)
                    {
                        int index = Ruto.Hints[R.Next(Ruto.Hints.Count())];
                        if (index == 25 || index == 26)
                        {
                            texts[25] = hint;
                            texts[26] = hint;
                            placedIndex.Add(25);
                            placedIndex.Add(26);
                        }
                        else
                        {
                            texts[index] = hint;
                            placedIndex.Add(index);
                        }

                    }
                    else if (town == 2)
                    {
                        int index = Mido.Hints[R.Next(Mido.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 3)
                    {
                        int index = Saria.Hints[R.Next(Saria.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 4)
                    {
                        int index = Nabooru.Hints[R.Next(Nabooru.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 5)
                    {
                        int index = Darunia.Hints[R.Next(Darunia.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 6)
                    {
                        int index = NewKasuto.Hints[R.Next(NewKasuto.Hints.Count())];
                        texts[index] = hint;
                        placedIndex.Add(index);
                    }
                    else if (town == 7)
                    {
                        texts[OldKasuto.Hint] = hint;
                        placedIndex.Add(OldKasuto.Hint);
                    }
                    else if (town == 8)
                    {
                        texts[KingsTomb.Hint] = hint;
                        placedIndex.Add(KingsTomb.Hint);
                    }

                    placedTowns.Add(town);
                    placedItems.Add(doThis);
                    if (smallItems.Contains(doThis))
                    {
                        placedSmall = true;
                    }
                }
            }

            if (Props.HintType.Equals("Spell") || Props.HintType.Equals("Helpful") || Props.HintType.Equals("Spell + Helpful"))
            {
                List<int> stationary = new List<int>();
                stationary.AddRange(Rauru.Hints.ToList());
                stationary.AddRange(Ruto.Hints.ToList());
                stationary.AddRange(Saria.Hints.ToList());
                stationary.AddRange(Mido.Hints.ToList());
                stationary.AddRange(Nabooru.Hints.ToList());
                stationary.AddRange(Darunia.Hints.ToList());
                stationary.AddRange(NewKasuto.Hints.ToList());
                stationary.Add(KingsTomb.Hint);
                stationary.Add(OldKasuto.Hint);

                List<int> moving = new List<int>();
                moving.AddRange(Rauru.Moving.ToList());
                moving.AddRange(Ruto.Moving.ToList());
                moving.AddRange(Saria.Moving.ToList());
                moving.AddRange(Mido.Moving.ToList());
                moving.AddRange(Nabooru.Moving.ToList());
                moving.AddRange(Darunia.Moving.ToList());
                moving.AddRange(NewKasuto.Moving.ToList());

                List<char> knowNothing = ToGameText("i know$nothing").ToList();
                knowNothing.Add((char)0xFF);
                for (int i = 0; i < stationary.Count(); i++)
                {
                    if (!placedIndex.Contains(stationary[i]))
                    {
                        texts[stationary[i]] = knowNothing;
                    }
                }

                for (int i = 0; i < moving.Count(); i++)
                {
                    texts[moving[i]] = knowNothing;
                }


            }

            if (Props.CommunityHints || Props.HintType.Equals("Spell") || Props.HintType.Equals("Helpful") || Props.HintType.Equals("Spell + Helpful"))
            {
                TextToRom(texts);
            }
        }

        private List<char> CreateHint(Location location, Items item)
        {
            string hint = "";
            if (location.PalNum == 1)
            {
                hint += "horsehead$neighs$with the$";
            }
            else if (location.PalNum == 2)
            {
                hint += "helmethead$guards the$";
            }
            else if (location.PalNum == 3)
            {
                hint += "rebonack$rides$with the$";
            }
            else if (location.PalNum == 4)
            {
                hint += "carock$disappears$with the$";
            }
            else if (location.PalNum == 5)
            {
                hint += "gooma sits$on the$";
            }
            else if (location.PalNum == 6)
            {
                hint += "barba$slithers$with the$";
            }
            else if (_eastHyrule.AllLocations.Contains(location))
            {
                hint += "go east to$find the$";
            }
            else if (_westHyrule.AllLocations.Contains(location))
            {
                hint += "go west to$find the$";
            }
            else if (_deathMountain.AllLocations.Contains(location))
            {
                hint += "death$mountain$holds the$";
            }
            else if (location == _eastHyrule._newKasuto || location == _eastHyrule._newKasuto2)
            {
                hint += "go east to$find the$";
            }
            else
            {
                hint += "in a maze$lies the$";
            }

            if (item == Items.Bluejar)
            {
                hint += "blue jar";
            }
            else if (item == Items.Boots)
            {
                hint += "boots";
            }
            else if (item == Items.Candle)
            {
                hint += "candle";
            }
            else if (item == Items.Cross)
            {
                hint += "cross";
            }
            else if (item == Items.Fivehundobag)
            {
                hint += "500 bag";
            }
            else if (item == Items.Glove)
            {
                hint += "glove";
            }
            else if (item == Items.Hammer)
            {
                hint += "hammer";
            }
            else if (item == Items.Heartcontainer)
            {
                hint += "heart";
            }
            else if (item == Items.Horn)
            {
                hint += "flute";
            }
            else if (item == Items.Hundobag)
            {
                hint += "100 bag";
            }
            else if (item == Items.Key)
            {
                hint += "small key";
            }
            else if (item == Items.Kid)
            {
                hint += "child";
            }
            else if (item == Items.Magiccontainer)
            {
                hint += "magic jar";
            }
            else if (item == Items.Magickey)
            {
                hint += "magic key";
            }
            else if (item == Items.Medicine)
            {
                hint += "medicine";
            }
            else if (item == Items.Oneup)
            {
                hint += "link doll";
            }
            else if (item == Items.Raft)
            {
                hint += "raft";
            }
            else if (item == Items.Redjar)
            {
                hint += "red jar";
            }
            else if (item == Items.Smallbag)
            {
                hint += "50 bag";
            }
            else if (item == Items.Trophy)
            {
                hint += "trophy";
            }
            else if (item == Items.Twohundobag)
            {
                hint += "200 bag";
            }

            List<char> result = ToGameText(hint).ToList();
            result.Add((char)0xFF);
            return result;
        }

        private void TextToRom(List<List<char>> texts)
        {
            int textptr = 0xE390;
            int ptr = 0xE390 - 0x4010;
            int ptrptr = 0xEFCE;

            for (int i = 0; i < texts.Count; i++)
            {
                int high = (ptr & 0xff00) >> 8;
                int low = (ptr & 0xff);
                RomData.Put(ptrptr, (byte)low);
                RomData.Put(ptrptr + 1, (byte)high);
                ptrptr += 2;
                for (int j = 0; j < texts[i].Count; j++)
                {
                    RomData.Put(textptr, (byte)texts[i][j]);
                    textptr++;
                    ptr++;
                }
            }
        }

        private int TextLength(List<List<char>> texts)
        {
            int sum = 0;
            for (int i = 0; i < texts.Count(); i++)
            {
                sum += texts[i].Count;
            }
            return sum;
        }

        private void ShuffleAttackEffectiveness(bool ohko)
        {
            if (!ohko)
            {
                int[] atk = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    atk[i] = RomData.GetByte(0x1E67D + i);
                }

                for (int i = 0; i < atk.Length; i++)
                {
                    int minAtk = (int)(atk[i] - atk[i] * .5);
                    int maxAtk = (int)(atk[i] + atk[i] * .5);
                    int next = atk[i];

                    if (Props.RandomAttackEffectiveness)
                    {
                        next = R.Next(minAtk, maxAtk);
                    }
                    else if (Props.HighAttackEffectiveness)
                    {
                        next = (int)(atk[i] + (atk[i] * .4));
                    }
                    else if (Props.LowAttackEffectiveness)
                    {
                        next = (int)(atk[i] * .5);
                    }

                    if (Props.RandomAttackEffectiveness)
                    {
                        if (i == 0)
                        {
                            atk[i] = Math.Max(next, 2);
                        }
                        else
                        {
                            if (next < atk[i - 1])
                            {
                                atk[i] = atk[i - 1];
                            }
                        }
                    }
                    else
                    {
                        atk[i] = next;
                    }
                }


                for (int i = 0; i < 8; i++)
                {
                    RomData.Put(0x1E67D + i, (byte)atk[i]);
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    RomData.Put(0x1E67D + i, 192);
                }
            }
        }

        private void ShuffleItems()
        {
            _ = new List<Items> { Items.Candle, Items.Glove, Items.Raft, Items.Boots, Items.Horn, Items.Cross, Items.Heartcontainer, Items.Heartcontainer, Items.Magiccontainer, Items.Medicine, Items.Trophy, Items.Heartcontainer, Items.Heartcontainer, Items.Magiccontainer, Items.Magickey, Items.Magiccontainer, Items.Hammer, Items.Kid, Items.Magiccontainer };
            _ = new List<Items> { Items.Bluejar, Items.Redjar, Items.Smallbag, Items.Hundobag, Items.Twohundobag, Items.Fivehundobag, Items.Oneup, Items.Key };
            _ = _mazeIsland._kid;
            _ = _westHyrule._medicineCave;
            _ = _westHyrule._trophyCave;
            _numHContainers = _maxHearts - _startHearts;
            for (int i = 0; i < _itemGet.Count(); i++)
            {
                _itemGet[i] = false;
            }
            foreach (Location l in _itemLocs)
            {
                l._itemGet = false;
            }
            _westHyrule._pbagCave._itemGet = false;
            _eastHyrule._pbagCave1._itemGet = false;
            _eastHyrule._pbagCave2._itemGet = false;
            if (Props.ShuffleStartingItems)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasItem = R.NextDouble() > .75;
                    RomData.Put(0x17B01 + i, hasItem ? (byte)1 : (byte)0);
                    _itemGet[i] = hasItem;
                }
            }
            else
            {
                RomData.Put(0x17B01, Props.StartWithCandle ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Candle] = Props.StartWithCandle;
                RomData.Put(0x17B02, Props.StartWithGlove ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Glove] = Props.StartWithGlove;
                RomData.Put(0x17B03, Props.StartWithRaft ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Raft] = Props.StartWithRaft;
                RomData.Put(0x17B04, Props.StartWithBoots ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Boots] = Props.StartWithBoots;
                RomData.Put(0x17B05, Props.StartWithFlute ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Horn] = Props.StartWithFlute;
                RomData.Put(0x17B06, Props.StartWithCross ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Cross] = Props.StartWithCross;
                RomData.Put(0x17B07, Props.StartWithHammer ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Hammer] = Props.StartWithHammer;
                RomData.Put(0x17B08, Props.StartWithKey ? (byte)1 : (byte)0);
                _itemGet[(int)Items.Magickey] = Props.StartWithKey;
            }

            List<Items> itemList = new List<Items> { Items.Candle, Items.Glove, Items.Raft, Items.Boots, Items.Horn, Items.Cross, Items.Heartcontainer, Items.Heartcontainer, Items.Magiccontainer, Items.Medicine, Items.Trophy, Items.Heartcontainer, Items.Heartcontainer, Items.Magiccontainer, Items.Magickey, Items.Magiccontainer, Items.Hammer, Items.Kid, Items.Magiccontainer };

            if (Props.IncludePbagCavesInItemShuffle)
            {
                _westHyrule._pbagCave._item = (Items)RomData.GetByte(0x4FE2);
                _eastHyrule._pbagCave1._item = (Items)RomData.GetByte(0x8ECC);
                _eastHyrule._pbagCave2._item = (Items)RomData.GetByte(0x8FB3);
                itemList.Add(_westHyrule._pbagCave._item);
                itemList.Add(_eastHyrule._pbagCave1._item);
                itemList.Add(_eastHyrule._pbagCave2._item);

            }
            _pbagHearts = new List<Location>();
            if (_numHContainers < 4)
            {
                int x = 4 - _numHContainers;
                while (x > 0)
                {
                    int remove = R.Next(itemList.Count);
                    if (itemList[remove] == Items.Heartcontainer)
                    {
                        itemList[remove] = Items.Fivehundobag;
                        x--;
                    }
                }
            }

            if (_numHContainers > 4)
            {
                if (Props.IncludePbagCavesInItemShuffle)
                {
                    int x = _numHContainers - 4;
                    while (x > 0)
                    {
                        itemList[22 - x] = Items.Heartcontainer;
                        x--;
                    }
                }
                else
                {
                    int x = _numHContainers - 4;
                    while (x > 0)
                    {
                        int y = R.Next(3);
                        if (y == 0 && !_pbagHearts.Contains(_westHyrule._pbagCave))
                        {
                            _pbagHearts.Add(_westHyrule._pbagCave);
                            _westHyrule._pbagCave._item = Items.Heartcontainer;
                            itemList.Add(Items.Heartcontainer);
                            _itemLocs.Add(_westHyrule._pbagCave);
                            x--;
                        }
                        if (y == 1 && !_pbagHearts.Contains(_eastHyrule._pbagCave1))
                        {
                            _pbagHearts.Add(_eastHyrule._pbagCave1);
                            _eastHyrule._pbagCave1._item = Items.Heartcontainer;
                            itemList.Add(Items.Heartcontainer);
                            _itemLocs.Add(_eastHyrule._pbagCave1);
                            x--;
                        }
                        if (y == 2 && !_pbagHearts.Contains(_eastHyrule._pbagCave2))
                        {
                            _pbagHearts.Add(_eastHyrule._pbagCave2);
                            _eastHyrule._pbagCave2._item = Items.Heartcontainer;
                            itemList.Add(Items.Heartcontainer);
                            _itemLocs.Add(_eastHyrule._pbagCave2);
                            x--;
                        }
                    }
                }
            }

            if (Props.RemoveSpellItems)
            {
                itemList[9] = Items.Fivehundobag;
                itemList[10] = Items.Fivehundobag;
                itemList[17] = Items.Fivehundobag;
                _itemGet[(int)Items.Trophy] = true;
                _itemGet[(int)Items.Medicine] = true;
                _itemGet[(int)Items.Kid] = true;

            }

            if (_spellGet[(int)_spellMap[Spells.Fairy]])
            {
                itemList[9] = Items.Fivehundobag;
                _itemGet[(int)Items.Medicine] = true;
            }

            if (_spellGet[(int)_spellMap[Spells.Jump]])
            {
                itemList[10] = Items.Fivehundobag;
                _itemGet[(int)Items.Trophy] = true;
            }

            if (_spellGet[(int)_spellMap[Spells.Reflect]])
            {
                itemList[17] = Items.Fivehundobag;
                _itemGet[(int)Items.Kid] = true;
            }

            if (_itemGet[0])
            {
                itemList[0] = Items.Fivehundobag;
            }

            if (_itemGet[1])
            {
                itemList[1] = Items.Fivehundobag;
            }

            if (_itemGet[2])
            {
                itemList[2] = Items.Fivehundobag;
            }

            if (_itemGet[3])
            {
                itemList[3] = Items.Fivehundobag;
            }

            if (_itemGet[4])
            {
                itemList[4] = Items.Fivehundobag;
            }

            if (_itemGet[5])
            {
                itemList[5] = Items.Fivehundobag;
            }

            if (_itemGet[7])
            {
                itemList[14] = Items.Fivehundobag;
            }

            if (_itemGet[6])
            {
                itemList[16] = Items.Fivehundobag;
            }


            if (Props.MixOverworldAndPalaceItems)
            {
                for (int i = 0; i < itemList.Count; i++)
                {

                    int s = R.Next(i, itemList.Count);
                    Items sl = itemList[s];
                    itemList[s] = itemList[i];
                    itemList[i] = sl;
                }
            }
            else
            {
                if (Props.ShufflePalaceItems)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int s = R.Next(i, 6);
                        Items sl = itemList[s];
                        itemList[s] = itemList[i];
                        itemList[i] = sl;
                    }
                }

                if (Props.ShuffleOverworldItems)
                {
                    for (int i = 6; i < itemList.Count; i++)
                    {
                        int s = R.Next(i, itemList.Count);
                        Items sl = itemList[s];
                        itemList[s] = itemList[i];
                        itemList[i] = sl;
                    }
                }
            }
            for (int i = 0; i < itemList.Count; i++)
            {
                _itemLocs[i]._item = itemList[i];
            }
            foreach (Location l in _itemLocs)
            {
                if (l._item == Items.Kid)
                {
                    Location kidLoc = l;
                }
                else if (l._item == Items.Trophy)
                {
                    Location trophyLoc = l;
                }
                else if (l._item == Items.Medicine)
                {
                    Location medicineLoc = l;
                }
            }

            for (int i = 0; i < 64; i++)
            {
                byte heartByte = RomData.GetByte(0x27810 + i);
                RomData.Put(0x29810 + i, heartByte);
                RomData.Put(0x2B810 + i, heartByte);
                RomData.Put(0x2D810 + i, heartByte);
                RomData.Put(0x33810 + i, heartByte);
                RomData.Put(0x35810 + i, heartByte);
                RomData.Put(0x37810 + i, heartByte);
                RomData.Put(0x39810 + i, heartByte);
            }
        }

        private bool EverythingReachable2()
        {
            int dm = 0;
            int mi = 0;
            int wh = 0;
            int eh = 0;
            int count = 1;
            int prevCount = 0;
            _magContainers = 4;
            _heartContainers = _startHearts;
            _ = _westHyrule.AllLocations.Count + _eastHyrule.AllLocations.Count + _deathMountain.AllLocations.Count + _mazeIsland.AllLocations.Count;
            while (prevCount != count)
            {
                prevCount = count;
                _westHyrule.UpdateVisit();
                if (CanGet(_westHyrule._hammerEnter))
                {
                    _deathMountain.SetStart();
                }

                if (CanGet(_westHyrule._hammerExit))
                {
                    _deathMountain.SetStart2();
                }

                if (CanGet(_westHyrule._raftSpot) && _itemGet[(int)Items.Raft])
                {
                    _eastHyrule.SetStart();
                }

                _deathMountain.UpdateVisit();
                _eastHyrule.UpdateVisit();
                if (CanGet(_eastHyrule._palace4))
                {
                    _mazeIsland.SetStart();
                }
                _mazeIsland.UpdateVisit();

                UpdateItems();
                UpdateSpells();

                count = 0;
                dm = 0;
                mi = 0;
                wh = 0;
                eh = 0;

                foreach (Location l in _westHyrule.AllLocations)
                {
                    if (l.Reachable)
                    {
                        count++;
                        wh++;
                    }
                }

                foreach (Location l in _eastHyrule.AllLocations)
                {
                    if (l.Reachable)
                    {
                        count++;
                        eh++;
                    }
                }

                foreach (Location l in _deathMountain.AllLocations)
                {
                    if (l.Reachable)
                    {
                        count++;
                        dm++;
                    }
                }

                foreach (Location l in _mazeIsland.AllLocations)
                {
                    if (l.Reachable)
                    {
                        count++;
                        mi++;
                    }
                }
            }

            Console.WriteLine("Reached: " + count);
            Console.WriteLine("wh: " + wh);
            Console.WriteLine("eh: " + eh);
            Console.WriteLine("dm: " + dm);
            Console.WriteLine("mi: " + mi);

            for (int i = 0; i < 8; i++)
            {
                if (_itemGet[i] == false)
                {
                    return false;
                }
            }

            for (int i = 19; i < 22; i++)
            {
                if (_itemGet[i] == false)
                {
                    return false;
                }
            }
            if (_magContainers != 8)
            {
                return false;
            }
            if (_heartContainers != _maxHearts)
            {
                return false;
            }
            for (int i = 0; i < _spellGet.Count(); i++)
            {
                if (!_spellGet[i])
                {
                    return false;
                }
            }

            return (CanGet(_westHyrule._palaceOne) && CanGet(_westHyrule._palaceTwo) && CanGet(_westHyrule._palaceThree) && CanGet(_mazeIsland._palace4) && CanGet(_eastHyrule._palace5) && CanGet(_eastHyrule._palace6) && CanGet(_eastHyrule._gp) && CanGet(_itemLocs));
        }

        private bool CanGet(List<Location> l)
        {
            foreach (Location ls in l)
            {
                if (ls.Reachable == false)
                {
                    return false;
                }
            }
            return true;
        }
        private bool CanGet(Location l)
        {

            return l.Reachable;
        }

        private void ShortenWizards()
        {
            /*
            Spell swap notes:
            Shield exit: 0xC7BB, 0xC1; enter: 0xC7EC, 0x90 //change map 48 pointer to map 40 pointer
        Jump exit: 0xC7BF, 0xC5; enter: 0xC7F0, 0x94 //change map 49 pointer to map 41 pointer
        Life exit: 0xC7C3, 0xC9; enter 0xC7F4, 0x98 //change map 50 pointer to map 42 pointer
        Fairy exit: 0xC7C7, 0xCD; enter 0xC7F8, 0x9C //change map 51 pointer to map 43 pointer
        Fire exit: 0xC7Cb, 0xD1; enter 0xC7FC, 0xA0 //change map 52 pointer to map 44 pointer
        Reflect exit: 0xC7Cf, 0xD5; enter 0xC800, 0xA4 //change map 53 pointer to map 45 pointer
        Spell exit: 0xC7D3, 0x6A; enter 0xC795, 0xC796, 0x4D //new kasuto item?
        Thunder exit: 0xC7D7, 0xDD; enter 0xC808, 0xAC
        Downstab exit: 0xC7DB, 0xE1; enter 0xC80C, 0xB0
        Upstab exit: 0xC7DF, 0xE5; enter 0xC810, 0xB4
    */
            for (int i = 0; i < 16; i += 2)
            {
                RomData.Put(0xC611 + i, 0x75);
                RomData.Put(0xC611 + i + 1, 0x70);
                RomData.Put(0xC593 + i, 0x48);
                RomData.Put(0xC593 + i + 1, 0x9B);
            }
            RomData.Put(0xC7BB, 0x07);
            RomData.Put(0xC7BF, 0x13);
            RomData.Put(0xC7C3, 0x21);
            RomData.Put(0xC7C7, 0x27);
            RomData.Put(0xC7CB, 0x37);
            RomData.Put(0xC7CF, 0x3F);
            RomData.Put(0xC7D7, 0x5E);
        }

        private void UpdateSpells()
        {
            Location[] t = new Location[11];
            t[_westHyrule._shieldTown.TownNum] = _westHyrule._shieldTown;
            t[_westHyrule._jump.TownNum] = _westHyrule._jump;
            t[_westHyrule._lifeNorth.TownNum] = _westHyrule._lifeNorth;
            t[_westHyrule._lifeSouth.TownNum] = _westHyrule._lifeSouth;
            t[_westHyrule._fairy.TownNum] = _westHyrule._fairy;
            t[_eastHyrule._fireTown.TownNum] = _eastHyrule._fireTown;
            t[_eastHyrule._darunia.TownNum] = _eastHyrule._darunia;
            t[_eastHyrule._newKasuto.TownNum] = _eastHyrule._newKasuto;
            t[_eastHyrule._newKasuto2.TownNum] = _eastHyrule._newKasuto2;
            t[_eastHyrule._oldKasuto.TownNum] = _eastHyrule._oldKasuto;

            foreach (Spells s in _spellMap.Keys)
            {
                if (s == Spells.Fairy && (_itemGet[(int)Items.Medicine] || Props.RemoveSpellItems) && CanGet(t[5]) && (_magContainers >= 4 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Jump && (_itemGet[(int)Items.Trophy] || Props.RemoveSpellItems) && CanGet(t[2]) && (_magContainers >= 2 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Downstab && (_spellGet[(int)Spells.Jump] || _spellGet[(int)Spells.Fairy]) && CanGet(t[5]))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Upstab && (_spellGet[(int)Spells.Jump]) && CanGet(t[7]))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Life && (CanGet(t[3])) && (_magContainers >= 3 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Shield && CanGet(t[1]) && (_magContainers >= 1 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Reflect && CanGet(t[7]) && (_itemGet[(int)Items.Kid] || Props.RemoveSpellItems) && (_magContainers >= 6 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Fire && CanGet(t[6]) && (_magContainers >= 5 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Spell && CanGet(t[8]) && (_magContainers >= 7 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
                else if (s == Spells.Thunder && CanGet(t[10]) && (_magContainers >= 8 || Props.DisableMagicContainerRequirements))
                {
                    _spellGet[(int)_spellMap[s]] = true;
                }
            }
        }

        private void UpdateItems()
        {
            foreach (Location l in _itemLocs)
            {
                bool itemGotten = l._itemGet;
                if (l.PalNum > 0 && l.PalNum < 7)
                {
                    Palace p = _palaces[l.PalNum - 1];
                    l._itemGet = _itemGet[(int)l._item] = CanGet(l) && (_spellGet[(int)Spells.Fairy] || _itemGet[(int)Items.Magickey]) && (!p.NeedDstab || (p.NeedDstab && _spellGet[(int)Spells.Downstab])) && (!p.NeedFairy || (p.NeedFairy && _spellGet[(int)Spells.Fairy])) && (!p.NeedGlove || (p.NeedGlove && _itemGet[(int)Items.Glove])) && (!p.NeedJumpOrFairy || (p.NeedJumpOrFairy && (_spellGet[(int)Spells.Jump]) || _spellGet[(int)Spells.Fairy]) && (l.PalNum != 6 || (_hiddenPalace && _itemGet[(int)Items.Horn]) || (!_hiddenPalace)));
                }

                else if (l.TownNum == 8)
                {
                    l._itemGet = _itemGet[(int)l._item] = CanGet(l) && (_magContainers >= _kasutoJars) && (!l.NeedHammer || _itemGet[(int)Items.Hammer]);
                }
                else if (l.TownNum == 9)
                {
                    l._itemGet = _itemGet[(int)l._item] = (CanGet(l) && _spellGet[(int)Spells.Spell]) && (!l.NeedHammer || _itemGet[(int)Items.Hammer]);
                }
                else
                {
                    l._itemGet = _itemGet[(int)l._item] = CanGet(l);
                }
                if (itemGotten != l._itemGet && l._item == Items.Magiccontainer)
                {
                    _magContainers++;
                }
                if (itemGotten != l._itemGet && l._item == Items.Heartcontainer)
                {
                    _heartContainers++;
                }
            }
        }
        private void ShuffleLifeEffectiveness(bool isMag)
        {
            int numBanks = 7;
            int start = 0x1E2BF;
            if (isMag)
            {
                numBanks = 8;
                start = 0xD8B;
            }
            int[,] life = new int[numBanks, 8];
            for (int i = 0; i < numBanks; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int lifeVal = RomData.GetByte(start + (i * 8) + j);
                    int highPart = (lifeVal & 0xF0) >> 4;
                    int lowPart = lifeVal & 0x0F;
                    life[i, j] = highPart * 8 + lowPart / 2;
                }
            }

            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < numBanks; i++)
                {
                    int nextVal = life[i, j];
                    if ((Props.RandomLifeEffectiveness && !isMag) || (Props.RandomMagicEffectiveness && isMag))
                    {
                        int max = (int)(life[i, j] + life[i, j] * .5);
                        int min = (int)(life[i, j] - life[i, j] * .5);
                        if (j == 0)
                        {
                            nextVal = R.Next(min, Math.Min(max, 120));
                        }
                        else
                        {
                            nextVal = R.Next(min, Math.Min(max, 120));
                            if (nextVal > life[i, j - 1])
                            {
                                nextVal = life[i, j - 1];
                            }
                        }
                    }
                    else if (Props.MagicEffectiveness && isMag)
                    {
                        nextVal = (int)(life[i, j] + (life[i, j] * .5));
                    }
                    else if (Props.HighLifeEffectiveness && !isMag || Props.LowMagicEffectiveness && isMag)
                    {
                        nextVal = (int)(life[i, j] * .5);
                    }

                    if (isMag && nextVal > 120)
                    {
                        nextVal = 120;
                    }
                    life[i, j] = nextVal;
                }
            }

            for (int i = 0; i < numBanks; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int highPart = (life[i, j] / 8) << 4;
                    int lowPart = (life[i, j] % 8);
                    RomData.Put(start + (i * 8) + j, (byte)(highPart + (lowPart * 2)));
                }
            }
        }

        private void RandomizeEnemies()
        {
            if (Props.ShuffleEnemyHp)
            {
                ShuffleHp(0x5434, 0x5453);
                ShuffleHp(0x9434, 0x944E);
                ShuffleHp(0x11435, 0x11435);
                ShuffleHp(0x11437, 0x11454);
                ShuffleHp(0x13C86, 0x13C87);
                ShuffleHp(0x15534, 0x15438);
                ShuffleHp(0x15540, 0x15443);
                ShuffleHp(0x15545, 0x1544B);
                ShuffleHp(0x1544E, 0x1544E);
                ShuffleHp(0x12935, 0x12935);
                ShuffleHp(0x12937, 0x12954);
            }

            if (Props.OhkoAttackEffectiveness)
            {
                ShuffleAttackEffectiveness(true);
                RomData.Put(0x005432, 193);
                RomData.Put(0x009432, 193);
                RomData.Put(0x11436, 193);
                RomData.Put(0x12936, 193);
                RomData.Put(0x15532, 193);
                RomData.Put(0x11437, 192);
                RomData.Put(0x1143F, 192);
                RomData.Put(0x12937, 192);
                RomData.Put(0x1293F, 192);
                RomData.Put(0x15445, 192);
                RomData.Put(0x15446, 192);
                RomData.Put(0x15448, 192);
                RomData.Put(0x15453, 193);
                RomData.Put(0x12951, 227);

            }
        }

        private void ShuffleHp(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                int newVal = 0;
                int val = RomData.GetByte(i);

                newVal = R.Next((int)(val * 0.5), (int)(val * 1.5));
                if (newVal > 255)
                {
                    newVal = 255;
                }

                RomData.Put(i, (byte)newVal);
            }
        }

        private void ProcessOverworld()
        {
            if (Props.ShuffleSmallItems)
            {
                ShuffleSmallItems(1, true);
                ShuffleSmallItems(1, false);
                ShuffleSmallItems(2, true);
                ShuffleSmallItems(2, false);
                ShuffleSmallItems(3, true);
            }

            bool f = false;
            do
            {
                _worlds = new List<World>();
                _westHyrule = new WestHyrule(this);
                do
                {
                    f = _westHyrule.Terraform();
                } while (!f);

                _deathMountain = new DeathMountain(this);
                do
                {
                    f = _deathMountain.Terraform();
                } while (!f);
                _eastHyrule = new EastHyrule(this);
                do
                {
                    f = _eastHyrule.Terraform();
                } while (!f);
                _mazeIsland = new MazeIsland(this);
                _mazeIsland.Terraform();
                LoadItemLocs();
                ShuffleSpells();
                ShuffleItems();
                ShufflePalaces();
                LoadItemLocs();
                ShuffleTowns();

                int x = 0;
                while (!EverythingReachable2() && x < 50)
                {

                    foreach (Location l in _westHyrule.AllLocations)
                    {
                        l.Reachable = false;
                    }

                    foreach (Location l in _eastHyrule.AllLocations)
                    {
                        l.Reachable = false;
                    }

                    foreach (Location l in _mazeIsland.AllLocations)
                    {
                        l.Reachable = false;
                    }

                    foreach (Location l in _deathMountain.AllLocations)
                    {
                        l.Reachable = false;
                    }
                    _eastHyrule._newKasuto2.Reachable = false;
                    _eastHyrule._palace4.Reachable = false;
                    _westHyrule.Reset();
                    _eastHyrule.Reset();
                    _mazeIsland.Reset();
                    LoadItemLocs();
                    _deathMountain.Reset();
                    _westHyrule.SetStart();
                    ShuffleSpells();
                    ShuffleItems();
                    ShufflePalaces();
                    LoadItemLocs();

                    x++;
                }
                int west = 0;
                if (x != 50)
                {
                    break;
                }
                foreach (Location l in _westHyrule.AllLocations)
                {
                    if (l.Reachable)
                    {
                        west++;
                    }
                }

                int east = 0;
                foreach (Location l in _eastHyrule.AllLocations)
                {
                    if (l.Reachable)
                    {
                        east++;
                    }
                }

                int maze = 0;
                foreach (Location l in _mazeIsland.AllLocations)
                {
                    if (l.Reachable)
                    {
                        maze++;
                    }
                }

                int dm = 0;
                foreach (Location l in _deathMountain.AllLocations)
                {
                    if (l.Reachable)
                    {
                        dm++;
                    }
                }

                Console.WriteLine("wr: " + west);
                Console.WriteLine("er: " + east);
                Console.WriteLine("dm: " + dm);
                Console.WriteLine("maze: " + maze);
            } while (!EverythingReachable2());

            _worlds.Add(_westHyrule);
            _worlds.Add(_eastHyrule);
            _worlds.Add(_deathMountain);
            _worlds.Add(_mazeIsland);

            if (Props.ShuffleOverworldEnemies)
            {
                foreach (World w in _worlds)
                {
                    w.ShuffleE();
                }
            }
        }

        private void ShuffleTowns()
        {
            _westHyrule._shieldTown.TownNum = 1;
            _westHyrule._jump.TownNum = 2;
            _westHyrule._lifeNorth.TownNum = 3;
            _westHyrule._lifeSouth.TownNum = 4;
            _westHyrule._fairy.TownNum = 5;
            _eastHyrule._fireTown.TownNum = 6;
            _eastHyrule._darunia.TownNum = 7;
            _eastHyrule._newKasuto.TownNum = 8;
            _eastHyrule._newKasuto2.TownNum = 9;
            _eastHyrule._oldKasuto.TownNum = 10;
        }

        private void ShufflePalaces()
        {

            if (Props.AllowPalacesToSwapContinents)
            {

                List<Location> pals = new List<Location> { _westHyrule._palaceOne, _westHyrule._palaceTwo, _westHyrule._palaceThree, _mazeIsland._palace4, _eastHyrule._palace5, _eastHyrule._palace6 };

                if (Props.IncludeGreatPalaceInShuffle)
                {
                    pals.Add(_eastHyrule._gp);
                }

                for (int i = 0; i < pals.Count; i++)
                {
                    int swapp = R.Next(i, pals.Count);
                    Swap(pals[i], pals[swapp]);
                }

                _westHyrule._palaceOne.World &= 0xFC;
                _westHyrule._palaceTwo.World &= 0xFC;
                _westHyrule._palaceThree.World &= 0xFC;

                _mazeIsland._palace4.World &= 0xFC;
                _mazeIsland._palace4.World |= 0x01;

                _eastHyrule._palace5.World &= 0xFC;
                _eastHyrule._palace5.World |= 0x02;

                _eastHyrule._palace6.World &= 0xFC;
                _eastHyrule._palace6.World |= 0x02;

                if (Props.IncludeGreatPalaceInShuffle)
                {
                    _eastHyrule._gp.World &= 0xFC;
                    _eastHyrule._gp.World |= 0x02;
                }

                /*
                subroutine start bf60(13f70)

                instruction: 20 60 bf

                subroutine:
                    load 22 into accumulator    A9 22
                    xor with $561               4D 61 05
                    return                      60


                Gooma / helmet head fix (CHECK THESE):
                    13c96 = d0--hitbox / exp / hp
                    13d88 = d0--sprite info
                    13ad6 = d0--behavior
                    11b2d = d0(don't need?)
                    */

                //write subroutine
                RomData.Put(0x13f70, 0xA9);
                RomData.Put(0x13f71, 0x22);
                RomData.Put(0x13f72, 0x4D);
                RomData.Put(0x13f73, 0x61);
                RomData.Put(0x13f74, 0x05);
                RomData.Put(0x13f75, 0x60);

                //jump to subroutine
                RomData.Put(0x13c93, 0x20);
                RomData.Put(0x13c94, 0x60);
                RomData.Put(0x13c95, 0xBF);

                RomData.Put(0x13d85, 0x20);
                RomData.Put(0x13d86, 0x60);
                RomData.Put(0x13d87, 0xBF);

                RomData.Put(0x13ad3, 0x20);
                RomData.Put(0x13ad4, 0x60);
                RomData.Put(0x13ad5, 0xBF);

                //fix for key glitch
                RomData.Put(0x11b37, 0xea);
                RomData.Put(0x11b38, 0xea);
                RomData.Put(0x11b39, 0xea);
            }

        }

        private void Swap(Location p1, Location p2)
        {
            int tempw = p1.World;
            p1.World = p2.World;
            p2.World = tempw;

            tempw = p1.Map;
            p1.Map = p2.Map;
            p2.Map = tempw;

            tempw = p1.PalNum;
            p1.PalNum = p2.PalNum;
            p2.PalNum = tempw;

            tempw = p1.TownNum;
            p1.TownNum = p2.TownNum;
            p2.TownNum = tempw;

            Items i = p1._item;
            p1._item = p2._item;
            p2._item = i;
        }

        private void LoadItemLocs()
        {
            _itemLocs = new List<Location>();
            if (_westHyrule._palaceOne.PalNum != 7)
            {
                _itemLocs.Add(_westHyrule._palaceOne);
            }
            if (_westHyrule._palaceTwo.PalNum != 7)
            {
                _itemLocs.Add(_westHyrule._palaceTwo);
            }
            if (_westHyrule._palaceThree.PalNum != 7)
            {
                _itemLocs.Add(_westHyrule._palaceThree);
            }
            if (_mazeIsland._palace4.PalNum != 7)
            {
                _itemLocs.Add(_mazeIsland._palace4);
            }
            if (_eastHyrule._palace5.PalNum != 7)
            {
                _itemLocs.Add(_eastHyrule._palace5);
            }
            if (_eastHyrule._palace6.PalNum != 7)
            {
                _itemLocs.Add(_eastHyrule._palace6);
            }
            if (_eastHyrule._gp.PalNum != 7)
            {
                _itemLocs.Add(_eastHyrule._gp);
            }
            _itemLocs.Add(_westHyrule._heartOne);
            _itemLocs.Add(_westHyrule._heartTwo);
            _itemLocs.Add(_westHyrule._jar);
            _itemLocs.Add(_westHyrule._medicineCave);
            _itemLocs.Add(_westHyrule._trophyCave);
            _itemLocs.Add(_eastHyrule._heart1);
            _itemLocs.Add(_eastHyrule._heart2);
            _itemLocs.Add(_eastHyrule._newKasuto);
            _itemLocs.Add(_eastHyrule._newKasuto2);
            _itemLocs.Add(_deathMountain._magicCave);
            _itemLocs.Add(_deathMountain._hammerCave);
            _itemLocs.Add(_mazeIsland._kid);
            _itemLocs.Add(_mazeIsland._magic);


            if (Props.IncludePbagCavesInItemShuffle)
            {
                _itemLocs.Add(_westHyrule._pbagCave);
                _itemLocs.Add(_eastHyrule._pbagCave1);
                _itemLocs.Add(_eastHyrule._pbagCave2);
            }
        }

        private void ShuffleSpells()
        {
            _spellMap = new Dictionary<Spells, Spells>();
            List<int> shuffleThis = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < _spellGet.Count(); i++)
            {
                _spellGet[i] = false;
            }
            if (Props.ShuffleSpellLocations)
            {
                for (int i = 0; i < shuffleThis.Count; i++)
                {

                    int s = R.Next(i, shuffleThis.Count);
                    int sl = shuffleThis[s];
                    shuffleThis[s] = shuffleThis[i];
                    shuffleThis[i] = sl;
                }
            }
            for (int i = 0; i < shuffleThis.Count; i++)
            {
                _spellMap.Add((Spells)i, (Spells)shuffleThis[i]);
            }
            _spellMap.Add(Spells.Upstab, Spells.Upstab);
            _spellMap.Add(Spells.Downstab, Spells.Downstab);

            if (Props.ShuffleStartingSpells)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasSpell = R.NextDouble() > .75;
                    RomData.Put(0x17AF7 + i, hasSpell ? (byte)1 : (byte)0);
                    _spellGet[(int)_spellMap[(Spells)i]] = hasSpell;
                }
            }
            else
            {
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Shield), Props.StartWithShield ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Shield] = Props.StartWithShield;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Jump), Props.StartWithJump ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Jump] = Props.StartWithJump;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Life), Props.StartWithLife ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Life] = Props.StartWithLife;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Fairy), Props.StartWithFairy ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Fairy] = Props.StartWithFairy;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Fire), Props.StartWithFire ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Fire] = Props.StartWithFire;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Reflect), Props.StartWithReflect ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Reflect] = Props.StartWithReflect;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Spell), Props.StartWithSpell ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Spell] = Props.StartWithSpell;
                RomData.Put(0x17AF7 + _spellMap.Values.ToList().IndexOf(Spells.Thunder), Props.StartWithThunder ? (byte)1 : (byte)0);
                _spellGet[(int)Spells.Thunder] = Props.StartWithThunder;
            }

            if (Props.CombineFireWithRandomSpell)
            {
                int newFire = R.Next(7);
                if (newFire > 3)
                {
                    newFire++;
                }
                byte newnewFire = (byte)(0x10 | RomData.GetByte(0xDCB + newFire));
                RomData.Put(0xDCF, newnewFire);
            }
        }

        private void ShuffleExp(int start)
        {
            int[] exp = new int[8];

            for (int i = 0; i < exp.Length; i++)
            {
                exp[i] = RomData.GetByte(start + i) * 256;
                exp[i] = exp[i] + RomData.GetByte(start + 24 + i);
            }

            for (int i = 0; i < exp.Length; i++)
            {
                int nextMin = (int)(exp[i] - exp[i] * 0.25);
                int nextMax = (int)(exp[i] + exp[i] * 0.25);
                if (i == 0)
                {
                    exp[i] = R.Next(Math.Max(10, nextMin), nextMax);
                }
                else
                {
                    exp[i] = R.Next(Math.Max(exp[i - 1], nextMin), Math.Min(nextMax, 9990));
                }
            }

            for (int i = 0; i < exp.Length; i++)
            {
                exp[i] = exp[i] / 10 * 10;
            }

            for (int i = 0; i < exp.Length; i++)
            {
                RomData.Put(start + i, (byte)(exp[i] / 256));
                RomData.Put(start + 24 + i, (byte)(exp[i] % 256));
            }

            for (int i = 0; i < exp.Length; i++)
            {

                RomData.Put(start + 2057 + i, IntToText(exp[i] / 1000));
                exp[i] = exp[i] - ((exp[i] / 1000) * 1000);
                RomData.Put(start + 2033 + i, IntToText(exp[i] / 100));
                exp[i] = exp[i] - ((exp[i] / 100) * 100);
                RomData.Put(start + 2009 + i, IntToText(exp[i] / 10));
            }
        }

        private void ShuffleBits(List<int> addr, bool fire)
        {
            int mask = 0x10;
            int notMask = 0xEF;
            if (fire)
            {
                mask = 0x20;
                notMask = 0xDF;
            }

            double count = 0;
            foreach (int i in addr)
            {
                if ((RomData.GetByte(i) & mask) > 0)
                {
                    count++;
                }
            }

            double fraction = count / addr.Count;

            foreach (int i in addr)
            {
                int part1 = 0;
                int part2 = RomData.GetByte(i) & notMask;
                bool havethis = R.NextDouble() <= fraction;
                if (havethis)
                {
                    part1 = mask;
                }
                RomData.Put(i, (byte)(part1 + part2));
            }
        }

        private void ShuffleEnemyExp(List<int> addr)
        {
            foreach (int i in addr)
            {
                byte exp = RomData.GetByte(i);
                int high = exp & 0xF0;
                int low = exp & 0x0F;

                low = R.Next(low - 2, low + 3);
                if (low < 0)
                {
                    low = 0;
                }
                else if (low > 15)
                {
                    low = 15;
                }
                RomData.Put(i, (byte)(high + low));
            }
        }

        private void ShuffleEncounters(List<int> addr)
        {
            for (int i = 0; i < addr.Count; i++)
            {
                int swap = R.Next(i, addr.Count);
                byte temp = RomData.GetByte(addr[i]);
                RomData.Put(addr[i], RomData.GetByte(addr[swap]));
                RomData.Put(addr[swap], temp);
            }
        }
        private void RandomizeStartingValues()
        {
            if (Props.RemoveSpellItems)
            {
                RomData.Put(0xF584, 0xA9);
                RomData.Put(0xF585, 0x01);
                RomData.Put(0xF586, 0xEA);
            }
            UpdateSprites();

            Dictionary<string, int> colorMap = new Dictionary<string, int> { { "Green", 0x2A }, { "Dark Green", 0x0A }, { "Aqua", 0x3C }, { "Dark Blue", 0x02 }, { "Purple", 0x04 }, { "Pink", 0x24 }, { "Red", 0x16 }, { "Orange", 0x27 }, { "Turd", 0x18 } };

            /*colors to include
                Green (2A)
                Dark Green (0A)
                Aqua (3C)
                Dark Blue (02)
                Purple (04)
                Pink (24)
                Red (16)
                Orange (27)
                Turd (08)
            */
            int c2 = 0;
            int c1 = 0;

            if (Props.TunicColor.Equals("Default"))
            {
                if (Props.CharacterSprite.Equals("Link"))
                {
                    c2 = colorMap["Green"];
                }
                else if (Props.CharacterSprite.Equals("Iron Knuckle"))
                {
                    c2 = colorMap["Dark Blue"];
                }
                else if (Props.CharacterSprite.Equals("Error"))
                {
                    c2 = 0x13;
                }
                else if (Props.CharacterSprite.Equals("Samus"))
                {
                    c2 = 0x27;
                }
                else if (Props.CharacterSprite.Equals("Simon"))
                {
                    c2 = 0x27;
                }
                else if (Props.CharacterSprite.Equals("Stalfos"))
                {
                    c2 = colorMap["Red"];
                }
                else if (Props.CharacterSprite.Equals("Vase Lady"))
                {
                    c2 = 0x13;
                }
                else if (Props.CharacterSprite.Equals("Ruto"))
                {
                    c2 = 0x30;
                }
            }
            else if (!Props.TunicColor.Equals("Random"))
            {
                c2 = colorMap[Props.TunicColor];
            }

            if (Props.ShieldTunicColor.Equals("Default"))
            {
                if (Props.CharacterSprite.Equals("Link"))
                {
                    c1 = colorMap["Red"];
                }
                else if (Props.CharacterSprite.Equals("Iron Knuckle"))
                {
                    c1 = colorMap["Red"];
                }
                else if (Props.CharacterSprite.Equals("Error"))
                {
                    c1 = colorMap["Red"];
                }
                else if (Props.CharacterSprite.Equals("Samus"))
                {
                    c1 = 0x37;
                }
                else if (Props.CharacterSprite.Equals("Simon"))
                {
                    c1 = 0x16;
                }
                else if (Props.CharacterSprite.Equals("Stalfos"))
                {
                    c1 = colorMap["Dark Blue"];
                }
                else if (Props.CharacterSprite.Equals("Vase Lady"))
                {
                    c1 = colorMap["Red"];
                }
                else if (Props.CharacterSprite.Equals("Ruto"))
                {
                    c1 = 0x3c;
                }

            }
            else if (!Props.ShieldTunicColor.Equals("Random"))
            {
                c1 = colorMap[Props.ShieldTunicColor];
            }
            if (Props.TunicColor.Equals("Random"))
            {
                Random r2 = new Random();

                int c2P1 = r2.Next(3);
                int c2P2 = r2.Next(1, 13);
                c2 = c2P1 * 16 + c2P2;

                while (c1 == c2)
                {
                    c2P1 = r2.Next(3);
                    c2P2 = r2.Next(1, 13);
                    c2 = c2P1 * 16 + c2P2;
                }
            }

            if (Props.ShieldTunicColor.Equals("Random"))
            {
                Random r2 = new Random();

                int c1P1 = r2.Next(3);
                int c1P2 = r2.Next(1, 13);

                c1 = c1P1 * 16 + c1P2;

                while (c1 == c2)
                {
                    c1P1 = r2.Next(3);
                    c1P2 = r2.Next(1, 13);
                    c1 = c1P1 * 16 + c1P2;
                }
            }

            int[] tunicLocs = { 0x285C, 0x40b1, 0x40c1, 0x40d1, 0x80e1, 0x80b1, 0x80c1, 0x80d1, 0x80e1, 0xc0b1, 0xc0c1, 0xc0d1, 0xc0e1, 0x100b1, 0x100c1, 0x100d1, 0x100e1, 0x140b1, 0x140c1, 0x140d1, 0x140e1, 0x17c1b, 0x1c466, 0x1c47e };

            foreach (int l in tunicLocs)
            {
                RomData.Put(0x10ea, (byte)c2);
                if (Props.CharacterSprite.Equals("Iron Knuckle"))
                {
                    RomData.Put(0x10ea, 0x30);
                    RomData.Put(0x2a0a, 0x0D);
                    RomData.Put(0x2a10, (byte)c2);
                    RomData.Put(l, 0x20);
                    RomData.Put(l - 1, (byte)c2);
                    RomData.Put(l - 2, 0x0D);
                }
                else if (Props.CharacterSprite.Equals("Samus"))
                {
                    RomData.Put(0x2a0a, 0x16);
                    RomData.Put(0x2a10, 0x1a);
                    RomData.Put(l, (byte)c2);
                    RomData.Put(l - 1, 0x1a);
                    RomData.Put(l - 2, 0x16);
                }
                else if (Props.CharacterSprite.Equals("Error") || Props.CharacterSprite.Equals("Vase Lady"))
                {
                    RomData.Put(0x2a0a, 0x0F);
                    RomData.Put(l, (byte)c2);
                    RomData.Put(l - 2, 0x0F);
                }
                else if (Props.CharacterSprite.Equals("Simon"))
                {
                    RomData.Put(0x2a0a, 0x07);
                    RomData.Put(0x2a10, 0x37);
                    RomData.Put(l, (byte)c2);
                    RomData.Put(l - 1, 0x37);
                    RomData.Put(l - 2, 0x07);
                }
                else if (Props.CharacterSprite.Equals("Stalfos"))
                {
                    RomData.Put(0x2a0a, 0x08);
                    RomData.Put(0x2a10, 0x20);
                    RomData.Put(l, (byte)c2);
                    RomData.Put(l - 1, 0x20);
                    RomData.Put(l - 2, 0x08);
                }
                else if (Props.CharacterSprite.Equals("Ruto"))
                {
                    RomData.Put(0x2a0a, 0x0c);
                    RomData.Put(0x2a10, 0x1c);
                    RomData.Put(l, (byte)c2);
                    RomData.Put(l - 1, 0x1c);
                    RomData.Put(l - 2, 0x0c);
                }
                else
                {
                    RomData.Put(0x10ea, (byte)c2);
                    RomData.Put(l, (byte)c2);
                }
            }

            RomData.Put(0xe9e, (byte)c1);



            int beamType = -1;
            if (Props.BeamSprite.Equals("Random"))
            {

                Random r2 = new Random();
                beamType = r2.Next(6);
            }
            else if (Props.BeamSprite.Equals("Fire"))
            {
                beamType = 0;
            }
            else if (Props.BeamSprite.Equals("Bubble"))
            {
                beamType = 1;
            }
            else if (Props.BeamSprite.Equals("Rock"))
            {
                beamType = 2;
            }
            else if (Props.BeamSprite.Equals("Axe"))
            {
                beamType = 3;
            }
            else if (Props.BeamSprite.Equals("Hammer"))
            {
                beamType = 4;
            }
            else if (Props.BeamSprite.Equals("Wizzrobe Beam"))
            {
                beamType = 5;
            }
            byte[] newSprite = new byte[32];

            if (beamType == 0 || beamType == 3 || beamType == 4)
            {
                RomData.Put(0x18f5, 0xa9);
                RomData.Put(0x18f6, 0x00);
                RomData.Put(0x18f7, 0xea);
            }
            else if (beamType != -1)
            {
                RomData.Put(0X18FB, 0x84);
            }

            if (beamType == 1)//bubbles
            {
                for (int i = 0; i < 32; i++)
                {
                    byte next = RomData.GetByte(0x20ab0 + i);
                    newSprite[i] = next;
                }
            }

            if (beamType == 2)//rocks
            {
                for (int i = 0; i < 32; i++)
                {
                    byte next = RomData.GetByte(0x22af0 + i);
                    newSprite[i] = next;
                }
            }

            if (beamType == 3)//axes
            {
                for (int i = 0; i < 32; i++)
                {
                    byte next = RomData.GetByte(0x22fb0 + i);
                    newSprite[i] = next;
                }
            }

            if (beamType == 4)//hammers
            {
                for (int i = 0; i < 32; i++)
                {
                    byte next = RomData.GetByte(0x32ef0 + i);
                    newSprite[i] = next;
                }
            }

            if (beamType == 5)//wizzrobe beam
            {
                for (int i = 0; i < 32; i++)
                {
                    byte next = RomData.GetByte(0x34dd0 + i);
                    newSprite[i] = next;
                }
            }


            if (beamType != 0 && beamType != -1)
            {
                foreach (int loc in Pointers.FireLocations)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        RomData.Put(loc + i, newSprite[i]);
                    }
                }
            }


            if (Props.DisableLowHealthBeep)
            {
                RomData.Put(0x1D4E4, 0xEA);
                RomData.Put(0x1D4E5, 0x38);
            }
            if (Props.ShuffleLifeRefill)
            {
                int lifeRefill = R.Next(1, 6);
                RomData.Put(0xE7A, (byte)(lifeRefill * 16));
            }

            if (Props.ShuffleAmountExpStolen)
            {
                int small = RomData.GetByte(0x1E30E);
                int big = RomData.GetByte(0x1E314);
                small = R.Next((int)(small - small * .5), (int)(small + small * .5) + 1);
                big = R.Next((int)(big - big * .5), (int)(big + big * .5) + 1);
                RomData.Put(0x1E30E, (byte)small);
                RomData.Put(0x1E314, (byte)big);
            }

            List<int> addr = new List<int>();
            for (int i = 0x54E8; i < 0x54ED; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x54EF; i < 0x54F8; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x54F9; i < 0x5508; i++)
            {
                addr.Add(i);
            }

            if (Props.ShuffleWhichEnemiesStealExp)
            {
                ShuffleBits(addr, false);
            }

            if (Props.ShuffleSwordImmunity)
            {
                ShuffleBits(addr, true);
            }

            if (Props.ShuffleEnemyExp)
            {
                ShuffleEnemyExp(addr);
            }
            addr = new List<int>();
            for (int i = 0x94E8; i < 0x94ED; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x94EF; i < 0x94F8; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x94F9; i < 0x9502; i++)
            {
                addr.Add(i);
            }
            if (Props.ShuffleWhichEnemiesStealExp)
            {
                ShuffleBits(addr, false);
            }

            if (Props.ShuffleSwordImmunity)
            {
                ShuffleBits(addr, true);
            }
            if (Props.ShuffleEnemyExp)
            {
                ShuffleEnemyExp(addr);
            }

            addr = new List<int>();
            for (int i = 0x114E8; i < 0x114EA; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x114EB; i < 0x114ED; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x114EF; i < 0x114F8; i++)
            {
                addr.Add(i);
            }
            for (int i = 0x114FD; i < 0x11505; i++)
            {
                addr.Add(i);
            }
            addr.Add(0x11508);

            if (Props.ShuffleWhichEnemiesStealExp)
            {
                ShuffleBits(addr, false);
            }

            if (Props.ShuffleSwordImmunity)
            {
                ShuffleBits(addr, true);
            }
            if (Props.ShuffleEnemyExp)
            {
                ShuffleEnemyExp(addr);
            }

            addr = new List<int>();
            for (int i = 0x129E8; i < 0x129EA; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x129EB; i < 0x129ED; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x129EF; i < 0x129F4; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x129F5; i < 0x129F7; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x129FD; i < 0x12A05; i++)
            {
                addr.Add(i);
            }

            addr.Add(0x12A08);

            if (Props.ShuffleWhichEnemiesStealExp)
            {
                ShuffleBits(addr, false);
            }

            if (Props.ShuffleSwordImmunity)
            {
                ShuffleBits(addr, true);
            }
            if (Props.ShuffleEnemyExp)
            {
                ShuffleEnemyExp(addr);
            }

            addr = new List<int>();
            for (int i = 0x154E9; i < 0x154ED; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x154F2; i < 0x154F8; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x154F9; i < 0x15500; i++)
            {
                addr.Add(i);
            }

            for (int i = 0x15502; i < 15504; i++)
            {
                addr.Add(i);
            }

            if (Props.ShuffleWhichEnemiesStealExp)
            {
                ShuffleBits(addr, false);
            }

            if (Props.ShuffleSwordImmunity)
            {
                ShuffleBits(addr, true);
            }
            if (Props.ShuffleEnemyExp)
            {
                ShuffleEnemyExp(addr);
            }

            if (Props.ShuffleBossExp)
            {
                addr = new List<int>
                {
                    0x11505,
                    0x13C88,
                    0x13C89,
                    0x12A05,
                    0x12A06,
                    0x12A07,
                    0x15507
                };
                ShuffleEnemyExp(addr);
            }

            if (Props.ShuffleEncounters)
            {
                addr = new List<int>
                {
                    0x441b,
                    0x4419,
                    0x441D,
                    0x4420,
                    0x441C,
                    0x441A,
                    0x4422,
                    0x441E
                };

                if (Props.AllowUnsafePathEncounters)
                {
                    addr.Add(0x4424);
                    addr.Add(0x4423);
                }

                ShuffleEncounters(addr);

                addr = new List<int>
                {
                    0x841B,
                    0x8419,
                    0x841D,
                    0x8422,
                    0x8420,
                    0x841A,
                    0x841E,
                    0x8426
                };

                if (Props.AllowUnsafePathEncounters)
                {
                    addr.Add(0x8423);
                    addr.Add(0x8424);
                }

                ShuffleEncounters(addr);
            }

            if (Props.JumpAlwaysOn)
            {
                RomData.Put(0x1482, RomData.GetByte(0x1480));
                RomData.Put(0x1483, RomData.GetByte(0x1481));
                RomData.Put(0x1486, RomData.GetByte(0x1484));
                RomData.Put(0x1487, RomData.GetByte(0x1485));

            }

            if (Props.DisableMagicContainerRequirements)
            {
                RomData.Put(0xF539, 0xC9);
                RomData.Put(0xF53A, 0);
            }

            if (Props.ShuffleAllExperienceNeeded)
            {
                ShuffleExp(0x1669);//atk
                ShuffleExp(0x1671);//mag
                ShuffleExp(0x1679);//life
            }
            else
            {
                if (Props.ShuffleAttackExperienceNeeded)
                {
                    ShuffleExp(0x1669);
                }

                if (Props.ShuffleMagicExperienceNeeded)
                {
                    ShuffleExp(0x1671);
                }

                if (Props.ShuffleLifeExperienceNeeded)
                {
                    ShuffleExp(0x1679);
                }
            }

            ShuffleAttackEffectiveness(false);

            ShuffleLifeEffectiveness(true);

            ShuffleLifeEffectiveness(false);

            if (Props.NumberOfPalacesToComplete.Equals("Random"))
            {
                RomData.Put(0x17B10, (byte)R.Next(0, 7));
            }
            else
            {
                RomData.Put(0x17B10, (byte)int.Parse(Props.NumberOfPalacesToComplete));
            }

            if (Props.StartingHeartContainers.Equals("Random"))
            {
                _startHearts = R.Next(1, 9);
                RomData.Put(0x17B00, (byte)_startHearts);
            }
            else
            {
                _startHearts = int.Parse(Props.StartingHeartContainers);
                RomData.Put(0x17B00, (byte)_startHearts);
            }

            if (Props.MaximumHeartContainers.Equals("Random"))
            {
                _maxHearts = R.Next(_startHearts, 9);
            }
            else
            {
                _maxHearts = int.Parse(Props.MaximumHeartContainers);
            }

            _numHContainers = _maxHearts - _startHearts;

            if (Props.RandomizeNumberOfLives)
            {
                RomData.Put(0x1C369, (byte)R.Next(2, 6));
            }

            if (Props.StartingTechs.Equals("Random"))
            {
                int swap = R.Next(7);
                if (swap <= 3)
                {
                    RomData.Put(0x17B12, 0);
                }
                else if (swap == 4)
                {
                    RomData.Put(0x17B12, 0x10);
                }
                else if (swap == 5)
                {
                    RomData.Put(0x17B12, 0x04);
                }
                else
                {
                    RomData.Put(0x17B12, 0x14);
                }
            }
            else if (Props.StartingTechs.Equals("Downstab"))
            {
                RomData.Put(0x17B12, 0x10);
            }
            else if (Props.StartingTechs.Equals("Upstab"))
            {
                RomData.Put(0x17B12, 0x04);
            }
            else if (Props.StartingTechs.Equals("Both"))
            {
                RomData.Put(0x17B12, 0x14);
            }
            else
            {
                RomData.Put(0x17B12, 0x00);
            }

            if (Props.InvincibleLifeEffectiveness)
            {
                for (int i = 0x1E2BF; i < 0x1E2BF + 56; i++)
                {
                    RomData.Put(i, 0);
                }
            }

            if (Props.OhkoLifeEffectiveness)
            {
                for (int i = 0x1E2BF; i < 0x1E2BF + 56; i++)
                {
                    RomData.Put(i, 0xFF);
                }
            }

            if (Props.FreeMagicEffectiveness)
            {
                for (int i = 0xD8B; i < 0xD8b + 64; i++)
                {
                    RomData.Put(i, 0);
                }
            }

            if (Props.ChangePalacePalettes)
            {

                int[,] bSprites = new int[7, 32];
                int[,] binSprites = new int[7, 32];
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        bSprites[i, j] = RomData.GetByte(Pointers.BrickSpritePointers[i] + j);
                        binSprites[i, j] = RomData.GetByte(Pointers.InsidePalaceBrickSprites[i] + j);
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    int group = R.Next(3);
                    int[] bricks = new int[3];
                    int[] curtains = new int[3];
                    if (group == 0)
                    {
                        int brickRow = R.Next(BrickGroups.BrickGroupZero.GetLength(0));
                        bricks[0] = BrickGroups.BrickGroupZero[brickRow, 0];
                        bricks[1] = BrickGroups.BrickGroupZero[brickRow, 1];
                        bricks[2] = BrickGroups.BrickGroupZero[brickRow, 2];
                        int curtainRow = R.Next(CurtainGroups.CurtainGroupZero.GetLength(0));
                        curtains[0] = CurtainGroups.CurtainGroupZero[curtainRow, 0];
                        curtains[1] = CurtainGroups.CurtainGroupZero[curtainRow, 1];
                        curtains[2] = CurtainGroups.CurtainGroupZero[curtainRow, 2];

                    }
                    else if (group == 1)
                    {
                        int brickRow = R.Next(BrickGroups.BrickGroupOne.GetLength(0));
                        bricks[0] = BrickGroups.BrickGroupOne[brickRow, 0];
                        bricks[1] = BrickGroups.BrickGroupOne[brickRow, 1];
                        bricks[2] = BrickGroups.BrickGroupOne[brickRow, 2];
                        int curtainRow = R.Next(CurtainGroups.CurtainGroupOne.GetLength(0));
                        curtains[0] = CurtainGroups.CurtainGroupOne[curtainRow, 0];
                        curtains[1] = CurtainGroups.CurtainGroupOne[curtainRow, 1];
                        curtains[2] = CurtainGroups.CurtainGroupOne[curtainRow, 2];
                    }
                    else
                    {
                        int brickRow = R.Next(BrickGroups.BrickGroupTwo.GetLength(0));
                        bricks[0] = BrickGroups.BrickGroupTwo[brickRow, 0];
                        bricks[1] = BrickGroups.BrickGroupTwo[brickRow, 1];
                        bricks[2] = BrickGroups.BrickGroupTwo[brickRow, 2];
                        int curtainRow = R.Next(CurtainGroups.CurtainGroupTwo.GetLength(0));
                        curtains[0] = CurtainGroups.CurtainGroupTwo[curtainRow, 0];
                        curtains[1] = CurtainGroups.CurtainGroupTwo[curtainRow, 1];
                        curtains[2] = CurtainGroups.CurtainGroupTwo[curtainRow, 2];
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        RomData.Put(Pointers.OutsidePalaceBricks[i] + j, (byte)bricks[j]);
                        RomData.Put(Pointers.InsidePalaceBricks[i] + j, (byte)bricks[j]);
                        RomData.Put(Pointers.InsidePalaceCurtains[i] + j, (byte)curtains[j]);
                        if (j == 0)
                        {
                            RomData.Put(Pointers.InsidePalaceWindows[i] + j, (byte)bricks[j]);
                        }
                    }
                    int bRow = R.Next(7);
                    int binRow = R.Next(7);
                    for (int j = 0; j < 32; j++)
                    {
                        RomData.Put(Pointers.BrickSpritePointers[i] + j, (byte)bSprites[bRow, j]);
                        RomData.Put(Pointers.InsidePalaceBrickSprites[i] + j, (byte)binSprites[binRow, j]);
                    }
                }
            }

            if (Props.ShuffleItemDropFrequency)
            {
                int drop = R.Next(5) + 4;
                RomData.Put(0x1E8B0, (byte)drop);
            }

            char[] randoby = ToGameText("RANDO BY  ");
            for (int i = 0; i < randoby.Length; i++)
            {
                RomData.Put(0x15377 + i, (byte)randoby[i]);
            }

            char[] digshake = ToGameText("DIGSHAKE ");
            for (int i = 0; i < digshake.Length; i++)
            {
                RomData.Put(0x15384 + i, (byte)digshake[i]);
            }
        }

        private byte IntToText(int x)
        {
            switch (x)
            {
                case 0:
                    return 0xD0;
                case 1:
                    return 0xD1;
                case 2:
                    return 0xD2;
                case 3:
                    return 0xD3;
                case 4:
                    return 0xD4;
                case 5:
                    return 0xD5;
                case 6:
                    return 0xD6;
                case 7:
                    return 0xD7;
                case 8:
                    return 0xD8;
                default:
                    return 0xD9;
            }
        }

        private void LoadPalaces()
        {
            for (int i = 1; i < 8; i++)
            {
                Palace p = new Palace(i, Pointers.PalaceAddresses[i], Pointers.PalaceConnectionLocations[i], this);
                if (i == 7 && Props.ShortenGreatPalace)
                {
                    p.Shorten();
                }
                if (Props.ShufflePalaceRooms)
                {
                    p.ShuffleRooms();
                }
                while (!p.AllReachable() || (i == 7 && (Props.ThunderbirdRequired && !p.RequiresThunderbird())) || p.HasDeadEnd())
                {
                    p = new Palace(i, Pointers.PalaceAddresses[i], Pointers.PalaceConnectionLocations[i], this);
                    if (i == 7 && Props.ShortenGreatPalace)
                    {
                        p.Shorten();
                    }
                    if (Props.ShufflePalaceRooms)
                    {
                        p.ShuffleRooms();
                    }
                }
                _palaces.Add(p);
            }
            if (Props.ShufflePalaceEnemies)
            {
                ShuffleE(Pointers.EnemyPointerOne, Pointers.EnemyAddressOne, Pointers.EnemiesOne, Pointers.GeneratorsOne, Pointers.ShortEnemiesOne, Pointers.TallEnemiesOne, Pointers.FlyingEnemiesOne, false);
                ShuffleE(Pointers.EnemyPointerTwo, Pointers.EnemyAddressOne, Pointers.EnemiesTwo, Pointers.GeneratorsTwo, Pointers.ShortEnemiesTwo, Pointers.TallEnemiesTwo, Pointers.FlyingEnemiesTwo, false);
                ShuffleE(Pointers.EnemyPointerThree, Pointers.EnemyAddressTwo, Pointers.EnemiesThree, Pointers.GeneratorsThree, Pointers.ShortEnemiesThree, Pointers.TallEnemiesThree, Pointers.FlyingEnemiesThree, true);
            }
            if (Props.ShuffleSmallItems || Props.PalacesContainsExtraKeys)
            {
                _palaces[0].ShuffleSmallItems(4, true);
                _palaces[1].ShuffleSmallItems(4, true);
                _palaces[2].ShuffleSmallItems(4, false);
                _palaces[3].ShuffleSmallItems(4, false);
                _palaces[4].ShuffleSmallItems(4, true);
                _palaces[5].ShuffleSmallItems(4, false);
                _palaces[6].ShuffleSmallItems(5, true);
            }
            _palaces[1].NeedJumpOrFairy = !_palaces[1].CheckBlocks(new List<int> { 32 * 4 }).Contains(80);
            _palaces[1].NeedGlove = !_palaces[1].CheckBlocks(new List<int> { 26 * 4 }).Contains(80);
            _palaces[2].NeedDstab = !_palaces[2].CheckBlocks(new List<int> { 52 }).Contains(44);
            _palaces[2].NeedGlove = !_palaces[2].CheckBlocks(new List<int> { 40, 52 }).Contains(44);
            _palaces[3].NeedFairy = !_palaces[2].CheckBlocks(new List<int> { 96 }).Contains(124);
            _palaces[4].NeedFairy = !_palaces[4].CheckBlocks(new List<int> { 148 }).Contains(244);
            _palaces[5].NeedFairy = !_palaces[5].CheckBlocks(new List<int> { 204, 228 }).Contains(176);
            _palaces[5].NeedGlove = !_palaces[5].CheckBlocks(new List<int> { 172, 188, 196 }).Contains(176);
        }

        private void UpdateRom()
        {
            foreach (World w in _worlds)
            {
                List<Location> locs = w.AllLocations;
                foreach (Location l in locs)
                {
                    l.UpdateBytes();
                    RomData.Put(l.MemAddress, l.LocationBytes[0]);
                    RomData.Put(l.MemAddress + Pointers.OverworldXOffset, l.LocationBytes[1]);
                    RomData.Put(l.MemAddress + Pointers.OverworldMapOffset, l.LocationBytes[2]);
                    RomData.Put(l.MemAddress + Pointers.OverworldWorldOffset, l.LocationBytes[3]);
                }
            }

            foreach (Palace p in _palaces)
            {
                p.UpdateRom();
            }
            Location medicineLoc = null;
            Location trophyLoc = null;
            Location kidLoc = null;
            foreach (Location l in _itemLocs)
            {
                if (l._item == Items.Medicine)
                {
                    medicineLoc = l;
                }
                if (l._item == Items.Trophy)
                {
                    trophyLoc = l;
                }
                if (l._item == Items.Kid)
                {
                    kidLoc = l;
                }
            }

            byte[] medSprite = new byte[32];
            byte[] trophySprite = new byte[32];
            byte[] kidSprite = new byte[32];

            for (int i = 0; i < 32; i++)
            {
                medSprite[i] = RomData.GetByte(0x23310 + i);
                trophySprite[i] = RomData.GetByte(0x232f0 + i);
                kidSprite[i] = RomData.GetByte(0x25310 + i);
            }
            bool medEast = (_eastHyrule.AllLocations.Contains(medicineLoc) || _mazeIsland.AllLocations.Contains(medicineLoc));
            bool trophyEast = (_eastHyrule.AllLocations.Contains(trophyLoc) || _mazeIsland.AllLocations.Contains(trophyLoc));
            bool kidWest = (_westHyrule.AllLocations.Contains(kidLoc) || _deathMountain.AllLocations.Contains(kidLoc));
            Dictionary<int, int> palaceMems = new Dictionary<int, int>
            {
                { 1, 0x29AD0 },
                { 2, 0x2BAD0 },
                { 3, 0x33AD0 },
                { 4, 0x35AD0 },
                { 5, 0x37AD0 },
                { 6, 0x39AD0 }
            };

            if (medEast && _eastHyrule._palace5._item != Items.Medicine && _eastHyrule._palace6._item != Items.Medicine && _mazeIsland._palace4._item != Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x25430 + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0x43);
                RomData.Put(0x1eeba, 0x43);
            }

            if (trophyEast)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x25410 + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0x41);
                RomData.Put(0x1eeb8, 0x41);
            }

            if (kidWest && _westHyrule._palaceOne._item != Items.Kid && _westHyrule._palaceTwo._item != Items.Kid && _westHyrule._palaceThree._item != Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x23570 + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0x57);
                RomData.Put(0x1eeb6, 0x57);
            }

            if (_eastHyrule._newKasuto._item == Items.Trophy || _eastHyrule._newKasuto2._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x27210 + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0x21);
                RomData.Put(0x1eeb8, 0x21);
            }

            if (_eastHyrule._newKasuto._item == Items.Medicine || _eastHyrule._newKasuto2._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x27230 + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0x23);
                RomData.Put(0x1eeba, 0x23);
            }

            if (_eastHyrule._newKasuto._item == Items.Kid || _eastHyrule._newKasuto2._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(0x27250 + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0x25);
                RomData.Put(0x1eeb6, 0x25);
            }

            if (_westHyrule._palaceOne._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceOne.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_westHyrule._palaceTwo._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceTwo.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_westHyrule._palaceThree._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceThree.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_mazeIsland._palace4._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_mazeIsland._palace4.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_eastHyrule._palace5._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace5.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_eastHyrule._palace6._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace6.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }
            if (_eastHyrule._gp._item == Items.Trophy)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._gp.PalNum] + i, trophySprite[i]);
                }
                RomData.Put(0x1eeb7, 0xAD);
                RomData.Put(0x1eeb8, 0xAD);
            }

            if (_westHyrule._palaceOne._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceOne.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_westHyrule._palaceTwo._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceTwo.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_westHyrule._palaceThree._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceThree.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_mazeIsland._palace4._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_mazeIsland._palace4.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_eastHyrule._palace5._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace5.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_eastHyrule._palace6._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace6.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }
            if (_eastHyrule._gp._item == Items.Medicine)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._gp.PalNum] + i, medSprite[i]);
                }
                RomData.Put(0x1eeb9, 0xAD);
                RomData.Put(0x1eeba, 0xAD);
            }

            if (_westHyrule._palaceOne._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceOne.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_westHyrule._palaceTwo._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceTwo.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_westHyrule._palaceThree._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_westHyrule._palaceThree.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_mazeIsland._palace4._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_mazeIsland._palace4.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_eastHyrule._palace5._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace5.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_eastHyrule._palace6._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._palace6.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }
            if (_eastHyrule._gp._item == Items.Kid)
            {
                for (int i = 0; i < 32; i++)
                {
                    RomData.Put(palaceMems[_eastHyrule._gp.PalNum] + i, kidSprite[i]);
                }
                RomData.Put(0x1eeb5, 0xAD);
                RomData.Put(0x1eeb6, 0xAD);
            }


            RomData.Put(0x1CD3A, (byte)Pointers.PalaceGraphics[_westHyrule._palaceOne.PalNum]);


            RomData.Put(0x1CD3B, (byte)Pointers.PalaceGraphics[_westHyrule._palaceTwo.PalNum]);


            RomData.Put(0x1CD3C, (byte)Pointers.PalaceGraphics[_westHyrule._palaceThree.PalNum]);


            RomData.Put(0x1CD3E, (byte)Pointers.PalaceGraphics[_mazeIsland._palace4.PalNum]);


            RomData.Put(0x1CD42, (byte)Pointers.PalaceGraphics[_eastHyrule._palace5.PalNum]);

            RomData.Put(0x1CD43, (byte)Pointers.PalaceGraphics[_eastHyrule._palace6.PalNum]);
            RomData.Put(0x1CD44, (byte)Pointers.PalaceGraphics[_eastHyrule._gp.PalNum]);


            RomData.Put(0x1CD45, (byte)Pointers.PalacePallates[_westHyrule._palaceOne.PalNum]);

            RomData.Put(0x1CD46, (byte)Pointers.PalacePallates[_westHyrule._palaceTwo.PalNum]);

            RomData.Put(0x1CD47, (byte)Pointers.PalacePallates[_westHyrule._palaceThree.PalNum]);

            RomData.Put(0x1CD49, (byte)Pointers.PalacePallates[_mazeIsland._palace4.PalNum]);

            RomData.Put(0x1CD4D, (byte)Pointers.PalacePallates[_eastHyrule._palace5.PalNum]);

            RomData.Put(0x1CD4E, (byte)Pointers.PalacePallates[_eastHyrule._palace6.PalNum]);

            RomData.Put(0x1CD4F, (byte)Pointers.PalacePallates[_eastHyrule._gp.PalNum]);

            if (Props.ShuffleDripperEnemies)
            {
                RomData.Put(0x11927, (byte)Pointers.EnemiesOne[R.Next(Pointers.EnemiesOne.Count)]);
            }

            if (Props.ShuffleSpritePallate)
            {
                List<int> doubleLocs = new List<int> { 0x40b4, 0x80b4, 0x100b4, 0x100b8, 0x100bc, 0x140b4, 0x140b8, 0x140bc };
                List<int> singleLocs = new List<int> { 0x40b8, 0x40bc, 0x80b8, 0x80bc };

                foreach (int i in doubleLocs)
                {
                    int low = R.Next(12) + 1;
                    int high = (R.Next(2) + 1) * 16;
                    int color = high + low;
                    RomData.Put(i, (byte)color);
                    RomData.Put(i + 16, (byte)color);
                    RomData.Put(i - 1, (byte)(color - 15));
                    RomData.Put(i + 16 - 1, (byte)(color - 15));
                }
                foreach (int i in singleLocs)
                {
                    int low = R.Next(13);
                    int high = (R.Next(3)) * 16;
                    int color = high + low;
                    RomData.Put(i, (byte)color);
                    RomData.Put(i + 16, (byte)color);
                    RomData.Put(i + 16 - 1, (byte)(color - 15));
                }

                for (int i = 0x54e5; i < 0x5508; i++)
                {
                    int b = RomData.GetByte(i);
                    int p = b & 0x3F;
                    int n = R.Next(4);
                    n <<= 6;
                    RomData.Put(i, (byte)(n + p));
                }

                for (int i = 0x94e5; i < 0x9508; i++)
                {
                    int b = RomData.GetByte(i);
                    int p = b & 0x3F;
                    int n = R.Next(4);
                    n <<= 6;
                    RomData.Put(i, (byte)(n + p));
                }
                for (int i = 0x114e5; i < 0x11508; i++)
                {
                    int b = RomData.GetByte(i);
                    int p = b & 0x3F;
                    int n = R.Next(4);
                    n <<= 6;
                    RomData.Put(i, (byte)(n + p));
                }
                for (int i = 0x129e5; i < 0x12a09; i++)
                {
                    int b = RomData.GetByte(i);
                    int p = b & 0x3F;
                    int n = R.Next(4);
                    n <<= 6;
                    RomData.Put(i, (byte)(n + p));
                }
                for (int i = 0x154e5; i < 0x15508; i++)
                {
                    int b = RomData.GetByte(i);
                    int p = b & 0x3F;
                    int n = R.Next(4);
                    n <<= 6;
                    RomData.Put(i, (byte)(n + p));
                }
            }

            Console.WriteLine("Here");
            RomData.Put(0x4DEA, (byte)_westHyrule._trophyCave._item);
            RomData.Put(0x502A, (byte)_westHyrule._jar._item);
            RomData.Put(0x4DD7, (byte)_westHyrule._heartTwo._item);

            int[] itemLocs2 = { 0x10E91, 0x10E9A, 0x1252D, 0x12538, 0x10EA3, 0x12774 };


            RomData.Put(0x5069, (byte)_westHyrule._medicineCave._item);
            RomData.Put(0x4ff5, (byte)_westHyrule._heartOne._item);
            if (_westHyrule._palaceOne.PalNum != 7)
            {
                RomData.Put(itemLocs2[_westHyrule._palaceOne.PalNum - 1], (byte)_westHyrule._palaceOne._item);
            }
            if (_westHyrule._palaceTwo.PalNum != 7)
            {
                RomData.Put(itemLocs2[_westHyrule._palaceTwo.PalNum - 1], (byte)_westHyrule._palaceTwo._item);
            }
            if (_westHyrule._palaceThree.PalNum != 7)
            {
                RomData.Put(itemLocs2[_westHyrule._palaceThree.PalNum - 1], (byte)_westHyrule._palaceThree._item);
            }
            RomData.Put(0x65C3, (byte)_deathMountain._magicCave._item);
            RomData.Put(0x6512, (byte)_deathMountain._hammerCave._item);
            RomData.Put(0x8FAA, (byte)_eastHyrule._heart1._item);
            RomData.Put(0x9011, (byte)_eastHyrule._heart2._item);

            if (_eastHyrule._palace5.PalNum != 7)
            {
                RomData.Put(itemLocs2[_eastHyrule._palace5.PalNum - 1], (byte)_eastHyrule._palace5._item);
            }
            if (_eastHyrule._palace6.PalNum != 7)
            {
                RomData.Put(itemLocs2[_eastHyrule._palace6.PalNum - 1], (byte)_eastHyrule._palace6._item);
            }
            RomData.Put(0xDB95, (byte)_eastHyrule._newKasuto2._item); //map 47

            RomData.Put(0xDB8C, (byte)_eastHyrule._newKasuto._item); //map 46

            RomData.Put(0xA5A8, (byte)_mazeIsland._magic._item);
            RomData.Put(0xA58B, (byte)_mazeIsland._kid._item);
            if (_mazeIsland._palace4.PalNum != 7)
            {
                RomData.Put(itemLocs2[_mazeIsland._palace4.PalNum - 1], (byte)_mazeIsland._palace4._item);
            }

            if (_eastHyrule._gp.PalNum != 7)
            {
                RomData.Put(itemLocs2[_eastHyrule._gp.PalNum - 1], (byte)_eastHyrule._gp._item);
            }
            if (Props.IncludePbagCavesInItemShuffle)
            {
                RomData.Put(0x4FE2, (byte)_westHyrule._pbagCave._item);
                RomData.Put(0x8ECC, (byte)_eastHyrule._pbagCave1._item);
                RomData.Put(0x8FB3, (byte)_eastHyrule._pbagCave2._item);

            }

            foreach (Location l in _pbagHearts)
            {
                if (l == _westHyrule._pbagCave)
                {
                    RomData.Put(0x4FE2, (byte)_westHyrule._pbagCave._item);
                }

                if (l == _eastHyrule._pbagCave1)
                {
                    RomData.Put(0x8ECC, (byte)_eastHyrule._pbagCave1._item);
                }
                if (l == _eastHyrule._pbagCave2)
                {
                    RomData.Put(0x8FB3, (byte)_eastHyrule._pbagCave2._item);
                }
            }

            //Update raft animation
            RomData.Put(0x538, (byte)_westHyrule._raftSpot.XPos);
            RomData.Put(0x53A, (byte)_westHyrule._raftSpot.YPos);
            RomData.Put(0x539, (byte)_eastHyrule._start.XPos);
            RomData.Put(0x53B, (byte)_eastHyrule._start.YPos);

            //Fix Maze Island Bridge music bug
            RomData.Put(0x565, (byte)_eastHyrule._palace4.XPos);
            RomData.Put(0x567, (byte)_eastHyrule._palace4.YPos);
            RomData.Put(0x564, (byte)_mazeIsland._start.XPos);
            RomData.Put(0x566, (byte)_mazeIsland._start.YPos);

            //Update world check for p7
            if (_westHyrule._palaceOne.PalNum == 7 || _westHyrule._palaceTwo.PalNum == 7 || _westHyrule._palaceThree.PalNum == 7)
            {
                RomData.Put(0x1dd3b, 0x05);
            }

            if (_mazeIsland._palace4.PalNum == 7)
            {
                RomData.Put(0x1dd3b, 0x0a);
            }

            Console.WriteLine("Here");

            int spellNameBase = 0x1c3a, effectBase = 0x00e58, spellCostBase = 0xd8b, functionBase = 0xdcb;

            int[,] magLevels = new int[8, 8];
            int[,] magNames = new int[8, 7];
            int[] magEffects = new int[16];
            int[] magFunction = new int[8];

            int[,] textPointers = new int[8, 2];
            for (int i = 0; i < Pointers.SpellTextPointers.Length; i++)
            {
                textPointers[i, 0] = RomData.GetByte(Pointers.SpellTextPointers[i]);
                textPointers[i, 1] = RomData.GetByte(Pointers.SpellTextPointers[i] + 1);
            }

            for (int i = 0; i < Pointers.SpellTextPointers.Length; i++)
            {
                RomData.Put(Pointers.SpellTextPointers[i], (byte)textPointers[(int)_spellMap[(Spells)i], 0]);
                RomData.Put(Pointers.SpellTextPointers[i] + 1, (byte)textPointers[(int)_spellMap[(Spells)i], 1]);
            }

            for (int i = 0; i < magFunction.Count(); i++)
            {
                magFunction[i] = RomData.GetByte(functionBase + (int)_spellMap[(Spells)i]);
            }

            for (int i = 0; i < magEffects.Count(); i += 2)
            {
                magEffects[i] = RomData.GetByte(effectBase + (int)_spellMap[(Spells)(i / 2)] * 2);
                magEffects[i + 1] = RomData.GetByte(effectBase + (int)_spellMap[(Spells)(i / 2)] * 2 + 1);
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    magLevels[i, j] = RomData.GetByte(spellCostBase + ((int)_spellMap[(Spells)i] * 8 + j));
                }

                for (int j = 0; j < 7; j++)
                {
                    magNames[i, j] = RomData.GetByte(spellNameBase + ((int)_spellMap[(Spells)i] * 0xe + j));
                }
            }

            for (int i = 0; i < magFunction.Count(); i++)
            {
                RomData.Put(functionBase + i, (byte)magFunction[i]);
            }

            for (int i = 0; i < magEffects.Count(); i += 2)
            {
                RomData.Put(effectBase + i, (byte)magEffects[i]);
                RomData.Put(effectBase + i + 1, (byte)magEffects[i + 1]);
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    RomData.Put(spellCostBase + (i * 8) + j, (byte)magLevels[i, j]);
                }

                for (int j = 0; j < 7; j++)
                {
                    RomData.Put(spellNameBase + (i * 0xe) + j, (byte)magNames[i, j]);
                }
            }

            //fix for rope graphical glitch
            for (int i = 0; i < 16; i++)
            {
                RomData.Put(0x32CD0 + i, RomData.GetByte(0x34CD0 + i));
            }

            if (_hiddenPalace)
            {
                RomData.Put(0x8664, 0);
            }

            if (_hiddenKasuto)
            {
                RomData.Put(0x8660, 0);
            }
        }

        private void UpdateSprites()
        {
            ISprite spriteObj = null;
            switch (Props.CharacterSprite)
            {
                case "Zelda":
                    spriteObj = new ZeldaSprite();
                    break;
                case "Iron Knuckle":
                    spriteObj = new IronKnuckleSprite();
                    break;
                case "Error":
                    spriteObj = new ErrorSprite();
                    break;
                case "Samus":
                    spriteObj = new SamusSprite();
                    break;
                case "Simon":
                    spriteObj = new SimonSprite();
                    break;
                case "Stalfos":
                    spriteObj = new StalfosSprite();
                    break;
                case "Vase Lady":
                    spriteObj = new VaseLadySprite();
                    break;
                case "Ruto":
                    spriteObj = new RutoSprite();
                    break;
            }

            if (spriteObj == null)
                return;

            int[] sprite = spriteObj.Sprite;
            int[] s1Up = spriteObj.OneUp;
            int[] sOw = spriteObj.Ow;
            int[] sTitle = spriteObj.Title;
            int[] sleeper = spriteObj.Sleeper;
            int[] end1 = spriteObj.EndOne;
            int[] end2 = spriteObj.EndTwo;
            int[] end3 = spriteObj.EndThree;
            int[] head = spriteObj.Head;
            int[] raft = spriteObj.Raft;

            if (sprite != null)
            {
                for (int i = 0; i < sprite.Count() * 3 / 8; i++)
                {
                    RomData.Put(0x20010 + i, (byte)sprite[i]);
                    RomData.Put(0x22010 + i, (byte)sprite[i]);
                    RomData.Put(0x24010 + i, (byte)sprite[i]);
                    RomData.Put(0x26010 + i, (byte)sprite[i]);
                    RomData.Put(0x28010 + i, (byte)sprite[i]);
                    RomData.Put(0x2a010 + i, (byte)sprite[i]);
                    RomData.Put(0x2c010 + i, (byte)sprite[i]);
                    RomData.Put(0x2e010 + i, (byte)sprite[i]);
                    RomData.Put(0x30010 + i, (byte)sprite[i]);
                    RomData.Put(0x32010 + i, (byte)sprite[i]);
                    RomData.Put(0x34010 + i, (byte)sprite[i]);
                    if (i < 0x4E0 || i >= 0x520)
                    {
                        RomData.Put(0x36010 + i, (byte)sprite[i]);
                    }
                    RomData.Put(0x38010 + i, (byte)sprite[i]);

                }

                for (int i = 0; i < 0x20; i++)
                {
                    RomData.Put(0x206d0 + i, (byte)sprite[0x6c0 + i]);
                    RomData.Put(0x2e6d0 + i, (byte)sprite[0x6c0 + i]);
                    RomData.Put(0x306d0 + i, (byte)sprite[0x6c0 + i]);
                }

                for (int i = 0; i < s1Up.Count(); i++)
                {
                    RomData.Put(0x20a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x22a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x24a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x26a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x28a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x2aa90 + i, (byte)s1Up[i]);
                    RomData.Put(0x2ca90 + i, (byte)s1Up[i]);
                    RomData.Put(0x2ea90 + i, (byte)s1Up[i]);
                    RomData.Put(0x30a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x32a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x34a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x36a90 + i, (byte)s1Up[i]);
                    RomData.Put(0x38a90 + i, (byte)s1Up[i]);


                }

                for (int i = 0; i < sOw.Count(); i++)
                {
                    RomData.Put(0x31750 + i, (byte)sOw[i]);
                }

                for (int i = 0; i < sTitle.Count(); i++)
                {
                    RomData.Put(0x20D10 + i, (byte)sTitle[i]);
                    RomData.Put(0x2ED10 + i, (byte)sTitle[i]);

                }

                for (int i = 0; i < sleeper.Count(); i++)
                {
                    RomData.Put(0x21010 + i, (byte)sleeper[i]);
                    if (i > 31)
                    {
                        RomData.Put(0x23270 + i, (byte)sleeper[i]);
                    }
                }

                for (int i = 0; i < end1.Count(); i++)
                {
                    RomData.Put(0x2ed90 + i, (byte)end1[i]);
                }

                for (int i = 0; i < end2.Count(); i++)
                {
                    RomData.Put(0x2f010 + i, (byte)end2[i]);
                }

                for (int i = 0; i < end3.Count(); i++)
                {
                    RomData.Put(0x2d010 + i, (byte)end3[i]);
                }

                for (int i = 0; i < head.Count(); i++)
                {
                    RomData.Put(0x21970 + i, (byte)head[i]);
                    RomData.Put(0x23970 + i, (byte)head[i]);
                    RomData.Put(0x25970 + i, (byte)head[i]);
                    RomData.Put(0x27970 + i, (byte)head[i]);
                    RomData.Put(0x29970 + i, (byte)head[i]);
                    RomData.Put(0x2B970 + i, (byte)head[i]);
                    RomData.Put(0x2D970 + i, (byte)head[i]);
                    RomData.Put(0x2F970 + i, (byte)head[i]);
                    RomData.Put(0x31970 + i, (byte)head[i]);
                    RomData.Put(0x33970 + i, (byte)head[i]);
                    RomData.Put(0x35970 + i, (byte)head[i]);
                    RomData.Put(0x37970 + i, (byte)head[i]);
                    RomData.Put(0x39970 + i, (byte)head[i]);
                }

                for (int i = 0; i < raft.Count(); i++)
                {
                    RomData.Put(0x31450 + i, (byte)raft[i]);
                }
            }
        }

        public void ShuffleE(int enemyPtr, int enemyAddr, List<int> enemies, List<int> generators, List<int> shorties, List<int> tallGuys, List<int> flyingEnemies, bool p7)
        {
            for (int i = enemyPtr; i < enemyPtr + 126; i += 2)
            {
                int low = RomData.GetByte(i);
                int high = RomData.GetByte(i + 1);
                high <<= 8;
                high &= 0x0FFF;
                _ = high + low + enemyAddr;
                ShuffleEnemies(high + low + enemyAddr, enemies, generators, shorties, tallGuys, flyingEnemies, p7);
            }
        }

        public void ShuffleEnemies(int addr, List<int> enemies, List<int> generators, List<int> shorties, List<int> tallGuys, List<int> flyingEnemies, bool p7)
        {
            if (!_visitedEnemies.Contains(addr))
            {
                int numBytes = RomData.GetByte(addr);
                for (int j = addr + 2; j < addr + numBytes; j += 2)
                {
                    int enemy = RomData.GetByte(j) & 0x3F;
                    int highPart = RomData.GetByte(j) & 0xC0;
                    if (Props.MixLargeAndSmallEnemies)
                    {
                        if (enemies.Contains(enemy))
                        {
                            int swap = enemies[R.Next(0, enemies.Count)];
                            int ypos = RomData.GetByte(j - 1) & 0xF0;
                            int xpos = RomData.GetByte(j - 1) & 0x0F;
                            if (shorties.Contains(enemy) && tallGuys.Contains(swap))
                            {
                                ypos -= 16;
                                while (swap == 0x1D && ypos != 0x70 && !p7)
                                {
                                    swap = tallGuys[R.Next(0, tallGuys.Count)];
                                }
                            }
                            else
                            {
                                while (swap == 0x1D && ypos != 0x70 && !p7)
                                {
                                    swap = enemies[R.Next(0, enemies.Count)];
                                }
                            }


                            RomData.Put(j - 1, (byte)(ypos + xpos));
                            RomData.Put(j, (byte)(swap + highPart));
                        }
                    }
                    else
                    {
                        if (tallGuys.Contains(enemy))
                        {
                            int swap = R.Next(0, tallGuys.Count);
                            int ypos = RomData.GetByte(j - 1) & 0xF0;
                            while (tallGuys[swap] == 0x1D && ypos != 0x70 && !p7)
                            {
                                swap = R.Next(0, tallGuys.Count);
                            }
                            RomData.Put(j, (byte)(tallGuys[swap] + highPart));
                        }

                        if (shorties.Contains(enemy))
                        {
                            int swap = R.Next(0, shorties.Count);
                            RomData.Put(j, (byte)(shorties[swap] + highPart));
                        }
                    }


                    if (flyingEnemies.Contains(enemy))
                    {
                        int swap = R.Next(0, flyingEnemies.Count);
                        while (enemy == 0x07 && (flyingEnemies[swap] == 0x06 || flyingEnemies[swap] == 0x0E))
                        {
                            swap = R.Next(0, flyingEnemies.Count);
                        }
                        RomData.Put(j, (byte)(flyingEnemies[swap] + highPart));
                    }

                    if (generators.Contains(enemy))
                    {
                        int swap = R.Next(0, generators.Count);
                        RomData.Put(j, (byte)(generators[swap] + highPart));
                    }

                    if (enemy == 0x0B)
                    {
                        int swap = R.Next(0, generators.Count + 1);
                        if (swap != generators.Count)
                        {
                            RomData.Put(j, (byte)(generators[swap] + highPart));
                        }
                    }
                }
                _visitedEnemies.Add(addr);
            }

        }
        public void ShuffleSmallItems(int world, bool first)
        {
            Console.WriteLine("World: " + world);
            List<int> addresses = new List<int>();
            List<int> items = new List<int>();
            int startAddr;
            if (first)
            {
                startAddr = 0x8523 - 0x8000 + (world * 0x4000) + 0x10;
            }
            else
            {
                startAddr = 0xA000 - 0x8000 + (world * 0x4000) + 0x10;
            }
            int map = 0;
            for (int i = startAddr; i < startAddr + 126; i += 2)
            {

                map++;
                int low = RomData.GetByte(i);
                int hi = RomData.GetByte(i + 1) * 256;
                int numBytes = RomData.GetByte(hi + low + 16 - 0x8000 + (world * 0x4000));
                for (int j = 4; j < numBytes; j += 2)
                {
                    int yPos = RomData.GetByte(hi + low + j + 16 - 0x8000 + (world * 0x4000)) & 0xF0;
                    yPos >>= 4;
                    if (RomData.GetByte(hi + low + j + 1 + 16 - 0x8000 + (world * 0x4000)) == 0x0F && yPos < 13)
                    {
                        int addr = hi + low + j + 2 + 16 - 0x8000 + (world * 0x4000);
                        int item = RomData.GetByte(addr);
                        if (item == 8 || (item > 9 && item < 14) || (item > 15 && item < 19) && !addresses.Contains(addr))
                        {
                            Console.WriteLine("Map: " + map);
                            Console.WriteLine("Item: " + item);
                            Console.WriteLine("Address: {0:X}", addr);
                            addresses.Add(addr);
                            items.Add(item);
                        }
                        j++;
                    }
                }
            }


            for (int i = 0; i < items.Count; i++)
            {
                int swap = R.Next(i, items.Count);
                int temp = items[swap];
                items[swap] = items[i];
                items[i] = temp;
            }
            for (int i = 0; i < addresses.Count; i++)
            {
                RomData.Put(addresses[i], (byte)items[i]);
            }
        }

        private char[] ToGameText(string s2)
        {
            s2 = s2.ToUpper();
            char[] s = s2.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= '0' && s[i] <= '9')
                    s[i] += (char)(0xd0 - '0');
                else if (s[i] >= 'A' && s[i] <= 'Z')
                    s[i] += (char)(0xda - 'A');
                else if (s[i] == '.')
                    s[i] = (char)0xcf;
                else if (s[i] == '/')
                    s[i] = (char)0xce;
                else if (s[i] == ',')
                    s[i] = (char)0x9c;
                else if (s[i] == '!')
                    s[i] = (char)0x36;
                else if (s[i] == '?')
                    s[i] = (char)0x34;
                else if (s[i] == '*')
                    s[i] = (char)0x32;
                else if (s[i] == ' ')
                    s[i] = (char)0xf4;
                else if (s[i] == '\n')
                    s[i] = (char)0xfd;
                else if (s[i] == '$')
                    s[i] = (char)0xfd;
            }

            return s;
        }
    }
}
