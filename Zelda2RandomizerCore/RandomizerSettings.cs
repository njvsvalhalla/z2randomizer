using Newtonsoft.Json;
using RandomizerApp.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Z2Randomizer;
using static RandomizerApp.Enums;

namespace RandomizerApp
{
    public class RandomizerSettings
    {
        private const string flags = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz1234567890!@#$";
        public string FileName { get; set; }

        public string Flags { get => GenerateFlags(); }
        public int Seed { get; set; } = GenerateSeed();

        //start config

        //items

        //if this is checked don't allow starting items
        public bool ShuffleStartingItems { get; set; }
        public bool StartWithCandle { get; set; }
        public bool StartWithGlove { get; set; }
        public bool StartWithRaft { get; set; }
        public bool StartWithBoots { get; set; }
        public bool StartWithFlute { get; set; }
        public bool StartWithCross { get; set; }
        public bool StartWithHammer { get; set; }
        public bool StartWithMagicKey { get; set; }

        public bool ShuffleItemSprites { get; set; }
        public bool FunPercentSprites { get; set; }

        //spells
        //if this is checked don't allow spells
        public bool ShuffleStartingSpells { get; set; }
        public bool StartWithShield { get; set; }
        public bool StartWithJump { get; set; }
        public bool StartWithLife { get; set; }
        public bool StartWithFairy { get; set; }
        public bool StartWithFire { get; set; }
        public bool StartWithReflect { get; set; }
        public bool StartWithSpell { get; set; }
        public bool StartWithThunder { get; set; }

        public Containers StartingHeartContainers { get; set; } = Containers.Four;
        public Containers MaxHeartContainers { get; set; } = Containers.Eight;
        public StartingTechs StartingTechs { get; set; }
        public HintType HintType { get; set; } = HintType.Normal;
        public bool CommunityHints { get; set; } = true;
        public bool RandomizeNumberOfLives { get; set; }

        //Overworld
        public bool AllowPalacesToSwapContinents { get; set; }
        //only enabled if above is true
        public bool IncludeGreatPalaceInShuffle { get; set; }

        public bool ShuffleEncounters { get; set; }
        //only enabled if above is true
        public bool AllowUnsafePathEncounters { get; set; }

        public HiddenOptions HiddenPalace { get; set; } = HiddenOptions.Off;
        public HiddenOptions HiddenKasuto { get; set; } = HiddenOptions.Off;

        //Palaces
        public bool ShufflePalaceRooms { get; set; }
        //the following three only editable if above is true
        public bool ShortenGreatPalace { get; set; }
        //only one or the other
        public bool ThunderbirdRequired { get; set; } = true;
        public bool RemoveThunderbird { get; set; }
        public bool ChangePalacePalettes { get; set; }

        public Palaces NumberOfPalacesToComplete { get; set; } = Palaces.Six;

        public bool RestartAtPalacesIfGameOver { get; set; }

        //Levels and Spells
        public bool ShuffleAllExperienceNeeded { get; set; }
        //if true, all are automatically true
        public bool ShuffleAttackExperienceNeeded { get; set; }
        public bool ShuffleMagicExperienceNeeded { get; set; }
        public bool ShuffleLifeExperienceNeeded { get; set; }

        public bool ShuffleLifeRefill { get; set; }
        public bool ShuffleSpellLocations { get; set; }
        public bool DisableMagicContainerRequirements { get; set; }
        public bool CombineFireWithRandomSpell { get; set; }

        //one of these only
        public AttackEffectiveness AttackEffectiveness { get; set; } = AttackEffectiveness.Normal;
        public MagicEffectiveness MagicEffectiveness { get; set; } = MagicEffectiveness.Normal;
        public LifeEffectiveness LifeEffectiveness { get; set; } = LifeEffectiveness.Normal;

        //Enemies
        public bool ShuffleOverworldEnemies { get; set; }
        public bool ShufflePalaceEnemies { get; set; }
        //if either true, allow this
        public bool MixLargeAndSmallEnemies { get; set; }
        public bool ShuffleDripperEnemies { get; set; }

        public bool ShuffleEnemyHP { get; set; }
        public bool ShuffleEnemyExp { get; set; }
        public bool ShuffleBossExp { get; set; }
        public bool ShuffleWhichEnemiesStealExp { get; set; }
        public bool ShuffleAmountExpStolen { get; set; }
        public bool ShuffleSwordImmunity { get; set; }

