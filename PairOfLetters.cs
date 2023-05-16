using System;
namespace GeneticAlgorithm
{
    public class PairOfLetters
    {
        private Dictionary<char, char> pairs; // a possible solution.
        public Dictionary<char, char> Pairs
        {
            get { return pairs;}
            set { pairs = value;}
        }

        private double fitnessValue;
        public double FitnessValue
        {
            get { return fitnessValue; }
            set { fitnessValue = value; }
        }
        /***********************************************************************
          * Name:Clone
          * Description: this method is to copy the object by value;
          **********************************************************************/
        public PairOfLetters Clone()
        {
            PairOfLetters copy = new PairOfLetters();
            foreach (var pair in this.Pairs)
            {
                copy.Pairs.Add(pair.Key, pair.Value);
            }
            copy.FitnessValue = this.FitnessValue;
            return copy;       
        }

        public PairOfLetters()
        {
            pairs = new Dictionary<char, char>();
        }
    }
}