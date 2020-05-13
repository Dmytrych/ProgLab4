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
            //byte[] byteArr = BitConverter.GetBytes(256);
            //for (int i = 0; i < byteArr.Length; i++)
            //    writer.Write(byteArr[i]);
            //writer.Write('s');
            //writer.Close();
            //BinaryReader reader = new BinaryReader(new FileStream("D:\\Delete\\Kka.txt", FileMode.OpenOrCreate));
            //for(int i = 0; i < 100; i++)
            //{
            //    Console.WriteLine(reader.ReadByte());
            //}
            //Console.WriteLine(BitConverter.ToInt16(reader.ReadBytes(2), 0));
            //Console.WriteLine((char)101);
            //StreamReader reader = new StreamReader("D:\\Delete\\input12.txt", Encoding.GetEncoding(1251));
            //LZTable table = new LZTable();
            //table.EncodeToFile(reader.ReadToEnd(), "D:\\Delete\\output.txt");
            //LZTable.Pack("D:\\Delete\\input.txt");
            StreamReader reader = new StreamReader("C:\\Users\\Богдан\\Desktop\\input.txt");
            LZTable table = new LZTable();
            StreamWriter writer = new StreamWriter(new FileStream("C:\\Users\\Богдан\\Desktop\\text.txt",FileMode.OpenOrCreate),Encoding.Unicode);

            table.EncodeToFile(reader.ReadToEnd(), "C:\\Users\\Богдан\\Desktop\\output.txt");
            reader.Close();

            table = new LZTable();
            writer.Write(table.DecodeFromFile("C:\\Users\\Богдан\\Desktop\\output.txt"));
            writer.Close();
            //LZTable.Pack("D:\\Delete");
            //table.EncodeToFile("notyounotyounotyou", "D:\\Delete_Packed.txt");
            //BinaryReader reader = new BinaryReader(new FileStream("D:\\Delete_Packed.txt", FileMode.OpenOrCreate), Encoding.Unicode);
            //Console.WriteLine(reader.ReadInt64());
            //Console.ReadLine();
        }
    }
}
