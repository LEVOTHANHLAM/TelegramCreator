namespace AppDestop.TelegramCreator.ChoThueSimCodeApi
{
    public class ChoThueSimCodeApiResponse<T> where T : class 
    {
        public  T Result { get; set; }
        public int ResponseCode { get; set; }
        public string Msg { get; set; }

    }
}
