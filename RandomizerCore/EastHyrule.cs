using System;
using System.Collections.Generic;
using System.Linq;
using RandomizerCore.Constants.Enums;

namespace Z2Randomizer
{
    //6A31 - address in memory of kasuto y coord;
    //6A35 - address in memory of palace 6 y coord
    internal class EastHyrule : World
    {
        private int _bridgeCount;
        private readonly SortedDictionary<int, Terrain> _terrains = new SortedDictionary<int, Terrain>
            {
                { 0x862F, Terrain.Forest },
                { 0x8630, Terrain.Forest },
                { 0x8631, Terrain.Road },
                { 0x8632, Terrain.Road },
                { 0x8633, Terrain.Road },
                { 0x8634, Terrain.Road },
                { 0x8635, Terrain.Bridge },
                { 0x8636, Terrain.Bridge },
                { 0x8637, Terrain.Desert },
                { 0x8638, Terrain.Desert },
                { 0x8639, Terrain.WalkableWater },
                { 0x863A, Terrain.Cave },
                { 0x863B, Terrain.Cave },
                { 0x863C, Terrain.Cave },
                { 0x863D, Terrain.Cave },
                { 0x863E, Terrain.Cave },
                { 0x863F, Terrain.Cave },
                { 0x8640, Terrain.Cave },
                { 0x8641, Terrain.Cave },
                { 0x8642, Terrain.Cave },
                { 0x8643, Terrain.Cave },
                { 0x8644, Terrain.Swamp },
                { 0x8645, Terrain.Lava },
                { 0x8646, Terrain.Desert },
                { 0x8647, Terrain.Desert },
                { 0x8648, Terrain.Desert },
                { 0x8649, Terrain.Desert },
                { 0x864A, Terrain.Forest },
                { 0x864B, Terrain.Lava },
                { 0x864C, Terrain.Lava },
                { 0x864D, Terrain.Lava },
                { 0x864E, Terrain.Lava },
                { 0x864F, Terrain.Lava },
                { 0x8657, Terrain.Bridge },
                { 0x8658, Terrain.Bridge },
                { 0x865C, Terrain.Town },
                { 0x865E, Terrain.Town },
                { 0x8660, Terrain.Town },
                { 0x8662, Terrain.Town },
                { 0x8663, Terrain.Palace },
                { 0x8664, Terrain.Palace },
                { 0x8665, Terrain.Palace },

            };

        public Location _start;
        public Location _palace4;
        public Location _palace5;
        public Location _palace6;
        public Location _heart1;
        public Location _heart2;
        public Location _darunia;
        public Location _newKasuto;
        public Location _newKasuto2;
        public Location _fireTown;
        public Location _oldKasuto;
        public Location _gp;
        public Location _pbagCave1;
        public Location _pbagCave2;
        public Location _hpCallSpot;


