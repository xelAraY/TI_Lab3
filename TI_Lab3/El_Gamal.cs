using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TI_Lab3
{
    public static class El_Gamal
    {
        public static int FindGCD(int a, int b)
        {
            while (a != b)
            {
                if (a > b)
                    a -= b;
                else
                    b -= a;
            }
            return a;
        }

        public static bool IsPrime(int number)
        {
            if (number == 1 || number == 2)
                return true;

            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }

        public static int FastExponentiation(BigInteger mod, BigInteger num, BigInteger deg)
        {
            BigInteger y = 1;
            while (deg != 0)
            {
                while (deg % 2 == 0)
                {
                    deg /= 2;
                    num = (num * num) % mod;
                }
                deg--;
                y = (y * num) % mod;
            }
            return (int)y;
        }

        private static List<int> PrimeDivisors(int value)
        {
            List<int> result = new List<int>();
            for (int i = 2; i < value; i++)
            {
                if (IsPrime(i) && value % i == 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public static List<int> PrimitiveRoots(int value)
        {
            List<int> primitiveRoots = new List<int>();
            List<int> primeDivisors = PrimeDivisors(value - 1);
            for (int i = 2; i < value; i++)
            {
                int j = 0;
                while (j < primeDivisors.Count)
                {
                    if (FastExponentiation(value, i, (value - 1) / primeDivisors[j]) == 1)
                        break;
                    j++;
                }
                if (j == primeDivisors.Count)
                    primitiveRoots.Add(i);
            }
            return primitiveRoots;
        }

        public static short[] Encrypt(List<int> data, byte[] bytes)//g p k x
        {
            short[] result = new short[0];
            int y = FastExponentiation(data[1], data[0], data[3]);
            short a = (short)FastExponentiation(data[1], data[0], data[2]);
            int i = 0;
            foreach (byte by in bytes)
            {
                int firstNum = FastExponentiation(data[1], y, data[2]);
                int secondNum = FastExponentiation(data[1], by, 1);
                short b = (short)FastExponentiation(data[1], firstNum * secondNum, 1);
                Array.Resize(ref result, result.Length+2);
                result[i] = a;
                result[i+1] = b;
                i += 2;
            }
            return result;
        }

        public static byte[] Decrypt(List<int> data, short[] cipciphertext)//x p
        {
            byte[] result = new byte[0];
            int j = 0;
            for(int i = 0; i < cipciphertext.Length; i += 2)
            {
                int firstNum = FastExponentiation(data[1], cipciphertext[i+1], 1);
                int secondNum = FastExponentiation(data[1], cipciphertext[i], data[0]*(data[1]-2));
                byte m = (byte)FastExponentiation(data[1], firstNum*secondNum, 1);
                Array.Resize(ref result, result.Length + 1);
                result[j] = m;
                j++; 
            }
            return result;
        }
    }
}
