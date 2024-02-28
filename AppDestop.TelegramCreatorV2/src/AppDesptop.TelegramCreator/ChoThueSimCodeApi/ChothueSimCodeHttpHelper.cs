using Newtonsoft.Json;

namespace AppDestop.TelegramCreator.ChoThueSimCodeApi
{
    public  class ChothueSimCodeHttpHelper
    {
        public static async Task<ChoThueSimCodeApiResponse<ChoThueSimCodeResult>> BuyPhoneNumber(string key)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ChoThueSimCodeConstant.ChothuesimcodeApiUrl);
            string appId = ChoThueSimCodeConstant.AppTele;
            string query = "api?act=number&apik=" + key + "&appId=" + appId;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ChoThueSimCodeApiResponse<ChoThueSimCodeResult> data = JsonConvert.DeserializeObject<ChoThueSimCodeApiResponse<ChoThueSimCodeResult>>(body);
            return data;

        }
        public static async Task<ChoThueSimCodeApiResponse<ChoThueSimCodeOtpResult>>GetOtp(string key, string id)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ChoThueSimCodeConstant.ChothuesimcodeApiUrl);
            string query = "api?act=code&apik=" + key + "&id=" + id;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ChoThueSimCodeApiResponse<ChoThueSimCodeOtpResult> data = JsonConvert.DeserializeObject<ChoThueSimCodeApiResponse<ChoThueSimCodeOtpResult>>(body);
            return data;

        }
    }
}
