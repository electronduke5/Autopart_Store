using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace University.Cards
{
    public partial class MyDialog : Window
    {
        public MyDialog(int maxCount)
        {
            InitializeComponent();
            MaxCount = maxCount;
        }
        private int MaxCount { get; }
        public string ResponseText {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if(Int32.Parse(ResponseTextBox.Text) > MaxCount)
                MessageBox.Show($"Количество бракованных запчастей не должно превышать {MaxCount} шт!", "=(",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            else DialogResult = true;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
    }
}