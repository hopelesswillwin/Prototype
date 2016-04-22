using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;

        Model shots;
        Vector3 shotsPosition = new Vector3 (0,-40,0);

        Vector3 modelPosition = new Vector3(0,-33,0);
        float modelRotation = 0.0f;
        Model myModel;
        Vector3 modelVelocity = Vector3.Zero;
        float aspectRatio;
        bool iscolli = false;
        int amount = 20;

        Model[] spheres = new Model[20];
        float[] velocity = new float[20];
        float[] x_position = new float[20];
        float[] y_position = new float[20];
        float[] z_position = new float[20];
        float[] size = new float[20];
        Vector3[] position = new Vector3[20];

        Vector3 camPosition;
        Vector3 camtarget;
        Matrix projectionmatrix;
        Matrix viewmatrix;
        Matrix world;

        SpriteBatch dead;
        GameTimer timer;
        Vector2 vectFont;
        bool over; //whether game over or not


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            camtarget = new Vector3(0.0f,0.0f,0.0f);
            camPosition = new Vector3(0.0f, -50.0f, 10.0f); //0,-50,10
            projectionmatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                GraphicsDevice.DisplayMode.AspectRatio, 1.0f, 1000f);
            viewmatrix = Matrix.CreateLookAt(camPosition, camtarget, Vector3.Up);
            world = Matrix.CreateWorld(camtarget, Vector3.Forward, Vector3.Up);

            over = false;
            

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
            this.background = this.Content.Load<Texture2D>("bg");

            dead = new SpriteBatch(GraphicsDevice);
            timer = new GameTimer(this, 10.0f);
            timer.Font = Content.Load<SpriteFont>("font");
            timer.Position = new Vector2(this.Window.ClientBounds.Width / 2 - timer.Font.MeasureString(timer.Text).X / 2, 0);
            Components.Add(timer);
            vectFont = new Vector2(this.Window.ClientBounds.Width / 3, this.Window.ClientBounds.Height / 3);


            myModel = Content.Load<Model>("Ship");
            shots = Content.Load<Model>("UntexturedSphere");
          

            Random rnd = new Random();

            for (int i = 0; i < amount; i++)
            {
                int v = rnd.Next(1, 4);
                velocity[i] = v * 0.1f;
                x_position[i] = rnd.Next(-20, 20);
                y_position[i] = rnd.Next(12, 20);

                switch (v)
                {
                    case 1:
                        size[i] = 2;
                        break;
                    case 2:
                        size[i] = 3;
                        break;
                    case 3:
                        size[i] = 4;
                        break;
                    default:
                        size[i] = 1;
                        break;
                }

                switch (v)
                {
                    case 1:
                        spheres[i] = Content.Load<Model>("Untexturedsphere1");
                        break;
                    case 2:
                        spheres[i] = Content.Load<Model>("Untexturedsphere2");
                        break;
                    case 3:
                        spheres[i] = Content.Load<Model>("Untexturedsphere3");
                        break;
                    default:
                        spheres[i] = Content.Load<Model>("Untexturedsphere");
                        break;
                }

                //spheres[i] = Content.Load<Model>("Untexturedsphere");

                position[i] = new Vector3(x_position[i], y_position[i], 0);

            }

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            // Get some input.
            

            for (int i = 0; i < amount; i++)
            {
                if (IsCollision(myModel, Matrix.CreateTranslation(modelPosition),
                    spheres[i], Matrix.CreateTranslation(position[i])))
                {
                    iscolli = true;
                    over = true;
                    for (int j = 0; j < amount; j++)
                    {
                        velocity[j] = 0;
                     
                    }
                }
            }


            if(iscolli == false)
            {
                UpdateInput();
                if (over)
                {
                    for (int j = 0; j < amount; j++)
                    {
                        velocity[j] = 0;
                       
                    }
                }

            }

            // Add velocity to the current position.
            modelPosition += modelVelocity;

            for(int i = 0; i < amount; i++)
            {
                position[i].Y -= velocity[i];
            }

            //timer stops
            if (timer.Time <= 0.0f)
            {
                timer.Finished = true;
                over = true;
            }
                
            timer.Update(gameTime);

            // Bleed off velocity over time.
            modelVelocity *= 0.95f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Draw(GameTime gameTime)
        {
                

            if (iscolli || over)
            {
                if(iscolli)
                {
                    graphics.GraphicsDevice.Clear(Color.DarkRed);
                    modelPosition.Z -= 0.1f;
                    for (int r=0; r<=5; r++)
                    {
                        modelRotation += 0.05f;
                    }
                    dead.Begin();
                    dead.DrawString(timer.Font, "Game over! You are dead!", vectFont, Color.White);
                    dead.End();

                }
                else
                {
                    graphics.GraphicsDevice.Clear(Color.Green);
                                      
                    dead.Begin();
                    dead.DrawString(timer.Font, "Congratulation! You won!", vectFont, Color.White);
                    dead.End();
                }
  
  
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                this.spriteBatch.Begin();
                timer.Draw(spriteBatch);
                this.spriteBatch.Draw(background, new Rectangle(-10, 30, background.Width - 1100, background.Height - 500), Color.White);
                this.spriteBatch.End();
                base.Draw(gameTime);

            }

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(camPosition,
                        camtarget, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1, 400);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            // Copy any parent transforms.

            for (int i = 0; i < amount; i++)
            {
                Matrix[] transforms2 = new Matrix[spheres[i].Bones.Count];
                spheres[i].CopyAbsoluteBoneTransformsTo(transforms2);
                
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in spheres[i].Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms2[mesh.ParentBone.Index] *
                            Matrix.CreateRotationY(0)
                            * Matrix.CreateTranslation(new Vector3(x_position[i],position[i].Y,0));
                        effect.View = Matrix.CreateLookAt(camPosition,
                            camtarget, Vector3.Up);
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(45.0f), aspectRatio,
                            1, 400);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            //draw shots
            Matrix[] transforms3 = new Matrix[shots.Bones.Count];
            shots.CopyAbsoluteBoneTransformsTo(transforms3);

            foreach (ModelMesh mesh in shots.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms3[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(0)
                        * Matrix.CreateTranslation(shotsPosition);
                    effect.View = Matrix.CreateLookAt(camPosition,
                        camtarget, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1, 400);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }



            base.Draw(gameTime);
        }

        protected void UpdateInput()
        {
            // Get the game pad state.
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space))
            {
                //shotsPosition = modelPosition;
                shotsPosition.X = modelPosition.X;
                shotsPosition.Y += 1;
                

            }

            // If they hit esc, exit
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            //timer startet
            timer.Started = true;


            // Move our sprite based on arrow keys being pressed:
            modelRotation = 0f;
            
                if (state.IsKeyDown(Keys.Right))
                {
                    if (modelPosition.X < 12)
                    {
                       modelPosition.X += 0.3f;
                    }
                    modelRotation = 0.5f;
                }
                if (state.IsKeyDown(Keys.Left))
                {
                    if (modelPosition.X > -12)
                    {
                    modelPosition.X -= 0.3f;
                    }
                    modelRotation = -0.5f;
                }
             
                if (state.IsKeyDown(Keys.Up))
                    modelPosition.Y += 0.3f;

                if (state.IsKeyDown(Keys.Down))
                    if(modelPosition.Y > -35)
                    modelPosition.Y -= 0.3f;

                if (state.IsKeyDown(Keys.P))
                    timer.Paused = true;
                if (state.IsKeyDown(Keys.Q))
                    timer.Paused = false;
        }
        
  
private bool IsCollision(Model model1, Matrix world1, Model model2, Matrix world2)
        {   if (iscolli || over) return false;
            else { 
            for (int meshIndex1 = 0; meshIndex1 < model1.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = model1.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(world1);

                for (int meshIndex2 = 0; meshIndex2 < model2.Meshes.Count; meshIndex2++)
                {
                    BoundingSphere sphere2 = model2.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(world2);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            }
            return false;
        }
    }
}


