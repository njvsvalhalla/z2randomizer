using System;
using System.Collections.Generic;
using System.Linq;

namespace Z2Randomizer
{
    internal class Palace
    {
        private readonly int _num;
        private Room _root;
        private readonly SortedDictionary<int, List<Room>> _rooms;
        private readonly List<Room> _upExits;
        private readonly List<Room> _downExits;
        private readonly List<Room> _leftExits;
        private readonly List<Room> _rightExits;
        private readonly List<Room> _onlyDownExits;
        private int _numRooms;
        private readonly int _baseAddr;
        private readonly int _connAddr;
        private readonly Hyrule _hy;
        private readonly List<int> _p4downOnly = new List<int> { 0x60, 0x78 };
        private readonly int _p5Block = 0xc8;
        private readonly List<int> _p6downOnly = new List<int> { 0xa0, 0xb4, 0xc0, 0xdc, 0xd8, 0xe4, 0xac };
        private readonly int _p6WeirdRoom = 0xac;
        private readonly int _p71Up = 0xa4;
        private readonly int _p7Block = 0xb4;
        private readonly int _thunderbird = 0xd4;
        private readonly int _darklink = 0xd8;
        private readonly List<int> _gems = new List<int> { 0x34, 0x88, 0x38, 0x70, 0xa4, 0xe8, 0xd8 };
        private readonly List<int> _p7downOnly = new List<int> { 0xa4, 0xb4, 0xc4, 0x2c };

        internal List<Room> AllRooms { get; set; }

        public bool NeedJumpOrFairy { get; set; }
        public bool NeedFairy { get; set; }
        public bool NeedGlove { get; set; }
        public bool NeedDstab { get; set; }

        public Palace(int number, int b, int c, Hyrule h)
        {
            _num = number;
            _root = null;
            _upExits = new List<Room>();
            _downExits = new List<Room>();
            _leftExits = new List<Room>();
            _rightExits = new List<Room>();
            _onlyDownExits = new List<Room>();
            _rooms = new SortedDictionary<int, List<Room>>();
            AllRooms = new List<Room>();
            _numRooms = 0;
            _baseAddr = b;
            _connAddr = c;
            _hy = h;
            NeedDstab = false;
            NeedFairy = false;
            NeedGlove = false;
            NeedJumpOrFairy = false;
            LoadMaps();
            CreateTree();
        }

        private void LoadMaps()
        {
            var mapsToLoad = new List<int>();
            var finishedMaps = new List<int>();
            mapsToLoad.Add(_hy.RomData.GetByte(_baseAddr + 0x7e) * 4);
            Room tbird = null;
            while (mapsToLoad.Count > 0)
            {
                var addr = _connAddr + mapsToLoad[0];
                var connectBytes = new byte[4];
                for (var i = 0; i < 4; i++)
                {
                    connectBytes[i] = _hy.RomData.GetByte(addr + i);
                    var nextRoom = connectBytes[i] & 0xFC;
                    if (finishedMaps.Contains(nextRoom) || nextRoom == 0 || nextRoom == 0xFC) continue;
                    mapsToLoad.Add(connectBytes[i] & 0xFC);
                    finishedMaps.Add(connectBytes[i] & 0xFC);
                }
                Room r;
                if ((_num == 6 && mapsToLoad[0] == _p6WeirdRoom) || _num == 7 && _p7downOnly.Contains(mapsToLoad[0]))
                {
                    r = new Room(mapsToLoad[0], connectBytes, addr, true);
                }
                else
                {
                    r = new Room(mapsToLoad[0], connectBytes, addr, false);
                }

                switch (_num)
                {
                    case 5 when mapsToLoad[0] == _p5Block:
                        {
                            var r2 = new Room(mapsToLoad[0], connectBytes, addr, false);
                            var l = new List<Room>
                            {
                                r2,
                                r
                            };
                            r.LeftByte = 0xFC;
                            r2.RightByte = 0xFC;
                            _rooms.Add(r.Map, l);
                            AllRooms.Add(r);
                            AllRooms.Add(r2);
                            mapsToLoad.Remove(mapsToLoad[0]);
                            continue;
                        }
                    case 7 when r.Map == _p7Block:
                        {
                            var r2 = new Room(r.Map, connectBytes, addr, true);
                            var l = new List<Room>();
                            r.RightByte = 0xFC;
                            r.DownByte = 0xFC;
                            r2.LeftByte = 0xFC;
                            l.Add(r2);
                            l.Add(r);
                            _rooms.Add(r.Map, l);
                            AllRooms.Add(r);
                            AllRooms.Add(r2);
                            mapsToLoad.Remove(mapsToLoad[0]);
                            continue;
                        }
                    case 7 when r.Map == _p71Up:
                        {
                            var r2 = new Room(mapsToLoad[0], connectBytes, addr, true);
                            var l = new List<Room>
                            {
                                r2,
                                r
                            };
                            r.LeftByte = 0xFC;
                            r.DownByte = 0xFC;
                            r2.RightByte = 0xFC;
                            _rooms.Add(r.Map, l);
                            AllRooms.Add(r);
                            AllRooms.Add(r2);
                            mapsToLoad.Remove(mapsToLoad[0]);
                            continue;
                        }
                    case 7 when r.Map == _thunderbird:
                        tbird = r;
                        break;
                }

                if (!_rooms.ContainsKey(r.Map))
                {
                    if (_root == null)
                    {
                        _root = r;
                        r.IsRoot = true;
                    }
                    var l = new List<Room>
                    {
                        r
                    };
                    _rooms.Add(r.Map, l);
                    AllRooms.Add(r);
                }
                mapsToLoad.Remove(mapsToLoad[0]);
            }
            CreateTree();
            foreach (var r in AllRooms)
            {
                AddRoom(r);
            }

            if (_num != 7 || !_hy.Props.RemoveThunderbird) return;

            tbird.Left.Right = tbird.Right;
            tbird.Left.RightByte = tbird.RightByte;
            tbird.Right.Left = tbird.Left;
            tbird.Right.LeftByte = tbird.LeftByte;
            AllRooms.Remove(tbird);
            _leftExits.Remove(tbird);
            _rightExits.Remove(tbird);
        }

