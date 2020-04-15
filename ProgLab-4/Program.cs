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
            LZTable table = new LZTable();
            Console.WriteLine(table.Add("fu fu fuck fucking"));
            Console.ReadKey();
        }
    }
}
