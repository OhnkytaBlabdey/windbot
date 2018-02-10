using System;
using System.Collections.Generic;
using System.IO;
using WindBot.Game.AI;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Threading;
using System.Diagnostics;

namespace WindBot.Game
{
    public class GameBehavior
    {
        public GameClient Game { get; private set; }
        public YGOClient Connection { get; private set; }
        public Deck Deck { get; private set; }

        private GameAI _ai;
        private BotCombo _combo;
        private bool ison;

        private IDictionary<StocMessage, Action<BinaryReader>> _packets;
        private IDictionary<GameMessage, Action<BinaryReader>> _messages;

        private Room _room;
        private Duel _duel;
        private int _hand;
        private int _select_hint;

        public GameBehavior(GameClient game)
        {
            Game = game;
            Connection = game.Connection;
            _hand = game.Hand;

            _packets = new Dictionary<StocMessage, Action<BinaryReader>>();
            _messages = new Dictionary<GameMessage, Action<BinaryReader>>();
            RegisterPackets();

            _room = new Room();
            _duel = new Duel();

            _ai = new GameAI(Game, _duel);
            _ai.Executor = DecksManager.Instantiate(_ai, _duel);
            Deck = Deck.Load(_ai.Executor.Deck);

            _select_hint = 0;

            _combo = new BotCombo();
            LoadCombo();
            ison = true;
        }


