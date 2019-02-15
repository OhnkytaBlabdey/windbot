using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
namespace WindBot.Game.AI.Decks
{
    [Deck("SecondHand", "second_hand")]
    class SecondHand : Executor
    {
        SecondHand(GameAI ai, Duel duel)
            : base(ai, duel)
        {

        }
        public override bool OnSelectHand()
        {
            return false;
        }
        public override int OnRockPaperScissors()
        {
            return 2;
        }
    }
}
