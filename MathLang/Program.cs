using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLang
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.Write("Enter your problem: ");
            string problem = Console.ReadLine();
            Console.WriteLine(Expression.Evaluate(problem).ToString());

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}