        public EastHyrule(Hyrule hy)
            : base(hy)
        {
            LoadLocations(0x862F, 22, _terrains);
            LoadLocations(0x8646, 10, _terrains);
            LoadLocations(0x8657, 2, _terrains);
            LoadLocations(0x865C, 1, _terrains);
            LoadLocations(0x865E, 1, _terrains);
            LoadLocations(0x8660, 1, _terrains);
            LoadLocations(0x8662, 4, _terrains);

            _reachableAreas = new HashSet<string>();

            _connections.Add(GetLocationByMem(0x863A), GetLocationByMem(0x863B));
            _connections.Add(GetLocationByMem(0x863B), GetLocationByMem(0x863A));
            _connections.Add(GetLocationByMem(0x863E), GetLocationByMem(0x863F));
            _connections.Add(GetLocationByMem(0x863F), GetLocationByMem(0x863E));
            _connections.Add(GetLocationByMem(0x8640), GetLocationByMem(0x8641));
            _connections.Add(GetLocationByMem(0x8641), GetLocationByMem(0x8640));
            _connections.Add(GetLocationByMem(0x8642), GetLocationByMem(0x8643));
            _connections.Add(GetLocationByMem(0x8643), GetLocationByMem(0x8642));

            _palace6 = GetLocationByMem(0x8664);
            _palace6.PalNum = 6;
            _start = GetLocationByMem(0x8658);
            _darunia = GetLocationByMem(0x865E);
            _palace5 = GetLocationByMap(0x23, 0x0E);
            _palace5.PalNum = 5;

            _newKasuto = GetLocationByMem(0x8660);
            _newKasuto2 = new Location(_newKasuto.LocationBytes, _newKasuto.TerrainType, _newKasuto.MemAddress);
            _heart1 = GetLocationByMem(0x8639);
            _heart2 = GetLocationByMem(0x8649);
            if (_palace5 == null)
            {
                _palace5 = GetLocationByMem(0x8657);
                _palace5.PalNum = 5;
            }
            _palace4 = GetLocationByMap(0x0F, 0x11) ?? GetLocationByMem(0x8657);

            _hpCallSpot = new Location {XPos = 0, YPos = 0};

            _enemyAddr = 0x88B0;
            _enemies = new List<int> { 03, 04, 05, 0x11, 0x12, 0x14, 0x16, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
            _flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E, 0x15 };
            _generators = new List<int> { 0x0B, 0x0F, 0x17 };
            _shorties = new List<int> { 0x03, 0x04, 0x05, 0x11, 0x12, 0x16 };
            _tallGuys = new List<int> { 0x14, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
            _enemyPtr = 0x85B1;
            _fireTown = GetLocationByMem(0x865C);
            _oldKasuto = GetLocationByMem(0x8662);
            _gp = GetLocationByMem(0x8665);
            _gp.PalNum = 7;
            _gp._item = Items.Donotuse;
            _pbagCave1 = GetLocationByMem(0x863C);
            _pbagCave2 = GetLocationByMem(0x863D);

            _overworldMaps = new List<int> { 0x22, 0x1D, 0x27, 0x35, 0x30, 0x1E, 0x28, 0x3C };

            _mapRows = 75;
            _mapCols = 64;
        }

        public bool Terraform()
        {
            _bcount = 900;
            while (_bcount > 792)
            {
                _map = new Terrain[_mapRows, _mapCols];

                for (var i = 0; i < _mapRows; i++)
                {
                    for (var j = 0; j < _mapCols; j++)
                    {
                        _map[i, j] = Terrain.None;
                    }
                }
                DrawRoad();
                DaruniaPath();
                DrawMountains();
                DrawRiver();
                DrawVod();
                DrawOcean(false);
                DrawOcean(true);
                if (_hy._hiddenKasuto)
                {
                    DrawHiddenKasuto();
                }
                PlaceLocations();
                if (_hy._hiddenPalace)
                {
                    DrawHiddenPalace();
                }

                PlaceRandomTerrain(10);
                if (!GrowTerrain())
                {
                    return false;
                }
                DrawRaft(true);
                DrawRaft(false);
                WriteBytes(false, 0x9056, 792, _palace6.YPos - 30, _palace6.XPos);
                Console.WriteLine("East:" + _bcount);
            }
            if (_hy._hiddenPalace)
            {
                UpdateHPspot();
            }
            if (_hy._hiddenKasuto)
            {
                UpdateKasuto();
            }
            WriteBytes(true, 0x9056, 792, _palace6.YPos - 30, _palace6.XPos);


            var loc3 = 0x7C00 + _bcount;
            var high = (loc3 & 0xFF00) >> 8;
            var low = loc3 & 0xFF;

            _hy.RomData.Put(0x879F, (byte)low);
            _hy.RomData.Put(0x87A0, (byte)high);
            _hy.RomData.Put(0x87A1, (byte)low);
            _hy.RomData.Put(0x87A2, (byte)high);

            _v = new bool[_mapRows, _mapCols];
            for (var i = 0; i < _mapRows; i++)
            {
                for (var j = 0; j < _mapCols; j++)
                {
                    _v[i, j] = false;
                }
            }

            _newKasuto.ExternalWorld = 128;
            _palace6.ExternalWorld = 128;

            return true;
        }

        private void UpdateKasuto()
        {
            _hy.RomData.Put(0x1df79, (byte)(_newKasuto.YPos + 128));
            _hy.RomData.Put(0x1dfac, (byte)(_newKasuto.YPos - 30));
            _hy.RomData.Put(0x1dfb2, (byte)(_newKasuto.XPos + 1));
            _hy.RomData.Put(0x1ccd4, (byte)(_newKasuto.XPos));
            _hy.RomData.Put(0x1ccdb, (byte)(_newKasuto.YPos));
            _newKasuto.NeedHammer = true;
            _newKasuto2.NeedHammer = true;
        }

        private void DrawHiddenKasuto()
        {
            _hy.RomData.Put(0x8660, 0);
            _newKasuto.TerrainType = Terrain.Forest;
            _newKasuto2.TerrainType = Terrain.Forest;
        }

        private void DrawHiddenPalace()
        {
            var done = false;
            var xpos = _hy.R.Next(6, _mapCols - 6);
            var ypos = _hy.R.Next(6, _mapRows - 6);
            _palace6.NeedRecorder = true;
            while (!done)
            {
                xpos = _hy.R.Next(6, _mapCols - 6);
                ypos = _hy.R.Next(6, _mapRows - 6);
                done = true;
                for (var i = ypos - 3; i < ypos + 4; i++)
                {
                    for (var j = xpos - 3; j < xpos + 4; j++)
                    {
                        if (_map[i, j] != Terrain.None)
                        {
                            done = false;
                        }
                    }
                }
            }

            var t = _walkable[_hy.R.Next(_walkable.Count())];

            while (t == Terrain.Forest)
            {
                t = _walkable[_hy.R.Next(_walkable.Count())];
            }

            //t = terrain.desert;
            for (var i = ypos - 3; i < ypos + 4; i++)
            {
                for (var j = xpos - 3; j < xpos + 4; j++)
                {
                    if ((i == ypos - 2 && j == xpos) || (i == ypos && j == xpos - 2) || (i == ypos && j == xpos + 2))
                    {
                        _map[i, j] = Terrain.Mountain;
                    }
                    else
                    {
                        _map[i, j] = t;
                    }
                }
            }
            _map[_palace6.YPos - 30, _palace6.XPos] = _map[_palace6.YPos - 29, _palace6.XPos];
            _palace6.XPos = xpos;
            _palace6.YPos = ypos + 2 + 30;
            _hpCallSpot.XPos = xpos;
            _hpCallSpot.YPos = ypos + 30;
            _hy.RomData.Put(0x1df70, (byte)t);
        }

        public void UpdateHPspot()
        {
            _hy.RomData.Put(0x8382, (byte)_hpCallSpot.YPos);
            _hy.RomData.Put(0x8388, (byte)_hpCallSpot.XPos);
            var pos = _palace6.YPos;

            _hy.RomData.Put(0x1df78, (byte)(pos + 128));
            _hy.RomData.Put(0x1df84, 0xff);
            _hy.RomData.Put(0x1ccc0, (byte)pos);

            var ppuAddr1 = 0x2000 + 2 * (32 * (_palace6.YPos % 15) + (_palace6.XPos % 16)) + 2048 * (_palace6.YPos % 30 / 15);
            var ppuAddr2 = ppuAddr1 + 32;
            var ppu1Low = ppuAddr1 & 0x00ff;
            var ppu1High = (ppuAddr1 >> 8) & 0xff;
            var ppu2Low = ppuAddr2 & 0x00ff;
            var ppu2High = (ppuAddr2 >> 8) & 0xff;
            _hy.RomData.Put(0x1df7a, (byte)ppu1High);
            _hy.RomData.Put(0x1df7b, (byte)ppu1Low);
            _hy.RomData.Put(0x1df7f, (byte)ppu2High);
            _hy.RomData.Put(0x1df80, (byte)ppu2Low);
            _hy.RomData.Put(0x8664, 0);
        }

        private void DrawRoad()
        {
            //Draw road structure
            var r1X = _hy.R.Next(20, _mapCols - 20);
            var r1Y = _hy.R.Next(_mapRows / 3);

            var rl = new Location
            {
                XPos = r1X,
                YPos = r1Y + 30
            };

            var r2X = _hy.R.Next(_mapCols - 20, _mapCols);
            var r2Y = _hy.R.Next(_mapRows / 3, _mapRows);

            var r2 = new Location
            {
                XPos = r2X,
                YPos = r2Y + 30
            };

            _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
            _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
            DrawLine(rl, r2, Terrain.Road);
            rl = r2;

            if (_hy.R.NextDouble() > .5) //left
            {
                r2X = _hy.R.Next(_mapCols / 2);
                r2Y = _hy.R.Next(_mapRows / 3, _mapRows);
            }
            else
            {
                r2X = _hy.R.Next(_mapCols / 2, _mapCols);
                r2Y = _hy.R.Next(_mapRows / 3, _mapRows);
            }
            r2 = new Location
            {
                XPos = r2X,
                YPos = r2Y + 30
            };

            _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
            _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
            DrawLine(rl, r2, Terrain.Road);
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
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if ((x != to.XPos || y != (to.YPos - 30)) && _map[y, x] == Terrain.None)
                        {
                            _map[y, x] = t;
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
                else
                {
                    var diff = to.YPos - 30 - y;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if ((x != to.XPos || y != (to.YPos - 30)) && _map[y, x] == Terrain.None)
                        {
                            _map[y, x] = t;
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
        private void DrawLine(Location to, Location from, Terrain t, Terrain fill)
        {
            var x = from.XPos;
            var y = from.YPos - 30;
            _bridgeCount = 0;
            while (x != to.XPos || y != (to.YPos - 30))
            {
                if (_hy.R.NextDouble() > .5 && x != to.XPos)
                {
                    var diff = to.XPos - x;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if (t == Terrain.WalkableWater && (_map[y, x] == Terrain.Road || (_map[y, x] == Terrain.Desert && fill != Terrain.Desert)))
                        {
                            if (_bridgeCount == 0 && move > 1 && (diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x8635);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                _map[y, x] = Terrain.Bridge;
                                b.CanShuffle = false;
                            }
                            else if (_bridgeCount == 1 && move > 1 && (diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x8636);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                _map[y, x] = Terrain.Bridge;
                                b.CanShuffle = false;
                            }
                            else if ((diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                _map[y, x] = Terrain.Bridge;
                            }
                            else
                            {
                                _map[y, x] = t;
                            }
                        }
                        else if (_map[y, x] != Terrain.Bridge)
                        {
                            _map[y, x] = t;
                        }


                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] == Terrain.None)
                                {
                                    _map[i, j] = fill;
                                }

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
                else
                {
                    var diff = to.YPos - y - 30;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if (t == Terrain.WalkableWater && (_map[y, x] == Terrain.Road || (_map[y, x] == Terrain.Desert && fill != Terrain.Desert)))
                        {
                            if (_bridgeCount == 0 && move > 1 && (diff > 0 && (_map[y + 1, x] == Terrain.None || _map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && (_map[y - 1, x] == Terrain.None || _map[y - 1, x] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x8635);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                b.CanShuffle = false;
                                _map[y, x] = Terrain.Bridge;
                            }
                            else if (_bridgeCount == 1 && move > 1 && (diff > 0 && (_map[y + 1, x] == Terrain.None || _map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && (_map[y - 1, x] == Terrain.None || _map[y - 1, x] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x8636);
                                b.XPos = x;
                                b.YPos = y + 30;
                                b.CanShuffle = false;
                                _bridgeCount++;
                                _map[y, x] = Terrain.Bridge;
                            }
                            else if ((diff > 0 && (_map[y + 1, x] == Terrain.None || _map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && (_map[y - 1, x] == Terrain.None || _map[y - 1, x] == Terrain.Mountain)))
                            {
                                _map[y, x] = Terrain.Bridge;
                            }
                            else
                            {
                                _map[y, x] = t;
                            }
                        }
                        else
                        {
                            _map[y, x] = t;
                        }
                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] == Terrain.None)
                                {
                                    _map[i, j] = fill;
                                }

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

        public void SetStart()
        {
            _v[_start.YPos - 30, _start.XPos] = true;
        }

        public void UpdateVisit()
        {
            UpdateReachable();

            foreach (var l in AllLocations)
            {
                if (l.YPos > 30)
                {
                    if (_v[l.YPos - 30, l.XPos])
                    {
                        if ((!l.NeedRecorder && !l.NeedHammer) || (l.NeedRecorder && _hy._itemGet[(int)Items.Horn]) || (l.NeedHammer && _hy._itemGet[(int)Items.Hammer]))
                        {
                            l.Reachable = true;
                            if (_connections.Keys.Contains(l))
                            {
                                var l2 = _connections[l];

                                if (l.NeedFairy && _hy._spellGet[(int)Spells.Fairy])
                                {
                                    l2.Reachable = true;
                                    _v[l2.YPos - 30, l2.XPos] = true;
                                }

                                if (l.NeedJump && _hy._spellGet[(int)Spells.Jump])
                                {
                                    l2.Reachable = true;
                                    _v[l2.YPos - 30, l2.XPos] = true;
                                }

                                if (!l.NeedFairy && !l.NeedBagu && !l.NeedJump)
                                {
                                    l2.Reachable = true;
                                    _v[l2.YPos - 30, l2.XPos] = true;
                                }
                            }
                        }
                    }
                }
                if (_newKasuto.Reachable)
                {
                    _newKasuto2.Reachable = true;
                }
            }
        }

        private static double ComputeDistance(Location l, Location l2)
        {
            return Math.Sqrt(Math.Pow(l.XPos - l2.XPos, 2) + Math.Pow(l.YPos - l2.YPos, 2));
        }

        private void DaruniaPath()
        {
            //darunia path
            var gpx = _hy.R.Next(_mapCols - 6) + 3;
            var gpy = _hy.R.Next(_mapRows / 2 - 2) + 1;

            var from = new Location();
            var to = new Location();

            from.XPos = gpx;
            from.YPos = gpy + 30;
            double d = 26;
            while (d > 25)
            {
                to.XPos = _hy.R.Next(_mapCols - 6) + 3;
                to.YPos = _hy.R.Next(_mapRows / 2 - 2) + 34;

                d = ComputeDistance(from, to);
            }
            var encounterCount = 0;

            var x = from.XPos;
            var y = from.YPos - 30;
            while (x != to.XPos || y != (to.YPos - 30))
            {
                if (_hy.R.NextDouble() > .5 && x != to.XPos)
                {
                    var diff = to.XPos - x;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    var first = true;
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        var curr3 = new Location
                        {
                            XPos = x,
                            YPos = y + 30
                        };

                        _map[y, x] = Terrain.Desert;
                        if (ComputeDistance(curr3, to) < .666 * d && move > 1 && encounterCount == 0 && !first)
                        {
                            var b = GetLocationByMem(0x8637);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .333 * d && move > 1 && encounterCount == 1 && !first)
                        {
                            var b = GetLocationByMem(0x8638);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }

                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] == Terrain.None)
                                {
                                    _map[i, j] = Terrain.Mountain;
                                }

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
                        first = false;
                    }
                }
                else
                {
                    var diff = to.YPos - y - 30;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    var first = true;
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        var curr3 = new Location
                        {
                            XPos = x,
                            YPos = y + 30
                        };

                        _map[y, x] = Terrain.Desert;
                        if (ComputeDistance(curr3, to) < .666 * d && move > 1 && encounterCount == 0 && !first)
                        {
                            var b = GetLocationByMem(0x8637);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .333 * d && move > 1 && encounterCount == 1 && !first)
                        {
                            var b = GetLocationByMem(0x8638);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }

                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] == Terrain.None)
                                {
                                    _map[i, j] = Terrain.Mountain;
                                }

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
                        first = false;
                    }
                }
            }
            if (encounterCount < 2)
            {
                var b = GetLocationByMem(0x8638);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (encounterCount < 1)
            {
                var b = GetLocationByMem(0x8637);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            _map[to.YPos - 30, to.XPos] = Terrain.Desert;
            _map[to.YPos - 30 - 1, to.XPos] = Terrain.Desert;
            _map[to.YPos - 30 + 1, to.XPos] = Terrain.Desert;
            _map[to.YPos - 30 - 1, to.XPos + 1] = Terrain.Desert;
            _map[to.YPos - 30 - 1, to.XPos - 1] = Terrain.Desert;
            _map[to.YPos - 30 + 1, to.XPos + 1] = Terrain.Desert;
            _map[to.YPos - 30 + 1, to.XPos - 1] = Terrain.Desert;
            _map[to.YPos - 30, to.XPos + 1] = Terrain.Desert;
            _map[to.YPos - 30, to.XPos - 1] = Terrain.Desert;

            _map[from.YPos - 30, from.XPos] = Terrain.Desert;
            _map[to.YPos - 30 - 1, from.XPos] = Terrain.Desert;
            _map[from.YPos - 30 + 1, from.XPos] = Terrain.Desert;
            _map[from.YPos - 30 - 1, from.XPos + 1] = Terrain.Desert;
            _map[from.YPos - 30 - 1, from.XPos - 1] = Terrain.Desert;
            _map[from.YPos - 30 + 1, from.XPos + 1] = Terrain.Desert;
            _map[from.YPos - 30 + 1, from.XPos - 1] = Terrain.Desert;
            _map[from.YPos - 30, from.XPos + 1] = Terrain.Desert;
            _map[from.YPos - 30, from.XPos - 1] = Terrain.Desert;
        }

        private void DrawMountains()
        {
            //create some mountains
            var mounty = _hy.R.Next(_mapCols / 3 - 10, _mapCols / 3 + 10);
            _map[mounty, 0] = Terrain.Mountain;
            var placedSpider = false;


            var endmounty = _hy.R.Next(_mapCols / 3 - 10, _mapCols / 3 + 10);
            var endmountx = _hy.R.Next(2, 8);
            var x2 = 0;
            var y2 = mounty;
            var roadEncounters = 0;
            while (x2 != (_mapCols - endmountx) || y2 != endmounty)
            {
                if (Math.Abs(x2 - (_mapCols - endmountx)) >= Math.Abs(y2 - endmounty))
                {
                    if (x2 > _mapCols - endmountx)
                    {
                        x2--;
                    }
                    else
                    {
                        x2++;
                    }
                }
                else
                {
                    if (y2 > endmounty)
                    {
                        y2--;
                    }
                    else
                    {
                        y2++;
                    }
                }

                if (x2 == _mapCols - endmountx && y2 == endmounty) continue;

                switch (_map[y2, x2])
                {
                    case Terrain.None:
                        _map[y2, x2] = Terrain.Mountain;
                        break;
                    case Terrain.Road when !placedSpider:
                        _map[y2, x2] = Terrain.Spider;
                        placedSpider = true;
                        break;
                    case Terrain.Road:
                    {
                        if (_map[y2, x2 + 1] == Terrain.None && (((y2 > 0 && _map[y2 - 1, x2] == Terrain.Road) && (y2 < _mapRows - 1 && _map[y2 + 1, x2] == Terrain.Road)) || ((x2 > 0 && _map[y2, x2 - 0] == Terrain.Road) && (x2 < _mapCols - 1 && _map[y2, x2 + 1] == Terrain.Road))))
                        {
                            if (roadEncounters == 0)
                            {
                                var roadEnc = GetLocationByMem(0x8631);
                                roadEnc.XPos = x2;
                                roadEnc.YPos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEncounters++;
                            }
                            else if (roadEncounters == 1)
                            {
                                var roadEnc = GetLocationByMem(0x8632);
                                roadEnc.XPos = x2;
                                roadEnc.YPos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEncounters++;
                            }
                            else if (roadEncounters == 2)
                            {
                                var roadEnc = GetLocationByMem(0x8633);
                                roadEnc.XPos = x2;
                                roadEnc.YPos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEncounters++;
                            }
                            else if (roadEncounters == 3)
                            {
                                var roadEnc = GetLocationByMem(0x8634);
                                roadEnc.XPos = x2;
                                roadEnc.YPos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEncounters++;
                            }
                        }

                        break;
                    }
                }
            }

            mounty = _hy.R.Next(_mapCols * 2 / 3 - 10, _mapCols * 2 / 3 + 10);
            _map[mounty, 0] = Terrain.Mountain;

            endmounty = _hy.R.Next(_mapCols * 2 / 3 - 10, _mapCols * 2 / 3 + 10);
            endmountx = _hy.R.Next(2, 8);
            x2 = 0;
            y2 = mounty;

            while (x2 != (_mapCols - endmountx) || y2 != endmounty)
            {
                if (Math.Abs(x2 - (_mapCols - endmountx)) >= Math.Abs(y2 - endmounty))
                {
                    if (x2 > _mapCols - endmountx)
                    {
                        x2--;
                    }
                    else
                    {
                        x2++;
                    }
                }
                else
                {
                    if (y2 > endmounty)
                    {
                        y2--;
                    }
                    else
                    {
                        y2++;
                    }
                }

                if (x2 == _mapCols - endmountx && y2 == endmounty) continue;

                switch (_map[y2, x2])
                {
                    case Terrain.None:
                        _map[y2, x2] = Terrain.Mountain;
                        break;
                    case Terrain.Road when !placedSpider:
                        _map[y2, x2] = Terrain.Spider;
                        placedSpider = true;
                        break;
                    case Terrain.Road:
                    {
                        if (_map[y2, x2 + 1] == Terrain.None && (((y2 > 0 && _map[y2 - 1, x2] == Terrain.Road) && (y2 < _mapRows - 1 && _map[y2 + 1, x2] == Terrain.Road)) || ((x2 > 0 && _map[y2, x2 - 0] == Terrain.Road) && (x2 < _mapCols - 1 && _map[y2, x2 + 1] == Terrain.Road))))
                        {
                            switch (roadEncounters)
                            {
                                case 0:
                                {
                                    var roadEnc = GetLocationByMem(0x8631);
                                    roadEnc.XPos = x2;
                                    roadEnc.YPos = y2 + 30;
                                    roadEnc.CanShuffle = false;
                                    roadEncounters++;
                                    break;
                                }
                                case 1:
                                {
                                    var roadEnc = GetLocationByMem(0x8632);
                                    roadEnc.XPos = x2;
                                    roadEnc.YPos = y2 + 30;
                                    roadEnc.CanShuffle = false;
                                    roadEncounters++;
                                    break;
                                }
                                case 2:
                                {
                                    var roadEnc = GetLocationByMem(0x8633);
                                    roadEnc.XPos = x2;
                                    roadEnc.YPos = y2 + 30;
                                    roadEnc.CanShuffle = false;
                                    roadEncounters++;
                                    break;
                                }
                                case 3:
                                {
                                    var roadEnc = GetLocationByMem(0x8634);
                                    roadEnc.XPos = x2;
                                    roadEnc.YPos = y2 + 30;
                                    roadEnc.CanShuffle = false;
                                    roadEncounters++;
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            if (roadEncounters < 4)
            {
                var b = GetLocationByMem(0x8634);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (roadEncounters < 3)
            {
                var b = GetLocationByMem(0x8633);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }
            if (roadEncounters < 2)
            {
                var b = GetLocationByMem(0x8632);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (roadEncounters >= 1) return;

            {
                var b = GetLocationByMem(0x8631);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }
        }

        private void DrawRiver()
        {
            //draw a river

            var dirr = _hy.R.Next(4);
            var dirr2 = dirr;
            while (dirr == dirr2)
            {
                dirr2 = _hy.R.Next(4);
            }

            Location lr;
            switch (dirr)
            {
                //start north
                case 0:
                {
                    var startx = _hy.R.Next(_mapCols);
                    lr = new Location
                    {
                        XPos = startx,
                        YPos = 30
                    };
                    break;
                }
                //start east
                case 1:
                {
                    var startx = _hy.R.Next(_mapRows);
                    lr = new Location
                    {
                        YPos = startx + 30,
                        XPos = _mapCols - 1
                    };
                    break;
                }
                //start south
                case 2:
                {
                    var startx = _hy.R.Next(_mapCols);
                    lr = new Location
                    {
                        XPos = startx,
                        YPos = _mapRows - 1 + 30
                    };
                    break;
                }
                //start west
                default:
                {
                    var startx = _hy.R.Next(_mapRows);
                    lr = new Location
                    {
                        YPos = startx + 30,
                        XPos = 0
                    };
                    break;
                }
            }

            Location lr2;
            if (dirr2 == 0) //start north
            {
                var startx = _hy.R.Next(_mapCols);
                lr2 = new Location
                {
                    XPos = startx,
                    YPos = 30
                };
            }
            else if (dirr2 == 1) //start east
            {
                var startx = _hy.R.Next(_mapRows);
                lr2 = new Location
                {
                    YPos = startx + 30,
                    XPos = _mapCols - 1
                };
            }
            else if (dirr2 == 2) //start south
            {
                var startx = _hy.R.Next(_mapCols);
                lr2 = new Location
                {
                    XPos = startx,
                    YPos = _mapRows - 1 + 30
                };
            }
            else //start west
            {
                var startx = _hy.R.Next(_mapRows);
                lr2 = new Location
                {
                    YPos = startx + 30,
                    XPos = 0
                };
            }

            _map[lr.YPos - 30, lr.XPos] = Terrain.WalkableWater;
            _map[lr2.YPos - 30, lr.XPos] = Terrain.WalkableWater;
            DrawLine(lr, lr2, Terrain.WalkableWater, _walkable[_hy.R.Next(_walkable.Length)]);

            if (_bridgeCount < 2)
            {
                var b = GetLocationByMem(0x8636);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (_bridgeCount >= 1) return;
            {
                var b = GetLocationByMem(0x8635);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }
        }

        private void DrawVod()
        {
            //place gp and valley of death
            var gpx = _hy.R.Next(_mapCols - 6) + 3;
            var gpy = _hy.R.Next((_mapRows / 2) + 3, _mapRows - 3);

            _map[gpy + 1, gpx] = Terrain.Lava;
            _map[gpy - 1, gpx] = Terrain.Lava;
            _map[gpy + 1, gpx + 1] = Terrain.Lava;
            _map[gpy - 1, gpx - 1] = Terrain.Lava;
            _map[gpy + 1, gpx - 1] = Terrain.Lava;
            _map[gpy - 1, gpx + 1] = Terrain.Lava;
            _map[gpy, gpx + 1] = Terrain.Lava;
            _map[gpy, gpx - 1] = Terrain.Lava;

            _map[gpy + 2, gpx + 2] = Terrain.Mountain;
            _map[gpy + 1, gpx + 2] = Terrain.Mountain;
            _map[gpy, gpx + 2] = Terrain.Mountain;
            _map[gpy - 1, gpx + 2] = Terrain.Mountain;
            _map[gpy - 2, gpx + 2] = Terrain.Mountain;
            _map[gpy - 2, gpx + 1] = Terrain.Mountain;
            _map[gpy - 2, gpx] = Terrain.Mountain;
            _map[gpy - 2, gpx - 1] = Terrain.Mountain;
            _map[gpy - 2, gpx - 2] = Terrain.Mountain;
            _map[gpy - 1, gpx - 2] = Terrain.Mountain;
            _map[gpy, gpx - 2] = Terrain.Mountain;
            _map[gpy + 1, gpx - 2] = Terrain.Mountain;
            _map[gpy + 2, gpx - 2] = Terrain.Mountain;
            _map[gpy + 2, gpx - 1] = Terrain.Mountain;
            _map[gpy + 2, gpx] = Terrain.Mountain;
            _map[gpy + 2, gpx + 1] = Terrain.Mountain;

            var from = GetLocationByMem(0x8665);
            from.XPos = gpx;
            from.YPos = gpy + 30;
            from.CanShuffle = false;
            var to = new Location();

            double d = 26;
            while (d > 25)
            {
                to.XPos = _hy.R.Next(_mapCols - 6) + 3;
                to.YPos = _hy.R.Next((_mapRows / 2) + 5, _mapRows - 5) + 30;
                d = ComputeDistance(from, to);
            }
            var encounterCount = 0;

            var x = from.XPos;
            var y = from.YPos - 30;
            while (x != to.XPos || y != (to.YPos - 30))
            {
                if (_hy.R.NextDouble() > .5 && x != to.XPos)
                {
                    var diff = to.XPos - x;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    var first = true;
                    while (Math.Abs(move) > 0 && x != to.XPos)
                    {
                        var curr3 = new Location
                        {
                            XPos = x,
                            YPos = y + 30
                        };

                        _map[y, x] = Terrain.Lava;
                        if (ComputeDistance(curr3, to) < .75 * d && move > 1 && encounterCount == 0 && !first)
                        {
                            var b = GetLocationByMem(0x864F);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .5 * d && move > 1 && encounterCount == 1 && !first)
                        {
                            var b = GetLocationByMem(0x864E);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .5 * d && move > 1 && encounterCount == 2 && !first)
                        {
                            var b = GetLocationByMem(0x864D);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }

                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] != Terrain.Lava)
                                {
                                    _map[i, j] = Terrain.Mountain;
                                }

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
                        first = false;
                    }
                }
                else
                {
                    var diff = to.YPos - y - 30;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    var first = true;
                    while (Math.Abs(move) > 0 && y != to.YPos - 30)
                    {
                        var curr3 = new Location
                        {
                            XPos = x,
                            YPos = y + 30
                        };

                        _map[y, x] = Terrain.Lava;
                        if (ComputeDistance(curr3, to) < .75 * d && move > 1 && encounterCount == 0 && !first)
                        {
                            var b = GetLocationByMem(0x864F);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .5 * d && move > 1 && encounterCount == 1 && !first)
                        {
                            var b = GetLocationByMem(0x864E);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        else if (ComputeDistance(curr3, to) < .25 * d && move > 1 && encounterCount == 2 && !first)
                        {
                            var b = GetLocationByMem(0x864D);
                            b.XPos = x;
                            b.YPos = y + 30;
                            encounterCount++;
                            b.CanShuffle = false;
                        }
                        for (var i = y - 1; i <= y + 1; i++)
                        {
                            for (var j = x - 1; j <= x + 1; j++)
                            {
                                if ((i != y || j != x) && i >= 0 && i < _mapRows && j >= 0 && j < _mapCols && _map[i, j] != Terrain.Lava)
                                {
                                    _map[i, j] = Terrain.Mountain;
                                }

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
                        first = false;
                    }
                }
            }
            if (encounterCount < 3)
            {
                var b = GetLocationByMem(0x864D);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (encounterCount < 2)
            {
                var b = GetLocationByMem(0x864E);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (encounterCount < 1)
            {
                var b = GetLocationByMem(0x864F);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            var t5 = _walkable[_hy.R.Next(_walkable.Length)];
            _map[to.YPos - 30, to.XPos] = t5;
            if (_map[to.YPos - 30, to.XPos + 1] == Terrain.Lava || _map[to.YPos - 30, to.XPos - 1] == Terrain.Lava) //moving left/right
            {
                _map[to.YPos - 30 - 1, to.XPos] = t5;
                _map[to.YPos - 30 + 1, to.XPos] = t5;
            }
            else
            {
                _map[to.YPos - 30, to.XPos - 1] = t5;
                _map[to.YPos - 30, to.XPos + 1] = t5;
            }

            _map[gpy, gpx] = Terrain.Palace;
        }


        private void DrawRaft(bool left)
        {
            Console.WriteLine(left);
            var rafty = _hy.R.Next(0, _mapRows);
            var raftx = 0;
            if (!left)
            {
                raftx = _mapCols - 1;
                while (_map[rafty, raftx] != Terrain.WalkableWater)
                {
                    rafty = _hy.R.Next(0, _mapRows);
                }
                while (_map[rafty, raftx] == Terrain.WalkableWater && raftx < _mapCols - 1)
                {
                    raftx++;
                }
            }

            else
            {
                while (_map[rafty, raftx] != Terrain.WalkableWater)
                {
                    rafty = _hy.R.Next(0, _mapRows);
                }
                while (_map[rafty, raftx] == Terrain.WalkableWater && raftx > 0)
                {
                    raftx--;
                }
            }

            var tries = 0;
            while (!_walkable.Contains(_map[rafty, raftx]))
            {
                tries++;
                if (tries > 100)
                {
                    //terraform();
                    _palace4.XPos = 0;
                    _palace4.YPos = 0;
                    return;
                }
                rafty = _hy.R.Next(0, _mapRows);
                raftx = 0;
                if (!left)
                {
                    raftx = _mapCols - 1;
                    while (_map[rafty, raftx] != Terrain.WalkableWater)
                    {
                        rafty = _hy.R.Next(0, _mapRows);
                    }
                    while (_map[rafty, raftx] == Terrain.WalkableWater && raftx > 0)
                    {
                        raftx--;
                    }
                }
                else
                {
                    while (_map[rafty, raftx] != Terrain.WalkableWater)
                    {
                        rafty = _hy.R.Next(0, _mapRows);
                    }
                    while (_map[rafty, raftx] == Terrain.WalkableWater && raftx < _mapCols - 1)
                    {
                        raftx++;
                    }
                }
            }

            if (left)
            {
                _map[rafty, raftx] = Terrain.Bridge;
                _start.XPos = raftx;
                _start.YPos = rafty + 30;
            }
            else
            {
                _map[rafty, raftx] = Terrain.Bridge;
                _palace4.XPos = raftx;
                _palace4.YPos = rafty + 30;
                _palace4.PassThrough = 0;
                for (var i = raftx + 1; i < _mapCols; i++)
                {
                    _map[rafty, i] = Terrain.Bridge;
                }
            }

        }
    }
}
