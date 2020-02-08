using System;
using System.Collections.Generic;
using System.Linq;


namespace Z2Randomizer
{
    internal class WestHyrule : World
    {
        public Location _start;
        public Location _hammerEnter;
        public Location _hammerExit;
        public Location _fairy;
        public Location _bagu;
        public Location _jump;
        public Location _medicineCave;
        public Location _trophyCave;
        public Location _raftSpot;
        public Location _palaceOne;
        public Location _palaceTwo;
        public Location _palaceThree;
        public Location _jar;
        public Location _heartOne;
        public Location _heartTwo;
        public Location _lifeNorth;
        public Location _lifeSouth;
        public Location _shieldTown;
        public Location _bridgeOne;
        public Location _bridgeTwo;
        public Location _pbagCave;
        private int _bridgeCount;

        private readonly Dictionary<Location, Location> _bridgeConn;
        private readonly Dictionary<Location, Location> _cityConn;
        private readonly Dictionary<Location, Location> _caveConn;
        private readonly Dictionary<Location, Location> _graveConn;

        private readonly SortedDictionary<int, Terrain> _terrains = new SortedDictionary<int, Terrain>
            {
                { 0x462F, Terrain.Palace},
                { 0x4630,  Terrain.Cave },
                { 0x4631, Terrain.Forest},
                { 0x4632, Terrain.Cave },
                { 0x4633, Terrain.Forest },
                { 0x4634, Terrain.Grass },
                { 0x4635, Terrain.Forest },
                { 0x4636, Terrain.Road },
                { 0x4637, Terrain.Swamp },
                { 0x4638, Terrain.Grave },
                { 0x4639, Terrain.Cave },
                { 0x463A, Terrain.Cave },
                { 0x463B, Terrain.Cave },
                { 0x463C, Terrain.Cave },
                { 0x463D, Terrain.Cave },
                { 0x463E, Terrain.Cave },
                { 0x463F, Terrain.Cave },
                { 0x4640, Terrain.Grave },
                { 0x4641, Terrain.Cave },
                { 0x4642, Terrain.Bridge },
                { 0x4643, Terrain.Bridge },
                { 0x4644, Terrain.Bridge },
                { 0x4645, Terrain.Bridge },
                { 0x4646, Terrain.Forest },
                { 0x4647, Terrain.Swamp },
                { 0x4648, Terrain.Forest },
                { 0x4649, Terrain.Forest },
                { 0x464A, Terrain.Forest },
                { 0x464B, Terrain.Forest },
                { 0x464C, Terrain.Forest },
                { 0x464D, Terrain.Road },
                //{ 0x464E, terrain.desert },
                { 0x464F, Terrain.Desert },
                { 0x4658, Terrain.Bridge },
                { 0x4659, Terrain.Cave },
                { 0x465A, Terrain.Cave },
                { 0x465B, Terrain.Grave },
                { 0x465C, Terrain.Town },
                { 0x465E, Terrain.Town },
                { 0x465F, Terrain.Town },
                { 0x4660, Terrain.Town },
                { 0x4661, Terrain.Forest },
                { 0x4662, Terrain.Town },
                { 0x4663, Terrain.Palace },
                { 0x4664, Terrain.Palace },
                { 0x4665, Terrain.Palace }
        };

