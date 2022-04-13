using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using University.Database;
using University.Main;

namespace University.Cards
{
    public partial class EmployeeCard : UserControl
    {
        private UniversityContext db;

        private Employee LoggedUser;

        public EmployeeCard(Employee loggedUser)
        {
            InitializeComponent();
            DataContext = this;
            db = new UniversityContext();
            LoggedUser = loggedUser;
        }

        public Employee Employee { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            System.Diagnostics.Debug.WriteLine($"Card is flipped = {e.NewValue}");

            cbRole.DataContext = db.Roles.ToList();
            cbRole.DisplayMemberPath = "RoleName";
            cbRole.SelectedValuePath = "IdRole";
            cbRole.SelectedIndex = 0;

            string[] words = FullName.Split(new char[] {' '});
            tbSurname.Text = words[0];
            tbName.Text = words[1];
            tbPatronimyc.Text = words[2];
            tbPassword.Text = Password;
        }


        private async void btnDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (Employee.IdEmployee != LoggedUser.IdEmployee)
            {
                Employee deleteEmployee = await db.Employees.FirstOrDefaultAsync( a => a.IdEmployee == Employee.IdEmployee);
                db.Employees.Remove(deleteEmployee);
                await db.SaveChangesAsync();

                Button btn = sender as Button;
                var conditionUserControl = FindParent<EmployeeCard>(btn);
                if (conditionUserControl != null)
                {
                    var sp = FindParent<StackPanel>(conditionUserControl);
                    if (sp != null)
                        sp.Children.Remove(conditionUserControl);
                }

                MessageBox.Show("Сотрудник успешно удалён!", "=)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else MessageBox.Show("Себя удалить нельзя!", "Ну ты че??", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        private async void BtnEditEmployee_OnClick(object sender, RoutedEventArgs e)
        {
            if (tbSurname.Text == ""
                || tbSurname.Text == ""
                || tbName.Text == ""
                || tbPassword.Text == "")
            {
                MessageBox.Show("Не все поля заполнены!",
                    "Ашипка =(",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                if (!Regex.IsMatch(tbSurname.Text.Trim(), "^[А-Яа-яёЁ]+$")
                    || !Regex.IsMatch(tbName.Text.Trim(), "^[А-Яа-яёЁ]+$")
                    || !Regex.IsMatch(tbPatronimyc.Text.Trim(), "^[А-Яа-яёЁ]+$"))
                    MessageBox.Show("ФИО должна быть введена русскими буквами!",
                        "Ашипка =(",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (!Regex.IsMatch(tbPassword.Text.Trim(), "^[a-zA-Z0-9_.-]+$"))
                    MessageBox.Show("В пароле могут использоваться только английские символы, цфиры и '-_'!",
                        "Ашипка =(",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    if (tbSurname.Text.Trim().Length > 30
                        || tbSurname.Text.Trim().Length > 30
                        || tbName.Text.Trim().Length > 30
                        || tbPassword.Text.Trim().Length > 30)
                        MessageBox.Show("Значения в некоторых полях слишком длинные!",
                            "Ашипка =(",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        try
                        {
                            Employee newEmployee = new Employee
                            {
                                IdEmployee = Employee.IdEmployee,
                                Surname = tbSurname.Text.Trim(),
                                Name = tbName.Text.Trim(),
                                Patronymic = tbPatronimyc.Text.Trim(),
                                PhoneNumber = Employee.PhoneNumber,
                                Password = tbPassword.Text.Trim(),
                                RoleId = (int) cbRole.SelectedValue
                            };
                            Employee = newEmployee;
                            FullName = $"{Employee.Surname} {Employee.Name} {Employee.Patronymic}";
                            PhoneNumber = Employee.PhoneNumber;
                            Password = Employee.Password;
                            Role = cbRole.SelectedItem.ToString();
                            txtFullName.Text = FullName;
                            txtPassword.Text = Password;
                            Employee.Role = (Role) cbRole.SelectedItem;
                            txtRole.Text = Employee.Role.RoleName;

                            var empl = db.Employees.First(g => g.IdEmployee == Employee.IdEmployee);
                            db.Entry(empl).CurrentValues.SetValues(Employee);
                            
                            //db.Employees.Update(newEmployee);
                            await db.SaveChangesAsync();
                        }
                        catch
                        {
                            MessageBox.Show("Данные уже обновлены!", "=(", 
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                        
                    }
                }
            }
        }
    }
}