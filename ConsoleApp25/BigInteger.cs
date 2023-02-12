using System;
using System.Collections.Generic;
using System.Linq;

namespace BignumArithmetic
{
    internal class BigInteger
    {
        private BigInteger(List<byte> bytes, bool sign = false)
        {
            this.sign = sign;
            data = bytes;
        }
        public BigInteger()
        {
            data = new List<byte>();
        }
        public BigInteger(Int64 x)
        {
            if (x < 0)
            {
                sign = true;
                x *= -1;
            }
            data = new List<byte>(BitConverter.GetBytes(x));
            DeleteZeroBytes();
        }
        public BigInteger(Int32 x)
        {
            if (x < 0)
            {
                sign = true;
                x *= -1;
            }
            data = new List<byte>(BitConverter.GetBytes(x));
            DeleteZeroBytes();
        }


        private List<byte> data;
        private bool sign = false;
        public void AddZeroBytes(int k)
        {
            if (data.Count > k)
                throw new Exception("Error zero bytes count");
            for (int i = data.Count; i < k; i++)
                data.Add(0);
        }
        public void DeleteZeroBytes()
        {
            while (data.Count > 0 && data.Last() == 0)
                data.RemoveAt(data.Count - 1);
        }
        public static bool IsNull(BigInteger x)
        {
            return x.data.Count == 0;
        }
        public static bool IsNegative(BigInteger x)
        {
            return x.sign && !IsNull(x);
        }
        public static bool IsPositive(BigInteger x)
        {
            return !x.sign && !IsNull(x);
        }
        public static BigInteger FromString(string str)
        {
            List<byte> data = new List<byte>();
            StringInteger x = new StringInteger(str);
            bool sign = StringInteger.IsNegative(x);
            while (!StringInteger.IsNull(x))
            {
                byte c = 0;
                for (int i = 0; i < sizeof(byte) * 8; i++)
                {
                    if (StringInteger.IsEven(x))
                        c += (byte)(1 << i);
                    StringInteger.Divide(x);
                }
                data.Add(c);
            }
            return new BigInteger(data, sign);
        }
        public BigInteger Abs()
        {
            return new BigInteger(data);
        }
        public override string ToString()
        {
            StringInteger result = new StringInteger();
            if (sign)
                StringInteger.Negative(result);
            for (int i = data.Count - 1; i >= 0; i--)
            {
                byte c = data[i];
                int[] d = new int[sizeof(byte) * 8];
                for (int j = sizeof(byte) * 8 - 1; j > 0; j--)
                {
                    d[j] = c % 2;
                    c >>= 1;
                }
                d[0] = c;
                for (int j = 0; j < sizeof(byte) * 8; j++)
                {
#if DEBUG
                    Console.Write(d[j]);
#endif
                    StringInteger.Double(result);
                    if (d[j] == 1)
                        StringInteger.Increment(result);
                }
#if DEBUG
                Console.WriteLine();
#endif
            }

            return result.ToString();
        }
        public string ToString(string format)
        {
            if (format == "X" || format == "x")
            {
                string result = "0x";
                for (int i = data.Count - 1; i >= 0; i--)
                    result += data[i].ToString(format + "2");
                return result;
            }
            else
                throw new Exception("Format error");
        }
        public override int GetHashCode()
        {
            int result = 0;
            for (int i = 0; i < data.Count; i++)
                result ^= data[i].GetHashCode();
            return result;
        }
        public override bool Equals(object obj)
        {
            BigInteger other = obj as BigInteger;
            if (!(sign.Equals(other.sign)))
                return false;
            if (!(data.Count.Equals(other.data.Count)))
                return false;
            for (int i = 0; i < data.Count; i++)
                if (!data[i].Equals(other.data[i]))
                    return false;
            return true;
        }
        public static bool operator ==(BigInteger a, BigInteger b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(BigInteger a, BigInteger b)
        {
            return !a.Equals(b);
        }
        public static bool operator <(BigInteger a, BigInteger b)
        {
            if (IsNegative(a) && !IsNegative(b))
                return true;
            else if (!IsNegative(a) && IsNegative(b))
                return false;
            bool flag = IsNegative(a) && IsNegative(b);
            a.DeleteZeroBytes();
            b.DeleteZeroBytes();
            if (a.data.Count != b.data.Count)
                return (a.data.Count < b.data.Count) ^ flag;
            for (int i = a.data.Count - 1; i >= 0; i--)
                if (a.data[i] != b.data[i])
                    return (a.data[i] < b.data[i]) ^ flag;
            return false;
        }
        public static bool operator >(BigInteger a, BigInteger b)
        {
            if (IsNegative(a) && !IsNegative(b))
                return false;
            else if (!IsNegative(a) && IsNegative(b))
                return true;
            bool flag = IsNegative(a) && IsNegative(b);
            a.DeleteZeroBytes();
            b.DeleteZeroBytes();
            if (a.data.Count != b.data.Count)
                return (a.data.Count > b.data.Count) ^ flag;
            for (int i = a.data.Count - 1; i >= 0; i--)
                if (a.data[i] != b.data[i])
                    return (a.data[i] > b.data[i]) ^ flag;
            return false;
        }
        public static bool operator <=(BigInteger a, BigInteger b)
        {
            return !(a > b);
        }
        public static bool operator >=(BigInteger a, BigInteger b)
        {
            return !(a < b);
        }
        public static BigInteger operator <<(BigInteger a, int k)
        {
            BigInteger result = new BigInteger(new List<byte>(a.data));
            result.sign = a.sign;
            byte r = (byte)(k % 8);
            byte buff = 0;
            for (int i = 0; i < a.data.Count; i++)
            {
                result.data[i] = (byte)((a.data[i] << r) + buff);
                buff = (byte)(a.data[i] >> (8 - r));
            }
            if (buff != 0)
                result.data.Add(buff);
            for (int i = 0; i < k / 8; i++)
                result.data.Insert(0, 0);
            return result;
        }
        public static BigInteger operator -(BigInteger a)
        {
            return new BigInteger(a.data, !a.sign);
        }
        public static BigInteger operator +(BigInteger a, BigInteger b)
        {
            if (IsNegative(a))
                if (IsNegative(b))
                    return -(-a + -b);
                else
                    return b - (-a);
            if (IsNegative(b))
                return a - (-b);
                BigInteger result = new BigInteger();
            if (b.data.Count > a.data.Count)
                (a, b) = (b, a);
            byte flag = 0;
            for (int i = 0; i < a.data.Count; i++)
            {
                byte buff;
                byte u = 0;
                if (i < b.data.Count)
                    u = b.data[i];
                buff = (byte)((a.data[i] + u + flag) % 256);
                result.data.Add(buff);
                if (buff < a.data[i])
                    flag = 1;
                else
                    flag = 0;
            }
            if (flag != 0)
                result.data.Add(flag);
            return result;
        }
        public static BigInteger operator -(BigInteger a, BigInteger b)
        {
            if (IsNegative(a))
                if (IsNegative(b))
                    return -(-a - -b);
                else
                    return -(-a + b);
            if (IsNegative(b))
                return a + -b;
            if (a < b)
                return -(b - a);
            BigInteger result = new BigInteger();
            byte flag = 0;
            for (int i = 0; i < a.data.Count; i++)
            {
                byte buff;
                byte u = 0;
                if (i < b.data.Count)
                    u = b.data[i];
                buff = (byte)((a.data[i] - u - flag) % 256);
                result.data.Add(buff);
                if (buff > a.data[i])
                    flag = 1;
                else
                    flag = 0;
            }
            return result;
        }
        public static BigInteger operator *(BigInteger a, BigInteger b)
        {
            BigInteger result = KaratsubaMultiply(a, b);
            result.sign = a.sign ^ b.sign;
            result.DeleteZeroBytes();
            return result;
        }
        private static BigInteger KaratsubaMultiply(BigInteger x, BigInteger y)
        {
            if (x.data.Count == 0 || y.data.Count == 0)
                return new BigInteger();
            if (Math.Max(x.data.Count, y.data.Count) == 1)
            {
                List<byte> result = new List<byte>();
                int buff = x.data[0] * y.data[0];
                result.Add((byte)(buff % 256));
                result.Add((byte)(buff / 256));
                return new BigInteger(result);
            }
            int n = (Math.Max(x.data.Count, y.data.Count)+1)/ 2;
            x.AddZeroBytes(2*n);
            y.AddZeroBytes(2*n);
            BigInteger a = new BigInteger(x.data.GetRange(n, n));
            BigInteger b = new BigInteger(x.data.GetRange(0, n));
            BigInteger c = new BigInteger(y.data.GetRange(n, n));
            BigInteger d = new BigInteger(y.data.GetRange(0, n));
            BigInteger p1 = KaratsubaMultiply(a, c);
            BigInteger p3 = KaratsubaMultiply(b, d);
            BigInteger p2 = KaratsubaMultiply(a + b, c + d) - p1 - p3;
            BigInteger res = (p1 << (n * 8 * 2)) + (p2 << (n * 8)) + p3;
            return res;
        }

        public static BigInteger operator /(BigInteger a, BigInteger b)
        {
            BigInteger result = new BigInteger();
            BigInteger buff = new BigInteger(b.data);
            int n = -1;
            while(a>buff)
            {
                buff <<= 1;
                n++;
            }
            while (n >= 0)
            {
                if (a >= b << n)
                {
                    a -= b << n;
                    result += new BigInteger(1) << n;
                }
                n--;
            }
            return result;
        }
    }
}
