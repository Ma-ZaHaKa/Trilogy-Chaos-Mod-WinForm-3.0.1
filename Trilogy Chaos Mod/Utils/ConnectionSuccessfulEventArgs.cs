using System;

namespace GTAChaos.Utils
{
  public class ConnectionSuccessfulEventArgs : EventArgs
  {
    public bool IsHost;
    public string HostUsername;
  }
}
