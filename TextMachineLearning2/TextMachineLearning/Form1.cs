using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Analysis;

namespace TextMachineLearning
{
    public partial class Form1 : Form
    {
        private string SelectedTrainingFile = "";
        private string SelectedTestFile = "";
        private int NumberOfColumns = 0;
        private int SelectedClass = 0;
        private KNearestNeighbors knn;
        private KNearestNeighbors<string> knnStr;
        Dictionary<string, int> classlist;
        Dictionary<int, string> inverseClassList;

        private Word2Vec wv; 

        public Form1()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            DialogResult result = fbd.ShowDialog();
            textBox1.Text = fbd.FileName;

            try
            {
                string[] delimiters = { "," };
                string[] fields;
                TextFieldParser tfp;
                tfp = new TextFieldParser(textBox1.Text);
                tfp.HasFieldsEnclosedInQuotes = true;
                tfp.Delimiters = delimiters;
                fields = tfp.ReadFields();

                if (fields.Length > 1)
                {
                    if (SelectedTestFile != "" && fields.Length == NumberOfColumns)
                    {

                        SelectedTrainingFile = textBox1.Text;
                        SelectedClass = 0;
                        label2.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox1.DataSource = fields;
                        label3.Enabled = true;
                        comboBox2.Enabled = true;
                        label4.Enabled = true;
                        textBox2.Enabled = true;
                        label6.Enabled = true;
                        comboBox4.Enabled = true;

                    }
                    else
                    {

                        SelectedTrainingFile = textBox1.Text;
                        NumberOfColumns = fields.Length;
                        label2.Enabled = false;
                        comboBox1.Enabled = false;
                        label3.Enabled = false;
                        comboBox2.Enabled = false;
                        label4.Enabled = false;
                        textBox2.Enabled = false;
                        label6.Enabled = false;
                        comboBox4.Enabled = false;
                    }

                    tfp.Close();
                    ValidateFields();
                }
                
            }
            catch (Exception ex) {
                label2.Enabled = false;
                comboBox1.Enabled = false;
                label3.Enabled = false;
                comboBox2.Enabled = false;
                label4.Enabled = false;
                textBox2.Enabled = false;
                label6.Enabled = false;
                comboBox4.Enabled = false;
            }

        }

        
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            DialogResult result = fbd.ShowDialog();
            textBox8.Text = fbd.FileName;

            try
            {
                string[] delimiters = { "," };
                string[] fields;
                TextFieldParser tfp;
                tfp = new TextFieldParser(textBox8.Text);
                tfp.HasFieldsEnclosedInQuotes = true;
                tfp.Delimiters = delimiters;
                fields = tfp.ReadFields();

                if (fields.Length > 1)
                {
                    if (SelectedTrainingFile != "" && fields.Length == NumberOfColumns)
                    {

                        SelectedTestFile = textBox8.Text;
                        SelectedClass = 0;
                        label2.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox1.DataSource = fields;
                        label3.Enabled = true;
                        comboBox2.Enabled = true;
                        label4.Enabled = true;
                        textBox2.Enabled = true;
                        label6.Enabled = true;
                        comboBox4.Enabled = true;

                    }
                    else
                    {
                        SelectedTestFile = textBox8.Text;
                        NumberOfColumns = fields.Length;
                        label2.Enabled = false;
                        comboBox1.Enabled = false;
                        label3.Enabled = false;
                        comboBox2.Enabled = false;
                        label4.Enabled = false;
                        textBox2.Enabled = false;
                        label6.Enabled = false;
                        comboBox4.Enabled = false;
                    }


                    tfp.Close();
                    ValidateFields();
                }
            }
            catch (Exception ex)
            {
                label2.Enabled = false;
                comboBox1.Enabled = false;
                label3.Enabled = false;
                comboBox2.Enabled = false;
                label4.Enabled = false;
                textBox2.Enabled = false;
                label6.Enabled = false;
                comboBox4.Enabled = false;
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateFields();
        }

        private void ValidateFields() {
            int temp;
            if (Int32.TryParse(textBox2.Text, out temp))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedClass = comboBox1.SelectedIndex;
        }