        //Items

        public bool ShufflePalaceItems { get; set; }
        public bool ShuffleOverworldItems { get; set; }
        //if both are true enable this
        public bool MixOverworldAndPalaceItems { get; set; }
        //only enable if shuffle overworld
        public bool IncludePbagCavesInItemShuffle { get; set; }
        public bool ShuffleSmallItems { get; set; }
        public bool PalacesContainsExtraKeys { get; set; }
        public bool RandomizeNewKasutoJarRequirements { get; set; }
        public bool RemoveSpellItems { get; set; }

        //Drops
        public bool ShuffleItemDropFrequency { get; set; }
        public bool ManuallySelectDrops { get; set; }
        public List<ItemPool> SmallEnemyPool { get; set; } = new List<ItemPool> { ItemPool.BlueJar, ItemPool.FiftyPbag };
        public List<ItemPool> LargeEnemyPool { get; set; } = new List<ItemPool> { ItemPool.RedJar, ItemPool.TwoHundredPbag };
        //only available if above is false
        public bool RandomizeDrops { get; set; }
        public bool StandardizeDrops { get; set; }

        //Misc
        public bool DisableLowHealthBeep { get; set; }
        public bool DisableMusic { get; set; }
        public bool JumpAlwaysOn { get; set; }
        public bool FastSpellCasting { get; set; }
        public bool ShuffleSpritePallate { get; set; }
        public bool PermanentBeamSword { get; set; }
        public CharacterSprite CharacterSprite { get; set; } = CharacterSprite.Link;
        public TunicColor TunicColor { get; set; } = TunicColor.Default;
        public TunicColor ShieldTunicColor { get; set; } = TunicColor.Orange;
        public BeamSprite BeamSprite { get; set; } = BeamSprite.Default;

        public string ToJson() => JsonConvert.SerializeObject(this);
        public static int GenerateSeed() => (new Random()).Next(1000000000);

