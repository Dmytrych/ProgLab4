using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgLab_4
{
    class LZTable
    {
        List<string> dictionary;
        public LZTable()
        {
            dictionary = new List<string>();
        }
        public string Add(string binaryCode)
        {
            string output = "";
            for (int i = 0; i < binaryCode.Length; i++)
            {
                string substr = binaryCode[i].ToString();
                for (int j = 1; j < binaryCode.Length - i + 1; j++)
                {
                    if (dictionary.Contains(substr) && i + j != binaryCode.Length)
                    {
                        substr += binaryCode[i + j].ToString();
                    }
                    else
                    {
                        if (i + j != binaryCode.Length)
                        {
                            dictionary.Add(substr);
                            i += j - 2;
                        }
                        else
                        {
                            dictionary.Add(substr);
                            output += dictionary.IndexOf(substr) + " ";
                            break;
                        }

                        if (substr.Length != 1)
                            output += dictionary.IndexOf(substr.Remove(substr.Length - 1)) + " ";
                        break;
                    }
                }
            }
            return output;
        }
    }
}
