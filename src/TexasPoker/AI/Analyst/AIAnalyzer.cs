using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;
using TexasPoker.Models;
using TexasPoker.Logic;

namespace TexasPoker.AI.Analyst
{
    public static class AIAnalyzer
    {
        // 模拟次数
        private const int SimulationCount = 1000;
        
        // 用于洗牌的随机数生成器实例
        private static readonly Random randomInstance = new Random();

        // 除了pre-flop阶段之外，用这个分析
        public static async Task<AnalysisResult> Analyze(
            List<Card> myHand,
            List<Card> communityCards,
            int amountToCall,
            int totalPot
        ) {
            // 计算当前的静态牌力
            var currentBest = HandEvaluator.GetBestHand(myHand.Concat(communityCards).ToList());
            float strength = MapHandTypeToHandStrength(currentBest.Type);

            // 计算底池赔率
            float potOdds = CalculatePotOdds(amountToCall, totalPot);

            // 异步执行蒙特卡洛模拟
            float winProb = await Task.Run(() => SimulateEquity(myHand, communityCards));

            // TODO : 计算outs

            // 返回结果
            return new AnalysisResult {
                WinProbability = winProb,
                HandStrength = strength,
                PotOdds = potOdds,
                OutsCount = 0
            };
        }

        // 使用1000次蒙特卡洛模拟，计算玩家在当前牌型下的胜率
        private static float SimulateEquity(List<Card> myHand, List<Card> communityCards) {
            float winPoints = 0;

            // 1. 初始化剩余牌堆
            List<Card> remainingDeck = CreateRemainingDeck(myHand, communityCards);

            // 2. 模拟1000次
            for (int i = 0; i < SimulationCount; i++) {
                // 洗牌
                Shuffle(remainingDeck);

                // 模拟对手的牌
                List<Card> opponentCards = new List<Card> { remainingDeck[0], remainingDeck[1] };

                // 补齐公共牌到五张
                List<Card> simulatedCommunity = new List<Card>(communityCards);
                int cardNeeded = 5 - communityCards.Count;
                for (int j = 0; j < cardNeeded; j++) {
                    simulatedCommunity.Add(remainingDeck[j + 2]);
                }

                // 评价双方最终牌力
                BestHandResult myResult = HandEvaluator.GetBestHand(myHand.Concat(simulatedCommunity).ToList());
                BestHandResult opponentResult = HandEvaluator.GetBestHand(opponentCards.Concat(simulatedCommunity).ToList());

                if (myResult.CompareTo(opponentResult) > 0) {
                    // 胜
                    winPoints += 1.0f; 
                } else if (myResult.CompareTo(opponentResult) == 0) {
                    // 平局
                    winPoints += 0.5f;
                }
            }

            return winPoints / SimulationCount;
        }

        // 创建不含已知牌的牌堆
        private static List<Card> CreateRemainingDeck(List<Card> myHand, List<Card> communityCards) {
            List<Card> deck = new List<Card>();
            var knownCards = myHand.Concat(communityCards).ToList();

            // 遍历52张牌
            foreach (Suit s in Enum.GetValues(typeof(Suit))) {
                foreach (Rank r in Enum.GetValues(typeof(Rank))) {
                    // 判断当前牌是否在已知牌中
                    if (!knownCards.Any(c => c.Rank == r && c.Suit == s)) {
                        deck.Add(new Card(s, r));
                    }
                }
            }
            return deck;
        }

        // 洗牌算法
        private static void Shuffle(List<Card> list) {
            for (int i = list.Count - 1; i > 0; i--) {
                int j = randomInstance.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // 计算底池赔率
        public static float CalculatePotOdds(int amountToCall, int currentPot) {
            if (amountToCall == 0) {
                return 0;
            }
            // 跟注额 / （当前总底池 + 跟注额）
            return (float) amountToCall / (currentPot + amountToCall);
        }

        // 计算牌力
        public static float MapHandTypeToHandStrength(HandType handType) {
            // 将枚举映射为0——1的浮点数
            return (float)handType / Enum.GetNames(typeof(HandType)).Length;
        }

        // 计算pre-flop的结果
        public static AnalysisResult AnalyzePreFlop(List<Card> myHand, int amountToCall, int totalPot) {
            float winProb = PreflopEquityTable.GetEquity(myHand[0], myHand[1]);
            float potOdds = CalculatePotOdds(amountToCall, totalPot);

            return new AnalysisResult {
                WinProbability = winProb,
                HandStrength = winProb,
                PotOdds = potOdds,
                OutsCount = 0
            };
        }
    }
}