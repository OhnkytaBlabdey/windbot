using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
//using cid =public const int;

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

        public class ChatManage
        {
            /// <summary>
            /// manage state
            /// </summary>
            public class State
            {


                public /*virtual*/ void Receive(string message)
                {
                    ;
                }

                public /*virtual*/ void Apply()
                {
                    ;
                }

                public State()
                {
                    ;
                }
            }


            private bool FUDUJI = true;
            private List<string> sentences = new List<string>();
            private string buf;
            private int maxlength;
            private string logfile="./logfile.txt";
            private List<string> cmdkeys = new List<string>();
            private List<string> reply = new List<string>();
            private State state;

            private void Save()
            {
                //StreamWriter sw = new StreamWriter(logfile);
                FileStream fs = new FileStream(logfile, FileMode.Append);
                foreach (string sen in sentences)
                {
                    //sw.Write(sen+' ');
                    byte[] data = Encoding.Default.GetBytes(sen + ' ');
                    fs.Write(data, 0, data.Length);
                }
                //sw.Flush();
                fs.Close();
            }

            public void Receive(string message)
            {
                buf=message;
                if (sentences.Count() >= maxlength)
                {
                    Save();
                    sentences.Clear();
                    //sentences.Add(message);
                }
                sentences.Add(message);

                //interactions
                Response(message);
            }

            public IList<string> GetResult()
            {
                //if (FUDUJI) return message;
                if (FUDUJI) {
                    //string[] res = { buf };
                    //return res;
                    reply.Add("您输入了【"+buf+"】。");
                }

                //
                if (reply.Count() > 0)
                {
                    string[] res = reply.ToArray();
                    //reply.CopyTo(res);
                    reply.Clear();
                    return res;
                }

                //default.
                return null;
            }

            public void Response(string message)
            {
                //change state
                if (message.Contains("repeat:"))
                {
                    //
                    switch (message)
                    {
                        case "repeat:on":
                            FUDUJI=true;
                            break;
                        case "repeat:off":
                            FUDUJI=false;
                            break;
                        default:
                            reply.Add("输入不合格式，请重新输入。");
                            break;
                    }
                    ;
                }
                else if(message.Contains("login"))
                {
                    //user login
                    //2 state
                    ;
                }
                else if (message.Contains("exit"))
                {
                    //exit
                    //call state
                    ;
                }
                //change state
                else
                {
                    //
                    ;
                }
            }

            public ChatManage()
            {
                maxlength = 10;
                cmdkeys.Add("repeat:");
                cmdkeys.Add("login");
                cmdkeys.Add("exit");
            }

            ~ChatManage()
            {
                Save();
            }
        }
        
        private ChatManage chat;

        public class CardId
        {
            //main
            public const int GrandMaster = 83039729;
            public const int Kizan = 49721904;
            public const int RoyalMagicianLibrary = 70791313;
            public const int Mizuho = 74094021;
            public const int Shinai = 48505422;
            public const int OneDayOfPeace = 33782437;
            public const int DoubleSummon = 43422537;
            public const int UpstartGoblin = 70368879;
            public const int CardDestruction = 72892473;
            public const int TerraForming = 73628505;
            public const int GoldenBambooSword = 74029853;
            public const int MonsterReborn = 83764718;
            public const int HandDestruction = 74519184;
            public const int GatewayOfTheSix = 27970830;
            public const int BrokenBambooSword = 41587307;
            public const int CursedBambooSword = 42199039;
            public const int TempleOfTheSix = 53819808;
            public const int ChickenGame = 67616300;
            public const int PseudoSpace = 77584012;

            //extra
            public const int DaiGustoEmeral = 581014;
            public const int ShadowOfTheSixShien = 1828513;
            public const int DiamondDireWolf = 95169481;
            public const int AbyssDweller = 21044178;
            public const int GagagaCowboy = 12014404;
        }

        public FunExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //chat
            chat = new ChatManage();

            // Set 
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);

            // Activate 
            AddExecutor(ExecutorType.Activate);
          
            AddExecutor(ExecutorType.SpSummon);

            // Set other monsters
            AddExecutor(ExecutorType.SummonOrSet);
            AddExecutor(ExecutorType.MonsterSet);

        }

        public override IList<string> OnChat(string message, int player)
        {
            //TODO:resume message
            chat.Receive(message);
            //TODO:get result
            IList<string> res = chat.GetResult();
            
            if (res != null) return res;
            //goes by default.
            return null;
        }

        public override bool OnSelectHand()
        {
            return false;
        }
        
    }
}
