using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;

namespace TexasPoker.Models
{
    // 底牌信息
    public class HoleCardInfo
    {
        // 大的牌
        public Rank HigherRank;
        // 小的牌
        public Rank LowerRank;
        // 底牌类型
        public HoleCardType Type;

        public HoleCardInfo(Rank higherRank, Rank lowerRank, HoleCardType type)
        {
            HigherRank = higherRank;
            LowerRank = lowerRank;
            Type = type;
        }

        public override string ToString()
        {
            string suffix = Type == HoleCardType.Suited ? "s" : (Type == HoleCardType.OffSuit ? "o" : "");
            return $"{RankToString(HigherRank)}{RankToString(LowerRank)}{suffix}";
        }

        private String RankToString(Rank r) {
            if ((int) r <= 9) return ((int)r).ToString();
            return r.ToString().Substring(0, 1);
        }
    }
}