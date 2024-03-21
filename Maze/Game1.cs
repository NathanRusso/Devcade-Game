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

// MAKE SURE YOU RENAME ALL PROJECT FILES FROM DevcadeGame TO YOUR YOUR GAME NAME
namespace DevcadeGame
{
    // An enum to establish which state the game is currently in.
    public enum ScreenType
    {
        LoadingScreen,
        MazeScreen
    }


    public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

        // This stores the window dimensions in a rectangle object for easy use
        private Rectangle windowSize;

        // This determines which screen type we are in.
        private ScreenType screenType;

        // This is an integer used to determine how to move the arrow in both screen types.
        private int button_pressed;

        // This is a boolean used to determine when you made it to the end of the maze.
        private bool completedMaze;

        // This declares the current position of the arrow in the loading screen.
        private Rectangle vCurrentPosition;

        // This declares the vectors needed to draw the different possible maze sizes in the loading screen.
        private Vector2 vPosition_50_25;
        private Vector2 vPosition_40_20;
        private Vector2 vPosition_32_16;
        private Vector2 vPosition_20_10;
        private Vector2 vPosition_16_8;
        private Vector2 vPosition_10_5;
        private Vector2 vPosition_8_4;
        private Vector2 vPosition_4_2;
        private Vector2 vPosition_2_1;

        // This declares list of integers that must be used depending on if the game is running in Devcade.
        private List<int> selectedList;

        // This declares the height and width of the maze in pixels.
        private int mazePixelHeight;
        private int mazePixelWidth;

        // This declares the start and end pixel locations for the x-axis and y-axis of the maze.
        private int mazePixelXStart;
        private int mazePixelXEnd;
        private int mazePixelYStart;
        private int mazePixelYEnd;

        // This declares the middle point on the x-axis.
        private int screenPixelXCenter;

        // This declares the y-axis start and distance between for drawing maze sizes on the loading screen.
        private int loadScreenSizeStart;
        private int loadScreenSizeDisplacement;

        // This declares the y-axis current, start, and end position of the arrow sprite in the loading screen.
        private int loadScreenArrowCurrent;
        private int loadScreenArrowStart;
        private int loadScreenArrowEnd;

        // This declares values need to help when drawing the border of the maze.
        private int borderPixel1;
        private int borderPixel2;
        private int borderPixel3;
        private int borderPixel4;

        // This declares the y-axis positions for different strings that need to be drawn.
        private int drawPositionY1;
        private int drawPositionY2;
        private int drawPositionY3;
        private int drawPositionY4;

        // A BlockGrid with a 2D array of Blocks which makes up the grid for the maze.
        private BlockGrid blockGrid1;

        // The size of the block for the current maze, along with a smaller version.
        private int blockSizeC;
        private int blockSize9;
        private int blockSize8;
        private int blockSize1;

        // This declares integers used to mark drawing corners on the maze.
        private int topRightX;
        private int topRightY;
        private int bottomLeftX;
        private int bottomLeftY;

        // This declares the Texture2D needed to draw the maze lines.
        private Texture2D line;

        // This declares the SpriteFont needed to draw the maze title, creator name, other maze message.
        private SpriteFont titleFont;
        private SpriteFont creatorFont;
        private SpriteFont otherFont;

        // This declares the Texture2Ds needed to draw the image sprites.
        private Texture2D northArrow;
        private Texture2D eastArrow;
        private Texture2D southArrow;
        private Texture2D westArrow;
        private Texture2D startButton;
        private Texture2D stopSign;

        // This declares the Rectangles needed to draw the maze borders.
        private Rectangle northRectangle;
        private Rectangle eastRectangle;
        private Rectangle southRectangle;
        private Rectangle westRectangle;

        // This declares the start button and stop sign.
        private Rectangle startRectangle;
        private Rectangle stopRectangle;

        // This declares the different widths of different displayed text. These are used to center text.
        private int textWidth1;
        private int textWidth2;
        private int textWidth3;
        private int textWidth4;
        private int textWidth5;
        private int textWidth6;
        private int textWidth7;

        // This initializes the Vectors needed to draw the title, the creator, and other text for the game.
        private Vector2 titleVector;
        private Vector2 creatorVector;
        private Vector2 selectionVector;
        private Vector2 directionVector;
        private Vector2 luckVector;
        private Vector2 winVector;
        private Vector2 restartVector;

        // These are integers and a string set at your current position and direction.
        // These will change as you move.
        private int x_index;
        private int y_index;
        private string direction;

