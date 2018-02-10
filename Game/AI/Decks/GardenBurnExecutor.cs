using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("GB", "AI_GardenBurn")]
    class GardenBurnExecutor:Executor
    {
        public class CardId
        {

        }

        public GardenBurnExecutor (GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate);
            AddExecutor(ExecutorType.SpSummon);
            AddExecutor(ExecutorType.SpellSet);
            AddExecutor(ExecutorType.Summon);
            AddExecutor(ExecutorType.MonsterSet);
            AddExecutor(ExecutorType.Repos);
            AddExecutor(ExecutorType.SummonOrSet);
        }
        public override bool OnSelectHand()
        {
            return true;
        }
        public override int OnRockPaperScissors()
        {
            return 2;
        }

    }
}
