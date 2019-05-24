using System;
using System.Collections.Generic;

namespace Tolldo.Data
{
    /// <summary>
    /// A static class that generates random names.
    /// </summary>
    public static class RandomNameGenerator
    {
        /// <summary>
        /// Generate random To-do name
        /// </summary>
        /// <returns></returns>
        public static string GetRandomTodoName()
        {
            List<string> names = new List<string>()
            {
                "Things to do",
                "Awesome recipie",
                "Time to be productive",
                "Bucketlist",
                "Procrastination"
            };

            Random random = new Random();
            int index = random.Next(0, names.Count);

            return names[index];
        }
    }
}
