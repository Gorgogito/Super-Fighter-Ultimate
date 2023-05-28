using System;

namespace SuperFighterUltimate
{
#if WINDOWS || XBOX
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      using (SuperFighterUltimateGame game = new SuperFighterUltimateGame())
      { game.Run(); }
    }
  }
#endif
}

