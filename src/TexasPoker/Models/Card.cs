using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;

namespace TexasPoker.Models
{
    // 德州扑克中的一张牌
    public class Card
    {
        // 获得牌面和花色
        // 创建之后不可变
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }

        // 获得CardID
        // 0-12 是 方块2-A
        // 13-25 是 梅花2-A
        // 26-38 是 红心2-A
        // 39-51 是 黑桃2-A
        public int GetCardID()
        {
            return (int)Suit * 13 + ((int)Rank - 2);
        }
    }
}