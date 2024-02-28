using Newtonsoft.Json;

namespace AppDestop.TelegramCreator.ViotpApi
{
    public class ViotpHttpHelper
    {
        public static async Task<ViotpApiResponse<ViotpResult>> BuyPhoneNumber(string key)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ViotpConstant.ViotpApiUrl);
            string appId = ViotpConstant.AppTele;
            string query = "request/getv2?token=" + key + "&serviceId=" + appId;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ViotpApiResponse<ViotpResult> data = JsonConvert.DeserializeObject<ViotpApiResponse<ViotpResult>>(body);
            return data;

        }
        public static async Task<ViotpApiResponse<ViotpOtpResult>> GetOtp(string key, string id)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ViotpConstant.ViotpApiUrl);
            string query = "session/getv2?requestId=" + id + "&token=" + key;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ViotpApiResponse<ViotpOtpResult> data = JsonConvert.DeserializeObject<ViotpApiResponse<ViotpOtpResult>>(body);
            return data;

        }
    }
}
