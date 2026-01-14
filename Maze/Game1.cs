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
        LoadingScreen, MazeScreen
    }


    public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

        // This stores the window dimensions in a rectangle object for easy use
        private Rectangle windowSize;

        // This determines which screen type we are in.
        private ScreenType screenType;

        // This is used to increase pixel numbers if running on Devcade.
        private int devcadeMultiplier;

        // This is an integer used to determine how to move the arrow in both screen types.
        private int buttonPressed;

        // This is a boolean used to determine when you made it to the end of the maze.
        private bool completedMaze;

        // This declares both list of variables that must be used depending on if the game is running in Devcade.
        private List<string> selectedList2;

        // This declares the current position of the arrow in the loading screen.
        private Rectangle loadArrow;

        // This declares the current position of the arrow in the maze screen.
        private Rectangle mazeArrow;

        // This declares the vectors needed to draw the different possible maze sizes in the loading screen.
        private Vector2 sizePosition_80_50; // 800x500 -> 3200x2000
        private Vector2 sizePosition_72_45; // 792x495 -> 3168x1980
        private Vector2 sizePosition_61_38; // 793x494 -> 3172x1976
        private Vector2 sizePosition_50_31; // 800x496 -> 3200x1984
        private Vector2 sizePosition_44_27; // 792x486 -> 3168x1944
        private Vector2 sizePosition_40_25; // 800x500 -> 3200x2000
        private Vector2 sizePosition_36_22; // 792x484 -> 3168x1936
        private Vector2 sizePosition_30_19; // 780x486 -> 3120x1944
        private Vector2 sizePosition_25_15; // 800x480 -> 3200x1920
        private Vector2 sizePosition_20_12; // 800x480 -> 3200x1920
        private Vector2 sizePosition_16_10; // 800x500 -> 3200x2000
        private Vector2 sizePosition_10_6;  // 800x480 -> 3200x1920
        private Vector2 sizePosition_8_5;  // 800x500 -> 3200x2000
        private Vector2 sizePosition_5_3;  // 800x480 -> 3200x1920
        private Vector2 sizePosition_4_2; // 800x400 --> 3200x1600
        private Vector2 sizePosition_2_1; // 800x400 --> 3200x1600

        // This declares the load screen values for the possible maze size corners and displacements.
        private int loadSizeTopY, loadSizeLeftX, loadSizeDisplacementY, loadSizeDisplacementX;

        // This declares the load screen values for the arrow corners.
        private int loadArrowLeftX, loadArrowRightX, loadArrowTopY, loadArrowBottomY;

        // This declares the pixel location of the arrow in the load screen.
        private ValueTuple<int, int> loadArrowPosition;

        // This declares the y-axis positions for different strings that need to be drawn.
        private int drawPositionY1, drawPositionY2, drawPositionY3, drawPositionY4;

        // This declares the size of the block for the maze, along with a smaller versions.
        private int blockSize100, blockSize90, blockSize80, blockSize10;

        // This declares the height and width of the maze in pixels.
        private int mazePixelMaxHeight, mazePixelHeight, mazePixelWidth;

        // This declares the start and end pixel locations for the x-axis and y-axis of the maze.
        private int mazePixelLeftX, mazePixelRightX, mazePixelTopY, mazePixelBottomY;

        // This declares the start and end pixel locations for the player on the x-axis and y-axis of the maze.
        private int playerPixelRightX, playerPixelLeftX, playerPixelTopY, playerPixelBottomY;

        // These are integers and a string set at your current position and direction. These will change as you move.
        private int xIndex, yIndex;
        private string direction;

        // These are integers which will be used to convert pixel position to grid position.
        private int convertedX, convertedY;

        // A BlockGrid with a 2D array of Blocks which makes up the grid for the maze.
        private BlockGrid blockGrid1;

        // This declares the block value in the grid based off of the current position.
        private Block currentBlock;

        // This declares the Texture2D needed to draw the maze lines.
        private Texture2D line;

        // This declares the SpriteFont needed to draw the maze title, creator name, other maze message.
        private SpriteFont titleFont, creatorFont, otherFont;

        // This declares the Texture2Ds needed to draw the image sprites.
        private Texture2D northArrow, eastArrow, southArrow, westArrow, startButton, stopSign;

        // This declares the Rectangles needed to draw the maze borders, start button, and stop sign.
        private Rectangle northRectangle, eastRectangle, southRectangle, westRectangle, startRectangle, stopRectangle;

        // This declares the different widths of different displayed text. These are used to center text.
        private int textWidth1, textWidth2, textWidth3, textWidth4, textWidth5, textWidth6, textWidth7;

        // This initializes the Locations needed to draw the title, the creator, and other text for the game.
        private Vector2 titleLocation, creatorLocation, selectionLocation, directionLocation, luckLocation, winLocation, restartLocation;

        // This declares the display texts that depend on if the game is running in Devcade.
        private string text3, text7;


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
            // This initializes the mazePixelHeight, mazePixelWidth, and block sizes based off of the selected maze size.
            blockSize100 = (int)Math.Floor( (double)mazePixelMaxHeight / rows ); // 100%
            blockSize90 = Convert.ToInt32(blockSize100 * 0.9); // 90%
            blockSize80 = Convert.ToInt32(blockSize100 * 0.8); // 80%
            blockSize10 = blockSize100 - blockSize90; // 10%
            mazePixelHeight = blockSize100 * rows;
            mazePixelWidth = blockSize100 * columns;

            // This initializes the start and end pixel locations for the x-axis and y-axis of the maze.
            mazePixelLeftX = (int)Math.Floor( (double)(windowSize.Width - mazePixelWidth) / 2);
            mazePixelRightX = mazePixelLeftX + mazePixelWidth;
            mazePixelBottomY = mazePixelTopY + mazePixelHeight;

            // This initializes the start and end pixel locations for the player on the x-axis and y-axis of the maze.
            playerPixelRightX = mazePixelRightX - blockSize90;
            playerPixelTopY = mazePixelTopY + blockSize10;
            playerPixelLeftX = mazePixelLeftX + blockSize10;
            playerPixelBottomY = mazePixelBottomY - blockSize90;

            // This sets your starting position and direction.
            xIndex = playerPixelLeftX;
            yIndex = playerPixelBottomY;
            direction = "N";

            // This initializes the rectangles needed to draw the four borders of the maze.
            int h3 = 3 * devcadeMultiplier;
            int h4 = 4 * devcadeMultiplier;
            int h6 = 6 * devcadeMultiplier;
            northRectangle = new Rectangle(mazePixelLeftX - h3, mazePixelTopY - h3, mazePixelWidth + h6, h4);
            eastRectangle = new Rectangle(mazePixelRightX, mazePixelTopY - h3, h4, mazePixelHeight + h6);
            southRectangle = new Rectangle(mazePixelLeftX - h3, mazePixelBottomY, mazePixelWidth + h6, h4);
            westRectangle = new Rectangle(mazePixelLeftX - h3, mazePixelTopY - h3, h4, mazePixelHeight + h6);

            // This initializes the start button and stop sign.
            startRectangle = new Rectangle(playerPixelLeftX, playerPixelBottomY, blockSize80, blockSize80);
            stopRectangle = new Rectangle(playerPixelRightX, playerPixelTopY, blockSize80, blockSize80);

            // It constructs a BlockGrid with a 2D array of Blocks that has a given number of rows and columns. 
            blockGrid1 = new BlockGrid(rows, columns, blockSize100);

            // This generates a maze for blockGrid1.
            blockGrid1.GenerateMaze();
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
			_graphics.PreferredBackBufferWidth = 540;
			_graphics.PreferredBackBufferHeight = 960;
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

            // This sets the devcadeMultiplier based off of the current window size.
            if (windowSize.Width == 540) { devcadeMultiplier = 1; /*The game is on my computer.*/ }
            else { devcadeMultiplier = 4; /*The game is on Devcade.*/ }
            
			base.Initialize();
	    }


        /// Performs any setup that requires loaded content before the first frame.
        protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

            // This initializes line to draw the lines, which includes the borders and inner lines.
            line = new Texture2D(GraphicsDevice, 1, 1);
            line.SetData(new[] { Color.White });

            // This initializes both list of variables that must be used depending on if the game is running in Devcade.
            if (windowSize.Width == 540) // The game is on my computer.
            {
                selectedList2 = new List<string> { "Select the maze size.", "Press 'Back' for another maze.", 
                    "fontTitle", "fontCreator", "fontCompletedMaze" };
            }
            else // The game is on Devcade.
            {
                selectedList2 = new List<string> { "Press A1 to select a maze size.", "Press Menu1 for another maze.",
                    "fontTitle2", "fontCreator2", "fontCompletedMaze2" };
            }

            // This initializes the height and width of the maze in pixels.
            mazePixelMaxHeight = 800 * devcadeMultiplier;
            mazePixelTopY = 150 * devcadeMultiplier;

            // This initializes the load screen values for the possible maze size corners and displacements.
            loadSizeTopY = 160 * devcadeMultiplier;
            loadSizeLeftX = (int)Math.Floor( (double)windowSize.Width * 2 / 9 );
            loadSizeDisplacementY = (int)Math.Floor( (double)(windowSize.Height - loadSizeTopY) / 8 );
            loadSizeDisplacementX = (int)Math.Floor( (double)windowSize.Width / 3 );

            // This initializes the load screen values for the arrow corners and position.
            loadArrowLeftX = loadSizeLeftX - (50 * devcadeMultiplier);
            loadArrowRightX = loadArrowLeftX + loadSizeDisplacementX;
            loadArrowTopY = loadSizeTopY - (2 * devcadeMultiplier);
            loadArrowBottomY = loadArrowTopY + 7 * devcadeMultiplier * loadSizeDisplacementY;
            loadArrowPosition = new ValueTuple<int, int>(loadArrowBottomY, loadArrowLeftX);

            // This initializes the y-axis positions for different strings that need to be drawn
            drawPositionY1 = 0 * devcadeMultiplier;
            drawPositionY2 = 40 * devcadeMultiplier;
            drawPositionY3 = 70 * devcadeMultiplier;
            drawPositionY4 = 105 * devcadeMultiplier;

            // This initializes the display texts that depend on if the game is running in Devcade.
            text3 = selectedList2[0];
            text7 = selectedList2[1];

            // These initializes the sprites/images I have that change if the game is on Devcade.
            titleFont = Content.Load<SpriteFont>(selectedList2[2]);
            creatorFont = Content.Load<SpriteFont>(selectedList2[3]);
            otherFont = Content.Load<SpriteFont>(selectedList2[4]);

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
            titleLocation = new Vector2((windowSize.Width - textWidth1) / 2, drawPositionY1);
            creatorLocation = new Vector2((windowSize.Width - textWidth2) / 2, drawPositionY2);
            selectionLocation = new Vector2((windowSize.Width - textWidth3) / 2, drawPositionY3);
            directionLocation = new Vector2((windowSize.Width - textWidth4) / 2, drawPositionY3);
            luckLocation = new Vector2((windowSize.Width - textWidth5) / 2, drawPositionY4);
            winLocation = new Vector2((windowSize.Width - textWidth6) / 2, drawPositionY3);
            restartLocation = new Vector2((windowSize.Width - textWidth7) / 2, drawPositionY4);

            // This initializes the vectors needed to draw the different possible maze sizes in the loading screen.
            sizePosition_80_50 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY);
            sizePosition_72_45 = new Vector2(loadSizeLeftX, loadSizeTopY);
            sizePosition_61_38 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + loadSizeDisplacementY);
            sizePosition_50_31 = new Vector2(loadSizeLeftX, loadSizeTopY + loadSizeDisplacementY);
            sizePosition_44_27 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 2 * loadSizeDisplacementY);
            sizePosition_40_25 = new Vector2(loadSizeLeftX, loadSizeTopY + 2 * loadSizeDisplacementY);
            sizePosition_36_22 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 3 * loadSizeDisplacementY);
            sizePosition_30_19 = new Vector2(loadSizeLeftX, loadSizeTopY + 3 * loadSizeDisplacementY);
            sizePosition_25_15 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 4 * loadSizeDisplacementY);
            sizePosition_20_12 = new Vector2(loadSizeLeftX, loadSizeTopY + 4 * loadSizeDisplacementY);
            sizePosition_16_10 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 5 * loadSizeDisplacementY);
            sizePosition_10_6 = new Vector2(loadSizeLeftX, loadSizeTopY + 5 * loadSizeDisplacementY);
            sizePosition_8_5 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 6 * loadSizeDisplacementY);
            sizePosition_5_3 = new Vector2(loadSizeLeftX, loadSizeTopY + 6 * loadSizeDisplacementY);
            sizePosition_4_2 = new Vector2(loadSizeLeftX + loadSizeDisplacementX, loadSizeTopY + 7 * loadSizeDisplacementY);
            sizePosition_2_1 = new Vector2(loadSizeLeftX, loadSizeTopY + 7 * loadSizeDisplacementY);
        }


        /// Your main update loop. This runs once every frame, over and over.
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Update(GameTime gameTime)
		{
			Input.Update(); // Updates the state of the input library

            // Exit when both menu buttons are pressed (or escape for keyboard debugging)
            // You can change this but it is suggested to keep the key bind of both menu
            // buttons at once for a graceful exit.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (Input.GetButton(1, Input.ArcadeButtons.Menu) && Input.GetButton(2, Input.ArcadeButtons.Menu)))
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
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        if (loadArrowPosition.Item1 != loadArrowTopY) { loadArrowPosition.Item1 -= loadSizeDisplacementY; }
                        buttonPressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) || Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                    {
                        if (loadArrowPosition.Item2 != loadArrowRightX) { loadArrowPosition.Item2 += loadSizeDisplacementX; }
                        buttonPressed = 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) || Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        if (loadArrowPosition.Item1 != loadArrowBottomY) { loadArrowPosition.Item1 += loadSizeDisplacementY; }
                        buttonPressed = 3;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) || Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                    {
                        if (loadArrowPosition.Item2 != loadArrowLeftX) { loadArrowPosition.Item2 -= loadSizeDisplacementX; }
                        buttonPressed = 4;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) || Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (buttonPressed == 1) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.D) || Input.GetButtonUp(1, Input.ArcadeButtons.StickRight))
                    {
                        if (buttonPressed == 2) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) || Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (buttonPressed == 3) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.A) || Input.GetButtonUp(1, Input.ArcadeButtons.StickLeft))
                    {
                        if (buttonPressed == 4) { buttonPressed = 0; }
                    }
                }

                // This initializes the current position of the arrow in the loading screen.
                loadArrow = new Rectangle(loadArrowPosition.Item2, loadArrowPosition.Item1, drawPositionY2, drawPositionY2);

                // This initializes the maze and changes the screen type if the user has selected a maze size.
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Input.GetButton(1, Input.ArcadeButtons.A1))
                {
                    // This calls initializeMaze() based off of the arrow's position.
                    if (loadArrowPosition.Item2 == loadArrowLeftX)
                    {
                        if (loadArrowPosition.Item1 == loadArrowTopY) { InitializeMaze(72, 45); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + loadSizeDisplacementY) { InitializeMaze(50, 31); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 2 * loadSizeDisplacementY) { InitializeMaze(40, 25); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 3 * loadSizeDisplacementY) { InitializeMaze(30, 19); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 4 * loadSizeDisplacementY) { InitializeMaze(20, 12); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 5 * loadSizeDisplacementY) { InitializeMaze(10, 6); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 6 * loadSizeDisplacementY) { InitializeMaze(5, 3); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 7 * loadSizeDisplacementY) { InitializeMaze(2, 1); }
                    }
                    else if (loadArrowPosition.Item2 == loadArrowRightX)
                    {
                        if (loadArrowPosition.Item1 == loadArrowTopY) { InitializeMaze(80, 50); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + loadSizeDisplacementY) { InitializeMaze(61, 38); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 2 * loadSizeDisplacementY) { InitializeMaze(44, 27); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 3 * loadSizeDisplacementY) { InitializeMaze(36, 22); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 4 * loadSizeDisplacementY) { InitializeMaze(25, 15); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 5 * loadSizeDisplacementY) { InitializeMaze(16, 10); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 6 * loadSizeDisplacementY) { InitializeMaze(8, 5); }
                        else if (loadArrowPosition.Item1 == loadArrowTopY + 7 * loadSizeDisplacementY) { InitializeMaze(4, 2); }
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
                    if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W) || Input.GetButtonDown(1, Input.ArcadeButtons.StickUp))
                    {
                        direction = "N";
                        if (yIndex != playerPixelTopY && !currentBlock.HasNorthWall()) { yIndex -= blockSize100; }
                        buttonPressed = 1;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D) || Input.GetButtonDown(1, Input.ArcadeButtons.StickRight))
                    {
                        direction = "E";
                        if (xIndex != playerPixelRightX && !currentBlock.HasEastWall()) { xIndex += blockSize100; }
                        buttonPressed = 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S) || Input.GetButtonDown(1, Input.ArcadeButtons.StickDown))
                    {
                        direction = "S";
                        if (yIndex != playerPixelBottomY && !currentBlock.HasSouthWall()) { yIndex += blockSize100; }
                        buttonPressed = 3;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A) || Input.GetButtonDown(1, Input.ArcadeButtons.StickLeft))
                    {
                        direction = "W";
                        if (xIndex != playerPixelLeftX && !currentBlock.HasWestWall()) { xIndex -= blockSize100; }
                        buttonPressed = 4;
                    }
                }
                else
                {
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.W) || Input.GetButtonUp(1, Input.ArcadeButtons.StickUp))
                    {
                        if (buttonPressed == 1) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.D) || Input.GetButtonUp(1, Input.ArcadeButtons.StickRight))
                    {
                        if (buttonPressed == 2) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.S) || Input.GetButtonUp(1, Input.ArcadeButtons.StickDown))
                    {
                        if (buttonPressed == 3) { buttonPressed = 0; }
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.A) || Input.GetButtonUp(1, Input.ArcadeButtons.StickLeft))
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
            GraphicsDevice.Clear(new Color(0xB0, 0x19, 0x7E)); // CSH Color

            // Batches all the draw calls for this frame, and then performs them all at once
            _spriteBatch.Begin();

            // This draws the title and creator.
            _spriteBatch.DrawString(titleFont, "Dev-Maze", titleLocation, Color.White);
            _spriteBatch.DrawString(creatorFont, "By  Nathan Russo", creatorLocation, Color.White);

            // Performs the draw commands based on whether we are in the loading screen or in the maze screen.
            if (screenType == ScreenType.LoadingScreen)
            {
                // This draws the loading screen instructions.
                _spriteBatch.DrawString(otherFont, text3, selectionLocation, Color.White);

                // This draws the different maze size options.
                _spriteBatch.DrawString(otherFont, "80 by 50", sizePosition_80_50, Color.White);
                _spriteBatch.DrawString(otherFont, "72 by 45", sizePosition_72_45, Color.White);
                _spriteBatch.DrawString(otherFont, "61 by 38", sizePosition_61_38, Color.White);
                _spriteBatch.DrawString(otherFont, "50 by 31", sizePosition_50_31, Color.White);
                _spriteBatch.DrawString(otherFont, "44 by 27", sizePosition_44_27, Color.White);
                _spriteBatch.DrawString(otherFont, "40 by 25", sizePosition_40_25, Color.White);
                _spriteBatch.DrawString(otherFont, "36 by 22", sizePosition_36_22, Color.White);
                _spriteBatch.DrawString(otherFont, "30 by 19", sizePosition_30_19, Color.White);
                _spriteBatch.DrawString(otherFont, "25 by 15", sizePosition_25_15, Color.White);
                _spriteBatch.DrawString(otherFont, "20 by 12", sizePosition_20_12, Color.White);
                _spriteBatch.DrawString(otherFont, "16 by 10", sizePosition_16_10, Color.White);
                _spriteBatch.DrawString(otherFont, "10 by 6", sizePosition_10_6, Color.White);
                _spriteBatch.DrawString(otherFont, "8 by 5", sizePosition_8_5, Color.White);
                _spriteBatch.DrawString(otherFont, "5 by 3", sizePosition_5_3, Color.White);
                _spriteBatch.DrawString(otherFont, "4 by 2", sizePosition_4_2, Color.White);
                _spriteBatch.DrawString(otherFont, "2 by 1", sizePosition_2_1, Color.White);

                // This draws the arrow at its current position/which size it's pointed at.
                _spriteBatch.Draw(eastArrow, loadArrow, Color.White);
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
                    _spriteBatch.DrawString(otherFont, "Move to the stop sign to win.", directionLocation, Color.White);
                    _spriteBatch.DrawString(otherFont, "Good Luck!", luckLocation, Color.White);
                }
                else
                {
                    _spriteBatch.DrawString(otherFont, "You win! Close the game.", winLocation, Color.White);
                    _spriteBatch.DrawString(otherFont, text7, restartLocation, Color.White);
                }

                // This initializes the current position of the arrow in the maze screen.
                mazeArrow = new Rectangle(xIndex, yIndex, blockSize80, blockSize80);

                // This draws the corresponding arrow based off of the current direction and x & y position.
                if (direction == "N") { _spriteBatch.Draw(northArrow, mazeArrow, Color.White); }
                else if (direction == "E") { _spriteBatch.Draw(eastArrow, mazeArrow, Color.White); }
                else if (direction == "S") { _spriteBatch.Draw(southArrow, mazeArrow, Color.White); }
                else if (direction == "W") { _spriteBatch.Draw(westArrow, mazeArrow, Color.White); }
            }

            _spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}