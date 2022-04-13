using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using University.Database;
using University.Main;

namespace University.Cards
{
    public partial class ProviderCard : UserControl
    {
        private UniversityContext db;
        private Employee LoggedUser;
        public ProviderCard(Employee loggedUser)
        {
            InitializeComponent();
            LoggedUser = loggedUser;
            db = new UniversityContext();
            DataContext = this;
        }

        public Provider _Provider { get; set; }

        public string ProviderName { get; set; }

        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            System.Diagnostics.Debug.WriteLine($"Card is flipped = {e.NewValue}");

            tbNameProvider.Text = ProviderName;
        }

        private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var autopart = await db.Autoparts.FirstOrDefaultAsync(o => o.ProviderId == _Provider.IdProvider);

            if (autopart == null)
            {
                Provider deleteProvider =
                    await db.Providers.FirstOrDefaultAsync(a => a.IdProvider == _Provider.IdProvider);
                db.Providers.Remove(deleteProvider);
                await db.SaveChangesAsync();

                Button btn = sender as Button;
                var conditionUserControl = FindParent<ProviderCard>(btn);
                if (conditionUserControl != null)
                {
                    var sp = FindParent<StackPanel>(conditionUserControl);
                    if (sp != null)
                        sp.Children.Remove(conditionUserControl);
                }
                //Надо обновлять ComboBox при удалении поставщика!!!!
                
                // AdminPage adminPage = new AdminPage(LoggedUser);
                // adminPage.cbProvider.DataContext = db.Providers.ToList();
                // adminPage.cbProvider.DisplayMemberPath = "ProviderName";
                // adminPage.cbProvider.SelectedValuePath = "IdProvider";
                // adminPage.cbProvider.SelectedIndex = 0;

                MessageBox.Show("Поставщик успешно удалён!", "=)", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Данного поставщика удалить нельзя, так как имеются запчасти от данного поставщика!",
                    "=(", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        private async void BtnEditProvider_OnClick(object sender, RoutedEventArgs e)
        {
            var provider = await db.Providers.Where(p => p.ProviderName == tbNameProvider.Text.Trim())
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                if (tbNameProvider.Text.Trim() != "")
                {
                    if (tbNameProvider.Text.Length <= 30)
                    {
                        Provider newProvider = new Provider
                        {
                            IdProvider = _Provider.IdProvider,
                            ProviderName = tbNameProvider.Text.Trim()
                        };

                        _Provider = newProvider;
                        ProviderName = newProvider.ProviderName;
                        txtProvider.Text = ProviderName;

                        var prov = await db.Providers.FirstAsync(p => p.IdProvider == _Provider.IdProvider);
                        db.Entry(prov).CurrentValues.SetValues(_Provider);
                        await db.SaveChangesAsync();
                    }
                    else
                        MessageBox.Show("Название должно быть меньше 30 символов!", "=(",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Заполните поля названия поставщика!", "=(", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Данный поставщик уже добавлен!", "=(",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}