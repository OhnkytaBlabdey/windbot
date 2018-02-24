using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WindBot.Game;

namespace WindBot.Game
{
    class GameCombo
    {
        private Queue<ComboStep> steps;
        private ComboStep curstep;
        bool isable;
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