        // These are integers which will be used to convert pixel position to grid position.
        private int converted_x;
        private int converted_y;


        public Game1 game
        {
            get;
        }

        // Game constructor
        public Game1()
        {
            game = this;
            _graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
        }


        // This initializes all maze information needed after the loading screen.
        public void initializeMaze(int rows, int columns)
        {
            // It constructs a BlockGrid with a 2D array of Blocks that has a given number of rows and columns. 
            // 800 x 400 pixels or 1000 x 2000 pixels
            blockGrid1 = new BlockGrid(rows, columns, mazePixelWidth);
            /* Y by X, with block sizes.
             * 50 by 25, with a block size of 16 --> 40
             * 40 by 20, with a block size of 20 --> 50 
             * 32 by 16, with a block size of 25 ISSUE FOR UPSCALE 62.5
             * 20 by 10, with a block size of 40 --> 100
             * 16 by 8, with a block size of 50 --> 125
             * 10 by 5, with a block size of 80 --> 200
             * 8 by 4, with a block size of 100 --> 250
             * 4 by 2, with a block size of 200 --> 500
             * 2 by 1, with a block size of 400 --> 1000
             */

            // It sets the current and smaller block sizes based off of the current blockGrid.
            blockSizeC = blockGrid1.getBlockSize();
            blockSize9 = Convert.ToInt32(blockSizeC * 0.9);
            blockSize8 = Convert.ToInt32(blockSizeC * 0.8);
            blockSize1 = blockSizeC - blockSize9;

            // This initializes integers used to mark drawing corners on the maze.
            topRightX = mazePixelXEnd - blockSize9;
            topRightY = mazePixelYStart + blockSize1;
            bottomLeftX = mazePixelXStart + blockSize1;
            bottomLeftY = mazePixelYEnd - blockSize9;

            // This sets your starting position and direction.
            x_index = bottomLeftX;
            y_index = bottomLeftY;
            direction = "N";

            // This generates a maze for blockGrid1.
            blockGrid1.generateMaze();

            // This initializes the start button and stop sign.
            startRectangle = new Rectangle(bottomLeftX, bottomLeftY, blockSize8, blockSize8);
            stopRectangle = new Rectangle(topRightX, topRightY, blockSize8, blockSize8);
        }


        /// Performs any setup that doesn't require loaded content before the first frame.
        protected override void Initialize()
		{
			// Sets up the input library
			Input.Initialize();
            
			// Set window size if running debug (in release it will be full screen)
			#region
#if DEBUG
			_graphics.PreferredBackBufferWidth = 420;
			_graphics.PreferredBackBufferHeight = 980;
			_graphics.ApplyChanges();
#else
			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			_graphics.ApplyChanges();
#endif
            #endregion

            // This sets the current screen type to the loading screen.
            screenType = ScreenType.LoadingScreen;

            // This sets up the initial button and completed maze values.
            button_pressed = 0;
            completedMaze = false;

            windowSize = GraphicsDevice.Viewport.Bounds;
            
			base.Initialize();
	    }


        /// Performs any setup that requires loaded content before the first frame.
        protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

            // This initializes line to draw the lines, which includes the borders and inner lines.
            line = new Texture2D(GraphicsDevice, 1, 1);
            line.SetData(new[] { Color.White });

            // This initializes list of integers that must be used depending on if the game is running in Devcade.
            if (windowSize.Width == 420) // The game is on my computer.
            { 
                selectedList = new List<int> { 800, 400, 10, 410, 150, 950, 210, 130, 90, 847, 127, 
                                               7, 4, 3, 407, 0, 40, 70, 105}; 
            }
            else // The game is on Devcade.
            { 
                selectedList = new List<int> { 2000, 1000, 40, 1040, 375, 2375, 540, 325, 225, 2122, 322,
                                               31, 12, 9, 1021, 0, 100, 175, 260}; 
            }

            // This initializes the height and width of the maze in pixels.
            mazePixelHeight = selectedList[0];
            mazePixelWidth = selectedList[1];

            // This initializes the start and end pixel locations for the x-axis and y-axis of the maze.
            mazePixelXStart = selectedList[2];
            mazePixelXEnd = selectedList[3];
            mazePixelYStart = selectedList[4];
            mazePixelYEnd = selectedList[5];

            // This initializes the middle point on the x-axis.
            screenPixelXCenter = selectedList[6];

            // This initializes the y-axis start and distance between for drawing maze sizes on the loading screen.
            loadScreenSizeStart = selectedList[7];
            loadScreenSizeDisplacement = selectedList[8];

