﻿using System;
using System.IO;
using System.Diagnostics;

namespace WindBot.Game
{
    class ExternalsUtil
    {
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
