using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test 
{
    class GameTimer : GameComponent
    {
    private SpriteFont font;
    private String text;
    private float time;
    private bool started;
    private bool paused;
    private bool finished;
    private Vector2 position;

    public GameTimer(Game game, float startTime)
        : base(game)
    {
        time = startTime;
        started = false;
        paused = false;
        finished = false;
        Text = "";
    }

    #region Properties
    public SpriteFont Font
    {
        get { return font; }
        set { font = value; }
    }
    public string Text
    {
        get { return text; }
        set { text = value; }
    }
    public bool Started
    {
        get { return started; }
        set { started = value; }
    }
    public bool Paused
    {
        get { return paused; }
        set { paused = value; }
    }
    public bool Finished
    {
        get { return finished; }
        set { finished = value; }
    }
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    public float Time
    {
        get { return time; }
    }
    #endregion

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (started)
        {
            if (!paused)
            {
                if (time > 0)
                {
                    time -= deltaTime;
                }
                else
                    finished = true;
            }
        }
        Text = ((int)time).ToString();
        base.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, Text, Position, Color.AntiqueWhite);
    }
}
}

