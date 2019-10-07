using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime;

namespace WindBot.Game
{
    class ExternalsUtil
    {
        class Choser
        {
            private List<byte> chosen;
            private int n;
            private int ct;
            private int m_index;
            private bool exit;

            public Choser(int nn) {
                ct = 0; exit = false;n = nn;
                chosen = new List<byte>(n);
            }
            public Choser():this(0){}
            public void SetN(int n1) { n = n1; ct = 0; }
            
            private void Dump()
            {
                for(int i = 0; i < chosen.Count; ++i)
                {
                    Logger.WriteLine(i + " : " + chosen[i]);
                }
                //Logger.WriteLine(chosen.ToString());
                //printf("[ %2d ]\t", ct);
                //for (vector<int>::iterator it = chosen.begin(); it != chosen.end(); ++it) printf("%2d ", *it); putchar('\n');
            }

            public byte[] GetResult()
            {
                Logger.WriteLine("dump again.");
                Dump();
                byte[] res = new byte[chosen.Count + 1];
                res[0] = (byte)chosen.Count;
                Logger.WriteLine("get res count :" + chosen.Count);
                int ct = 0;
                foreach(byte i in chosen)
                {
                    res[++ct] = (byte)(i - 1);
                    Logger.WriteLine("add " + ct + " : " + i);
                }
                return res;
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
            private void Sel(byte x, int m, int M, bool isindex = false)
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
                x++;
                Sel(x, m, M, isindex);
                if (exit) return;
                x--;
                chosen.Add(x);
                //select x
                x++;
                Sel(x, m, M, isindex);
                if (exit) return;
                // restore
                x--;
                chosen.Remove(x);
            }
            public void Select(int m, int M)
            {
                Sel(1, m, M);
                ct = 0;
            }
        }
        static Process process;
        static Choser choser;
        static public void Init()
        {
            choser = new Choser();
            process = new Process();
            process.StartInfo.FileName = "ans.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            process.Start();
        }

        static public void Release()
        {
            if (!process.StandardError.EndOfStream)
            {
                string msg = process.StandardError.ReadToEnd();
                Logger.WriteLine(msg);
            }
            process.Close();
            choser.Reset();
        }
        static private int C(int n,int m)
        {
            int resu = 1, resd = 1;
            for(int i = 1; i <= m; ++i)
            {
                resu *= n + 1 - i;
                resd *= i;
            }
            return resu / resd;
        }
        static public byte[] SelectCard(int n,int m,int M)
        {
            int count = 0;
            for (int i = m; i <= M; i++) count += C(n, i);
            int index = Choose(count);
            choser.Reset();
            choser.SetN(n);
            choser.Select(m, M, index);
            byte[] res=choser.GetResult();
            return res;
        }
        static public int Choose(int count)
        {
            if (count == 0)
            {
                process.StandardInput.WriteLine(0);
                Release();
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
                if(ct>100)
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
        static public string getHttp(string url, int timeout)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null) return null;
                string text = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                return text;
            }catch (Exception ex)
            {
                Logger.WriteErrorLine(ex.Message);
                return null;
            }
        }
    }
}
