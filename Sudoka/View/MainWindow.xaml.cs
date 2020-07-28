using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp2
{
    enum Difficulty {Easy = 25, Medium = 35, Hard = 45}
    public partial class MainWindow : Window
    {
        List<TextBox> TB;
        List<TextBox> textBoxDef;
        Difficulty difficulty;
        SudokaGenerate sudoka;

        bool Mode;

        public MainWindow()
        {
            InitializeComponent();

            comboBoxDifficulty.SelectedIndex = 2;
            comboBoxMode.SelectedIndex = 0;
            difficulty = Difficulty.Hard;
            TB = new List<TextBox>();
            textBoxDef = new List<TextBox>();


            for (int i =0; i<table.Children.Count; i++)
            {
                TB.Add((TextBox)table.Children[i]);
            }

            sudoka = new SudokaGenerate();
            sudoka.GenerateSudoka(difficulty);
            ShowGenerateSudoku();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TB = (TextBox)sender;
            char[] str;

            str = TB.Text.ToCharArray();
            int length = str.Length - 1;
            if(length>=0) TB.Text = str[length].ToString();
            TB.SelectionStart = TB.Text.Length;

            if (!int.TryParse(TB.Text, out int result)) TB.Text = "";
            if (result < 1 || result > 9) TB.Text = "";
        }

        private void ButtonSolution_Click(object sender, RoutedEventArgs e)
        {
            if(Mode == false)
            {
                try
                {
                    for(int i = 0; i<TB.Count; i++)
                    {
                        TB[i].Background = Brushes.White;
                        if(TB[i].Text == "") throw new Exception(string.Format("Все ячейки должны быть заполнены!"));
                    }

                    ReadTable();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    ReadTable();

                    ButtonSolution.IsEnabled = false;
                    ButtonGenerate.IsEnabled = false;

                    Thread thread = new Thread(new ThreadStart(ThreadStartSolution));
                    thread.IsBackground = true;
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ReadTable()
        {
            int res;
            bool IsExpection = false;

            for (int i = 0; i < sudoka.N; i++)
            {
                for (int j = 0; j < sudoka.N; j++)
                {
                    sudoka.baseTable[i, j] = 0;
                }
            }

            for(int i = 0, k = 0; i<sudoka.N; i++)
            {
                for(int j = 0; j< sudoka.N; j++, k++)
                {
                    if (TB[k].Text == "") continue;

                    if(int.TryParse(TB[k].Text, out res) && res < 10 && res>0)
                    {
                        if (!sudoka.isSafe(sudoka.baseTable, i, j, res))
                        {
                            IsExpection = true;
                            TB[k].Background = Brushes.Red;

                        }

                        sudoka.baseTable[i, j] = res;
                    }
                    else
                    {
                        throw new Exception(string.Format("В ячейке {0},{1} некорректное значение", i, j));
                    }
                }
            }

            if(IsExpection == true)
            {
                throw new Exception(string.Format("Значение в таблице не удоволетворяет правилам судоку"));
            }
            else if(Mode == false)
            {
                MessageBox.Show("Судоку решен верно!");
            }
        }
        private void ThreadStartSolution()
        {
            if (sudoka.solveSudokySafe(false, sudoka.baseTable) == false)
            {
                Dispatcher.Invoke((ThreadStart)delegate { ButtonGenerate.IsEnabled = true; });
                MessageBox.Show("Для данной таблицы отсутствуют решения");
                Thread.CurrentThread.Abort();
            }

            Dispatcher.Invoke((ThreadStart)delegate 
            {
                ButtonGenerate.IsEnabled = true;
                ShowSolutionSudoku();
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(ThreadGenerateSudoka));
            thread.IsBackground = true;
            ButtonGenerate.IsEnabled = false;
            ButtonSolution.IsEnabled = false;


            thread.Start();
        }

        private void ThreadGenerateSudoka()
        {
            sudoka.GenerateSudoka(difficulty);
            Dispatcher.Invoke((ThreadStart)delegate
            {
                ButtonGenerate.IsEnabled = true;
                ButtonSolution.IsEnabled = true;
                ShowGenerateSudoku();
                WriteInFile();
            });
        }
        private void ShowGenerateSudoku()
        {
            if(textBoxDef.Count != 0)
            {
                for(int i = 0; i<textBoxDef.Count; i++)
                {
                    textBoxDef[i].Background = Brushes.White;
                }
                textBoxDef.Clear();
            }

            for (int i = 0, k = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++, k++)
                {
                    if (sudoka.baseTable[i, j] != 0)
                    {
                        TB[k].Text = sudoka.baseTable[i, j].ToString();
                        TB[k].IsReadOnly = true;
                    }
                    else
                    {
                        TB[k].Text = "";
                        textBoxDef.Add(TB[k]);
                        TB[k].IsReadOnly = false;
                    }
                }
            }
        }

        private void WriteInFile()
        {
            string writePath = "sudoku.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false))
                {
                    for(int i = 0; i < 9; i++)
                    {
                        for(int j = 0; j < 9; j++)
                        {
                            sw.Write(sudoka.baseTable[i, j].ToString() + " ");
                        }
                        sw.WriteLine("");
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ShowSolutionSudoku()
        {
            for (int i = 0, k = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++, k++)
                {
                    for (int x = 0; x < textBoxDef.Count; x++)
                    {
                        if (TB[k] == textBoxDef[x])
                        {
                           // TB[k].Foreground = Brushes.Green;
                            TB[k].Background = Brushes.LightGreen;
                            break;
                        }
                    }
                    TB[k].Text = sudoka.baseTable[i, j].ToString();
                }
            }
        }

        private void ComboBoxDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;

            switch(comboBox.SelectedIndex)
            {
                case 0:
                    {
                        difficulty = Difficulty.Easy;
                        break;
                    }
                case 1:
                    {
                        difficulty = Difficulty.Medium;
                        break;
                    }
                case 2:
                    {
                        difficulty = Difficulty.Hard;
                        break;
                    }
            }
        }

        private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    {
                        Mode = false;
                        ButtonSolution.Content = "Проверить решение";

                        if(textBoxDef != null)
                        {
                            for (int i = 0; i < textBoxDef.Count; i++)
                            {
                                textBoxDef[i].IsReadOnly = false;
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        Mode = true;
                        ButtonSolution.Content = "Решить судоку";

                        for (int i = 0; i < TB.Count; i++)
                        {
                            TB[i].IsReadOnly = true;
                        }
                        break;
                    }
            }
        }

        private void ButtonRules_Click(object sender, RoutedEventArgs e)
        {
            string rules = " - В одной строке цифра может встречаться только один раз\n" +
                           " - В одном столбце цифра может встречаться только один раз\n" +
                           " - В одном маленьком квадрате цифра может встречаться только один раз";

            MessageBox.Show(rules, "Правила игры:");
        }

        private void OpenSudoku_Click(object sender, RoutedEventArgs e)
        {
            DefaultDialogService dds = new DefaultDialogService();
            dds.OpenFileDialog();

            if (dds.FilePath == null) return;

            try
            {
                string str;
                using (StreamReader sr = new StreamReader(dds.FilePath))
                {
                    str = sr.ReadToEnd();
                }
                string [] num = new string[81];
                num = str.Split(' ');

                for(int i = 0; i<81; i++)
                {
                    TB[i].Text = num[i];
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            
            
        }
    }
}
