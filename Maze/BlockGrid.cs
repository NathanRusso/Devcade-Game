using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace DevcadeGame
{
    // BlockGrid- A class to represent the maze board.
    // It has row and column counts, a 2D array of blocks for the grid, and a list of available points.
    // availablePoints is  list of points that will contain all of the non used points for maze generation.
    public class BlockGrid
    {
        // This is a list of strings for the 4 direction one can move.
        private readonly List<string> directions = new List<string> { "N", "E", "S", "W" };

        // The list of all of the points that have yet to be added to the maze.
        private readonly List<ValueTuple<int, int>> availablePoints = new List<ValueTuple<int, int>> { };

        // This is a random object which will be used for number generation.
        private readonly Random random = new Random();

        // The 2D array of blocks that is the framework of the maze.
        private readonly Block[,] blockGrid;

        // The number of rows and columns in the maze.
        private readonly int rows;
        private readonly int columns;

        // The size of the blocks in the maze.
        private readonly int blockSize;


        /// <summary>
        /// This creates a new empty maze with the given number of rows and columns.
        /// This also sets the block size based on the pixel width of the maze.
        /// </summary>
        /// <param name="_rows">The number of rows in the maze.</param>
        /// <param name="_columns">The number of columns in the maze.</param>
        /// <param name="pixelWidth">The pixel width of the maze.</param>
        public BlockGrid(int _rows, int _columns, int pixelWidth)
        {
            rows = _rows;
            columns = _columns;
            blockSize = pixelWidth / _columns;
            blockGrid = new Block[rows, columns];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    blockGrid[y, x] = new Block(y, x); // New Block
                    availablePoints.Add(new ValueTuple<int, int>(y, x)); // New point
                }
            }
        }

        /// <summary>
        /// This gets the pixel size of the blocks in the maze.
        /// </summary>
        /// <returns> The pixel size of the blocks in the maze. </returns>
        public int GetBlockSize() { return blockSize; }

        /// <summary>
        /// This draws the inner lines of the maze, not the borders.
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="yEnd"></param>
        /// <param name="innerLine"></param>
        /// <param name="spriteBatch"></param>
        public void DrawMaze(int xStart, int yEnd, Texture2D innerLine, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = y % 2; x < columns; x += 2)
                {
                    Block spot = GetBlockAt(y, x);
                    if (spot.HasNorthWall())
                    {
                        Rectangle nLinePosition = new Rectangle(xStart + (x * blockSize),
                            yEnd - ((y + 1) * blockSize), blockSize, 1);
                        spriteBatch.Draw(innerLine, nLinePosition, Color.Red);
                    }
                    if (spot.HasEastWall())
                    {
                        Rectangle eLinePosition = new Rectangle(xStart + ((x + 1) * blockSize),
                            yEnd - ((y + 1) * blockSize), 1, blockSize);
                        spriteBatch.Draw(innerLine, eLinePosition, Color.Beige);
                    }
                    if (spot.HasSouthWall())
                    {
                        Rectangle sLinePosition = new Rectangle(xStart + (x * blockSize),
                            yEnd - (y * blockSize), blockSize, 1);
                        spriteBatch.Draw(innerLine, sLinePosition, Color.Gray);
                    }
                    if (spot.HasWestWall())
                    {
                        Rectangle wLinePosition = new Rectangle(xStart + (x * blockSize),
                            yEnd - ((y + 1) * blockSize), 1, blockSize);
                        spriteBatch.Draw(innerLine, wLinePosition, Color.Green);
                    }
                }
            }
        }

        /// <summary>
        /// This gets the block at the given coordinates in the maze.
        /// </summary>
        /// <returns> The Block at the given coordinates. </returns>
        public Block GetBlockAt(int yValue, int xValue) { return blockGrid[yValue, xValue]; }

        /// <summary>
        /// This gets the block at the given tuple of coordinates in the maze.
        /// </summary>
        /// <returns> The Block at the given coordinates. </returns>
        public Block GetBlockAt(ValueTuple<int, int> point) { return blockGrid[point.Item1, point.Item2]; }


        /// <summary>
        /// This sets the point's block to in the maze.
        /// It also removes the point from the available points.
        /// </summary>
        /// <param name="point">The point to remove.</param>
        private void AddBlockRemovePoint(ValueTuple<int, int> point) {
            Block block = GetBlockAt(point);
            block.RemoveVisit();
            block.AddToTheMaze();
            availablePoints.Remove(point);
        }

        /// <summary>
        /// This removes a loop in the visited points.
        /// The unnecessary points are removed and the blocks are reset.
        /// </summary>
        /// <param name="next">The next point to check.</param>
        /// <param name="visited">The list of visited points.</param>
        private void RemoveLoop(ValueTuple<int, int> next, List<ValueTuple<int, int>> visited) {
            // This saves the initial length of visited
            int length = visited.Count;

            // This saves the index of the point after the duplicate
            int afterIndex = visited.IndexOf(next) + 1;
            ValueTuple<int, int> after = visited[afterIndex];

            // This fixes the walls for the duplicated point
            /*if (next.Item1 - 1 == after.Item1)
            {
                GetBlockAt(next).AddNorthWall();
            }
            else if (next.Item2 + 1 == after.Item2)
            {
                GetBlockAt(next).AddEastWall();
            }
            else if (next.Item1 + 1 == after.Item1)
            {
                GetBlockAt(next).AddSouthWall();
            }
            else if (next.Item2 - 1 == after.Item2)
            {
                GetBlockAt(next).AddWestWall();
            }*/

            // This resets all blocks after the duplicate
            for (int i = afterIndex; i < length; i++)
            {
                GetBlockAt(visited[i]).Reset();
            }

            // This removes the loop from the path
            visited.RemoveRange(afterIndex, length - afterIndex);
        }

        /// <summary>
        /// This generates a path in the maze starting at the given point.
        /// </summary>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="visited">The list of visited points.</param>
        private void GeneratePath(ValueTuple<int, int> start, List<ValueTuple<int, int>> visited) {
            // This saves the current point and block and creates a next point
            ValueTuple<int, int> current = start;
            Block currentBlock = GetBlockAt(current);
            ValueTuple<int, int> next;
            Block nextBlock;

            // This loops until the current block is in the maze, the path is complete
            while (currentBlock.IsNotInTheMaze()) {
                // This generates a random direction for the path to go
                string direction = directions[random.Next(0, 4)];

                // This move the maze path in the direction if possible
                if (direction == "N" && current.Item1 != 0)
                {
                    // This gets the next point and block in the maze
                    next = new ValueTuple<int, int>(current.Item1 - 1, current.Item2);
                    nextBlock = GetBlockAt(next);

                    // This decides what to do with the next block
                    if (nextBlock.IsVisited())
                    {
                        // A loop in the path needs to be removed
                        RemoveLoop(next, visited);
                    } 
                    else
                    {
                        // This opens the blocks' walls so the path can go through
                        currentBlock.RemoveNorthWall();
                        nextBlock.RemoveSouthWall();

                        if (nextBlock.IsNotInTheMaze())
                        {
                            // This saves the next blocks as visited
                            nextBlock.AddVisit();
                            visited.Add(next);
                        }
                    }

                    // This moves the path onto the next point
                    current = next;
                    currentBlock = nextBlock;
                }
                else if (direction == "E" && current.Item2 != columns - 1) 
                {
                    // This gets the next point and block in the maze
                    next = new ValueTuple<int, int>(current.Item1, current.Item2 + 1);
                    nextBlock = GetBlockAt(next);

                    // This decides what to do with the next block
                    if (nextBlock.IsVisited())
                    {
                        // A loop in the path need to be removed
                        RemoveLoop(next, visited);
                    }
                    else
                    {
                        // This opens the blocks' walls so the path can go through
                        currentBlock.RemoveEastWall();
                        nextBlock.RemoveWestWall();

                        if (nextBlock.IsNotInTheMaze())
                        {
                            // This saves the next blocks as visited
                            nextBlock.AddVisit();
                            visited.Add(next);
                        }
                    }

                    // This moves the path onto the next point
                    current = next;
                    currentBlock = nextBlock;
                }
                else if (direction == "S" && current.Item1 != rows - 1)
                {
                    // This gets the next point and block in the maze
                    next = new ValueTuple<int, int>(current.Item1 + 1, current.Item2);
                    nextBlock = GetBlockAt(next);

                    // This decides what to do with the next block
                    if (nextBlock.IsVisited())
                    {
                        // A loop in the path need to be removed
                        RemoveLoop(next, visited);
                    } 
                    else
                    {
                        // This opens the blocks' walls so the path can go through
                        currentBlock.RemoveSouthWall();
                        nextBlock.RemoveNorthWall();

                        if (nextBlock.IsNotInTheMaze())
                        {
                            // This saves the next blocks as visited
                            nextBlock.AddVisit();
                            visited.Add(next);
                        }
                    }

                    // This moves the path onto the next point
                    current = next;
                    currentBlock = nextBlock;
                }
                else if (direction == "W" && current.Item2 != 0)
                {
                    // This gets the next point and block in the maze
                    next = new ValueTuple<int, int>(current.Item1, current.Item2 - 1);
                    nextBlock = GetBlockAt(next);

                    // This decides what to do with the next block
                    if (nextBlock.IsVisited())
                    {
                        // A loop in the path need to be removed
                        RemoveLoop(next, visited);
                    } 
                    else
                    {
                        // This opens the blocks' walls so the path can go through
                        currentBlock.RemoveWestWall();
                        nextBlock.RemoveEastWall();

                        if (nextBlock.IsNotInTheMaze())
                        {
                            // This saves the next blocks as visited
                            nextBlock.AddVisit();
                            visited.Add(next);
                        }
                    }

                    // This moves the path onto the next point
                    current = next;
                    currentBlock = nextBlock;
                }
            }
        }

        /// <summary>
        /// This generates the maze using a variation of Wilson's Maze Algorithm.
        /// </summary>
        public void GenerateMaze()
        {
            // This gets the start index, a random integer from 0 to the number of available points - 1
            // This then gets the first point to start the maze generation using the first index
            int firstIndex = random.Next(0, availablePoints.Count);
            ValueTuple<int, int> firstPoint = availablePoints[firstIndex];

            // This adds the first block to the maze and removes the first point from the available points
            GetBlockAt(firstPoint).AddToTheMaze();
            availablePoints.Remove(firstPoint);

            // This loops until there are no more available points to add to the maze
            while (availablePoints.Count > 0)
            {
                // This gets a start point for the new path in the maze.
                ValueTuple<int, int> startPoint = availablePoints[random.Next(0, availablePoints.Count)];

                // This will hold the points in the maze that will be added in as a new path
                List<ValueTuple<int, int>> visitedPoints = new List<ValueTuple<int, int>> { startPoint};

                // This marks the start point as visited
                GetBlockAt(startPoint).AddVisit();

                // This creates a new path of points for the maze
                GeneratePath(startPoint, visitedPoints);

                // This adds all of the visited points to the maze
                // It then removes the points from the available points
                foreach (var point in visitedPoints)
                {
                    AddBlockRemovePoint(point);
                }
            }
        }   
    }
}
