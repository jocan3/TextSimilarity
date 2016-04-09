using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TextMachineLearning
{
    class Word2Vec
    {
        public static int[] Transform(string word, bool treshold) {

            word = word.ToLower();
            char[] wordVec = word.ToCharArray();
            int[] vectorResult = {
                                    0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,
                                    0,0,0
                                 };
            char[] alphabet = { 
                                  'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 
                                  'i', 'j', 'k', 'l', 'm', 'n', 'ñ', 'o', 
                                  'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 
                                  'x', 'y', 'z'
                              };

            for (int i = 0; i < wordVec.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++) {
                    if (alphabet[j].Equals(wordVec[i]))
                    {
                        if (treshold)
                        {
                            vectorResult[j] = vectorResult[j] + 1;
                        }
                        else {
                            vectorResult[j] = 1;
                        }
                    }
                }
            }
            return vectorResult;
        }
    }
}

//comment
