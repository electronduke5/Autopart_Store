using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using University.Database;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace University.Cards
{
    public partial class CartItem : UserControl
    {
        private UniversityContext db;
        public CartItem()
        {
            InitializeComponent();
            DataContext = this;
            db = new UniversityContext();
            tbSelectCount.Text = SelectedCount.ToString();
        }

        public CombinationOrder _CombinationOrder { get; set; }
        public string AutopartName { get; set; }
        public int SelectedCount { get; set; }
        public decimal Price { get; set; }
        public int MaxCount { get; set; }
        public decimal FullPrice { get; set; }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CombinationOrder deleteAutopart =
                await db.CombinationOrders.FirstOrDefaultAsync(a => a.IdCombination == _CombinationOrder.IdCombination);
            db.CombinationOrders.Remove(deleteAutopart);
            await db.SaveChangesAsync();
            
            Button btn = sender as Button;
            var conditionUserControl = Helper.Helper.FindParent<CartItem>(btn);
            if (conditionUserControl != null)
            {
                var sp = Helper.Helper.FindParent<StackPanel>(conditionUserControl);
                if (sp != null)
                    sp.Children.Remove(conditionUserControl);
            }

            MessageBox.Show($"Товар удален из корзины!", "=)",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void TbSelectCount_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //MessageBox.Show($"{((IntegerUpDown) sender).Text}");
             
             CombinationOrder updateCombination =
                 await db.CombinationOrders.FirstOrDefaultAsync(a =>
                     a.IdCombination == _CombinationOrder.IdCombination);
             updateCombination.Count = Int32.Parse(((IntegerUpDown) sender).Text);
             db.Update(updateCombination);
             await db.SaveChangesAsync();
        }
    }
}