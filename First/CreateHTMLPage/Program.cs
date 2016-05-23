using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CreateHTMLPage
{
    class Program
    {
        //const int fileLenghtLimit = 2097152;
        const int fileLenghtLimit = 104857600; // до 100 мб

        static void Main(string[] args)
        {
            int N = 0;
            while(!(N >= 10 && N <= 100000))
            {
                Console.Clear();
                Console.WriteLine("Enter N (10..100000): ");
                try
                {
                    N = Convert.ToInt32(Console.ReadLine());

                    if(N < 10 || N > 100000)
                    {
                        Console.Write("N must be equal 10..100000.\nPress Enter.");
                        Console.ReadLine();
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message + "\nPress Enter.");
                    Console.ReadLine();
                }
            }

            string fTextFile = "text2.txt", fDictFile = "dict2.txt", fHtmlFile = "output";

            // вводим имена (директории) текстовых файлов
            do
            {
                Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Enter name file of text: ");
                fTextFile = Console.ReadLine();

                Console.WriteLine("Enter name file of dictionary: ");
                fDictFile = Console.ReadLine();
            }
            while (!(isCanRun(fTextFile, false) && isCanRun(fDictFile, true)));

            Console.WriteLine("Enter name of html file: ");
            fHtmlFile = Console.ReadLine();

            Console.WriteLine("Preliminary data:\nN = {0};\nText File = '{1}';\nDictionary File = '{2}';\nHtml File(s) = '{3}.html'", N, fTextFile, fDictFile, fHtmlFile);
            Console.WriteLine("Press any key and press enter.");
            Console.ReadLine();

            Processing WriteTextToHTML = new Processing(N, fTextFile, fDictFile, fHtmlFile);
            WriteTextToHTML.InitializeDictionary();
            WriteTextToHTML.Proccess();

            Console.ReadLine();
        }

        static bool isCanRun(string fName, bool bDict) //Проверка файлов на существование и удовлетворение условиям задачи
        {
            try
            {
                if (bDict) // если проверяем файл словаря, считаем дополнительно кол-во строк
                {
                    StreamReader f = new StreamReader(fName);
                    int countLine = 0; // счётчик кол-ва строк в файле
                    while(!f.EndOfStream)
                    {
                        f.ReadLine();
                        countLine++;
                    }

                    if (countLine > 100000)
                    {
                        Console.WriteLine("Error! File " + fName + " too long. Try again.");
                        return false;
                    }
                }
                else
                {
                    using (FileStream fs = File.Open(fName, FileMode.Open))
                    {
                        if (!File.Exists(fName) || !fs.CanRead)
                        {
                            Console.WriteLine("Error! File " + fName + " don't exists, read or execute. Try again.");
                            return false;
                        }

                        if (fs.Length > fileLenghtLimit)
                        {
                            Console.WriteLine("Error! File " + fName + " too long. Try again.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
