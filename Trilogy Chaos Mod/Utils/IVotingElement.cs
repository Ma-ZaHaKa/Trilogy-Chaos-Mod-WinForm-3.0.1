using GTAChaos.Effects;

namespace GTAChaos.Utils
{
  public interface IVotingElement
  {
    int GetId();

    AbstractEffect GetEffect();

    int GetVotes();

    int GetPercentage();
  }
}