        public void AddRoom(Room r)
        {
            if (r.Down != null)
            {
                if ((_num == 4 && _p4downOnly.Contains(r.Map) || _num == 6 && _p6downOnly.Contains(r.Map) || _num == 7 && _p7downOnly.Contains(r.Map)))
                {
                    _onlyDownExits.Add(r);
                }
                else
                {
                    _downExits.Add(r);
                }
            }

            if (r.Left != null)
            {
                _leftExits.Add(r);
            }

            if (r.Right != null)
            {
                if (!(_num == 1 && r.Map == 28))
                {
                    _rightExits.Add(r);
                }
            }

            if (r.Up != null)
            {
                _upExits.Add(r);
            }
            _numRooms++;
        }

        public bool RequiresThunderbird()
        {
            CheckSpecialPaths(_root);
            return !_rooms[_darklink][0].BeforeThunderBird;
        }

        public bool HasDeadEnd()
        {
            if (_onlyDownExits.Count == 0)
            {
                return false;
            }
            var end = _rooms[_gems[_num - 1]][0];
            foreach (var r in _onlyDownExits)
            {
                var reachable = new List<Room>();
                var roomsToCheck = new List<Room>();
                reachable.Add(r.Down);
                roomsToCheck.Add(r.Down);

                while (roomsToCheck.Count > 0)
                {
                    var c = roomsToCheck[0];
                    if (c.Left != null && !reachable.Contains(c.Left))
                    {
                        reachable.Add(c.Left);
                        roomsToCheck.Add(c.Left);
                    }
                    if (c.Right != null && !reachable.Contains(c.Right))
                    {
                        reachable.Add(c.Right);
                        roomsToCheck.Add(c.Right);
                    }
                    if (c.Up != null && !reachable.Contains(c.Up))
                    {
                        reachable.Add(c.Up);
                        roomsToCheck.Add(c.Up);
                    }
                    if (c.Down != null && !reachable.Contains(c.Down))
                    {
                        reachable.Add(c.Down);
                        roomsToCheck.Add(c.Down);
                    }
                    roomsToCheck.Remove(c);
                }
                if (!reachable.Contains(_root) && !reachable.Contains(end))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckSpecialPaths(Room r)
        {
            while (true)
            {
                if (r.BeforeThunderBird) return;
                if ((_num == 7) && r.Map == _thunderbird)
                {
                    r.BeforeThunderBird = true;
                    return;
                }

                r.BeforeThunderBird = true;
                if (r.Left != null)
                {
                    CheckSpecialPaths(r.Left);
                }

                if (r.Right != null)
                {
                    CheckSpecialPaths(r.Right);
                }

                if (r.Up != null)
                {
                    CheckSpecialPaths(r.Up);
                }

                if (r.Down != null)
                {
                    r = r.Down;
                    continue;
                }

                break;
            }
        }

        public bool AllReachable()
        {
            if (CanEnterTbirdFromLeft())
            {
                return false;
            }
            CheckPaths(_root, 2);
            return AllRooms.All(r => r.IsPlaced);
        }
        //0 = up, 1 = down, 2 = left, 3 = right
        private void CheckPaths(Room r, int dir)
        {
            while (true)
            {
                if (r.IsPlaced) return;
                if ((_num == 7) && r.Map == _thunderbird)
                {
                    if (dir == 3)
                    {
                        r.IsPlaced = false;
                        return;
                    }
                }

                r.IsPlaced = true;
                if (r.Left != null)
                {
                    CheckPaths(r.Left, 3);
                }

                if (r.Right != null)
                {
                    CheckPaths(r.Right, 2);
                }

                if (r.Up != null)
                {
                    CheckPaths(r.Up, 1);
                }

                if (r.Down != null)
                {
                    r = r.Down;
                    dir = 0;
                    continue;
                }

                break;
            }
        }

        private bool CanEnterTbirdFromLeft()
        {
            var reachable = new List<Room>();
            var roomsToCheck = new List<Room>();
            reachable.Add(_root.Down);
            roomsToCheck.Add(_root.Down);

            while (roomsToCheck.Count > 0)
            {
                var c = roomsToCheck[0];
                if (c.Left != null && c.Left.Map == _thunderbird)
                {
                    return true;
                }
                if (c.Left != null && !reachable.Contains(c.Left))
                {
                    reachable.Add(c.Left);
                    roomsToCheck.Add(c.Left);
                }
                if (c.Right != null && !reachable.Contains(c.Right) && c.Right.Map != _thunderbird)
                {
                    reachable.Add(c.Right);
                    roomsToCheck.Add(c.Right);
                }
                if (c.Up != null && !reachable.Contains(c.Up))
                {
                    reachable.Add(c.Up);
                    roomsToCheck.Add(c.Up);
                }
                if (c.Down != null && !reachable.Contains(c.Down))
                {
                    reachable.Add(c.Down);
                    roomsToCheck.Add(c.Down);
                }
                roomsToCheck.Remove(c);
            }
            return false;
        }

        public void ShuffleRooms()
        {
            //This method is so ugly and i hate it.
            for (var i = 0; i < _upExits.Count; i++)
            {
                var swap = _hy.R.Next(i, _upExits.Count);
                var temp = _upExits[i].Up;
                var down1 = _upExits[swap].Up;
                temp.Down = _upExits[swap];
                down1.Down = _upExits[i];
                _upExits[i].Up = down1;
                _upExits[swap].Up = temp;

                var tempByte = _upExits[i].UpByte;
                _upExits[i].UpByte = _upExits[swap].UpByte;
                _upExits[swap].UpByte = tempByte;

                tempByte = temp.DownByte;
                temp.DownByte = down1.DownByte;
                down1.DownByte = tempByte;
            }
            for (var i = 0; i < _onlyDownExits.Count; i++)
            {
                var swap = _hy.R.Next(i, _onlyDownExits.Count);

                var temp = _onlyDownExits[i].Down;
                var tempByte = _onlyDownExits[i].DownByte;

                _onlyDownExits[i].Down = _onlyDownExits[swap].Down;
                _onlyDownExits[i].DownByte = _onlyDownExits[swap].DownByte;
                _onlyDownExits[swap].Down = temp;
                _onlyDownExits[swap].DownByte = tempByte;
            }

            for (var i = 0; i < _downExits.Count; i++)
            {
                var swap = _hy.R.Next(i, _downExits.Count);
                var temp = _downExits[i].Down;
                var down1 = _downExits[swap].Down;
                temp.Up = _downExits[swap];
                down1.Up = _downExits[i];
                _downExits[i].Down = down1;
                _downExits[swap].Down = temp;

                var tempByte = _downExits[i].DownByte;
                _downExits[i].DownByte = _downExits[swap].DownByte;
                _downExits[swap].DownByte = tempByte;

                tempByte = temp.UpByte;
                temp.UpByte = down1.UpByte;
                down1.UpByte = tempByte;
            }

            for (var i = 0; i < _leftExits.Count; i++)
            {
                var swap = _hy.R.Next(i, _leftExits.Count);
                var temp = _leftExits[i].Left;
                var down1 = _leftExits[swap].Left;
                temp.Right = _leftExits[swap];
                down1.Right = _leftExits[i];
                _leftExits[i].Left = down1;
                _leftExits[swap].Left = temp;

                var tempByte = _leftExits[i].LeftByte;
                _leftExits[i].LeftByte = _leftExits[swap].LeftByte;
                _leftExits[swap].LeftByte = tempByte;

                tempByte = temp.RightByte;
                temp.RightByte = down1.RightByte;
                down1.RightByte = tempByte;
            }

            for (var i = 0; i < _rightExits.Count; i++)
            {
                var swap = _hy.R.Next(i, _rightExits.Count);
                var temp = _rightExits[i].Right;
                var down1 = _rightExits[swap].Right;
                temp.Left = _rightExits[swap];
                down1.Left = _rightExits[i];
                _rightExits[i].Right = down1;
                _rightExits[swap].Right = temp;

                var tempByte = _rightExits[i].RightByte;
                _rightExits[i].RightByte = _rightExits[swap].RightByte;
                _rightExits[swap].RightByte = tempByte;

                tempByte = temp.LeftByte;
                temp.LeftByte = down1.LeftByte;
                down1.LeftByte = tempByte;
            }

            if (_num != 6) return;
            foreach (var r in _onlyDownExits)
            {
                if (r.Down.Map != 0xBC)
                {
                    var db = r.DownByte;
                    r.DownByte = (db & 0xFC) + 1;
                }
                else
                {
                    var db = r.DownByte;
                    r.DownByte = (db & 0xFC) + 2;
                }
            }
        }

        public void UpdateRom()
        {
            foreach (var r in AllRooms)
            {
                r.UpdateBytes();
                for (var i = 0; i < 4; i++)
                {
                    if (r.Connections[i] != 0xFC)
                    {
                        _hy.RomData.Put(r.MemAddr + i, r.Connections[i]);
                    }
                }
            }
        }

        private void CreateTree()
        {
            foreach (var r in AllRooms)
            {
                if (r.Left == null && (r.LeftByte < 0xFC && r.LeftByte > 3))
                {
                    var l = _rooms[r.LeftByte & 0xFC];
                    foreach (var r2 in l.Where(r2 => (r2.RightByte & 0xFC) == r.Map))
                    {
                        r.Left = r2;
                    }
                }

                if (r.Right == null && (r.RightByte < 0xFC && r.RightByte > 3))
                {
                    var l = _rooms[r.RightByte & 0xFC];
                    foreach (var r2 in l.Where(r2 => (r2.LeftByte & 0xFC) == r.Map))
                    {
                        r.Right = r2;
                    }
                }

                if (r.Up == null && (r.UpByte < 0xFC && r.UpByte > 3))
                {
                    var l = _rooms[r.UpByte & 0xFC];
                    foreach (var r2 in l.Where(r2 => (r2.DownByte & 0xFC) == r.Map))
                    {
                        r.Up = r2;
                    }
                }

                if (r.Down == null && (r.DownByte < 0xFC && r.DownByte > 3))
                {
                    var l = _rooms[r.DownByte & 0xFC];
                    foreach (var r2 in l.Where(r2 => r2.Map == (r.DownByte & 0xFC)))
                    {
                        r.Down = r2;
                    }
                }
                if ((r.UpByte & 0xFC) == 0 && (_root.DownByte & 0xFC) == r.Map)
                {
                    r.Up = _root;
                }
            }
        }

        public void Shorten()
        {
            var target = _hy.R.Next(_numRooms / 2, (_numRooms * 3) / 4) + 1;
            var rooms = _numRooms;
            var tries = 0;
            while (rooms > target && tries < 100000)
            {
                var r = _hy.R.Next(rooms);
                Room remove = null;
                if (_leftExits.Count < _rightExits.Count)
                {
                    remove = _rightExits[_hy.R.Next(_rightExits.Count)];
                }

                if (r < _leftExits.Count)
                {
                    remove = _leftExits[r];
                }

                r -= _leftExits.Count;
                if (r < _upExits.Count && r >= 0)
                {
                    remove = _upExits[r];
                }
                r -= _upExits.Count;
                if (r < _rightExits.Count && r >= 0)
                {
                    remove = _rightExits[r];
                }
                r -= _rightExits.Count;
                if (r < _downExits.Count && r >= 0)
                {
                    remove = _downExits[r];
                }

                if (_onlyDownExits.Contains(remove) || remove.Map == _thunderbird || remove.Map == _darklink)
                {
                    tries++;
                    continue;
                }

                var hasRight = remove.Right != null;
                var hasLeft = remove.Left != null;
                var hasUp = remove.Up != null;
                var hasDown = remove.Down != null;

                var n = 0;
                n = hasRight ? n + 1 : n;
                n = hasLeft ? n + 1 : n;
                n = hasUp ? n + 1 : n;
                n = hasDown ? n + 1 : n;

                //Console.WriteLine(n);

                if (n >= 3 || n == 1)
                {
                    tries++;
                    continue;
                }

                if (hasLeft && hasRight && (_onlyDownExits[0].Down != remove && _onlyDownExits[1].Down != remove && _onlyDownExits[2].Down != remove && _onlyDownExits[3].Down != remove))
                {
                    remove.Left.Right = remove.Right;
                    remove.Right.Left = remove.Left;
                    remove.Left.RightByte = remove.RightByte;
                    remove.Right.LeftByte = remove.LeftByte;
                    rooms--;
                    //Console.WriteLine("removed 1 room");
                    _leftExits.Remove(remove);
                    _rightExits.Remove(remove);
                    AllRooms.Remove(remove);
                    tries = 0;
                    continue;
                }

                if (hasUp && hasDown)
                {
                    remove.Up.Down = remove.Down;
                    remove.Down.Up = remove.Up;
                    remove.Up.DownByte = remove.DownByte;
                    remove.Down.UpByte = remove.UpByte;
                    rooms--;
                    _upExits.Remove(remove);
                    _downExits.Remove(remove);
                    AllRooms.Remove(remove);
                    tries = 0;
                    continue;
                }

                if (hasDown)
                {
                    if (hasLeft)
                    {
                        var count = 1;
                        count = remove.Left.Up != null ? count + 1 : count;
                        count = remove.Left.Down != null ? count + 1 : count;
                        count = remove.Left.Left != null ? count + 1 : count;
                        if (count >= 3 || count == 1)
                        {
                            tries++;
                            continue;
                        }

                        if (remove.Left.Up == null || remove.Left.Up != _root)
                        {
                            tries++;
                            continue;
                        }

                        remove.Left.Up.Down = remove.Down;
                        remove.Left.Up.DownByte = remove.DownByte;
                        remove.Down.Up = remove.Left.Up;
                        remove.Down.UpByte = remove.Left.UpByte;

                        _downExits.Remove(remove);
                        _leftExits.Remove(remove);
                        _rightExits.Remove(remove.Left);
                        _upExits.Remove(remove.Left);
                        AllRooms.Remove(remove);
                        AllRooms.Remove(remove.Left);
                        rooms -= 2;
                        tries = 0;
                    }
                    else
                    {
                        var count = 1;
                        count = remove.Right.Up != null ? count + 1 : count;
                        count = remove.Right.Down != null ? count + 1 : count;
                        count = remove.Right.Right != null ? count + 1 : count;
                        if (count >= 3 || count == 1)
                        {
                            tries++;
                            continue;
                        }

                        if (remove.Right.Up == null || remove.Right.Up == _root)
                        {
                            tries++;
                            continue;
                        }

                        remove.Right.Up.Down = remove.Down;
                        remove.Right.Up.DownByte = remove.DownByte;
                        remove.Down.Up = remove.Right.Up;
                        remove.Down.UpByte = remove.Right.UpByte;

                        _downExits.Remove(remove);
                        _rightExits.Remove(remove);
                        _leftExits.Remove(remove.Right);
                        _upExits.Remove(remove.Right);
                        AllRooms.Remove(remove);
                        AllRooms.Remove(remove.Right);

                        rooms -= 2;
                        tries = 0;
                    }
                }
                else
                {
                    if (hasLeft)
                    {
                        var count = 1;
                        count = remove.Left.Up != null ? count + 1 : count;
                        count = remove.Left.Down != null ? count + 1 : count;
                        count = remove.Left.Left != null ? count + 1 : count;
                        if (count >= 3 || count == 1)
                        {
                            tries++;
                            continue;
                        }

                        if (remove.Left.Down == null || _onlyDownExits.Contains(remove.Left))
                        {
                            tries++;
                            continue;
                        }

                        remove.Left.Down.Up = remove.Up;
                        remove.Left.Down.UpByte = remove.UpByte;
                        remove.Up.Down = remove.Left.Down;
                        remove.Up.DownByte = remove.Left.DownByte;

                        _upExits.Remove(remove);
                        _leftExits.Remove(remove);
                        _rightExits.Remove(remove.Left);
                        _downExits.Remove(remove.Left);
                        AllRooms.Remove(remove);
                        AllRooms.Remove(remove.Left);

                        rooms -= 2;
                        tries = 0;
                    }
                    else
                    {
                        var count = 1;
                        count = remove.Right.Up != null ? count + 1 : count;
                        count = remove.Right.Down != null ? count + 1 : count;
                        count = remove.Right.Right != null ? count + 1 : count;
                        if (count >= 3 || count == 1)
                        {
                            tries++;
                            continue;
                        }

                        if (remove.Right.Down == null || _onlyDownExits.Contains(remove.Right))
                        {
                            tries++;
                            continue;
                        }

                        remove.Right.Down.Up = remove.Up;
                        remove.Right.Down.UpByte = remove.UpByte;
                        remove.Up.Down = remove.Right.Down;
                        remove.Up.DownByte = remove.Right.DownByte;

                        _upExits.Remove(remove);
                        _rightExits.Remove(remove);
                        _leftExits.Remove(remove.Right);
                        _downExits.Remove(remove.Right);
                        AllRooms.Remove(remove);
                        AllRooms.Remove(remove.Right);

                        rooms -= 2;
                        tries = 0;
                    }
                }
            }
            Console.WriteLine("Target: " + target + " Rooms: " + rooms);
        }

        public void ShuffleSmallItems(int world, bool first)
        {
            var addresses = new List<int>();
            var items = new List<int>();
            int startAddr;
            if (first)
            {
                startAddr = 0x8523 - 0x8000 + (world * 0x4000) + 0x10;
            }
            else
            {
                startAddr = 0xA000 - 0x8000 + (world * 0x4000) + 0x10;
            }
            foreach (var r in AllRooms)
            {
                var i = startAddr + (r.Map / 2);
                int low = _hy.RomData.GetByte(i);
                var hi = _hy.RomData.GetByte(i + 1) * 256;
                int numBytes = _hy.RomData.GetByte(hi + low + 16 - 0x8000 + (world * 0x4000));
                for (var j = 4; j < numBytes; j += 2)
                {
                    var yPos = _hy.RomData.GetByte(hi + low + j + 16 - 0x8000 + (world * 0x4000)) & 0xF0;
                    yPos >>= 4;
                    if (_hy.RomData.GetByte(hi + low + j + 1 + 16 - 0x8000 + (world * 0x4000)) != 0x0F ||
                        yPos >= 13) continue;
                    var addr = hi + low + j + 2 + 16 - 0x8000 + (world * 0x4000);
                    int item = _hy.RomData.GetByte(addr);
                    if (item == 8 || (item > 9 && item < 14) || (item > 15 && item < 19) && !addresses.Contains(addr))
                    {
                        addresses.Add(addr);
                        items.Add(item);
                    }

                    j++;
                }
            }
            for (var i = 0; i < items.Count; i++)
            {
                var swap = _hy.R.Next(i, items.Count);
                var temp = items[swap];
                items[swap] = items[i];
                items[i] = temp;
            }
            for (var i = 0; i < addresses.Count; i++)
            {
                if (_hy.Props.ShuffleSmallItems)
                {
                    _hy.RomData.Put(addresses[i], (byte)items[i]);
                }

                if (_hy.Props.PalacesContainsExtraKeys && _num != 7)
                {
                    _hy.RomData.Put(addresses[i], 0x08);
                }
            }
        }

        public List<int> CheckBlocks(List<int> blockers)
        {
            return CheckBlocksHelper(new List<int>(), blockers, _root);
        }

        private List<int> CheckBlocksHelper(List<int> c, List<int> blockers, Room r)
        {
            c.Add(r.Map);
            if (r.Up != null && !blockers.Contains(r.Up.Map) && !c.Contains(r.Up.Map))
            {
                CheckBlocksHelper(c, blockers, r.Up);
            }
            if (r.Down != null && !blockers.Contains(r.Down.Map) && !c.Contains(r.Down.Map))
            {
                CheckBlocksHelper(c, blockers, r.Down);
            }
            if (r.Left != null && !blockers.Contains(r.Left.Map) && !c.Contains(r.Left.Map))
            {
                CheckBlocksHelper(c, blockers, r.Left);
            }
            if (r.Right != null && !blockers.Contains(r.Right.Map) && !c.Contains(r.Right.Map))
            {
                CheckBlocksHelper(c, blockers, r.Right);
            }
            return c;
        }
    }
}
