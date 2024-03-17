using System;

namespace DevcadeGame
{
    // Block- An object to represent a block on the grid in the maze.
    public class Block
    {
        // The y and x integers used for the  of the Block.
        private int y_index;
        private int x_index;

        // The wall values to determine if a wall exist.
        private bool north_wall;
        private bool east_wall;
        private bool south_wall;
        private bool west_wall;

        // The values used to mark when a Block is visisted and when it is added to the maze.
        private bool visited;
        private bool in_the_maze;

        // This creates the Block with specific x and y values.
        // All of the walls exist, and it hasn't been visited or added to the maze.
        public Block(int _y_index, int _x_index)
        {
            y_index = _y_index;
            x_index = _x_index;
            north_wall = true;
            east_wall = true;
            south_wall = true;
            west_wall = true;
            visited = false;
            in_the_maze = false;
        }

        // This resets all of the values except for the x_index and y_index.
        public void reset()
        {
            north_wall = true;
            east_wall = true;
            south_wall = true;
            west_wall = true;
            visited = false;
            in_the_maze = false;
        }

        // This returns the Block's y cooridnate.
        public int getYCoordinate() { return y_index; }

        // This returns the Block's x coordinate.
        public int getXCoordinate() { return x_index; }

        // This returns whether a Block has a north wall.
        public bool hasNorthWall() { return north_wall; }

        // This returns whether a Block has a east wall.
        public bool hasEastWall() { return east_wall; }

        // This returns whether a Block has a south wall.
        public bool hasSouthWall() { return south_wall; }

        // This returns whether a Block has a west wall.
        public bool hasWestWall() { return west_wall; }

        // This returns whether a Block has been visisted.
        public bool hasBeenVisited() { return visited; }

        // This returns whether a Block has been added to the maze.
        public bool isInTheMaze() { return in_the_maze; }

        // This sets the north wall to a given boolean.
        public void setNorthWall(bool north) { north_wall = north; }

        // This sets the east wall to a given boolean.
        public void setEastWall(bool east) { east_wall = east; }

        // This sets the south wall to a given boolean.
        public void setSouthWall(bool south) { south_wall = south; }

        // This sets the west wall to a given boolean.
        public void setWestWall(bool west) { west_wall = west; }

        // This sets the Block's visited value to a given boolean.
        public void setVisitedTo(bool visit) { visited = visit; }

        // This changes the maze value to true since a Block has been added to the maze.
        public void addToTheMaze() { in_the_maze = true; }

        // This changes the maze value to false since a Block has been removed from the maze.
        public void removeFromTheMaze() { in_the_maze = false; }
    }
}
