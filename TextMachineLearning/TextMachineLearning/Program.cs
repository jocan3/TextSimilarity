﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.MachineLearning;
using Accord.Math;
using Microsoft.VisualBasic.FileIO;


namespace TextMachineLearning
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            
        }


        private static void meh() {
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;
            tfp = new TextFieldParser("");
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;

            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();

            }

            tfp.Close();

            // The k-Nearest Neighbors algorithm can be used with
            // any kind of data. In this example, we will see how
            // it can be used to compare, for example, Strings.

            string[] inputs = 
{
    "Car",    // class 0
    "Bar",    // class 0
    "Jar",    // class 0

    "Charm",  // class 1
    "Chair"   // class 1
};

            int[] outputs =
{
    0, 0, 0,  // First three are from class 0
    1, 1,     // And next two are from class 1
};


            // Now we will create the K-Nearest Neighbors algorithm. For this
            // example, we will be choosing k = 1. This means that, for a given
            // instance, only its nearest neighbor will be used to cast a new
            // decision. 


            
            // In order to compare strings, we will be using Levenshtein's string distance
            KNearestNeighbors<string> knn = new KNearestNeighbors<string>(k: 1, classes: 2,
                inputs: inputs, outputs: outputs, distance: Distance.Levenshtein);


            // After the algorithm has been created, we can use it:
            int answer = knn.Compute("Chars"); // answer should be 1.

        
        }
    }
}
