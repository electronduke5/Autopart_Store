using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using University.Cards;
using University.Database;
using University.Helper;

namespace University.Main
{
    public partial class CustomerPage : Page
    {
        private UniversityContext db;
        private Customer _LoggedCustomer;

        public CustomerPage(Customer loggedCustomer)
        {
            InitializeComponent();
            _LoggedCustomer = loggedCustomer;
            db = new UniversityContext();
            DataContext = this;
            tbFullName.Text = $"{_LoggedCustomer.Surname} {_LoggedCustomer.Name} {_LoggedCustomer.Patronymic}";
            tbPhoneNumber.Text = $"{_LoggedCustomer.PhoneNumber}";

            SetProduct();
            SetProductInCart();
        }

        private void SetProduct()
        {
            StackTO.Children.Clear();
            StackDisc.Children.Clear();
            StackTire.Children.Clear();
            StackBattery.Children.Clear();
            StackOil.Children.Clear();
            StackChemistry.Children.Clear();
            StackAccessories.Children.Clear();
            StackInstruments.Children.Clear();
            StackElectronics.Children.Clear();
            StackBrushes.Children.Clear();


            foreach (var autopart in db.Autoparts
                         .Include(a => a.Category)
                         .Include(a => a.Provider).ToList())
            {
                if (autopart.Count > 0)
                {
                    ProductCard cardProduct = new ProductCard(_LoggedCustomer)
                    {
                        Autopart = autopart,
                        AutopartName = autopart.AutopartName,
                        ProviderName = autopart.Provider.ProviderName,
                        MaxCount = autopart.Count,
                        Price = autopart.Price
                    };
                    switch (autopart.CategoryId)
                    {
                        case 1:
                            StackTO.Children.Add(cardProduct);
                            break;
                        case 2:
                            StackTire.Children.Add(cardProduct);
                            break;
                        case 3:
                            StackDisc.Children.Add(cardProduct);
                            break;
                        case 4:
                            StackBattery.Children.Add(cardProduct);
                            break;
                        case 5:
                            StackOil.Children.Add(cardProduct);
                            break;
                        case 6:
                            StackChemistry.Children.Add(cardProduct);
                            break;
                        case 7:
                            StackAccessories.Children.Add(cardProduct);
                            break;
                        case 8:
                            StackInstruments.Children.Add(cardProduct);
                            break;
                        case 9:
                            StackElectronics.Children.Add(cardProduct);
                            break;
                        case 10:
                            StackBrushes.Children.Add(cardProduct);
                            break;
                    }
                }
            }
        }

        private async void SetProductInCart()
        {
            StackCart.Children.Clear();
            //Заказ пользователя, который зашел в систему
            var order = await db.Orders.FirstOrDefaultAsync(order => order.CustomerId == _LoggedCustomer.IdCustomer);
            //Зааказ вместе с запчастями
            var combOrders = db.CombinationOrders.Where(c => c.OrdersId == order.IdOrders);

            foreach (var combinOrder in combOrders
                         .Include(comb => comb.Autopart)
                         .Include(comb => comb.Orders)
                         .ThenInclude(order1 => order1.Customer).ToList())
            {
                CartItem itemInCart = new CartItem()
                {
                    AutopartName = combinOrder.Autopart.AutopartName,
                    MaxCount = combinOrder.Autopart.Count,
                    SelectedCount = combinOrder.Count,
                    Price = combinOrder.Autopart.Price,
                    FullPrice = combinOrder.Count * combinOrder.Autopart.Price,
                    _CombinationOrder = combinOrder
                };
                StackCart.Children.Add(itemInCart);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (tiCatalog.IsSelected)
                    SetProduct();
                else if (tiCart.IsSelected)
                    SetProductInCart();    
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var order = await db.Orders.FirstOrDefaultAsync(order =>
                    order.CustomerId == _LoggedCustomer.IdCustomer);
                if (order == null)
                    MessageBox.Show("Вы еще не добавили ни одного товара в корзину!", "=)",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    //Вот здесь при изменении числа кнопками в корзине не меняется значение в таблице
                    var combOrders = db.CombinationOrders.Where(c => c.OrdersId == order.IdOrders);
                    if (!combOrders.Any())
                    {
                        MessageBox.Show("Вы еще не добавили ни одного товара в корзину!", "=)",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        decimal FullPrice = 0;
                        foreach (var combinOrder in combOrders
                                     .Include(comb => comb.Autopart)
                                     .Include(comb => comb.Orders)
                                     .ThenInclude(order1 => order1.Customer).ToList())
                        {
                            //Удаление из таблицы Autopats количества запчастей, которое указано в табл. Combinatioin_Order
                            Autopart autopart =
                                await db.Autoparts.FirstOrDefaultAsync(a => a.IdAutopart == combinOrder.AutopartId);
                            //Вычитаем купленное количество запчастей
                            autopart.Count -= combinOrder.Count;
                            if (autopart.Count < 0) autopart.Count = 0;

                            //Добавляем цену товара в переменную
                            FullPrice += autopart.Price * combinOrder.Count;


                            //Удаление из таблицы Combinatioin_Order всех товаров
                            db.CombinationOrders.Remove(combinOrder);

                            //Убираем элемнт товара с экрана
                            StackCart.Children.Clear();
                            
                            order.OrdersDate = DateTime.Today;
                            await db.SaveChangesAsync();
                        }

                        //Добавление в таблицу Accounting записи с профитом
                        db.Accountings.Add(new Accounting()
                        {
                            Profit = FullPrice
                        });
                        await db.SaveChangesAsync();
                        MessageBox.Show("Заказ оплачен, ожидайте СМС на телефон с информацией о заказе.", "=)",
                            MessageBoxButton.OK);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Технические шоколадки...", "=/", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }
    }
}