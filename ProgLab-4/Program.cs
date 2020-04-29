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
        static void Main()
        {
            
            // Path for yasbog`s tests C:\Users\Богдан\Desktop\H.txt
            Console.WriteLine("test.txt");
            string path = Console.ReadLine();
            StreamReader reader = new StreamReader(path);
            LZTable table = new LZTable();
            StreamWriter writer0 = new StreamWriter("NonEncoded0.txt");
            StreamWriter writer1 = new StreamWriter("Encoded1.txt");
            StreamWriter writer2 = new StreamWriter("Decoded2.txt");
            StreamWriter writer3 = new StreamWriter("Translated3.txt");
            string data = reader.ReadToEnd();
            writer0.Write(Interpreter.T2B(data));
            writer1.Write(table.Encode(Interpreter.T2B(data)));
            writer2.Write(table.Decode(table.Encode(Interpreter.T2B(data))));
            writer0.Flush();
            writer1.Flush();
            writer2.Flush();
            writer3.Write(Interpreter.B2T(table.Decode(table.Encode(Interpreter.T2B(data)))));
            writer3.Flush();
            //Console.WriteLine(table.Add(reader.ReadToEnd()));
            //string input = reader.ReadLine();
            //while(input != null)
            //{
            //    foreach (byte b in System.Text.Encoding.Unicode.GetBytes(input))
            //    {
            //        Console.WriteLine(Convert.ToString(b, 2));
            //    }
            //    input = reader.ReadLine();
            //}
            Console.ReadKey();
        }
    }
}
