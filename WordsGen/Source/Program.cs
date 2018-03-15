using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace HapaxGen
{
    class Program
    {
        public static Hashtable slogs1;
        public static Hashtable slogs2; 
        public static string vowel = "аеёиоуыэюя";
        public static string voiced = "бвгджзлмнрхцчшщ";
        public static string deaf = "кпстф";
        public static string[][] str1;
        public static string[][] str2;
        public static int[] slin1;
        public static int[] slin2;
       



        public static bool IsSep(char l1, char l2)
        {
            bool ans = false;

            if ((voiced.Contains(l1) == true) && (deaf.Contains(l2) == true)) { ans = true; }
            else if ((voiced.Contains(l2) == true) && (deaf.Contains(l1) == true)) { ans = true; }

            return ans;
        }

        public static int GetSlen(Random rnd, int slc)
        {
            int result = 2;
            int[,] slmat = new int[,] {{10000, 15000, 21000, 25000}, {57000, 69489, 75589, 78589}, {87000, 92489, 94589, 96589}, {97000, 99489, 99589, 99589}, 
                {99800, 99989, 99989, 99989}, {99999, 99999, 99999, 99999}, {100000, 100000, 100000, 100000}};

            slc = slc - 2;
            if (slc > 4) { slc = 4; }

            int s = rnd.Next(1, 100000);
            if (s < slmat[0,slc]) { result = 1; } else
            if (s < slmat[1,slc]) { result = 2; } else
            if (s < slmat[2,slc]) { result = 3; } else
            if (s < slmat[3,slc]) { result = 4; } else
            if (s < slmat[4,slc]) { result = 5; } else
            if (s < slmat[5,slc]) { result = 6; } else
            if (s < slmat[6,slc]) { result = 7; }
            return result;
        }

        public static int GetSlogs(string wrd)
        {
            wrd = wrd.ToLower();
            int result = 0;
            int lw = wrd.Length;
            char[] wd = wrd.ToCharArray();
            int[] wd1 = new int[lw+1];
            int[] wd2 = new int[lw+1];

            for(int i=0; i<lw; i++)  
            {
                if (vowel.Contains(wd[i])==true) { result++; wd1[i] = result; }
                else if (result == 0) { wd1[i] = 1; }
            } //detecting vowels
            {
            int i = lw-1;
            while (wd1[i] < 1)
            { wd1[i] = result; i--; }
            } //detecting first slog
            Array.Copy(wd1, wd2, lw + 1);

            for (int i = 1; i < result; i++)
            {
                int l0 = 0;
                while (wd1[l0 + 1] < i) { l0++; }
                int l1 = l0;
                while (wd1[l1+1] == i) { l1++; }
                int l2 = l1+1;
                while (wd1[l2] != i+1) { l2++; }
                int n = l2 - l1 - 1;   

                if (n > 0) 
                {
                    if (vowel.Contains(wd[l0]) == true)
                    {
                        for (int j = l0 + 1; j < l2; j++) { wd1[j] = wd1[l2]; wd2[j] = wd2[l2]; }
                    }
                    else if (n == 1) { wd1[l1 + 1] = wd1[l2]; wd2[l1 + 1] = wd2[l2]; }
                    else if (n > 1)
                    {
                        if (n % 2 == 0) 
                        {
                            for (int j = l1 + 1; j < l2; j++)
                            { 
                                if (l2 - j > (n / 2)) { wd2[j] = wd2[l1]; }
                                else { wd2[j] = wd2[l2]; } 
                            }
                        }
                        else if (n % 2 != 0)
                        {
                            for (int j = l1 + 1; j < l2; j++)
                            {
                                if (l2 - j > ((n-1) / 2)) { wd2[j] = wd2[l1]; }
                                else { wd2[j] = wd2[l2]; }
                            }
                        } //met 1

                        int t = wd1[l2];
                        int mt = 0;
                        for (int j = l2 - 1; j > l1; j--) { if (wd[j] == 'й') { mt = 1; } }
                        for (int j = l2 - 1; j > l1; j--)
                        {
                            if ((mt == 0) && (IsSep(wd[j - 1], wd[j]) == true)) { wd1[j] = t; t = wd1[l1]; }
                            else if ((mt==1)&&(wd[j] == 'й')) { t = wd1[l1]; wd1[j] = t; }
                            else { wd1[j] = t; }

                        } //met2


                    }
                }

            }
            string buf = "";
            for (int k = 0; k < lw; k++)
            {
                
                int t = 1;
                if (wd1[k] != t)
                {
                    t = wd1[k]; 
                    if (slogs1.ContainsKey(buf) != true) { slogs1.Add(buf, 1); buf = ""; }
                }
                else buf += wd[k].ToString();
            }
            buf = "";
            for (int k = 0; k < lw; k++)
            {
                
                int t = 1;
                if (wd2[k] != t)
                {
                    t = wd2[k]; 
                    if (slogs2.ContainsKey(buf) != true) { slogs2.Add(buf, 1); buf = ""; }
                }
                else buf += wd[k].ToString();
            }

            return result;
        }

        public static string GetGap(int w1, int w2, int avr)
        {
            string gaps = "";
            int itn = (avr * 6) - w1 - w2+3;
            if (itn < 1) {itn = 5;}
            for (int i = 0; i < itn; i++) { gaps += " "; }
            return gaps;
        }

        public static string GetWords(int ns, int n)
        {
            string output = "";
            Random rnd = new Random();
            int gp = 0;
            if (ns == 0) { gp = 6; } else { gp = ns; }
            output += "fonetic:"+GetGap(8,11,gp)+"morfologic:" + Environment.NewLine;
            for (int i = 0; i < n; i++)
            {
                if (ns == 0) { ns = (int)rnd.Next(2,6); }
                string wrd1 = "";
                string wrd2 = "";
                for (int j = 0; j < ns; j++)
                {
                    int slr = GetSlen(rnd, ns);
                wrd1 += str1[slr][(int)rnd.Next(slin1[slr])];
                wrd2 += str2[slr][(int)rnd.Next(slin2[slr])];
                }
                output += wrd1 + GetGap(wrd1.Length, wrd2.Length, gp) + wrd2 + Environment.NewLine;
            }

            return output;
        }


        static void Main(string[] args)
        {
            slogs1 = new Hashtable();
            slogs2 = new Hashtable();
            
            int sl = 0; string inbuf;
            int ns = 0;
            int n = 1;
            foreach (string line in File.ReadLines("words.txt", System.Text.Encoding.GetEncoding(1251)))
            {
                int sl1 = GetSlogs(line);
                if (sl < sl1) { sl = sl1; }
            }
            Console.Write("Create dumps? y/n \r\n");
            string d = Console.ReadLine();
            if (d == "y")
            {
                File.WriteAllText("slogs-fon.txt", "фонетичексие слоги: \r\n");
                File.WriteAllText("slogs-morf.txt", "морфологические слоги: \r\n");
                foreach (DictionaryEntry slo in slogs1) { File.AppendAllText("slogs-fon.txt", slo.Key + Environment.NewLine); }
                foreach (DictionaryEntry slo in slogs2) { File.AppendAllText("slogs-morf.txt", slo.Key + Environment.NewLine); }

            }
            Console.Write("Укажите количество слогов? (Оставьте 0 для рандома) \r\n");
            inbuf = Console.ReadLine();
            if (inbuf.Length > 0) { ns = int.Parse(inbuf); } else {ns = 0;}
            Console.Write("Сколько слов сгенерить? \r\n");
            inbuf = Console.ReadLine();
            if (inbuf.Length > 0) { n = int.Parse(inbuf); } else { n = 1; }

            str1 = new string[8][];
            for (int i = 0; i < 8; i++)
            { str1[i] = new string[(int)slogs1.Count]; }
            str2 = new string[8][];
            for (int i = 0; i < 8; i++)
            { str2[i] = new string[(int)slogs2.Count]; }


             slin1 = new int[8];
            foreach (DictionaryEntry slo in slogs1) 
            { 
                if (slo.Key.ToString().Length < 8) 
                 { str1[slo.Key.ToString().Length][slin1[slo.Key.ToString().Length]] = slo.Key.ToString(); slin1[slo.Key.ToString().Length]++; }
            }
            slin2 = new int[8];
            foreach (DictionaryEntry slo in slogs2)
            {
                if (slo.Key.ToString().Length < 8)
                { str2[slo.Key.ToString().Length][slin2[slo.Key.ToString().Length]] = slo.Key.ToString(); slin2[slo.Key.ToString().Length]++; }
            }
            while (true)
            {
                Console.Clear();
                Console.Write(GetWords(ns, n));
                Console.ReadLine();
            }

        }
    }
}
