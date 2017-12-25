using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEU_Class2017
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================");
                Console.WriteLine("Choose the Program");
                Console.WriteLine("--------------------");
                Console.WriteLine("1 - Exercise 1");
                Console.WriteLine("2 - Exercise 2");
                Console.WriteLine("3 - Exercise 3");
                Console.WriteLine("4 - Exercise 4");
                Console.WriteLine("====================");
                Console.Write("Your Choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine();
                        Exercise_1.Main();
                        break;
                    case "2":
                        Console.WriteLine();
                        Exercise_2.Main();
                        break;
                    case "3":
                        Console.WriteLine();
                        Exercise_3.Main();
                        break;
                    case "4":
                        Console.WriteLine();
                        Exercise_4.Main();
                        break;
                    default:
                        Console.WriteLine("Invalid Input!");
                        break;
                }
                Console.Write("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
