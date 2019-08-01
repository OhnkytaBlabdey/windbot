using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("FirstHand", "AI_FirstHand","Normal")]
    class FirstHand : DefaultExecutor
    {
        public FirstHand(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            GameBehavior.iscon = true;
        }
        public override int OnRockPaperScissors()
        {
            return 1;
        }
        public override bool OnSelectHand()
        {
            return true;
        }
    }
}
