#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;
#endregion

namespace SuperFighterUltimate.GameStateManagement
{

  /// <summary>
  /// The screen manager is a component which manages one or more GameScreen
  /// instances. It maintains a stack of screens, calls their Update and Draw
  /// methods at the appropriate times, and automatically routes input to the
  /// topmost active screen.
  /// </summary>
  public class ScreenManager : DrawableGameComponent
  {

    #region Fields

    List<GameScreen> screens = new List<GameScreen>();
    List<GameScreen> screensToUpdate = new List<GameScreen>();

    public InputState input = new InputState();

    SpriteBatch spriteBatch;
    SpriteFont font;
    Texture2D blankTexture;
    Texture2D buttonBackground;
    Texture2D selectBorder;

    bool isInitialized;

    bool traceEnabled;

    #endregion

    #region Properties

    /// <summary>
    /// A default SpriteBatch shared by all the screens. This saves
    /// each screen having to bother creating their own local instance.
    /// </summary>
    public SpriteBatch SpriteBatch
    {
      get { return spriteBatch; }
    }

    public Texture2D ButtonBackground
    {
      get { return buttonBackground; }
    }

    public Texture2D BlankTexture
    {
      get { return blankTexture; }
    }

    public Texture2D SelectBorder
    {
      get { return selectBorder; }
    }

    /// <summary>
    /// A default font shared by all the screens. This saves
    /// each screen having to bother loading their own local copy.
    /// </summary>
    public SpriteFont Font
    {
      get { return font; }
    }

    /// <summary>
    /// If true, the manager prints out a list of all the screens
    /// each time it is updated. This can be useful for making sure
    /// everything is being added and removed at the right times.
    /// </summary>
    public bool TraceEnabled
    {
      get { return traceEnabled; }
      set { traceEnabled = value; }
    }

