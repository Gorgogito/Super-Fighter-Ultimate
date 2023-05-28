#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using SuperFighterUltimate.GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace SuperFighterUltimate.Screens
{
  class SelectCharScreen : GameScreen
  {

    #region Fields and Properties

    string theme;

    //static Vector2[] playerCardOffset = new Vector2[] 
    //    { 
    //        new Vector2(100f * BlackjackGame.WidthScale, 190f * BlackjackGame.HeightScale),
    //        new Vector2(336f * BlackjackGame.WidthScale, 210f * BlackjackGame.HeightScale),
    //        new Vector2(570f * BlackjackGame.WidthScale, 190f * BlackjackGame.HeightScale) 
    //    };

    #endregion

    #region Initiaizations

    public SelectCharScreen(string theme)
    {
      TransitionOnTime = TimeSpan.FromSeconds(0.0);
      TransitionOffTime = TimeSpan.FromSeconds(0.5);
      this.theme = theme;
    }

    #endregion

    #region Loading

    /// <summary>
    /// Load content and initializes the actual game.
    /// </summary>
    public override void LoadContent()
    { 
     

      base.LoadContent();
    }

    /// <summary>
    /// Unload content loaded by the screen.
    /// </summary>
    public override void UnloadContent()
    {
      base.UnloadContent();
    }
    #endregion


  }
}