        private string GetString(string[] fields, string separator) { 
            string result = "";
            for (int i = 0; i < fields.Length; ++i) {
                if (i != SelectedClass) {
                    result += fields[i].ToUpper() + separator;
                }
            }
            return result;
        }

        private bool NotNullFields(string[] fields)
        {
            bool result = true;
            foreach (var f in fields)
            {
                result = result & (f.Trim().Length > 0);
            }
            return result;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            wv = new Word2Vec(3);

            textBox3.Text = "Processing";

            System.Diagnostics.Debug.WriteLine("Starting Processing");
            
            //Get list of classes, input and output vectors
            classlist = new Dictionary<string, int>();
            inverseClassList = new Dictionary<int, string>();
            List<List<double>> IntMatrixInputs = new List<List<double>>();
            List<string> stringInputs = new List<string>();

            List<int> IntOutputs = new List<int>();

            bool frequency = true;
 
            int temp = 0;
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;
            tfp = new TextFieldParser(textBox1.Text);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;
            fields = tfp.ReadFields();
            int indexClass = 0;
            int indexRows = 0;
            while (!tfp.EndOfData) {
                System.Diagnostics.Debug.WriteLine("Processing training row: " + indexRows);
                fields = tfp.ReadFields();
                if (NotNullFields(fields))
                {
                    if (!classlist.TryGetValue(fields[SelectedClass], out temp))
                    {
                        classlist[fields[SelectedClass]] = indexClass;
                        inverseClassList[indexClass] = fields[SelectedClass];
                        ++indexClass;
                    }
                    /*
                    stringInputs.Add(GetString(fields,""));
                    IntMatrixInputs.Add(Word2Vec.Transform(GetString(fields,""), frequency).ToList()); //getLettersVector.toList();
                    */

                    stringInputs.Add(GetString(fields, " "));
                    wv.addSentence(GetString(fields," "));
                    IntOutputs.Add(classlist[fields[SelectedClass]]);

                    ++indexRows;
                }
            }

            wv.ComputeVectors();
            
            List<string> strInputsTrain = new List<string>(); 
            List<string> strInputsTest = new List<string>();

            double[][] IntInputsTrain = new double[indexRows][];
   
            int[] outputsTrain = new int[indexRows];
   
            double [][] IntInputs = new double[IntMatrixInputs.Count][];
            for (int i =0; i < indexRows; ++i){
                 //IntInputs[i] = IntMatrixInputs[i].ToArray();
                System.Diagnostics.Debug.WriteLine("Creating input row: " + i);
               // IntInputsTrain[i] = wv.transform(stringInputs[i], comboBox4.SelectedItem.ToString()); //IntMatrixInputs[i].ToArray();
                     strInputsTrain.Add(stringInputs[i]);
                     outputsTrain[i] = IntOutputs[i];
                
            }

            if (comboBox2.SelectedItem.ToString() == "Levenshtein")
            {
                knnStr = new KNearestNeighbors<string>(k: Int32.Parse(textBox2.Text), classes: classlist.Count,
    inputs: strInputsTrain.ToArray(), outputs: outputsTrain, distance: Distance.Levenshtein);    
            }
            else if (comboBox2.SelectedItem.ToString() == "Euclidean")
            {
                knn = new KNearestNeighbors(k: Int32.Parse(textBox2.Text), classes: classlist.Count,
                inputs: IntInputsTrain, outputs: outputsTrain, distance: Distance.Euclidean);
            }
            else if (comboBox2.SelectedItem.ToString() == "Jaccard")
            {
                knn = new KNearestNeighbors(k: Int32.Parse(textBox2.Text), classes: classlist.Count,
                inputs: IntInputsTrain, outputs: outputsTrain, distance: Jaccard);
            }
            else
            {
                knn = new KNearestNeighbors(k: Int32.Parse(textBox2.Text), classes: classlist.Count,
                inputs: IntInputsTrain, outputs: outputsTrain, distance: Distance.Cosine);
            }


            int correctCount = 0;
            int wrongCount = 0;

            List<int> expected = new List<int>();
            List<int> predicted = new List<int>();

            int positiveValue = 1;
            int negativeValue = 0;

            tfp = new TextFieldParser(textBox8.Text);
            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;
            fields = tfp.ReadFields();
            indexRows = 0;

            string outputPath = textBox8.Text.Replace(".csv", "_Labeled.csv");
            StreamWriter sw = new StreamWriter(outputPath);
            sw.WriteLine("FirstName,LastName,Country");

            while (!tfp.EndOfData)
            {
                System.Diagnostics.Debug.WriteLine("Processing test row: " + indexRows);
               // Console.WriteLine("Processing row: " + indexRows);

                fields = tfp.ReadFields();
                if (fields[1] != "" && fields[2] != "")
                {
                    int answer;
                    if (comboBox2.SelectedItem.ToString() == "Levenshtein")
                    {
                        answer = knnStr.Compute(GetString(fields, " "));
                    }
                    else
                    {
                        answer = knn.Compute(wv.transform(GetString(fields, " "), comboBox4.SelectedItem.ToString()));
                    }

                    int tempClass=-1;

                    classlist.TryGetValue(fields[SelectedClass], out tempClass);
                    expected.Add(tempClass);
                    predicted.Add(answer);

                    if (answer == tempClass)
                    {
                        correctCount++;
                    }
                    else
                    {
                        wrongCount++;
                    }

                    ++indexRows;


                    sw.WriteLine(fields[1] + "," + fields[2] + "," + inverseClassList[answer]);
                }
                else {
                    sw.WriteLine(fields[1] + "," + fields[2] + ",Undefined");   
                }
            }

            sw.Flush();
            sw.Close();

            ConfusionMatrix matrix = new ConfusionMatrix(predicted.ToArray(), expected.ToArray());

            GeneralConfusionMatrix matrixGen = new GeneralConfusionMatrix(classlist.Count, expected.ToArray(), predicted.ToArray());

            

            textBox3.Text = DateTime.Now + "   " + "k: " + textBox2.Text + "   Distance: " + comboBox2.SelectedItem.ToString() + "   Vector: " + comboBox4.SelectedItem.ToString();
            textBox3.Text += "   Number of instances: " + indexRows + "    Number of classes: " + classlist.Count;
            textBox3.Text += "   Correctly classified: " + correctCount + "   Wrongly classified: " + wrongCount;
            textBox3.Text += "   Standard Error " + matrixGen.StandardError;
            textBox3.Text += "   Accuracy " + (double)((double)correctCount / (double)indexRows);
            //textBox3.Text += "   Conf. Matrix " + matrix;

            //textBox8.Text = "Rows " + matrixGen.Matrix.Rows();
            if (checkBox1.Checked)
            {
                paintMatrix(matrixGen, inverseClassList);
            }
            label8.Text = "-";
        }

        