            // This initializes the y-axis current, start, and end position of the arrow sprite in the loading screen.
            loadScreenArrowCurrent = selectedList[9]; // This changes
            loadScreenArrowStart = selectedList[9];
            loadScreenArrowEnd = selectedList[10];

            // This initializes values need to help when drawing the border of the maze.
            borderPixel1 = selectedList[11];
            borderPixel2 = selectedList[12];
            borderPixel3 = selectedList[13];
            borderPixel4 = selectedList[14];

            // This initializes the y-axis positions for different strings that need to be drawn
            drawPositionY1 = selectedList[15];
            drawPositionY2 = selectedList[16];
            drawPositionY3 = selectedList[17];
            drawPositionY4 = selectedList[18];

            // These initializes the sprites/images I have.
            titleFont = Content.Load<SpriteFont>("fontTitle");
            creatorFont = Content.Load<SpriteFont>("fontCreator");
            otherFont = Content.Load<SpriteFont>("fontCompletedMaze");
            northArrow = Content.Load<Texture2D>("arrow_north");
            eastArrow = Content.Load<Texture2D>("arrow_east");
            southArrow = Content.Load<Texture2D>("arrow_south");
            westArrow = Content.Load<Texture2D>("arrow_west");
            startButton = Content.Load<Texture2D>("Start Button");
            stopSign = Content.Load<Texture2D>("Stop Sign");

            // This initializes the different widths of different displayed text. These are used to center text. 
            textWidth1 = (int)titleFont.MeasureString("Dev-Maze").Length();
            textWidth2 = (int)creatorFont.MeasureString("By  Nathan Russo").Length();
            textWidth3 = (int)otherFont.MeasureString("Select the maze size.").Length();
            textWidth4 = (int)otherFont.MeasureString("Move to the stop sign to win.").Length();
            textWidth5 = (int)otherFont.MeasureString("Good Luck!").Length();
            textWidth6 = (int)otherFont.MeasureString("You win! Close the game.").Length();
            textWidth7 = (int)otherFont.MeasureString("Press 'Space' for another maze.").Length();

            // This initializes the Vectors needed to draw the title, the creator, and other text for the game.
            titleVector = new Vector2((windowSize.Width - textWidth1) / 2, drawPositionY1);
            creatorVector = new Vector2((windowSize.Width - textWidth2) / 2, drawPositionY2);
            selectionVector = new Vector2((windowSize.Width - textWidth3) / 2, drawPositionY3);
            directionVector = new Vector2((windowSize.Width - textWidth4) / 2, drawPositionY3);
            luckVector = new Vector2((windowSize.Width - textWidth5) / 2, drawPositionY4);
            winVector = new Vector2((windowSize.Width - textWidth6) / 2, drawPositionY3);
            restartVector = new Vector2((windowSize.Width - textWidth7) / 2, drawPositionY4);

