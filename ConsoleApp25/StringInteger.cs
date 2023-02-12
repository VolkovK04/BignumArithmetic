using System;
using System.Collections.Generic;
using System.Linq;

namespace BignumArithmetic
{
    internal class StringInteger
    {
        public StringInteger()
        {
            data = new List<char>();
            data.Add('0');
        }
        public StringInteger(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] < '0' || str[i] > '9') && (i != 0 || str[i] != '-'))
                    throw new Exception("Error string");
            }
            data = new List<char>(str.ToCharArray());
            if (data[0] == '-')
            {
                data.Remove('-');
                sign = true;
            }
        }

        private List<char> data;
        private bool sign = false;

        public static bool IsNull(StringInteger x)
        {
            return x.data.Count == 1 && x.data[0] == '0';
        }
        public static bool IsOdd(StringInteger x)
        {
            return (x.data.Last() - '0') % 2 == 0;
        }
        public static bool IsEven(StringInteger x)
        {
            return (x.data.Last() - '0') % 2 == 1;
        }
        public static bool IsNegative(StringInteger x)
        {
            return x.sign;
        }
        public static void Divide(StringInteger x)
        {
            int flag = 0;
            for (int i = 0; i < x.data.Count; i++)
            {
                char c = (char)(((x.data[i] - '0') / 2 + flag) + '0');
                if ((x.data[i] - '0') % 2 == 1)
                    flag = 5;
                else
                    flag = 0;
                x.data[i] = c;
            }
            if (x.data.Count > 1 && x.data[0] == '0')
                x.data.RemoveAt(0);
        }
        public static void Double(StringInteger x)
        {
            int flag = 0;
            for (int i = x.data.Count - 1; i >= 0; i--)
            {
                char c = (char)(((x.data[i] - '0') * 2 + flag) % 10 + '0');
                if (x.data[i] >= '5')
                    flag = 1;
                else
                    flag = 0;
                x.data[i] = c;
            }
            if (flag == 1)
                x.data.Insert(0, '1');
        }
        public static void Decrement(StringInteger x)
        {
            int i = x.data.Count - 1;
            while (x.data[i] == '0')
            {
                x.data[i] = '9';
                if (i == 0)
                {
                    x.data = new List<char>();
                    x.data.Add('0');
                    return;
                }
                i--;
            }
            x.data[i] = (char)(x.data[i] - 1);
        }
        public static void Increment(StringInteger x)
        {
            int i = x.data.Count - 1;
            while (x.data[i] == '9')
            {
                x.data[i] = '0';
                if (i == 0)
                {
                    x.data.Insert(0, '1');
                    return;
                }
                i--;
            }
            x.data[i] = (char)(x.data[i] + 1);
        }
        public static void Negative(StringInteger x)
        {
            x.sign = !x.sign;
        }
        public override string ToString()
        {
            string result = new string(data.ToArray());
            if (sign)
                result = "-" + result;
            return result;
        }

    }
}
