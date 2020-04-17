using System.Collections.Generic;

namespace WebApplication.Utilities
{
    public class Collections
    {
        /// <summary>
        /// Merge dictionaries into a single one.
        /// 
        /// NOTE: in case of several key-value pairs - the last value will "survive".
        /// </summary>
        public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var output = new Dictionary<TKey, TValue>();

            foreach (var dict in dictionaries)
            {
                foreach (var (key, value) in dict)
                {
                    // to avoid exception on collision
                    output[key] = value;
                }
            }

            return output;
        }
    }
}
