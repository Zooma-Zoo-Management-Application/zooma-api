using zooma_api.DTO;
using zooma_api.Models;

namespace VNPayDemo
{
    public class ListCart
    {
        private static ListCart instance;
        private List<CartItemDTO> list; 

        public static ListCart Instance
        {
            get
            {
                if (instance == null)
                    instance = new ListCart();
                return instance;
            }
        }

        private ListCart()
        {
            list = new List<CartItemDTO>();
        }

        public void AddToCart(CartItemDTO product)
        {

            var existingCartItem = list.FirstOrDefault(c =>
                c.Id == product.Id);

            if (existingCartItem != null) {
                existingCartItem.quantity += product.quantity;
            }
            else
            {
                list.Add(product);

            }


        }

        public List<CartItemDTO> GetLists()
        {
            return this.list;
        }

        public void ClearCart() {
            list.Clear();
        
        }


    }
}
