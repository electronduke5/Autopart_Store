using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using University.Cards;
using University.Database;
using University.Main;

namespace University.Login
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        UniversityContext db;
        MainWindow mainWindow;

        public Login(MainWindow main)
        {
            InitializeComponent();
            db = new UniversityContext();
            mainWindow = main;
        }

        private async void btLogin_Click(object sender, RoutedEventArgs e)
        {
            string phoneNumber = tbLogin.Text;
            if (!tbLogin.IsMaskCompleted)
            {
                tbErrorLogin.Text = "Номер телефона не может быть пустым!";
                //MessageBox.Show("Поле не может быть пустым!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = await db.Employees.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var customer = await db.Customers.FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);

            if (user == null && customer == null)
            {
                tbErrorLogin.Text = "Аккаунта с таким телефоном не существует!";
                return;
            }
    
            if ( user != null && phoneNumber == user.PhoneNumber)
            {
                if (tbPassword.Password.Trim() == user.Password)
                    mainWindow.Frame.Content = new AdminPage(user);
                else tbErrorPassword.Text = "Неверный пароль!";
            }

            else if (customer!= null && phoneNumber == customer.PhoneNumber)
            {
                if (tbPassword.Password.Trim() == customer.Password)
                    mainWindow.Frame.Content = new CustomerPage(customer);
                else tbErrorPassword.Text = "Неверный пароль";
            }
            else MessageBox.Show("Вход нельзя!", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void BtnRegister_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new RegisterDialog();
            if (dialog.ShowDialog() == true)
            {
                var customer = await db.Customers.FirstOrDefaultAsync(a => a.PhoneNumber == dialog.tbPhoneNumber.Text.Trim());
                mainWindow.Frame.Content = new CustomerPage(customer);
            }
        }
    }
}