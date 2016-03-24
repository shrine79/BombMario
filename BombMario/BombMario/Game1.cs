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
using RC_Framework;


namespace BombMario
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Input stuff
        KeyboardState kbState;
        KeyboardState kbStatePre;
        /// <summary>
        /// memebers are initialized to default value(false) by default constructor of c# struct
        /// </summary>
        public struct keyLock
        {
            public bool l_lock;
            public bool r_lock;
            public bool space_lock;
        }
        keyLock key_lock;

        //Show Bounding box
        bool showBB = false;

        // Background stuff
        ScrollBackGround bg;
        Rectangle bgSrc = new Rectangle(0, 0, 826, 357);
        Rectangle bgDes = new Rectangle(0, 0, 800, 600);
        const int bgDir = 2;
        const float bgSpeed = -3;

        
        //Plane
        Sprite3 plane;
        Rectangle planeDest;
        Texture2D planeTex;

        //player
        player infantry;
        Texture2D infantryTexTalk;
        Texture2D infantryTexRun;
        Vector2 infantryOffset;

        //Foreground stuff
        Rectangle fgSrc = new Rectangle(256, 48, 84, 23);
        Sprite3 foreground;
        






        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
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
            planeDest = new Rectangle(400, 300, 600, 200);


            infantryOffset = new Vector2(0, -10);


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
            // initialize LineBatch
            LineBatch.init(GraphicsDevice);

            /////////////////////////////////
            // Background
            /////////////////////////////////
            bg = new ScrollBackGround(Content.Load<Texture2D>("desert1"), bgSrc, bgDes, bgSpeed , bgDir);
            

            /////////////////////////////////
            // Plane sprite
            /////////////////////////////////
            planeTex = Content.Load<Texture2D>("plane");
            Util.ChangeColourInTexturePNG(planeTex, new Color(255, 224, 87, 255), new Color(0, 0, 0, 0));
            plane = new Sprite3(true, planeTex, planeDest.X, planeDest.Y);
            //plane.setBBFractionOfTex(0.3f, 50, 50);
            plane.setBB(40, 60, planeTex.Width - 100, planeTex.Height/3);
            plane.setHSoffset(new Vector2(plane.getBB().X + plane.getBB().Width/2, plane.getBB().Y + plane.getBB().Height / 2));

            /////////////////////////////////
            // Infantry
            /////////////////////////////////
            infantryTexRun = Content.Load<Texture2D>("infantry_running");
            Util.ChangeColourInTexturePNG(infantryTexRun, new Color(255, 255, 255, 255), new Color(0, 0, 0, 0));
            infantryTexTalk = Content.Load<Texture2D>("infantry_talking");
            Util.ChangeColourInTexturePNG(infantryTexTalk, new Color(255, 255, 255, 255), new Color(0, 0, 0, 0));
            infantry = new player(true, infantryTexTalk, plane.getPos().X + infantryOffset.X,
                                                         plane.getPos().Y + infantryOffset.Y - plane.getBB().Height / 2,
                                                         FaceDir.left,
                                                         new Vector2(11,1));



            //add more animation textures, currently reuse run-animation for jumping
            infantry.setTextures(infantryTexRun, Status.running, new Vector2(12, 1));
            infantry.setTextures(infantryTexRun, Status.jumping, new Vector2(12, 1));

            //Fore ground
            foreground = new Sprite3(true, planeTex, planeDest.X + 22, planeDest.Y - 33);
            foreground.setImageSource(fgSrc);
            foreground.setWidthHeight(fgSrc.Width, fgSrc.Height);
            

            // TODO: use this.Content to load your game content here
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
            kbStatePre = kbState;
            kbState = Keyboard.GetState();
            if (kbState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            bg.Update(gameTime);

            /*            // Jump control
                        if (infantry.getSt() == Status.jumping)
                        {
                            var bot = infantry.getBoundingBoxAA().Bottom;
                            var top = plane.getBoundingBoxAA().Top;
                            if (bot >= top + 3)
                            {
                                infantry.setSt(Status.talking);
                                infantry.setDeltaSpeed(new Vector2(0, 0));
                                infantry.setPosY(plane.getPos().Y + infantryOffset.Y - plane.getBB().Height / 2);
                            }
                            else
                            {
                                infantry.setDeltaSpeed(new Vector2(infantry.getDeltaSpeed().X, infantry.getDeltaSpeed().Y + 0.2f));
                            }
                            if (kbState.IsKeyDown(Keys.Left) && infantry.getPosX() > plane.getBoundingBoxAA().Left)
                                infantry.setPosX(infantry.getPosX() - 1);
                            else if (kbState.IsKeyDown(Keys.Right) && infantry.getPosX() < plane.getBoundingBoxAA().Right)
                                infantry.setPosX(infantry.getPosX() + 1);

                            infantry.moveByDeltaXY();
                        }
                        else // walk control
                        {
                            // space pressed
                            if (kbState.IsKeyDown(Keys.Space) && kbStatePre.IsKeyUp(Keys.Space))
                            {
                                IsJumping = true;
                                infantry.setDeltaSpeed(new Vector2(0, -5));
                                infantry.moveByDeltaXY();
                            }
                        }

                        // Left pressed and released
                        if (key_lock.l_lock == false && kbState.IsKeyDown(Keys.Left))
                            {
                                var x = infantry.getPosX();
                                if (x >= plane.getBoundingBoxAA().Left) infantry.setPosX(infantry.getPosX() - 1);
                                // Is left key locked by other keys?
                                if (kbStatePre.IsKeyUp(Keys.Left))
                                {
                                    infantry.setTexture(infantryTexRun, true);
                                    infantry.setFlip(SpriteEffects.None);
                                    infantry.setAnimation(12, 1);
                                    key_lock.r_lock = true; // lock the right key
                                }
                            }
                            else if (key_lock.l_lock == false && kbState.IsKeyUp(Keys.Left) && kbStatePre.IsKeyDown(Keys.Left))
                            {
                                infantry.setTexture(infantryTexTalk, true);
                                infantry.setFlip(SpriteEffects.None);
                                infantry.setAnimation(11, 1);
                                key_lock.r_lock = false; // free right key
                            }

                            // right pressed and released
                            if (key_lock.r_lock == false && kbState.IsKeyDown(Keys.Right))
                            {
                                var x = infantry.getPosX();
                                if (x <= plane.getBoundingBoxAA().Right) infantry.setPosX(x + 1);
                                // is Right key locked?
                                if (kbStatePre.IsKeyUp(Keys.Right))
                                {
                                    infantry.setTexture(infantryTexRun, true);
                                    infantry.setFlip(SpriteEffects.FlipHorizontally);
                                    infantry.setAnimation(12, 1);
                                    key_lock.l_lock = true; // lock the left key
                                }
                            }
                            else if (key_lock.r_lock == false && kbState.IsKeyUp(Keys.Right) && kbStatePre.IsKeyDown(Keys.Right))
                            {
                                infantry.setTexture(infantryTexTalk, true);
                                infantry.setFlip(SpriteEffects.FlipHorizontally);
                                infantry.setAnimation(11, 1);
                                key_lock.l_lock = false; // free the left key
                            }
            */

            if (kbState.IsKeyDown(Keys.Space) && kbStatePre.IsKeyUp(Keys.Space))
            {
                // space pressed, jump
                 infantry.setSt(Status.jumping);
            }

            if (kbState.IsKeyDown(Keys.Left))
            {
                // Left is pressed
                if (key_lock.l_lock == false)
                {
                    infantry.setDir(FaceDir.left);
                    if (infantry.getSt() != Status.jumping) infantry.setSt(Status.running);
                    infantry.speed = 1.5f;
                    key_lock.r_lock = true;
                }
            }
            else
            {
                key_lock.r_lock = false;
            }

            if (kbState.IsKeyDown(Keys.Right))
            {
                // right is pressed
                if (key_lock.r_lock == false)
                {
                    infantry.setDir(FaceDir.right);
                    if(infantry.getSt() != Status.jumping) infantry.setSt(Status.running);
                    infantry.speed = 1.5f;
                    key_lock.l_lock = true;
                }
            }
            else
            {
                key_lock.l_lock = false;
            }


            if (kbState.IsKeyUp(Keys.Left) &&
               kbState.IsKeyUp(Keys.Right))
            {
                infantry.speed = 0f;
                if(infantry.getSt() != Status.jumping)
                {
                    // if no key pressed and not jumping, then talk.
                    infantry.setSt(Status.talking);
                }
            }
               
           

            // Update player
            infantry.Update(gameTime);

            // Doing boundary check for player

            if ((kbState.IsKeyDown(Keys.Left) || (infantry.getSt() == Status.jumping)) 
                && infantry.getPosX() < plane.getBoundingBoxAA().Left)
            {
                infantry.setDeltaSpeed(new Vector2(0, infantry.getDeltaSpeed().Y));
            }
            else if ((kbState.IsKeyDown(Keys.Right) || (infantry.getSt() == Status.jumping)) 
                     && infantry.getPosX() > plane.getBoundingBoxAA().Right)
            {
                infantry.setDeltaSpeed(new Vector2(0, infantry.getDeltaSpeed().Y));
            }
            // Move player
            infantry.moveByDeltaXY();
            // trigger animation
            infantry.animationTick();
            // Enable debug bounding box drawing
            if (kbState.IsKeyDown(Keys.B) && kbStatePre.IsKeyUp(Keys.B))
                showBB = !showBB;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //Draw background
            bg.Draw(spriteBatch);

            //Draw player and play ground
            plane.Draw(spriteBatch);
            infantry.Draw(spriteBatch);

            //Draw fore ground
            foreground.Draw(spriteBatch);




            //Draw Bounding Box on top
            if (showBB)
            {
                plane.drawInfo(spriteBatch, Color.Red, Color.Red);
                infantry.drawInfo(spriteBatch, Color.White, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
