using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using University.Database;
using University.Main;

namespace University.Cards
{
    public partial class RegisterDialog : Window
    {
        private readonly UniversityContext db;

        public RegisterDialog()
        {
            InitializeComponent();
            db = new UniversityContext();
        }

        private string Surname => tbSurname.Text.Trim();
        private string Name => tbName.Text.Trim();
        private string Patronimyc => tbPatronimyc.Text.Trim();
        private string PhoneNumber => tbPhoneNumber.Text.Trim();
        private string Password => tbPassword.Text.Trim();

        private async void BtAddCustomer_OnClick(object sender, RoutedEventArgs e)
        {
            if (tbPhoneNumber.IsMaskCompleted)
            {
                var employee = await db.Employees.FirstOrDefaultAsync(a => a.PhoneNumber == PhoneNumber);
                var customer = await db.Customers.FirstOrDefaultAsync(a => a.PhoneNumber == PhoneNumber);

                if (employee == null && customer == null)
                {
                    if (Surname != "" || Name != "" || Patronimyc != "")
                    {
                        if (Regex.IsMatch(tbSurname.Text.Trim(), "^[А-Яа-яёЁ]+$")
                            || Regex.IsMatch(tbName.Text.Trim(), "^[А-Яа-яёЁ]+$")
                            || Regex.IsMatch(tbPatronimyc.Text.Trim(), "^[А-Яа-яёЁ]+$"))
                        {
                            if (Regex.IsMatch(tbPassword.Text.Trim(), "^[a-zA-Z0-9_.-]+$"))
                            {
                                if (tbSurname.Text.Trim().Length <= 30
                                    || tbSurname.Text.Trim().Length <= 30
                                    || tbName.Text.Trim().Length <= 30
                                    || tbPassword.Text.Trim().Length <= 30)
                                {
                                    db.Customers.Add(new Customer
                                    {
                                        Surname = Surname,
                                        Name = Name,
                                        Patronymic = Patronimyc,
                                        Password = Password,
                                        PhoneNumber = PhoneNumber
                                    });
                                    await db.SaveChangesAsync();
                                    DialogResult = true;
                                }
                                else
                                    MessageBox.Show("Значения в некоторых полях слишком длинные!",
                                        "Ашипка =(",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                                MessageBox.Show(
                                    "В пароле могут использоваться только английские символы, цфиры и '-_'!",
                                    "Ашипка =(",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                        }
                        else
                            MessageBox.Show("ФИО должно быть введено русскими буквами!",
                                "Ашипка =(",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show("Не все поля заполнены!",
                            "Ашипка =(",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Аккаунт с данным номером телефона уже ссуществует!", "=(",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Заполните номер телефона!", "=(",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}