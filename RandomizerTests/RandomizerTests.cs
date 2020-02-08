using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomizerApp.Helpers;
using static RandomizerApp.Enums;

namespace RandomizerTests
{
    [TestClass]
    public class RandomizerTests
    {
        [TestMethod]
        public void TestFlags()
        {
            var settings = RandomizerSettingsFactory.Beginner();
            Assert.AreEqual("jhmhMROm7DZ$cHRBTA", settings.Flags);

            settings = RandomizerSettingsFactory.Swiss();
            Assert.AreEqual("jhhhDcM#$Za$bHRBTA", settings.Flags);

            settings = RandomizerSettingsFactory.Finals();
            Assert.AreEqual("hhAhC0j#x78gZGRBTA", settings.Flags);

            settings = RandomizerSettingsFactory.Max();
            Assert.AreEqual("iyhqh$j#g7@$JJRBTA", settings.Flags);
        }

        [TestMethod]
        public void TestRandomizerProperties()
        {
            var settings = RandomizerSettingsFactory.Beginner();
            var properties = settings.GetRandomizerProperties("test");

            Assert.AreEqual(properties.AllowUnsafePathEncounters, false);
            Assert.AreEqual(properties.BeamSprite, "Default");
            Assert.AreEqual(properties.CharacterSprite, "Link");
            Assert.AreEqual(properties.CombineFireWithRandomSpell, true);
            Assert.AreEqual(properties.CommunityHints, true);
            Assert.AreEqual(properties.DisableLowHealthBeep, true);
            Assert.AreEqual(properties.DisableMagicContainerRequirements, true);
            Assert.AreEqual(properties.DisableMusic, false);
            Assert.AreEqual(properties.PalacesContainsExtraKeys, true);
            Assert.AreEqual(properties.FastSpellCasting, false);
            Assert.AreEqual(properties.FileName, "test");
            Assert.AreEqual(properties.Flags, "jhmhMROm7DZ$cHRBTA");
            Assert.AreEqual(properties.HiddenKasuto, "Off");
            Assert.AreEqual(properties.HiddenPalace, "Off");
            Assert.AreEqual(properties.HighAttackEffectiveness, true);
            Assert.AreEqual(properties.HighLifeEffectiveness, true);
            Assert.AreEqual(properties.MagicEffectiveness, false);
            Assert.AreEqual(properties.HintType, "Normal");
            Assert.AreEqual(properties.JumpAlwaysOn, false);
            Assert.AreEqual(properties.RandomizeNewKasutoJarRequirements, true);
            Assert.AreEqual(properties.LargeEnemyOneHundredPbag, false);
            Assert.AreEqual(properties.LargeEnemyOneUp, false);
            Assert.AreEqual(properties.LargeEnemyTwoHundredPbag, true);
            Assert.AreEqual(properties.LargeEnemyFiftyPbag, false);
            Assert.AreEqual(properties.LargeEnemyFiveHundredPbag, false);
            Assert.AreEqual(properties.LargeEnemyBlueJar, false);
            Assert.AreEqual(properties.LargeEnemyKey, false);
            Assert.AreEqual(properties.LargeEnemyRedJar, true);
            Assert.AreEqual(properties.LowAttackEffectiveness, false);
            Assert.AreEqual(properties.LowMagicEffectiveness, true);
            Assert.AreEqual(properties.MaximumHeartContainers, "8");
            Assert.AreEqual(properties.MixLargeAndSmallEnemies, false);
            Assert.AreEqual(properties.MixOverworldAndPalaceItems, true);
            Assert.AreEqual(properties.OhkoAttackEffectiveness, false);
            Assert.AreEqual(properties.OhkoLifeEffectiveness, false);
            Assert.AreEqual(properties.IncludeGreatPalaceInShuffle, false);
            Assert.AreEqual(properties.ChangePalacePalettes, true);
            Assert.AreEqual(properties.ShuffleItemDropFrequency, false);
            Assert.AreEqual(properties.IncludePbagCavesInItemShuffle, false);
            Assert.AreEqual(properties.PermanentBeamSword, false);
            Assert.AreEqual(properties.RandomizeDrops, false);
            Assert.AreEqual(properties.RemoveSpellItems, false);
            Assert.AreEqual(properties.RemoveThunderbird, false);
            Assert.AreEqual(properties.ThunderbirdRequired, true);
            Assert.AreEqual(properties.Seed, 229721871);
            Assert.AreEqual(properties.ShieldTunicColor, "Orange");
            Assert.AreEqual(properties.ShortenGreatPalace, true);
            Assert.AreEqual(properties.ShuffleAllExperienceNeeded, false);
            Assert.AreEqual(properties.RandomAttackEffectiveness, false);
            Assert.AreEqual(properties.ShuffleAttackExperienceNeeded, false);
            Assert.AreEqual(properties.ShuffleBossExp, true);
            Assert.AreEqual(properties.ShuffleDripperEnemies, false);
            Assert.AreEqual(properties.ShuffleEncounters, false);
            Assert.AreEqual(properties.ManuallySelectDrops, false);
            Assert.AreEqual(properties.ShuffleEnemyExp, true);
            Assert.AreEqual(properties.ShuffleEnemyHp, false);
            Assert.AreEqual(properties.ShuffleSpritePallate, false);
            Assert.AreEqual(properties.ShuffleWhichEnemiesStealExp, false);
            Assert.AreEqual(properties.ShuffleStartingItems, false);
            Assert.AreEqual(properties.RandomLifeEffectiveness, false);
            Assert.AreEqual(properties.ShuffleLifeExperienceNeeded, false);
            Assert.AreEqual(properties.ShuffleLifeRefill, false);
            Assert.AreEqual(properties.RandomizeNumberOfLives, false);
            Assert.AreEqual(properties.RandomMagicEffectiveness, false);
            Assert.AreEqual(properties.ShuffleMagicExperienceNeeded, false);
            Assert.AreEqual(properties.ShuffleOverworldEnemies, true);
            Assert.AreEqual(properties.ShuffleOverworldItems, true);
            Assert.AreEqual(properties.ShufflePalaceEnemies, true);
            Assert.AreEqual(properties.ShufflePalaceItems, true);
            Assert.AreEqual(properties.ShufflePalaceRooms, true);
            Assert.AreEqual(properties.ShuffleSmallItems, true);
            Assert.AreEqual(properties.ShuffleSpellLocations, true);
            Assert.AreEqual(properties.ShuffleStartingSpells, false);
            Assert.AreEqual(properties.ShuffleAmountExpStolen, false);
            Assert.AreEqual(properties.ShuffleSwordImmunity, false);
            Assert.AreEqual(properties.SmallEnemyOneHundredPbag, false);
            Assert.AreEqual(properties.SmallEnemyOneUp, false);
            Assert.AreEqual(properties.SmallEnemyTwoHundredPbag, false);
            Assert.AreEqual(properties.SmallEnemyFiftyPbag, true);
            Assert.AreEqual(properties.SmallEnemyFiveHundredPbag, false);
            Assert.AreEqual(properties.SmallEnemyBlueJar, true);
            Assert.AreEqual(properties.SmallEnemyKey, false);
            Assert.AreEqual(properties.SmallEnemyRedJar, false);
            Assert.AreEqual(properties.StandardizeDrops, false);
            Assert.AreEqual(properties.StartWithBoots, false);
            Assert.AreEqual(properties.StartWithCandle, true);
            Assert.AreEqual(properties.StartWithCross, false);
            Assert.AreEqual(properties.StartWithFairy, false);
            Assert.AreEqual(properties.StartWithFire, false);
            Assert.AreEqual(properties.StartWithFlute, false);
            Assert.AreEqual(properties.NumberOfPalacesToComplete, "6");
            Assert.AreEqual(properties.StartWithGlove, false);
            Assert.AreEqual(properties.StartWithHammer, false);
            Assert.AreEqual(properties.StartingHeartContainers, "4");
            Assert.AreEqual(properties.StartWithJump, false);
            Assert.AreEqual(properties.StartWithKey, false);
            Assert.AreEqual(properties.StartWithLife, true);
            Assert.AreEqual(properties.StartWithRaft, false);
            Assert.AreEqual(properties.StartWithReflect, false);
            Assert.AreEqual(properties.StartWithShield, false);
            Assert.AreEqual(properties.StartWithSpell, false);
            Assert.AreEqual(properties.StartingTechs, "Downstab");
            Assert.AreEqual(properties.StartWithThunder, false);
            Assert.AreEqual(properties.AllowPalacesToSwapContinents, false);
            Assert.AreEqual(properties.InvincibleLifeEffectiveness, false);
            Assert.AreEqual(properties.TunicColor, "Default");
            Assert.AreEqual(properties.RestartAtPalacesIfGameOver, true);
            Assert.AreEqual(properties.FreeMagicEffectiveness, false);
        }

        [TestMethod]
        public void TestValidation()
        {

        }
    }
}
