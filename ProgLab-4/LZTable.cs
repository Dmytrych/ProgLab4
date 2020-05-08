﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ProgLab_4
{
    class LZTable
    {
        List<string> dictionary;
        private static readonly string separator = "<#$*%&#>";
        public LZTable()
        {
            dictionary = new List<string>();
            dictionary.Add("0");
            dictionary.Add("1");
        }
        public static void Pack(string path)
        {
            string originPath = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "");
            string allContains = "";
            LZTable table = new LZTable();
            if (Path.GetExtension("D:\\Games") == "")
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
                allContains += new StreamReader(path).ReadToEnd() + "\n" + separator + "\n";
            }
            StreamWriter writer = new StreamWriter(path + "_NotPacked.txt");
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
            string key,temp;
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
        public string Decode1(string encoded)
        {
            string output = "";
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            string[] parts = encoded.Split(new char[] { ' ' });
            string temp;
            for (int i = 0; i < parts.Length; i++)
            {
                temp = dictionary[Interpreter.From36B(parts[i])];
                if (i + 1 != parts.Length)
                {
                    Console.WriteLine(Interpreter.From36B(parts[i + 1]));
                    Console.WriteLine(i + " " + parts.Length);
                    dictionary.Add(temp + dictionary[Interpreter.From36B(parts[i + 1])][0]);
                }
                output += temp;
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
        public string EncodeToFile(string toCompress, string outputFilePath)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream(outputFilePath, FileMode.OpenOrCreate));
            string compressed = "";
            int writeCellWidth = 1;
            for(int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            string substr = "";
            foreach(char c in toCompress)
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
                        writeCellWidth++;
                    for (int i = 0; i < writeCellWidth; i++)
                        writer.Write(indexToByte[i]);
                    writer.Write(c);
                    dictionary.Add(substrPlusOne);
                    substr = c.ToString();
                }
            }
            writer.Close();
            return compressed;
        }
        #endregion
    }
}
