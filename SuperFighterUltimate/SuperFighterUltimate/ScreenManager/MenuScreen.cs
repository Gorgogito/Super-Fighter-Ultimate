#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
#endregion

namespace SuperFighterUltimate.GameStateManagement
{

  /// <summary>
  /// Base class for screens that contain a menu of options. The user can
  /// move up and down to select an entry, or cancel to back out of the screen.
  /// </summary>
  abstract class MenuScreen : GameScreen
  {

    #region Fields

    const int menuEntryPadding = 35;
    List<MenuEntry> menuEntries = new List<MenuEntry>();
    int selectedEntry = 0;
    string menuTitle;
    bool isMouseDown = false;
    Rectangle bounds;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of menu entries, so derived classes can add
    /// or change the menu contents.
    /// </summary>
    protected IList<MenuEntry> MenuEntries
    {
      get { return menuEntries; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Constructor.
    /// </summary>
    public MenuScreen(string menuTitle)
    {

      this.menuTitle = menuTitle;

      TransitionOnTime = TimeSpan.FromSeconds(0.5);
      TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    #endregion

    #region Handle Input

    /// <summary>
    /// Allows the screen to create the hit bounds for a particular menu entry.
    /// </summary>
    protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
    {
      return new Rectangle(
        0,
        (int)entry.Destination.Y - menuEntryPadding,
        ScreenManager.GraphicsDevice.Viewport.Width,
        entry.GetHeight(this) + (menuEntryPadding * 2));
    }

    /// <summary>
    /// Responds to user input, changing the selected entry and accepting
    /// or cancelling the menu.
    /// </summary>
    public override void HandleInput(InputState input)
    {
      PlayerIndex player;
      if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
      { OnCancel(player); }

      if (input.IsMenuUp(ControllingPlayer))
      {
        selectedEntry--;
        if (selectedEntry < 0)
        { selectedEntry = menuEntries.Count - 1; }
      }
      else if (input.IsMenuDown(ControllingPlayer))
      {
        selectedEntry++;
        if (selectedEntry >= menuEntries.Count)
        { selectedEntry = 0; }
      }
      else if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) || input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player))
      { OnSelectEntry(selectedEntry, player); }

      MouseState state = Mouse.GetState();
      if (state.LeftButton == ButtonState.Released)
      {
        if (isMouseDown)
        {
          isMouseDown = false;
          Point clickLocation = new Point(state.X, state.Y);
          for (int i = 0; i < menuEntries.Count; i++)
          {
            MenuEntry menuEntry = menuEntries[i];
            if (menuEntry.Destination.Contains(clickLocation))
            { OnSelectEntry(i, PlayerIndex.One); }
          }
        }
      }
      else if (state.LeftButton == ButtonState.Pressed)
      {
        isMouseDown = true;
        Point clickLocation = new Point(state.X, state.Y);
        for (int i = 0; i < menuEntries.Count; i++)
        {
          MenuEntry menuEntry = menuEntries[i];
          if (menuEntry.Destination.Contains(clickLocation))
          { selectedEntry = i; }
        }
      }

    }

    /// <summary>
    /// Handler for when the user has chosen a menu entry.
    /// </summary>
    protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
    { menuEntries[entryIndex].OnSelectEntry(playerIndex); }

    /// <summary>
    /// Handler for when the user has cancelled the menu.
    /// </summary>
    protected virtual void OnCancel(PlayerIndex playerIndex)
    { ExitScreen(); }

    /// <summary>
    /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
    /// </summary>
    protected void OnCancel(object sender, PlayerIndexEventArgs e)
    { OnCancel(e.PlayerIndex); }

    #endregion

    #region Loading

    public override void LoadContent()
    {
      bounds = ScreenManager.SafeArea;
      base.LoadContent();
    }

    #endregion

    #region Update and Draw

    /// <summary>
    /// Allows the screen the chance to position the menu entries. By default
    /// all menu entries are lined up in a vertical list, centered on the screen.
    /// </summary>
    protected virtual void UpdateMenuEntryLocations()
    {
      float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
      Vector2 position = new Vector2(
        0f,
        ScreenManager.Game.Window.ClientBounds.Height / 2 -
        (menuEntries[0].GetHeight(this) + (menuEntryPadding * 2) * menuEntries.Count)
        );

      for (int i = 0; i < menuEntries.Count; i++)
      {
        MenuEntry menuEntry = menuEntries[i];
        position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;
        if (ScreenState == ScreenState.TransitionOn)
        { position.X -= transitionOffset * 256; }
        else
        { position.X += transitionOffset * 512; }
        position.Y += menuEntry.GetHeight(this) + (menuEntryPadding * 2);
      }
    }

    /// <summary>
    /// Updates the menu.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      for (int i = 0; i < menuEntries.Count; i++)
      {
        bool isSelected = IsActive && (i == selectedEntry);
        UpdateMenuEntryDestination();
        menuEntries[i].Update(this, isSelected, gameTime);
      }
    }

    /// <summary>
    /// Draws the menu.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice graphics = ScreenManager.GraphicsDevice;
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      SpriteFont font = ScreenManager.Font;
      spriteBatch.Begin();

      for (int i = 0; i < menuEntries.Count; i++)
      {
        MenuEntry menuEntry = menuEntries[i];
        bool isSelected = IsActive && (i == selectedEntry);
        menuEntry.Draw(this, isSelected, gameTime);
      }

      // Make the menu slide into place during transitions, using a
      // power curve to make things look more interesting (this makes
      // the movement slow down as it nears the end).
      float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

      // Draw the menu title centered on the screen
      Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 375);
      Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
      Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
      float titleScale = 1.25f;

      titlePosition.Y -= transitionOffset * 100;

      spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                             titleOrigin, titleScale, SpriteEffects.None, 0);

      spriteBatch.End();
    }

    #endregion

    #region Public functions

    public void UpdateMenuEntryDestination()
    {
      Rectangle bounds = ScreenManager.SafeArea;
      Rectangle textureSize = ScreenManager.ButtonBackground.Bounds;
      int xStep = bounds.Width / (menuEntries.Count + 2);
      int maxWidth = 0;
      for (int i = 0; i < menuEntries.Count; i++)
      {
        int width = menuEntries[i].GetWidth(this);
        if (width > maxWidth)
        { maxWidth = width; }
      }
      maxWidth += 20;
      for (int i = 0; i < menuEntries.Count; i++)
      {
        menuEntries[i].Destination =
            new Rectangle((bounds.Left + (xStep - textureSize.Width) / 2 + (i + 1) * xStep), bounds.Bottom - textureSize.Height * 2, maxWidth + 25, 50);
      }
    }

    #endregion

  }

}
