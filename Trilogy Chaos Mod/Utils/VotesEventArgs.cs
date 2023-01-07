using System;

namespace GTAChaos.Utils
{
  public class VotesEventArgs : EventArgs
  {
    public string[] Effects;
    public int[] Votes;
    public int LastChoice;
  }
}
