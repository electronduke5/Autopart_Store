using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using University.Cards;
using University.Database;

namespace University.Main
{
    public partial class AdminPage : Page
    {
        Employee LoggedUser;
        private UniversityContext db;

        public AdminPage(Employee loggedUser)
        {
            InitializeComponent();
            this.LoggedUser = loggedUser;
            db = new UniversityContext();
            //Заполнение вкладки "Сотрудники"
            cbRole.DataContext = db.Roles.ToList();
            cbRole.DisplayMemberPath = "RoleName";
            cbRole.SelectedValuePath = "IdRole";
            cbRole.SelectedIndex = 0;
            SetEmployee();
            //Заполнение вкладки "Поставщики"
            SetProvider();
            //Заполнение вкладки "Товары"
            UpdateComboBoxesInAutopart();
            SetAutopart();
            //Заполнение вкладки "Доходы\Расходы"
            SetFinance();
        }

        public void UpdateComboBoxesInAutopart()
        {
            cbCategory.DataContext = db.Categories.ToList();
            cbCategory.DisplayMemberPath = "CategoryName";
            cbCategory.SelectedValuePath = "IdCategory";
            cbCategory.SelectedIndex = 0;

            cbProvider.DataContext = db.Providers.ToList();
            cbProvider.DisplayMemberPath = "ProviderName";
            cbProvider.SelectedValuePath = "IdProvider";
            cbProvider.SelectedIndex = 0;
        }
        private void SetEmployee()
        {
            StackEmployee.Children.Clear();
            foreach (var employee in db.Employees.Include(employee => employee.Role))
            {
                EmployeeCard cardEmployee = new EmployeeCard(LoggedUser)
                {
                    Employee = employee,
                    Password = employee.Password,
                    PhoneNumber = employee.PhoneNumber,
                    Role = employee.Role.RoleName,
                    FullName = $"{employee.Surname} {employee.Name} {employee.Patronymic}"
                };
                StackEmployee.Children.Add(cardEmployee);
            }
        }
        private void SetProvider()
        {
            StackProvider.Children.Clear();
            foreach (var provider in db.Providers)
            {
                ProviderCard cardProvider = new ProviderCard(LoggedUser)
                {
                    _Provider = provider,
                    ProviderName = provider.ProviderName
                };
                StackProvider.Children.Add(cardProvider);
            }
        }

        private void SetAutopart()
        {
            StackAutopart.Children.Clear();

            foreach (var autopart in db.Autoparts
                         .Include(autopart => autopart.Category)
                         .Include(autopart => autopart.Provider)
                         .ToList())
            {
                if (autopart.Count > 0)
                {
                    AutopartCard cardAutopart = new AutopartCard()
                    {
                        _Autopart = autopart,
                        _Category = autopart.Category,
                        _Provider = autopart.Provider,
                        Count = autopart.Count,
                        Price = autopart.Price,
                        AutopartName = autopart.AutopartName,
                        CategoryName = autopart.Category.CategoryName,
                        ProviderName = autopart.Provider.ProviderName
                    };
                    StackAutopart.Children.Add(cardAutopart);
                }
            }
        }
        private void SetFinance()
        {
            StackFinance.Children.Clear();
            decimal FullResult = 0;
            foreach (var accounting in db.Accountings.OrderBy(accounting => accounting.RecordDate))
            {
                var result = accounting.Profit == Decimal.Zero
                    ? accounting.Expenditure.Value
                    : accounting.Profit.Value;

                FullResult += result;

                FinanceCard financeCard = new FinanceCard(accounting.RecordDate.Value.Date.ToString("dd MMMM yyyy"), result);
                StackFinance.Children.Add(financeCard);
            }

            txtResultFinance.Text = $"Итог: {FullResult.ToString()} руб.";
        }

