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

namespace TextMachineLearning
{
    public partial class Form1 : Form
    {
        private string SelectedFile = "";
        private int SelectedClass = 0;
        private KNearestNeighbors knn;
        private KNearestNeighbors<string> knnStr;
        Dictionary<string, int> classlist;
        Dictionary<int, string> inverseClassList;

        public Form1()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
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

                if (fields.Length > 1) {

                    SelectedFile = textBox1.Text;
                    SelectedClass = 0;
                    label2.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox1.DataSource = fields;
                    label3.Enabled = true;
                    comboBox2.Enabled = true;
                    label4.Enabled = true;
                    textBox2.Enabled = true;

                    label5.Enabled = true;
                    comboBox3.Enabled = true;
                    label6.Enabled = true;
                    comboBox4.Enabled = true;
                    
                }                

                tfp.Close();
                ValidateFields();
            }
            catch (Exception ex) {
                label2.Enabled = false;
                comboBox1.Enabled = false;
                label3.Enabled = false;
                comboBox2.Enabled = false;
                label4.Enabled = false;
                textBox2.Enabled = false;
                label5.Enabled = false;
                comboBox3.Enabled = false;
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
            if (Int32.TryParse(textBox2.Text, out temp) && ((Int32.TryParse(textBox4.Text, out temp) && comboBox4.SelectedIndex == 1) || (comboBox4.SelectedIndex == 0)))
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 1)
            {
                textBox4.Enabled = true;
                label7.Enabled = true;
            }
            else {
                textBox4.Enabled = false;
                label7.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedClass = comboBox1.SelectedIndex;
        }

        private string GetString(string[] fields) { 
            string result = "";
            for (int i = 0; i < fields.Length; ++i) {
                if (i != SelectedClass) {
                    result += fields[i];
                }
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            //Get list of classes, input and output vectors
            classlist = new Dictionary<string, int>();
            inverseClassList = new Dictionary<int, string>();
            List<List<double>> IntMatrixInputs = new List<List<double>>();
            List<string> stringInputs = new List<string>();
            List<int> IntOutputs = new List<int>();
 
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
                fields = tfp.ReadFields();
                if (fields[SelectedClass] != "")
                {
                    if (!classlist.TryGetValue(fields[SelectedClass], out temp))
                    {
                        classlist[fields[SelectedClass]] = indexClass;
                        inverseClassList[indexClass] = fields[SelectedClass];
                        ++indexClass;
                    }
                    stringInputs.Add(GetString(fields));
                    IntMatrixInputs.Add(Word2Vec.Transform(GetString(fields), false).ToList()); //getLettersVector.toList();
                    IntOutputs.Add(classlist[fields[SelectedClass]]);

                    ++indexRows;
                }
            }

            int trainIndex = (int)(indexRows * 0.75);

            List<string> strInputsTrain = new List<string>(); 
            List<string> strInputsTest = new List<string>();

            double[][] IntInputsTrain = new double[trainIndex][];
            double[][] IntInputsTest = new double[IntMatrixInputs.Count-trainIndex][];

            int[] outputsTrain = new int[trainIndex];
            int[] outputsTest = new int[IntMatrixInputs.Count - trainIndex];

            double [][] IntInputs = new double[IntMatrixInputs.Count][];
            for (int i =0; i < IntInputs.Length; ++i){
                 IntInputs[i] = IntMatrixInputs[i].ToArray();

                 if (i < trainIndex)
                 {
                     IntInputsTrain[i] = IntMatrixInputs[i].ToArray();
                     strInputsTrain.Add(stringInputs[i]);

                     outputsTrain[i] = IntOutputs[i];
                 }
                 else {
                     IntInputsTest[i-trainIndex] = IntMatrixInputs[i].ToArray();
                     strInputsTest.Add(stringInputs[i]);
                     outputsTest[i-trainIndex] = IntOutputs[i];
                 }
            }

            textBox3.Text = "Processing";


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
            else {
                knn = new KNearestNeighbors(k: Int32.Parse(textBox2.Text), classes: classlist.Count,
                inputs: IntInputsTrain, outputs: outputsTrain, distance: Distance.Cosine);
            }


            int correctCount = 0;
            int wrongCount = 0;

            for (int i = 0; i < strInputsTest.Count; ++i) {
                int answer;
                if (comboBox2.SelectedItem.ToString() == "Levenshtein")
                {
                    answer = knnStr.Compute(strInputsTest[i]);
                    
                }
                else {
                    answer = knn.Compute(IntInputsTest[i]);
                }
                if (answer == outputsTest[i])
                {
                    correctCount++;
                }
                else {
                    wrongCount++;
                }

            }


            textBox3.Text = DateTime.Now + " Completed    Number of instances: " + indexRows + "    Number of classes: " + classlist.Count;
            textBox3.Text += "   Correctly classified: " + correctCount + "   Wrongly classified: " + wrongCount;
           
            label8.Text = "-";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int answer = 0;
            if (comboBox2.SelectedItem.ToString() == "Levenshtein")
            {
                answer = knnStr.Compute(textBox5.Text + textBox6.Text + textBox7.Text);
            }
            else
            {
                double[] input = Word2Vec.Transform(textBox5.Text + textBox6.Text + textBox7.Text, false);
                answer = knn.Compute(input);
            }

            label8.Text = inverseClassList[answer];
        }    
    
    }
}

