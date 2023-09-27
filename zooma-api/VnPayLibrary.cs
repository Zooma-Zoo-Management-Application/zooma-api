using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace zooma_api
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!_requestData.ContainsKey(key))
            {
                _requestData.Add(key, value);
            }
            else
            {
                _requestData[key] = value;
            }
        }

        public string CreateRequestUrl(string paymentUrl, string secretKey)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + HttpUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString().Remove(data.Length - 1, 1);
            string rawData = queryString + secretKey;

            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            string secureHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            return paymentUrl + "?" + queryString + "&vnp_SecureHashType=SHA256&vnp_SecureHash=" + secureHash;
        }
    }
}
