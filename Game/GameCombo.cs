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
        byte controller = 0;
        byte loc = 0;
        byte seq = 0;
        int desc = 0;

        public CardInfo(int id2, byte player, int desc2)
        {
            id = id2;
            controller = player;
            desc = desc2;
        }

        public CardInfo(int id2, byte player, byte loc2,byte seq2)
        {
            id = id2;
            controller = player;
            loc = loc2;
            seq = seq2;
        }
    }

    class StepVal
    {
        public CardInfo card = null;
        public int num = 0;
        public bool bin = true;
        public Func<Duel,CardMsg,CardInfo,bool> filter= delegate (Duel duel, CardMsg msg, CardInfo card)
            {
                return true;
            };

        public StepVal(int num2 ,bool bin2,int id,byte con, byte loc, byte seq)
        {
            num = num2;
            bin = bin2;
            card = new CardInfo(id,con,loc,seq);
        }

        public StepVal(int id,byte con,byte loc,byte seq)
        {
            //num = 0;
            //bin = true;
            card = new CardInfo(id, con, loc, seq);
        }
    }

    class ComboStep
    {
        MsgCategory category;
        public Queue<StepVal> vals;
        public Func<Duel, bool> checkcon = delegate (Duel duel)
          {
              return true;
          };
    }

    class GameCombo
    {
        Queue<ComboStep> steps;
        bool isable;

    }

}
