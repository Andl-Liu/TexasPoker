using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TexasPoker.Models;
using TexasPoker.Enums;

namespace TexasPoker.Logic
{
    // 手牌评估器
    public static class HandEvaluator
    {
        // 外部调用的主接口：输入5或6或7张牌，返回最强的5张牌和牌的类型
        public static BestHandResult GetBestHand(List<Card> cards)
        {
            // // 输入的牌数必须为7
            // if (sevenCards == null || sevenCards.Count != 7)
            // {
            //     throw new ArgumentException("Invalid number of cards");
            // }

            // 输入的牌数不能为null，且只能为5/6/7
            if (cards == null || (cards.Count != 5 && cards.Count != 6 && cards.Count != 7))
            {
                throw new ArgumentException("Invalid number of cards");
            }

            // 获取所有组合
            var combinations = GetCombinations(cards, 5);
            // 遍历并评估所有的组合，并从中找出最好的组合
            BestHandResult bestHand = new BestHandResult(HandType.HighCard, new List<Card>(), 0);
            foreach (var combination in combinations) {
                var cur = EvaluateFiveCards(combination);
                if (bestHand == null || cur.CompareTo(bestHand) > 0) 
                {
                    bestHand = cur;
                }
            }
            
            return bestHand;
        }

        // 判定 5 张牌的类型并打分
        internal static BestHandResult EvaluateFiveCards(List<Card> fiveCards) {
            // 1.将五张牌按点数降序排列
            var sortedCards = fiveCards.OrderByDescending(c => c.Rank).ToList();

            // 2.统计频率
            // 先按照点数进行分组
            // 然后按照频率进行降序
            // 然后按照点数进行降序
            var groups = sortedCards.GroupBy(c => c.Rank)
                                    .OrderByDescending(g => g.Count())
                                    .ThenByDescending(g => g.Key)
                                    .ToList();

            // 3.牌型判断

            // 判断是否为同花
            bool isFlush = sortedCards.GroupBy(c => c.Suit).Count() == 1;

            // 判断是否为顺子
            bool isStraight = IsStraight(sortedCards);

            // 判断是否为王牌同花顺
            bool isRoyalFlush = IsRoyalFlush(sortedCards, isFlush);

            HandType handType = DetermineHandType(groups, isFlush, isStraight, isRoyalFlush);

            // 4.计算得分
            long score = CalculateScore(handType, groups);

            return new BestHandResult(handType, fiveCards, score);
        }

        // 判断牌型
        private static HandType DetermineHandType(List<IGrouping<Rank, Card>> groups, bool isFlush, bool isStraight, bool isRoyalFlush) {
            HandType handType = HandType.HighCard;

            if (isRoyalFlush)
                handType = HandType.RoyalFlush;
            else if (isStraight && isFlush)
                handType = HandType.StraightFlush;
            else if (groups[0].Count() == 4) 
                handType = HandType.FourOfAKind;
            else if (groups[0].Count() == 3 && groups[1].Count() == 2)
                handType = HandType.FullHouse;
            else if (isFlush)
                handType = HandType.Flush;
            else if (isStraight) 
                handType = HandType.Straight;
            else if (groups[0].Count() == 3) 
                handType = HandType.ThreeOfAKind;
            else if (groups[0].Count() == 2 && groups[1].Count() == 2) 
                handType = HandType.TwoPairs;
            else if (groups[0].Count() == 2) 
                handType = HandType.OnePair;

            return handType;
        }

        // 判断是否为顺子
        private static bool IsStraight(List<Card> sortedCards) {
            // 普通顺子
            for (int i = 0;i < sortedCards.Count - 1;i++) {
                if (sortedCards[i].Rank != sortedCards[i + 1].Rank + 1) 
                    break;
                if (i == sortedCards.Count - 2) 
                    return true;
            }

            // A 5 4 3 2
            if (sortedCards[0].Rank == Rank.Ace 
                && sortedCards[1].Rank == Rank.Five 
                && sortedCards[2].Rank == Rank.Four 
                && sortedCards[3].Rank == Rank.Three 
                && sortedCards[4].Rank == Rank.Two) 
                return true;

            return false;
        }

        // 判断是否为王牌同花顺
        private static bool IsRoyalFlush(List<Card> sortedCards, bool isFlush) {
            if (!isFlush) return false;

            // A K Q J 10
            if (sortedCards[0].Rank == Rank.Ace 
                && sortedCards[1].Rank == Rank.King 
                && sortedCards[2].Rank == Rank.Queen 
                && sortedCards[3].Rank == Rank.Jack 
                && sortedCards[4].Rank == Rank.Ten) 
                return true;
            
            return false;
        }

        // 评分逻辑
        // 要求参数为按数量和点数排好的五张牌
        private static long CalculateScore(HandType type, List<IGrouping<Rank, Card>> groups) {
            // 牌型存储在score有效位最高的四位里，然后剩下的每四位从大到小依次存储牌面
            long score = (long) type;

            // 处理A2345这种特殊情况
            // 5是最大的牌，A不是
            if (type == HandType.Straight && groups.Any(g => g.Key == Rank.Ace) && groups.Any(g => g.Key == Rank.Five)) {
                for (int i = 5; i > 0;i--) 
                    score = (score << 4) | (long)i;
                return score;
            }

            // 处理其余情况
            foreach (var g in groups) {
                for (int i = 0; i < g.Count();i++)
                    score = (score << 4) | (long)g.Key;
            }

            return score;
        }

        public static HoleCardInfo GetHoleCardInfo(Card card1, Card card2) {
            // 1.确认大小排序
            Rank hi = card1.Rank > card2.Rank ? card1.Rank : card2.Rank;
            Rank lo = card1.Rank > card2.Rank ? card2.Rank : card1.Rank;

            // 2.判断类型
            HoleCardType type;
            if (card1.Rank == card2.Rank) {
                type = HoleCardType.Pair;
            } else if (card1.Suit == card2.Suit) {
                type = HoleCardType.Suited;
            } else {
                type = HoleCardType.OffSuit;
            }

            return new HoleCardInfo(hi, lo, type);
        }

        // 7选5 组合生成器
        private static IEnumerable<List<T>> GetCombinations<T>(List<T> list, int length) {
            if (length <= 0 || length > list.Count) {
                if (length == 0)
                    yield return new List<T>();
                yield break;
            }

            var indices = new int[length];
            for (int i = 0; i < length;i++) 
                indices[i] = i;

            while (true) {
                // 生成当前组合
                var combination = new List<T>(length);
                for (int i = 0; i < length; i++) {
                    combination.Add(list[indices[i]]);
                }
                // 返回当前组合,并将函数挂起
                yield return combination;

                // 找到下一个组合
                // 从后向前找第一个索引位置
                int position = length - 1;
                // 如果当前位置的索引已经是可以放的最大索引了，则再往前一个位置
                while (position >= 0 && indices[position] == list.Count - length + position) 
                    position--;

                // 如果当前位置来到了-1，说明所有组合已经生成完毕了
                if (position < 0)
                    break;
                
                // 把当前位置的索引加1
                indices[position]++;

                // 把当前位置之后的索引都设置为前一个位置的索引加1
                // 【0，1】-》【0，2】-》【0，3】-》【1，2】
                // 也就是把后面的部分恢复递增
                for (int i = position + 1; i < length; i++) {
                    indices[i] = indices[i - 1] + 1;
                }
            }
        }
    }
}