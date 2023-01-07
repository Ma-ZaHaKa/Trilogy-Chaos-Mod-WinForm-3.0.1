using System;

namespace GTAChaos.Utils
{
  public class RandomHandler
  {
    public static Random Random = new Random();

    public static void SetSeed(string seed) => RandomHandler.Random = string.IsNullOrEmpty(seed) ? new Random() : new Random(seed.GetHashCode());

    public static int Next() => RandomHandler.Random.Next();

    public static int Next(int maxValue) => RandomHandler.Random.Next(maxValue);

    public static int Next(int minValue, int maxValue) => RandomHandler.Random.Next(minValue, maxValue + 1);

    public static double NextDouble() => RandomHandler.Random.NextDouble();

    public static void NextBytes(byte[] buffer) => RandomHandler.Random.NextBytes(buffer);
  }
}
