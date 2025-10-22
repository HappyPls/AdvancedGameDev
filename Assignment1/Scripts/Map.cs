using System;
using System.CodeDom.Compiler;

namespace Lab6Dungeon
{
    /// <summary>
    /// Builds the Map and Navigates a grid of rooms
    /// </summary>
    public class Map
    {
        private Room[,] _grid;
        private int _rows = 0;
        private int _cols = 0;
        private Random _rng;

        public int Row { get; private set; }
        public int Col { get; private set; }

        public Map(int rows, int cols, Random rng)
        {
            _rows = Math.Max(3,rows);
            _cols = Math.Max(3,cols);
            _rng = rng;

            _grid = new Room[_rows, _cols];

            GenerateRooms();

            Row = _rows / 2;
            Col = _cols / 2;
            _grid[Row, Col] = new BasicRoom();

            WireNeighbours();
        }

        public Room CurrentRoom()
        {
            if (Row < 0) Row = 0;
            if (Row >= _rows) Row = _rows - 1;
            if (Col < 0) Col = 0;
            if (Col >= _cols) Col = _cols - 1;
            return _grid[Row, Col];
        }

        public bool TryMove(string dir)
        {
            dir = (dir ?? "").Trim().ToLower();

            Room cur = _grid[Row, Col];

            if (dir == "n" || dir == "north")
            {
                if (cur.North != null) { Row -= 1; return true; }
            }
            else if (dir == "s" || dir == "south")
            {
                if (cur.South != null) { Row += 1; return true; }
            }
            else if (dir == "e" || dir == "east")
            {
                if (cur.East != null) { Col += 1; return true; }
            }
            else if (dir == "w" || dir == "west")
            {
                if (cur.West != null) { Col -= 1; return true; }
            }

            return false;
        }

        private void GenerateRooms()
        {
            bool haveTreasure = false;
            bool haveEncounter = false;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    int roll = _rng.Next(0, 100);
                    Room room;

                    if (roll < 25)
                    {
                        room = new EncounterRoom();
                        haveEncounter = true;
                    }
                    else if (roll < 70)
                    {
                        room = new TreasureRoom();
                        haveTreasure = true;
                    }
                    else
                    {
                        room = new BasicRoom();
                    }

                    _grid[r, c] = room;
                }
            }

            if (!haveTreasure) _grid[_rows - 1, 0] = new TreasureRoom();
            if (!haveEncounter) _grid[0, _cols - 1] = new EncounterRoom();
        }

        private void WireNeighbours()
        {
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    Room room = _grid[r, c];
                    room.North = (r > 0) ? _grid[r - 1, c] : null;
                    room.South = (r < _rows - 1) ? _grid[r + 1, c] : null;
                    room.East = (c < _cols - 1) ? _grid[r, c + 1] : null;
                    room.West = (c > 0) ? _grid[r, c - 1] : null;
                }
            }
        }
    }
}
