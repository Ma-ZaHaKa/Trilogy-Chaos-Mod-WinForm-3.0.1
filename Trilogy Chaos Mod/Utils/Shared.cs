namespace GTAChaos.Utils
{
  public static class Shared
  {
    public static string Version = "3.0.1";
    public static string SelectedGame = "san_andreas";
    public static bool TimerEnabled;
    public static bool IsStreamMode;
    public static Shared.VOTING_MODE StreamVotingMode = Shared.VOTING_MODE.COOLDOWN;
    public static Sync Sync;

    public enum VOTING_MODE
    {
      COOLDOWN,
      VOTING,
      RAPID_FIRE,
      ERROR,
    }
  }
}
