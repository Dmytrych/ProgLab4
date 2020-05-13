using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgLab_4
{
   static class Interpreter
    {
        #region methods
      public static string B2T(string binary)
        {
            string text="";
            for (int i = 0; i < binary.Length - 1; i+=8)
            {
                string temp = binary.Substring(i, 8);
                int a=0;
                for (int j = 0; j < temp.Length; j++)
                {
                    if(temp[j]=='1')
                    a +=(int)Math.Pow(2, 7 - j);
                }
                char c = (char)a;
                text += c.ToString();
            }
            return text;
        }
       public static string T2B(string text)
        {
            string binary="";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int a = (int)c;
                int b = 0;
                for (int j = 7; j >=0; j--)
                {
                    if (a >=(int)Math.Pow(2, j))
                    {
                        b += (int)Math.Pow(10, j);
                        a -= (int)Math.Pow(2, j);
                    }
                }
                string temp = b.ToString();
                while (temp.Length < 8)
                {
                    temp = "0"+temp;
                }
                binary += temp;
            }
            return binary;
        }
        public static string To36B(int num)
        {
            char[] alph = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string num36 = "";
            while (num != 0)
            {
                num36 += alph[num % 36];
                num = num / 36;
            }
            return num36;
        }
        public static int From36B(string num36)
        {
            char[] alph = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            int sum = 0;
            for (int i = num36.Length - 1; i >= 0; i--)
            {
                int index = 0;
                for (int j = 0; j < alph.Length; j++)
                {
                    if (alph[j] == num36[i])
                    {
                        index = j;
                        break;
                    }
                }
                sum += index * (int)Math.Pow(36, i);
            }
            return sum;
        }

        #endregion
    }
}
