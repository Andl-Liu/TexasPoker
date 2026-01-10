using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;

namespace TexasPoker.Models
{
    // 最大的五张牌
    public class BestHandResult
    {
        // 牌的类型
        public HandType Type;
        // 五张具体的牌
        public List<Card> BestFiveCards;
        // 牌型得分，用于比较
        public long Score;

        // 构造函数
        public BestHandResult(HandType handType, List<Card> cards, long score) {
            this.Type = handType;
            this.BestFiveCards = cards;
            this.Score = score;
        }

        // 比较接口
        public int CompareTo(BestHandResult other) => Score.CompareTo(other.Score);
    }
}