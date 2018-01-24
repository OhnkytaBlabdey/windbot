using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("A", "AI_DeckA")]
    class ExecutorA : Executor
    {
        public class CardId
        {

        }

        public ExecutorA(GameAI ai, Duel duel)
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
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);

                Logger.DebugWriteLine("OnSelectCard MyBotA");
                //IList<ClientCard> result = new List<ClientCard>();
                //AI.Utils.CheckSelectCount(result, cards, min, max);
            //if(result!=null)
                //return result;
            Logger.DebugWriteLine("Use default.");
            return null;
        }

    }
}
