using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Exercise_1
    {
        public static void Main()
        {
            int n = 7, m = 6;

            // Pattern 1
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i + 1; j++) Console.Write("*");
                Console.WriteLine();
            }
            Console.WriteLine();

            // Pattern 2
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++) Console.Write(" ");
                for (int j = 0; j < m; j++) Console.Write("*");
                Console.WriteLine();
            }
            Console.WriteLine();

            // Pattern 3
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++) Console.Write(" ");
                for (int j = 0; j < i * 2 + 1; j++) Console.Write("*");
                Console.WriteLine();
            }
            Console.WriteLine();

            // Pattern 4
            for (int i = 0; i < n; i++)
            {
                int k = n / 2 - Math.Abs(n / 2 - i);
                for (int j = 0; j < n - k - 1; j++) Console.Write(" ");
                for (int j = 0; j < k * 2 + 1; j++) Console.Write("*");
                Console.WriteLine();
            }
            Console.WriteLine();

            // Pattern 5 (Christmas Tree)
            int a = 1, b = 2;
            Random rnd = new Random(0);
            for (int i = 0; i < n + 2; i++)
            {
                int k;
                if (i < a) k = i;
                else if (i < n) k = a + (i - a) % b;
                else k = 0;
                for (int j = 0; j < n - k - 1; j++) Console.Write(" ");
                for (int j = 0; j < k * 2 + 1; j++)
                    if (rnd.NextDouble() < 0.1) Console.Write("@");
                    else if (rnd.NextDouble() < 0.1) Console.Write("#");
                    else Console.Write("*");
                Console.WriteLine();
            }
            Console.WriteLine();

        }
    }
}