        public RandomizerProperties GetRandomizerProperties(string fileName, Stream stream = null)
        {
            var properties = new RandomizerProperties
            {
                InputFileStream = stream,
                AllowUnsafePathEncounters = AllowUnsafePathEncounters,
                BeamSprite = BeamSprite.GetStringValue(),
                CharacterSprite = CharacterSprite.GetStringValue(),
                CombineFireWithRandomSpell = CombineFireWithRandomSpell,
                CommunityHints = CommunityHints,
                DisableLowHealthBeep = DisableLowHealthBeep,
                DisableMagicContainerRequirements = DisableMagicContainerRequirements,
                DisableMusic = DisableMusic,
                PalacesContainsExtraKeys = PalacesContainsExtraKeys,
                FastSpellCasting = FastSpellCasting,
                FileName = fileName,
                Flags = Flags,
                HiddenKasuto = HiddenKasuto.GetStringValue(),
                HiddenPalace = HiddenPalace.GetStringValue(),
                HighAttackEffectiveness = (AttackEffectiveness == AttackEffectiveness.High),
                HighLifeEffectiveness = (LifeEffectiveness == LifeEffectiveness.High),
                MagicEffectiveness = (MagicEffectiveness == MagicEffectiveness.High),
                HintType = HintType.GetStringValue(),
                JumpAlwaysOn = JumpAlwaysOn,
                RandomizeNewKasutoJarRequirements = RandomizeNewKasutoJarRequirements,
                LowAttackEffectiveness = (AttackEffectiveness == AttackEffectiveness.Low),
                LowMagicEffectiveness = (MagicEffectiveness == MagicEffectiveness.Low),
                StandardizeDrops = StandardizeDrops,
                StartWithBoots = StartWithBoots,
                MaximumHeartContainers = MaxHeartContainers.GetStringValue(),
                MixLargeAndSmallEnemies = MixLargeAndSmallEnemies,
                MixOverworldAndPalaceItems = MixOverworldAndPalaceItems,
                OhkoAttackEffectiveness = (AttackEffectiveness == AttackEffectiveness.OHKO),
                OhkoLifeEffectiveness = (LifeEffectiveness == LifeEffectiveness.OHKO),
                IncludeGreatPalaceInShuffle = IncludeGreatPalaceInShuffle,
                ChangePalacePalettes = ChangePalacePalettes,
                ShuffleItemDropFrequency = ShuffleItemDropFrequency,
                PermanentBeamSword = PermanentBeamSword,
                IncludePbagCavesInItemShuffle = IncludePbagCavesInItemShuffle,
                RandomizeDrops = RandomizeDrops,
                RemoveSpellItems = RemoveSpellItems,
                RemoveThunderbird = RemoveThunderbird,
                ThunderbirdRequired = ThunderbirdRequired,
                Seed = Seed,
                ShieldTunicColor = ShieldTunicColor.GetStringValue(),
                ShortenGreatPalace = ShortenGreatPalace,
                ShuffleAllExperienceNeeded = ShuffleAllExperienceNeeded,
                RandomAttackEffectiveness = (AttackEffectiveness == AttackEffectiveness.Random),
                ShuffleAttackExperienceNeeded = ShuffleAllExperienceNeeded || ShuffleAttackExperienceNeeded,
                ShuffleBossExp = ShuffleBossExp,
                ShuffleDripperEnemies = ShuffleDripperEnemies,
                ShuffleEncounters = ShuffleEncounters,
                ManuallySelectDrops = ManuallySelectDrops,
                ShuffleEnemyExp = ShuffleEnemyExp,
                ShuffleEnemyHp = ShuffleEnemyHP,
                ShuffleSpritePallate = ShuffleSpritePallate,
                ShuffleWhichEnemiesStealExp = ShuffleWhichEnemiesStealExp,
                ShuffleStartingItems = ShuffleStartingItems,
                RandomLifeEffectiveness = (LifeEffectiveness == LifeEffectiveness.Random),
                ShuffleLifeExperienceNeeded = ShuffleAllExperienceNeeded || ShuffleLifeExperienceNeeded,
                ShuffleLifeRefill = ShuffleLifeRefill,
                RandomizeNumberOfLives = RandomizeNumberOfLives,
                RandomMagicEffectiveness = (MagicEffectiveness == MagicEffectiveness.Random),
                ShuffleMagicExperienceNeeded = ShuffleAllExperienceNeeded || ShuffleMagicExperienceNeeded,
                ShuffleOverworldEnemies = ShuffleOverworldEnemies,
                ShuffleOverworldItems = ShuffleOverworldItems,
                ShufflePalaceEnemies = ShufflePalaceEnemies,
                ShufflePalaceItems = ShufflePalaceItems,
                ShufflePalaceRooms = ShufflePalaceRooms,
                ShuffleSmallItems = ShuffleSmallItems,
                ShuffleSpellLocations = ShuffleSpellLocations,
                ShuffleStartingSpells = ShuffleStartingSpells,
                ShuffleAmountExpStolen = ShuffleAmountExpStolen,
                ShuffleSwordImmunity = ShuffleSwordImmunity,
                StartWithCandle = StartWithCandle,
                StartWithCross = StartWithCross,
                StartWithFairy = StartWithFairy,
                StartWithFire = StartWithFire,
                StartWithFlute = StartWithFlute,
                NumberOfPalacesToComplete = NumberOfPalacesToComplete.GetStringValue(),
                StartWithGlove = StartWithGlove,
                StartWithHammer = StartWithHammer,
                StartingHeartContainers = StartingHeartContainers.GetStringValue(),
                StartWithJump = StartWithJump,
                StartWithKey = StartWithMagicKey,
                StartWithLife = StartWithLife,
                StartWithRaft = StartWithRaft,
                StartWithReflect = StartWithReflect,
                StartWithShield = StartWithShield,
                StartWithSpell = StartWithSpell,
                StartingTechs = StartingTechs.GetStringValue(),
                StartWithThunder = StartWithThunder,
                AllowPalacesToSwapContinents = AllowPalacesToSwapContinents,
                InvincibleLifeEffectiveness = (LifeEffectiveness == LifeEffectiveness.Invincible),
                RestartAtPalacesIfGameOver = RestartAtPalacesIfGameOver,
                FreeMagicEffectiveness = (MagicEffectiveness == MagicEffectiveness.Free),
                TunicColor = TunicColor.GetStringValue(),
                ShuffleItemSprites = ShuffleItemSprites,
                FunPercentSprites = FunPercentSprites
            };
            ItemPoolPopulate(LargeEnemyPool, EnemySize.Large, ref properties);
            ItemPoolPopulate(SmallEnemyPool, EnemySize.Small, ref properties);
            return properties;
        }

