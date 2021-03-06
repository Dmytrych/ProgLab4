﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ProgLab_4
{
    class LZTable
    {
        List<string> dictionary;
        private static readonly string separator = "<#>";
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
            if (path.Contains(".txt"))
            {
                allContains += new StreamReader(path).ReadToEnd() + "\n" + separator + "\n";
            }
            else
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
            StreamWriter writer = new StreamWriter(path + "_Packed.txt");
            writer.Write(table.Encode1(allContains));
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
            translator.Write(table.Decode(reader.ReadToEnd()));
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
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(((char)i).ToString());
            }
            string[] parts = encoded.Split(new char[] { ' ' });
            string temp = dictionary[Interpreter.From36B(parts[0])];
            for (int i = 0; i < parts.Length; i++)
            {
                temp = dictionary[int.Parse(parts[i])];
                Console.WriteLine(int.Parse(parts[i + 1]));
                if (Interpreter.From36B(parts[i]) < dictionary.Count)
                {
                    temp = dictionary[Interpreter.From36B(parts[i])];
                }
                else if(Interpreter.From36B(parts[i]) == dictionary.Count)
                if (!dictionary.Contains(temp + dictionary[int.Parse(parts[i + 1])][0]) && i != parts.Length - 1)
                {
                    dictionary.Add(temp + dictionary[int.Parse(parts[i + 1])][0]);
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
        public string Encode1(string toCompress)
        {
            string compressed = "";
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
                    compressed += dictionary.IndexOf(substr) + " ";
                    dictionary.Add(substrPlusOne);
                    substr = c.ToString();
                }
            }
            return compressed;
        }
        #endregion
    }
}
