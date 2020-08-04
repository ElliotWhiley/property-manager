namespace PropertyManager.Data {
    public static class NumberExtensions {
        public static string PadToTwoDigits(this int number)
        {
            if (number >= 10) {
                return number.ToString();
            }
            return $"0{number}";
        }
    }
}
