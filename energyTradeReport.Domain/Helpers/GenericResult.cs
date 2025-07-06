namespace energyTradeReport.Domain.Helpers
{
    public class GenericResult<T>
    {
        public bool Sucess { get; set; }
        public string Message { get; set; }
        public T Object { get; set; }

        public GenericResult(bool sucess, string message, T data)
        {
            Sucess = sucess;
            Message = message;
            Object = data;                
        }
    }
}
