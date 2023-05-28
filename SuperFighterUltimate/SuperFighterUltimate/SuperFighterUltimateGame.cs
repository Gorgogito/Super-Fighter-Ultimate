#region Using Statements
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
using SuperFighterUltimate.GameStateManagement;
using SuperFighterUltimate.Screens;
#endregion

namespace SuperFighterUltimate
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class SuperFighterUltimateGame : Microsoft.Xna.Framework.Game
  {

    GraphicsDeviceManager graphics;
    ScreenManager screenManager;

    public static float HeightScale = 1.0f;
    public static float WidthScale = 1.0f;

    public SuperFighterUltimateGame()
    {
      this.Window.AllowUserResizing = true;
      graphics = new GraphicsDeviceManager(this);

      this.IsMouseVisible = true;
      Content.RootDirectory = "Content";

      screenManager = new ScreenManager(this);
      screenManager.AddScreen(new BackgroundScreen(), null);
      screenManager.AddScreen(new MainMenuScreen(), null);

      Components.Add(screenManager);

      // Initialize sound system
      //AudioManager.Initialize(this);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      base.Initialize();

      this.graphics.PreferredBackBufferWidth = 1280;
      this.graphics.PreferredBackBufferHeight = 720;

      Window.Title = "Super Fighter Ultimate X";
      graphics.ApplyChanges();
      Rectangle bounds = graphics.GraphicsDevice.Viewport.TitleSafeArea;
      HeightScale = bounds.Height / 480f;
      WidthScale = bounds.Width / 800f;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    { base.LoadContent(); }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    { }

  }
}
