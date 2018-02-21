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

        public bool SameTo(CardMsg cardmsg)
        {
            return (player == cardmsg.player && code == cardmsg.code && loc == cardmsg.loc && seq == cardmsg.seq);
        }
    }

    public enum MsgCategory
    {
        SelectCard = 1,
        SelectIdleCmd = 2,
        SelectChain = 3,
        SelectPlace = 4,
        SelectEffectYn = 5,
        SelectBattleCmd = 6
    }

    public enum InfoType
    {
        Card = 1,
        Num = 2,
        Bool = 3
    }

    class CardInfo
    {
        int id;
        byte controller;
        byte desc;
        delegate bool filter(Duel duel, int con, int code, int loc, int seq);

            public bool DefaultFilter (Duel duel, int con, int code, int loc, int seq)
            {
                return true;
            }
    }
    class ComboStep
    {
        MsgCategory category;
        Queue<Dictionary<InfoType, Object>> vals;
    }
    class GameCombo
    {
        Queue<ComboStep> steps;

    }
}
