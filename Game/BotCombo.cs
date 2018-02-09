using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


    class StepObject
    {
        ObjectType type;
        int value;
    }

    class ComboStep
    {
        ChoiceCategory category;
        StepObject main;
        StepObject other;
        List<StepObject> list;
    }

    class BotCombo
    {
        List<ComboStep> List;
        public BotCombo()
        {
            List = new List<ComboStep>();
        }
    }
}
