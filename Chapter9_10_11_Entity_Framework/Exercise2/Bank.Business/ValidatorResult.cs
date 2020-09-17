namespace Bank.Business
{
    public class ValidatorResult
    {
        private ValidatorResult(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; set; }
        public string Message { get; set; }

        public static ValidatorResult Success()
        {
            return new ValidatorResult(true);
        }

        public static ValidatorResult Fail(string message)
        {
            return new ValidatorResult(false, message);
        }
    }
}