        public void Validate()
        {
            if ((int)StartingHeartContainers != 8 && (int)StartingHeartContainers > (int)MaxHeartContainers && (int)MaxHeartContainers != 8)
                throw new Exception("ax hearts must be greater than or equal to starting hearts");

            if (ShuffleStartingItems && (StartWithCandle || StartWithGlove || StartWithRaft || StartWithBoots || StartWithFlute || StartWithCross || StartWithHammer || StartWithMagicKey))
                throw new Exception
                    ("You can't have any starting items if you select shuffle starting items enabled.");

            if (ShuffleStartingSpells && (StartWithShield || StartWithJump || StartWithLife || StartWithFairy || StartWithFire || StartWithReflect || StartWithSpell || StartWithThunder))
                throw new Exception("You can't have any starting spells if you have shuffle starting spell enabled.");

            if (!AllowPalacesToSwapContinents && IncludeGreatPalaceInShuffle)
                throw new Exception("You can't have the great palace shuffled if you don't have palaces swap continents on!");

            if (!ShuffleEncounters && AllowUnsafePathEncounters)
                throw new Exception("You can't allow unsafe path encounters without shuffle encounters enabled!");

            if (!ShufflePalaceRooms && !ThunderbirdRequired)
                throw new Exception("To have thunderbird requirements turned off, you must shuffle palace rooms");

            if (!ShufflePalaceRooms && (ShortenGreatPalace || RemoveThunderbird))
                throw new Exception("To shorten the great palace or remove thunderbird you must shuffle palace rooms");

            if (MixOverworldAndPalaceItems && !(ShufflePalaceItems && ShuffleOverworldItems))
                throw new Exception("You need to set MixOverworldAndPalaceItems to true to shuffle both palace and overworld items.");

            if (IncludePbagCavesInItemShuffle && !ShuffleOverworldItems)
                throw new Exception("You need to enable ShuffleOverworldItems to include pbag caves in item shuffle.");

            if (!ManuallySelectDrops && (SmallEnemyPool.Any() || LargeEnemyPool.Any()))
                throw new Exception("To add anything to enemy pools you need to enable manually select drops");

            if (ManuallySelectDrops && !(SmallEnemyPool.Any() || LargeEnemyPool.Any()))
                throw new Exception("You must have at least one item in an enemy drop pool");

            if (ManuallySelectDrops && RandomizeDrops)
                throw new Exception("You can't manually select drops and randomize drops");
        }

