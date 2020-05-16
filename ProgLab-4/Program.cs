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
            byte[] bytes = new byte[] { 0,25, 172, 111 };
            int c = (int)BitConverter.ToUInt32(bytes, 0);
            Console.WriteLine(c.ToString());
            //     LZTable.Pack("D:\\Delete");
            LZTable.Unpack("D:\\Delete_Packed.txt", "D:\\Game");
        }
    }
}
