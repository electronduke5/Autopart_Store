using University.Main;

namespace University.Helper
{
    public class CartViewModel
    {
        public string AutopartName { get; set; }
        public int SelectedCount { get; set; }
        public decimal Price { get; set; }
        public  int MaxCount { get; set; }
        public CustomerPage Page { get; set; }

        public CartViewModel(string autopartName, int selectedCount, decimal price, int maxCount,CustomerPage page)
        {
            AutopartName = autopartName;
            SelectedCount = selectedCount;
            Price = price;
            MaxCount = maxCount;
            Page = page;
            

        }

        public CartViewModel()
        {
           
        }
    }
}