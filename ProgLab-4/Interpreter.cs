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
        #endregion
    }
}
