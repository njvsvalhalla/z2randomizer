using System;
using System.IO;

namespace Z2Randomizer
{
    public class RandomizerProperties
    {
        //ROM Info
        public string FileName { get; set; }
        public Stream InputFileStream { get; set; }

        public int Seed { get; set; }
        public string Flags { get; set; }

        //Items
        public bool ShuffleStartingItems { get; set; }
        public bool StartWithCandle { get; set; }
        public bool StartWithGlove { get; set; }
        public bool StartWithRaft { get; set; }
        public bool StartWithBoots { get; set; }
        public bool StartWithFlute { get; set; }
        public bool StartWithCross { get; set; }
        public bool StartWithHammer { get; set; }
        public bool StartWithKey { get; set; }

        //Spells
        public bool ShuffleStartingSpells { get; set; }
        public bool StartWithShield { get; set; }
        public bool StartWithJump { get; set; }
        public bool StartWithLife { get; set; }
        public bool StartWithFairy { get; set; }
        public bool StartWithFire { get; set; }
        public bool StartWithReflect { get; set; }
        public bool StartWithSpell { get; set; }
        public bool StartWithThunder { get; set; }
        public bool CombineFireWithRandomSpell { get; set; }

        //Other starting attributes
        public string StartingHeartContainers { get; set; }
        public string MaximumHeartContainers { get; set; }
        public string StartingTechs { get; set; }
        public bool RandomizeNumberOfLives { get; set; }
        public string HintType { get; set; }
        public bool PermanentBeamSword { get; set; }
        public bool CommunityHints { get; set; }

        //Overworld
        public bool ShuffleEncounters { get; set; }
        public bool AllowUnsafePathEncounters { get; set; }
        public bool AllowPalacesToSwapContinents { get; set; }
        public bool IncludeGreatPalaceInShuffle { get; set; }
        public string HiddenPalace { get; set; }
        public string HiddenKasuto { get; set; }

        //Palaces
        public bool ShufflePalaceRooms { get; set; }
        public string NumberOfPalacesToComplete { get; set; }
        public bool ThunderbirdRequired { get; set; }
        public bool ChangePalacePalettes { get; set; }
        public bool RestartAtPalacesIfGameOver { get; set; }
        public bool ShortenGreatPalace { get; set; }
        public bool RemoveThunderbird { get; set; }

        //Enemies
        public bool ShuffleEnemyHp { get; set; }
        public bool ShuffleWhichEnemiesStealExp { get; set; }
        public bool ShuffleAmountExpStolen { get; set; }
        public bool ShuffleSwordImmunity { get; set; }
        public bool ShuffleEnemyExp { get; set; }
        public bool ShuffleBossExp { get; set; }
        public bool ShuffleOverworldEnemies { get; set; }
        public bool ShufflePalaceEnemies { get; set; }
        public bool MixLargeAndSmallEnemies { get; set; }
        public bool ShuffleDripperEnemies { get; set; }
        public bool ShuffleSpritePallate { get; set; }

        //Levels
        public bool ShuffleAllExperienceNeeded { get; set; }
        public bool ShuffleAttackExperienceNeeded { get; set; }
        public bool ShuffleMagicExperienceNeeded { get; set; }
        public bool ShuffleLifeExperienceNeeded { get; set; }
        public bool RandomAttackEffectiveness { get; set; }
        public bool RandomMagicEffectiveness { get; set; }
        public bool RandomLifeEffectiveness { get; set; }
        public bool ShuffleLifeRefill { get; set; }
        public bool ShuffleSpellLocations { get; set; }
        public bool DisableMagicContainerRequirements { get; set; }
        public bool OhkoAttackEffectiveness { get; set; }
        public bool InvincibleLifeEffectiveness { get; set; }
        public bool OhkoLifeEffectiveness { get; set; }
        public bool FreeMagicEffectiveness { get; set; }
        public bool HighAttackEffectiveness { get; set; }
        public bool LowAttackEffectiveness { get; set; }
        public bool HighLifeEffectiveness { get; set; }
        public bool MagicEffectiveness { get; set; }
        public bool LowMagicEffectiveness { get; set; }

        //Items
        public bool ShuffleOverworldItems { get; set; }
        public bool ShufflePalaceItems { get; set; }
        public bool MixOverworldAndPalaceItems { get; set; }
        public bool ShuffleSmallItems { get; set; }
        public bool PalacesContainsExtraKeys { get; set; }
        public bool RandomizeNewKasutoJarRequirements { get; set; }
        public bool IncludePbagCavesInItemShuffle { get; set; }
        public bool RemoveSpellItems { get; set; }

        //Drops
        public bool ShuffleItemDropFrequency { get; set; }
        public bool ManuallySelectDrops { get; set; }
        public bool SmallEnemyBlueJar { get; set; }
        public bool SmallEnemyRedJar { get; set; }
        public bool SmallEnemyFiftyPbag { get; set; }
        public bool SmallEnemyOneHundredPbag { get; set; }
        public bool SmallEnemyTwoHundredPbag { get; set; }
        public bool SmallEnemyFiveHundredPbag { get; set; }
        public bool SmallEnemyOneUp { get; set; }
        public bool SmallEnemyKey { get; set; }
        public bool LargeEnemyBlueJar { get; set; }
        public bool LargeEnemyRedJar { get; set; }
        public bool LargeEnemyFiftyPbag { get; set; }
        public bool LargeEnemyOneHundredPbag { get; set; }
        public bool LargeEnemyTwoHundredPbag { get; set; }
        public bool LargeEnemyFiveHundredPbag { get; set; }
        public bool LargeEnemyOneUp { get; set; }
        public bool LargeEnemyKey { get; set; }
        public bool StandardizeDrops { get; set; }
        public bool RandomizeDrops { get; set; }

        //Misc.
        public bool DisableLowHealthBeep { get; set; }
        public bool JumpAlwaysOn { get; set; }
        public bool FastSpellCasting { get; set; }
        public string BeamSprite { get; set; }
        public bool DisableMusic { get; set; }
        public string CharacterSprite { get; set; }
        public string TunicColor { get; set; }
        public string ShieldTunicColor { get; set; }
    }
}