        public WestHyrule(Hyrule hy)
            : base(hy)
        {
            LoadLocations(0x462F, 31, _terrains);
            LoadLocations(0x464F, 1, _terrains);
            LoadLocations(0x4658, 5, _terrains);
            LoadLocations(0x465E, 8, _terrains);
            _start = GetLocationByMap(0x80, 0x00);
            _reachableAreas = new HashSet<string>();
            _hammerEnter = GetLocationByMap(0x2A, 1);
            _hammerExit = GetLocationByMap(0x2B, 1);
            var jumpCave = GetLocationByMap(9, 0);
            jumpCave.NeedJump = true;
            _medicineCave = GetLocationByMap(0x0E, 0);
            var heartCave = GetLocationByMap(0x10, 0);
            var fairyCave = GetLocationByMap(0x12, 0);
            fairyCave.NeedFairy = true;
            _jump = GetLocationByMap(0xC5, 4);
            _bagu = GetLocationByMap(0x18, 4);
            _fairy = GetLocationByMap(0xCB, 4);
            _lifeNorth = GetLocationByMap(0xC8, 4);
            _lifeSouth = GetLocationByMap(0x06, 4);
            _lifeNorth.NeedBagu = true;
            _lifeSouth.NeedBagu = true;
            _trophyCave = GetLocationByMap(0xE1, 0);
            _raftSpot = GetLocationByMem(0x4658);
            _palaceOne = GetLocationByMem(0x4663);
            _palaceOne.PalNum = 1;
            _palaceTwo = GetLocationByMem(0x4664);
            _palaceTwo.PalNum = 2;
            _palaceThree = GetLocationByMem(0x4665);
            _palaceThree.PalNum = 3;
            _jar = GetLocationByMem(0x4632);
            _heartOne = GetLocationByMem(0x463F);
            _heartTwo = GetLocationByMem(0x4634);
            _shieldTown = GetLocationByMem(0x465C);
            _pbagCave = GetLocationByMem(0x463D);


            var parapaCaveOne = GetLocationByMap(07, 0);
            var parapaCaveTwo = GetLocationByMap(0xC7, 0);
            var jumpCaveTwo = GetLocationByMap(0xCB, 0);
            var fairyCaveTwo = GetLocationByMap(0xD3, 0);
            _bridgeOne = GetLocationByMap(0x04, 0);
            _bridgeTwo = GetLocationByMap(0xC5, 0);

            _caveConn = new Dictionary<Location, Location>();
            _bridgeConn = new Dictionary<Location, Location>();
            _cityConn = new Dictionary<Location, Location>();
            _graveConn = new Dictionary<Location, Location>();

            //connections.Add(hammerEnter, hammerExit);
            //connections.Add(hammerExit, hammerEnter);
            //caveConn.Add(hammerEnter, hammerExit);
            //caveConn.Add(hammerExit, hammerEnter);
            _connections.Add(parapaCaveOne, parapaCaveTwo);
            _connections.Add(parapaCaveTwo, parapaCaveOne);
            _caveConn.Add(parapaCaveOne, parapaCaveTwo);
            _caveConn.Add(parapaCaveTwo, parapaCaveOne);
            _connections.Add(jumpCave, jumpCaveTwo);
            _connections.Add(jumpCaveTwo, jumpCave);
            _caveConn.Add(jumpCave, jumpCaveTwo);
            _caveConn.Add(jumpCaveTwo, jumpCave);
            _connections.Add(fairyCave, fairyCaveTwo);
            _connections.Add(fairyCaveTwo, fairyCave);
            _caveConn.Add(fairyCaveTwo, fairyCave);
            _graveConn.Add(fairyCave, fairyCaveTwo);
            _connections.Add(_lifeNorth, _lifeSouth);
            _connections.Add(_lifeSouth, _lifeNorth);
            _cityConn.Add(_lifeSouth, _lifeNorth);
            _cityConn.Add(_lifeNorth, _lifeSouth);
            _connections.Add(_bridgeOne, _bridgeTwo);
            _connections.Add(_bridgeTwo, _bridgeOne);
            _bridgeConn.Add(_bridgeOne, _bridgeTwo);
            _bridgeConn.Add(_bridgeTwo, _bridgeOne);

            _enemies = new List<int> { 3, 4, 5, 17, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 31, 32 };
            _flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E };
            _generators = new List<int> { 11, 12, 15, 29 };
            _shorties = new List<int> { 3, 4, 5, 17, 18, 0x1C, 0x1F };
            _tallGuys = new List<int> { 0x20, 20, 21, 22, 23, 24, 25, 26, 27 };
            _enemyAddr = 0x48B0;
            _enemyPtr = 0x45B1;

            _overworldMaps = new List<int>() { 0x22, 0x1D, 0x27, 0x30, 0x23, 0x3A, 0x1E, 0x35, 0x28 };
            _mapRows = 75;
            _mapCols = 64;
    }

