using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindBot.Game;

namespace WindBot.Game
{
    class CardMsg
    {
        public int player;
        public int code;
        public int loc;
        public int seq;
        public CardMsg(int con,int id,int l,int s)
        {
            player = con;
            code = id;
            loc = l;
            seq = s;
        }
    }
    class CardInfo
    {
        int id;
        byte controller;
        delegate bool filter(Duel duel, int con, int code, int loc, int seq);

            public bool DefaultFilter (Duel duel, int con, int code, int loc, int seq)
            {
                return true;
            }
    }
    class ComboStep
    {

    }
    class GameCombo
    {
    }
}
