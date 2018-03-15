using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace VoidnichTranslator
{
    class Program
    {
        public static Hashtable slogs; //слоги
        public static Hashtable ends;  //окончания
        public static Hashtable words;  //слова без окончаний
        public static Hashtable unnouns; //союзы, междометия, частицы и тд
        public static Hashtable hapax;  //синонимы
        public static string[][] wmap;
        public static int synonims = 0;
        public static int sinestesy = 0;
        public static string uper = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯQWERTYUIOPASDFGHJKLZXCVBNM";
        public static string lower = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяqwertyuiopasdfghjklzxcvbnm";
        public static string vowel = "аеёиоуыэюяeyuioaEYUOAIУЕЫАОЭЯИЮ";
        public static Random rnd;
        public static string[][] str1;
        public static int[] slin1;

        public static string[] SplitIt(string buf, string sep1)
        {
            string[] seps = new string[] { sep1 };
            string[] buf1 = buf.Split(seps, StringSplitOptions.RemoveEmptyEntries);
            return buf1;
        }

         public static int GetSlen(int slc)
        {
            int result = 2;
            int[,] slmat = new int[,] {{10000, 15000, 21000, 25000}, {57000, 69489, 75589, 78589}, {87000, 92489, 94589, 96589}, {97000, 99489, 99589, 99589}, 
                {99800, 99989, 99989, 99989}, {99999, 99999, 99999, 99999}, {100000, 100000, 100000, 100000}};

            if (slc < 1) { slc = 2; }
            if (slc == 1) { result = rnd.Next(1, 5); }
            else {
            slc = slc - 2;
            if (slc > 3) { slc = 3; }

            int s = rnd.Next(1, 100000);
            if (s < slmat[0,slc]) { result = 1; } else
            if (s < slmat[1,slc]) { result = 2; } else
            if (s < slmat[2,slc]) { result = 3; } else
            if (s < slmat[3,slc]) { result = 4; } else
            if (s < slmat[4,slc]) { result = 5; } else
            if (s < slmat[5,slc]) { result = 6; } else
            if (s < slmat[6,slc]) { result = 7; }
                }
            return result;
        }

        public static string[] GetFormula(string wrd)
        {
            string[] result = new string[2];
            wrd = wrd.Replace("<", "(");
            wrd = wrd.Replace(">", ")");
            result[0] = "";
            result[1] = "";
            char[] wd = wrd.ToCharArray();
            string wr = "";
            for (int i = 0; i < wrd.Length; i++)
            {
                if (uper.Contains(wd[i]) == true)
                { if (wr == "") { wr = "<"; result[0] += wr; } result[1] += wd[i].ToString(); }
                else if (lower.Contains(wd[i]) == true) 
                { if (wr == "") { wr = ">"; result[0] += wr; } result[1] += wd[i].ToString(); }
                else { result[0] += wd[i].ToString(); wr = ""; }
            }

            int slg = 0;
            for (int i = 0; i < result[1].Length; i++)
            {
                if (vowel.Contains(result[1][i]) == true) { slg++; }
            }
            //if (slg == 0) { result[0] = result[0].Replace("<", result[1]); result[0] = result[0].Replace(">", result[1]); result[1] = ""; } 
            if (slg == 0)
            {
                string tmp0 ="";
                string tmp1 ="";
                for (int i = 0; i < result[1].Length; i++)
                { 
                    tmp0 += uper[rnd.Next(uper.Length)];
                    tmp1 += lower[rnd.Next(lower.Length)];
                }
                 result[0] = result[0].Replace("<", tmp0); result[0] = result[0].Replace(">", tmp1); result[1] = ""; 
            }
            result[1] = result[1].ToLower();
            return result;
        }

        public static string GetSlog(bool clos, int slc)
        {
            string result = "";
            int s = 0;
            while (s == 0)
            {
                int slr = GetSlen(slc);
                char[] buf = str1[slr][(int)rnd.Next(slin1[slr])].ToCharArray();
                if (vowel.Contains(buf[buf.Length - 1]) != clos) 
                {
                    result = new string(buf);
                    s = 1;
                }
            }
           
            return result;
        }
        public static string GetSlog(int slc)
        {
            string result = "";
            int slr = GetSlen(slc);
            result = str1[slr][(int)rnd.Next(slin1[slr])];
            return result;
        }


        public static string GetSynonim(string wrd, bool clos, int slc)
        {
            string result = wrd;
            if (words.ContainsKey(wrd) == true)
            {
                int w1 = (int)words[wrd];
                int w2 = wmap[w1].Count();
                result = wmap[w1][rnd.Next(0, w2 - 1)]; //первое слово не обрабатывается
            }
            else
            {
                int w1 = wmap.Count();
                int w2 = 0; int s = 0;
                string[] buf2 = new string[20];
                Array.Resize(ref wmap, w1 + 1);
                words.Add(wrd, w1);
                while ((s==0)&&(w2<10))
                {
                    if ((rnd.Next(100) < synonims)||(w2==0)) 
                    {
                        buf2[w2] = "";
                        bool dn = false;
                        while (dn == false)
                        {
                       for (int i=0; i<(slc-1); i++)
                       {
                           buf2[w2] += GetSlog(slc); 
                       }
                       buf2[w2] += GetSlog(clos, slc);
                       if (hapax.ContainsKey(buf2[w2]) == false) { dn = true; hapax.Add(buf2[w2], 1); }
                        }
                        w2++;  
                    } else {s=1;} 
                }
                s = 0;
                while ((s == 0) && (w2 < 10))
                {
                    if ((rnd.Next(100) < sinestesy)&&(wmap.Count()>2))
                    {
                        int ww1 = rnd.Next(1, wmap.Count() - 2);
                        buf2[w2] += wmap[ww1][rnd.Next(wmap[ww1].Count()-1)];
                        w2++;
                    }
                    else { s = 1; }
                }
                wmap[w1] = new string[w2+1];
                Array.Copy(buf2, wmap[w1], w2+1);
                result = GetSynonim(wrd, clos, slc);

            }
            return result;
        }

        public static string GetWord(string wrrd)
        {
            string result = wrrd; 
            if ((unnouns.ContainsKey(wrrd) == false)&&(wrrd.Length >2)) 
            {
                result = "";
                string[] buf1 = SplitIt(wrrd, "-");
                if (buf1.Count() > 1)
                {
                    for (int i = 0; i < buf1.Count(); i++)
                    {
                        result += GetWord(buf1[i]);
                        if (i < buf1.Count() - 1) { result += "-"; }
                    }
                }
                else
                { 
                    char[] chrd = wrrd.ToCharArray();
                    string end1 = "";
                    string wrd1 = "";
                    int slg = 0; 
                    bool ew = false;
                    bool clos = false;
                    if (vowel.Contains(chrd[chrd.Count() - 1]) == false) { clos = true; } 
                    for (int i = chrd.Count() - 1; i > -1; i--)
                    {
                        if (vowel.Contains(chrd[i]) == true) {slg++;}
                        if (ew == false) 
                        {
                            end1 = chrd[i].ToString() + end1;
                            if (ends.ContainsKey(end1) == true)
                            { ew = true; slg = 0; if (vowel.Contains(chrd[i - 1]) == false) { clos = true; } }
                        }
                        else if (ew == true)
                        {
                            wrd1 = chrd[i].ToString() + wrd1;
                        }
                    }
                    if (wrd1.Length < 2) { wrd1 += end1; end1 = ""; result = GetSynonim(wrd1, clos, slg); } 
                    else
                    {
                        result = GetSynonim(wrd1, clos, slg) + end1;
                    }
                }
            }
            return result;
        }

        public static void ProcessBook(string book)
        {
            foreach (string line in File.ReadLines(book, System.Text.Encoding.GetEncoding(1251))) //losed end
            {
                string wrd3 = "";
                string[] buf1 = SplitIt(line, " ");
                for (int i = 0; i < buf1.Count(); i++)
                {
                    string[] wrd = GetFormula(buf1[i]);

                    if (wrd[1].Length < 1) { wrd3 += wrd[0] + " "; }
                    else
                    {
                        char[] wrd0 = wrd[0].ToCharArray();
                        char[] wrd2 = GetWord(wrd[1]).ToCharArray();


                        for (int j = 0; j < wrd[0].Length; j++)
                        {
                            if ((wrd0[j].ToString() == "<") || (wrd0[j].ToString() == ">"))
                            {
                                int cou = 1;
                                if (wrd0[j].ToString() == "<") { wrd3 += wrd2[0].ToString().ToUpper(); }
                                else { cou = 0; }
                                while (cou < wrd2.Count())
                                {
                                    wrd3 += wrd2[cou].ToString();
                                    cou++;
                                }
                            }
                            else { wrd3 += wrd0[j].ToString(); }
                        }
                        wrd3 += " ";
                    }
                }
                File.AppendAllText("result.txt",  wrd3 + Environment.NewLine); 
            }
        }

        static void Main(string[] args)
        {
            string inbuf;
            rnd = new Random();
            slogs = new Hashtable();
            
            foreach (string line in File.ReadLines("slogs.txt"))
            {
                if (slogs.ContainsKey(line) != true) { slogs.Add(line, line.Length); }
            }
            str1 = new string[8][];
            for (int i = 0; i < 8; i++)
            { str1[i] = new string[(int)slogs.Count]; }
            slin1 = new int[8];
            foreach (DictionaryEntry slo in slogs)
            {
                if ((int)slo.Value < 8)
                { str1[(int)slo.Value][slin1[(int)slo.Value]] = slo.Key.ToString(); slin1[(int)slo.Value]++; }
            }
            slogs.Clear();

            unnouns = new Hashtable();
            foreach (string line in File.ReadLines("words.txt", System.Text.Encoding.GetEncoding(1251)))
            {
                if (unnouns.ContainsKey(line) != true) { unnouns.Add(line, 1); }
            }
            ends = new Hashtable();
            foreach (string line in File.ReadLines("ends.txt", System.Text.Encoding.GetEncoding(1251)))
            {
                if (ends.ContainsKey(line) != true) { ends.Add(line, 1); }
            }
            words = new Hashtable();
            hapax = new Hashtable();
            wmap = new string[1][];

            Console.Write("Система листпих-шифрования книг 'Манускрипт Войдича(тм)' \r\n");
            Console.Write("Задайте коэффициент синонимичности. (%) \r\n");
            inbuf = Console.ReadLine();
            if (inbuf.Length > 0) { synonims = int.Parse(inbuf); } else { synonims = 0; }
            Console.Write("Задайте коэффициент смысловой синестезии. (%) \r\n");
            inbuf = Console.ReadLine();
            if (inbuf.Length > 0) { sinestesy = int.Parse(inbuf); } else { sinestesy = 0; }
            Console.Write("Введите имя исходного файла. \r\n");
            inbuf = Console.ReadLine();
            File.WriteAllText("result.txt", " "); 
            ProcessBook(inbuf);
            Console.Write("Готово! Результат в файле result.txt \r\n");
            Console.ReadLine();
        }
    }
}


