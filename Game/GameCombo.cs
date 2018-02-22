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
        public int id;
        public byte controller = 0;
        public byte loc = 0;
        public byte seq = 0;
        public int desc = 0;

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
        private Queue<StepVal> vals;
        private StepVal curval;

        public Func<Duel, bool> checkcon = delegate (Duel duel)
          {
              return true;
          };

        public ComboStep(MsgCategory category2,StepVal val)
        {
            category = category2;
            curval = val;
            vals = new Queue<StepVal>();
            vals.Enqueue(val);
        }

        public StepVal GetCurVal()
        {
            if(curval!=null)
            return curval;
            //if cur val is null
            return new StepVal(0, 2, 0, 0);
        }

        public void UpdateVals()
        {
            if (vals.Count == 0)
                return;
            curval = vals.Dequeue();
        }

        public int GetValCount()
        {
            if(vals!=null)
            return vals.Count;
            return -1;
        }

    }

    class GameCombo
    {
        private Queue<ComboStep> steps;
        private ComboStep curstep;
        bool isable;

        public void LoadCombo(string deck)
        {
            switch(deck)
            {
                case "GB":
                    steps.Enqueue(new ComboStep(MsgCategory.SelectCard, new StepVal(9929398, 0, 0, 0)));
                    steps.Enqueue(new ComboStep(MsgCategory.SelectCard, new StepVal(9929399, 0, 0, 0)));
                    break;
            }

        }

        public ComboStep GetCurStep()
        {
            return curstep;
        }

        public void UpdateStep()
        {
            if (steps.Count == 0)
                return;
            curstep = steps.Dequeue();
        }

    }

}
