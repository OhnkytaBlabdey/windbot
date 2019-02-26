﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WindBot.Game
{
    class ExternalsUtil
    {
        class Choser
        {
            private List<int> chosen;
            private int n;
            private int ct;
            private int m_index;
            private bool exit;

            public Choser(int nn) {
                ct = 0; exit = false;n = nn;
            }
            public Choser() { n = 0; ct = 0; exit = false; }
            public void SetN(int n1) { n = n1; ct = 0; }
            
            void Dump()
            {
                Logger.WriteLine(chosen.ToString());
                //printf("[ %2d ]\t", ct);
                //for (vector<int>::iterator it = chosen.begin(); it != chosen.end(); ++it) printf("%2d ", *it); putchar('\n');
            }

            byte[] GetResult()
            {
                List<byte> res = new List<byte>();
                res.Add((byte)chosen.Count);
                foreach(int i in chosen)
                {
                    res.Add((byte)i);
                }
                return res.ToArray();
            }
            public void Reset()
            {
                chosen.Clear();
            }
            public void Select(int m, int M, int index)
            {
                m_index = index;
                Sel(1, m, M, true);
                exit = false;
                ct = 0;
            }
            private void Sel(int x, int m, int M, bool isindex = false)
            {
                if (exit || chosen.Count > M || (n + 1 - x + chosen.Count) < m) return;
                if (x == n + 1)
                {
                    ++ct;
                    if (!isindex)
                    {
                        Dump();
                    }
                    else if (ct == m_index)
                    {
                        Dump();
                        exit = true;
                    }
                    return;
                }
                //not select x
                Sel(x + 1, m, M, isindex);
                chosen.Add(x);
                //select x
                Sel(x + 1, m, M, isindex);
                // restore
                chosen.Remove(x);
            }
            public void Select(int m, int M)
            {
                Sel(1, m, M);
                ct = 0;
            }
        }
        static Process process;
        static public void init()
        {
            process = new Process();
            process.StartInfo.FileName = "ans.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            process.Start();
        }

        static public void release()
        {
            if (!process.StandardError.EndOfStream)
            {
                string msg = process.StandardError.ReadToEnd();
                Logger.WriteLine(msg);
            }
            process.Close();
        }
        static public int Choose(int count)
        {
            if (count == 0)
            {
                process.StandardInput.WriteLine(0);
                release();
                return 0;
            }
            if (count == 1) return 1;
            //Console.WriteLine(count);
            //Console.Out.Flush();
            try
            {
                process.StandardInput.WriteLine(count);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.StackTrace);
                throw (ex);
            }
            
            string str;
            int ct = 0;
            while (process.StandardOutput.EndOfStream)
            {
                ct++;
                if(ct>10)
                {
                    Console.WriteLine("fail to read");
                    throw new Exception("...");
                }
            }
            str = process.StandardOutput.ReadLine();
            int res = int.Parse(str);
            
            Logger.WriteLine("resp " + res);
            return res;
        }
        static public int Choose(int signature,int count)
        {
            int res = 0;
            Console.WriteLine(signature+", "+count);
            int choice = int.Parse(Console.ReadLine());
            res = choice;
            return res;
        }
    }
}
