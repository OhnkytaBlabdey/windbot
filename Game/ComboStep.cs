using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WindBot.Game;

namespace WindBot.Game
{

    class CardMsg
    {
        public int player;
        public int code;
        public int loc;
        public int seq;
        public CardMsg(int con, int id, int l, int s)
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
        None = 0,
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

        public CardInfo(int id2, byte player, byte loc2, byte seq2)
        {
            id = id2;
            controller = player;
            loc = loc2;
            seq = seq2;
        }
    }

    class StepVal
    {
        public int serial = -1; // 表示不会随着duel而改变的
        public CardInfo card = null;
        public int num = 0;
        public bool bin = true;
        //public Func<Duel, int> feature = delegate (Duel duel)
        //{
        //    return 0;
        //};

        public StepVal(int s, int num2, bool bin2, int id, byte con, byte loc, byte seq)
        {
            serial = s;
            num = num2;
            bin = bin2;
            card = new CardInfo(id, con, loc, seq);
        }

        public StepVal(int s, int id, byte con, byte loc, byte seq)
        {
            serial = s;
            num = 0;
            bin = true;
            card = new CardInfo(id, con, loc, seq);
        }

        public StepVal(int id, byte con, byte loc, byte seq)
        {
            serial = -1;
            num = 0;
            bin = true;
            card = new CardInfo(id, con, loc, seq);
        }
        //public StepVal(int num2, bool bin2, int id, byte con, byte loc, byte seq, Func<Duel, int> func)
        //{
        //    num = num2;
        //    bin = bin2;
        //    card = new CardInfo(id, con, loc, seq);
        //    feature = func;
        //}


    }

    class ComboStep
    {
        MsgCategory category;
        private List<StepVal> vals;
        private StepVal curval;

        public Func<Duel, bool> checkcon = delegate (Duel duel)
        {
            return true;
        };

        public ComboStep(MsgCategory category2, StepVal val)
        {
            category = category2;
            curval = val;
            vals = new List<StepVal>();
            vals.Add(val);
        }

        public StepVal GetCurVal()
        {
            if (curval != null)
                return curval;
            //if cur val is null
            return new StepVal(0, 2, 0, 0);
        }

        public void UpdateVals()
        {
            if (vals.Count == 0)
                return;
            //curval = vals.Dequeue();
            //由val的代号与duel状态得出val随着duel而改变后的正确值
        }

        public int GetValCount()
        {
            if (vals != null)
                return vals.Count;
            return -1;
        }

    }

}
