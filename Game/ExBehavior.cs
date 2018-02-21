using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    class ExBehavior
    {
        Queue<GameCombo> combos;

        public ExBehavior ()
        {

        }

        public void SelectCard(GameBehavior behavior,BinaryReader packet)
        {
            packet.ReadByte(); // player
            bool cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            //IList<ClientCard> cards = new List<ClientCard>();

            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = behavior.GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // pos
                //ClientCard card;
                //if (((int)loc & (int)CardLocation.Overlay) != 0)
                    //card = new ClientCard(id, CardLocation.Overlay);
                //else
                    //card = behavior._duel.GetCard(player, loc, seq);
                //if (card == null) continue;
                //if (card.Id == 0)
                    //card.SetId(id);
                //cards.Add(card);
            }

            //IList<ClientCard> selected = new List<ClientCard>();

            //if (selected.Count == 0 && cancelable)
            if (cancelable)
            {
                behavior.Connection.Send(CtosMessage.Response, -1);
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
                    if (cards[j].Equals(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i + 1] = (byte)id;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            behavior.Connection.Send(reply);
        }
    }
}
