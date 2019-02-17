using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindBot.Game
{
    class ExternalsUtil
    {
        static public int Choose(int count)
        {
            Console.WriteLine(count);
            int res = int.Parse(Console.ReadLine());
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
