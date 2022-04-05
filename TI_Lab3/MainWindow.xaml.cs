using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace TI_Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _filePath = String.Empty;

        private byte[] bytes;

        private short[] ciphertext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindPrimRoots_Click(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(PrimeNum_TextBox.Text, out int primNum))
            {
                MessageBox.Show("Некорректное значение в поле с простым числом!");
                return;
            }

            if (!El_Gamal.IsPrime(primNum))
            {
                MessageBox.Show("Число p должно быть простое!");
                return;
            }

            AllPrimRoots_ListBox.Items.Clear();
            List<int> primitiveRoots = El_Gamal.PrimitiveRoots(primNum);
            for(int i = 0; i < primitiveRoots.Count; i++)
            {
                AllPrimRoots_ListBox.Items.Add(primitiveRoots[i]);
            }
            PrimRootsCount_TextBox.Text = AllPrimRoots_ListBox.Items.Count.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AllPrimRoots_ListBox.SelectedItem == null)
                return;
            ChoosenPrimRoot_TextBox.Text = AllPrimRoots_ListBox.SelectedItem.ToString();
        }

        private void OpenFile_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                _filePath = ofd.FileName;
                bytes = File.ReadAllBytes(ofd.FileName);
                StringBuilder sb = new StringBuilder();
                foreach(byte b in bytes)
                {
                    sb.Append(Convert.ToString(b, 10));
                    sb.Append(" ");
                }
                PlainText_TextBox.Text = sb.ToString();
            }
        }

        private void Encrypt_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            if (PrimeNum_TextBox.Text == String.Empty)
            {
                MessageBox.Show("Введите простое число p!");
                return;
            }

            if (!int.TryParse(SecretKey_TextBox.Text, out int secretKey))
            {
                MessageBox.Show("Секретный ключ содержит недопустимые символы!");
                return;
            }
            else if(secretKey < 2 || secretKey >= int.Parse(PrimeNum_TextBox.Text) - 1)
            {
                MessageBox.Show("Введен неверный секретный ключ!");
                return;
            }

            if(!int.TryParse(SecretNum_TextBox.Text, out int secretNum))
            {
                MessageBox.Show("Секретное число содержит недопустимые символы!");
                return;
            }
            else if(secretNum < 2 || secretNum >= int.Parse(PrimeNum_TextBox.Text) - 1 || 
                El_Gamal.FindGCD(int.Parse(PrimeNum_TextBox.Text) - 1, secretNum) != 1)
            {
                MessageBox.Show("Введено неверное секретное число!");
                return;
            }

            if (AllPrimRoots_ListBox.Items.Count == 0)
            {
                MessageBox.Show("Необходимо найти первообразные корни");
                return;
            }

            if (ChoosenPrimRoot_TextBox.Text == String.Empty)
            {
                MessageBox.Show("Выберите один из первообразных корней!");
                return;
            }           

            if(PlainText_TextBox.Text == String.Empty)
            {
                MessageBox.Show("Выберите файл для шифрования!");
                return;
            }
                
            if(btn.Tag.ToString() == "Encrypt")
            {
                List<int> encrData = new List<int>();
                encrData.Add(int.Parse(ChoosenPrimRoot_TextBox.Text));
                encrData.Add(int.Parse(PrimeNum_TextBox.Text));
                encrData.Add(int.Parse(SecretNum_TextBox.Text));
                encrData.Add(int.Parse(SecretKey_TextBox.Text));

                ciphertext = El_Gamal.Encrypt(encrData, bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ciphertext.Length; i += 2)
                {
                    sb.Append(Convert.ToString(ciphertext[i], 10));
                    sb.Append(',');
                    sb.Append(Convert.ToString(ciphertext[i + 1], 10));
                    sb.Append(' ');
                }
                ResultText_TextBox.Text = sb.ToString();

                string fileName = System.IO.Path.GetDirectoryName(_filePath) + '\\' + "New_" +
                    System.IO.Path.GetFileName(_filePath);
                var span = new Span<short>(ciphertext);
                var byt = MemoryMarshal.AsBytes(span);
                var byteArray = byt.ToArray();
                File.WriteAllBytes(fileName, byteArray);
            }
            else
            {
                List<int> decrData = new List<int>();
                decrData.Add(int.Parse(SecretKey_TextBox.Text));
                decrData.Add(int.Parse(PrimeNum_TextBox.Text));
                byte[] resultText = El_Gamal.Decrypt(decrData, ciphertext);
                StringBuilder sb = new StringBuilder();
                foreach(byte b in resultText)
                {
                    sb.Append(Convert.ToString(b, 10));
                    sb.Append(' ');
                }
                ResultText_TextBox.Text = sb.ToString();

                string fileName = System.IO.Path.GetDirectoryName(_filePath) + '\\' + "New_" +
                    System.IO.Path.GetFileName(_filePath);
                File.WriteAllBytes(fileName, resultText);
            }        
        }
    }
}
