using System;

namespace PropertyManager.Data {
    public static class StringExtensions {
        public static string Base64Encode(this string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