        public bool Terraform()
        {
            _bcount = 900;
            while (_bcount > 799)
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
                DrawMountains();
                DrawBridge();
                DrawRiver();
               
                DrawOcean(false);
                PlaceLocations();
                PlaceRocks();
                PlaceRandomTerrain(5);

                if(!GrowTerrain())
                {
                    return false;
                }

                DrawRaft();

                //check bytes and adjust
                WriteBytes(false, 0x506C, 799, 0, 0);
                Console.WriteLine("West:" + _bcount);
            }
            WriteBytes(true, 0x506C, 800, 0, 0);

            var loc3 = 0x7C00 + _bcount;
            var high = (loc3 & 0xFF00) >> 8;
            var low = loc3 & 0xFF;

            _hy.RomData.Put(0x479F, (byte)low);
            _hy.RomData.Put(0x47A0, (byte)high);
            _hy.RomData.Put(0x47A1, (byte)low);
            _hy.RomData.Put(0x47A2, (byte)high);
            _hy.RomData.Put(0x47A3, (byte)low);
            _hy.RomData.Put(0x47A4, (byte)high);

            _v = new bool[_mapRows, _mapCols];
            for(var i = 0; i < _mapRows; i++)
            {
                for(var j = 0; j < _mapCols; j++)
                {
                    _v[i, j] = false;
                }
            }

