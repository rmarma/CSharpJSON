// R.M.A., 2018

using System;
using CSharpJSON;

namespace CSharpJSONTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();

            Console.ReadKey();
        }


        private static void Test1()
        {
            string jsonString = "{}";
            JSONObject jsonObject = new JSONObject(jsonString);
            Console.WriteLine("=== TEST ===");
            Console.WriteLine(jsonString);
            Console.WriteLine(jsonObject.ToString());
            Console.WriteLine("=== END ===");
        }

        private static void Test2()
        {
            string jsonString = "{\"int\":1,\"double\":1.1,\"bool\":true,\"string\":\"string\"}";
            JSONObject jsonObject = new JSONObject(jsonString);
            Console.WriteLine("=== TEST ===");
            Console.WriteLine(jsonString);
            Console.WriteLine(jsonObject.ToString());
            Console.WriteLine("int: " + jsonObject.OptInt("int", 0));
            Console.WriteLine("double: " + jsonObject.OptDouble("double", 0.0));
            Console.WriteLine("bool: " + jsonObject.OptBoolean("bool", false));
            Console.WriteLine("string: " + jsonObject.OptString("string", ""));
            Console.WriteLine("=== END ===");
        }
    }
}
