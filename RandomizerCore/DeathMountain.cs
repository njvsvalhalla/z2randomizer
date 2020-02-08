using System;
using System.Collections.Generic;
using System.Linq;

namespace Z2Randomizer
{
    internal class DeathMountain : World
    {
        private new readonly Terrain[] _walkable = { Terrain.Desert, Terrain.Forest, Terrain.Grave };
        private new readonly Terrain[] _randomTerrains = { Terrain.Desert, Terrain.Forest, Terrain.Grave, Terrain.Mountain, Terrain.WalkableWater };

        private readonly SortedDictionary<int, Terrain> _terrains = new SortedDictionary<int, Terrain>
            {
                { 0x610C, Terrain.Cave },
                { 0x610D, Terrain.Cave },
                { 0x610E, Terrain.Cave },
                { 0x610F, Terrain.Cave },
                { 0x6110, Terrain.Cave },
                { 0x6111, Terrain.Cave },
                { 0x6112, Terrain.Cave },
                { 0x6113, Terrain.Cave },
                { 0x6114, Terrain.Cave },
                { 0x6115, Terrain.Cave },
                { 0x6116, Terrain.Cave },
                { 0x6117, Terrain.Cave },
                { 0x6118, Terrain.Cave },
                { 0x6119, Terrain.Cave },
                { 0x611A, Terrain.Cave },
                { 0x611B, Terrain.Cave },
                { 0x611C, Terrain.Cave },
                { 0x611D, Terrain.Cave },
                { 0x611E, Terrain.Cave },
                { 0x611F, Terrain.Cave },
                { 0x6120, Terrain.Cave },
                { 0x6121, Terrain.Cave },
                { 0x6122, Terrain.Cave },
                { 0x6123, Terrain.Cave },
                { 0x6124, Terrain.Cave },
                { 0x6125, Terrain.Cave },
                { 0x6126, Terrain.Cave },
                { 0x6127, Terrain.Cave },
                { 0x6128, Terrain.Cave },
                { 0x6129, Terrain.Cave },
                { 0x612A, Terrain.Cave },
                { 0x612B, Terrain.Cave },
                { 0x612C, Terrain.Cave },
                { 0x612D, Terrain.Cave },
                { 0x612E, Terrain.Cave },
                { 0x612F, Terrain.Cave },
                { 0x6130, Terrain.Cave },
                { 0x6136, Terrain.Cave },
                { 0x6137, Terrain.Cave },
                { 0x6144, Terrain.Cave }
        };

        public Dictionary<Location, List<Location>> _connectionsDm;
        public Location _hammerCave;
        public Location _magicCave;
        public Location _hammerEnter;
        public Location _hammerExit;

        public DeathMountain(Hyrule hy)
            : base(hy)
        {
            LoadLocations(0x610C, 37, _terrains);
            LoadLocations(0x6136, 2, _terrains);
            LoadLocations(0x6144, 1, _terrains);

            _hammerCave = GetLocationByMem(0x6128);
            _hammerEnter = GetLocationByMem(0x6136);
            _hammerExit = GetLocationByMem(0x6137);
            _magicCave = GetLocationByMem(0x6144);

            _reachableAreas = new HashSet<string>();
            _connectionsDm = new Dictionary<Location, List<Location>>
            {
                {GetLocationByMem(0x610C), new List<Location>() {GetLocationByMem(0x610D)}},
                {GetLocationByMem(0x610D), new List<Location>() {GetLocationByMem(0x610C)}},
                {GetLocationByMem(0x610E), new List<Location>() {GetLocationByMem(0x610F)}},
                {GetLocationByMem(0x610F), new List<Location>() {GetLocationByMem(0x610E)}},
                {GetLocationByMem(0x6110), new List<Location>() {GetLocationByMem(0x6111)}},
                {GetLocationByMem(0x6111), new List<Location>() {GetLocationByMem(0x6110)}},
                {GetLocationByMem(0x6112), new List<Location>() {GetLocationByMem(0x6113)}},
                {GetLocationByMem(0x6113), new List<Location>() {GetLocationByMem(0x6112)}},
                {GetLocationByMem(0x6114), new List<Location>() {GetLocationByMem(0x6115)}},
                {GetLocationByMem(0x6115), new List<Location>() {GetLocationByMem(0x6114)}},
                {GetLocationByMem(0x6116), new List<Location>() {GetLocationByMem(0x6117)}},
                {GetLocationByMem(0x6117), new List<Location>() {GetLocationByMem(0x6116)}},
                {GetLocationByMem(0x6118), new List<Location>() {GetLocationByMem(0x6119)}},
                {GetLocationByMem(0x6119), new List<Location>() {GetLocationByMem(0x6118)}},
                {GetLocationByMem(0x611A), new List<Location>() {GetLocationByMem(0x611B)}},
                {GetLocationByMem(0x611B), new List<Location>() {GetLocationByMem(0x611A)}},
                {GetLocationByMem(0x611C), new List<Location>() {GetLocationByMem(0x611D)}},
                {GetLocationByMem(0x611D), new List<Location>() {GetLocationByMem(0x611C)}},
                {GetLocationByMem(0x611E), new List<Location>() {GetLocationByMem(0x611F)}},
                {GetLocationByMem(0x611F), new List<Location>() {GetLocationByMem(0x611E)}},
                {GetLocationByMem(0x6120), new List<Location>() {GetLocationByMem(0x6121)}},
                {GetLocationByMem(0x6121), new List<Location>() {GetLocationByMem(0x6120)}},
                {GetLocationByMem(0x6122), new List<Location>() {GetLocationByMem(0x6123)}},
                {GetLocationByMem(0x6123), new List<Location>() {GetLocationByMem(0x6122)}},
                {GetLocationByMem(0x6124), new List<Location>() {GetLocationByMem(0x6125)}},
                {GetLocationByMem(0x6125), new List<Location>() {GetLocationByMem(0x6124)}},
                {GetLocationByMem(0x6126), new List<Location>() {GetLocationByMem(0x6127)}},
                {GetLocationByMem(0x6127), new List<Location>() {GetLocationByMem(0x6126)}},
                {
                    GetLocationByMem(0x6129),
                    new List<Location>()
                    {
                        GetLocationByMem(0x612A), GetLocationByMem(0x612B), GetLocationByMem(0x612C)
                    }
                },
                {
                    GetLocationByMem(0x612D),
                    new List<Location>()
                    {
                        GetLocationByMem(0x612E), GetLocationByMem(0x612F), GetLocationByMem(0x6130)
                    }
                },
                {
                    GetLocationByMem(0x612E),
                    new List<Location>()
                    {
                        GetLocationByMem(0x612D), GetLocationByMem(0x612F), GetLocationByMem(0x6130)
                    }
                },
                {
                    GetLocationByMem(0x612F),
                    new List<Location>()
                    {
                        GetLocationByMem(0x612E), GetLocationByMem(0x612D), GetLocationByMem(0x6130)
                    }
                },
                {
                    GetLocationByMem(0x6130),
                    new List<Location>()
                    {
                        GetLocationByMem(0x612E), GetLocationByMem(0x612F), GetLocationByMem(0x612D)
                    }
                }
            };

            _enemies = new List<int> { 3, 4, 5, 17, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 31, 32 };
            _flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E };
            _generators = new List<int> { 11, 12, 15, 29 };
            _shorties = new List<int> { 3, 4, 5, 17, 18, 0x1C, 0x1F };
            _tallGuys = new List<int> { 0x20, 20, 21, 22, 23, 24, 25, 26, 27 };
            _enemyAddr = 0x48B0;
            _enemyPtr = 0x608E;

