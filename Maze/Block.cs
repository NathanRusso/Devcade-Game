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

        /// <summary>
        /// Creates the Block with specific x and y values.
        /// All of the walls exist, and it hasn't been visited or added to the maze.
        /// </summary>
        /// <param name="_yIndex"> The y index of the Block. </param>
        /// <param name="_xIndex"> The x index of the Block. </param>
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

        /// <summary> Resets all of the values except for the xIndex and yIndex. </summary>
        public void Reset()
        {
            northWall = true;
            eastWall = true;
            southWall = true;
            westWall = true;
            visited = false;
            inTheMaze = false;
        }

        /// <returns> Whether a Block has been added to the maze. </returns>
        public bool IsInTheMaze() { return inTheMaze; }

        /// <returns>Whether a Block has NOT been added to the maze. </returns>
        public bool IsNotInTheMaze() { return !inTheMaze; }

        /// <summary> Sets the Block's in the maze value to true. </returns>
        public void AddToTheMaze() { inTheMaze = true; }


        /// <returns> Whether a Block has been visited. </returns>
        public bool IsVisited() { return visited; }

        /// <returns> Whether a Block has NOT been visited. </returns>
        public bool IsNotVisited() { return !visited; }

        /// <summary> Sets the Block's visited value to true. </summary>
        public void AddToVisited() { visited = true; }

        /// <summary> Sets the Block's visited value to false. </summary>
        public void RemoveFromVisited() { visited = false; }


        /// <returns> Whether a Block has a north wall. </returns>
        public bool HasNorthWall() { return northWall; }

        /// <returns> Whether a Block has a east wall. </returns>
        public bool HasEastWall() { return eastWall; }

        /// <returns> Whether a Block has a south wall. </returns>
        public bool HasSouthWall() { return southWall; }

        /// <returns> Whether a Block has a west wall. </returns>
        public bool HasWestWall() { return westWall; }

        /// <summary> Removes the north wall from the Block. </summary>
        public void RemoveNorthWall() { northWall = false; }

        /// <summary> Removes the east wall from the Block. </summary>
        public void RemoveEastWall() { eastWall = false; }

        /// <summary> Removes the south wall from the Block. </summary>
        public void RemoveSouthWall() { southWall = false; }

        /// <summary> Removes the west wall from the Block. </summary>
        public void RemoveWestWall() { westWall = false; }

        /// <summary> Adds the north wall from the Block. </summary>
        public void AddNorthWall() { northWall = true; }

        /// <summary> Adds the east wall from the Block. </summary>
        public void AddEastWall() { eastWall = true; }

        /// <summary> Adds the south wall from the Block. </summary>
        public void AddSouthWall() { southWall = true; }

        /// <summary> Adds the west wall from the Block. </summary>
        public void AddWestWall() { westWall = true; }
    }
}
