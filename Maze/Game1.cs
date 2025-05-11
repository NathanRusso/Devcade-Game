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
        private int buttonPressed;

        // This is a boolean used to determine when you made it to the end of the maze.
        private bool completedMaze;

        // This declares the current position of the arrow in the loading screen.
        private Rectangle vCurrentPosition;

        // This declares the current position of the arrow in the maze screen.
        private Rectangle arrowPosition;

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

        // This declares both list of variables that must be used depending on if the game is running in Devcade.
        private List<int> selectedList1;
        private List<String> selectedList2;

        // This declares the height and width of the maze in pixels.
        private int mazePixelHeight, mazePixelWidth;

        // This declares the start and end pixel locations for the x-axis and y-axis of the maze.
        private int mazePixelLeftX, mazePixelRightX, mazePixelTopY, mazePixelBottomY;

        // This declares the middle point on the x-axis.
        private int screenPixelXCenter;

        // This declares the y-axis start and distance between for drawing maze sizes on the loading screen.
        private int loadScreenSizeStart, loadScreenSizeDisplacement;

        // This declares the y-axis current, start, and end position of the arrow sprite in the loading screen.
        private int loadScreenArrowCurrent, loadScreenArrowStart, loadScreenArrowEnd;

        // This declares values need to help when drawing the border of the maze.
        private int borderPixel1, borderPixel2, borderPixel3, borderPixel4;

        // This declares the y-axis positions for different strings that need to be drawn.
        private int drawPositionY1, drawPositionY2, drawPositionY3, drawPositionY4;

        // A BlockGrid with a 2D array of Blocks which makes up the grid for the maze.
        private BlockGrid blockGrid1;

        // This declares the block value in the grid based off of the current position.
        private Block currentBlock;

        // This declares the size of the block for the maze, along with a smaller versions.
        private int blockSize100, blockSize90, blockSize80, blockSize10;

        // This declares the start and end pixel locations for the player on the x-axis and y-axis of the maze.
        private int playerPixelRightX, playerPixelLeftX, playerPixelTopY, playerPixelBottomY;

        // These are integers and a string set at your current position and direction. These will change as you move.
        private int xIndex, yIndex;
        private string direction;

        // These are integers which will be used to convert pixel position to grid position.
        private int convertedX, convertedY;

        // This declares the Texture2D needed to draw the maze lines.
        private Texture2D line;

        // This declares the SpriteFont needed to draw the maze title, creator name, other maze message.
        private SpriteFont titleFont, creatorFont, otherFont;

        // This declares the Texture2Ds needed to draw the image sprites.
        private Texture2D northArrow, eastArrow, southArrow, westArrow, startButton, stopSign;

        // This declares the Rectangles needed to draw the maze borders.
        private Rectangle northRectangle, eastRectangle, southRectangle, westRectangle;

        // This declares the start button and stop sign.
        private Rectangle startRectangle, stopRectangle;

        // This declares the different widths of different displayed text. These are used to center text.
        private int textWidth1, textWidth2, textWidth3, textWidth4, textWidth5, textWidth6, textWidth7;

        // This initializes the Vectors needed to draw the title, the creator, and other text for the game.
        private Vector2 titleVector, creatorVector, selectionVector, directionVector, luckVector, winVector, restartVector;

        // This declares the display texts that depend on if the game is running in Devcade.
        private string text3, text7;

        // This declares 3 of the texts that show the possible maze sizes.
        // The options are slightly different depending on if the game is running in Devcade.
        private string mazeSizeText1, mazeSizeText2, mazeSizeText3;


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
        public void InitializeMaze(int rows, int columns)
        {
            // It constructs a BlockGrid with a 2D array of Blocks that has a given number of rows and columns. 
            // 800 x 400 pixels --> 1000 x 2000 pixels
            blockGrid1 = new BlockGrid(rows, columns, mazePixelWidth);
            /* Y by X, with block sizes.
             * 80 by 40, with a block size of 10 --> 25
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

            // It sets the full and smaller block sizes based off of the current blockGrid.
            blockSize100 = blockGrid1.GetBlockSize();
            blockSize90 = Convert.ToInt32(blockSize100 * 0.9);
            blockSize80 = Convert.ToInt32(blockSize100 * 0.8);
            blockSize10 = blockSize100 - blockSize90;

            // This initializes the start and end pixel locations for the player on the x-axis and y-axis of the maze.
            playerPixelRightX = mazePixelRightX - blockSize90;
            playerPixelTopY = mazePixelTopY + blockSize10;
            playerPixelLeftX = mazePixelLeftX + blockSize10;
            playerPixelBottomY = mazePixelBottomY - blockSize90;

            // This sets your starting position and direction.
            xIndex = playerPixelLeftX;
            yIndex = playerPixelBottomY;
            direction = "N";

            // This generates a maze for blockGrid1.
            blockGrid1.GenerateMaze();

            // This initializes the start button and stop sign.
            startRectangle = new Rectangle(playerPixelLeftX, playerPixelBottomY, blockSize80, blockSize80);
            stopRectangle = new Rectangle(playerPixelRightX, playerPixelTopY, blockSize80, blockSize80);
        }


        /// Performs any setup that doesn't require loaded content before the first frame.
        protected override void Initialize()
		{
			// Sets up the input library
			Input.Initialize();
            
			// Set window size if running debug (in release it will be full screen)
            // Original computer screen dimensions 420 x 980.
            // Original Devcade screen dimensions 1080 x 2560.
            // New computer screen dimensions 540 x 960.
            // New Devcade screen dimensions 2160 x 3840.
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
            buttonPressed = 0;
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

            // This initializes both list of variables that must be used
            // depending on if the game is running in Devcade.
            if (windowSize.Width == 420) // The game is on my computer.
            {
                selectedList1 = new List<int> { 800, 400, 10, 410, 150, 950, 210, 130, 90, 847, 127,
                                               7, 4, 3, 407, 0, 40, 70, 105};
                selectedList2 = new List<string> { "Select the maze size.", "Press 'Back' for another maze.", 
                    "50 by 25", "40 by 20", "32 by 16", "fontTitle", "fontCreator", "fontCompletedMaze" };
            }
            else // The game is on Devcade.
            { 
                selectedList1 = new List<int> { 2000, 1000, 40, 1040, 375, 2375, 540, 325, 225, 2122, 322,
                                               31, 12, 9, 1021, 0, 100, 175, 260};
                selectedList2 = new List<string> { "Press A1 to select a maze size.", "Press Menu1 for another maze.",
                    "80 by 40", "50 by 25", "40 by 20", "fontTitle2", "fontCreator2", "fontCompletedMaze2" } ;
            }

            // This initializes the height and width of the maze in pixels.
            mazePixelHeight = selectedList1[0];
            mazePixelWidth = selectedList1[1];

            // This initializes the start and end pixel locations for the x-axis and y-axis of the maze.
            mazePixelLeftX = selectedList1[2];
            mazePixelRightX = selectedList1[3];
            mazePixelTopY = selectedList1[4];
            mazePixelBottomY = selectedList1[5];

            // This initializes the middle point on the x-axis.
            screenPixelXCenter = selectedList1[6];

            // This initializes the y-axis start and distance between for drawing maze sizes on the loading screen.
            loadScreenSizeStart = selectedList1[7];
            loadScreenSizeDisplacement = selectedList1[8];

            // This initializes the y-axis current, start, and end position of the arrow sprite in the loading screen.
            loadScreenArrowCurrent = selectedList1[9]; // This changes
            loadScreenArrowStart = selectedList1[9];
            loadScreenArrowEnd = selectedList1[10];

            // This initializes values need to help when drawing the border of the maze.
            borderPixel1 = selectedList1[11];
            borderPixel2 = selectedList1[12];
            borderPixel3 = selectedList1[13];
            borderPixel4 = selectedList1[14];

            // This initializes the y-axis positions for different strings that need to be drawn
            drawPositionY1 = selectedList1[15];
            drawPositionY2 = selectedList1[16];
            drawPositionY3 = selectedList1[17];
            drawPositionY4 = selectedList1[18];

            // This initializes the display texts that depend on if the game is running in Devcade.
            text3 = selectedList2[0];
            text7 = selectedList2[1];

            // This initializes 3 of the texts that show the possible maze sizes.
            mazeSizeText1 = selectedList2[2];
            mazeSizeText2 = selectedList2[3];
            mazeSizeText3 = selectedList2[4];

            // These initializes the sprites/images I have that change if the game is on Devcade.
            titleFont = Content.Load<SpriteFont>(selectedList2[5]);
            creatorFont = Content.Load<SpriteFont>(selectedList2[6]);
            otherFont = Content.Load<SpriteFont>(selectedList2[7]);

            // These initializes the other sprites/images I have.
            northArrow = Content.Load<Texture2D>("arrow_north");
            eastArrow = Content.Load<Texture2D>("arrow_east");
            southArrow = Content.Load<Texture2D>("arrow_south");
            westArrow = Content.Load<Texture2D>("arrow_west");
            startButton = Content.Load<Texture2D>("Start Button");
            stopSign = Content.Load<Texture2D>("Stop Sign");

            // This initializes the different widths of different displayed text. These are used to center text. 
            textWidth1 = (int)titleFont.MeasureString("Dev-Maze").Length();
            textWidth2 = (int)creatorFont.MeasureString("By  Nathan Russo").Length();
            textWidth3 = (int)otherFont.MeasureString(text3).Length();
            textWidth4 = (int)otherFont.MeasureString("Move to the stop sign to win.").Length();
            textWidth5 = (int)otherFont.MeasureString("Good Luck!").Length();
            textWidth6 = (int)otherFont.MeasureString("You win! Close the game.").Length();
            textWidth7 = (int)otherFont.MeasureString(text7).Length();

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
            northRectangle = new Rectangle(borderPixel1, mazePixelTopY - borderPixel3 + 1, borderPixel4, borderPixel3);
            eastRectangle = new Rectangle(mazePixelRightX, mazePixelTopY, borderPixel2, mazePixelHeight);
            southRectangle = new Rectangle(borderPixel1, mazePixelBottomY, borderPixel4, borderPixel3);
            westRectangle = new Rectangle(borderPixel1, mazePixelTopY, borderPixel2, mazePixelHeight);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Back) || Input.GetButton(1, Input.ArcadeButtons.Menu))
            {
                screenType = ScreenType.LoadingScreen;
                buttonPressed = 0;
                completedMaze = false;
            }

            // Performs the update commands based on whether we are in the loading screen or in the maze screen.
            if (screenType == ScreenType.LoadingScreen) // If we are in the loading screen
            {
                // This changed the arrow's y axis position based off of the user's input.
                if (buttonPressed == 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        if (loadScreenArrowCurrent != loadScreenArrowEnd) 
                        {
                            loadScreenArrowCurrent -= loadScreenSizeDisplacement;
                        }
                        buttonPressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        if (loadScreenArrowCurrent != loadScreenArrowStart)
                        {
                            loadScreenArrowCurrent += loadScreenSizeDisplacement;
                        }
                        buttonPressed = 3;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (buttonPressed == 1) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (buttonPressed == 3) { buttonPressed = 0; }
                    }
                }

                // This initializes the current position of the arrow in the loading screen.
                vCurrentPosition = new Rectangle(mazePixelTopY, loadScreenArrowCurrent, drawPositionY2, drawPositionY2);

                // This initializes the maze and changes the screen type if the user has selected a maze size.
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Input.GetButton(1, Input.ArcadeButtons.A1))
                {
                    // This calls initializeMaze() based off of the arrow's position.
                    switch (loadScreenArrowCurrent)
                    {
                        // Computer cases:
                        case 127:  InitializeMaze(50, 25);  break;
                        case 217:  InitializeMaze(40, 20);  break;
                        case 307:  InitializeMaze(32, 16);  break;
                        case 397:  InitializeMaze(20, 10);  break;
                        case 487:  InitializeMaze(16, 8);   break;
                        case 577:  InitializeMaze(10, 5);   break;
                        case 667:  InitializeMaze(8, 4);    break;
                        case 757:  InitializeMaze(4, 2);    break;
                        case 847:  InitializeMaze(2, 1);    break;
                        // Devcade cases (NO 32 by 16):
                        case 322:  InitializeMaze(80, 40);  break;
                        case 547:  InitializeMaze(50, 25);  break;
                        case 772:  InitializeMaze(40, 20);  break;
                        case 997:  InitializeMaze(20, 10);  break;
                        case 1222: InitializeMaze(16, 8);   break;
                        case 1447: InitializeMaze(10, 5);   break;
                        case 1672: InitializeMaze(8, 4);    break;
                        case 1897: InitializeMaze(4, 2);    break;
                        case 2122: InitializeMaze(2, 1);    break;
                    }

                    // This sets the current screen type to the maze screen.
                    screenType = ScreenType.MazeScreen;
                }
            } 
            else if (screenType == ScreenType.MazeScreen) // If we are in the maze screen
            {
                // This updates the completed variable if your make it to the end of the maze.
                if (xIndex == playerPixelRightX && yIndex == playerPixelTopY && completedMaze == false)
                {
                    completedMaze = true;
                }

                // This takes the x, y coordinates and makes each a matching value to fit in the grid.
                convertedX = (xIndex - playerPixelLeftX) / blockSize100;
                convertedY = (yIndex - playerPixelTopY) / blockSize100;

                // This initializes the block value in the grid based off of the current position.
                currentBlock = blockGrid1.GetBlockAt(convertedY, convertedX);

                // This changes the direction variable of the arrow.
                // It also changes the position variables if it is possible.
                if (buttonPressed == 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        direction = "N";
                        if (yIndex != playerPixelTopY && !currentBlock.HasNorthWall()) { yIndex -= blockSize100; }
                        buttonPressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                    {
                        direction = "E";
                        if (xIndex != playerPixelRightX && !currentBlock.HasEastWall()) { xIndex += blockSize100; }
                        buttonPressed = 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        direction = "S";
                        if (yIndex != playerPixelBottomY && !currentBlock.HasSouthWall()) { yIndex += blockSize100; }
                        buttonPressed = 3;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) ||
                         Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                    {
                        direction = "W";
                        if (xIndex != playerPixelLeftX && !currentBlock.HasWestWall()) { xIndex -= blockSize100; }
                        buttonPressed = 4;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (buttonPressed == 1) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.D) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickRight))
                    {
                        if (buttonPressed == 2) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (buttonPressed == 3) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.A) ||
                         Input.GetButtonUp(1, Input.ArcadeButtons.StickLeft))
                    {
                        if (buttonPressed == 4) { buttonPressed = 0; }
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
                _spriteBatch.DrawString(otherFont, text3, selectionVector, Color.White);

                // This draws the different maze size options.
                _spriteBatch.DrawString(otherFont, mazeSizeText1, vPosition_50_25, Color.White);
                _spriteBatch.DrawString(otherFont, mazeSizeText2, vPosition_40_20, Color.White);
                _spriteBatch.DrawString(otherFont, mazeSizeText3, vPosition_32_16, Color.White);
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
                blockGrid1.DrawMaze(mazePixelLeftX, mazePixelTopY, line, _spriteBatch);

                // This displays a message when you have or have not completed the maze.
                if (!completedMaze)
                {
                    _spriteBatch.DrawString(otherFont, "Move to the stop sign to win.", directionVector, Color.White);
                    _spriteBatch.DrawString(otherFont, "Good Luck!", luckVector, Color.White);
                }
                else
                {
                    _spriteBatch.DrawString(otherFont, "You win! Close the game.", winVector, Color.White);
                    _spriteBatch.DrawString(otherFont, text7, restartVector, Color.White);
                }

                // This initializes the current position of the arrow in the maze screen.
                arrowPosition = new Rectangle(xIndex, yIndex, blockSize80, blockSize80);

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