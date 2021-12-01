namespace Bank.Domain
{
    public class Result
    {
        private Result(bool isSuccess, string message = "")
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result Success()
        {
            return new Result(true);
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }
    }
}