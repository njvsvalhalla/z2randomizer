using System;

namespace Z2Randomizer
{
    internal class Room
    {
        private int _upByte;
        private int _downByte;
        private Room _up;
        private Room _down;
        private readonly bool _udRev;

        public int Map { get; set; }

        public bool IsRoot { get; set; }

        public Room Left { get; set; }

        public Room Right { get; set; }

        public Room Up
        {
            get => _udRev ? _down : _up;

            set
            {
                if(_udRev)
                {
                    _down = value;
                    return;
                }
                _up = value;
            }
        }

        public Room Down
        {
            get => _udRev ? _up : _down;

            set
            {
                if(_udRev)
                {
                    _up = value;
                    return;
                }
                _down = value;
            }
        }

        public bool IsReachable { get; set; }

        public int MemAddr { get; set; }

        public byte[] Connections { get; set; }

        public bool IsPlaced { get; set; }

        public int LeftByte { get; set; }

        public int RightByte { get; set; }

        public int UpByte
        {
            get => _udRev ? _downByte : _upByte;

            set
            {
                if(_udRev)
                {
                    _downByte = value;
                    return;
                }
                _upByte = value;
            }
        }

        public int DownByte
        {
            get => _udRev ? _upByte : _downByte;

            set
            {
                if(_udRev)
                {
                    _upByte = value;
                    return;
                }
                _downByte = value;
            }
        }

        public bool BeforeThunderBird { get; set; }

        public Room(int map, byte[] conn, int memAddr, bool upDownRev)
        {
            Map = map;
            Connections = conn;
            LeftByte = conn[0];
            _downByte = conn[1];
            _upByte = conn[2];
            RightByte = conn[3];
            IsRoot = false;
            IsReachable = false;
            MemAddr = memAddr;
            IsPlaced = false;
            Left = null;
            Right = null;
            _up = null;
            _down = null;
            BeforeThunderBird = false;
            _udRev = upDownRev;
        }

        public void UpdateBytes()
        {
            Connections[0] = (byte)LeftByte;
            Connections[1] = (byte)_downByte;
            Connections[2] = (byte)_upByte;
            Connections[3] = (byte)RightByte;
        }
    }
}
