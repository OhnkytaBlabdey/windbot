using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Fun", "AI_Fun")]
    class FunExecutor :DefaultExecutor
    {
        public class ComboManage
        {
            ComboManage()
            {
                ;
            }
        }
        private bool FUDUJI = true;
        public class CardId
        {
            public const int GrandMaster = 83039729;
            public const int Kizan = 49721904;
            public const int RoyalMagicianLibrary = 70791313;
        }

        public FunExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Set 
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Activate 
            AddExecutor(ExecutorType.Activate);
          
            AddExecutor(ExecutorType.SpSummon);

            // Set other monsters
            AddExecutor(ExecutorType.SummonOrSet);
            AddExecutor(ExecutorType.MonsterSet);

        }

        public override string OnChat(string message, int player)
        {
            //TODO:resume message
            //TODO:get result
            if (FUDUJI) return message;
            //goes by default.
            return null;
        }

        public override bool OnSelectHand()
        {
            return false;
        }
        
    }
}
