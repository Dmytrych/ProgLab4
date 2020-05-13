using System;
using System.Collections.Generic;
using System.IO;
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
            StreamReader reader = new StreamReader(fromPath); ;
            StreamWriter translator = new StreamWriter(toPath + "\\temp.txt");
            //translator.Write(table.Decode1(table.Encode1(reader.ReadToEnd())));
            translator.Close();
            reader = new StreamReader(toPath + "\\temp.txt");
            string txtPath = toPath + "\\" + reader.ReadLine();
            Directory.CreateDirectory(txtPath.Remove(txtPath.LastIndexOf('\\')));
            StreamWriter writer = new StreamWriter(txtPath);
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
                        writer = new StreamWriter(txtPath);
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
        public string Decode(string encoded)
        {

            string output = "";
            string[] parts = encoded.Split(new char[] { ' ' });
            string key, temp;
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 1)
                {
                    key = parts[i].Substring(0, parts[i].Length - 1);
                    temp = dictionary[Convert.ToInt32(key)] + parts[i][parts[i].Length - 1];
                }
                else
                {
                    temp = parts[i];
                }
                if (!dictionary.Contains(temp))
                {
                    dictionary.Add(temp);
                }
                output += temp;
            }
            return output;
        }
        public string DecodeFromFile(string encodedFilePath)
        {
            BinaryReader reader = new BinaryReader(new FileStream(encoded, FileMode.Open));
            string output = "";
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            int charPerStep = 1;
            int index = 0;
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                if (dictionary.Count > Math.Pow(2, 8 * charPerStep))
                {
                    charPerStep++;
                }
                switch (charPerStep)
                {
                    case 1:
                        index = reader.ReadByte();
                        break;
                    case 2:
                        index = BitConverter.ToUInt16(reader.ReadBytes(charPerStep), 0);
                        break;
                    case 3:
                        index = (int)BitConverter.ToUInt32(reader.ReadBytes(charPerStep), 0);
                        break;
                    case 4:
                        index = (int)BitConverter.ToUInt32(reader.ReadBytes(charPerStep), 0);
                        break;
                }
                string part = dictionary[index];
                output += part;
                dictionary.Add(part + dictionary[reader.ReadByte()]);
            }

            return output;
        }
        public string Encode(string binaryCode)
        {
            string output = "";
            for (int i = 0; i < binaryCode.Length; i++)
            {
                string substr = binaryCode[i].ToString();
                for (int j = 1; j < binaryCode.Length - i + 1; j++)
                {
                    //Если подстрока уже есть в словаре и не конец строки - добавляем ещё одну букву
                    if (dictionary.Contains(substr) && i + j != binaryCode.Length)
                    {
                        substr += binaryCode[i + j].ToString();
                    }
                    else
                    {
                        if (i + j != binaryCode.Length)
                        {
                            //Если не конец строки - добавляем в словарь
                            dictionary.Add(substr);
                            i += j - 1;
                        }
                        else
                        {
                            //Если конец - добавляем подстроку в ответ и возвращаем
                            dictionary.Add(substr);
                            output += dictionary.IndexOf(substr) + " ";
                            break;
                        }
                        if (substr.Length > 1)
                            output += dictionary.IndexOf(substr.Remove(substr.Length - 1)) + " ";
                        output += substr[substr.Length - 1];
                        break;
                    }
                }
            }
            return output;
        }
        public void EncodeToFile(string toCompress, string outputFilePath)
        {
            StreamWriter log = new StreamWriter("D:\\logs.txt");
            BinaryWriter writer = new BinaryWriter(new FileStream(outputFilePath, FileMode.OpenOrCreate));
            int writeCellWidth = 1;
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            string substr = "";
            foreach (char c in toCompress)
            {
                string substrPlusOne = substr + c;
                if (dictionary.Contains(substrPlusOne))
                {
                    substr = substrPlusOne;
                }
                else
                {
                    byte[] indexToByte = BitConverter.GetBytes(dictionary.IndexOf(substr));
                    if (dictionary.IndexOf(substr) > Math.Pow(2, 8 * writeCellWidth))
                    {
                        writeCellWidth++;
                        log.WriteLine("Increased");
                    }
                    log.WriteLine("Indexof " + dictionary.IndexOf(substr));
                    log.WriteLine("I am coding:" + substr);
                    for (int i = 0; i < writeCellWidth; i++)
                    {
                        writer.Write(indexToByte[i]);
                    writer.Write(c);
                    dictionary.Add(substrPlusOne);
                    substr = c.ToString();
                }
            }
            writer.Close();
        }

        public void DecodeToData(string path)
        {
            BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.OpenOrCreate));
            int readCellWidth = 1;
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            string substr = "";
            foreach (char c in reader.ReadBytes(readCellWidth))
            {
                string substrPlusOne = substr + c;
                if (dictionary.Contains(substrPlusOne))
                {
                    substr = substrPlusOne;
                }
                else
                {
                    string output = "";
                    while (reader.PeekChar() != null)
                    {
                        char k;
                        switch (readCellWidth)
                        {
                            case 1:
                                k = (char)BitConverter.ToInt16(reader.ReadBytes(readCellWidth), 0);
                                break;
                            case 2:
                                k = (char)BitConverter.ToInt32(reader.ReadBytes(readCellWidth), 0);
                                break;
                            default:
                                k = (char)BitConverter.ToInt64(reader.ReadBytes(readCellWidth), 0);
                                break;
                        }
                        output += k;
                    }
                    Console.WriteLine(output);
                    if (dictionary.IndexOf(substr) > Math.Pow(2, 8 * readCellWidth))
                    {
                        readCellWidth++;
                    }
                    for (int i = 0; i < readCellWidth; i++)
                    {
                        //writer.Write(indexToByte[i]);
                    }
                    dictionary.Add(substrPlusOne);
                    substr = c.ToString();
                }
            }
        }
    }


            #endregion
}
    

