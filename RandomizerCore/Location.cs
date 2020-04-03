using System;
using RandomizerCore.Constants.Enums;

namespace Z2Randomizer
{
    public enum Terrain
    {
        Town = 0,
        Cave = 1,
        Palace = 2,
        Bridge = 3,
        Desert = 4,
        Grass = 5,
        Forest = 6,
        Swamp = 7,
        Grave = 8,
        Road = 9,
        Lava = 10,
        Mountain = 11,
        Water = 12,
        WalkableWater = 13,
        Rock = 14,
        Spider = 15,
        None = 16
    }

    public class Location
    {
        private readonly int _secondPartOfCave;
        private readonly int _appearToLowerUponExit;
        private readonly int _forceEnterRight;
        private readonly int _fallInHole;
        public Items _item;
        public bool _itemGet;

        public Terrain TerrainType { get; set; }

        public int YPos { get; set; }

        public int XPos { get; set; }

        public byte[] LocationBytes { get; set; }

        public int MemAddress { get; set; }

        public int PassThrough { get; set; }

        public int Map { get; set; }

        public int World { get; set; }

        public Tuple<int, int> Coords => Tuple.Create(YPos, XPos);

        public bool NeedJump { get; set; }

        public bool NeedHammer { get; set; }

        public bool NeedFairy { get; set; }

        public bool NeedRecorder { get; set; }

        public bool NeedBagu { get; set; }

        public bool CanShuffle { get; set; }

        public int HorizontalPos { get; set; }

        public int ExternalWorld { get; set; }

        public bool Reachable { get; set; }

        public int PalNum { get; set; }

        public int TownNum { get;  set; }

        /*
        Byte 0

        .xxx xxxx - Y position
        x... .... - External to this world

        Byte 1 (offset 3F bytes from Byte 0)

        ..xx xxxx - X position
        .x.. .... - Second part of a cave
        x... .... - Appear at the position of the area in ROM offset 2 lower than this one upon exit

        Byte 2 (offset 7E bytes from Byte 0)

        ..xx xxxx - Map number
        xx.. .... - Horizontal position to enter within map
            0 = enter from the left
            1 = enter at x=256 or from the right for 2 screens maps
            2 = enter at x=512 or from the right for 3 screens maps
            3 = enter from the right for 4 screens maps

        Byte 3 (offset BD bytes from Byte 0)

        ...x xxxx - World number
        ..x. .... - Forced enter from the right edge of screen
        .x.. .... - Pass through
        x... .... - Fall in hole
        */
        public Location(byte[] bytes, Terrain t, int mem)
        {
            LocationBytes = bytes;
            ExternalWorld = bytes[0] & 128;
            YPos = bytes[0] & 127;
            _appearToLowerUponExit = bytes[1] & 128;
            _secondPartOfCave = bytes[1] & 64;
            XPos = bytes[1] & 63;
            HorizontalPos = bytes[2] & 192;
            Map = bytes[2] & 63;
            _fallInHole = bytes[3] & 128;
            PassThrough = bytes[3] & 64;
            _forceEnterRight = bytes[3] & 32;
            World = bytes[3] & 31;
            TerrainType = t;
            MemAddress = mem;
            CanShuffle = true;
            _item = Items.Donotuse;
            _itemGet = false;
            Reachable = false;
            PalNum = 0;
            TownNum = 0;
        }

        public Location()
        {

        }

        public void UpdateBytes()
        {
            LocationBytes[0] = (byte)(ExternalWorld + YPos);
            LocationBytes[1] = (byte)(_appearToLowerUponExit + _secondPartOfCave + XPos);
            LocationBytes[2] = (byte)(HorizontalPos + Map);
            LocationBytes[3] = (byte)(_fallInHole + PassThrough + _forceEnterRight + World);
        }
    }
}
