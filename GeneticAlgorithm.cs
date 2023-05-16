using System;
namespace GeneticAlgorithm
{

    public class GeneticAlgorithm
    {
        // fields

        private List<PairOfLetters> candidateSolutions;
        private HashSet<string> wordHashSet;
        private Dictionary<char, double> letterFrequency;
        private string encText;
        int size;
        public int fitnessFunctionCalls;
        
        // fields to prevent early coverage and to know when to stop.
        public PairOfLetters bestSolution;
        private int generationWithOutImprovement;
        private int currentSize;
        private int maxSize;
        public bool shouldRun;

        //methods



        public GeneticAlgorithm(int startingPopulationSize,int maxSize,string Encpath,string dictPath,string lfreqPath)
        {
            candidateSolutions = new List<PairOfLetters>();
            wordHashSet = new HashSet<string>();
            letterFrequency = new Dictionary<char, double>();
            LoadEncryptedText(Encpath);
            CreateWordHashSet(dictPath);
            CreateLetterFrequencyDictionary(lfreqPath);
            size = startingPopulationSize;
            this.maxSize = maxSize;
            shouldRun = true;
            CreateCandidates(size);
            currentSize = size;
            FitnessFunction();
            bestSolution = candidateSolutions.OrderByDescending( x => x.FitnessValue).First();


        }
        /*****************************************************************************
         * Name:CreateCandidates
         * Description:this method create a starting possible solutions,
         * i used this method on the constructor since this step is happaned only once.
         ******************************************************************************/
        private void CreateCandidates(int size)
        {


            List<char> alphaBet = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            for (int i = 0; i < size; i++)
            {
                PairOfLetters cn = new PairOfLetters();
                List<char> originMap = new List<char>(alphaBet);
                List<char> randomMap = new List<char>(alphaBet); // this list goind to be permutation.

                // creating the permutation, change each index place with other random index place.
                Random r = new Random();
                for (int j = 25; j >= 0; j--)
                {
                    int replaceIndex = r.Next(j + 1);
                    HelperMethods.switchPlace(randomMap, replaceIndex, j);

                }
                for (int x = 0; x < 26; x++)
                {
                    cn.Pairs.Add(randomMap[x], originMap[x]);
                }
                candidateSolutions.Add(cn);
            }


        }
        /********************************************************************
         * Name:FitnessFunction
         * Description:for each candidate find the fitness score.
         * i am using two methods to find the score
         * 1) word matching - the amount of word in the decrypted text
         * that appear in the dictionary of words.
         * 2) letter frequent - see how close the letters fruquent
         * int the decryptet text to the original.
         *
         *******************************************************************/
        public void FitnessFunction()
        {
            foreach(var candidate in candidateSolutions)
            {
                FitnessFunction(candidate);
            }
            

        }

        private void FitnessFunction(PairOfLetters candidate)
        {
            string dt = HelperMethods.DecryptText(candidate, encText); // first decrypt the text.
            double wordMatchScore = WordMatchingFitness(dt);
            double letterFreqScore = letterFrequencyFitness(dt);
            int w1 = 2;
            int w2 = 1;
            double finalScore = w1 * wordMatchScore + w2 * letterFreqScore;
            candidate.FitnessValue = finalScore;
            ++fitnessFunctionCalls;
        }

