using AppDestop.TelegramCreator.FiveSimApi;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AppDesptop.TelegramCreator.GetSmsCode
{
    public class GetSmsCodeHttpHelper
    {
        public static async Task<GetSmsCodeResult> BuyPhoneNumber(string key, string countryId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(GetSmsCodeConstant.GetSmsCodeURL);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string query = $"api/order-number?service=tg&country={countryId}";
            var response = await httpClient.GetAsync(query);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = await response.Content.ReadAsStringAsync();
                GetSmsCodeResult data = JsonConvert.DeserializeObject<GetSmsCodeResult>(body);
                return data;
            }
            return null;
        }
        public static async Task<GetSmsCodeResult> GetOtp(string key, string id)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(GetSmsCodeConstant.GetSmsCodeURL);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string query = $"api/get-sms-code?activationId={id}";
            var response = await httpClient.GetAsync(query);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = await response.Content.ReadAsStringAsync();
                GetSmsCodeResult data = JsonConvert.DeserializeObject<GetSmsCodeResult>(body);
                return data;
            }
            return null;

        }
    }
}
