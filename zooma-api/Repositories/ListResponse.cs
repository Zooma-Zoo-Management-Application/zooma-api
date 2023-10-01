using zooma_api.DTO;

namespace VNPayDemo
{
    public class ListResponse
    {
        private static ListResponse instance;
        public List<PaymentResponseModel> list; 

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
            list = new List<PaymentResponseModel>();
        }

        public void AddToCart(PaymentResponseModel payment)
        {

            list.Add(payment);
            
        }

        public List<PaymentResponseModel> GetLists()
        {
            return this.list;
        }

    }
}
