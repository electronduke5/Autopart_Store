using System;
using System.Windows.Controls;
using System.Windows.Media;
using University.Database;

namespace University.Cards
{
    public partial class FinanceCard : UserControl
    {
        private UniversityContext db;
        public FinanceCard( string date, decimal result)
        {
            InitializeComponent();
            DataContext = this;
            db = new UniversityContext();
            Date = date;
            Result = result;
            txtDate.Text = Date;
            txtResult.Text = Result.ToString();
            txtResult.Foreground = Result > 0 ? Brushes.Green : Brushes.Tomato;
        }

        public string Date { get; set; }
        public decimal Result { get; set; }
    }
}