        private string GenerateFlags()
        {
            var flagStr = "";
            var v = new BitArray(6);
            var array = new int[1];

            v[0] = ShuffleStartingItems;
            v[1] = StartWithCandle;
            v[2] = StartWithGlove;
            v[3] = StartWithRaft;
            v[4] = StartWithBoots;
            v[5] = ShuffleOverworldEnemies;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = StartWithFlute;
            v[1] = StartWithCross;
            v[2] = StartWithHammer;
            v[3] = StartWithMagicKey;
            v[4] = ShuffleStartingSpells;
            v[5] = ShuffleEnemyExp;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = StartWithShield;
            v[1] = StartWithJump;
            v[2] = StartWithLife;
            v[3] = StartWithFairy;
            v[4] = StartWithFire;
            v[5] = CombineFireWithRandomSpell;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = StartWithReflect;
            v[1] = StartWithSpell;
            v[2] = StartWithThunder;
            v[3] = RandomizeNumberOfLives;
            v[4] = RemoveThunderbird;
            v[5] = ShuffleBossExp;

            var w = new BitArray(new int[] { (int)StartingHeartContainers, (int)StartingTechs });
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];
            v[0] = w[0];
            v[1] = w[1];
            v[2] = w[2];
            v[3] = w[32];
            v[4] = w[33];
            v[5] = w[34];
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = ShuffleItemDropFrequency;
            v[1] = IncludePbagCavesInItemShuffle;
            v[2] = w[3];
            v[3] = IncludeGreatPalaceInShuffle;
            v[4] = ChangePalacePalettes;
            v[5] = ShuffleEncounters;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)AttackEffectiveness });
            v[0] = PalacesContainsExtraKeys;
            v[1] = AllowPalacesToSwapContinents;
            v[2] = w[0];
            v[3] = w[1];
            v[4] = w[2];
            v[5] = AllowUnsafePathEncounters;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = PermanentBeamSword;
            v[1] = ShuffleDripperEnemies;
            v[2] = ShufflePalaceRooms;
            v[3] = ShuffleEnemyHP;
            v[4] = ShuffleAllExperienceNeeded;
            v[5] = ShufflePalaceEnemies;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = ShuffleAttackExperienceNeeded;
            v[1] = ShuffleLifeExperienceNeeded;
            v[2] = ShuffleMagicExperienceNeeded;
            v[3] = RestartAtPalacesIfGameOver;
            v[4] = ShortenGreatPalace;
            v[5] = ThunderbirdRequired;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)MagicEffectiveness });
            v[0] = w[0];
            v[1] = w[1];
            v[2] = w[2];
            v[3] = ShuffleWhichEnemiesStealExp;
            v[4] = ShuffleAmountExpStolen;
            v[5] = ShuffleLifeRefill;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)NumberOfPalacesToComplete });
            v[0] = ShuffleSwordImmunity;
            v[1] = JumpAlwaysOn;
            v[2] = w[0];
            v[3] = w[1];
            v[4] = w[2];
            v[5] = MixLargeAndSmallEnemies;

            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = ShufflePalaceItems;
            v[1] = ShuffleOverworldItems;
            v[2] = MixOverworldAndPalaceItems;
            v[3] = ShuffleSmallItems;
            v[4] = ShuffleSpellLocations;
            v[5] = DisableMagicContainerRequirements;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)LifeEffectiveness });
            v[0] = w[0];
            v[1] = w[1];
            v[2] = w[2];
            v[3] = RandomizeNewKasutoJarRequirements;
            v[4] = CommunityHints;
            v[5] = ShuffleSpritePallate;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)MaxHeartContainers });
            v[0] = w[0];
            v[1] = w[1];
            v[2] = w[2];
            v[3] = w[3];
            w = new BitArray(new int[] { (int)HiddenPalace });
            v[4] = w[0];
            v[5] = w[1];
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)HiddenKasuto });
            v[0] = w[0];
            v[1] = w[1];
            v[2] = ManuallySelectDrops;
            v[3] = RemoveSpellItems;
            v[4] = SmallEnemyPool.Any(x => x == ItemPool.BlueJar);
            v[5] = SmallEnemyPool.Any(x => x == ItemPool.RedJar);
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = SmallEnemyPool.Any(x => x == ItemPool.FiftyPbag);
            v[1] = SmallEnemyPool.Any(x => x == ItemPool.HundredPbag);
            v[2] = SmallEnemyPool.Any(x => x == ItemPool.TwoHundredPbag);
            v[3] = SmallEnemyPool.Any(x => x == ItemPool.FiveHundredPbag);
            v[4] = SmallEnemyPool.Any(x => x == ItemPool.OneUp);
            v[5] = SmallEnemyPool.Any(x => x == ItemPool.Key);
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v[0] = LargeEnemyPool.Any(x => x == ItemPool.BlueJar);
            v[1] = LargeEnemyPool.Any(x => x == ItemPool.RedJar);
            v[2] = LargeEnemyPool.Any(x => x == ItemPool.FiftyPbag);
            v[3] = LargeEnemyPool.Any(x => x == ItemPool.HundredPbag);
            v[4] = LargeEnemyPool.Any(x => x == ItemPool.TwoHundredPbag);
            v[5] = LargeEnemyPool.Any(x => x == ItemPool.FiveHundredPbag);
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            w = new BitArray(new int[] { (int)HintType });
            v[0] = LargeEnemyPool.Any(x => x == ItemPool.OneUp);
            v[1] = LargeEnemyPool.Any(x => x == ItemPool.Key);
            v[2] = w[0];
            v[3] = w[1];
            v[4] = StandardizeDrops;
            v[5] = RandomizeDrops;
            v.CopyTo(array, 0);
            flagStr = flagStr + flags[array[0]];

            v = new BitArray(2);
            v[0] = ShuffleItemSprites;
            v[1] = FunPercentSprites;
            v.CopyTo(array,0);
            flagStr = flagStr + flags[array[0]];

            return flagStr;
        }

        public void GenerateFromFlags(string inputFlags)
        {
            var v = new BitArray(new int[] { flags.IndexOf(inputFlags[0]) });
            var array = new int[1];
            var s = v[0];

            ShuffleStartingItems = v[0];
            StartWithCandle = v[1];
            StartWithGlove = v[2];
            StartWithRaft = v[3];
            StartWithBoots = v[4];
            ShuffleOverworldEnemies = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[1]) });
            StartWithFlute = v[0];
            StartWithCross = v[1];
            StartWithHammer = v[2];
            StartWithMagicKey = v[3];
            ShuffleStartingSpells = v[4];
            ShuffleEnemyExp = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[2]) });
            StartWithShield = v[0];
            StartWithJump = v[1];
            StartWithLife = v[2];
            StartWithFairy = v[3];
            StartWithFire = v[4];
            CombineFireWithRandomSpell = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[3]) });
            StartWithReflect = v[0];
            StartWithSpell = v[1];
            StartWithThunder = v[2];
            RandomizeNumberOfLives = v[3];
            RemoveThunderbird = v[4];
            ShuffleBossExp = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[4]) });
            BitArray w = new BitArray(3);
            w[0] = v[3];
            w[1] = v[4];
            w[2] = v[5];
            w.CopyTo(array, 0);
            StartingTechs = (StartingTechs)array[0];

            w = new BitArray(4);
            w[0] = v[0];
            w[1] = v[1];
            w[2] = v[2];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[5]) });
            w[3] = v[2];
            w.CopyTo(array, 0);
            StartingHeartContainers = (Containers)array[0];
            ShuffleItemDropFrequency = v[0];
            IncludePbagCavesInItemShuffle = v[1];
            IncludeGreatPalaceInShuffle = v[3];
            ChangePalacePalettes = v[4];
            ShuffleEncounters = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[6]) });
            PalacesContainsExtraKeys = v[0];
            AllowPalacesToSwapContinents = v[1];
            w = new BitArray(3);
            w[0] = v[2];
            w[1] = v[3];
            w[2] = v[4];
            w.CopyTo(array, 0);
            AttackEffectiveness = (AttackEffectiveness)array[0];
            AllowUnsafePathEncounters = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[7]) });
            PermanentBeamSword = v[0];
            ShuffleDripperEnemies = v[1];
            ShufflePalaceRooms = v[2];
            ShuffleEnemyHP = v[3];
            ShuffleAllExperienceNeeded = v[4];
            ShufflePalaceEnemies = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[8]) });
            ShuffleAttackExperienceNeeded = v[0];
            ShuffleLifeExperienceNeeded = v[1];
            ShuffleMagicExperienceNeeded = v[2];
            RestartAtPalacesIfGameOver = v[3];
            ShortenGreatPalace = v[4];
            ThunderbirdRequired = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[9]) });
            w[0] = v[0];
            w[1] = v[1];
            w[2] = v[2];
            w.CopyTo(array, 0);
            MagicEffectiveness = (MagicEffectiveness)array[0];
            ShuffleWhichEnemiesStealExp = v[3];
            ShuffleAmountExpStolen = v[4];
            ShuffleLifeRefill = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[10]) });
            ShuffleSwordImmunity = v[0];
            JumpAlwaysOn = v[1];
            w[0] = v[2];
            w[1] = v[3];
            w[2] = v[4];
            w.CopyTo(array, 0);
            NumberOfPalacesToComplete = (Palaces)array[0];
            MixLargeAndSmallEnemies = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[11]) });
            ShufflePalaceItems = v[0];
            ShuffleOverworldItems = v[1];
            MixOverworldAndPalaceItems = v[2];
            ShuffleSmallItems = v[3];
            ShuffleSpellLocations = v[4];
            DisableMagicContainerRequirements = v[5];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[12]) });
            w[0] = v[0];
            w[1] = v[1];
            w[2] = v[2];
            w.CopyTo(array, 0);
            LifeEffectiveness = (LifeEffectiveness)array[0];
            RandomizeNewKasutoJarRequirements = v[3];
            CommunityHints = v[4];
            ShuffleSpritePallate = v[5];

            w = new BitArray(4);
            v = new BitArray(new int[] { flags.IndexOf(inputFlags[13]) });
            w[0] = v[0];
            w[1] = v[1];
            w[2] = v[2];
            w[3] = v[3];
            w.CopyTo(array, 0);
            MaxHeartContainers = (Containers)array[0];
            w = new BitArray(2);
            w[0] = v[4];
            w[1] = v[5];
            w.CopyTo(array, 0);
            HiddenPalace = (HiddenOptions)array[0];

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[14]) });
            w = new BitArray(2);
            w[0] = v[0];
            w[1] = v[1];
            w.CopyTo(array, 0);
            HiddenKasuto = (HiddenOptions)array[0];
            ManuallySelectDrops = v[2];
            RemoveSpellItems = v[3];

            SmallEnemyPool = new List<ItemPool>();
            LargeEnemyPool = new List<ItemPool>();

            if (v[4])
                SmallEnemyPool.Add(ItemPool.BlueJar);
            if (v[5])
                SmallEnemyPool.Add(ItemPool.RedJar);

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[15]) });
            if (v[0])
                SmallEnemyPool.Add(ItemPool.FiftyPbag);
            if (v[1])
                SmallEnemyPool.Add(ItemPool.HundredPbag);
            if (v[2])
                SmallEnemyPool.Add(ItemPool.TwoHundredPbag);
            if (v[3])
                SmallEnemyPool.Add(ItemPool.FiveHundredPbag);
            if (v[4])
                SmallEnemyPool.Add(ItemPool.OneUp);
            if (v[5])
                SmallEnemyPool.Add(ItemPool.Key);

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[16]) });
            if (v[0])
                LargeEnemyPool.Add(ItemPool.BlueJar);
            if (v[1])
                LargeEnemyPool.Add(ItemPool.RedJar);
            if (v[2])
                LargeEnemyPool.Add(ItemPool.FiftyPbag);
            if (v[3])
                LargeEnemyPool.Add(ItemPool.HundredPbag);
            if (v[4])
                LargeEnemyPool.Add(ItemPool.TwoHundredPbag);
            if (v[5])
                LargeEnemyPool.Add(ItemPool.FiveHundredPbag);

            v = new BitArray(new int[] { flags.IndexOf(inputFlags[17]) });
            if (v[0])
                LargeEnemyPool.Add(ItemPool.OneUp);
            if (v[1])
                LargeEnemyPool.Add(ItemPool.Key);
            w[0] = v[2];
            w[1] = v[3];
            StandardizeDrops = v[4];
            RandomizeDrops = v[5];
            w.CopyTo(array, 0);
            HintType = (HintType)array[0];

            if (GenerateFlags() != inputFlags)
                throw new Exception("Did not calculate flags correctly :(");
        }


        private void ItemPoolPopulate(IEnumerable<ItemPool> pool, EnemySize enemySize, ref RandomizerProperties settings)
        {
            foreach (var item in pool)
            {
                switch (item)
                {
                    case ItemPool.BlueJar:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyBlueJar = true;
                        else
                            settings.SmallEnemyBlueJar = true;
                        break;
                    case ItemPool.RedJar:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyRedJar = true;
                        else
                            settings.SmallEnemyRedJar = true;
                        break;
                    case ItemPool.FiftyPbag:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyFiftyPbag = true;
                        else
                            settings.SmallEnemyFiftyPbag = true;
                        break;
                    case ItemPool.HundredPbag:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyOneHundredPbag = true;
                        else
                            settings.SmallEnemyOneHundredPbag = true;
                        break;
                    case ItemPool.TwoHundredPbag:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyTwoHundredPbag = true;
                        else
                            settings.SmallEnemyTwoHundredPbag = true;
                        break;
                    case ItemPool.FiveHundredPbag:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyFiveHundredPbag = true;
                        else
                            settings.SmallEnemyFiveHundredPbag = true;
                        break;
                    case ItemPool.OneUp:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyOneUp = true;
                        else
                            settings.SmallEnemyOneUp = true;
                        break;
                    case ItemPool.Key:
                        if (enemySize == EnemySize.Large)
                            settings.LargeEnemyKey = true;
                        else
                            settings.SmallEnemyKey = true;
                        break;
                }
            }
        }
    }
}