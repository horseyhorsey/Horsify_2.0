namespace Horsesoft.Music.Engine
{
    public static class StringExtensions
    {
        /// <summary>
        /// Cleans the string from unwanted chars.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string CleanString(this string input)
        {
            return input.Replace(":", string.Empty)
                .Replace("?", string.Empty)
                .Replace("ÿ", string.Empty)                                
                .Replace("�", string.Empty)
                .Replace("\u0000", string.Empty)
                .Replace("\u0001", string.Empty)
                .Trim();
        }
    }
}