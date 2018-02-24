using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using WindBot.Game;

namespace WindBot.Game
{
    class ExBehavior
    {
        private List<GameCombo> combos;
        private GameCombo curcombo;
        private ComboStep curstep;


        public void SelectCard(Duel duel, YGOClient Connection, BinaryReader packet)
        {
            if(curstep==null)
            {
                return;
            }

            int stepcount = curstep.GetValCount();



            packet.ReadByte(); // player
            bool cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            //IList<ClientCard> cards = new List<ClientCard>();
            List<CardMsg> selected = new List<CardMsg>();


            int count = packet.ReadByte();
            CardMsg[] cards = new CardMsg[count];
            //int[][] cards = new int[count][];

            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = duel.IsFirst ? (packet.ReadByte()) : 1 - (packet.ReadByte()); //get local player
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // 
                cards[i] = new CardMsg(player, id, (int)loc, seq);

            }


            //


            if (selected.Count == 0 && cancelable)
            //if (cancelable)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;
            for (int i = 0; i < selected.Count; ++i)
            {
                int id = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (cards[j] == null) continue;
                    if (cards[j].SameTo(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i + 1] = (byte)id;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
            return;
        }

        private bool CehckFilter(Duel duel, CardMsg msg, StepVal val)
        {
            return val.filter(duel, msg, val.card);
        }

    }
}
