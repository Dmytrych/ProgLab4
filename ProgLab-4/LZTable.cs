using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProgLab_4
{
    class LZTable
    {
        List<string> dictionary;
        private static readonly string separator = "<#$*%&#>";
        public LZTable()
        {
            dictionary = new List<string>();
            dictionary.Add("");
        }
        public static void Pack(string path)
        {
            string originPath = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "");
            string allContains = "";
            LZTable table = new LZTable();
            if (Path.GetExtension(path) == "")
            {
                foreach (string dirPath in Directory.GetDirectories(path))
                {
                    allContains += PackDir(originPath, dirPath);
                }
                foreach (string filename in Directory.GetFiles(path))
                {
                    allContains += filename.Replace(originPath, "") + "\n" + new StreamReader(filename).ReadToEnd() + "\n" + separator + "\n";
                }
            }
            else
            {
                allContains += new StreamReader(path).ReadToEnd();
            }
            StreamWriter writer = new StreamWriter(new FileStream(path + "_NotPacked.txt", FileMode.OpenOrCreate));
            table.EncodeToFile(allContains, path + "_Packed.txt");
            writer.Write(allContains);
            writer.Close();
        }
        private static string PackDir(string originPath, string currentPath)
        {
            string allContains = "";
            foreach (string dirPath in Directory.GetDirectories(currentPath))
            {
                allContains += PackDir(originPath, dirPath);
            }
            foreach (string filename in Directory.GetFiles(currentPath))
            {
                allContains += filename.Replace(originPath, "") + "\n" + new StreamReader(filename).ReadToEnd() + "\n" + separator + "\n";
            }
            return allContains;
        }
        public static void Unpack(string fromPath, string toPath)
        {
            LZTable table = new LZTable();
            string input;

            StreamWriter writer = new StreamWriter( new FileStream(toPath + "\\temp.txt", FileMode.OpenOrCreate), Encoding.Unicode);
            writer.Write(table.DecodeFromFile(fromPath));
            writer.Close();

            StreamReader reader = new StreamReader(fromPath);
            reader = new StreamReader(new FileStream(toPath + "\\temp.txt", FileMode.OpenOrCreate), Encoding.Unicode);
            string txtPath = toPath + "\\" + reader.ReadLine();
            Directory.CreateDirectory(txtPath.Remove(txtPath.LastIndexOf('\\')));

            writer = new StreamWriter(new FileStream(txtPath, FileMode.OpenOrCreate), Encoding.Unicode);
            while (!reader.EndOfStream)
            {
                input = reader.ReadLine();
                if (input.Contains(separator))
                {
                    writer.Close();
                    if (!reader.EndOfStream)
                    {
                        txtPath = toPath + "\\" + reader.ReadLine();
                        Directory.CreateDirectory(txtPath.Remove(txtPath.LastIndexOf('\\')));
                        writer = new StreamWriter(new FileStream(txtPath, FileMode.OpenOrCreate), Encoding.Unicode);
                    }
                }
                else
                    writer.WriteLine(input);
            }
            reader.Close();
            File.Delete(toPath + "\\temp.txt");
        }
  
         public string DecodeFromFile(string encoded)
        {
            BinaryReader reader = new BinaryReader(new FileStream(encoded, FileMode.Open));

            string output = "";
            dictionary.Clear();
            dictionary.Add("");
            int charPerStep = 2;
            byte[] elementByte  , indexByte;
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                if (dictionary.Count >= Math.Pow(2, 8 * charPerStep))
                {
                    charPerStep++;
                }

                indexByte = reader.ReadBytes(charPerStep);
                elementByte = reader.ReadBytes(2);
                string s = "";
                switch (charPerStep)
                {
                    case 1:case 2:
                        s = dictionary[(int)BitConverter.ToUInt16(indexByte, 0)];
                        dictionary.Add(s+BitConverter.ToChar(elementByte, 0).ToString());
                        break;
                    case 3:
                        indexByte = new byte[] {indexByte[0], indexByte[1],indexByte[2], 0};
                        s = dictionary[(int)BitConverter.ToUInt32(indexByte, 0)];
                        dictionary.Add(s + BitConverter.ToChar(elementByte, 0).ToString());
                        break;
                    case 4:
                        s = dictionary[(int)BitConverter.ToUInt32(indexByte, 0)];
                        dictionary.Add(s+ BitConverter.ToChar(elementByte, 0).ToString());
                        break;
                }
             
                output += s;
            }
            reader.Close();
            return output;
        }
        public void EncodeToFile(string toCompress, string outputFilePath)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(outputFilePath, FileMode.OpenOrCreate), Encoding.Unicode);
            int writeCellWidth = 2;
            string substr = "";
            dictionary.Clear();
            dictionary.Add(substr);
            foreach (char c in toCompress)
            {
                string substrPlusOne = substr + c;
                if (!dictionary.Contains(substr))
                {
                    for (int i = 0; i < writeCellWidth; i++)
                    {
                        writer.Write((byte)0);
                    }
                    dictionary.Add(substr);
                    writer.Write(substr[substr.Length - 1]);
                }
                if (dictionary.Contains(substrPlusOne))
                {
                    substr = substrPlusOne;
                }
                else
                {
                    byte[] indexToByte = BitConverter.GetBytes(dictionary.IndexOf(substr));
                    if (dictionary.IndexOf(substr) >= Math.Pow(2, 8 * writeCellWidth))
                    {
                        writeCellWidth++;
                        Console.WriteLine(writeCellWidth.ToString());
                    }
                    for (int i = 0; i < writeCellWidth; i++)
                    {
                        writer.Write(indexToByte[i]);
                    }
                    writer.Write(c);
                    dictionary.Add(substrPlusOne);
                    substr = c.ToString();
                }
            }
            if (dictionary.Contains(substr))
            {
                byte[] indexToByte = BitConverter.GetBytes(dictionary.IndexOf(substr));
                for (int i = 0; i < writeCellWidth; i++)
                {
                    writer.Write(indexToByte[i]);
                }
                writer.Write(' ');
            }
            else
            {
                for (int i = 0; i < writeCellWidth; i++)
                {
                    writer.Write((byte)0);
                }
                dictionary.Add(substr);
                writer.Write(substr[substr.Length - 1]);
                byte[] indexToByte = BitConverter.GetBytes(dictionary.IndexOf(substr));
                for (int i = 0; i < writeCellWidth; i++)
                {
                    writer.Write(indexToByte[i]);
                }
                writer.Write(' ');
            }
            writer.Close();
        }
     
    }
}


