using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CreateHTMLPage
{
    public class Processing
    {
        private int N;
        private string fileText;
        private string fileDict;
        private string fileHtml;

        //private List<string> dict;
        //private List<char> separators; 

        private Dictionary<int, string> dict = new Dictionary<int, string>(); // словарь
        private Dictionary<int, char> separators = new Dictionary<int, char>(); // множество разделителей, необходимых для разделения слов в тексте

        public Processing(int n, string fTName, string fDName, string fHName)
        {
            this.N = n;
            this.fileText = fTName;
            this.fileDict = fDName;
            this.fileHtml = fHName;
        }

        public void Proccess()
        {
            Console.WriteLine("Waiting...");

            StreamReader readText = new StreamReader(fileText, Encoding.GetEncoding(1251));

            int countLines = 0, k = 0, s = 1; // countLines - кол-во строк в текущем файле, k - число строк в конкретном предложении, s - номер html-страницы
            string line = "";
            string buffLine = "";
            string temp = "";
            string preBuff = "", afterBuff = "";

            for (s = 1; s <= 10000; s++) //стираем старые html-файлы с тем же именем, на всякий случай, чтобы не захламлять. К примеру, первые 100
            {
                if (File.Exists(fileHtml + s + ".html"))
                {
                    File.Delete(fileHtml + s + ".html");
                }
            }
            s = 1;

            StreamWriter writeText;
            try
			{
				writeText = new StreamWriter(fileHtml + s + ".html", false, Encoding.GetEncoding(1251)); //заводим поток для записи
	    		//создаем первую html-страницу
				writeText.WriteLine("<!DOCTYPE html PUBLIC '-//W3C//DTD HTML 4.01 Strict//EN' 'http://www.w3.org/TR/html4/strict.dtd'>");
				writeText.WriteLine("<html>\n<head>\n<meta http-equiv='Content-Type' content='text/html; charset=cp1251'>");
				writeText.WriteLine("<title>Text from file " + fileText + ", page 1</title>");
				writeText.WriteLine("<body>");
			}
			catch(IOException ex)
			{
                Console.WriteLine(ex.Message);
				return;
			}

            do
            {
                line = readText.ReadLine();
                if(line != null)
                {
                    countLines++; // строка прочитана - увеличиваем кол-во строк в текущем файле
                	/* проверяем, содержит ли строка разделители для предложений (.?!), и если содержит, то разбиваем ее на до разделителя
                	 * и после. Если не содержит, накапливаем полностью строчку как часть предложения.
                	 * Ниже (count > N) смотрим, стоит ли создавать новую страницу
                	 */

                    if (line.Contains("!") || line.Contains(".") || line.Contains("?"))
                    {
                        k++;

                        // определяем "дальний" разделитель в строке среди различных
                        if (line.IndexOf(".") > line.IndexOf("!") && line.IndexOf(".") > line.IndexOf("?")) { temp = "."; }
                        if (line.IndexOf("!") > line.IndexOf(".") && line.IndexOf("!") > line.IndexOf(".")) { temp = "!"; }
                        if (line.IndexOf("?") > line.IndexOf("!") && line.IndexOf("?") > line.IndexOf(".")) { temp = "?"; }

                        preBuff = line; // считаем изначально что все что ДО совпадает с line
                        afterBuff = ""; // тогда и ПОСЛЕ пусто

                        if (line.IndexOf(temp) < line.Length - 2) // если же все таки что то ПОСЛЕ в строке еще есть, разбиваем
                        {
                            preBuff = line.Substring(0, line.IndexOf(temp) + 2);
                            afterBuff = line.Substring(line.IndexOf(temp) + 2, line.Length - line.IndexOf(temp) - 2) + " ";
                        }

                        buffLine += preBuff; // накапливаем в переменную предложения то, что до разделителя вместе с ним

                        if (countLines > N) // оговорено выше - если число строк превышено, создаем новую страницу
                        {
                            s++;
                            writeText.Write("\n</body>\n</html>");
                            writeText.Close();

                            try
                            {
                                writeText = new StreamWriter(fileHtml + s + ".html", false, Encoding.GetEncoding(1251)); //заводим поток для записи
                                //создание новой страницы и заполнение ее необходимыми тегами
                                writeText.WriteLine("<!DOCTYPE html PUBLIC '-//W3C//DTD HTML 4.01 Strict//EN' 'http://www.w3.org/TR/html4/strict.dtd'>");
                                writeText.WriteLine("<html>\n<head>\n<meta http-equiv='Content-Type' content='text/html; charset=cp1251'>");
                                writeText.WriteLine("<title>Text from file " + fileText + ", page " + s + "</title>");
                                writeText.WriteLine("<body>");
                            }
                            catch (IOException ex)
                            {
                                Console.WriteLine(ex.Message);
                                return;
                            }

                            countLines = k; /* так как в новый файл записывается предложение с "предыдущего", 
                                             * то его кол-во строк в предложении учитывается. */
                        }

                        /*char[] separators = { ',', ' ', '.', '_', '!', '?', ':', ':' };
                        string[] words = buffLine.Split(separators);

                        foreach (string word in words)
                        {
                            //Console.Write(word + " ");
                            if (dict.Contains(word))
                            {
                                writeText.Write("<b><i>" + word + "</i></b>" + buffLine.Substring(buffLine.IndexOf(word) + word.Length, 1));
                            }
                            else
                            {
                                writeText.Write(word + " ");
                            }
                        }*/

                        string word = "";
                        char[] chr = buffLine.ToCharArray();
                        for (int i = 0; i < chr.Length; i++)
                        {
                            if (!separators.ContainsValue(chr[i])) // если i-й символ не разделитель, приписываем его
                            {
                                word += chr[i];
                            }
                            else // иначе получаем готовое слово
                            {
                                if (dict.ContainsValue(word)) // проверка на наличие слова в словаре
                                {
                                    writeText.Write("<b><i>" + word + "</i></b>");
                                }
                                else
                                {
                                    writeText.Write(word);
                                }
                                writeText.Write(chr[i]); // приписываем и разделитель, чтобы не портить текст

                                word = ""; //обнуляем переменную слова и продолжаем считывать.
                            }
                        }

                        k = 1;
                        buffLine = "</br> \n" + afterBuff; // как бы обнуляем предложение, полагая его теперь началу следующего
                    }
                    else
                    {
                        buffLine += line + "</br> \n"; // накапливаем полностью строку в предложение, если она не содержит !, ? или .
                        k++; // число строк в предложении - нужно для корректного счета count
                    }
                }

            } 
            while(line != null);
			writeText.Write("\n</body>\n</html>");
			
			readText.Close();
            writeText.Close();

            Console.WriteLine("Done.");
        }

        public void InitializeDictionary() // заполнение словаря и мн-во разделителей
        {
            try
            {
                StreamReader f = new StreamReader(fileDict, Encoding.GetEncoding(1251));

                //dict = new List<string>();

                int k = 0;
                dict = new Dictionary<int, string>();
                while (!f.EndOfStream)
                {
                    k++;
                    dict.Add(k, f.ReadLine());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            separators = new Dictionary<int, char>();
            separators.Add(1, ','); separators.Add(2, ' '); separators.Add(3, '.');
            separators.Add(4, '_'); separators.Add(5, '!'); separators.Add(6, '?');
            separators.Add(7, ':'); separators.Add(8, ';'); separators.Add(9, '\n');
        }
    }
}