using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Babble
{
    /// <summary>
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    /// </summary>
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        private Dictionary<string, ArrayList> wordDict = new Dictionary<string, ArrayList>();

        private void WordCountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // Retrieve word count (with default fallback)
        private int GetWordCount()
        {
            if (int.TryParse(wordCountTextBox.Text, out int wordCount))
            {
                return wordCount;
            }
            return 200; // Default word count
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool)ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
                textBlock1.Text = ""; //I removed the loading message because it confused me for so long thinking the program was stuck when it was working
                analyzeInput(orderComboBox.SelectedIndex); //analyzes Input for selected order
            }
        }

        private void analyzeInput(int order)
        {
            //check if the file has words
            if (words == null || words.Length == 0)
            {
                MessageBox.Show("No words found. Please load a valid text file.");
                return;
            }

            wordDict.Clear(); // Clear the previous data

            // create thye dictionary of followers
            for (int i = 0; i < words.Length - order; i++)
            {
                string key = string.Join("-", words.Skip(i).Take(order));

                string nextWord = words[i + order];

                if (!wordDict.ContainsKey(key))
                    wordDict.Add(key, new ArrayList());

                wordDict[key].Add(nextWord);
            }

            // display the total number of words and the number of unique keys found
            textBlock1.Text += $"Analyzed {words.Length} words. Found {wordDict.Count} unique keys at order {order}.\n";
        }

        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            wordCount = GetWordCount();
            if (orderComboBox.SelectedIndex == 0)
                wordCount = GetWordCount() * 2;

            textBlock1.Text = ""; // Clear the output
            Random rand = new Random();

            //check if there are words in the dictionary
            if (wordDict.Count == 0) return;

            // Randomly pick a starting key from the dictionary
            var randomKey = wordDict.Keys.ElementAt(rand.Next(wordDict.Count));
            textBlock1.Text += randomKey + " "; // Output the starting key

            string currentKey = randomKey;

            //Generate the output
            for (int i = 1; i < wordCount; i++)
            {
                if (wordDict.ContainsKey(currentKey))
                {
                    ArrayList followers = wordDict[currentKey];
                    string nextWord = (string)followers[rand.Next(followers.Count)];

                    textBlock1.Text += nextWord + " ";

                    // Update currentKey by removing the first word and adding the next word
                    string[] keyParts = currentKey.Split('-');
                    currentKey = string.Join("-", keyParts.Skip(1).Concat(new[] { nextWord }));
                }
                else
                {
                    // If there is no follower for the current key, pick a new starting key
                    currentKey = wordDict.Keys.ElementAt(rand.Next(wordDict.Count));
                }
            }
        }

        //check if selection has changed and reanilyse
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (words != null && words.Length > 0)
            {
                textBlock1.Text = ""; // Clear the output
                analyzeInput(orderComboBox.SelectedIndex);
            }
        }
    }
}
