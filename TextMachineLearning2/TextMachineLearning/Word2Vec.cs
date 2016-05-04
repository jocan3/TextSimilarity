
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;

namespace TextMachineLearning
{
    class Word2Vec
    {
        public int n { get; set; }
        public HashSet<string> words { get; set; }
        public HashSet<string> ngrams { get; set; }
        public Regex StopWords { get; set; }

        public string[] WordsVector { get; set; }
        public string[] NGramsVector { get; set; }



        public Word2Vec(int n) {
            this.n = n;
            words = new HashSet<string>();
            ngrams = new HashSet<string>();
            StopWords = new Regex(@"^(a|about|above|after|again|against|all|am|an|and|any|are|aren't|as|at|be|because|been|before|being|below|between|both|but|by|can't|cannot|could|couldn't|did|didn't|do|does|doesn't|doing|don't|down|during|each|few|for|from|further|had|hadn't|has|hasn't|have|haven't|having|he|he'd|he'll|he's|her|here|here's|hers|herself|him|himself|his|how|how's|i|i'd|i'll|i'm|i've|if|in|into|is|isn't|it|it's|its|itself|let's|me|more|most|mustn't|my|myself|no|nor|not|of|off|on|once|only|or|other|ought|our|ours|ourselves|out|over|own|same|shan't|she|she'd|she'll|she's|should|shouldn't|so|some|such|than|that|that's|the|their|theirs|them|themselves|then|there|there's|these|they|they'd|they'll|they're|they've|this|those|through|to|too|under|until|up|very|was|wasn't|we|we'd|we'll|we're|we've|were|weren't|what|what's|when|when's|where|where's|which|while|who|who's|whom|why|why's|with|won't|would|wouldn't|you|you'd|you'll|you're|you've|your|yours|yourself|yourselves|\d)$");
        }



        public void addSentence(string sentence) {
            sentence = RemoveSpecialCharacters(sentence);
            var sentenceWithoutStopWords = String.Join(" ", sentence.Split(' ').Where(x => !StopWords.IsMatch(x.ToLower().Trim())));
            string[] result = sentenceWithoutStopWords.Trim().Split(' ');
           // string sentenceWithoutStopWords = "";
            
            foreach (var r in result) {
                if (r.Length > 1)
                {
                    words.Add(r);
                }
                //sentenceWithoutStopWords += r;
            }
            createNGrams(sentenceWithoutStopWords.Replace(" ",""));
        }

        public void createNGrams(string sentence) {
            int lenght = sentence.Length;

            for (int i = 0; i < lenght; ++i) {
                if (i + n < lenght) {
                    ngrams.Add(sentence.Substring(i, n).ToUpper());
                }
            }

        }


        public void ComputeVectors() {
            WordsVector = words.ToArray();
            NGramsVector = ngrams.ToArray();
        }

        public string RemoveSpecialCharacters(string str) {
            return str.Replace("*", " ").Replace(",", " ").Replace("(", " ").Replace(")", " ").Replace("-", " ").Replace("+", " ").Replace("!", " ").Replace("?", " ").Replace(@"\", " ").Replace("/", " ").Replace("[", " ").Replace("]", " ").Replace(".", " ").Replace("|", " ").Replace("^", " ");
        }

        public double[] transform(string sentence, string type) {
            sentence = RemoveSpecialCharacters(sentence);//.Replace("*", "").Replace(",", "").Replace("(", "").Replace(")", "");
            sentence = String.Join(" ", sentence.Split(' ').Where(x => !StopWords.IsMatch(x.ToLower().Trim())));
            
            if (type == "3-grams"){
                return GetNGramsVector(sentence);
            }else if (type == "words"){
                return GetWordsVector(sentence);
            }else{
                return GetLettersVector(sentence);
            }
        }

        public string transformInverse(double [] sentence, string type)
        {
            string result = "";
            if (type == "3-grams")
            {
                for (int i =0; i < sentence.Length; ++i) {
                    result += (sentence[i] > 0) ? NGramsVector[i]+" " : "";
                }

            }
            else if (type == "words")
            {
                for (int i = 0; i < sentence.Length; ++i)
                {
                    result += (sentence[i] > 0) ? WordsVector[i]+" " : "";
                }
            }
            else
            {
                for (int i = 0; i < sentence.Length; ++i)
                {
                    result += (sentence[i] > 0) ? (char)(i + 65) + "" : "";
                }
            }

            return result;
        }

        public double [] GetWordsVector(string sentence){
            double[] result = new double[WordsVector.Length];
            for (int i = 0; i < result.Length; ++i) {
                result[i] = Regex.Matches(sentence, @"\b" + WordsVector[i] + @"\b").Count;
                
            }

            return result;
        }

        public double[] GetLettersVector(string word)
        {
            word = word.Replace(" ", "");

            word = word.ToLower();
            char[] wordVec = word.ToCharArray();
            double[] vectorResult = {
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
                for (int j = 0; j < alphabet.Length; j++)
                {
                    if (alphabet[j].Equals(wordVec[i]))
                    {
                            vectorResult[j] = vectorResult[j] + 1;

                    }
                }
            }
            return vectorResult;
        }



        public double[] GetNGramsVector(string sentence)
        {
            sentence = sentence.Replace(" ", "");
            double[] result = new double[NGramsVector.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = Regex.Matches(sentence, NGramsVector[i]).Count;
            }

            return result;
        }


        public static double[] Transform(string word, bool treshold)
        {

            word = word.ToLower();
            char[] wordVec = word.ToCharArray();
            double[] vectorResult = {
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
