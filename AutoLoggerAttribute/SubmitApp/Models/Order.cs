namespace SubmitApp.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public Product Product { get; set; }
    }
}
