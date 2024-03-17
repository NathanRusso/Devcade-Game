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
        private int block_size;

        // It is an integer to count the number of times the generateMaze() while loop runs.
        private int iterations = 0;

        // It is an integer to track the number of points used.
        private int point_count = 0;

        // These integers are used to help prevent a stack overflow.
        private int countR = 0;
        private int recursionLimit = 2600;

        // This is the BlockGrid constructor.
        public BlockGrid(int _rows, int _columns, int pixel_width)
        {
            rows = _rows;
            columns = _columns;
            block_size = pixel_width / _columns;
            blockGrid = new Block[_rows, _columns];
            availablePoints = new List<ValueTuple<int, int>> { };
            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    Block block = new Block(y, x);
                    blockGrid[y, x] = block;
                    ValueTuple<int, int> point = new ValueTuple<int, int>(y, x);
                    availablePoints.Add(point);
                }
            }
        }


        public int getRows() { return rows; }

        public int getColumns() { return columns; }

        public int getBlockSize() { return block_size; }

        public int getIterations() { return iterations; }

        public int getPointCount() { return point_count; }

        public Block[,] getBlockGrid() { return blockGrid; }

        public List<ValueTuple<int, int>> getAvailablePoints() { return availablePoints; }

        public Block getBlockAt(int y_value, int x_value) { return blockGrid[y_value, x_value]; }

        public Block getBlockAt(ValueTuple<int, int> point) { return blockGrid[point.Item1, point.Item2]; }


        // It draws the inner lines of the maze, not the borders.
        public void drawMaze(int x_start, int y_end, Texture2D innerLine, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = y % 2; x < columns; x += 2)
                {
                    Block spot = this.getBlockAt(y, x);
                    if (spot.hasNorthWall())
                    {
                        Rectangle n_line_position = new Rectangle(x_start + (x * block_size),
                            y_end - ((y + 1) * block_size), block_size, 1);
                        spriteBatch.Draw(innerLine, n_line_position, Color.White);
                    }
                    if (spot.hasEastWall())
                    {
                        Rectangle e_line_position = new Rectangle(x_start + ((x + 1) * block_size),
                            y_end - ((y + 1) * block_size), 1, block_size);
                        spriteBatch.Draw(innerLine, e_line_position, Color.White);
                    }
                    if (spot.hasSouthWall())
                    {
                        Rectangle s_line_position = new Rectangle(x_start + (x * block_size),
                            y_end - (y * block_size), block_size, 1);
                        spriteBatch.Draw(innerLine, s_line_position, Color.White);
                    }
                    if (spot.hasWestWall())
                    {
                        Rectangle w_line_position = new Rectangle(x_start + (x * block_size),
                            y_end - ((y + 1) * block_size), 1, block_size);
                        spriteBatch.Draw(innerLine, w_line_position, Color.White);
                    }
                }
            }
        }

        // It generates a randomized maze using Wilson's algorithm.
        // The following 5 private functions all help this function run.
        public void generateMaze()
        {
            // A random point of coordinates is generated.
            // The block at said coordinates is added to the maze.
            // The point of coordinates is then removed from available points.
            ValueTuple<int, int> first_point = generate_random_available_point();
            this.getBlockAt(first_point).addToTheMaze();
            availablePoints.Remove(first_point);

            // Increases the point count due to the creation of a point for first_point.
            point_count++;

            // It generates the framework of the maze.
            // It gives every Block in blockGrid1 an updated set of values.
            // These values will determine the type of Block to be drawn.
            while (availablePoints.Count > 0)
            {
                // It is a random point of coordinates to determine the starting position.
                ValueTuple<int, int> start = generate_random_available_point();

                // It is an empty list which is passed as a parameter to keep track of visited points.
                List<ValueTuple<int, int>> empty = new List<ValueTuple<int, int>> { };

                // It is an empty list which will be filled with all of the traveled to points.
                List<ValueTuple<int, int>> visited = generate_path(empty, start, 0);

                // Increases the point count by the amount of points to be added to the maze.
                point_count += visited.Count;

                // It sets all of the optimized points into the maze.
                // It then deletes all of those points from the available list.
                foreach (var point in visited)
                {
                    this.getBlockAt(point).addToTheMaze();
                    availablePoints.Remove(point);
                }

                // It adds one to the count of iterations.
                iterations++;
            }
        }

        // It generates a random point from the list of available points.
        private ValueTuple<int, int> generate_random_available_point()
        {
            Random random = new Random();
            int index = random.Next(0, availablePoints.Count);
            ValueTuple<int, int> point = availablePoints[index];
            return point;
        }

        // It generates a random direction between north, east, south, and west.
        // Given a previous direction, if stated, it removes the opposite as a possibility.
        private string generate_random_direction()
        {
            countR++;
            if (countR >= recursionLimit)
            {
                countR = 0;
                return "";
            }
            List<string> directions = new List<string> { "N", "E", "S", "W" };
            Random random = new Random();
            return directions[random.Next(0, 4)];
        }
        
        // It adds back the correct wall value based on change in position between itself and the previous point.
        private void re_add_wall_value(ValueTuple<int, int> current, ValueTuple<int, int> prior)
        {
            if (prior.Item1 + 1 == current.Item1)
            {
                this.getBlockAt(current).setSouthWall(false);
            }
            else if (prior.Item2 + 1 == current.Item2)
            {
                this.getBlockAt(current).setWestWall(false);
            }
            else if (prior.Item1 - 1 == current.Item1)
            {
                this.getBlockAt(current).setNorthWall(false);
            }
            else if (prior.Item2 - 1 == current.Item2)
            {
                this.getBlockAt(current).setEastWall(false);
            }
        }

        // It returns a list of visited points with any loops removed.
        private List<ValueTuple<int, int>> remove_loop(List<ValueTuple<int, int>> visited_points,
            ValueTuple<int, int> current)
        {
            List<ValueTuple<int, int>> updated_points = new List<ValueTuple<int, int>> { };
            if (this.getBlockAt(current).hasBeenVisited())
            {
                int index = visited_points.IndexOf(current);
                for (int i = 0; i < visited_points.Count; i++)
                {
                    if (i < index)
                    {
                        updated_points.Add(visited_points[i]);
                    }
                    else if (i >= index)
                    {
                        this.getBlockAt(visited_points[i]).reset();
                        if (i == index && i > 0)
                        {
                            re_add_wall_value(visited_points[i], visited_points[i - 1]);
                        }
                    }
                }
            }
            else
            {
                updated_points = visited_points;
            }
            updated_points.Add(current);
            this.getBlockAt(current).setVisitedTo(true);
            return updated_points;
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
        private List<ValueTuple<int, int>> generate_path(List<ValueTuple<int, int>> visited_points,
            ValueTuple<int, int> start, int recursive_count)
        {
            string direction = generate_random_direction();
            if (direction == "")
            {
                foreach (var point in visited_points)
                {
                    this.getBlockAt(point).reset();
                }
                List<ValueTuple<int, int>> nothing = new List<ValueTuple<int, int>> { };
                return nothing;
            }
            if (direction == "N" && start.Item1 != rows - 1)
            {
                List<ValueTuple<int, int>> updated_visited = remove_loop(visited_points, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1 + 1, start.Item2);
                this.getBlockAt(start).setNorthWall(false);
                this.getBlockAt(next).setSouthWall(false);
                if (this.getBlockAt(next).isInTheMaze())
                {
                    this.getBlockAt(next).setVisitedTo(true);
                    return updated_visited;
                }
                else
                {
                    return generate_path(updated_visited, next, recursive_count);
                }
            }
            else if (direction == "E" && start.Item2 != columns - 1)
            {
                List<ValueTuple<int, int>> updated_visited = remove_loop(visited_points, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1, start.Item2 + 1);
                this.getBlockAt(start).setEastWall(false);
                this.getBlockAt(next).setWestWall(false);
                if (this.getBlockAt(next).isInTheMaze())
                {
                    this.getBlockAt(next).setVisitedTo(true);
                    return updated_visited;
                }
                else
                {
                    return generate_path(updated_visited, next, recursive_count);
                }
            }
            else if (direction == "S" && start.Item1 != 0)
            {
                List<ValueTuple<int, int>> updated_visited = remove_loop(visited_points, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1 - 1, start.Item2);
                this.getBlockAt(start).setSouthWall(false);
                this.getBlockAt(next).setNorthWall(false);
                if (this.getBlockAt(next).isInTheMaze())
                {
                    this.getBlockAt(next).setVisitedTo(true);
                    return updated_visited;
                }
                else
                {
                    return generate_path(updated_visited, next, recursive_count);
                }
            }
            else if (direction == "W" && start.Item2 != 0)
            {
                List<ValueTuple<int, int>> updated_visited = remove_loop(visited_points, start);
                ValueTuple<int, int> next = new ValueTuple<int, int>(start.Item1, start.Item2 - 1);
                this.getBlockAt(start).setWestWall(false);
                this.getBlockAt(next).setEastWall(false);
                if (this.getBlockAt(next).isInTheMaze())
                {
                    this.getBlockAt(next).setVisitedTo(true);
                    return updated_visited;
                }
                else
                {
                    return generate_path(updated_visited, next, recursive_count);
                }
            }
            else
            {
                return generate_path(visited_points, start, recursive_count);
            }
        }
    }
}
