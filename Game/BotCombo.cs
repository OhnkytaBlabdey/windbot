using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindBot.Game;

namespace WindBot.Game
{
    public enum ChoiceCategory
    {
        OnSelectcard = 1,
        OnSelectIdlecmd = 2,
        OnSelectChain = 3
    }

    public enum ObjectType
    {
        card = 1,
        bin = 2,
        number = 3
    }

    struct CardMsg
    {
        public int id;
        //card original code
        public int loc;
        //card location
        public int seq;
        //card zone index
        public int ispub;
        // is public or not
        public int player;
    }

    class StepObject
    {
        public ObjectType type;
        public int value;
        public CardMsg stepcard;
        public StepObject(ObjectType objectType, int val, int id_a, int loc_a, int seq_a, int ispub_a)
        {
            type = objectType;
            value = val;
            if(id_a != 0)
            {
                stepcard = new CardMsg
                {
                    id = id_a,
                    loc = loc_a,
                    seq = seq_a,
                    ispub = ispub_a
                };
            }
        }
    }

    class ComboStep
    {
        public ChoiceCategory category;
        //StepObject main;
        //StepObject other;
        public Queue<StepObject> objlist;

        public ComboStep(ChoiceCategory choiceCategory)
        {
            category = choiceCategory;
            objlist = new Queue<StepObject>();
        }

        public ComboStep(ChoiceCategory choiceCategory,StepObject obj)
        {
            category = choiceCategory;
            objlist = new Queue<StepObject>();
            objlist.Enqueue(obj);
        }
    }

    class BotCombo
    {
        public Queue<ComboStep> queue;
        public BotCombo()
        {
            queue = new Queue<ComboStep>();
        }
    }
}
