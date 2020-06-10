using System;
using System.Collections.Generic;
using System.Linq;
using RandomizerCore.Constants.Enums;

namespace Z2Randomizer
{
    internal class MazeIsland : World
    {
        private readonly SortedDictionary<int, Terrain> _terrains = new SortedDictionary<int, Terrain>
        {
            { 0xA131, Terrain.Road },
                { 0xA132, Terrain.Road },
                { 0xA133, Terrain.Road },
                { 0xA134, Terrain.Bridge },
                { 0xA140, Terrain.Palace },
                { 0xA143, Terrain.Road },
                { 0xA145, Terrain.Road },
                { 0xA146, Terrain.Road },
                { 0xA147, Terrain.Road },
                { 0xA148, Terrain.Road },
                { 0xA149, Terrain.Road }
        };

        public Location _kid;
        public Location _magic;
        public Location _palace4;
        public Location _start;

        public MazeIsland(Hyrule hy)
            : base(hy)
        {
            LoadLocations(0xA131, 4, _terrains);
            LoadLocations(0xA140, 1, _terrains);
            LoadLocations(0xA143, 1, _terrains);
            LoadLocations(0xA145, 5, _terrains);

            _enemyAddr = 0x88B0;
            _enemies = new List<int> { 03, 04, 05, 0x11, 0x12, 0x14, 0x16, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
            _flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E, 0x15 };
            _generators = new List<int> { 0x0B, 0x0F, 0x17 };
            _shorties = new List<int> { 0x03, 0x04, 0x05, 0x11, 0x12, 0x16 };
            _tallGuys = new List<int> { 0x14, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
            _enemyPtr = 0xA08E;
            _overworldMaps = new List<int>();

            _kid = GetLocationByMem(0xA143);
            _magic = GetLocationByMem(0xA133);
            _palace4 = GetLocationByMem(0xA140);
            _palace4.PalNum = 4;
            _start = GetLocationByMem(0xA134);
            _mapRows = 20;
            _mapCols = 64;
        }


        public void SetStart()
        {
            _v[_start.YPos - 30, _start.XPos] = true;
        }

        public void Terraform()
        {
            _bcount = 900;
            if (!_hy.Props.IsClassicMode)
            {
                while (_bcount > 801)
                {
                    _map = new Terrain[_mapRows, _mapCols];
                    var visited = new bool[_mapRows, 21];
                    for (var i = 0; i < _mapRows; i += 2)
                    {
                        for (var j = 0; j < 21; j++)
                        {
                            _map[i, j] = Terrain.Mountain;
                        }
                    }

                    for (var i = 0; i < _mapRows; i++)
                    {
                        for (var j = 21; j < _mapCols; j++)
                        {
                            _map[i, j] = Terrain.Water;
                        }
                    }

                    for (var j = 0; j < 21; j += 2)
                    {
                        for (var i = 0; i < _mapRows; i++)
                        {
                            _map[i, j] = Terrain.Mountain;
                        }
                    }

                    for (var i = 0; i < _mapRows; i++)
                    {
                        for (var j = 0; j < 21; j++)
                        {
                            if (_map[i, j] != Terrain.Mountain && _map[i, j] != Terrain.Water)
                            {
                                _map[i, j] = Terrain.Road;
                                visited[i, j] = false;
                            }
                            else
                            {
                                visited[i, j] = true;
                            }
                        }
                    }

                    //choose starting position
                    var starty = _hy.R.Next(_mapRows);
                    if (starty == 0)
                    {
                        starty++;
                    }
                    else if (starty % 2 == 0)
                    {
                        starty--;
                    }

                    _map[starty, 0] = Terrain.Bridge;
                    _start.XPos = 0;
                    _start.YPos = starty + 30;
                    _start.PassThrough = 0;

                    //generate maze
                    var currx = 1;
                    var curry = starty;
                    var s = new Stack<Tuple<int, int>>();

                    while (MoreToVisit(visited))
                    {
                        var n = GetListOfNeighbors(currx, curry, visited);
                        if (n.Count > 0)
                        {
                            var next = n[_hy.R.Next(n.Count)];
                            s.Push(next);
                            if (next.Item1 > currx)
                            {
                                _map[curry, currx + 1] = Terrain.Road;
                            }
                            else if (next.Item1 < currx)
                            {
                                _map[curry, currx - 1] = Terrain.Road;
                            }
                            else if (next.Item2 > curry)
                            {
                                _map[curry + 1, currx] = Terrain.Road;
                            }
                            else
                            {
                                _map[curry - 1, currx] = Terrain.Road;
                            }

                            currx = next.Item1;
                            curry = next.Item2;
                            visited[curry, currx] = true;
                        }
                        else if (s.Count > 0)
                        {
                            var n2 = s.Pop();
                            currx = n2.Item1;
                            curry = n2.Item2;
                        }
                    }

                    //place palace 4


                    var p4X = _hy.R.Next(15) + 3;
                    var p4Y = _hy.R.Next(_mapRows - 6) + 3;

                    _palace4.XPos = p4X;
                    _palace4.YPos = p4Y + 30;
                    _map[p4Y, p4X] = Terrain.Palace;
                    _map[p4Y + 1, p4X] = Terrain.Road;
                    _map[p4Y - 1, p4X] = Terrain.Road;
                    _map[p4Y, p4X + 1] = Terrain.Road;
                    _map[p4Y, p4X - 1] = Terrain.Road;

                    //draw a river
                    var riverstart = starty;
                    while (riverstart == starty)
                    {
                        riverstart = _hy.R.Next(10) * 2;
                    }

                    var riverend = _hy.R.Next(10) * 2;

                    var rs = new Location
                    {
                        XPos = 0,
                        YPos = riverstart + 30
                    };

                    var re = new Location
                    {
                        XPos = 20,
                        YPos = riverend + 30
                    };

                    DrawLine(rs, re, Terrain.WalkableWater);

                    foreach (var l in AllLocations)
                    {
                        if (l.TerrainType != Terrain.Road) continue;
                        int x;
                        int y;
                        if (l != _magic && l != _kid)
                        {
                            do
                            {
                                x = _hy.R.Next(19) + 2;
                                y = _hy.R.Next(_mapRows - 4) + 2;
                            } while (_map[y, x] != Terrain.Road ||
                                     !((_map[y, x + 1] == Terrain.Mountain && _map[y, x - 1] == Terrain.Mountain) ||
                                       (_map[y + 1, x] == Terrain.Mountain && _map[y - 1, x] == Terrain.Mountain)) ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x + 1)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x - 1)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 31, x)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 29, x)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x)) != null);
                        }
                        else
                        {
                            do
                            {
                                x = _hy.R.Next(19) + 2;
                                y = _hy.R.Next(_mapRows - 4) + 2;
                            } while (_map[y, x] != Terrain.Road ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x + 1)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x - 1)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 31, x)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 29, x)) != null ||
                                     GetLocationByCoords(new Tuple<int, int>(y + 30, x)) != null);
                        }

                        l.XPos = x;
                        l.YPos = y + 30;
                    }

                    //check bytes and adjust
                    WriteBytes(false, 0xA65C, 801, 0, 0);
                }

                WriteBytes(true, 0xA65C, 801, 0, 0);

                var loc3 = 0x7C00 + _bcount;
                var high = (loc3 & 0xFF00) >> 8;
                var low = loc3 & 0xFF;


                _hy.RomData.Put(0x87A5, (Byte) low);
                _hy.RomData.Put(0x87A6, (Byte) high);

                for (var i = 0xA10C; i < 0xA149; i++)
                {
                    if (!_terrains.Keys.Contains(i))
                    {
                        _hy.RomData.Put(i, 0x00);
                    }
                }

                _v = new bool[_mapRows, _mapCols];
                for (var i = 0; i < _mapRows; i++)
                {
                    for (var j = 0; j < _mapCols; j++)
                    {
                        _v[i, j] = false;
                    }
                }
            }
            else
            {
                if (_hy.Props.ShuffleAll)
                {
                    if (_hy.Props.AllowTerrainChanges)
                    {
                        ShuffleLocations(AllLocations);
                        while (GetLocationByMem(0xA134).YPos == 0x43 && GetLocationByMem(0xA134).XPos == 0x28)
                        {
                            ShuffleLocations(AllLocations);
                        }
                    }
                    else
                    {
                        ShuffleLocations(Roads);
                    }
                }
                else if (_hy.Props.ShuffleEverythingElse)
                {
                    ShuffleLocations(Roads);
                }
                var palace = GetLocationByMem(0xA140);
                foreach (Location l in AllLocations)
                {
                    if (l != _kid && l != _palace4 && l != _magic && l != _start)
                    {
                        l.PassThrough = 64;
                    }
                    else
                    {
                        l.PassThrough = 0;
                    }
                }
            }

        }

        private bool MoreToVisit(bool[,] v)
        {
            for (var i = 0; i < v.GetLength(0); i++)
            {
                for (var j = 0; j < v.GetLength(1); j++)
                {
                    if (v[i, j] == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<Tuple<int, int>> GetListOfNeighbors(int currx, int curry, bool[,] v)
        {
            var x = new List<Tuple<int, int>>();

            if (currx - 2 > 0 && v[curry, currx - 2] == false)
            {
                x.Add(new Tuple<int, int>(currx - 2, curry));
            }

            if (currx + 2 < 21 && v[curry, currx + 2] == false)
            {
                x.Add(new Tuple<int, int>(currx + 2, curry));
            }

            if (curry - 2 > 0 && v[curry - 2, currx] == false)
            {
                x.Add(new Tuple<int, int>(currx, curry - 2));
            }

            if (curry + 2 < _mapRows && v[curry + 2, currx] == false)
            {
                x.Add(new Tuple<int, int>(currx, curry + 2));
            }

            return x;
        }

        private void DrawLine(Location to, Location from, Terrain t)
        {
            var x = from.XPos;
            var y = from.YPos - 30;
            while (x != to.XPos || y != (to.YPos - 30))
            {
                if (_hy.R.NextDouble() > .5 && x != to.XPos)
                {
                    var diff = to.XPos - x;
                    var move = (_hy.R.Next(Math.Abs(diff / 2)) + 1) * 2;


                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            if ((x != to.XPos || y != (to.YPos - 30)) && GetLocationByCoords(new Tuple<int, int>(y + 30, x)) == null)
                            {
                                if (_map[y, x] == Terrain.Mountain)
                                {
                                    _map[y, x] = t;
                                }
                                else if (_map[y, x] == Terrain.Road && (diff > 0 && (_map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && _map[y, x - 1] == Terrain.Mountain))
                                {
                                    _map[y, x] = Terrain.Bridge;
                                }
                                else if (_map[y, x] != Terrain.Palace && (x != _start.XPos || y != _start.YPos - 30))
                                {
                                    _map[y, x] = t;
                                }

                            }
                            if (diff > 0 && x < _mapCols - 1)
                            {
                                x++;
                            }
                            else if (x > 0)
                            {
                                x--;
                            }

                            move--;
                        }
                    }
                }
                else if (y != to.YPos - 30)
                {
                    var diff = to.YPos - 30 - y;
                    var move = (_hy.R.Next(Math.Abs(diff / 2)) + 1) * 2;
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            if ((x != to.XPos || y != (to.YPos - 30)) && GetLocationByCoords(new Tuple<int, int>(y + 30, x)) == null)
                            {
                                if (_map[y, x] == Terrain.Mountain)
                                {
                                    _map[y, x] = t;
                                }
                                else if (_map[y, x] == Terrain.Road && (diff > 0 && (_map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && _map[y - 1, x] == Terrain.Mountain))
                                {
                                    _map[y, x] = Terrain.Bridge;
                                }
                                else if (_map[y, x] != Terrain.Palace && (x != _start.XPos || y != _start.YPos - 30))
                                {
                                    _map[y, x] = t;
                                }
                            }
                            if (diff > 0 && y < _mapRows - 1)
                            {
                                y++;
                            }
                            else if (y > 0)
                            {
                                y--;
                            }
                            move--;
                        }
                    }
                }
            }
        }
        public void UpdateVisit()
        {
            var changed = true;
            while (changed)
            {
                changed = false;
                for (var i = 0; i < _mapRows; i++)
                {
                    for (var j = 0; j < _mapCols; j++)
                    {
                        if (_v[i, j] || ((_map[i, j] != Terrain.WalkableWater || !_hy._itemGet[(int) Items.Boots]) &&
                                        _map[i, j] != Terrain.Road && _map[i, j] != Terrain.Palace &&
                                        _map[i, j] != Terrain.Bridge)) continue;
                        if (i - 1 >= 0)
                        {
                            if (_v[i - 1, j])
                            {
                                _v[i, j] = true;
                                changed = true;
                                continue;
                            }

                        }

                        if (i + 1 < _mapRows)
                        {
                            if (_v[i + 1, j])
                            {
                                _v[i, j] = true;
                                changed = true;
                                continue;
                            }
                        }

                        if (j - 1 >= 0)
                        {
                            if (_v[i, j - 1])
                            {
                                _v[i, j] = true;
                                changed = true;
                                continue;
                            }
                        }

                        if (j + 1 < _mapCols)
                        {
                            if (_v[i, j + 1])
                            {
                                _v[i, j] = true;
                                changed = true;
                            }
                        }
                    }
                }
            }

            foreach (var l in AllLocations.Where(l => _v[l.YPos - 30, l.XPos]))
            {
                l.Reachable = true;
            }
        }
    }
}
