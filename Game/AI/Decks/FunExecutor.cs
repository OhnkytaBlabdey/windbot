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

            public enum State
            {
                Menu,
                Setting,
                Login,
                NameLogin,
                PasswordLogin,
                Logined,
                Exit,
                RepeatMode,
                Back,
                Restart,
                Pause,
                InGame,
                Save,
                Download
            }

            public enum Resp
            {
                No,
                EnterMenu,
                BackToMenu,
                EnterSetting,
                SetRepeat,
                RepeatOn,
                RepeatOff,
                EnterLogin,
                GetName,
                GetPw,
                Exit,
                Restart,
                Pause,
                Continue, // to InGame
                Save,
                Download
            }


            private bool FUDUJI = true;
            private List<string> sentences = new List<string>();
            private string buf;
            private int maxlength;
            private string user;
            private string pw;
            private string logfile="./logfile.txt";
            private List<string> reply = new List<string>();
            //private StateMan stateman;
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

            private void Talk(string s)
            {
                reply.Add(s);
            }

            public void Response(string message)
            {
                Resp resp = Resp.No;
                switch (state)
                {
                    case State.Setting:
                        switch (message)
                        {
                            case "back":
                                resp = Resp.BackToMenu;
                                break;
                            case "repeat":
                                resp = Resp.SetRepeat;
                                break;
                            default:
                                reply.Add("输入不合格式，请重新输入。");
                                resp = Resp.No;
                                break;
                        }
                        break;
                    case State.RepeatMode:
                            //
                            switch (message)
                            {
                                case "repeat:on":
                                    //FUDUJI = true;
                                    //state = State.Setting;
                                    resp = Resp.RepeatOn;
                                    break;
                                case "repeat:off":
                                    //FUDUJI = false;
                                    //state = State.Setting;
                                    resp = Resp.RepeatOff;
                                    break;
                                default:
                                    Talk("输入不合格式，请重新输入。");
                                    resp = Resp.SetRepeat;
                                    break;
                            }
                        break;
                    case State.Menu:
                        switch(message)
                        {
                            case "login":
                                //state = State.Login;
                                resp = Resp.EnterLogin;
                            //user login
                            //2 state
                            break;
                            case "setting":
                                //state = State.Setting;
                                resp = Resp.EnterSetting;
                                break;
                            case "exit":
                                //state = State.Exit;
                                resp = Resp.Exit;
                                break;
                            default:
                                Talk("输入不合格式，请重新输入。");
                                resp = Resp.EnterMenu;
                                break;
                        }
                        break;
                    case State.Login:
                        resp = Resp.EnterLogin;
                        break;
                    case State.NameLogin:
                        resp = Resp.GetName;
                        break;
                    case State.PasswordLogin:
                        resp = Resp.GetPw;
                        break;
                    //???
                    default:
                        switch (message)
                        {
                            default:
                                reply.Add("输入不合格式，请重新输入。");
                                resp = Resp.No;
                                break;
                        }
                        break;
                }
                //change state

                switch (resp)
                {
                    case Resp.GetName:
                        state = State.PasswordLogin;
                        string t=message.Substring(0,message.Length>12? 12: message.Length).ToLower();
                        foreach(char c in t)
                        {
                            if (c >= 'a'&& c <= 'z')
                            {
                                user = user + c;
                            }
                        }
                        logfile = "./[log][user]" + user + ".txt";
                        Talk("Input your Password");
                        break;
                    case Resp.GetPw:
                        state = State.Logined;
                        Talk("1. Restart");
                        Talk("2. Continue");
                        pw = message;
                        break;
                    case Resp.EnterLogin:
                        state = State.NameLogin;
                        Talk("\tLogin");
                        Talk("Input your Name");
                        Talk("(English letter and no longer than 12)");
                        break;
                    case Resp.BackToMenu:
                    case Resp.EnterMenu:
                        state = State.Menu;
                        Talk("\tMenu");
                        Talk("1. Options (setting)");
                        Talk("2. Login (login)");
                        Talk("3. Exit (exit)");
                        break;
                    case Resp.Exit:
                        state = State.Exit;
                        reply.Add("结束。");
                        break;
                    case Resp.RepeatOff:
                        state = State.Setting;
                        FUDUJI = false;
                        Talk("RepeatModeOff");
                        Talk("\tSetting:");
                        Talk("1. RepeatMode : " + (FUDUJI ? "On" : "Off") + " (repeat)");
                        Talk("BackToMenu(back)");
                        break;
                    case Resp.RepeatOn:
                        state = State.Setting;
                        FUDUJI = true;
                        Talk("RepeatModeOn");
                        Talk("\tSetting:");
                        Talk("1. RepeatMode : " + (FUDUJI ? "On" : "Off") + " (repeat)");
                        Talk("BackToMenu(back)");
                        break;
                    case Resp.EnterSetting:
                        state = State.Setting;
                        Talk("\tSetting:");
                        Talk("1. RepeatMode : " + (FUDUJI ? "On" : "Off") + " (repeat)");
                        Talk("BackToMenu(back)");
                        break;
                    case Resp.SetRepeat:
                        state = State.RepeatMode;
                        Talk("1. RepeatMode : " + (FUDUJI ? "On" : "Off") + " (repeat)");
                        Talk("(repeat:on/off)");
                        break;

                    case Resp.No:
                    default:
                        break;
                }
                //apply response
            }

            public ChatManage()
            {
                state = State.Menu;
                
                maxlength = 10;
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