    /// <summary>
    /// Returns the portion of the screen where drawing is safely allowed.
    /// </summary>
    public Rectangle SafeArea
    {
      get
      { return Game.GraphicsDevice.Viewport.TitleSafeArea; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Constructs a new screen manager component.
    /// </summary>
    public ScreenManager(SuperFighterUltimateGame game)
      : base(game)
    {
      // we must set EnabledGestures before we can query for them, but
      // we don't assume the game wants to read them.
    }

    /// <summary>
    /// Initializes the screen manager component.
    /// </summary>
    public override void Initialize()
    {
      base.Initialize();
      isInitialized = true;
    }

    /// <summary>
    /// Load your graphics content.
    /// </summary>
    protected override void LoadContent()
    {
      ContentManager content = Game.Content;

      spriteBatch = new SpriteBatch(GraphicsDevice);
      font = content.Load<SpriteFont>("Fonts/MenuFont");
      blankTexture = content.Load<Texture2D>("Images/blank");
      buttonBackground = content.Load<Texture2D>("Images/ButtonRegular");
      selectBorder = content.Load<Texture2D>("Images/SelectBorder");

      foreach (GameScreen screen in screens)
      { screen.LoadContent(); }
    }

    /// <summary>
    /// Unload your graphics content.
    /// </summary>
    protected override void UnloadContent()
    {
      // Tell each of the screens to unload their content.
      foreach (GameScreen screen in screens)
      { screen.UnloadContent(); }
    }

    #endregion

    #region Update and Draw

    /// <summary>
    /// Allows each screen to run logic.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
      input.Update();

      screensToUpdate.Clear();

      foreach (GameScreen screen in screens)
        screensToUpdate.Add(screen);

      bool otherScreenHasFocus = !Game.IsActive;
      bool coveredByOtherScreen = false;

      while (screensToUpdate.Count > 0)
      {
        GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

        screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

        screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        if (screen.ScreenState == ScreenState.TransitionOn ||
            screen.ScreenState == ScreenState.Active)
        {
          if (!otherScreenHasFocus)
          {
            screen.HandleInput(input);
            otherScreenHasFocus = true;
          }

          if (!screen.IsPopup)
            coveredByOtherScreen = true;
        }
      }

      if (traceEnabled)
        TraceScreens();
    }

    /// <summary>
    /// Prints a list of all the screens, for debugging.
    /// </summary>
    void TraceScreens()
    {
      List<string> screenNames = new List<string>();

      foreach (GameScreen screen in screens)
        screenNames.Add(screen.GetType().Name);

      Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
    }

    /// <summary>
    /// Tells each screen to draw itself.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
      foreach (GameScreen screen in screens)
      {
        if (screen.ScreenState == ScreenState.Hidden)
          continue;

        screen.Draw(gameTime);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new screen to the screen manager.
    /// </summary>
    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
      screen.ControllingPlayer = controllingPlayer;
      screen.ScreenManager = this;
      screen.IsExiting = false;

      if (isInitialized)
      { screen.LoadContent(); }

      screens.Add(screen);
    }

    /// <summary>
    /// Removes a screen from the screen manager. You should normally
    /// use GameScreen.ExitScreen instead of calling this directly, so
    /// the screen can gradually transition off rather than just being
    /// instantly removed.
    /// </summary>
    public void RemoveScreen(GameScreen screen)
    {
      if (isInitialized)
      { screen.UnloadContent(); }

      screens.Remove(screen);
      screensToUpdate.Remove(screen);
    }

    /// <summary>
    /// Expose an array holding all the screens. We return a copy rather
    /// than the real master list, because screens should only ever be added
    /// or removed using the AddScreen and RemoveScreen methods.
    /// </summary>
    public GameScreen[] GetScreens()
    { return screens.ToArray(); }

    /// <summary>
    /// Helper draws a translucent black fullscreen sprite, used for fading
    /// screens in and out, and for darkening the background behind popups.
    /// </summary>
    public void FadeBackBufferToBlack(float alpha)
    {
      Viewport viewport = GraphicsDevice.Viewport;
      spriteBatch.Begin();
      spriteBatch.Draw(blankTexture,
                       new Rectangle(0, 0, viewport.Width, viewport.Height),
                       Color.Black * alpha);
      spriteBatch.End();
    }

    /// <summary>
    /// Informs the screen manager to serialize its state to disk.
    /// </summary>
    public void SerializeState()
    {
      using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storage.DirectoryExists("ScreenManager"))
        { DeleteState(storage); }
        else
        { storage.CreateDirectory("ScreenManager"); }

        using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
        {
          using (BinaryWriter writer = new BinaryWriter(stream))
          {
            foreach (GameScreen screen in screens)
            {
              if (screen.IsSerializable)
              { writer.Write(screen.GetType().AssemblyQualifiedName); }
            }
          }
        }

        int screenIndex = 0;
        foreach (GameScreen screen in screens)
        {
          if (screen.IsSerializable)
          {
            string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);
            using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
            { screen.Serialize(stream); }
            screenIndex++;
          }
        }
      }
    }

    public bool DeserializeState()
    {
      using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storage.DirectoryExists("ScreenManager"))
        {
          try
          {
            if (storage.FileExists("ScreenManager\\ScreenList.dat"))
            {
              using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
              {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                  while (reader.BaseStream.Position < reader.BaseStream.Length)
                  {
                    string line = reader.ReadString();
                    if (!string.IsNullOrEmpty(line))
                    {
                      Type screenType = Type.GetType(line);
                      GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                      AddScreen(screen, PlayerIndex.One);
                    }
                  }
                }
              }
            }

            for (int i = 0; i < screens.Count; i++)
            {
              string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
              using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
              { screens[i].Deserialize(stream); }
            }

            return true;
          }
          catch (Exception)
          { DeleteState(storage); }
        }
      }

      return false;
    }

    /// <summary>
    /// Deletes the saved state files from isolated storage.
    /// </summary>
    private void DeleteState(IsolatedStorageFile storage)
    {
      string[] files = storage.GetFileNames("ScreenManager\\*");
      foreach (string file in files)
      { storage.DeleteFile(Path.Combine("ScreenManager", file)); }
    }

    #endregion

  }

}
