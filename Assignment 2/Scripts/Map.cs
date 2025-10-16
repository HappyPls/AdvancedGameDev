using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
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
            _rows = Math.Max(5, rows);
            _cols = Math.Max(5, cols);
            _rng = rng;

            _grid = new Room[_rows, _cols];

            GenerateRooms();

            //center character
            Row = _rows / 2;
            Col = _cols / 2;
            _grid[Row, Col] = new EmptyRoom();

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
        public bool TryMove(string direction)
        {
            int newRow = Row;
            int newCol = Col;

            // Movement logic
            switch (direction.ToLower())
            {
                case "north":
                case "n": newRow -= 1; break;
                case "south":
                case "s": newRow += 1; break;
                case "east":
                case "e": newCol += 1; break;
                case "west":
                case "w": newCol -= 1; break;
                default:
                    return false;
            }

            // Check bounds first BEFORE updating
            if (newRow < 0 || newRow >= _rows || newCol < 0 || newCol >= _cols)
            {
                return false;
            }

            Row = newRow;
            Col = newCol;

            Room newRoom = _grid[Row, Col];
            newRoom.VisitCount++;
            return true;
        }

        private void GenerateRooms()
        {
            bool haveTreasure = false;
            bool haveEncounter = false;

            for (int r = 0; r< _rows; r++)
            {
                for (int c  = 0; c < _cols; c++)
                {
                    int roll = _rng.Next(0, 100);
                    Room room;

                    //set room random chance to spawn
                    if (roll < 20)
                    {
                        room = new TreasureRoom();
                        haveTreasure = true;
                    }
                    else if (roll < 50)
                    {
                         room = new EncounterRoom();
                         haveEncounter = true;
                    }
                    else
                    {
                        room = new EmptyRoom();
                    }

                    _grid[r, c] = room;
                }
            }

            if (!haveTreasure) _grid[_rows - 1, 0] = new TreasureRoom();
            if (!haveEncounter) _grid[0, _cols - 1] = new EncounterRoom();
        }

        private void WireNeighbours()
        {
            for (int r = 0; r< _rows; r++)
            {
                for (int c = 0; c< _cols; c++)
                {
                    Room room = _grid[r, c];
                    room.North = (r > 0) ? _grid[r - 1, c] : null;
                    room.South = (r < _rows -1) ? _grid[r + 1, c] : null;
                    room.East = (c < _cols - 1) ? _grid[r, c+1] : null;
                    room.West = (c > 0) ? _grid[r, c -1] : null;
                }
            }
        }
        public void SetPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