        private void paintMatrix(GeneralConfusionMatrix matrix, Dictionary<int,string> classes) 
        {
            string logfile = SelectedTestFile.Replace(".csv", "_Result_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt");
            StreamWriter log = new StreamWriter(logfile);
            log.WriteLine(textBox3.Text);
            log.Flush();
            log.Close();

            string outputFile = SelectedTestFile.Replace(".csv","_Matrix_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv");
            StreamWriter file = new StreamWriter(outputFile);
            string line = "";
            for (int i = 0; i < classes.Count; ++i) {
                line += "," + classes[i];
            }

            file.WriteLine(line);

            for (int i = 0; i < matrix.Matrix.Rows(); ++i) {
                line = classes[i];
                for (int j = 0; j < matrix.Matrix.Columns(); ++j) {
                    line += "," + matrix.Matrix[i, j];      
                }
                file.WriteLine(line);
            }

            file.Flush();
            file.Close();

            createMatrix(outputFile);
        }


        public void createMatrix(string fileName)
        {
            string[] delimiters = { "," };
            string[] fields;
            TextFieldParser tfp;

            string file = fileName;
            tfp = new TextFieldParser(file);
            StreamWriter sw = new StreamWriter(file.Replace(".csv", "_clean.csv"));
            int threshold = 10;

            tfp.HasFieldsEnclosedInQuotes = true;
            tfp.Delimiters = delimiters;
            fields = tfp.ReadFields();
            string line = fields[0];

            List<List<string>> strMatrix = new List<List<string>>();

            strMatrix.Add(new List<string>());
            strMatrix[0].Add(fields[0]);

            Dictionary<string, int> classList = new Dictionary<string, int>();
            Dictionary<string, int> classMax = new Dictionary<string, int>();
            Dictionary<string, int> classMin = new Dictionary<string, int>();


            for (int i = 1; i < fields.Length; ++i)
            {

                classList[fields[i]] = 0;
                classMax[fields[i]] = -1;
                classMin[fields[i]] = 99999;
                line += "," + fields[i];
                strMatrix[0].Add(fields[i]);
            }

            //            sw.WriteLine(line);
            int temp;

            int idx = 1;
            List<int> indexRemove = new List<int>();

            while (!tfp.EndOfData)
            {
                fields = tfp.ReadFields();
                line = fields[0];
                temp = 0;
                for (int i = 1; i < fields.Length; ++i)
                {
                    line += "," + fields[i];
                    temp += Int32.Parse(fields[i]);
                    classList[fields[0]] += Int32.Parse(fields[i]);
                    classMax[fields[0]] = (Int32.Parse(fields[i]) > classMax[fields[0]]) ? Int32.Parse(fields[i]) : classMax[fields[0]];
                    classMin[fields[0]] = (Int32.Parse(fields[i]) < classMin[fields[0]]) ? Int32.Parse(fields[i]) : classMin[fields[0]];
                }

                if (temp > threshold)
                {
                    strMatrix.Add(line.Split(',').ToList());
                    //sw.WriteLine(line);               
                }
                else
                {
                    indexRemove.Add(idx);
                }
                ++idx;
            }


            for (int i = 0; i < strMatrix.Count; ++i)
            {
                line = strMatrix[i][0];
                for (int j = 1; j < strMatrix[0].Count; ++j)
                {
                    if (!indexRemove.Contains(j))
                    {
                        if (i == 0)
                        {
                            line += "," + strMatrix[i][j];
                        }
                        else {
                            //double dtemp = (double)((Double.Parse(strMatrix[i][j]) - (double)classMin[strMatrix[i][0]]) / ((double)classMax[strMatrix[i][0]] - (double)classMin[strMatrix[i][0]]));
                            double dtemp = Double.Parse(strMatrix[i][j]) / (double)classList[strMatrix[i][0]];
                            line += "," + dtemp;
                        }
                    }
                }
                sw.WriteLine(line);
            }

            tfp.Close();
            sw.Flush();
            sw.Close();

        }

