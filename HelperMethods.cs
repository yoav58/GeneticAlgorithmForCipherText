using System;
using System.Text;

namespace GeneticAlgorithm
{
    public class HelperMethods
    {
        public HelperMethods()
        {
        }

        public static void switchPlace(List<char> l,int i,int j)
        {
            char temp = l[i];
            l[i] = l[j];
            l[j] = temp;
        }

        /**********************************************************
         * Name:DecryptText
         * Description:given map of letters, decrypt the encoded text.
         **********************************************************/
        public static string DecryptText(PairOfLetters candidte,string encText)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in encText)
            {
                if (candidte.Pairs.ContainsKey(c))
                {
                    sb.Append(candidte.Pairs[c]);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        /**********************************************************
         * Name:fillValues
         * Description:since we can have duplicate values,
         * this method fix this. 
         **********************************************************/
        public static void fillValues(PairOfLetters son,List<char> pk)
        {
            char[] letters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            HashSet<char> hs = new HashSet<char>(letters);
            // check which letters not appear which means we can use them to fill the '|'
            for(int i = 0; i < 26; i++)
            {
                char c = son.Pairs.Values.ElementAt(i);
                if (hs.Contains(c)) hs.Remove(c);
            }

            int e = 0;
            // fill the '|' with the letter that not appear
            foreach(var c in pk)
            {
                char newChar = hs.ElementAt(e);
                son.Pairs[c] = newChar;
                hs.Remove(newChar);
            }
        }
        /**********************************************************
         * Name:CreatePlainTextFile
         * Description:this method create the "plain.txt" file.
         **********************************************************/
        public static void CreatePlainTextFile(string encText, PairOfLetters bestSolution)
        {
            string decText = HelperMethods.DecryptText(bestSolution,encText);
            string filePath = Path.Combine(AppContext.BaseDirectory,"OutPutFiles","plain.txt");
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(decText);
            }
        }
        /**********************************************************
         * Name:createPermutationFile
         * Description:create the "perm.txt" file.
         **********************************************************/
        public static void createPermutationFile(PairOfLetters bestSolution)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory,"OutPutFiles","perm.txt");
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (var pair in  bestSolution.Pairs.OrderBy(x => x.Key))
                {
                    sw.WriteLine($"{pair.Key} {pair.Value}");
                }
            }
        }


    }
}