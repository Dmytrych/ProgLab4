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
            //LZTable.Pack("D:\\Delete");
            //LZTable.Unpack("D:\\Delete_Packed.txt", "D:\\Game");
            LZTable table = new LZTable();
            StreamReader reader = new StreamReader("D:\\Delete\\Text1.txt");
            StreamWriter writer = new StreamWriter("D:\\Delete_Packed.txt");
            //StreamWriter writer1 = new StreamWriter("D:\\Delete_Packed12.txt");
            writer.Write(table.Encode1(reader.ReadToEnd()));
            //reader = new StreamReader("D:\\Delete\\Text1.txt");
            //writer1.Write(Interpreter.T2B(reader.ReadToEnd()));
            //writer1.Close();
            writer.Close();
            //Console.ReadLine();
        }
    }
}
