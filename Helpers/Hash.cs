
using Microsoft.Extensions.Logging;

namespace MVC_App.Helpers
{
    public static class Hash
    {
        private static readonly string LETTERS = "acdegiklmnoprsuw";
        private static readonly int HASH_START = 7;
        private static readonly int HASH_FACTOR = 37;


        public static string Unhash(long hash, ILogger _logger)
        {
            // hash is of the form h = (h * 37 + a) for some value 0 <= a <= 15
            // for a hash of a three letter string:
            // a3 + 37(37(37*7 + a1) + a2)
            // so we subtract our letter index and then divide by 37 for the next letter

            string result = "";

            while (hash > HASH_START)
            {
                int remainder = (int)(hash % HASH_FACTOR);

                if (remainder >= LETTERS.Length)
                {
                    _logger.LogError("Error: Hash remainder out of array range.");
                    return "Invalid Hash";
                }

                result = LETTERS[remainder] + result;
                hash -= remainder;
                hash /= HASH_FACTOR;

            }

            if (hash != HASH_START)
            {
                _logger.LogError("Error: Hash base value is incorrect.");
                return "Invalid Hash";
            } 

            return result;
        } // end method Unhash
    } // end class Hash
}