            _v[_start.YPos - 30, _start.XPos] = true;
            return true;
        }

        public void SetStart()
        {
            _v[_start.YPos - 30, _start.XPos] = true;
        }

        private void PlaceRocks()
        {
            var rockNum = _hy.R.Next(3);
            while (rockNum > 0)
            {
                var cave = Caves[_hy.R.Next(Caves.Count)];
                if (_connections.Keys.Contains(cave) || cave == _hammerEnter || cave == _hammerExit) continue;
                if(_map[cave.YPos - 30, cave.XPos - 1] != Terrain.Mountain)
                {
                    _map[cave.YPos - 30, cave.XPos - 1] = Terrain.Rock;
                    rockNum--;
                }
                else if (_map[cave.YPos - 30, cave.XPos + 1] != Terrain.Mountain)
                {
                    _map[cave.YPos - 30, cave.XPos + 1] = Terrain.Rock;
                    rockNum--;
                }
                else if (_map[cave.YPos - 29, cave.XPos] != Terrain.Mountain)
                {
                    _map[cave.YPos - 29, cave.XPos] = Terrain.Rock;
                    rockNum--;
                }
                else if (_map[cave.YPos - 31, cave.XPos] != Terrain.Mountain)
                {
                    _map[cave.YPos - 31, cave.XPos] = Terrain.Rock;
                    rockNum--;
                }
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
            switch (dirr2)
            {
                //start north
                case 0:
                {
                    var startx = _hy.R.Next(_mapCols);
                    lr2 = new Location
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
                    lr2 = new Location
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
                    lr2 = new Location
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
                    lr2 = new Location
                    {
                        YPos = startx + 30,
                        XPos = 0
                    };
                    break;
                }
            }

            _map[lr.YPos - 30, lr.XPos] = Terrain.WalkableWater;
            _map[lr2.YPos - 30, lr.XPos] = Terrain.WalkableWater;
            DrawLine(lr, lr2, Terrain.WalkableWater, _walkable[_hy.R.Next(_walkable.Length)]);

            if (_bridgeCount < 2)
            {
                var b = GetLocationByMem(0x4643);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }

            if (_bridgeCount >= 1) return;
            {
                var b = GetLocationByMem(0x4642);
                b.XPos = 0;
                b.YPos = 0;
                b.Reachable = true;
                b.CanShuffle = false;
            }
        }

        private void DrawBridge()
        {
            var bridgex = _hy.R.Next(1, _mapCols - 10);
            var bridgey = _hy.R.Next(1, _mapRows - 10) + 30;

            _bridgeOne.XPos = bridgex;
            _bridgeOne.YPos = bridgey;
            var wt = _walkable[_hy.R.Next(_walkable.Length)];
            var wt2 = _walkable[_hy.R.Next(_walkable.Length)];

            int bdiff;
            if (_hy.R.NextDouble() > .5)
            {

                bdiff = _hy.R.Next(6, 9);
                _map[_bridgeOne.YPos - 1 - 30, _bridgeOne.XPos] = wt;
                _map[_bridgeOne.YPos - 1 - 30, _bridgeOne.XPos - 1] = wt;
                _map[_bridgeOne.YPos - 1 - 30, _bridgeOne.XPos + 1] = wt;
                _map[_bridgeOne.YPos + 1 + bdiff - 30, _bridgeOne.XPos] = wt2;
                _map[_bridgeOne.YPos + 1 + bdiff - 30, _bridgeOne.XPos - 1] = wt2;
                _map[_bridgeOne.YPos + 1 + bdiff - 30, _bridgeOne.XPos + 1] = wt2;

                _bridgeTwo.XPos = _bridgeOne.XPos;
                _bridgeTwo.YPos = _bridgeOne.YPos + bdiff;
            }
            else
            {

                bdiff = _hy.R.Next(6, 9);
                _map[_bridgeOne.YPos - 30, _bridgeOne.XPos - 1] = wt;
                _map[_bridgeOne.YPos - 30 - 1, _bridgeOne.XPos - 1] = wt;
                _map[_bridgeOne.YPos - 30 + 1, _bridgeOne.XPos - 1] = wt;
                _map[_bridgeOne.YPos - 30, _bridgeOne.XPos + bdiff + 1] = wt2;
                _map[_bridgeOne.YPos - 30 - 1, _bridgeOne.XPos + bdiff + 1] = wt2;
                _map[_bridgeOne.YPos - 30 + 1, _bridgeOne.XPos + bdiff + 1] = wt2;

                _bridgeTwo.XPos = _bridgeOne.XPos + bdiff;
                _bridgeTwo.YPos = _bridgeOne.YPos;
            }
            _map[_bridgeOne.YPos - 30, _bridgeOne.XPos] = Terrain.Bridge;
            _map[_bridgeTwo.YPos - 30, _bridgeTwo.XPos] = Terrain.Bridge;
            DrawLine(_bridgeTwo, _bridgeOne, Terrain.Bridge, Terrain.WalkableWater);
            _bridgeTwo.CanShuffle = false;
            _bridgeOne.CanShuffle = false;
        }

        private void DrawMountains()
        {
            //create some mountains
            var mounty = _hy.R.Next(22, 42);
            _map[mounty, 0] = Terrain.Mountain;
            var placedRoad = false;

            var endmounty = _hy.R.Next(22, 42);
            var endmountx = _hy.R.Next(2, 8);
            var x2 = 0;
            var y2 = mounty;
            var placedRocks = 0;
            while (x2 != (_mapCols - endmountx) || y2 != endmounty)
            {
                if (Math.Abs(x2 - (_mapCols - endmountx)) >= Math.Abs(y2 - endmounty))
                {
                    if (x2 > _mapCols - endmountx && x2 > 0)
                    {
                        x2--;
                    }
                    else if (x2 < _mapCols - 1)
                    {
                        x2++;
                    }
                }
                else
                {
                    if (y2 > endmounty && y2 > 0)
                    {
                        y2--;
                    }
                    else if (y2 < _mapRows - 1)
                    {
                        y2++;
                    }
                }
                if (x2 != _mapCols - endmountx || y2 != endmounty)
                {
                    if (_map[y2, x2] == Terrain.None)
                    {
                        _map[y2, x2] = Terrain.Mountain;
                    }
                    else
                    {
                        if (!placedRoad && _map[y2, x2 + 1] != Terrain.Road)
                        {
                            if (_hy.R.NextDouble() > .5 && (x2 > 0 && _map[y2, x2 - 1] != Terrain.Rock) && (x2 < _mapCols - 1 && _map[y2, x2 + 1] != Terrain.Rock) && (((y2 > 0 && _map[y2 - 1, x2] == Terrain.Road) && (y2 < _mapRows - 1 && _map[y2 + 1, x2] == Terrain.Road)) || ((x2 > 0 && _map[y2, x2 - 0] == Terrain.Road) && (x2 < _mapCols - 1 && _map[y2, x2 + 1] == Terrain.Road))))
                            {
                                var roadEnc = GetLocationByMem(0x4636);
                                roadEnc.XPos = x2;
                                roadEnc.YPos = y2 + 30;
                                roadEnc.CanShuffle = false;
                                roadEnc.Reachable = true;
                                placedRoad = true;
                            }
                            else if (placedRocks < 1)
                            {
                                var roadEnc = GetLocationByMem(0x4636);
                                if ((roadEnc.YPos - 30 != y2 && roadEnc.XPos - 1 != x2) && (roadEnc.YPos - 30 + 1 != y2 && roadEnc.XPos != x2) && (roadEnc.YPos - 30 - 1 != y2 && roadEnc.XPos != x2) && (roadEnc.YPos - 30 != y2 && roadEnc.XPos + 1 != x2))
                                {
                                    _map[y2, x2] = Terrain.Rock;
                                    placedRocks++;
                                }
                            }
                        }
                        else if (placedRocks < 1)
                        {

                            _map[y2, x2] = Terrain.Rock;
                            placedRocks++;
                        }
                    }
                }
            }

            if (!placedRoad)
            {
                var roadEnc = GetLocationByMem(0x4636);
                roadEnc.XPos = 0;
                roadEnc.YPos = 0;
                roadEnc.CanShuffle = false;
            }
        }

        private void DrawRoad()
        {
            var roadType = _hy.R.Next(3);
            switch (roadType)
            {
                //two vertical
                case 0:
                {
                    var rl = new Location
                    {
                        XPos = _hy.R.Next(_mapCols / 2),
                        YPos = _hy.R.Next(_mapRows / 2) + 30
                    };
                    _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                    var r2 = new Location
                    {
                        XPos = _hy.R.Next(_mapCols / 2),
                        YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30
                    };
                    _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                    DrawLine(rl, r2, Terrain.Road);
                    rl.XPos = _hy.R.Next(_mapCols / 2, _mapCols);
                    rl.YPos = _hy.R.Next(_mapRows / 2) + 30;
                    r2.XPos = _hy.R.Next(_mapCols / 2, _mapCols);
                    r2.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                    _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                    _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                    DrawLine(rl, r2, Terrain.Road);
                    break;
                }
                //two horizontal
                case 1:
                {
                    var rl = new Location
                    {
                        XPos = _hy.R.Next(_mapCols / 2),
                        YPos = _hy.R.Next(_mapRows / 2) + 30
                    };
                    _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                    var r2 = new Location
                    {
                        XPos = _hy.R.Next(_mapCols / 2, _mapCols),
                        YPos = _hy.R.Next(_mapRows / 2) + 30
                    };
                    _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                    DrawLine(rl, r2, Terrain.Road);
                    rl.XPos = _hy.R.Next(_mapCols / 2);
                    rl.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                    r2.XPos = _hy.R.Next(_mapCols / 2, _mapCols);
                    r2.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                    _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                    _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                    DrawLine(rl, r2, Terrain.Road);
                    break;
                }
                //one of each
                default:
                {
                    if(_hy.R.Next() > .5) //north horizontal
                    {
                        var rl = new Location
                        {
                            XPos = _hy.R.Next(_mapCols / 2),
                            YPos = _hy.R.Next(_mapRows / 2) + 30
                        };
                        _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                        var r2 = new Location
                        {
                            XPos = _hy.R.Next(_mapCols / 2, _mapCols),
                            YPos = _hy.R.Next(_mapRows / 2) + 30
                        };
                        _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                        DrawLine(rl, r2, Terrain.Road);
                    }
                    else
                    {
                        var rl = new Location();
                        var r2 = new Location();
                        _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                        DrawLine(rl, r2, Terrain.Road);
                        rl.XPos = _hy.R.Next(_mapCols / 2);
                        rl.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                        r2.XPos = _hy.R.Next(_mapCols / 2, _mapCols);
                        r2.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                        _map[rl.YPos - 30, rl.XPos] = Terrain.Road;
                        _map[r2.YPos - 30, r2.XPos] = Terrain.Road;
                        DrawLine(rl, r2, Terrain.Road);
                    }
                    var r3 = new Location();
                    var r4 = new Location();
                    r3.XPos = _hy.R.Next(_mapCols);
                    r3.YPos = _hy.R.Next(_mapRows / 2) + 30;
                    r4.XPos = _hy.R.Next(_mapCols);
                    r4.YPos = _hy.R.Next(_mapRows / 2, _mapRows) + 30;
                    _map[r3.YPos - 30, r3.XPos] = Terrain.Road;
                    _map[r4.YPos - 30, r4.XPos] = Terrain.Road;
                    DrawLine(r3, r4, Terrain.Road);
                    break;
                }
            }
        }

        public void UpdateVisit()
        {
            UpdateReachable();

            foreach (var l in AllLocations.Where(l => l.YPos > 30).Where(l => _v[l.YPos - 30, l.XPos]))
            {
                l.Reachable = true;
                if (!_connections.Keys.Contains(l)) continue;
                var l2 = _connections[l];
                if ((l.NeedBagu && (_bagu.Reachable || _hy._spellGet[(int)Spells.Fairy])))
                {
                    l2.Reachable = true;
                    _v[l2.YPos - 30, l2.XPos] = true;
                }

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

                if (l.NeedFairy || l.NeedBagu || l.NeedJump) continue;
                l2.Reachable = true;
                _v[l2.YPos - 30, l2.XPos] = true;
            }
        }

        private void DrawLine(Location to, Location from, Terrain t)
        {
            var x = from.XPos;
            var y = from.YPos - 30;
            while(x != to.XPos || y != (to.YPos - 30))
            {
                if (_hy.R.NextDouble() > .5 && x != to.XPos)
                {
                    var diff = to.XPos - x;
                    var move = _hy.R.Next(Math.Abs(diff) + 1);
                    while (Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if ((x != to.XPos || y != (to.YPos - 30)))
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
                        if ((x != to.XPos || y != (to.YPos - 30)))
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
                    while(Math.Abs(move) > 0 && !(x == to.XPos && y == to.YPos - 30))
                    {
                        if (t == Terrain.WalkableWater && _map[y, x] == Terrain.Road)
                        {
                            if (_bridgeCount == 0 && move > 1 && (diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x4642);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                _map[y, x] = Terrain.Bridge;
                                b.CanShuffle = false;
                            }
                            else if (_bridgeCount == 1 && move > 1 && (diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x4643);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                _map[y, x] = Terrain.Bridge;
                                b.CanShuffle = false;
                            }
                            else if((diff > 0 && (_map[y, x + 1] == Terrain.None || _map[y, x + 1] == Terrain.Mountain)) || (diff < 0 && (_map[y, x - 1] == Terrain.None || _map[y, x - 1] == Terrain.Mountain)))
                            {
                                _map[y, x] = Terrain.Bridge;
                            }
                            else 
                            {
                                _map[y, x] = t;
                            }
                        }
                        else if(_map[y, x] != Terrain.Bridge)
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
                        if (t == Terrain.WalkableWater && _map[y, x] == Terrain.Road)
                        {
                            if (_bridgeCount == 0 && move > 1 && (diff > 0 && (_map[y+ 1, x] == Terrain.None || _map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && (_map[y - 1, x] == Terrain.None || _map[y - 1, x] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x4642);
                                b.XPos = x;
                                b.YPos = y + 30;
                                _bridgeCount++;
                                b.CanShuffle = false;
                                _map[y, x] = Terrain.Bridge;
                            }
                            else if (_bridgeCount == 1 && move > 1 && (diff > 0 && (_map[y + 1, x] == Terrain.None || _map[y + 1, x] == Terrain.Mountain)) || (diff < 0 && (_map[y - 1, x] == Terrain.None || _map[y - 1, x] == Terrain.Mountain)))
                            {
                                var b = GetLocationByMem(0x4643);
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
                        else if(y > 0)
                        {
                            y--;
                        }
                        move--;
                    }
                }
            }
        }

        private void DrawRaft()
        {
            var rafty = _hy.R.Next(0, _mapRows);
            var raftx = _mapCols - 1;
            while (_map[rafty, raftx] != Terrain.WalkableWater)
            {
                rafty = _hy.R.Next(0, _mapRows);
            }
            while (_map[rafty, raftx] == Terrain.WalkableWater && raftx > 0)
            {
                raftx--;
            }

            var tries = 0;
            while (!_walkable.Contains(_map[rafty, raftx]))
            {
                tries++;
                if(tries > 100)
                {
                    Terraform();
                    return;
                }
                rafty = _hy.R.Next(0, _mapRows);
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
            _map[rafty, raftx] = Terrain.Bridge;
            _raftSpot.XPos = raftx;
            _raftSpot.YPos = rafty + 30;
        }
    }
}