        /*******************************************************
         * Name:CreateWordHashSet
         * Description:for easy use of the fitness function.
         * this method take the words in "dict.txt" and insert them
         * to hash set.
         ******************************************************/
        private void CreateWordHashSet(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string word;
                    while ((word = sr.ReadLine()) != null)
                    {
                        wordHashSet.Add(word.ToLower());
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("There a problem with reading the dict.txt file"); }

        }
        /*********************************************************************
         * Name:CreateLetterFrequencyDictionary
         * Description:for easy use of the fitness function.
         * this method take the latter frequency from the "Letter_Freq.txt"
         * file and put them in Dictionary(hash table) 
         *********************************************************************/
        private void CreateLetterFrequencyDictionary(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] spliitedLine = line.Split('\t');
                        char letter = char.Parse(spliitedLine[1].ToLower());
                        double frequency = double.Parse(spliitedLine[0]);
                        letterFrequency.Add(letter, double.Parse(spliitedLine[0]));
                    }
                }
            }
            catch (Exception e) { Console.WriteLine("There a problem reading Letter_Freq.txt file."); }

        }

        /**********************************************************
         * Name:LoadEncryptedText
         * Description:read the encrypted file.
         **********************************************************/
        private void LoadEncryptedText(string path)
        {
            if (File.Exists(path)) encText = File.ReadAllText(path).ToLower();
            else
            {
                throw new FileNotFoundException($"file dont exist {path}");
            }
        }

        private double WordMatchingFitness(string decText)
        {
            char[] splittedChars = new char[] { ' ', '\t', '\r', '\n', ',', '.', ';' };
            string[] words = decText.Split(splittedChars, StringSplitOptions.RemoveEmptyEntries);
            int matchingWords = 0;
            foreach(var word in words)
            {
                if (wordHashSet.Contains(word)) matchingWords++;
            }

            return (double)matchingWords / words.Length;

        }

        /***************************************************************
         * Name:letterFrequencyFitness
         * Description:check the fitness by the frequency of letters.
         ***************************************************************/
        private double letterFrequencyFitness(string decText)
        {
            Dictionary<char, double> decFreq = new Dictionary<char, double>();
            for (char c = 'a'; c <= 'z'; c++) decFreq.Add(c, 0); // initialize all the freq to 0;
            foreach (char c in decText) 
            {
                if (char.IsLetter(c)) decFreq[c]++;
            }
            double total = decText.Count(char.IsLetter);
            // normalize the values.
            foreach(var k in decFreq.Keys)
            {
                decFreq[k] /= total;
            }

            // calcualte the diffrence
            double returnScore = 0;
            foreach(var c in decFreq.Keys)
            {
                double candidateFrequent = decFreq[c];
                double generalFrequent = letterFrequency[c];
                returnScore += Math.Abs(candidateFrequent - generalFrequent);
            }
            return (1 - returnScore);
        }

        /**********************************************************
        * Name:CandidateSelection
        * Description:in this we find parent to the next generations.
        * this method first choose randomly k candidate and then
        * choose the best one from them.
         **********************************************************/
        public PairOfLetters CandidateSelection(int kIndividuals)
        {
            List<PairOfLetters> candidateToNextGeneration = new List<PairOfLetters>();
            Random r = new Random();
            int genSize = candidateSolutions.Count;
            for (int i = 0; i < kIndividuals; i++) candidateToNextGeneration.Add(candidateSolutions[r.Next(genSize)]);
            PairOfLetters parent = candidateToNextGeneration[0];
            foreach (PairOfLetters c in candidateToNextGeneration) parent = c.FitnessValue > parent.FitnessValue ? c : parent;
            return parent;
        }

        /**********************************************************
        * Name:CrossOver
        * Description:given two parent this method return their son.
        * i selected random division of the genes. so the first
        *  k genes came from parent1 and the rest from parent2
        *  at the end i dealt with cases of duplicates genes which 
        *  can give us non valid solution
         **********************************************************/
        public PairOfLetters CrossOver(PairOfLetters parent1,PairOfLetters parent2)
        {
            Random r = new Random();
            int crossPoint = r.Next(26);
            PairOfLetters son = new PairOfLetters();
            int i = 0;
            char c = 'a';
            // add from parent1
            for(; i < crossPoint; i++)
            {
                son.Pairs.Add(c, parent1.Pairs[c]);
                ++c;
            }
            List<char> problemaTicKeys = new List<char>();
            // add from paren
            for(; i < 26; i++)
            {
                if (!son.Pairs.ContainsValue(parent2.Pairs[c])) son.Pairs.Add(c, parent2.Pairs[c]);
                else
                {
                    son.Pairs.Add(c, '|'); // deal with that case later.
                    problemaTicKeys.Add(c);
                }

                ++c;
            }

            // fix the '|' case
            HelperMethods.fillValues(son, problemaTicKeys);
            return son;
        }
        /***************************************************************
         * Name:Mutate
         * Description:change random genes. this function swaps just 
         * 
         ***************************************************************/
        public void Mutate(PairOfLetters son,double mutationPossibility)
        {
            Random r = new Random();
            if(r.NextDouble() < mutationPossibility)
            {
                char letterKey1 = (char)('a' + r.Next(26));
                char letterKey2 = (char)('a' + r.Next(26));
                bool sameLetterkeys = letterKey1 == letterKey2 ? true : false; ;
                while (sameLetterkeys)
                {
                    letterKey1 = (char)('a' + r.Next(26));
                    sameLetterkeys = letterKey1 == letterKey2 ? true : false;
                }

                char temp = son.Pairs[letterKey1];
                son.Pairs[letterKey1] = son.Pairs[letterKey2];
                son.Pairs[letterKey2] = temp;

            }
        }

        public void EvolveNextGeneration(int newGenerationSize,double mutateRate,int maxGenerationWithOutImprovment, string optimizationType)
        {
            
            // the new generation
            List<PairOfLetters> newGeneration = new List<PairOfLetters>();
            for (int i = 0; i < newGenerationSize; i++)
            {
                // select the two parents
                PairOfLetters parent1 = CandidateSelection(newGenerationSize);
                PairOfLetters parent2 = CandidateSelection(newGenerationSize);
                
                // crossover
                PairOfLetters son = CrossOver(parent1, parent2);
                
                // mutate
                Mutate(son,mutateRate);
                newGeneration.Add(son);
            }
            newGeneration.Add(bestSolution); // keep the best solution. as we dont want lose him.
            candidateSolutions = newGeneration;
            FitnessFunction();
            if(optimizationType == "Lamarckian") LamarckianOptimization(2);
            else if(optimizationType == "Darwinian") DarwinOptimization(2);
            CheckIfImprove();
            // check if should stop
            if (CheckIfShouldStop(maxGenerationWithOutImprovment)) return;
            // to prevent early cover, if we didnt improve over some time then increase the next generation size.
            if (generationWithOutImprovement >= maxGenerationWithOutImprovment / 2)
            {
                currentSize = Math.Min(currentSize * 2, maxSize);
            } else if (currentSize >= maxSize) currentSize = size;
            
            



        }

        /***********************************************************************
         * Name:CheckIfImprove
         * Description: check if we there improvment, if so update the best
         * solution, otherwise update that we didnt improve.
         **********************************************************************/
        private void CheckIfImprove()
        {
            PairOfLetters currentBestSolution = candidateSolutions.OrderByDescending(x => x.FitnessValue).First();
            if (currentBestSolution.FitnessValue > bestSolution.FitnessValue)
            {
                bestSolution = currentBestSolution;
                generationWithOutImprovement = 0;
            }
            else generationWithOutImprovement++;
        }
        /***********************************************************************
         * Name:CheckIfShouldStop
         * Description: check if we should stop, if so save the text file
         * using SaveBestSolution and let know that the algorithm should stop
         **********************************************************************/
        private bool CheckIfShouldStop(int maxGenerationwithOutImprove)
        {
            if (generationWithOutImprovement >= maxGenerationwithOutImprove)
            {
                shouldRun = false;
                SaveBestSolution();
                return true;
            }

            return false;
        }
        /***************************************************************
         * Name:SaveBestSolution
         * Description: when we finished the algorithm, save the text
         * files with best solution.
         ***************************************************************/
        private void SaveBestSolution()
        {
            HelperMethods.CreatePlainTextFile(encText,bestSolution);
            HelperMethods.createPermutationFile(bestSolution);
        }

        /***********************************************************************
         * Name:LamarckianOptimization
         * Description: doing Lamarkian optmization.
         **********************************************************************/
        public void LamarckianOptimization(int n)
        {
            for(int c = 0; c < candidateSolutions.Count; c++)
            {
                
                for (int i = 0; i < n; i++)
                {
                    PairOfLetters originalCandidate = candidateSolutions[c].Clone();
                    bool t = candidateSolutions[c] == bestSolution ? true : false;
                    double originalFitness = candidateSolutions[c].FitnessValue;
                    SwapPairs(candidateSolutions[c]);
                    if (candidateSolutions[c].FitnessValue <= originalFitness)
                    {
                        candidateSolutions[c] = originalCandidate;
                        if(t) bestSolution = candidateSolutions[c];
                    }

                }
            }
        }

        /***********************************************************************
         * Name:DarwinOptimization
         * Description: doing Darwin optmization.
         **********************************************************************/
        public void DarwinOptimization(int n)
        {
            //
            for (int c = 0; c < candidateSolutions.Count; c++)
            {
                PairOfLetters improvedCandidate = candidateSolutions[c].Clone();
                for (int i = 0; i < n; i++)
                {
                    SwapPairs(improvedCandidate);
                }

                if (improvedCandidate.FitnessValue > candidateSolutions[c].FitnessValue)
                    candidateSolutions[c].FitnessValue = improvedCandidate.FitnessValue;
            }
        }

        /***********************************************************************
          * Name:SwapPairs
          * Description: this method is for swap between map of plain letter to
         * cipher letter.
          **********************************************************************/
        private void SwapPairs(PairOfLetters candidate)
        {
            Random r = new Random();
            char key1 = (char)('a' + r.Next(26));
            char key2 = (char)('a' + r.Next(26));
            while (key1 == key2)
            {
                key2 = (char)('a' + r.Next(26));
            }

            char temp = candidate.Pairs[key1];
            candidate.Pairs[key1] = candidate.Pairs[key2];
            candidate.Pairs[key2] = temp;

        }
        
        
        
        

       
    }






}