        private void computebtn_Click(object sender, EventArgs e)
        {
            int[] output;
            knnTxt.Text = "";
            if (comboBox2.SelectedItem.ToString() == "Levenshtein")
            {
                var answer = knnStr.GetNearestNeighbors(tesTxt.Text.ToUpper(), out output);
                for (int i = 0; i < answer.Length; ++i) {
                    knnTxt.Text += answer[i] + "," + inverseClassList[output[i]] + Environment.NewLine;
                }

                int classInt = knnStr.Compute(tesTxt.Text.ToUpper());
                label5.Text = inverseClassList[classInt];

            }
            else
            {
                var answer = knn.GetNearestNeighbors(wv.transform(tesTxt.Text.ToUpper(), comboBox4.SelectedItem.ToString()), out output);
                for (int i = 0; i < answer.Length; ++i)
                {
                    knnTxt.Text += wv.transformInverse(answer[i], comboBox4.SelectedItem.ToString()) + "," + inverseClassList[output[i]] + Environment.NewLine;
                }

                int classInt = knn.Compute(wv.transform(tesTxt.Text.ToUpper(), comboBox4.SelectedItem.ToString()));
                label5.Text = inverseClassList[classInt];
                //answer = knn.Compute(wv.transform(GetString(fields, " "), comboBox4.SelectedItem.ToString()));
            }
        }

        

        public static double Jaccard(double [] x1, double [] x2)
        {
            HashSet<int> hs1 = new HashSet<int>();
            HashSet<int> hs2 = new HashSet<int>();

            for (int i = 0; i < x1.Length; ++i) {
                if (x1[i] > 0) { hs1.Add(i); }
                if (x2[i] > 0) { hs2.Add(i); }
            }

            return (1-((double)hs1.Intersect(hs2).Count() / (double)hs1.Union(hs2).Count()));
        }

            }
}

