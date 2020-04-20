using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProgLab_4
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("text.txt");
            LZTable table = new LZTable();
            StreamWriter writer = new StreamWriter("Encoded.txt");
            writer.Write(table.Add(reader.ReadToEnd()));
            //string input = reader.ReadLine();
            //while(input != null)
            //{
            //    foreach (byte b in System.Text.Encoding.Unicode.GetBytes(input))
            //    {
            //        Console.WriteLine(Convert.ToString(b, 2));
            //    }
            //    input = reader.ReadLine();
            //}
            writer.Flush();
            //Console.ReadKey();
        }
    }
}
