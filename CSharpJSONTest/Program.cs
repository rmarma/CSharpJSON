﻿// R.M.A., 2018

using System;
using System.Globalization;
using CSharpJSON;

namespace CSharpJSONTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            Test2();
            Test3();
            Test4();

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

        private static void Test3()
        {
            string jsonString = "[{\"int\":1,\"double\":1.1,\"bool\":true,\"string\":\"string\"},{\"int\":2,\"double\":2.2,\"bool\":false,\"string\":\"string\"}]";
            JSONArray jsonArray = new JSONArray(jsonString);
            Console.WriteLine("=== TEST ===");
            Console.WriteLine(jsonString);
            Console.WriteLine(jsonArray.ToString());
            jsonArray.Remove(0);
            Console.WriteLine(jsonArray.ToString());
            Console.WriteLine("=== END ===");
        }

        private static void Test4()
        {
            double en = 1.1;
            double ru = 1.2;
            var enUS = CultureInfo.CreateSpecificCulture("en-US");
            var ruRU = CultureInfo.CreateSpecificCulture("ru-RU");

            JSONObject json = new JSONObject();
            json.Put("en", en.ToString(enUS));
            json.Put("ru", ru.ToString(ruRU));
            Console.WriteLine("=== TEST ===");
            Console.WriteLine(json.ToString());
            Console.WriteLine(json.OptDouble("en"));
            Console.WriteLine(json.OptDouble("ru", double.NaN, NumberStyles.Any, ruRU));
            Console.WriteLine("=== END ===");
        }
    }
}
