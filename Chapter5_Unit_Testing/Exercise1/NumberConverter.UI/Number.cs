namespace NumberConverter.UI
{
    public class Number
    {
        public Number(int value)
        {
            NumberAsText = value.ToString();
        }
        public string NumberAsText { get; set; }
    }
}