using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GTAChaos.Effects
{
    public class WeightedRandomBag<T>
    {
        private readonly List<WeightedRandomBag<T>.Entry> entries = new List<WeightedRandomBag<T>.Entry>();
        private readonly Random rand = RandomHandler.Random;
        private double AccumulatedWeight;

        public WeightedRandomBag<T>.Entry Add(T item, string word, double weight = 1.0)
        {
            WeightedRandomBag<T>.Entry entry = new WeightedRandomBag<T>.Entry()
            {
                item = item,
                weight = weight,
                word = word
            };
            this.entries.Add(entry);
            this.CalculateAccumulatedWeight();
            return entry;
        }

        public T GetRandom(Random rand)
        {
            if (rand == null)
                rand = this.rand;
            double num = rand.NextDouble() * this.AccumulatedWeight;
            foreach (WeightedRandomBag<T>.Entry entry in this.entries)
            {
                if (entry.weight >= num)
                    return entry.item;
                num -= entry.weight;
            }
            return this.Count <= 0 ? default(T) : this.entries[0].item;
        }

        public (bool success, T entry) GetRandom(
          Random rand,
          Func<WeightedRandomBag<T>.Entry, bool> predicate, bool is_youtube = false, string youtube_name = "")
        {
            if (rand == null)
                rand = this.rand;
            IEnumerable<WeightedRandomBag<T>.Entry> source = this.entries.Where<WeightedRandomBag<T>.Entry>(predicate);
            if (source.Count<WeightedRandomBag<T>.Entry>() <= 0)
                return (false, default(T));
            if (is_youtube)
            {
                foreach (WeightedRandomBag<T>.Entry entry in source)
                {
                    if (entry.word.ToLower() == youtube_name.ToLower())
                        return (true, entry.item);  //bool succ add
                }
                return (false, default(T));
            }
            double num1 = 0.0;
            foreach (WeightedRandomBag<T>.Entry entry in source)
                num1 += entry.weight;
            double num2 = rand.NextDouble() * num1;
            foreach (WeightedRandomBag<T>.Entry entry in source)
            {
                if (entry.weight >= num2)
                    return (true, entry.item);
                num2 -= entry.weight;
            }
            return (false, default(T));
        }

        private void CalculateAccumulatedWeight()
        {
            this.AccumulatedWeight = 0.0;
            foreach (WeightedRandomBag<T>.Entry entry in this.entries)
                this.AccumulatedWeight += entry.weight;
        }

        public int Count => this.Get().Count;

        public List<WeightedRandomBag<T>.Entry> Get() => this.entries;

        public WeightedRandomBag<T>.Entry Add(WeightedRandomBag<T>.Entry entry) => this.Add(entry.item, entry.word, entry.weight);

        public void Remove(WeightedRandomBag<T>.Entry item) => this.entries.Remove(item);

        public bool Contains(WeightedRandomBag<T>.Entry item) => this.entries.Contains(item);

        public void Sort(Comparison<WeightedRandomBag<T>.Entry> comparison) => this.entries.Sort(comparison);

        public WeightedRandomBag<T>.Entry Find(
          Predicate<WeightedRandomBag<T>.Entry> match)
        {
            return this.entries.Find(match);
        }

        public void Clear() => this.entries.Clear();

        public struct Entry
        {
            public double weight;
            public T item;
            public string word;
        }
    }
}
