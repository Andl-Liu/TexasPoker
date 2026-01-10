using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;
using TexasPoker.Logic;
using TexasPoker.Models;

namespace TexasPoker.AI.Analyst
{
    // 在Heads-Up(1V1)中，在Pre-flop阶段，面对随机手牌的胜率表
    public static class PreflopEquityTable
    {
        private static readonly Dictionary<string, float> EquityMap = new Dictionary<string, float>() {
            // --- 顶级强牌 (Top Tier) ---
            {"AA", 0.85f}, {"KK", 0.82f}, {"QQ", 0.80f}, {"JJ", 0.77f}, {"TT", 0.75f},
            {"AKs", 0.67f}, {"AQs", 0.66f}, {"AJs", 0.65f}, {"AKo", 0.65f}, {"KQs", 0.63f},

            // --- 强牌 (Strong) ---
            {"99", 0.72f}, {"88", 0.69f}, {"AQo", 0.64f}, {"ATs", 0.62f}, {"KJs", 0.62f},
            {"QJs", 0.60f}, {"AJo", 0.59f}, {"KTs", 0.59f}, {"KQo", 0.58f}, {"77", 0.66f},

            // --- 中等牌 (Speculative/Medium) ---
            {"A9s", 0.59f}, {"QTs", 0.58f}, {"JTs", 0.57f}, {"A5s", 0.56f}, {"T9s", 0.54f},
            {"66", 0.63f}, {"55", 0.60f}, {"KTo", 0.54f}, {"QJo", 0.53f}, {"JTo", 0.51f},
            
            // --- 常见弱牌 (Weak) ---
            {"A2s", 0.54f}, {"K5s", 0.51f}, {"76s", 0.49f}, {"A2o", 0.48f}, {"44", 0.56f},
            {"33", 0.53f}, {"22", 0.50f}, {"87s", 0.48f}, {"T8s", 0.46f}, {"97s", 0.45f},

            // --- 垃圾牌 (Trash / 抢衣服目标) ---
            {"72o", 0.32f}, {"Q2o", 0.40f}, {"J3o", 0.38f}, {"94o", 0.36f}, {"52o", 0.34f}
        };


        // 获取胜率
        public static float GetEquity(Card card1, Card card2) {
            var info = HandEvaluator.GetHoleCardInfo(card1, card2);
            return GetEquity(info);
        }

        public static float GetEquity(HoleCardInfo info) {
            string key = info.ToString();

            // 查表
            if (EquityMap.ContainsKey(key))
                return EquityMap[key];
            // 表中没有则估算
            return EstimateEquity(info);
        }

        private static float EstimateEquity(HoleCardInfo info) {
            // 估算不在表中的组合的胜率
            // 1. A 高牌杂色为 52%
            // 2. K~10 中等牌杂色为 45%
            // 3. 其他杂色牌为 38%
            if (info.HigherRank == Rank.Ace) return 0.52f;
            if (info.HigherRank >= Rank.Ten) return 0.45f;
            return 0.38f;
        }
    }
}