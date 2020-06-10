using RandomizerCore.Constants.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Z2Randomizer
{
    internal abstract class World
    {
        private readonly List<Location>[] _locations;
        public Dictionary<Location, Location> _connections;
        protected HashSet<string> _reachableAreas;
        protected int _enemyAddr;
        protected List<int> _enemies;
        protected List<int> _flyingEnemies;
        protected List<int> _generators;
        protected List<int> _shorties;
        protected List<int> _tallGuys;
        protected int _enemyPtr;
        protected List<int> _overworldMaps;
        protected SortedDictionary<Tuple<int, int>, Location> _locsByCoords;
        protected Hyrule _hy;
        protected Terrain[,] _map;
        private const int OverworldXOff = 0x3F;
        private const int OverworldMapOff = 0x7E;
        private const int OverworldWorldOff = 0xBD;
        private List<int> _visitedEnemies;
        protected int _mapRows;
        protected int _mapCols;
        protected int _bcount;
        protected readonly Terrain[] _randomTerrains = { Terrain.Desert, Terrain.Grass, Terrain.Forest, Terrain.Swamp, Terrain.Grave, Terrain.Mountain, Terrain.WalkableWater };
        protected readonly Terrain[] _walkable = { Terrain.Desert, Terrain.Grass, Terrain.Forest, Terrain.Swamp, Terrain.Grave };
        protected bool[,] _v;

        public List<Location> AllLocations { get; }
        public List<Location> Palaces { get; set; }
        public List<Location> Towns { get; set; }
        internal List<Location> Caves { get; set; }
        internal List<Location> Grasses { get; set; }
        internal List<Location> Swamps { get; set; }
        internal List<Location> Bridges { get; set; }
        internal List<Location> Deserts { get; set; }
        internal List<Location> Forests { get; set; }
        internal List<Location> Graves { get; set; }
        internal List<Location> Lavas { get; set; }
        internal List<Location> Roads { get; set; }

        protected SortedDictionary<Tuple<int, int>, string> Section;


        protected World(Hyrule parent)
        {
            _hy = parent;
            Caves = new List<Location>();
            Towns = new List<Location>();
            Palaces = new List<Location>();
            Grasses = new List<Location>();
            Swamps = new List<Location>();
            Bridges = new List<Location>();
            Deserts = new List<Location>();
            Forests = new List<Location>();
            Graves = new List<Location>();
            Lavas = new List<Location>();
            Roads = new List<Location>();
            _connections = new Dictionary<Location, Location>();
            _locations = new List<Location>[11] { Towns, Caves, Palaces, Bridges, Deserts, Grasses, Forests, Swamps, Graves, Roads, Lavas };
            AllLocations = new List<Location>();
            _locsByCoords = new SortedDictionary<Tuple<int, int>, Location>();
            _reachableAreas = new HashSet<string>();
            _visitedEnemies = new List<int>();
        }

        public void AddLocation(Location l)
        {
            if (l.TerrainType == Terrain.WalkableWater)
            {
                _locations[(int)Terrain.Swamp].Add(l);
            }
            else
            {
                _locations[(int)l.TerrainType].Add(l);
            }
            AllLocations.Add(l);
            _locsByCoords.Add(l.Coords, l);
        }

        protected void Swap(Location l1, Location l2)
        {
            var tempX = l1.XPos;
            var tempY = l1.YPos;
            var tempPass = l1.PassThrough;
            l1.XPos = l2.XPos;
            l1.YPos = l2.YPos;
            l1.PassThrough = l2.PassThrough;
            l2.XPos = tempX;
            l2.YPos = tempY;
            l2.PassThrough = tempPass;
        }
        protected void UpdateLocations()
        {
            _locsByCoords = new SortedDictionary<Tuple<int, int>, Location>();
            foreach (var l in AllLocations)
            {
                _locsByCoords.Add(l.Coords, l);
            }
        }

        public void ShuffleEnemies(int addr, bool isOver)
        {
            if (isOver)
            {
                addr += _hy.RomData.GetByte(addr);
            }

            if (_visitedEnemies.Contains(addr) || addr == 0x95A4) return;
            int numBytes = _hy.RomData.GetByte(addr);
            for (var j = addr + 2; j < addr + numBytes; j += 2)
            {
                var enemy = _hy.RomData.GetByte(j) & 0x3F;
                var highPart = _hy.RomData.GetByte(j) & 0xC0;
                if (_hy.Props.MixLargeAndSmallEnemies)
                {
                    if (_enemies.Contains(enemy))
                    {
                        var swap = _enemies[_hy.R.Next(0, _enemies.Count)];
                        _hy.RomData.Put(j, (byte)(swap + highPart));
                        if ((_shorties.Contains(enemy) && _tallGuys.Contains(swap) && swap != 0x20))
                        {
                            var ypos = _hy.RomData.GetByte(j - 1) & 0xF0;
                            var xpos = _hy.RomData.GetByte(j - 1) & 0x0F;
                            ypos -= 32;
                            _hy.RomData.Put(j - 1, (byte)(ypos + xpos));
                        }
                        else if (swap == 0x20 && swap != enemy)
                        {
                            var ypos = _hy.RomData.GetByte(j - 1) & 0xF0;
                            var xpos = _hy.RomData.GetByte(j - 1) & 0x0F;
                            ypos -= 48;
                            _hy.RomData.Put(j - 1, (byte)(ypos + xpos));
                        }
                        else if (enemy == 0x1F && swap != enemy)
                        {
                            var ypos = _hy.RomData.GetByte(j - 1) & 0xF0;
                            var xpos = _hy.RomData.GetByte(j - 1) & 0x0F;
                            ypos -= 16;
                            _hy.RomData.Put(j - 1, (byte)(ypos + xpos));
                        }
                    }
                }
                else
                {
                    if (_tallGuys.Contains(enemy))
                    {
                        var swap = _hy.R.Next(0, _tallGuys.Count);
                        if (_tallGuys[swap] == 0x20 && _tallGuys[swap] != enemy)
                        {
                            var ypos = _hy.RomData.GetByte(j - 1) & 0xF0;
                            var xpos = _hy.RomData.GetByte(j - 1) & 0x0F;
                            ypos -= 48;
                            _hy.RomData.Put(j - 1, (byte)(ypos + xpos));
                        }
                        _hy.RomData.Put(j, (byte)(_tallGuys[swap] + highPart));
                    }

                    if (_shorties.Contains(enemy))
                    {
                        var swap = _hy.R.Next(0, _shorties.Count);
                        _hy.RomData.Put(j, (byte)(_shorties[swap] + highPart));
                    }
                }

                if (_flyingEnemies.Contains(enemy))
                {
                    var swap = _hy.R.Next(0, _flyingEnemies.Count);
                    _hy.RomData.Put(j, (byte)(_flyingEnemies[swap] + highPart));

                    if (_flyingEnemies[swap] == 0x07 || _flyingEnemies[swap] == 0x0a || _flyingEnemies[swap] == 0x0d || _flyingEnemies[swap] == 0x0e)
                    {
                        var ypos = 0x00;
                        var xpos = _hy.RomData.GetByte(j - 1) & 0x0F;
                        _hy.RomData.Put(j - 1, (byte)(ypos + xpos));
                    }
                }

                if (_generators.Contains(enemy))
                {
                    var swap = _hy.R.Next(0, _generators.Count);
                    _hy.RomData.Put(j, (byte)(_generators[swap] + highPart));
                }

                if (enemy != 33) continue;
                {
                    var swap = _hy.R.Next(0, _generators.Count + 1);
                    if (swap != _generators.Count)
                    {
                        _hy.RomData.Put(j, (byte)(_generators[swap] + highPart));
                    }
                }
            }
            _visitedEnemies.Add(addr);
        }

        protected void LoadLocations(int startAddr, int locNum, SortedDictionary<int, Terrain> terrains)
        {
            for (var i = 0; i < locNum; i++)
            {
                var bytes = new byte[4] { _hy.RomData.GetByte(startAddr + i), _hy.RomData.GetByte(startAddr + OverworldXOff + i), _hy.RomData.GetByte(startAddr + OverworldMapOff + i), _hy.RomData.GetByte(startAddr + OverworldWorldOff + i) };
                AddLocation(new Location(bytes, terrains[startAddr + i], startAddr + i));
            }
        }

        protected Location GetLocationByMap(int map, int world)
        {
            return AllLocations.FirstOrDefault(loc => loc.LocationBytes[2] == map && loc.World == world);
        }

        protected Location GetLocationByCoords(Tuple<int, int> coords)
        {
            return AllLocations.FirstOrDefault(loc => loc.Coords.Equals(coords));
        }

        protected Location GetLocationByMem(int mem)
        {
            return AllLocations.FirstOrDefault(loc => loc.MemAddress == mem);
        }

        public void ShuffleE()
        {
            for (var i = _enemyPtr; i < _enemyPtr + 126; i += 2)
            {
                int low = _hy.RomData.GetByte(i);
                int high = _hy.RomData.GetByte(i + 1);
                high <<= 8;
                high &= 0x0FFF;
                ShuffleEnemies(high + low + _enemyAddr, false);
            }

            foreach (var i in _overworldMaps)
            {
                var ptrAddr = _enemyPtr + i * 2;
                int low = _hy.RomData.GetByte(ptrAddr);
                int high = _hy.RomData.GetByte(ptrAddr + 1);
                high <<= 8;
                high &= 0x0FFF;
                ShuffleEnemies(high + low + _enemyAddr, true);
            }
        }

        protected void ShuffleLocations(List<Location> locs)
        {
            for (var i = 0; i < locs.Count; i++)
            {

                var s = _hy.R.Next(i, locs.Count);
                var sl = locs[s];
                if (sl.CanShuffle && locs[i].CanShuffle)
                {
                    Swap(locs[i], locs[s]);
                }
            }
        }

        protected void ChooseConn(string section, Dictionary<Location, Location> co, bool changeType)
        {
            if (co.Count <= 0) return;

            var start = _hy.R.Next(_hy.AreasByLocation[section].Count);
            var s = _hy.AreasByLocation[section][start];
            var conn = _hy.R.Next(co.Count);
            var c = co.Keys.ElementAt(conn);
            var count = 0;
            while ((!c.CanShuffle || !s.CanShuffle || (!changeType && (c.TerrainType != s.TerrainType))) && count < co.Count)
            {
                start = _hy.R.Next(_hy.AreasByLocation[section].Count);
                s = _hy.AreasByLocation[section][start];
                conn = _hy.R.Next(co.Count);
                c = co.Keys.ElementAt(conn);
                count++;
            }
            Swap(s, c);
            c.CanShuffle = false;
        }

        protected void PlaceLocations()
        {
            foreach (var l in AllLocations)
            {
                if (l.TerrainType == Terrain.Bridge || !l.CanShuffle) continue;
                int x;
                int y;
                do
                {
                    x = _hy.R.Next(_mapCols - 2) + 1;
                    y = _hy.R.Next(_mapRows - 2) + 1;
                } while (_map[y, x] != Terrain.None || _map[y - 1, x] != Terrain.None || _map[y + 1, x] != Terrain.None || _map[y + 1, x + 1] != Terrain.None || _map[y, x + 1] != Terrain.None || _map[y - 1, x + 1] != Terrain.None || _map[y + 1, x - 1] != Terrain.None || _map[y, x - 1] != Terrain.None || _map[y - 1, x - 1] != Terrain.None);

                _map[y, x] = l.TerrainType;
                switch (l.TerrainType)
                {
                    case Terrain.Cave:
                    {
                        var dir = _hy.R.Next(4);
                        var s = _walkable[_hy.R.Next(_walkable.Length)];
                        switch (dir)
                        {
                            //south
                            case 0:
                                _map[y + 1, x] = s;
                                _map[y + 1, x + 1] = s;
                                _map[y + 1, x - 1] = s;
                                _map[y, x - 1] = Terrain.Mountain;
                                _map[y, x + 1] = Terrain.Mountain;
                                _map[y - 1, x - 1] = Terrain.Mountain;
                                _map[y - 1, x] = Terrain.Mountain;
                                _map[y - 1, x + 1] = Terrain.Mountain;
                                break;
                            //west
                            case 1:
                                _map[y + 1, x] = Terrain.Mountain;
                                _map[y + 1, x + 1] = Terrain.Mountain;
                                _map[y + 1, x - 1] = s;
                                _map[y, x - 1] = s;
                                _map[y, x + 1] = Terrain.Mountain;
                                _map[y - 1, x - 1] = s;
                                _map[y - 1, x] = Terrain.Mountain;
                                _map[y - 1, x + 1] = Terrain.Mountain;
                                break;
                            //north
                            case 2:
                                _map[y + 1, x] = Terrain.Mountain;
                                _map[y + 1, x + 1] = Terrain.Mountain;
                                _map[y + 1, x - 1] = Terrain.Mountain;
                                _map[y, x - 1] = Terrain.Mountain;
                                _map[y, x + 1] = Terrain.Mountain;
                                _map[y - 1, x - 1] = s;
                                _map[y - 1, x] = s;
                                _map[y - 1, x + 1] = s;
                                break;
                            //east
                            case 3:
                                _map[y + 1, x] = Terrain.Mountain;
                                _map[y + 1, x + 1] = s;
                                _map[y + 1, x - 1] = Terrain.Mountain;
                                _map[y, x - 1] = Terrain.Mountain;
                                _map[y, x + 1] = s;
                                _map[y - 1, x - 1] = Terrain.Mountain;
                                _map[y - 1, x] = Terrain.Mountain;
                                _map[y - 1, x + 1] = s;
                                break;
                        }

                        break;
                    }
                    case Terrain.Palace:
                    {
                        var s = _walkable[_hy.R.Next(_walkable.Length)];
                        _map[y + 1, x] = s;
                        _map[y + 1, x + 1] = s;
                        _map[y + 1, x - 1] = s;
                        _map[y, x - 1] = s;
                        _map[y, x + 1] = s;
                        _map[y - 1, x - 1] = s;
                        _map[y - 1, x] = s;
                        _map[y - 1, x + 1] = s;
                        break;
                    }
                    default:
                    {
                        Terrain t;
                        do
                        {
                            t = _walkable[_hy.R.Next(_walkable.Length)];
                        } while (t == l.TerrainType);
                        _map[y + 1, x] = t;
                        _map[y + 1, x + 1] = t;
                        _map[y + 1, x - 1] = t;
                        _map[y, x - 1] = t;
                        _map[y, x + 1] = t;
                        _map[y - 1, x - 1] = t;
                        _map[y - 1, x] = t;
                        _map[y - 1, x + 1] = t;
                        break;
                    }
                }
                l.XPos = x;
                l.YPos = y + 30;
            }
        }

        protected bool GrowTerrain()
        {
            var mapCopy = new Terrain[_mapRows, _mapCols];
            var placed = new List<Tuple<int, int>>();
            for(var i = 0; i < _mapRows; i++)
            {
                for(var j = 0; j < _mapCols; j++)
                {
                    if (_map[i, j] != Terrain.None && _randomTerrains.Contains(_map[i, j]))
                    {
                        placed.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            for (var i = 0; i < _mapRows; i++)
            {
                for (var j = 0; j < _mapCols; j++)
                {
                    if (_map[i, j] != Terrain.None) continue;
                    var choices = new List<Terrain>();
                    double minDistance = 9999999999999999999;

                    foreach(var t in placed)
                    {
                        double tx = t.Item1 - i;
                        double ty = t.Item2 - j;
                        var distance = Math.Sqrt(tx * tx + ty * ty);
                        if (distance < minDistance)
                        {
                            choices = new List<Terrain>
                            {
                                _map[t.Item1, t.Item2]
                            };
                            minDistance = distance;
                        }
                        else if(distance == minDistance)
                        {
                            choices.Add(_map[t.Item1, t.Item2]);
                        }
                    }
                    mapCopy[i, j] = choices[_hy.R.Next(choices.Count)];
                }
            }

            for (var i = 0; i < _mapRows; i++)
            {
                for (var j = 0; j < _mapCols; j++)
                {
                    if (_map[i, j] != Terrain.None)
                    {
                        mapCopy[i, j] = _map[i, j];
                    }
                }
            }
            _map = (Terrain[,])mapCopy.Clone();
            return true;
        }

        protected void PlaceRandomTerrain(int num)
        {
            //randomly place remaining terrain
            var placed = 0;
            while (placed < num)
            {
                var t = _randomTerrains[_hy.R.Next(_randomTerrains.Length)];
                int x;
                int y;
                do
                {
                    x = _hy.R.Next(_mapCols);
                    y = _hy.R.Next(_mapRows);
                } while (_map[y, x] != Terrain.None);
                _map[y, x] = t;
                placed++;
            }
        }

        protected void WriteBytes(bool doWrite, int loc, int total, int h1, int h2)
        {
            _bcount = 0;
            var curr2 = _map[0, 0];
            var count2 = 0;
            for (var i = 0; i < _mapRows; i++)
            {
                for (var j = 0; j < _mapCols; j++)
                {
                    if(_hy._hiddenPalace && i == h1 && j == h2 && i != 0 && j != 0)
                    {
                        count2--;
                        var b = count2 * 16 + (int)curr2;
                        //Console.WriteLine("Hex: {0:X}", b);
                        if (doWrite)
                        {
                            _hy.RomData.Put(loc, (byte)b);
                            _hy.RomData.Put(loc + 1, (byte)curr2);
                        }
                        count2 = 0;
                        loc += 2;
                        _bcount += 2;
                        continue;
                    }
                    if (_map[i, j] == curr2 && count2 < 16)
                    {
                        count2++;
                    }
                    else
                    {
                        count2--;
                        var b = count2 * 16 + (int)curr2;
                        //Console.WriteLine("Hex: {0:X}", b);
                        if (doWrite)
                        {
                            _hy.RomData.Put(loc, (byte)b);
                        }

                        curr2 = _map[i, j];
                        count2 = 1;
                        loc++;
                        _bcount++;
                    }
                }
                count2--;
                var b2 = count2 * 16 + (int)curr2;
                //Console.WriteLine("Hex: {0:X}", b2);
                if (doWrite)
                {
                    _hy.RomData.Put(loc, (byte)b2);
                }

                if (i < _mapRows - 1)
                {
                    curr2 = _map[i + 1, 0];
                }
                count2 = 0;
                loc++;
                _bcount++;
            }

            while (_bcount < total)
            {
                _hy.RomData.Put(loc, (byte)0x0B);
                _bcount++;
                loc++;
            }
        }

        protected void DrawOcean(bool left)
        {
            var x = 0;
            if (!left)
            {
                x = _mapCols - 1;
            }
            //draw ocean on right side
            var ostart = _hy.R.Next(_mapRows);
            var olength = _hy.R.Next(10, 30);
            if (ostart < _mapRows / 2)
            {
                for (var i = 0; i < olength; i++)
                {
                    _map[ostart + i, x] = Terrain.WalkableWater;
                }
            }
            else
            {
                for (var i = 0; i < olength; i++)
                {
                    _map[ostart - i, x] = Terrain.WalkableWater;
                }
            }
        }
        protected void UpdateReachable()
        {
            var changed = true;
            while (changed)
            {
                changed = false;
                for (var i = 0; i < _mapRows; i++)
                {
                    for (var j = 0; j < _mapCols; j++)
                    {
                        if (_v[i, j] || (_map[i, j] != Terrain.Lava && _map[i, j] != Terrain.Bridge &&
                                        _map[i, j] != Terrain.Cave && _map[i, j] != Terrain.Road &&
                                        _map[i, j] != Terrain.Palace && _map[i, j] != Terrain.Town &&
                                        (_map[i, j] != Terrain.WalkableWater || !_hy._itemGet[(int) Items.Boots]) &&
                                        !_walkable.Contains(_map[i, j]) &&
                                        (_map[i, j] != Terrain.Rock || !_hy._itemGet[(int) Items.Hammer]) &&
                                        (_map[i, j] != Terrain.Spider || !_hy._itemGet[(int) Items.Horn]))) continue;
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

                        if (j + 1 >= _mapCols) continue;
                        if (!_v[i, j + 1]) continue;
                        _v[i, j] = true;
                        changed = true;
                    }
                }
            }
        }

        public void Reset()
        {
            for(var i = 0; i < _mapRows; i++)
            {
                for(var j = 0; j < _mapCols; j++)
                {
                    _v[i, j] = false;
                }
            }
        }
    }
}
