using System;
using System.Collections.Generic;
using System.Linq;
using TexasPoker.Enums;
using TexasPoker.Logic;
using TexasPoker.Models;
using Xunit;

namespace TexasPoker.Tests.Logic
{
    public class HandEvaluatorTests
    {
        [Fact]
        public void GetBestHand_WithInvalidCardCount_ThrowsArgumentException()
        {
            // 测试无效的牌数
            var cards = new List<Card> { CreateCard(Suit.Spade, Rank.Ace) };
            
            Assert.Throws<ArgumentException>(() => HandEvaluator.GetBestHand(cards));
            
            // 测试2张牌的情况
            var twoCards = new List<Card> 
            { 
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.King)
            };
            
            Assert.Throws<ArgumentException>(() => HandEvaluator.GetBestHand(twoCards));
            
            // 测试空列表
            Assert.Throws<ArgumentException>(() => HandEvaluator.GetBestHand(new List<Card>()));
        }

        [Fact]
        public void GetBestHand_WithNullCards_ThrowsArgumentException()
        {
            // 测试空牌列表
            Assert.Throws<ArgumentException>(() => HandEvaluator.GetBestHand(null));
        }

        [Fact]
        public void GetBestHand_WithFiveCards_EvaluatesCorrectly()
        {
            // 测试5张牌的正常情况
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Ten)
            };

            var result = HandEvaluator.GetBestHand(cards);
            
            Assert.Equal(HandType.RoyalFlush, result.Type);
        }

        [Fact]
        public void GetBestHand_WithSixCards_EvaluatesCorrectly()
        {
            // 测试6张牌的正常情况
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Ten),
                CreateCard(Suit.Heart, Rank.Two)
            };

            var result = HandEvaluator.GetBestHand(cards);
            
            Assert.Equal(HandType.RoyalFlush, result.Type);
        }

        [Fact]
        public void GetBestHand_WithSevenCards_EvaluatesCorrectly()
        {
            // 测试7张牌的正常情况
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Ten),
                CreateCard(Suit.Heart, Rank.Two),
                CreateCard(Suit.Diamond, Rank.Three)
            };

            var result = HandEvaluator.GetBestHand(cards);
            
            Assert.Equal(HandType.RoyalFlush, result.Type);
        }

        [Fact]
        public void EvaluateRoyalFlush()
        {
            // 测试皇家同花顺
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Ten)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.RoyalFlush, result.Type);
        }

        [Fact]
        public void EvaluateStraightFlush()
        {
            // 测试同花顺
            var cards = new List<Card>
            {
                CreateCard(Suit.Heart, Rank.Nine),
                CreateCard(Suit.Heart, Rank.Eight),
                CreateCard(Suit.Heart, Rank.Seven),
                CreateCard(Suit.Heart, Rank.Six),
                CreateCard(Suit.Heart, Rank.Five)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.StraightFlush, result.Type);
        }

        [Fact]
        public void EvaluateFourOfAKind()
        {
            // 测试四条
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.FourOfAKind, result.Type);
        }

        [Fact]
        public void EvaluateFullHouse()
        {
            // 测试葫芦
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.King),
                CreateCard(Suit.Spade, Rank.King)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.FullHouse, result.Type);
        }

        [Fact]
        public void EvaluateFlush()
        {
            // 测试同花
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Nine)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.Flush, result.Type);
        }

        [Fact]
        public void EvaluateStraight()
        {
            // 测试顺子
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Five),
                CreateCard(Suit.Heart, Rank.Four),
                CreateCard(Suit.Diamond, Rank.Three),
                CreateCard(Suit.Club, Rank.Two),
                CreateCard(Suit.Spade, Rank.Ace)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.Straight, result.Type);
        }

        [Fact]
        public void EvaluateStraight_AceToFive()
        {
            // 测试A-5-4-3-2的顺子
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Five),
                CreateCard(Suit.Diamond, Rank.Four),
                CreateCard(Suit.Club, Rank.Three),
                CreateCard(Suit.Spade, Rank.Two)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.Straight, result.Type);
        }

        [Fact]
        public void EvaluateThreeOfAKind()
        {
            // 测试三条
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.ThreeOfAKind, result.Type);
        }

        [Fact]
        public void EvaluateTwoPairs()
        {
            // 测试两对
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.King),
                CreateCard(Suit.Club, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.TwoPairs, result.Type);
        }

        [Fact]
        public void EvaluateOnePair()
        {
            // 测试一对
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.King),
                CreateCard(Suit.Club, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.OnePair, result.Type);
        }

        [Fact]
        public void EvaluateHighCard()
        {
            // 测试高牌
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.King),
                CreateCard(Suit.Diamond, Rank.Queen),
                CreateCard(Suit.Club, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Nine)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.HighCard, result.Type);
        }

        [Fact]
        public void EvaluateScoreComparison()
        {
            // 测试不同牌型的评分比较
            var highCard = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.King),
                CreateCard(Suit.Diamond, Rank.Queen),
                CreateCard(Suit.Club, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Nine)
            };

            var onePair = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.King),
                CreateCard(Suit.Club, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack)
            };

            var highCardResult = HandEvaluator.EvaluateFiveCards(highCard);
            var onePairResult = HandEvaluator.EvaluateFiveCards(onePair);

            Assert.True(onePairResult.Score > highCardResult.Score);
        }

        [Fact]
        public void EvaluateStraightWithSixHigh()
        {
            // 测试6-5-4-3-2的顺子
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Six),
                CreateCard(Suit.Heart, Rank.Five),
                CreateCard(Suit.Diamond, Rank.Four),
                CreateCard(Suit.Club, Rank.Three),
                CreateCard(Suit.Spade, Rank.Two)
            };

            var result = HandEvaluator.EvaluateFiveCards(cards);
            Assert.Equal(HandType.Straight, result.Type);
        }

        [Fact]
        public void EvaluateBestHandFromFiveCards()
        {
            // 测试从5张牌中选择最好的5张
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),    // 5张牌
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.Ace),    // 4个A - 四条
                CreateCard(Suit.Spade, Rank.King)
            };

            var result = HandEvaluator.GetBestHand(cards);
            Assert.Equal(HandType.FourOfAKind, result.Type);
        }

        [Fact]
        public void EvaluateBestHandFromSixCards()
        {
            // 测试从6张牌中选择最好的5张
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),    // 6张牌
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.Ace),    // 4个A - 四条
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Heart, Rank.King)   // 2个K - 一对
            };

            var result = HandEvaluator.GetBestHand(cards);
            Assert.Equal(HandType.FourOfAKind, result.Type);
        }

        [Fact]
        public void EvaluateBestHandFromSevenCards()
        {
            // 测试从7张牌中选择最好的5张
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),    // 7张牌
                CreateCard(Suit.Heart, Rank.Ace),
                CreateCard(Suit.Diamond, Rank.Ace),
                CreateCard(Suit.Club, Rank.Ace),    // 4个A - 四条
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Heart, Rank.King),  // 2个K - 一对
                CreateCard(Suit.Diamond, Rank.Queen)
            };

            var result = HandEvaluator.GetBestHand(cards);
            Assert.Equal(HandType.FourOfAKind, result.Type);
        }

        [Fact]
        public void EvaluateBestHandWithRoyalFlush()
        {
            // 测试从7张牌中找到皇家同花顺
            var cards = new List<Card>
            {
                CreateCard(Suit.Spade, Rank.Ace),
                CreateCard(Suit.Spade, Rank.King),
                CreateCard(Suit.Spade, Rank.Queen),
                CreateCard(Suit.Spade, Rank.Jack),
                CreateCard(Suit.Spade, Rank.Ten),
                CreateCard(Suit.Heart, Rank.Two),
                CreateCard(Suit.Diamond, Rank.Three)
            };

            var result = HandEvaluator.GetBestHand(cards);
            Assert.Equal(HandType.RoyalFlush, result.Type);
        }

        private static Card CreateCard(Suit suit, Rank rank)
        {
            return new Card(suit, rank);
        }
        
        [Fact]
        public void GetHoleCardInfo_Pair_ReturnsCorrectInfo()
        {
            // 测试两张相同点数的牌应该返回 Pair 类型
            var card1 = CreateCard(Suit.Spade, Rank.Ace);
            var card2 = CreateCard(Suit.Heart, Rank.Ace);
            
            var result = HandEvaluator.GetHoleCardInfo(card1, card2);
            
            Assert.Equal(HoleCardType.Pair, result.Type);
            Assert.Equal(Rank.Ace, result.HigherRank);
            Assert.Equal(Rank.Ace, result.LowerRank);
            Assert.Equal("AA", result.ToString());
        }

        [Fact]
        public void GetHoleCardInfo_SuitedSameRankOrderDoesNotMatter()
        {
            // 测试同花牌，验证顺序不影响结果
            var suitedAceKing1 = CreateCard(Suit.Spade, Rank.Ace);
            var suitedAceKing2 = CreateCard(Suit.Spade, Rank.King);
            var result1 = HandEvaluator.GetHoleCardInfo(suitedAceKing1, suitedAceKing2);
            
            var suitedAceKing3 = CreateCard(Suit.Spade, Rank.King);
            var suitedAceKing4 = CreateCard(Suit.Spade, Rank.Ace);
            var result2 = HandEvaluator.GetHoleCardInfo(suitedAceKing3, suitedAceKing4);
            
            Assert.Equal(HoleCardType.Suited, result1.Type);
            Assert.Equal(HoleCardType.Suited, result2.Type);
            Assert.Equal(Rank.Ace, result1.HigherRank);
            Assert.Equal(Rank.King, result1.LowerRank);
            Assert.Equal(Rank.Ace, result2.HigherRank);
            Assert.Equal(Rank.King, result2.LowerRank);
            Assert.Equal("AKs", result1.ToString());
            Assert.Equal("AKs", result2.ToString());
        }

        [Fact]
        public void GetHoleCardInfo_OffsuitSameRankOrderDoesNotMatter()
        {
            // 测试杂色牌，验证顺序不影响结果
            var offsuitAceKing1 = CreateCard(Suit.Spade, Rank.Ace);
            var offsuitAceKing2 = CreateCard(Suit.Heart, Rank.King);
            var result1 = HandEvaluator.GetHoleCardInfo(offsuitAceKing1, offsuitAceKing2);
            
            var offsuitAceKing3 = CreateCard(Suit.Heart, Rank.King);
            var offsuitAceKing4 = CreateCard(Suit.Spade, Rank.Ace);
            var result2 = HandEvaluator.GetHoleCardInfo(offsuitAceKing3, offsuitAceKing4);
            
            Assert.Equal(HoleCardType.OffSuit, result1.Type);
            Assert.Equal(HoleCardType.OffSuit, result2.Type);
            Assert.Equal(Rank.Ace, result1.HigherRank);
            Assert.Equal(Rank.King, result1.LowerRank);
            Assert.Equal(Rank.Ace, result2.HigherRank);
            Assert.Equal(Rank.King, result2.LowerRank);
            Assert.Equal("AKo", result1.ToString());
            Assert.Equal("AKo", result2.ToString());
        }

        [Fact]
        public void GetHoleCardInfo_AllHoleCardTypesCovered()
        {
            // 测试所有类型的底牌
            // 一对
            var pair = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Ace), 
                CreateCard(Suit.Heart, Rank.Ace));
            Assert.Equal(HoleCardType.Pair, pair.Type);
            Assert.Equal(Rank.Ace, pair.HigherRank);
            Assert.Equal(Rank.Ace, pair.LowerRank);

            // 同花
            var suited = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Ace), 
                CreateCard(Suit.Spade, Rank.King));
            Assert.Equal(HoleCardType.Suited, suited.Type);
            Assert.Equal(Rank.Ace, suited.HigherRank);
            Assert.Equal(Rank.King, suited.LowerRank);

            // 杂色
            var offsuit = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Ace), 
                CreateCard(Suit.Heart, Rank.King));
            Assert.Equal(HoleCardType.OffSuit, offsuit.Type);
            Assert.Equal(Rank.Ace, offsuit.HigherRank);
            Assert.Equal(Rank.King, offsuit.LowerRank);
        }

        [Fact]
        public void GetHoleCardInfo_LowestPair()
        {
            // 测试最小的一对牌
            var pair = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Two), 
                CreateCard(Suit.Heart, Rank.Two));
            Assert.Equal(HoleCardType.Pair, pair.Type);
            Assert.Equal(Rank.Two, pair.HigherRank);
            Assert.Equal(Rank.Two, pair.LowerRank);
            Assert.Equal("22", pair.ToString());
        }

        [Fact]
        public void GetHoleCardInfo_HighestSuited()
        {
            // 测试最高级别的同花牌
            var suited = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Ace), 
                CreateCard(Suit.Spade, Rank.King));
            Assert.Equal(HoleCardType.Suited, suited.Type);
            Assert.Equal(Rank.Ace, suited.HigherRank);
            Assert.Equal(Rank.King, suited.LowerRank);
            Assert.Equal("AKs", suited.ToString());
        }

        [Fact]
        public void GetHoleCardInfo_LowestOffsuit()
        {
            // 测试最低的杂色牌
            var offsuit = HandEvaluator.GetHoleCardInfo(
                CreateCard(Suit.Spade, Rank.Two), 
                CreateCard(Suit.Heart, Rank.Three));
            Assert.Equal(HoleCardType.OffSuit, offsuit.Type);
            Assert.Equal(Rank.Three, offsuit.HigherRank);
            Assert.Equal(Rank.Two, offsuit.LowerRank);
            Assert.Equal("32o", offsuit.ToString());
        }
    }
}