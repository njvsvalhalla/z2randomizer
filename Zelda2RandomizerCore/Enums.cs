using RandomizerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RandomizerApp
{
    public class Enums
    {
        public enum RandomizerPresets {
            [StringValue("jhmhMROm7DZ$cHRBTA")]
            Beginner,
            [StringValue("jhhhDcM#$Za$LpTBT!")]
            Swiss,
            [StringValue("hhAhC0j#x78gJqTBTR")]
            Finals,
            [StringValue("iyhqh$j#g7@$ZqTBT!")]
            Max,
            [StringValue("hhhhD0j#$Z8$JpTBT!")]
            Bracket
        }

        public enum EnemySize
        {
            Small,
            Large
        }

        public enum AttackEffectiveness
        {
            Random,
            Low,
            Normal,
            High,
            OHKO
        }

        public enum MagicEffectiveness
        {
            Random,
            High,
            Normal,
            Low,
            Free
        }

        public enum LifeEffectiveness
        {
            Random,
            OHKO,
            Normal,
            High,
            Invincible
        }

        public enum ItemPool
        {
            [StringValue("Blue Jar")]
            BlueJar,
            [StringValue("Red Jar")]
            RedJar,
            [StringValue("50 Pbag")]
            FiftyPbag,
            [StringValue("100 Pbag")]
            HundredPbag,
            [StringValue("200 Pbag")]
            TwoHundredPbag,
            [StringValue("500 Pbag")]
            FiveHundredPbag,
            [StringValue("1-Up")]
            OneUp,
            [StringValue("Key")]
            Key
        }

        public enum CharacterSprite
        {
            Link,
            Zelda,
            [StringValue("Iron Knuckle")]
            IronKnuckle,
            Error,
            Samus,
            Simon,
            Stalfos,
            [StringValue("Vase Lady")]
            VaseLady,
            Ruto,

        }

        public enum TunicColor
        {
            Default,
            Green,
            DarkGreen,
            Aqua,
            DarkBlue,
            Purple,
            Pink,
            Orange,
            Red,
            Turd,
            Random
        }

        public enum BeamSprite
        {
            Default,
            Fire,
            Bubble,
            Rock,
            Axe,
            Hammer,
            [StringValue("Wizzrobe Beam")]
            WizzrobeBeam,
            Random
        }

        public enum Containers
        {
            [StringValue("1")]
            One,
            [StringValue("2")]
            Two,
            [StringValue("3")]
            Three,
            [StringValue("4")]
            Four,
            [StringValue("5")]
            Five,
            [StringValue("6")]
            Six,
            [StringValue("7")]
            Seven,
            [StringValue("8")]
            Eight,
            Random
        }

        public enum Palaces
        {
            [StringValue("0")]
            Zero,
            [StringValue("1")]
            One,
            [StringValue("2")]
            Two,
            [StringValue("3")]
            Three,
            [StringValue("4")]
            Four,
            [StringValue("5")]
            Five,
            [StringValue("6")]
            Six,
            Random
        }

        public enum StartingTechs
        {
            None,
            Downstab,
            Upstab,
            Both,
            Random
        }

        public enum HintType
        {
            Normal,
            Spell,
            Helpful,
            [StringValue("Spell + Helpful")]
            Both
        }

        public enum HiddenOptions
        {
            Off,
            On,
            Random
        }
    }
}
