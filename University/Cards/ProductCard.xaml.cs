using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using University.Database;

namespace University.Cards
{
    public partial class ProductCard : UserControl
    {
        private UniversityContext db;
        private Customer _LoggedCustomer;

        public ProductCard(Customer loggedCustomer)
        {
            InitializeComponent();
            DataContext = this;
            _LoggedCustomer = loggedCustomer;
            db = new UniversityContext();
        }

        public Autopart Autopart { get; set; }
        public string AutopartName { get; set; }
        public string ProviderName { get; set; }
        public int MaxCount { get; set; }
        public decimal Price { get; set; }

        public string MaxCountString
        {
            get => $"В наличии - {MaxCount} шт.";
        }

        public string PriceRUB
        {
            get => $"{Price} ₽";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");

        private async void BtnAddToCart_OnClick(object sender, RoutedEventArgs e)
        {
            
            int SelectCount = Int32.Parse(tbSelectCount.Text.Trim());
            
            Autopart _Autopart = await db.Autoparts.FirstOrDefaultAsync
            (
                a => a.IdAutopart == Autopart.IdAutopart
            );

            var orderEquals =
                await db.Orders.FirstOrDefaultAsync(order => order.CustomerId == _LoggedCustomer.IdCustomer);
            if (orderEquals == null)
            {
                db.Orders.Add(new Order()
                {
                    CustomerId = _LoggedCustomer.IdCustomer,
                });
                await db.SaveChangesAsync();
            }


            //Проверяем, есть ли в таблице с комбинацией заказа и запчасти (корзина)
            //запись с таким же ID запчасти. 
            //И если есть, то мы ее перезапиысываем, если нет - то создаем новую запись.

            //Все заказы
            var order = await db.Orders.FirstOrDefaultAsync(order => order.CustomerId == _LoggedCustomer.IdCustomer);
            //Все заказы с запчастями
            var _combinationOrder = db.CombinationOrders
                .Include(order => order.Autopart)
                .Include(order => order.Orders)
                .ToList();
            
            var combOrders = db.CombinationOrders.Where(c => c.OrdersId == order.IdOrders );


            var combPart = await db.CombinationOrders.FirstOrDefaultAsync
            (
                a => a.AutopartId == _Autopart.IdAutopart
                && a.OrdersId == order.IdOrders
            );

            
            if (combPart == null)
            {
                db.CombinationOrders.Add(new CombinationOrder
                {
                    OrdersId = order.IdOrders,
                    AutopartId = _Autopart.IdAutopart,
                    Count = SelectCount,
                    
                });
                //_Autopart.Count -= SelectCount;
                MaxCount = _Autopart.Count - SelectCount;
                tbSelectCount.Maximum = MaxCount;
                await db.SaveChangesAsync();
                MessageBox.Show("Товар добавлен в корзину!", "=)",MessageBoxButton.OK);
            }
            else
            {
                if (SelectCount + combPart.Count <= combPart.Autopart.Count)
                {
                    combPart.Count += SelectCount;
                    await db.SaveChangesAsync();
                    MaxCount = _Autopart.Count - combPart.Count;
                    tbSelectCount.Maximum = MaxCount;
                    MessageBox.Show("Товар добавлен в корзину!", "=)", MessageBoxButton.OK);
                }
                else
                    MessageBox.Show(
                        $"У вас в корзине уже есть {combPart.Count} шт. И в сумме количесвто товаров не может превышать {combPart.Autopart.Count}",
                        "=(", MessageBoxButton.OK, MessageBoxImage.Warning);

                //1 вариант
                //_Autopart.Count -= SelectCount;
                //2 вариант
                //Получить из бд Autopart и изменить там
            }
        }
    }
}