        private async void BtAddEmployee_OnClick(object sender, RoutedEventArgs e)
        {
            var user = await db.Employees.Where(u => u.PhoneNumber == tbPhoneNumber.Text).FirstOrDefaultAsync();
            if (user == null)
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
                        MessageBox.Show("ФИО должно быть введено русскими буквами!",
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
                            db.Employees.Add(new Employee
                            {
                                Surname = tbSurname.Text.Trim(),
                                Name = tbName.Text.Trim(),
                                Patronymic = tbPatronimyc.Text.Trim(),
                                PhoneNumber = tbPhoneNumber.Text.Trim(),
                                Password = tbPassword.Text.Trim(),
                                RoleId = (int) cbRole.SelectedValue
                            });
                            await db.SaveChangesAsync();
                            SetEmployee();
                        }
                    }
                }
            }
            else
                MessageBox.Show("Пользователь с таким номером телефона уже уществует!",
                    "Ашипка =(",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        private async void BtAddProvider_OnClick(object sender, RoutedEventArgs e)
        {
            var prov = await db.Providers.Where(p => p.ProviderName == tbProviderName.Text.Trim())
                .FirstOrDefaultAsync();
            if (prov == null)
            {
                if (tbProviderName.Text.Trim() != "")
                {
                    if (tbProviderName.Text.Trim().Length <= 30)
                    {
                        db.Providers.Add(new Provider
                        {
                            ProviderName = tbProviderName.Text.Trim()
                        });
                        await db.SaveChangesAsync();
                        //Обновление комбобоксов на странице с товарами
                        UpdateComboBoxesInAutopart();
                        //Обновление карточек поставщиков
                        SetProvider();
                    }
                    else
                        MessageBox.Show("Название поставщика должно быть менше 30 символов!", "=(",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Заполните поле названия поставщика!", "=(",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Данный поставщик уже добавлен!", "=(",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");


        private async void BtAddAutopart_OnClick(object sender, RoutedEventArgs e)
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
                        db.Autoparts.Add(new Autopart
                        {
                            Count = Int32.Parse(tbCount.Text.Trim()),
                            Price = Decimal.Parse(tbPrice.Text.Trim()),
                            AutopartName = autopartName,
                            CategoryId = (int) cbCategory.SelectedValue,
                            ProviderId = (int) cbProvider.SelectedValue
                        });
                        await db.SaveChangesAsync();
                        SetAutopart();
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

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (tiFinance.IsSelected)
                    SetFinance();
            }
        }
        private void BtnExport_OnClick(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Data");
            var accounting = db.Accountings.ToList();

            sheet.Cells["A1"].Value = $"№ п/п";
            sheet.Cells["B1"].Value = $"Дата";
            sheet.Cells["C1"].Value = $"Сумма";
            sheet.Cells["A1:C1"].Style.Font.Bold = true;
            
            decimal FullResult = 0;
            for (int i = 0; i < accounting.Count; i++)
            {
                var report = accounting[i];
                var money = report.Profit == 0 ? report.Expenditure.Value : report.Profit.Value;
                sheet.Cells[$"A{i + 2}"].Value = i + 1;
                sheet.Cells[$"B{i + 2}"].Value = report.RecordDate.Value.Date.ToString("dd MMMM yyyy");
                sheet.Cells[$"C{i + 2}"].Value = money;
                sheet.Cells[$"C{i + 2}"].Style.Font.Color.Indexed = money < 0 ? 2 : 3;
                
                FullResult += money;
            }
            sheet.Cells[$"C{accounting.Count + 2}"].Value = $"Итог на {DateTime.Today:dd.MM.yyyy}: {FullResult}";
            var range = sheet.Cells[$"A1:C{accounting.Count + 2}"];
            range.AutoFitColumns();
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            //Путь и название будущего файла
            string filePath = $"C:\\Users\\gpav5\\Downloads\\Отчёт-{DateTime.Now.ToString().Replace(":", "-")}.xlsx";
            
            FileInfo fi = new FileInfo(filePath);
            package.SaveAs(fi);
            
            MessageBox.Show("Отчёт сформирован!", "=)", MessageBoxButton.OK);
        }
    }
}