            // This initializes the vectors needed to draw the different possible maze sizes in the loading screen.
            vPosition_50_25 = new Vector2(screenPixelXCenter, loadScreenSizeStart);
            vPosition_40_20 = new Vector2(screenPixelXCenter, loadScreenSizeStart + loadScreenSizeDisplacement);
            vPosition_32_16 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 2 * loadScreenSizeDisplacement);
            vPosition_20_10 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 3 * loadScreenSizeDisplacement);
            vPosition_16_8 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 4 * loadScreenSizeDisplacement);
            vPosition_10_5 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 5 * loadScreenSizeDisplacement);
            vPosition_8_4 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 6 * loadScreenSizeDisplacement);
            vPosition_4_2 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 7 * loadScreenSizeDisplacement);
            vPosition_2_1 = new Vector2(screenPixelXCenter, loadScreenSizeStart + 8 * loadScreenSizeDisplacement);

            /** This initializes the rectangles needed to draw the four borders of the maze.
             *  northRectangle = new Rectangle(7, 150, 407, 3) or (31, 375, 1021, 9);   
             *  eastRectangle = new Rectangle(410, 150, 4, 800) or (1040, 375, 12, 2000);
             *  southRectangle = new Rectangle(7, 950, 407, 3) or (31, 2375, 1021, 9);
             *  westRectangle = new Rectangle(7, 150, 4, 800) or (31, 375, 12, 2000);  */
            northRectangle = new Rectangle(borderPixel1, mazePixelYStart, borderPixel4, borderPixel3);
            eastRectangle = new Rectangle(mazePixelXEnd, mazePixelYStart, borderPixel2, mazePixelHeight);
            southRectangle = new Rectangle(borderPixel1, mazePixelYEnd, borderPixel4, borderPixel3);
            westRectangle = new Rectangle(borderPixel1, mazePixelYStart, borderPixel2, mazePixelHeight);
        }


        /// Your main update loop. This runs once every frame, over and over.
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Update(GameTime gameTime)
		{
			Input.Update(); // Updates the state of the input library

            // Exit when both menu buttons are pressed (or escape for keyboard debugging)
            // You can change this but it is suggested to keep the key bind of both menu
            // buttons at once for a graceful exit.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                (Input.GetButton(1, Input.ArcadeButtons.Menu) &&
                Input.GetButton(2, Input.ArcadeButtons.Menu)))
            {
                Exit();
            }

            // This will send the user back to the loading screen if true.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Input.GetButton(1, Input.ArcadeButtons.Menu))
            {
                screenType = ScreenType.LoadingScreen;
                button_pressed = 0;
                completedMaze = false;
            }

            // Performs the update commands based on whether we are in the loading screen or in the maze screen.
            if (screenType == ScreenType.LoadingScreen) // If we are in the loading screen
            {
                // This changed the arrow's y_axis position based off of the user's input.
                if (button_pressed == 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        if (loadScreenArrowCurrent != loadScreenArrowEnd) 
                        {
                            loadScreenArrowCurrent -= loadScreenSizeDisplacement;
                        }
                        button_pressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        if (loadScreenArrowCurrent != loadScreenArrowStart)
                        {
                            loadScreenArrowCurrent += loadScreenSizeDisplacement;
                        }
                        button_pressed = 3;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (button_pressed == 1) { button_pressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (button_pressed == 3) { button_pressed = 0; }
                    }
                }

                // This initializes the current position of the arrow in the loading screen.
                vCurrentPosition = new Rectangle(mazePixelYStart, loadScreenArrowCurrent, 40, 40);

                // This initializes the maze and changes the screen type if the user has selected a maze size.
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Input.GetButton(1, Input.ArcadeButtons.A1))
                {
                    // This calls initializeMaze() based off of the arrow's position.
                    switch (loadScreenArrowCurrent)
                    {
                        // Computer cases
                        case 127:  initializeMaze(50, 25);  break;
                        case 217:  initializeMaze(40, 20);  break;
                        case 307:  initializeMaze(32, 16);  break;
                        case 397:  initializeMaze(20, 10);  break;
                        case 487:  initializeMaze(16, 8);   break;
                        case 577:  initializeMaze(10, 5);   break;
                        case 667:  initializeMaze(8, 4);    break;
                        case 757:  initializeMaze(4, 2);    break;
                        case 847:  initializeMaze(2, 1);    break;
                        // Devcade cases
                        case 322:  initializeMaze(50, 25);  break;
                        case 547:  initializeMaze(40, 20);  break;
                        // case 772:  initializeMaze(32, 16);  break;
                        case 997:  initializeMaze(20, 10);  break;
                        case 1222: initializeMaze(16, 8);   break;
                        case 1447: initializeMaze(10, 5);   break;
                        case 1672: initializeMaze(8, 4);    break;
                        case 1897: initializeMaze(4, 2);    break;
                        case 2122: initializeMaze(2, 1);    break;
                    }

                    // This sets the current screen type to the maze screen.
                    screenType = ScreenType.MazeScreen;
                }
            } 
            else if (screenType == ScreenType.MazeScreen) // If we are in the maze screen
            {
                // This updates the completed variable if your make it to the end of the maze.
                if (x_index == topRightX && y_index == topRightY && completedMaze == false)
                {
                    completedMaze = true;
                }

                // This takes the x, y coordinates and makes each a matching value to fit in the grid.
                converted_x = (x_index - bottomLeftX) / blockSizeC;
                converted_y = (bottomLeftY - y_index) / blockSizeC;

                // This is the block value in the grid based off of your position.
                Block current = blockGrid1.getBlockAt(converted_y, converted_x);

                // This changes the direction variable of the arrow.
                // It also changes the position variables if it is possible.
                if (button_pressed == 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        direction = "N";
                        if (y_index != topRightY && !current.hasNorthWall()) { y_index -= blockSizeC; }
                        button_pressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                    {
                        direction = "E";
                        if (x_index != topRightX && !current.hasEastWall()) { x_index += blockSizeC; }
                        button_pressed = 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        direction = "S";
                        if (y_index != bottomLeftY && !current.hasSouthWall()) { y_index += blockSizeC; }
                        button_pressed = 3;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                    {
                        direction = "W";
                        if (x_index != bottomLeftX && !current.hasWestWall()) { x_index -= blockSizeC; }
                        button_pressed = 4;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (button_pressed == 1) { button_pressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.D) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickRight))
                    {
                        if (button_pressed == 2) { button_pressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (button_pressed == 3) { button_pressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.A) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickLeft))
                    {
                        if (button_pressed == 4) { button_pressed = 0; }
                    }
                }
            } 
            
			base.Update(gameTime);
		}


        /// Your main draw loop. This runs once every frame, over and over.
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Draw(GameTime gameTime)
		{
            GraphicsDevice.Clear(Color.DarkMagenta);

            // Batches all the draw calls for this frame, and then performs them all at once
            _spriteBatch.Begin();

            // This draws the title and creator.
            _spriteBatch.DrawString(titleFont, "Dev-Maze", titleVector, Color.White);
            _spriteBatch.DrawString(creatorFont, "By  Nathan Russo", creatorVector, Color.White);

            // Performs the draw commands based on whether we are in the loading screen or in the maze screen.
            if (screenType == ScreenType.LoadingScreen)
            {
                // This draws the loading screen instructions.
                _spriteBatch.DrawString(otherFont, "Select the maze size.", selectionVector, Color.White);

                // This draws the different maze size options.
                _spriteBatch.DrawString(otherFont, "50 by 25", vPosition_50_25, Color.White);
                _spriteBatch.DrawString(otherFont, "40 by 20", vPosition_40_20, Color.White);
                _spriteBatch.DrawString(otherFont, "32 by 16", vPosition_32_16, Color.White);
                _spriteBatch.DrawString(otherFont, "20 by 10", vPosition_20_10, Color.White);
                _spriteBatch.DrawString(otherFont, "16 by 8", vPosition_16_8, Color.White);
                _spriteBatch.DrawString(otherFont, "10 by 5", vPosition_10_5, Color.White);
                _spriteBatch.DrawString(otherFont, "8 by 4", vPosition_8_4, Color.White);
                _spriteBatch.DrawString(otherFont, "4 by 2", vPosition_4_2, Color.White);
                _spriteBatch.DrawString(otherFont, "2 by 1", vPosition_2_1, Color.White);

                // This draws the arrow at its current position/which size it's pointed at.
                _spriteBatch.Draw(eastArrow, vCurrentPosition, Color.White);
            }
            else if (screenType == ScreenType.MazeScreen)
            {
                // This draws all 4 of the maze borders.
                _spriteBatch.Draw(line, northRectangle, Color.White); // North border
                _spriteBatch.Draw(line, eastRectangle, Color.White); // East border
                _spriteBatch.Draw(line, southRectangle, Color.White); // South border 
                _spriteBatch.Draw(line, westRectangle, Color.White); // West border

                // This draws the start button and stop sign.
                _spriteBatch.Draw(startButton, startRectangle, Color.White);
                _spriteBatch.Draw(stopSign, stopRectangle, Color.White);

                // This draws the maze for blockGrid1.
                blockGrid1.drawMaze(mazePixelXStart, mazePixelYEnd, line, _spriteBatch);

                // This displays a message when you have or have not completed the maze.
                if (!completedMaze)
                {
                    _spriteBatch.DrawString(otherFont, "Move to the stop sign to win.", directionVector, Color.White);
                    _spriteBatch.DrawString(otherFont, "Good Luck!", luckVector, Color.White);
                }
                else
                {
                    _spriteBatch.DrawString(otherFont, "You win! Close the game.", winVector, Color.White);
                    _spriteBatch.DrawString(otherFont, "Press \"Space\" for another maze.", restartVector, Color.White);
                }

                // This creates the current Rectangle for the arrow based on the current position
                Rectangle arrowPosition = new Rectangle(x_index, y_index, blockSize8, blockSize8);

                // This draws the corresponding arrow based off of the current direction and x & y position.
                if (direction == "N") { _spriteBatch.Draw(northArrow, arrowPosition, Color.White); }
                else if (direction == "E") { _spriteBatch.Draw(eastArrow, arrowPosition, Color.White); }
                else if (direction == "S") { _spriteBatch.Draw(southArrow, arrowPosition, Color.White); }
                else if (direction == "W") { _spriteBatch.Draw(westArrow, arrowPosition, Color.White); }
            }

            _spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}