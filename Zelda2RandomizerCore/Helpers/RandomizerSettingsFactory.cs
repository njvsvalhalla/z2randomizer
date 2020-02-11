using System;
using System.Collections.Generic;
using System.Text;
using static RandomizerApp.Enums;
using RandomizerApp.Helpers;

namespace RandomizerApp.Helpers
{
    public class RandomizerSettingsFactory
    {
        public static RandomizerSettings Generate(string preset)
        {
            var enumValue = ((RandomizerPresets)Enum.Parse(typeof(RandomizerPresets),preset)).GetStringValue();
            return GenerateFromFlags(enumValue);
        }

        public static RandomizerSettings GenerateFromFlags(string flags) {
            //I don't like generatefromflags there, will move it to here eventually
            var settings =  new RandomizerSettings();
            settings.GenerateFromFlags(flags);
            return settings;
        }

        public static RandomizerSettings Beginner()
        {
            return new RandomizerSettings
            {
                StartWithCandle = true,
                StartWithLife = true,
                StartingTechs = StartingTechs.Downstab,
                CommunityHints = true,
                ShufflePalaceRooms = true,
                ShortenGreatPalace = true,
                ChangePalacePalettes = true,
                RestartAtPalacesIfGameOver = true,
                ShuffleSpellLocations = true,
                DisableMagicContainerRequirements = true,
                DisableLowHealthBeep = true,
                CombineFireWithRandomSpell = true,
                AttackEffectiveness = AttackEffectiveness.High,
                MagicEffectiveness = MagicEffectiveness.Low,
                LifeEffectiveness = LifeEffectiveness.High,
                ShuffleOverworldEnemies = true,
                ShufflePalaceEnemies = true,
                ShuffleEnemyExp = true,
                ShuffleBossExp = true,
                ShufflePalaceItems = true,
                ShuffleOverworldItems = true,
                MixOverworldAndPalaceItems = true,
                ShuffleSmallItems = true,
                PalacesContainsExtraKeys = true,
                RandomizeNewKasutoJarRequirements = true,
                TunicColor = TunicColor.Default,
                ShieldTunicColor = TunicColor.Orange,
                BeamSprite = BeamSprite.Default
            };
        }

        public static RandomizerSettings Swiss()
        {
            return new RandomizerSettings
            {
                StartWithCandle = true,
                CommunityHints = true,
                AllowPalacesToSwapContinents = true,
                IncludeGreatPalaceInShuffle = true,
                ShufflePalaceRooms = true,
                ShortenGreatPalace = true,
                ChangePalacePalettes = true,
                RestartAtPalacesIfGameOver = true,
                ShuffleAllExperienceNeeded = true,
                ShuffleAttackExperienceNeeded = true,
                ShuffleMagicExperienceNeeded = true,
                ShuffleLifeExperienceNeeded = true,
                ShuffleSpellLocations = true,
                DisableMagicContainerRequirements = true,
                CombineFireWithRandomSpell = true,
                MagicEffectiveness = MagicEffectiveness.Random,
                ShuffleOverworldEnemies = true,
                ShufflePalaceEnemies = true,
                ShuffleDripperEnemies = true,
                ShuffleEnemyExp = true,
                ShuffleEnemyHP = true,
                ShuffleBossExp = true,
                ShuffleWhichEnemiesStealExp = true,
                ShuffleAmountExpStolen = true,
                ShuffleSwordImmunity = true,
                ShufflePalaceItems = true,
                ShuffleOverworldItems = true,
                MixOverworldAndPalaceItems = true,
                IncludePbagCavesInItemShuffle = true,
                ShuffleSmallItems = true,
                PalacesContainsExtraKeys = true,
                RandomizeNewKasutoJarRequirements = true,
                ShuffleItemDropFrequency = true,
            };
        }

        public static RandomizerSettings Finals()
        {
            return new RandomizerSettings
            {
                StartingHeartContainers = Containers.Three,
                MaxHeartContainers = Containers.Seven,
                CommunityHints = true,
                AllowPalacesToSwapContinents = true,
                IncludeGreatPalaceInShuffle = true,
                ShuffleEncounters = true,
                AllowUnsafePathEncounters = true,
                ShufflePalaceRooms = true,
                ChangePalacePalettes = true,
                RestartAtPalacesIfGameOver = true,
                ShuffleAllExperienceNeeded = true,
                ShuffleAttackExperienceNeeded = true,
                ShuffleMagicExperienceNeeded = true,
                ShuffleLifeExperienceNeeded = true,
                ShuffleLifeRefill = true,
                ShuffleSpellLocations = true,
                AttackEffectiveness = AttackEffectiveness.Random,
                LifeEffectiveness = LifeEffectiveness.Random,
                MagicEffectiveness = MagicEffectiveness.Random,
                ShuffleOverworldEnemies = true,
                ShufflePalaceEnemies = true,
                ShuffleDripperEnemies = true,
                ShuffleEnemyExp = true,
                ShuffleEnemyHP = true,
                ShuffleBossExp = true,
                ShuffleWhichEnemiesStealExp = true,
                ShuffleAmountExpStolen = true,
                ShuffleSwordImmunity = true,
                MixLargeAndSmallEnemies = true,
                ShufflePalaceItems = true,
                ShuffleOverworldItems = true,
                MixOverworldAndPalaceItems = true,
                IncludePbagCavesInItemShuffle = true,
                ShuffleSmallItems = true,
                RandomizeNewKasutoJarRequirements = true,
                ShuffleItemDropFrequency = true
            };
        }

        public static RandomizerSettings Max()
        {
            return new RandomizerSettings
            {
                ShuffleStartingItems = true,
                ShuffleStartingSpells = true,
                StartingHeartContainers = Containers.Random,
                MaxHeartContainers = Containers.Random,
                StartingTechs = StartingTechs.Random,
                RandomizeNumberOfLives = true,
                AllowPalacesToSwapContinents = true,
                IncludeGreatPalaceInShuffle = true,
                ShuffleEncounters = true,
                AllowUnsafePathEncounters = true,
                ShufflePalaceRooms = true,
                ShortenGreatPalace = true,
                ChangePalacePalettes = true,
                RestartAtPalacesIfGameOver = true,
                NumberOfPalacesToComplete = Palaces.Random,
                ShuffleAllExperienceNeeded = true,
                ThunderbirdRequired = false,
                ShuffleAttackExperienceNeeded = true,
                ShuffleMagicExperienceNeeded = true,
                ShuffleLifeExperienceNeeded = true,
                ShuffleLifeRefill = true,
                ShuffleSpellLocations = true,
                DisableMagicContainerRequirements = true,
                CombineFireWithRandomSpell = true,
                AttackEffectiveness = AttackEffectiveness.Random,
                LifeEffectiveness = LifeEffectiveness.Random,
                MagicEffectiveness = MagicEffectiveness.Random,
                ShuffleOverworldEnemies = true,
                ShufflePalaceEnemies = true,
                ShuffleDripperEnemies = true,
                ShuffleEnemyExp = true,
                ShuffleEnemyHP = true,
                ShuffleBossExp = true,
                ShuffleWhichEnemiesStealExp = true,
                ShuffleAmountExpStolen = true,
                ShuffleSwordImmunity = true,
                MixLargeAndSmallEnemies = true,
                ShufflePalaceItems = true,
                ShuffleOverworldItems = true,
                MixOverworldAndPalaceItems = true,
                IncludePbagCavesInItemShuffle = true,
                ShuffleSmallItems = true,
                RandomizeNewKasutoJarRequirements = true,
                ShuffleItemDropFrequency = true
            };

        }
    }
}
