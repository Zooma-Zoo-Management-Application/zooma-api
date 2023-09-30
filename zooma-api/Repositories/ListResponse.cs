using VNPayDemo.DTO;

namespace VNPayDemo
{
    public class ListResponse
    {
        private static ListResponse instance;
        public List<PaymentReturnDtos> list; 

        public static ListResponse Instance
        {
            get
            {
                if (instance == null)
                    instance = new ListResponse();
                return instance;
            }
        }

        private ListResponse()
        {
            list = new List<PaymentReturnDtos>();
        }

        public void AddToCart(PaymentReturnDtos payment)
        {

            list.Add(payment);
            
        }

        public List<PaymentReturnDtos> GetLists()
        {
            return this.list;
        }

    }
}
