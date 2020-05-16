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
        //private static string IncludeInfo(string path)
        //{
        //    if (path.Contains(".txt"))
        //    {
        //        return "1\n" + path.Split('\\')[path.Split('\\').Length - 1] + "\n";
        //    }
        //    else
        //    {
        //        string info = Directory.GetFiles(path).Length.ToString() + "\n",
        //            dirName = path.Split('\\')[path.Split('\\').Length - 1];

        //        foreach(string filename in Directory.GetFiles(path))
        //        {
        //            info += dirName + "\\" + filename + "\n";
        //        }
        //        return info;
        //    }
        //}
        #region methods
         public string DecodeFromFile(string encoded)
        {
            BinaryReader reader = new BinaryReader(new FileStream(encoded, FileMode.Open));

            string output = "";
            dictionary.Clear();
            dictionary.Add("");
            int charPerStep = 2;
            byte[] elementByte = new byte[4], indexByte;
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                if (dictionary.Count > Math.Pow(2, 8 * charPerStep))
                {
                    charPerStep++;
                }

                //switch (charPerStep)
                //{
                //    case 2:
                //         index = (int)BitConverter.ToUInt16(element, 0);
                //        break; 
                //    case 3:
                //        index = (int)BitConverter.ToUInt32(element, 0);
                //        break;
                //    case 4:
                //        index = (int)BitConverter.ToUInt32(element, 0);
                //        break;
                //}
                indexByte = reader.ReadBytes(charPerStep);
                elementByte = reader.ReadBytes(charPerStep);

                dictionary.Add(dictionary[BitConverter.ToInt16(indexByte, 0)]
                    + BitConverter.ToChar(elementByte, 0).ToString());
                output += dictionary[BitConverter.ToInt16(indexByte, 0)];
            }
            reader.Close();
            return output;
        }
        public void EncodeToFile(string toCompress, string outputFilePath)
        {
            StreamWriter log = new StreamWriter(new FileStream("D:\\logs.txt",FileMode.OpenOrCreate), Encoding.Unicode);
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
                        log.WriteLine("Increased");
                    }
                    log.WriteLine("Dictionary count: " + dictionary.Count);
                    log.WriteLine("Indexof " + dictionary.IndexOf(substr));
                    log.WriteLine("I am coding substring of:" + substr);
                    for (int i = 0; i < writeCellWidth; i++)
                    {
                        writer.Write(indexToByte[i]);
                        log.WriteLine("Wrote byte");
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
        //if (dictionary.Contains(substr))
        //{
        //    byte[] indexToByte = BitConverter.GetBytes(dictionary.IndexOf(substr));
        //    for (int i = 0; i < writeCellWidth; i++)
        //    {
        //        writer.Write(indexToByte[i]);
        //    }
        //}

    }
    #endregion
}


