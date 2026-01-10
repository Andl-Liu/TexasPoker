using System;
using System.Collections.Generic;
using TexasPoker.AI.Analyst;
using TexasPoker.Enums;
using TexasPoker.Models;
using Xunit;

namespace TexasPoker.Tests.AI.Analyst
{
    public class PreflopEquityTableTests
    {
        [Fact]
        public void GetEquity_CardPair_ReturnsCorrectValue()
        {
            // 测试AA的胜率
            var card1 = new Card(Suit.Spade, Rank.Ace);
            var card2 = new Card(Suit.Heart, Rank.Ace);
            var equity = PreflopEquityTable.GetEquity(card1, card2);
            Assert.Equal(0.85f, equity, 2);
            
            // 测试KK的胜率
            card1 = new Card(Suit.Spade, Rank.King);
            card2 = new Card(Suit.Heart, Rank.King);
            equity = PreflopEquityTable.GetEquity(card1, card2);
            Assert.Equal(0.82f, equity, 2);
        }

        [Fact]
        public void GetEquity_HoleCardInfo_ReturnsCorrectValue()
        {
            // 测试AKs的胜率
            var info = new HoleCardInfo(Rank.Ace, Rank.King, HoleCardType.Suited);
            var equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(0.67f, equity, 2);
            
            // 测试AKo的胜率
            info = new HoleCardInfo(Rank.Ace, Rank.King, HoleCardType.OffSuit);
            equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(0.65f, equity, 2);
        }

        [Fact]
        public void GetEquity_KnownPairs_ReturnsCorrectValues()
        {
            // 测试顶级强牌
            Assert.Equal(0.80f, PreflopEquityTable.GetEquity(new Card(Suit.Spade, Rank.Queen), new Card(Suit.Heart, Rank.Queen)), 2);
            Assert.Equal(0.77f, PreflopEquityTable.GetEquity(new Card(Suit.Spade, Rank.Jack), new Card(Suit.Heart, Rank.Jack)), 2);
            Assert.Equal(0.75f, PreflopEquityTable.GetEquity(new Card(Suit.Spade, Rank.Ten), new Card(Suit.Heart, Rank.Ten)), 2);
        }

        [Fact]
        public void GetEquity_EstimatedValues_ReturnsCorrectEstimates()
        {
            // 测试A高牌的估算值
            var info = new HoleCardInfo(Rank.Ace, Rank.Five, HoleCardType.OffSuit);
            var equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(0.52f, equity, 2);

            // 测试K~10中等牌的估算值
            info = new HoleCardInfo(Rank.King, Rank.Jack, HoleCardType.OffSuit);
            equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(0.45f, equity, 2);

            // 测试其他杂色牌的估算值
            info = new HoleCardInfo(Rank.Six, Rank.Four, HoleCardType.OffSuit);
            equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(0.38f, equity, 2);
        }

        [Fact]
        public void GetEquity_CardInputAndHoleCardInfo_ReturnSameValue()
        {
            // 确保两种输入方式得到相同的结果
            var card1 = new Card(Suit.Diamond, Rank.Ace);
            var card2 = new Card(Suit.Diamond, Rank.King);
            var info = new HoleCardInfo(Rank.Ace, Rank.King, HoleCardType.Suited);

            var equityFromCards = PreflopEquityTable.GetEquity(card1, card2);
            var equityFromInfo = PreflopEquityTable.GetEquity(info);

            Assert.Equal(equityFromCards, equityFromInfo, 5);
        }

        [Fact]
        public void GetEquity_UnknownPair_EstimatesCorrectly()
        {
            // 使用一个不在预设表中的牌型
            var card1 = new Card(Suit.Heart, Rank.Nine);
            var card2 = new Card(Suit.Spade, Rank.Five);
            var equity = PreflopEquityTable.GetEquity(card1, card2);

            // 这应该是杂色牌，所以估算值是0.38
            Assert.Equal(0.38f, equity, 2);
        }

        [Theory]
        [InlineData(Rank.Ace, Rank.King, HoleCardType.Suited, 0.67f)]
        [InlineData(Rank.Ace, Rank.King, HoleCardType.OffSuit, 0.65f)]
        [InlineData(Rank.Queen, Rank.Queen, HoleCardType.Pair, 0.80f)]
        [InlineData(Rank.Jack, Rank.Jack, HoleCardType.Pair, 0.77f)]
        public void GetEquity_TheoryTest(Rank higherRank, Rank lowerRank, HoleCardType type, float expected)
        {
            var info = new HoleCardInfo(higherRank, lowerRank, type);
            var equity = PreflopEquityTable.GetEquity(info);
            Assert.Equal(expected, equity, 2);
        }
    }
}