        private void LoadCombo()
        {
            
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 9929398, (int)CardLocation.Hand, 0, 0)));//lv5
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 3, 0, 0, 0, 0)));//lv5
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectEffectYesNo, new StepObject(ObjectType.bin, 1, 0, 0, 0, 0)));//trigger effect lv5
            
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 4, 0, 0, 0, 0)));//token1
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//token2
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.Activate, 71645242, (int)CardLocation.Hand, 0, 0, 0)));//field
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 50588353, (int)CardLocation.Extra, 0, 0, 0)));//link2
            
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 9929398, (int)CardLocation.MonsterZone, 3, 1)));//select lv5 tuner as link material
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 9929399, (int)CardLocation.MonsterZone, 2, 1)));//select token1 as link material
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 6, 0, 0, 0, 0)));//link2
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 1, 50588353, (int)CardLocation.MonsterZone, 6, 1)));//chain link2
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 72291078, (int)CardLocation.Deck, 0, 0)));//select tuner
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//tuner

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 0, 0, 0, 0, 0)));//oppo1
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 1, 0, 0, 0, 0)));//oppo2

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 61665245, (int)CardLocation.Extra, 0, 0, 0)));//link3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 50588353, (int)CardLocation.MonsterZone, 6, 1)));//select link2 as link material
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 72291078, (int)CardLocation.MonsterZone, 2, 1)));//select tuner2 tuner as link material
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 6, 0, 0, 0, 0)));//link3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 1, 72291078, (int)CardLocation.Grave, 0, 1)));
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 0, 0, 0, 0, 0)));//token3

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//oppo3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 3, 0, 0, 0, 0)));//oppo4

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.Activate, 61665245, (int)CardLocation.MonsterZone, 6, 0, 0)));//act link3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 9929399, (int)CardLocation.MonsterZone, 4, 1)));//select 
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 10875327, (int)CardLocation.Deck, 0, 0)));//select 

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//place lv10
            
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 4, 0, 0, 0, 0)));//oppo5

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 22862454, (int)CardLocation.Extra, 0, 0, 0)));//spsm link2
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 72291079, (int)CardLocation.MonsterZone, 0, 0)));//select token3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 10875327, (int)CardLocation.MonsterZone, 2, 0)));//select lv10
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//place link2

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 1, 71645242, 0, 0, 1))); //chain field first
            
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 0, 0, 0, 0, 0)));//oppo1

            ////
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 5043010, (int)CardLocation.Extra, 0, 0, 0)));//spsm link4
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 61665245, (int)CardLocation.MonsterZone, 6, 0)));//select link3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 9929399, (int)CardLocation.MonsterZone, 4, 0)));//select token2

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectYesNo, new StepObject(ObjectType.bin, 0, 0, 0, 0, 0)));//not select more material

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 3, 0, 0, 0, 0)));//place link4: 3

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 0, 0, 0, 0, 0))); //no chain

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 1, 0, 0, 0, 0)));//oppo2

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 0, 0, 0, 0, 0))); //no chain

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.Activate, 5043010, (int)CardLocation.MonsterZone, 3,1,0)));//activate link4
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 10875327, (int)CardLocation.Grave, 0, 0)));//select lv10 and return it to hand

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 2220237, (int)CardLocation.Extra, 0, 0)));//spsm 
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 22862454, (int)CardLocation.MonsterZone, 2, 0)));//select link material

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//place link2:2

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 1, 5043010,(int)CardLocation.MonsterZone, 3, 1)));//chain link4
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 10875327, (int)CardLocation.Hand, 0, 0)));//select lv10 and spsm it

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 4, 0, 0, 0, 0)));//place lv10:4

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 2, 0, 0, 0, 0)));//oppo3
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 3, 0, 0, 0, 0)));//oppo4

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectIdleCmd, new StepObject(ObjectType.card, (int)MainPhaseAction.MainAction.SpSummon, 99111753, (int)CardLocation.Extra, 0, 0, 0)));//spsm 
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 5043010, (int)CardLocation.MonsterZone, 3, 0)));//select link material
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectCard, new StepObject(ObjectType.card, 0, 10875327, (int)CardLocation.MonsterZone, 4, 0)));//select link material

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 6, 0, 0, 0, 0)));//place :6
            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectChain, new StepObject(ObjectType.card, 1, 10875327, (int)CardLocation.Grave, 0, 1)));//chain lv10

            _combo.queue.Enqueue(new ComboStep(ChoiceCategory.OnSelectPlace, new StepObject(ObjectType.number, 4, 0, 0, 0, 0)));//oppo5


        }

        private void InitCombo()
        {
            //if every key card has got enough count, then say that card materials are okay. 
            //if opponent's lp is in a appropriate range and so on, then say that ... is okay. 
            if (ison)
                return;

            bool cards = CardCondition();
            bool con = _duel.LifePoints[1] <= 8000;
            if (cards && con)
                ison = true;
            if(ison)
            {
                _combo.queue.Clear();
                LoadCombo();
            }
        }

        private bool CardCondition()
        {
            int pl = 0;
            int ct = _duel.Fields[pl].Hand.GetCardCount(9929398) + _duel.Fields[pl].Hand.GetCardCount(71645242);
            if (ct < 2)
                return false;
            ct = _duel.Fields[pl].GetMonsterCount();
            if (ct > 0)
                return false;
            if ((_duel.Fields[pl].Graveyard.GetCardCount(10875327) + _duel.Fields[pl].Banished.GetCardCount(10875327) + _duel.Fields[pl].MonsterZone.GetCardCount(10875327) + _duel.Fields[pl].SpellZone.GetCardCount(10875327)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(72291078) + _duel.Fields[pl].Banished.GetCardCount(72291078) + _duel.Fields[pl].MonsterZone.GetCardCount(72291078) + _duel.Fields[pl].SpellZone.GetCardCount(72291078)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(5043010) + _duel.Fields[pl].Banished.GetCardCount(5043010) + _duel.Fields[pl].MonsterZone.GetCardCount(5043010) + _duel.Fields[pl].SpellZone.GetCardCount(5043010) + 2) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(61665245) + _duel.Fields[pl].Banished.GetCardCount(61665245) + _duel.Fields[pl].MonsterZone.GetCardCount(61665245) + _duel.Fields[pl].SpellZone.GetCardCount(61665245)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(50588353) + _duel.Fields[pl].Banished.GetCardCount(50588353) + _duel.Fields[pl].MonsterZone.GetCardCount(50588353) + _duel.Fields[pl].SpellZone.GetCardCount(50588353)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(22862454) + _duel.Fields[pl].Banished.GetCardCount(22862454) + _duel.Fields[pl].MonsterZone.GetCardCount(22862454) + _duel.Fields[pl].SpellZone.GetCardCount(22862454)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(99111753) + _duel.Fields[pl].Banished.GetCardCount(99111753) + _duel.Fields[pl].MonsterZone.GetCardCount(99111753) + _duel.Fields[pl].SpellZone.GetCardCount(99111753)) == 3 ||
                (_duel.Fields[pl].Graveyard.GetCardCount(2220237) + _duel.Fields[pl].Banished.GetCardCount(2220237) + _duel.Fields[pl].MonsterZone.GetCardCount(2220237) + _duel.Fields[pl].SpellZone.GetCardCount(2220237)) == 3)
                return false;

            return true;
        }

        public int GetLocalPlayer(int player)
        {
            return _duel.IsFirst ? player : 1 - player;
        }

        public void OnPacket(BinaryReader packet)
        {
            StocMessage id = (StocMessage)packet.ReadByte();
            if(!ison)
            {
                InitCombo();
            }
            if (id == StocMessage.GameMsg)
            {
                GameMessage msg = (GameMessage)packet.ReadByte();
                if (_messages.ContainsKey(msg))
                    _messages[msg](packet);
                Console.WriteLine("{\"msg\":\"" + msg + "\"},");
                return;
            }
            if (_packets.ContainsKey(id))
                _packets[id](packet);
            Console.WriteLine("{\"id\":\"" + id + "\"},");
        }

        private void RegisterPackets()
        {
            _packets.Add(StocMessage.JoinGame, OnJoinGame);
            _packets.Add(StocMessage.TypeChange, OnTypeChange);
            _packets.Add(StocMessage.HsPlayerEnter, OnPlayerEnter);
            _packets.Add(StocMessage.HsPlayerChange, OnPlayerChange);
            _packets.Add(StocMessage.SelectHand, OnSelectHand);
            _packets.Add(StocMessage.SelectTp, OnSelectTp);
            _packets.Add(StocMessage.TimeLimit, OnTimeLimit);
            _packets.Add(StocMessage.Replay, OnReplay);
            _packets.Add(StocMessage.DuelEnd, OnDuelEnd);
            _packets.Add(StocMessage.Chat, OnChat);
            _packets.Add(StocMessage.ChangeSide, OnChangeSide);
            _packets.Add(StocMessage.ErrorMsg, OnErrorMsg);

            _messages.Add(GameMessage.Retry, OnRetry);
            _messages.Add(GameMessage.Start, OnStart);
            _messages.Add(GameMessage.Hint, OnHint);
            _messages.Add(GameMessage.Win, OnWin);
            _messages.Add(GameMessage.Draw, OnDraw);
            _messages.Add(GameMessage.ShuffleDeck, OnShuffleDeck);
            _messages.Add(GameMessage.ShuffleHand, OnShuffleHand);
            _messages.Add(GameMessage.ShuffleExtra, OnShuffleExtra);
            _messages.Add(GameMessage.TagSwap, OnTagSwap);
            _messages.Add(GameMessage.NewTurn, OnNewTurn);
            _messages.Add(GameMessage.NewPhase, OnNewPhase);
            _messages.Add(GameMessage.Damage, OnDamage);
            _messages.Add(GameMessage.PayLpCost, OnDamage);
            _messages.Add(GameMessage.Recover, OnRecover);
            _messages.Add(GameMessage.LpUpdate, OnLpUpdate);
            _messages.Add(GameMessage.Move, OnMove);
            _messages.Add(GameMessage.Attack, OnAttack);
            _messages.Add(GameMessage.PosChange, OnPosChange);
            _messages.Add(GameMessage.Chaining, OnChaining);
            _messages.Add(GameMessage.ChainEnd, OnChainEnd);
            
            _messages.Add(GameMessage.UpdateCard, OnUpdateCard);
            _messages.Add(GameMessage.UpdateData, OnUpdateData);
            _messages.Add(GameMessage.BecomeTarget, OnBecomeTarget);

            _messages.Add(GameMessage.SortCard, OnCardSorting);
            _messages.Add(GameMessage.SortChain, OnChainSorting);

            _messages.Add(GameMessage.SelectBattleCmd, OnSelectBattleCmd);
            _messages.Add(GameMessage.SelectCard, OnSelectCard);
            _messages.Add(GameMessage.SelectChain, OnSelectChain);
            _messages.Add(GameMessage.SelectCounter, OnSelectCounter);
            _messages.Add(GameMessage.SelectDisfield, OnSelectDisfield);
            _messages.Add(GameMessage.SelectEffectYn, OnSelectEffectYn);
            _messages.Add(GameMessage.SelectIdleCmd, OnSelectIdleCmd);
            _messages.Add(GameMessage.SelectOption, OnSelectOption);
            _messages.Add(GameMessage.SelectPlace, OnSelectPlace);
            _messages.Add(GameMessage.SelectPosition, OnSelectPosition);
            _messages.Add(GameMessage.SelectSum, OnSelectSum);
            _messages.Add(GameMessage.SelectTribute, OnSelectTribute);
            _messages.Add(GameMessage.SelectYesNo, OnSelectYesNo);
            _messages.Add(GameMessage.AnnounceAttrib, OnAnnounceAttrib);
            _messages.Add(GameMessage.AnnounceCard, OnAnnounceCard);
            _messages.Add(GameMessage.AnnounceNumber, OnAnnounceNumber);
            _messages.Add(GameMessage.AnnounceRace, OnAnnounceRace);
            _messages.Add(GameMessage.AnnounceCardFilter, OnAnnounceCard);
            _messages.Add(GameMessage.RockPaperScissors, OnRockPaperScissors);

            _messages.Add(GameMessage.SpSummoning, OnSpSummon);
            _messages.Add(GameMessage.SpSummoned, OnSpSummon);
        }

        private void OnJoinGame(BinaryReader packet)
        {
            /*int lflist = (int)*/ packet.ReadUInt32();
            /*int rule = */ packet.ReadByte();
            /*int mode = */ packet.ReadByte();
            int duel_rule = packet.ReadByte();
            _ai.Duel.IsNewRule = (duel_rule == 4);
            BinaryWriter deck = GamePacketFactory.Create(CtosMessage.UpdateDeck);
            deck.Write(Deck.Cards.Count + Deck.ExtraCards.Count);
            deck.Write(Deck.SideCards.Count);
            foreach (NamedCard card in Deck.Cards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.ExtraCards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.SideCards)
                deck.Write(card.Id);
            Connection.Send(deck);
            _ai.OnJoinGame();
        }

        private void OnChangeSide(BinaryReader packet)
        {
            BinaryWriter deck = GamePacketFactory.Create(CtosMessage.UpdateDeck);
            deck.Write(Deck.Cards.Count + Deck.ExtraCards.Count);
            deck.Write(Deck.SideCards.Count);
            foreach (NamedCard card in Deck.Cards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.ExtraCards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.SideCards)
                deck.Write(card.Id);
            Connection.Send(deck);
            _ai.OnJoinGame();
        }

        private void OnTypeChange(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int pos = type & 0xF;
            if (pos < 0 || pos > 3)
            {
                Connection.Close();
                return;
            }
            _room.Position = pos;
            _room.IsHost = ((type >> 4) & 0xF) != 0;
            _room.IsReady[pos] = true;
            Connection.Send(CtosMessage.HsReady);
        }

        private void OnPlayerEnter(BinaryReader packet)
        {
            string name = packet.ReadUnicode(20);
            int pos = packet.ReadByte();
            if (pos < 8)
                _room.Names[pos] = name;
        }

        private void OnPlayerChange(BinaryReader packet)
        {
            int change = packet.ReadByte();
            int pos = (change >> 4) & 0xF;
            int state = change & 0xF;
            if (pos > 3)
                return;
            if (state < 8)
            {
                string oldname = _room.Names[pos];
                _room.Names[pos] = null;
                _room.Names[state] = oldname;
                _room.IsReady[pos] = false;
                _room.IsReady[state] = false;
            }
            else if (state == (int)PlayerChange.Ready)
                _room.IsReady[pos] = true;
            else if (state == (int)PlayerChange.NotReady)
                _room.IsReady[pos] = false;
            else if (state == (int)PlayerChange.Leave || state == (int)PlayerChange.Observe)
            {
                _room.IsReady[pos] = false;
                _room.Names[pos] = null;
            }

            if (_room.IsHost && _room.IsReady[0] && _room.IsReady[1])
                Connection.Send(CtosMessage.HsStart);
        }

        private void OnSelectHand(BinaryReader packet)
        {
            int result;
            if (_hand > 0)
                result = _hand;
            else
                result = _ai.OnRockPaperScissors();
            Connection.Send(CtosMessage.HandResult, (byte)result);
        }

        private void OnSelectTp(BinaryReader packet)
        {
            bool start = _ai.OnSelectHand();
            Connection.Send(CtosMessage.TpResult, (byte)(start ? 1 : 0));
        }

        private void OnTimeLimit(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            if (player == 0)
                Connection.Send(CtosMessage.TimeConfirm);
        }

        private void OnReplay(BinaryReader packet)
        {
            /*byte[] replay =*/ packet.ReadToEnd();

            /*
            const string directory = "Replays";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
            string file = DateTime.Now.ToString("yyyy-MM-dd.HH-mm.") + otherName + ".yrp";
            string fullname = Path.Combine(directory, file);

            if (Regex.IsMatch(file, @"^[\w\-. ]+$"))
                File.WriteAllBytes(fullname, replay);
            */

            //Connection.Close();
        }
        
        private void OnDuelEnd(BinaryReader packet)
        {
            //EndNote();
            Connection.Close();
        }

        private void EndNote()
        {
            System.IO.File.Move("./logs/q.txt", "./logs/q" + System.DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + ".txt");
            File.Move("./logs/a.txt", "./logs/a" + DateTime.Now.ToString("yy-MM-dd HH-mm-ss") + ".txt");
        }

        private void OnChat(BinaryReader packet)
        {
            int player = packet.ReadInt16();
            string message = packet.ReadUnicode(256);
            string myName = _room.Position == 0 ? _room.Names[0] : _room.Names[1];
            string otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
            if (player < 4)
                Logger.DebugWriteLine(otherName + " say to " + myName + ": " + message);
        }

        private void OnErrorMsg(BinaryReader packet)
        {
            int msg = packet.ReadByte();
            // align
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            int pcode = packet.ReadInt32();
            if (msg == 2) //ERRMSG_DECKERROR
            {
                int code = pcode & 0xFFFFFFF;
                int flag = pcode >> 28;
                if (flag <= 5) //DECKERROR_CARDCOUNT
                {
                    NamedCard card = NamedCard.Get(code);
                    if (card != null)
                        _ai.OnDeckError(card.Name);
                    else
                        _ai.OnDeckError("Unknown Card");
                }
                else
                    _ai.OnDeckError("DECK");
            }
            //Connection.Close();
        }

        private void OnRetry(BinaryReader packet)
        {
            _ai.OnRetry();
            Connection.Close();
            throw new Exception("Got MSG_RETRY.");
        }

        private void OnHint(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int player = packet.ReadByte();
            int data = packet.ReadInt32();
            if (type == 3) // HINT_SELECTMSG
            {
                _select_hint = data;
            }
        }

        private void OnStart(BinaryReader packet)
        {
            int type = packet.ReadByte();
            _duel.IsFirst = (type & 0xF) == 0;
            _duel.LifePoints[GetLocalPlayer(0)] = packet.ReadInt32();
            _duel.LifePoints[GetLocalPlayer(1)] = packet.ReadInt32();
            int deck = packet.ReadInt16();
            int extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(0)].Init(deck, extra);
            deck = packet.ReadInt16();
            extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(1)].Init(deck, extra);

            //EndNote();
            //StartNote();
            Logger.DebugWriteLine("Duel started: " + _room.Names[0] + " versus " + _room.Names[1]);
            _ai.OnStart();
        }

        private void StartNote()
        {
            FileStream file = new FileStream("./logs/q.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(file);
            writer.Write("0");
            writer.Close();
            file.Close();
            FileStream file2 = new FileStream("./logs/a.txt", FileMode.Create);
            file2.Close();
            Process choicemaker = new Process();
            try
            {
                choicemaker.StartInfo.UseShellExecute = false;
                choicemaker.StartInfo.FileName = "runscript";
                choicemaker.StartInfo.CreateNoWindow = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //mode = false;
            }

            choicemaker.Start();
            choicemaker.WaitForExit(1000);
            if (!choicemaker.HasExited)
            {
                //mode = false;
                try
                {
                    choicemaker.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + e.Message);
                }
            }
        }

        private void OnWin(BinaryReader packet)
        {
            int result = GetLocalPlayer(packet.ReadByte());

            string otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
            string textResult = (result == 2 ? "Draw" : result == 0 ? "Win" : "Lose");
            Logger.DebugWriteLine("Duel finished against " + otherName + ", result: " + textResult);

            Console.WriteLine("{\"duel_result\":\"" + textResult + "\"},");
        }

        private void OnDraw(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int count = packet.ReadByte();

            for (int i = 0; i < count; ++i)
            {
                _duel.Fields[player].Deck.RemoveAt(_duel.Fields[player].Deck.Count - 1);
                _duel.Fields[player].Hand.Add(new ClientCard(0, CardLocation.Hand));
            }
        }

        private void OnShuffleDeck(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            foreach (ClientCard card in _duel.Fields[player].Deck)
                card.SetId(0);
        }

        private void OnShuffleHand(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            packet.ReadByte();
            foreach (ClientCard card in _duel.Fields[player].Hand)
                card.SetId(packet.ReadInt32());
        }

        private void OnShuffleExtra(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            packet.ReadByte();
            foreach (ClientCard card in _duel.Fields[player].ExtraDeck)
            {
                if (!card.IsFaceup())
                    card.SetId(packet.ReadInt32());
            }
        }

        private void OnTagSwap(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int mcount = packet.ReadByte();
            int ecount = packet.ReadByte();
            /*int pcount = */ packet.ReadByte();
            int hcount = packet.ReadByte();
            /*int topcode =*/ packet.ReadInt32();
            _duel.Fields[player].Deck.Clear();
            for (int i = 0; i < mcount; ++i)
            {
                _duel.Fields[player].Deck.Add(new ClientCard(0, CardLocation.Deck));
            }
            _duel.Fields[player].ExtraDeck.Clear();
            for (int i = 0; i < ecount; ++i)
            {
                int code = packet.ReadInt32() & 0x7fffffff;
                _duel.Fields[player].ExtraDeck.Add(new ClientCard(code, CardLocation.Extra));
            }
            _duel.Fields[player].Hand.Clear();
            for (int i = 0; i < hcount; ++i)
            {
                int code = packet.ReadInt32();
                _duel.Fields[player].Hand.Add(new ClientCard(code, CardLocation.Hand));
            }
        }

        private void OnNewTurn(BinaryReader packet)
        {
            _duel.Turn++;
            _duel.Player = GetLocalPlayer(packet.ReadByte());
            _ai.OnNewTurn();
        }

        private void OnNewPhase(BinaryReader packet)
        {
            _duel.Phase = (DuelPhase)packet.ReadInt16();
            _ai.OnNewPhase();
        }

        private void OnDamage(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int final = _duel.LifePoints[player] - packet.ReadInt32();
            if (final < 0) final = 0;
            _duel.LifePoints[player] = final;
        }

        private void OnRecover(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            _duel.LifePoints[player] += packet.ReadInt32();
        }

        private void OnLpUpdate(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            _duel.LifePoints[player] = packet.ReadInt32();
        }

        private void OnMove(BinaryReader packet)
        {
            int cardId = packet.ReadInt32(); //cardid
            int pc = GetLocalPlayer(packet.ReadByte()); //pre controler
            int pl = packet.ReadByte(); //pre location
            int ps = packet.ReadSByte(); //pre sequence
            packet.ReadSByte(); // pre position
            int cc = GetLocalPlayer(packet.ReadByte()); //cur player
            int cl = packet.ReadByte(); //cur location
            int cs = packet.ReadSByte(); //cur sequence
            int cp = packet.ReadSByte(); //cur position
            packet.ReadInt32(); // reason

            ClientCard card = _duel.GetCard(pc, (CardLocation)pl, ps);

            if ((pl & (int)CardLocation.Overlay) != 0)
            {
                pl = pl & 0x7f;
                card = _duel.GetCard(pc, (CardLocation)pl, ps);
                if (card != null)
                    card.Overlays.Remove(cardId);
            }
            else
                _duel.RemoveCard((CardLocation)pl, card, pc, ps);
            //del

            if ((cl & (int)CardLocation.Overlay) != 0)
            {
                cl = cl & 0x7f;
                card = _duel.GetCard(cc, (CardLocation)cl, cs);
                if (card != null)
                    card.Overlays.Add(cardId);
            }
            else
            {
                _duel.AddCard((CardLocation)cl, cardId, cc, cs, cp); //新的card实例
                if ((pl & (int)CardLocation.Overlay) == 0 && card != null)
                {
                    ClientCard newcard = _duel.GetCard(cc, (CardLocation)cl, cs);
                    if (newcard != null)
                        newcard.Overlays.AddRange(card.Overlays);
                }
            }
            //add
        }

        private void OnAttack(BinaryReader packet)
        {
            int ca = GetLocalPlayer(packet.ReadByte());
            int la = packet.ReadByte();
            int sa = packet.ReadByte();
            packet.ReadByte(); //
            packet.ReadByte(); // cd
            int ld = packet.ReadByte();
            packet.ReadByte(); // sd
            packet.ReadByte(); //

            ClientCard attackcard = _duel.GetCard(ca, (CardLocation)la, sa);
            if (ld == 0 && (attackcard != null) && (ca != 0))
            {
                _ai.OnDirectAttack(attackcard);
            }
        }

        private void OnPosChange(BinaryReader packet)
        {
            packet.ReadInt32(); // card id
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            packet.ReadSByte(); // pp
            int cp = packet.ReadSByte();
            ClientCard card = _duel.GetCard(pc, (CardLocation)pl, ps);
            if (card != null)
                card.Position = cp;
        }

        private void OnChaining(BinaryReader packet)
        {
            packet.ReadInt32(); // card id
            int pcc = GetLocalPlayer(packet.ReadByte()); //player
            int pcl = packet.ReadByte(); //location
            int pcs = packet.ReadSByte(); //sequence
            int subs = packet.ReadSByte(); //sub index (for overlay materials )
            ClientCard card = _duel.GetCard(pcc, pcl, pcs, subs); //card
            int cc = GetLocalPlayer(packet.ReadByte()); //player
            _ai.OnChaining(card, cc);
            _duel.ChainTargets.Clear();
        }

        private void OnChainEnd(BinaryReader packet)
        {
            _ai.OnChainEnd();
            //_duel.ChainTargets.Clear();
        }

        private void OnCardSorting(BinaryReader packet)
        {
            /*BinaryWriter writer =*/ GamePacketFactory.Create(CtosMessage.Response);
            Connection.Send(CtosMessage.Response, -1);
        }

        private void OnChainSorting(BinaryReader packet)
        {
            /*BinaryWriter writer =*/ GamePacketFactory.Create(CtosMessage.Response);
            Connection.Send(CtosMessage.Response, -1);
        }

        private void OnUpdateCard(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int loc = packet.ReadByte();
            int seq = packet.ReadByte();

            packet.ReadInt32(); // ???

            ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
            if (card == null) return;
            
            card.Update(packet, _duel); //无需改动
            //card.Sequence = seq;
        }

        private void OnUpdateData(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();
            IList<ClientCard> cards = null;
            switch (loc)
            {
                case CardLocation.Hand:
                    cards = _duel.Fields[player].Hand;
                    break;
                case CardLocation.MonsterZone:
                    cards = _duel.Fields[player].MonsterZone;
                    break;
                case CardLocation.SpellZone:
                    cards = _duel.Fields[player].SpellZone;
                    break;
                case CardLocation.Grave:
                    cards = _duel.Fields[player].Graveyard;
                    break;
                case CardLocation.Removed:
                    cards = _duel.Fields[player].Banished;
                    break;
                case CardLocation.Deck:
                    cards = _duel.Fields[player].Deck;
                    break;
                case CardLocation.Extra:
                    cards = _duel.Fields[player].ExtraDeck;
                    break;
            }
            if (cards != null)
            {
                foreach (ClientCard card in cards)
                {
                    int len = packet.ReadInt32();
                    long pos = packet.BaseStream.Position;
                    if (len > 8)
                      card.Update(packet, _duel);
                    packet.BaseStream.Position = pos + len - 4;
                }
            }
        }

        private void OnBecomeTarget(BinaryReader packet)
        {
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                /*int sseq = */packet.ReadByte();
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (card == null) continue;
                _duel.ChainTargets.Add(card);
            }
        }

        private void OnSelectBattleCmd(BinaryReader packet)
        {
            packet.ReadByte(); // player
            _duel.BattlePhase = new BattlePhase();
            BattlePhase battle = _duel.BattlePhase;

            int count = packet.ReadByte(); //activate
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte()); //player
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[0] = i; //index
                    battle.ActivableCards.Add(card);
                    battle.ActivableDescs.Add(desc);
                }  //activateable cards
            }

            count = packet.ReadByte(); //attack
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte()); //player
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int diratt = packet.ReadByte();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[1] = i; //index
                    if (diratt > 0)
                        card.CanDirectAttack = true;
                    else
                        card.CanDirectAttack = false; //can direct attack
                    battle.AttackableCards.Add(card);
                    card.Attacked = false;
                }
            }
            List<ClientCard> monsters = _duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!battle.AttackableCards.Contains(monster))
                    monster.Attacked = true; // cannot attack
            }

            battle.CanMainPhaseTwo = packet.ReadByte() != 0;
            battle.CanEndPhase = packet.ReadByte() != 0;

            Connection.Send(CtosMessage.Response, _ai.OnSelectBattleCmd(battle).ToValue());
            //不影响同步的操作
        }

        private void InternalOnSelectCard(BinaryReader packet, Func<IList<ClientCard>, int, int, int, bool, IList<ClientCard>> func)
        {
            //Logger.DebugWriteLine("this is in GameBehavier.InternalOnSelectCard");
            packet.ReadByte(); // player
            bool cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            Console.WriteLine("\"choices\":{" + "\"category\":\"OnSelectCard\",");

            int index1 = -1;
            ComboStep step = null;
            StepObject obj = null;
            int ida = 0, loca = 0, seqa = 0, ispuba = 0;
            if (_combo.queue.Count > 0)
            {
                step = _combo.queue.Dequeue();
            }

            if (step == null || step.category != ChoiceCategory.OnSelectCard)
            {
                ison = false;
                ReEnq(step);
            }
            bool flag = false;
            if (ison)
            {
                obj = step.objlist.Dequeue();
                ida = obj.stepcard.id;
                loca = obj.stepcard.loc;
                seqa = obj.stepcard.seq;
                ispuba = obj.stepcard.ispub;
            }


            Console.WriteLine("\"cancelable\":\"" + cancelable + "\",\"min\":" + min + ",\"max\":" + max + ",");
            Console.WriteLine("\"list\":[");

            IList<ClientCard> selected = new List<ClientCard>();

            IList<ClientCard> cards = new List<ClientCard>();
            int count = packet.ReadByte();
            //Int64 minid = 100000000;
            //int minindex = 0;
            //ClientCard minc = null;
            //int myindex = 0;
            //byte[] myres = new byte[count + 1];
            byte[] myres = new byte[2];
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                //local player
                CardLocation loc = (CardLocation)packet.ReadByte();
                int nloc = (int)loc;
                int seq = packet.ReadByte();
                int pos = packet.ReadByte(); // pos

                Console.WriteLine("{");
                Console.WriteLine("\"id\": " + id + ",\"player\": " + player + ",\"loc\": \"" + (int)loc + "\",\"seq\": " + seq + ",\"pos\": " + pos + ",");

                ClientCard card;
                if (((int)loc & (int)CardLocation.Overlay) != 0)
                    card = new ClientCard(id, CardLocation.Overlay);
                else
                    card = _duel.GetCard(player, loc, seq);
                if (card == null) continue;
                if (card.Id == 0)
                    card.SetId(id);
                cards.Add(card);

                Console.Write("\"card\":");
                card.Show();
                Console.WriteLine("},");

                //
                if (ison)
                {
                    if ( id == ida && (loca == (int)loc || loca == 0))
                    {
                        if (seqa == seq || seqa == 0 || loca != 4 && loca != 8)
                        { 
                            if (index1<0)
                            { 
                            index1 = i;
                            flag = true;
                            }
                        }
                    }
                }
                //
            }
            if(count<1)
            {
                flag = true;
            }

            Console.WriteLine("null\n]");
            Console.WriteLine("},"); //choices,

            if(ison&&min==1&&flag)
            {
                myres[0] = 1;
                myres[1] = (byte)index1;
                BinaryWriter myreply = GamePacketFactory.Create(CtosMessage.Response);
                myreply.Write(myres);
                Connection.Send(myreply);
                Console.WriteLine("\"default\":null");
                return;
            }
            else if(!flag)
            {
                ison = false;
                //_combo.queue.Clear();
                //Console.WriteLine("\n,error,\n");
            }

            // add note
            bool mode = false;
            if(false)
            { 
            /*
            //AddNote(); 多于2种选择再询问
            if (count > 1)
            {
                AddNote();
                mode = true;
            }


            //IList<ClientCard> selected = new List<ClientCard>();

            if (mode)
            {
                Process choicemaker = new Process();
                try
                {
                    choicemaker.StartInfo.UseShellExecute = false;
                    choicemaker.StartInfo.FileName = "answer";
                    choicemaker.StartInfo.CreateNoWindow = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mode = false;
                }

                choicemaker.Start();
                choicemaker.WaitForExit(1000);
                if (!choicemaker.HasExited)
                {
                    mode = false;
                    try
                    {
                        choicemaker.Kill();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Source + e.Message);
                    }
                }
            }

            if (mode)
            {
                // read note
                selected = ReadCard(1);//"OnSelectCard"
            }
            */
            //if(ApplyNote("OnSelectCard"))
            //    return;
            /*
            if (selected == null || !mode)
            {
                Console.WriteLine("\"default\":true,");
                mode = false;
                selected = new List<ClientCard>();
                //后期再加入按序号作为第二排序
                if(minc!=null)
                {
                    selected.Add(minc);
                    mode = true;
                    //minc.Show();
                    //Console.WriteLine("xx");
                }
                
                {
                int minid = 100000000; //max
                foreach(ClientCard c in cards)
                {
                    if (c == null)
                        continue;
                    if(c.Id < minid && c.Id > 1000)
                    {
                        
                        
                        ClientCard card;
                        if (((int)c.Location & (int)CardLocation.Overlay) != 0)
                            card = new ClientCard(c.Id, CardLocation.Overlay);
                        else
                            card = _duel.GetCard(c.Controller, c.Location, c.Sequence);
                        if (card == null)
                        {
                            Console.WriteLine("\"error\":??,");
                            continue;
                        }
                        if (card.Id == 0)
                            card.SetId(c.Id);

                        minid = c.Id;
                        selected.Clear();
                        selected.Add(card);
                        mode = true;
                    }
                }
                }
                
            }
            */
            }

            //默认的选择
            if (selected == null || !mode)
                    selected = func(cards, min, max, _select_hint, cancelable);

            _select_hint = 0;

            //_duel.Show();

            if (selected.Count == 0 && cancelable) //cancel is true
            {
                Connection.Send(CtosMessage.Response, -1);
                Console.WriteLine("\"selected\":{" + "\"category\":\"OnSelectCard\",");
                Console.WriteLine("\"list\":[null]\n}");
                //selected
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;

            Console.WriteLine("\"selected\":{" + "\"category\":\"OnSelectCard\",");
            //Console.WriteLine("\"result_count\":" + result[0].ToString());
            Console.WriteLine("\"list\":[");

            for (int i = 0; i < selected.Count; ++i)
            {
                int id = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (cards[j] == null) continue;
                    if (cards[j].Equals(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                //对选择card的同样的表示方式
                //没有执行到这里的，说明selected不存在？还是说其实是空的？问题出在上面的一段

                result[i + 1] = (byte)id;

                Console.Write("{\"result\":"+result[i+1].ToString()+",\"card\":");
                selected[i].Show();
                Console.WriteLine("},");
                //Console.WriteLine("result[i + 1] is {0}", result[i + 1]);
            }

            Console.WriteLine("null\n]");

            Console.WriteLine("}"); //selected

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectCard(BinaryReader packet)
        {
            //Logger.DebugWriteLine("this is in GameBehavier.OnSelectCard");
            Console.Write("{");
            InternalOnSelectCard(packet, _ai.OnSelectCard);
            Console.WriteLine("},");
        }

        private IList<ClientCard> ReadCard(int category)
        {
            IList<ClientCard> selected = new List<ClientCard>();
            int count = 0;
            switch (category)
            {
                case 1://"OnSelectCard"
                    {
                        FileStream file = new FileStream("./logs/a.txt", FileMode.Open);
                        StreamReader reader = new StreamReader(file);
                        string line ;
                        while( (line = reader.ReadLine())!= null )
                        {
                            int id, con, loc, seq, sub;
                            //Console.Read("{0},{1},{2},{3},{4}", id, con, loc, seq, sub);
                            string[] arr = line.Split(',');
                            id = int.Parse(arr[0]);
                            con = int.Parse(arr[1]);
                            loc = int.Parse(arr[2]);
                            seq = int.Parse(arr[3]);
                            sub = int.Parse(arr[4]);

                            ClientCard card = null;

                            IList<ClientCard> cards = _duel.GetCards(con, (CardLocation)loc);
                            foreach(ClientCard c in cards)
                            {
                                if (c == null) continue;
                                if(c.Id == id && (c.Sequence == seq || loc == (int)CardLocation.Hand || loc==(int)CardLocation.Deck || loc == (int)CardLocation.Extra || ( loc == (int)CardLocation.Removed && (c.Position & (int)CardPosition.FaceDown)!=0) ))
                                {
                                    card = c;
                                }
                            }

                            //if (((int)loc & (int)CardLocation.Overlay) != 0)
                            //    card = new ClientCard(id, CardLocation.Overlay);
                            //else
                            //    card = _duel.GetCard(con, (CardLocation)loc, seq);

                            if (card != null)
                            {
                                count++;
                                selected.Add(card);
                            }
                                
                        }
                        reader.Close();
                        file.Close();
                    }
                    break;
                default:
                    break;
            }
            if (count > 0)
                return selected;
            return null;
        }

        /*
        private bool ApplyNote(string category)
        {
            int ct = 0;
            switch(category)
            {
                case "OnSelectCard":
                    Connection.Send(ReadNote("OnSelectCard"));
                    ct++;
                    break;
                default:
                    break;
            }
            return ct > 0;
        }

        private BinaryWriter ReadNote(string category)
        {
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            switch (category)
            {
                case "OnSelectCard":
                    reply.Write(result);
                    break;
                default:
                    break;

            }

            return reply;
        }*/

        private void AddNote()
        {
            int ct;
            FileStream fileStream = new FileStream("./logs/q.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            
            ct = Convert.ToInt32(reader.ReadLine());
            reader.Close();
            //fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Close();


            //streamWriter.Flush();
            FileStream file = new FileStream("./logs/q.txt", FileMode.Truncate);
            StreamWriter streamWriter = new StreamWriter(file);
            streamWriter.Write(ct+1);
            streamWriter.Close();
            file.Close();
            //fileStream.Flush();
            
        }

        private void OnSelectChain(BinaryReader packet)
        {
            int player = packet.ReadByte(); // player
            int count = packet.ReadByte();
            packet.ReadByte(); // specount
            bool forced = packet.ReadByte() != 0;
            packet.ReadInt32(); // hint1
            packet.ReadInt32(); // hint2

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> descs = new List<int>();

            Console.Write("{");

            int index1 = -1;
            ComboStep step = null;
            StepObject obj = null;
            int ida = 0, loca = 0, seqa = 0, ispuba = 0;
            begin:
            if (_combo.queue.Count > 0 && count > 0)
            {
                step = _combo.queue.Dequeue();
            }

            if (step == null || step.category != ChoiceCategory.OnSelectChain)
            {
                ison = false;
                if (step.category != ChoiceCategory.OnSelectPlace)
                {
                    if (step != null)
                        ReEnq(step);
                }
                else
                {
                    ison = true;
                    _combo.queue.Dequeue();
                    goto begin;
                    //step = _combo.queue.Dequeue();
                }
                
            }
            bool flagc = false;
            if (ison)
            {
                obj = step.objlist.Dequeue();
                ida = obj.stepcard.id;
                loca = obj.stepcard.loc;
                seqa = obj.stepcard.seq;
                ispuba = obj.stepcard.ispub;
            }


            Console.WriteLine("\"choices\":{\"category\":\"OnSelectChain\",");
            Console.WriteLine("\"player\":" + player + ",");
            Console.WriteLine("\"list\":[");

            for (int i = 0; i < count; ++i)
            {
                int flag = packet.ReadByte(); // flag
                int id = packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                int sseq = packet.ReadByte();

                int desc = packet.ReadInt32();

                Console.WriteLine("{\"flag\":" + flag + ",\"id\":" + id + ",\"controller\":" + con + ",\"loc\":\"" + (CardLocation)loc + "\",\"seq\":" + seq + ",\"sseq\":" + sseq + ",\"desc\":" + desc );
                Console.Write(",\"card\":");
                _duel.GetCard(con, loc, seq, sseq).Show();
                Console.WriteLine("},");
                cards.Add(_duel.GetCard(con, loc, seq, sseq));
                descs.Add(desc);

                //
                if (ison)
                {
                    if (id == ida && (loca == (int)loc || loca == 0))
                    {
                        if ((seqa == seq || seqa == 0 )|| (loca != 4 && loca != 8))
                        {
                            if (index1 < 0)
                            {
                                index1 = i;
                                flagc = true;
                            }
                        }
                    }
                }
                //

            }
            if(count<1)
            {
                flagc = true;
            }

            Console.WriteLine("null\n]");
            Console.WriteLine("},"); //choices,


            if(ison &&count>0&&obj.value==0)
            {
                Connection.Send(CtosMessage.Response, -1);
                Console.WriteLine("\"default\":null},");
                return;
            }

            if(ison && index1 > -1&&flagc)
            { 
                Connection.Send(CtosMessage.Response, index1);
                Console.WriteLine("\"default\":null},");
                return;
            }
            ison = true;
            if(!flagc&&count>0)
            {
                ison = false;
                _combo.queue.Clear();
                //_duel.Show();
                //Console.WriteLine("\n,error,\n");
            }
            //wait
            if (!forced && count > 1)
            {
                //AddNote();
            }

            //_duel.Show();
            //duel,
            

            if (cards.Count == 0)
            {
                Console.WriteLine("\"selected\":{\"category\":\"OnSelectChain\",");
                Console.WriteLine("\"list\":[null]");

                Console.WriteLine("}");

                Console.WriteLine("},");
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            if (cards.Count == 1 && forced)
            {
                Console.WriteLine("\"selected\":{\"category\":\"OnSelectChain\",");

                Console.WriteLine("\"list\":[");
                Console.Write("{\"card\":");
                cards[0].Show();
                Console.WriteLine("}");
                Console.WriteLine("]");

                Console.WriteLine("}");

                Console.WriteLine("},");
                Connection.Send(CtosMessage.Response, 0);
                return;
            }
            
            //chaining
            Connection.Send(CtosMessage.Response, _ai.OnSelectChain(cards, descs, forced));
        }

        private void OnSelectCounter(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int type = packet.ReadInt16(); //种类
            int quantity = packet.ReadInt16(); //数量

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> counters = new List<int>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation) packet.ReadByte();
                int seq = packet.ReadByte();
                int num = packet.ReadInt16();
                cards.Add(_duel.GetCard(player, loc, seq));
                counters.Add(num);
            }

            IList<int> used = _ai.OnSelectCounter(type, quantity, cards, counters);
            byte[] result = new byte[used.Count * 2];
            for (int i = 0; i < used.Count; ++i)
            {
                result[i * 2] = (byte)(used[i] & 0xff);
                result[i * 2 + 1] = (byte)(used[i] >> 8);
            }
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectDisfield(BinaryReader packet)
        {
            OnSelectPlace(packet);
        }

        private void OnSelectEffectYn(BinaryReader packet)
        {
            packet.ReadByte(); // player

            int cardId = packet.ReadInt32();
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();
            int seq = packet.ReadByte();
            packet.ReadByte();
            int desc = packet.ReadInt32();

            ComboStep step = null;
            StepObject obj = null;
            int ida = 0, loca = 0, seqa = 0, ispuba = 0;
            if (_combo.queue.Count > 0)
            {
                step = _combo.queue.Dequeue();
            }

            if (step == null || step.category != ChoiceCategory.OnSelectEffectYesNo)
            {
                ison = false;
                ReEnq(step);
            }
            
            if (ison)
            {
                obj = step.objlist.Dequeue();
                ida = obj.stepcard.id;
                loca = obj.stepcard.loc;
                seqa = obj.stepcard.seq;
                ispuba = obj.stepcard.ispub;
            }


            ClientCard card = _duel.GetCard(player, loc, seq);

            //
            if (ison)
            {
                if (cardId == ida && (loca == (int)loc || loca == 0))
                {
                    if (seqa == seq || seqa == 0 || loca != 4 && loca != 8)
                    {
                        Connection.Send(CtosMessage.Response, obj.value);
                        
                        return;
                    }
                }
            }
            //
            
            if (card == null)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }
            ison = true;
            if (card.Id == 0) card.SetId(cardId);

            int reply = _ai.OnSelectEffectYn(card, desc) ? (1) : (0);
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnSelectIdleCmd(BinaryReader packet)
        {
            packet.ReadByte(); // player
            Console.Write("{");//select idle
            int action=-1;
            //int index=-1;
            MainPhaseAction mpa = null;
            ComboStep step = null;
            StepObject obj = null;
            int ida = 0, loca = 0, seqa = 0, ispuba = 0;
            begin:
            if (_combo.queue.Count > 0)
            { 
                step = _combo.queue.Dequeue();
            }

                if(step==null || step.category!=ChoiceCategory.OnSelectIdleCmd)
                {
                    ison = false;
                if (step.category != ChoiceCategory.OnSelectPlace)
                    ReEnq(step);
                else
                {
                    ison = true;
                    _combo.queue.Dequeue();
                    goto begin;
                    //step = _combo.queue.Dequeue();
                }
                }
            bool flag = false;
            if(ison)
            {
                obj = step.objlist.Dequeue();
                action = obj.value;
                ida = obj.stepcard.id;
                loca = obj.stepcard.loc;
                seqa = obj.stepcard.seq;
                ispuba = obj.stepcard.ispub;
            }

            Console.WriteLine("\"choices\":{\"category\":\"OnSelectIdleCmd\",");//choices
            _duel.MainPhase = new MainPhase();
            MainPhase main = _duel.MainPhase;
            Console.WriteLine("\"list\":[");//list
            int count;
            for (int k = 0; k < 5; k++)
            {
                //k是cmd 的代号
                count = packet.ReadByte();
                Console.WriteLine("{");//cmd
                Console.WriteLine("\"cmd_type\":" + k + ","); //idle type
                Console.WriteLine("\"cardlist\":[");//card list
                for (int i = 0; i < count; ++i)
                {
                    int id = packet.ReadInt32(); // card id
                    int con = GetLocalPlayer(packet.ReadByte());//player
                    CardLocation loc = (CardLocation)packet.ReadByte();
                    int seq = packet.ReadByte();
                    ClientCard card = _duel.GetCard(con, loc, seq);
                    if (card == null) continue;
                    card.ActionIndex[k] = i;
                    //cmd的主体卡片
                    
                    //Console.Write("\"card\":");
                    // in card list 
                    card.Show();
                    Console.WriteLine(",");
                    //主要阶段1的idle操作
                    switch (k)
                    {
                        case 0:
                            main.SummonableCards.Add(card);
                            break;
                        case 1:
                            main.SpecialSummonableCards.Add(card);
                            break;
                        case 2:
                            main.ReposableCards.Add(card);
                            break;
                        case 3:
                            main.MonsterSetableCards.Add(card);
                            break;
                        case 4:
                            main.SpellSetableCards.Add(card);
                            break;
                    }
                    //
                    if(ison)
                    {
                        if(k==action && id==ida && (loca==(int)loc||loca==0))
                        {
                            if (seqa == seq || seqa == 0 || loca != 4 && loca != 8 )
                            {
                                mpa = new MainPhaseAction((MainPhaseAction.MainAction)action, i);
                                flag = true;
                            }
                        }
                    }
                    //
                }
                //
                Console.WriteLine("null]");//cardlist end
                Console.WriteLine("},");//cmd end
            }
            count = packet.ReadByte();
            Console.WriteLine("{");//cmd
            Console.WriteLine("\"cmd_type\":" + 5 + ","); //idle type
            Console.WriteLine("\"cardlist\":[");//card list
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card == null) continue;
                card.ActionIndex[5] = i;
                if (card.ActionActivateIndex.ContainsKey(desc))
                    card.ActionActivateIndex.Remove(desc);
                card.ActionActivateIndex.Add(desc, i);
                //
                card.Show();
                Console.WriteLine(",");
                //
                main.ActivableCards.Add(card);
                main.ActivableDescs.Add(desc);
                //
                if (ison)
                {
                    if ((int)MainPhaseAction.MainAction.Activate == action && id == ida && loca == (int)loc)
                    {
                        if (seqa == seq || seqa == 0 || loca != 4 && loca != 8&&(desc==obj.stepcard.desc|| obj.stepcard.desc==0))
                        {
                            mpa = new MainPhaseAction((MainPhaseAction.MainAction)action, i);
                            flag = true;
                        }
                    }
                }
                //
            }//不明觉厉，是发动吧
            // 
            Console.WriteLine("null]");//cardlist end
            Console.WriteLine("},");//cmd end

            main.CanBattlePhase = packet.ReadByte() != 0;
            main.CanEndPhase = packet.ReadByte() != 0;
            packet.ReadByte(); // CanShuffle
            Console.WriteLine("null],");//list end
            Console.WriteLine("\"can_bp\":" + "\"" + main.CanBattlePhase + "\"" + ",\"can_ep\":" + "\"" + main.CanEndPhase + "\"");
            Console.WriteLine("},");
            //choices

            //go on combo
            if(ison&&flag)
            {
                Console.WriteLine("\"default\":null},");
                Connection.Send(CtosMessage.Response, mpa.ToValue());
                return;
            }


            //wait
            //一定是有多种选择，或者是只有一种选择(废话嘛
            //AddNote();
            ison = true;
            //if(!flag && step.category == ChoiceCategory.OnSelectPlace)
            //{
            //    flag = true;
            //}
            if(!flag)
            {
                ison = false;
                _combo.queue.Clear();
                //_duel.Show();
                //Console.WriteLine("\n,error,\n");
            }
            Connection.Send(CtosMessage.Response, _ai.OnSelectIdleCmd(main).ToValue());
            Console.WriteLine("},");//select idle end
        }

        private void OnSelectOption(BinaryReader packet)
        {
            IList<int> options = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                options.Add(packet.ReadInt32());
            Connection.Send(CtosMessage.Response, _ai.OnSelectOption(options));
        }

        private void OnSelectPlace(BinaryReader packet)
        {
            //这是啥操作？

            Console.WriteLine("{\"choices\":{\"category\":\"OnSelectPlace\",");//choices
            int player = packet.ReadByte(); // player
            int min = packet.ReadByte(); // min
            int field = ~packet.ReadInt32();

            byte[] shi = BitConverter.GetBytes(field);
            int m = 4;

            Console.WriteLine("\"player\":" + player + ",");
            Console.WriteLine("\"min\":" + min + ",");
            Console.WriteLine("\"field\":" + field);
            for(int i=0;i<m;i++)
            {
                Console.Write(",\"" + i + "\":" + shi[i]);
            }//no use, because it may have lost accuracy

            byte[] resp = new byte[3];

            bool pendulumZone = false;

            ComboStep step = null;
            StepObject obj = null;
            int ida = 0, loca = 0, seqa = 0, ispuba = 0;
            if (_combo.queue.Count > 0)
            {
                step = _combo.queue.Dequeue();
            }

            if (step == null || step.category != ChoiceCategory.OnSelectPlace)
            {
                ison = false;
                ReEnq(step);
            }

            if (ison)
            {
                obj = step.objlist.Dequeue();
                ida = obj.stepcard.id;
                loca = obj.stepcard.loc;
                seqa = obj.stepcard.seq;
                ispuba = obj.stepcard.ispub;
            }
            

            int filter;
            if ((field & 0x7f) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x4;
                filter = field & 0x7f;
            }
            else if ((field & 0x1f00) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x8;
                filter = (field >> 8) & 0x1f;
            }
            else if ((field & 0xc000) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x8;
                filter = (field >> 14) & 0x3;
                pendulumZone = true;
            }
            else if ((field & 0x7f0000) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(1);
                resp[1] = 0x4;
                filter = (field >> 16) & 0x7f;
            }
            else if ((field & 0x1f000000) != 0)
            {
                resp[0] = (byte) GetLocalPlayer(1);
                resp[1] = 0x8;
                filter = (field >> 24) & 0x1f;
            }
            else
            {
                resp[0] = (byte) GetLocalPlayer(1);
                resp[1] = 0x8;
                filter = (field >> 30) & 0x3;
                pendulumZone = true;
            }

            if (!pendulumZone)
            {
                if ((filter & 0x40) != 0) resp[2] = 6; //6
                else if ((filter & 0x20) != 0) resp[2] = 5; //5
                else if ((filter & 0x4) != 0) resp[2] = 2; //2
                else if ((filter & 0x2) != 0) resp[2] = 1; //1
                else if ((filter & 0x8) != 0) resp[2] = 3; //3
                else if ((filter & 0x1) != 0) resp[2] = 0; //0
                else if ((filter & 0x10) != 0) resp[2] = 4; //4
            }
            else
            {
                if ((filter & 0x1) != 0) resp[2] = 6;
                if ((filter & 0x2) != 0) resp[2] = 7;
            }

            Console.WriteLine("},"); //choices end

            if(ison&&resp[0]==_duel.GetLocalPlayer(0))
            {
                resp[2] = (byte)obj.value;
            }

            Console.WriteLine("\"selected\":{\"category\":\"OnSelectPlace\",");//selected
            Console.WriteLine("\"filter\":" + filter + ",");//filter
            Console.WriteLine("\"list\":[" + resp[0] + "," + resp[1] + "," + resp[2] + "]");//list

            Console.Write("}");//selected end
            Console.WriteLine("},");

            ison = true;
            if(resp[0] == _duel.GetLocalPlayer(1))
            {
                ReEnq(step);
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(resp);
            Connection.Send(reply);
        }

        private void OnSelectPosition(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int cardId = packet.ReadInt32();
            int pos = packet.ReadByte();
            if (pos == 0x1 || pos == 0x2 || pos == 0x4 || pos == 0x8)
            {
                Connection.Send(CtosMessage.Response, pos);
                return;
            }
            IList<CardPosition> positions = new List<CardPosition>();
            if ((pos & (int)CardPosition.FaceUpAttack) != 0)
                positions.Add(CardPosition.FaceUpAttack);
            if ((pos & (int)CardPosition.FaceDownAttack) != 0)
                positions.Add(CardPosition.FaceDownAttack);
            if ((pos & (int)CardPosition.FaceUpDefence) != 0)
                positions.Add(CardPosition.FaceUpDefence);
            if ((pos & (int)CardPosition.FaceDownDefence) != 0)
                positions.Add(CardPosition.FaceDownDefence);
            Connection.Send(CtosMessage.Response, (int)_ai.OnSelectPosition(cardId, positions));
        }

        private void OnSelectSum(BinaryReader packet)
        {
            bool mode = packet.ReadByte() == 0;
            packet.ReadByte(); // player
            int sumval = packet.ReadInt32();
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            if (max <= 0)
                max = 99;

            Console.WriteLine("{\"choices\":{\"category\":\"OnSelectSum\",");//select_sum choices

            Console.WriteLine("\"sumval\":" + sumval + ",\"min\":" + min + ",\"max\":" + max + ",");

            IList<ClientCard> mandatoryCards = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>();
            Console.WriteLine("\"list\":[");//list
            for (int j = 0; j < 2; ++j)
            {
                Console.WriteLine("{");
                int count = packet.ReadByte();
                Console.WriteLine("\"count\":" + count + ",\"cardlist\":[");//count,cardlist
                for (int i = 0; i < count; ++i)
                {
                    int cardId = packet.ReadInt32();
                    int player = GetLocalPlayer(packet.ReadByte());
                    CardLocation loc = (CardLocation)packet.ReadByte();
                    int seq = packet.ReadByte();
                    ClientCard card = _duel.GetCard(player, loc, seq);
                    if (cardId != 0 && card.Id != cardId)
                        card.SetId(cardId);
                    card.SelectSeq = i;
                    int OpParam = packet.ReadInt32();
                    int OpParam1 = OpParam & 0xffff;
                    int OpParam2 = OpParam >> 16;
                    if (OpParam2 > 0 && OpParam1 > OpParam2)
                    {
                        card.OpParam1 = OpParam2;
                        card.OpParam2 = OpParam1;
                    }
                    else
                    {
                        card.OpParam1 = OpParam1;
                        card.OpParam2 = OpParam2;
                    }
                    if (j == 0)
                    {
                        mandatoryCards.Add(card);
                        card.Show();
                        Console.WriteLine(",-1,"); //mark for mandatory
                    }
                    else
                    {
                        cards.Add(card);
                        card.Show();
                        Console.WriteLine(",");
                    }
                }
                Console.WriteLine("null]\n},");//count,cardlist
            }
            Console.WriteLine("null]");//list
            Console.WriteLine("},");//choices

            //add 结构看不懂
            //AddNote();

            for (int k = 0; k < mandatoryCards.Count; ++k)
            {
                sumval -= mandatoryCards[k].OpParam1;
            }

            IList<ClientCard> selected = _ai.OnSelectSum(cards, sumval, min, max, _select_hint, mode);
            _select_hint = 0;

            byte[] result = new byte[mandatoryCards.Count + selected.Count + 1];
            int index = 0;

            result[index++] = (byte)(mandatoryCards.Count + selected.Count);
            while (index <= mandatoryCards.Count)
            {
                result[index++] = 0;
            }//mandatory cards
            for (int i = 0; i < selected.Count; ++i)
            {
                result[index++] = (byte)selected[i].SelectSeq;
            }

            //_duel.Show();
            //duel

            Console.WriteLine("\"selected\":{\"category\":\"OnSelectSum\",");//selected
            Console.WriteLine("\"list\":[");
            for (int i=0;i<selected.Count;++i)
            {
                Console.Write("{\"result\":" + result[i] + ",\"cardlist\":["); //为了一致性
                selected[i].Show();
                Console.WriteLine("]},");
            }
            Console.WriteLine("null]");
            Console.WriteLine("}\n},");//selected . select_sum
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectTribute(BinaryReader packet)
        {
            Console.WriteLine("{");
            InternalOnSelectCard(packet, _ai.OnSelectTribute);
            Console.WriteLine("},");
        }

        private void OnSelectYesNo(BinaryReader packet)
        {
            /* int player = */ packet.ReadByte(); // player
            int desc = packet.ReadInt32();
            ComboStep step = null;
            StepObject obj = null;

            if (_combo.queue.Count > 0)
            {
                step = _combo.queue.Dequeue();
            }

            if (step == null || step.category != ChoiceCategory.OnSelectYesNo)
            {
                ison = false;
                ReEnq(step);
            }
            //bool flag = false;
            if (ison)
            {
                obj = step.objlist.Dequeue();
                Connection.Send(CtosMessage.Response, obj.value);
                return;
            }
            int reply = _ai.OnSelectYesNo(desc) ? (1) : (0); //desc
            //内部没有发生影响同步的操作
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnAnnounceAttrib(BinaryReader packet)
        {
            IList<CardAttribute> attributes = new List<CardAttribute>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 7; ++i)
            {
                if ((available & filter) != 0)
                    attributes.Add((CardAttribute) filter);
                filter <<= 1;
            }
            attributes = _ai.OnAnnounceAttrib(count, attributes);
            int reply = 0;
            for (int i = 0; i < count; ++i)
                reply += (int)attributes[i];
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnAnnounceCard(BinaryReader packet)
        {
            // not fully implemented
            Connection.Send(CtosMessage.Response, _ai.OnAnnounceCard());
        }

        private void OnAnnounceNumber(BinaryReader packet)
        {
            IList<int> numbers = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                numbers.Add(packet.ReadInt32());
            Connection.Send(CtosMessage.Response, _ai.OnAnnounceNumber(numbers));
        }

        private void OnAnnounceRace(BinaryReader packet)
        {
            IList<CardRace> races = new List<CardRace>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 23; ++i)
            {
                if ((available & filter) != 0)
                    races.Add((CardRace)filter);
                filter <<= 1;
            }
            races = _ai.OnAnnounceRace(count, races);
            int reply = 0;
            for (int i = 0; i < count; ++i)
                reply += (int)races[i];
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnRockPaperScissors(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int result;
            if (_hand > 0)
                result = _hand;
            else
                result = _ai.OnRockPaperScissors();
            Connection.Send(CtosMessage.Response, result);
        }

        private void OnSpSummon(BinaryReader packet)
        {
            _ai.CleanSelectMaterials();
        }

        private void ReEnq(ComboStep step)
        {
            if (step == null)
                return;
            Queue<ComboStep> tmp = new Queue<ComboStep>();
            tmp.Enqueue(step);
            while (_combo.queue.Count > 0)
            {
                ComboStep tmpstep = _combo.queue.Dequeue();
                tmp.Enqueue(tmpstep);
            }
            while (tmp.Count > 0)
            {
                ComboStep tmpstep = tmp.Dequeue();
                _combo.queue.Enqueue(tmpstep);
            }
        }
    }
}