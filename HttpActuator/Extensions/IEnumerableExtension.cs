using System.Collections.Generic;

namespace Com.RFranco.HttpActuator
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// List To CSV format
        /// </summary>
        /// <param name="items">List of items</param>
        /// <returns>Items in CSV format</returns>
        public static string ToCsv(this IEnumerable<string> items)
        {
            return string.Join(",", items);            
        }
    }
}

