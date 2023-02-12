using System;
using System.IO;

namespace BignumArithmetic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool flag = true;
            for (int i=1; i<=5; i++)
            {
                StreamReader input = new StreamReader($"../../Tests/input{i}.txt");
                BigInteger a = BigInteger.FromString(input.ReadLine());
                BigInteger b = BigInteger.FromString(input.ReadLine());
                StreamReader output = new StreamReader($"../../Tests/output{i}.txt");
                BigInteger c = BigInteger.FromString(output.ReadLine());
                if (c == a * b)
                    Console.WriteLine($"Test №{i} passed");
                else
                {
                    Console.WriteLine($"Test №{i} failed");
                    flag = false;
                }
            }
            if (flag)
                Console.WriteLine("All tests passed");
            Console.ReadLine();

        }

    }
}
