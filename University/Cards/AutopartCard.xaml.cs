using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using University.Database;
using University.Helper;

namespace University.Cards
{
    public partial class AutopartCard : UserControl
    {
        private UniversityContext db;

        public AutopartCard()
        {
            InitializeComponent();
            db = new UniversityContext();
            DataContext = this;
        }

        public Autopart _Autopart { get; set; }
        public Category _Category { get; set; }
        public Provider _Provider { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string AutopartName { get; set; }

        public string CategoryName { get; set; }
        public string ProviderName { get; set; }

        private async void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            System.Diagnostics.Debug.WriteLine($"Card is flipped = {e.NewValue}");

            cbCategory.DataContext = await db.Categories.ToListAsync();
            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "IdCategory";
            cbCategory.SelectedIndex = _Autopart.CategoryId - 1;

            cbProvider.DataContext = await db.Providers.ToListAsync();
            cbProvider.DisplayMemberPath = "ProviderName";
            cbProvider.SelectedValuePath = "IdProvider";
            cbProvider.SelectedValue = _Autopart.ProviderId;

            tbAutopartName.Text = AutopartName;
            tbCount.Text = Count.ToString();
            tbPrice.Text = Price.ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");

        private int ResultDialog(int TotalCount)
        {
            int countPartDefect = 0;

            var dialog = new MyDialog(TotalCount);
            if (dialog.ShowDialog() == true)
            {
                countPartDefect = Int32.Parse(dialog.ResponseText);
            }

            return countPartDefect;
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Autopart deleteAutopart = await db.Autoparts.FirstOrDefaultAsync
            (
                a => a.IdAutopart == _Autopart.IdAutopart
            );
            //Количество бракованных запчастей
            int countPartDefect = ResultDialog(deleteAutopart.Count);
            
            //Проверяем, есть ли в таблице с бракованными запчастями
            //запись с таким же ID запчасти. 
            //И если есть, то мы ее перезапиысываем, если нет - то создаем новую запись.
            var defect = await db.DefectAutoparts.FirstOrDefaultAsync(
                a => a.AutopartId == deleteAutopart.IdAutopart);
            if (defect == null)
            {
                await db.DefectAutoparts.AddAsync(new DefectAutopart
                {
                    AutopartId = deleteAutopart.IdAutopart,
                    Count = countPartDefect
                });
            }
            else defect.Count += countPartDefect;

            await db.Accountings.AddAsync(new Accounting
            {
                Expenditure = (deleteAutopart.Price * countPartDefect) * -1
            });

            deleteAutopart.Count -= countPartDefect;
            await db.SaveChangesAsync();
            
            if (deleteAutopart.Count <= 0)
            {
                Button btn = sender as Button;
                var conditionUserControl = Helper.Helper.FindParent<AutopartCard>(btn);
                if (conditionUserControl != null)
                {
                    var sp = Helper.Helper.FindParent<StackPanel>(conditionUserControl);
                    if (sp != null)
                        sp.Children.Remove(conditionUserControl);
                }
            }
            txtCount.Text = deleteAutopart.Count.ToString();
            
            MessageBox.Show("Товар успешно переведен в брак!", "=)",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BtnEditAutopart_OnClick(object sender, RoutedEventArgs e)
        {
            string autopartName = tbAutopartName.Text.Trim();

            if (autopartName != ""
                || tbCount.Text.Trim() != ""
                || tbPrice.Text.Trim() != "")
            {
                if (Decimal.Parse(tbPrice.Text.Trim()) > 0)
                {
                    if (autopartName.Length <= 50)
                    {
                        Autopart newAutopart = new Autopart
                        {
                            IdAutopart = _Autopart.IdAutopart,
                            Price = Decimal.Parse(tbPrice.Text.Trim()),
                            Count = Int32.Parse(tbCount.Text.Trim()),
                            CategoryId = (int) cbCategory.SelectedValue,
                            ProviderId = (int) cbProvider.SelectedValue,
                            AutopartName = autopartName,
                        };

                        _Autopart = newAutopart;
                        Price = _Autopart.Price;
                        Count = _Autopart.Count;
                        _Autopart.Category = (Category) cbCategory.SelectedItem;
                        CategoryName = _Autopart.Category.CategoryName;
                        _Autopart.Provider = (Provider) cbProvider.SelectedItem;
                        ProviderName = _Autopart.Provider.ProviderName;
                        AutopartName = _Autopart.AutopartName;
                        //Заполнем поля на главной стороне карточки
                        txtCount.Text = Count.ToString();
                        txtPrice.Text = Price.ToString();
                        txtAutopartName.Text = AutopartName;
                        txtCategoryName.Text = CategoryName;
                        txtProviderName.Text = ProviderName;

                        var autopart = db.Autoparts.First
                        (
                            a => a.IdAutopart == _Autopart.IdAutopart
                        );
                        db.Entry(autopart).CurrentValues.SetValues(_Autopart);

                        await db.SaveChangesAsync();

                        MessageBox.Show("Данные обновленны!", "=)",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show("Название должно быть короче 50 символов!", "=(",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Цена должна быть больше 0!", "=(",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Заполните все поля!", "=(",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}