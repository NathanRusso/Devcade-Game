using System;

namespace DevcadeGame
{
    // Block- An object to represent a block on the grid in the maze.
    public class Block
    {
        // The y and x integers used for the of the Block.
        private readonly int yIndex;
        private readonly int xIndex;

        // The wall values to determine if a wall exist.
        private bool northWall;
        private bool eastWall;
        private bool southWall;
        private bool westWall;

        // The values used to mark when a Block is visited and when it is added to the maze.
        private bool visited;
        private bool inTheMaze;

        // This creates the Block with specific x and y values.
        // All of the walls exist, and it hasn't been visited or added to the maze.
        public Block(int _yIndex, int _xIndex)
        {
            yIndex = _yIndex;
            xIndex = _xIndex;
            northWall = true;
            eastWall = true;
            southWall = true;
            westWall = true;
            visited = false;
            inTheMaze = false;
        }

        // This resets all of the values except for the xIndex and yIndex.
        public void Reset()
        {
            northWall = true;
            eastWall = true;
            southWall = true;
            westWall = true;
            visited = false;
            inTheMaze = false;
        }

        // This returns the Block's y coordinate.
        public int GetYCoordinate() { return yIndex; }

        // This returns the Block's x coordinate.
        public int GetXCoordinate() { return xIndex; }

        // This returns whether a Block has a north wall.
        public bool HasNorthWall() { return northWall; }

        // This returns whether a Block has a east wall.
        public bool HasEastWall() { return eastWall; }

        // This returns whether a Block has a south wall.
        public bool HasSouthWall() { return southWall; }

        // This returns whether a Block has a west wall.
        public bool HasWestWall() { return westWall; }

        // This returns whether a Block has been visited.
        public bool HasBeenVisited() { return visited; }

        // This returns whether a Block has been added to the maze.
        public bool IsInTheMaze() { return inTheMaze; }

        // This sets the north wall to a given boolean.
        public void SetNorthWall(bool north) { northWall = north; }

        // This sets the east wall to a given boolean.
        public void SetEastWall(bool east) { eastWall = east; }

        // This sets the south wall to a given boolean.
        public void SetSouthWall(bool south) { southWall = south; }

        // This sets the west wall to a given boolean.
        public void SetWestWall(bool west) { westWall = west; }

        // This sets the Block's visited value to a given boolean.
        public void SetVisitedTo(bool visit) { visited = visit; }

        // This changes the maze value to true since a Block has been added to the maze.
        public void AddToTheMaze() { inTheMaze = true; }

        // This changes the maze value to false since a Block has been removed from the maze.
        public void removeFromTheMaze() { inTheMaze = false; }
    }
}
