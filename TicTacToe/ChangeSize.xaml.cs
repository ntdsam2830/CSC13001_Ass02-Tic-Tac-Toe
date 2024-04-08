using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class ChangeSize : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public int changedValue { get; set; }

        public ChangeSize()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            string size = sizeBoardTxt.Text;
            if (!string.IsNullOrEmpty(size))
            {
                int _size = int.Parse(size);
                if (_size <= 5)
                {
                    MessageBox.Show("Invalid Value");
                    DialogResult = false;
                    Close();
                }
                else
                {
                    changedValue = _size;
                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
