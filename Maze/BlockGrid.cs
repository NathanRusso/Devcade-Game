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
        // The 2D array of blocks that is the framework of the maze.
        private Block[,] blockGrid;

        // The list of all of the points that have yet to be added to the maze.
        private List<ValueTuple<int, int>> availablePoints;

        // The number of rows and columns in the maze.
        private int rows;
        private int columns;

        // The size of the blocks in the maze.
        private int blockSize;

        // It is an integer to count the number of times the generateMaze() while loop runs.
        private int iterations = 0;

        // It is an integer to track the number of points used.
        private int pointCount = 0; // REMOVE?????

        // These integers are used to help prevent a stack overflow.
        private int countR = 0;
        private int recursionLimit = 2600;

        // This is a list of strings for the 4 direction one can move.
        List<string> directions = new List<string> { "N", "E", "S", "W" };

        // This is the BlockGrid constructor.
        public BlockGrid(int _rows, int _columns, int pixelWidth)
        {
            rows = _rows;
            columns = _columns;
            blockSize = pixelWidth / _columns;
            blockGrid = new Block[rows, columns];
            availablePoints = new List<ValueTuple<int, int>> { };
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Block block = new Block(y, x);
                    blockGrid[y, x] = block;
                    ValueTuple<int, int> point = new ValueTuple<int, int>(y, x);
                    availablePoints.Add(point);
                }
            }
        }


        public int GetRows() { return rows; }

        public int GetColumns() { return columns; }

        public int GetBlockSize() { return blockSize; }

        public int GetIterations() { return iterations; }

        public int GetPointCount() { return pointCount; }

        public Block[,] GetBlockGrid() { return blockGrid; }

        public List<ValueTuple<int, int>> GetAvailablePoints() { return availablePoints; }

        public Block GetBlockAt(int yValue, int xValue) { return blockGrid[yValue, xValue]; }

        public Block GetBlockAt(ValueTuple<int, int> point) { return blockGrid[point.Item1, point.Item2]; }


        // It draws the inner lines of the maze, not the borders.
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
                        spriteBatch.Draw(innerLine, nLinePosition, Color.White);
                    }
                    if (spot.HasEastWall())
                    {
                        Rectangle eLinePosition = new Rectangle(xStart + ((x + 1) * blockSize),
                            yEnd - ((y + 1) * blockSize), 1, blockSize);
                        spriteBatch.Draw(innerLine, eLinePosition, Color.White);
                    }
                    if (spot.HasSouthWall())
                    {
                        Rectangle sLinePosition = new Rectangle(xStart + (x * blockSize),
                            yEnd - (y * blockSize), blockSize, 1);
                        spriteBatch.Draw(innerLine, sLinePosition, Color.White);
                    }
                    if (spot.HasWestWall())
                    {
                        Rectangle wLinePosition = new Rectangle(xStart + (x * blockSize),
                            yEnd - ((y + 1) * blockSize), 1, blockSize);
                        spriteBatch.Draw(innerLine, wLinePosition, Color.White);
                    }
                }
            }
        }

        // It generates a randomized maze using Wilson's algorithm.
        // The following 5 private functions all help this function run.
        public void GenerateMaze()
        {
            // A random point of coordinates is generated.
            // The block at said coordinates is added to the maze.
            // The point of coordinates is then removed from available points.
            ValueTuple<int, int> firstPoint = GenerateRandomAvailablePoint();
            GetBlockAt(firstPoint).AddToTheMaze();
            availablePoints.Remove(firstPoint);

            // Increases the point count due to the creation of a point for firstPoint.
            pointCount++;

            // It generates the framework of the maze.
            // It gives every Block in blockGrid1 an updated set of values.
            // These values will determine the type of Block to be drawn.
            while (availablePoints.Count > 0)
            {
                // It is a random point of coordinates to determine the starting position.
                ValueTuple<int, int> start = GenerateRandomAvailablePoint();

                // It is an empty list which is passed as a parameter to keep track of visited points.
                List<ValueTuple<int, int>> empty = new List<ValueTuple<int, int>> { };

                // It is an empty list which will be filled with all of the traveled to points.
                List<ValueTuple<int, int>> visited = GeneratePath(empty, start, 0);

                // Increases the point count by the amount of points to be added to the maze.
                pointCount += visited.Count;

                // It sets all of the optimized points into the maze.
                // It then deletes all of those points from the available list.
                foreach (var point in visited)
                {
                    GetBlockAt(point).AddToTheMaze();
                    availablePoints.Remove(point);
                }

                // It adds one to the count of iterations.
                iterations++;
            }
        }

        // It generates a random point from the list of available points.
        private ValueTuple<int, int> GenerateRandomAvailablePoint()
        {
            Random random = new Random();
            int index = random.Next(0, availablePoints.Count);
            ValueTuple<int, int> point = availablePoints[index];
            return point;
        }

        // It generates a random direction between north, east, south, and west.
        // Given a previous direction, if stated, it removes the opposite as a possibility.
        private string GenerateRandomDirection()
        {
            countR++;
            if (countR >= recursionLimit)
            {
                countR = 0;
                return "";
            }
            Random random = new Random();
            return directions[random.Next(0, 4)];
        }
        
        // It adds back the correct wall value based on change in position between itself and the previous point.
        private void ReAddWallValue(ValueTuple<int, int> current, ValueTuple<int, int> prior)
        {
            if (prior.Item1 + 1 == current.Item1)
            {
                GetBlockAt(current).SetSouthWall(false);
            }
            else if (prior.Item2 + 1 == current.Item2)
            {
                GetBlockAt(current).SetWestWall(false);
            }
            else if (prior.Item1 - 1 == current.Item1)
            {
                GetBlockAt(current).SetNorthWall(false);
            }
            else if (prior.Item2 - 1 == current.Item2)
            {
                GetBlockAt(current).SetEastWall(false);
            }
        }

        // It returns a list of visited points with any loops removed.
        private List<ValueTuple<int, int>> RemoveLoop(List<ValueTuple<int, int>> visitedPoints, ValueTuple<int, int> current)
        {
            List<ValueTuple<int, int>> updatedPoints = new List<ValueTuple<int, int>> { };
            if (GetBlockAt(current).HasBeenVisited())
            {
                int index = visitedPoints.IndexOf(current);
                for (int i = 0; i < visitedPoints.Count; i++)
                {
                    if (i < index)
                    {
                        updatedPoints.Add(visitedPoints[i]);
                    }
                    else if (i >= index)
                    {
                        GetBlockAt(visitedPoints[i]).Reset();
                        if (i == index && i > 0)
                        {
                            ReAddWallValue(visitedPoints[i], visitedPoints[i - 1]);
                        }
                    }
                }
            }
            else
            {
                updatedPoints = visitedPoints;
            }
            updatedPoints.Add(current);
            GetBlockAt(current).SetVisitedTo(true);
            return updatedPoints;
        }

        /* This function returns a list of all of the points visited.
        * Given a chosen start point, it randomly grabs the next direction.
        * In order to move a direction, it must fulfill two conditions.
        *     1. The direction string must match the correct string
        *     2. The current position must not be at the corresponding border.
        * It then goes through 1 of 4 if statements.
        *     - If the current, start is already visited, then the loop is erased.
        *     - The point is added to the list and wall values are adjusted.
        *     - If the next point is already in the maze, the function ends.
        *     - If not, the functions calls itself with a new start point.
        * If it never enters any of the 4 statements, it returns it self and tries again. */
        private List<ValueTuple<int, int>> GeneratePath(List<ValueTuple<int, int>> visitedPoints, ValueTuple<int, int> start, int RecursiveCount)
        {
            string direction = GenerateRandomDirection();
            if (direction == "")
            {
                foreach (var point in visitedPoints)
                {
                    GetBlockAt(point).Reset();
                }
                List<ValueTuple<int, int>> nothing = new List<ValueTuple<int, int>> { };
                return nothing;
            }
            if (direction == "N" && start.Item1 != rows - 1)
            {
                List<ValueTuple<int, int>> updatedVisited = RemoveLoop(visitedPoints, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1 + 1, start.Item2);
                GetBlockAt(start).SetNorthWall(false);
                GetBlockAt(next).SetSouthWall(false);
                if (GetBlockAt(next).IsInTheMaze())
                {
                    GetBlockAt(next).SetVisitedTo(true);
                    return updatedVisited;
                }
                else
                {
                    return GeneratePath(updatedVisited, next, RecursiveCount);
                }
            }
            else if (direction == "E" && start.Item2 != columns - 1)
            {
                List<ValueTuple<int, int>> updatedVisited = RemoveLoop(visitedPoints, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1, start.Item2 + 1);
                GetBlockAt(start).SetEastWall(false);
                GetBlockAt(next).SetWestWall(false);
                if (GetBlockAt(next).IsInTheMaze())
                {
                    GetBlockAt(next).SetVisitedTo(true);
                    return updatedVisited;
                }
                else
                {
                    return GeneratePath(updatedVisited, next, RecursiveCount);
                }
            }
            else if (direction == "S" && start.Item1 != 0)
            {
                List<ValueTuple<int, int>> updatedVisited = RemoveLoop(visitedPoints, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1 - 1, start.Item2);
                GetBlockAt(start).SetSouthWall(false);
                GetBlockAt(next).SetNorthWall(false);
                if (GetBlockAt(next).IsInTheMaze())
                {
                    GetBlockAt(next).SetVisitedTo(true);
                    return updatedVisited;
                }
                else
                {
                    return GeneratePath(updatedVisited, next, RecursiveCount);
                }
            }
            else if (direction == "W" && start.Item2 != 0)
            {
                List<ValueTuple<int, int>> updatedVisited = RemoveLoop(visitedPoints, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1, start.Item2 - 1);
                GetBlockAt(start).SetWestWall(false);
                GetBlockAt(next).SetEastWall(false);
                if (GetBlockAt(next).IsInTheMaze())
                {
                    GetBlockAt(next).SetVisitedTo(true);
                    return updatedVisited;
                }
                else
                {
                    return GeneratePath(updatedVisited, next, RecursiveCount);
                }
            }
            else
            {
                return GeneratePath(visitedPoints, start, RecursiveCount);
            }
        }
    }
}
