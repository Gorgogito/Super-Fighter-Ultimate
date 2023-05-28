#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using SuperFighterUltimate.GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SuperFighterUltimate.Screens
{

  class MainMenuScreen : MenuScreen
  {
    public static string Theme = "Red";

    #region Initializations

    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public MainMenuScreen()
      : base("")
    { }

    #endregion

    public override void LoadContent()
    {
      MenuEntry startGameMenuEntry = new MenuEntry("Jugar", 1);
      MenuEntry optionGameMenuEntry = new MenuEntry("Opciones", 2);
      MenuEntry exitMenuEntry = new MenuEntry("Salir", 3);

      startGameMenuEntry.Selected += StartGameMenuEntrySelected;
      optionGameMenuEntry.Selected += OptionGameMenuEntrySelected;
      exitMenuEntry.Selected += OnCancel;

      MenuEntries.Add(startGameMenuEntry);
      MenuEntries.Add(optionGameMenuEntry);
      MenuEntries.Add(exitMenuEntry);

      base.LoadContent();
    }

    #region Update

    /// <summary>
    /// Respond to "Play" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void StartGameMenuEntrySelected(object sender, EventArgs e)
    {
      foreach (GameScreen screen in ScreenManager.GetScreens())
      { screen.ExitScreen(); }
      ScreenManager.AddScreen(new SelectCharScreen(Theme), null);
    }

    /// <summary>
    /// Respond to "Theme" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OptionGameMenuEntrySelected(object sender, EventArgs e)
    {
      //ScreenManager.AddScreen(new OptionsMenu(), null);
    }

    /// <summary>
    /// Respond to "Exit" Item Selection
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void OnCancel(PlayerIndex playerIndex)
    { ScreenManager.Game.Exit(); }

    #endregion
  }
}
