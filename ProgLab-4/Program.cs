using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

namespace ProgLab_4
{
    class Program
    {
        static void Main()
        {
            //BinaryWriter writer = new BinaryWriter(new FileStream("D:\\Delete_Packed.txt", FileMode.OpenOrCreate),Encoding.UTF32);
            //byte[] byteArr = BitConverter.GetBytes(260);
            //for (int i = 0; i < byteArr.Length - 2; i++)
            //    writer.Write(byteArr[i]);
            //writer.Close();
            //LZTable table = new LZTable();
            LZTable.Pack("D:\\Delete");
            //table.EncodeToFile("notyounotyounotyou", "D:\\Delete_Packed.txt");
            //BinaryReader reader = new BinaryReader(new FileStream("D:\\Delete_Packed.txt", FileMode.OpenOrCreate), Encoding.Unicode);
            //Console.WriteLine(reader.ReadInt64());
            //Console.ReadLine();
        }
    }
}