            _overworldMaps = new List<int>();
            _mapRows = 45;
            _mapCols = 64;
    }

        public bool Terraform()
        {
            _bcount = 900;
            while (_bcount > 801)
            {
                _map = new Terrain[_mapRows, _mapCols];

                for (var i = 0; i < _mapRows; i++)
                {
                    for (var j = 0; j < 29; j++)
                    {
                        _map[i, j] = Terrain.None;
                    }
                    for (var j = 29; j < _mapCols; j++)
                    {
                        _map[i, j] = Terrain.WalkableWater;
                    }
                }

                int x;
                int y;
                foreach (var l in AllLocations.Where(l => l.TerrainType != Terrain.Bridge && l != _magicCave))
                {
                    do
                    {
                        x = _hy.R.Next(_mapCols - 2) + 1;
                        y = _hy.R.Next(_mapRows - 2) + 1;
                    } while (_map[y, x] != Terrain.None || _map[y - 1, x] != Terrain.None || _map[y + 1, x] != Terrain.None || _map[y + 1, x + 1] != Terrain.None || _map[y, x + 1] != Terrain.None || _map[y - 1, x + 1] != Terrain.None || _map[y + 1, x - 1] != Terrain.None || _map[y, x - 1] != Terrain.None || _map[y - 1, x - 1] != Terrain.None);

                    _map[y, x] = l.TerrainType;
                    l.XPos = x;
                    l.YPos = y + 30;
                    if (l.TerrainType != Terrain.Cave) continue;
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
                }

                if (!GrowTerrain())
                {
                    return false;
                }
                
                do
                {
                    x = _hy.R.Next(_mapCols - 2) + 1;
                    y = _hy.R.Next(_mapRows - 2) + 1;
                } while (!_walkable.Contains(_map[y, x]) || _map[y + 1, x] == Terrain.Cave || _map[y - 1, x] == Terrain.Cave || _map[y, x + 1] == Terrain.Cave || _map[y, x - 1] == Terrain.Cave);

                _map[y, x] = Terrain.Rock;
                _magicCave.YPos = y + 30;
                _magicCave.XPos = x;

                //check bytes and adjust
                WriteBytes(false, 0x665C, 801, 0, 0);
            }
            WriteBytes(true, 0x665C, 801, 0, 0);

            var loc = 0x7C00 + _bcount;
            var high = (loc & 0xFF00) >> 8;
            var low = loc & 0xFF;

            _hy.RomData.Put(0x47A5, (Byte)low);
            _hy.RomData.Put(0x47A6, (Byte)high);

            _v = new bool[_mapRows, _mapCols];
            for (var i = 0; i < _mapRows; i++)
            {
                for (var j = 0; j < _mapCols; j++)
                {
                    _v[i, j] = false;
                }
            }

            for (var i = 0x610C; i < 0x6149; i++)
            {
                if (!_terrains.Keys.Contains(i))
                {
                    _hy.RomData.Put(i, 0x00);
                }
            }
            return true;
        }

        public void UpdateVisit()
        {
            UpdateReachable();

            foreach (var l in AllLocations.Where(l => _v[l.YPos - 30, l.XPos]))
            {
                l.Reachable = true;
                if (!_connectionsDm.Keys.Contains(l)) continue;
                var l2 = _connectionsDm[l];

                foreach(var l3 in l2)
                { 
                    l3.Reachable = true;
                    _v[l3.YPos - 30, l3.XPos] = true;
                }
            }
        }

        public void SetStart()
        {
            _v[_hammerEnter.YPos - 30, _hammerEnter.XPos] = true;
        }

        public void SetStart2()
        {
            _v[_hammerExit.YPos - 30, _hammerExit.XPos] = true;
        }
    }
}
