namespace RedisSample.Models.Entities
{
    public class Cart
    {
        public Cart(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; set; }
        public ICollection<CartItems> Items { get; set; } = new List<CartItems>();

        public decimal TotalPrice
        {
            get
            {
                return Items.Sum(s => s.Price * s.Quantity);
            }
        }
    }
}
