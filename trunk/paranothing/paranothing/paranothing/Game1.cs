using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace paranothing
{
    public enum GameLevel { Title, Description, Level }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        # region Attributes

        SpriteSheet boySheet;
        Texture2D boyTex;
        Texture2D floorTex;
        SpriteSheet stairSheet;
        Texture2D stair;

        GameController control;
        float scale = 2.0f;
        int Width = 400;
        int Height = 300;

        Boy player;
        Stairs stairs;

        Floor f1;
        Floor f2;
        //Fonts
        private SpriteFont gameFont;
        private SpriteFont titleFont;
        //Title
        private GameTitle title;
        private Vector2 startPosition;
        private GameLevel gameState = GameLevel.Level;
        //Description
        private GameBackground description;


        # endregion

        # region Methods

        public GameLevel GameState
        {
            get
            {
                return gameState;
            }
            set
            {
                gameState = value;
            }
        }

        /// <summary>
        /// Draws text on the screen
        /// </summary>
        /// <param name="text">text to write</param>
        /// <param name="textColor">color of text</param>
        /// <param name="x">left hand edge of text</param>
        /// <param name="y">top of text</param>
        private void drawText(string text, SpriteFont font, Color textColor, float x, float y)
        {
            int layer;
            Vector2 vectorText = new Vector2(x, y);

            //solid
            Color backColor = new Color(190, 190, 190);
            for (layer = 0; layer < 3; layer++)
            {
                spriteBatch.DrawString(font, text, vectorText, backColor);
                vectorText.X++;
                vectorText.Y++;
            }

            //top of character
            spriteBatch.DrawString(font, text, vectorText, textColor);
        }

        //Title
        private void loadTitleContents()
        {
            titleFont = Content.Load<SpriteFont>("TitleFont");
            gameFont = Content.Load<SpriteFont>("GameFont");
            title = new GameTitle(Content.Load<Texture2D>("screenshot"), new Rectangle(0, 0, Width, Height));
            title.setBottomTextRectangle(gameFont.MeasureString("Press 'Enter' to start"));
            startPosition = new Vector2(title.BottomTextRectangle.X, title.BottomTextRectangle.Y);
        }
        private void drawTitleText()
        {
            title.setTopTextRectangle(titleFont.MeasureString("Welcome to Paranothing"));
            drawText("Welcome to Paranothing", titleFont, Color.WhiteSmoke, title.TopTextRectangle.X, title.TopTextRectangle.Y);
            spriteBatch.DrawString(gameFont, "Press 'Enter' to start", startPosition, Color.DarkMagenta);
        }

        //Description
        private void drawDescriptionText()
        {
            spriteBatch.DrawString(gameFont, "Press 'Space Bar' to continue", startPosition, Color.Black);
        }

        # endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            boyTex = Content.Load<Texture2D>("Sprites/sprite");
            boySheet = new SpriteSheet(boyTex);
            boySheet.splitSheet(5, 9);
            boySheet.addAnimation("standright", new int[] { 8 });
            boySheet.addAnimation("walkright", new int[] { 0, 1, 2, 3, 4, 5, 6, 7 });
            boySheet.addAnimation("standleft", new int[] { 17 });
            boySheet.addAnimation("walkleft", new int[] { 9, 10, 11, 12, 13, 14, 15, 16 });

            player = new Boy(254f, 100f, boySheet);

            floorTex = Content.Load<Texture2D>("Sprites/floor");
            f1 = new Floor(0, 68, 100, 8, floorTex);
            f2 = new Floor(100, 164, 400, 8, floorTex);

            stair = Content.Load<Texture2D>("Sprites/stairs_intact");
            stairSheet = new SpriteSheet(stair);
            stairs = new Stairs(100f, 68f, stairSheet);

            control = new GameController(player);
            control.addObject(f1);
            control.addObject(f2);
            control.addObject(stairs);
            
            // TODO: use this.Content to load your game content here
            loadTitleContents();
            description = new GameBackground(Content.Load<Texture2D>("GameThumbnail"), new Rectangle(0, 0, Width, Height));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            graphics.PreferredBackBufferWidth = (int)(Width * scale);
            graphics.PreferredBackBufferHeight = (int)(Height * scale);
            graphics.ApplyChanges();

            // TODO: Add your update logic here
            switch (gameState)
            {
                case GameLevel.Title:
                    title.Update(this, Keyboard.GetState());
                    break;
                case GameLevel.Description:
                    description.Update(this, Keyboard.GetState());
                    break;
                case GameLevel.Level:
                    control.updateObjs(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case GameLevel.Title:
                    spriteBatch.Begin();
                    title.Draw(spriteBatch);
                    drawTitleText();
                    spriteBatch.End();
                    break;
                case GameLevel.Description:
                    spriteBatch.Begin();
                    description.Draw(spriteBatch);
                    drawDescriptionText();
                    spriteBatch.End();
                    break;
                case GameLevel.Level:
                    spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(scale));
                    control.drawObjs(spriteBatch);
                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }
    }
}