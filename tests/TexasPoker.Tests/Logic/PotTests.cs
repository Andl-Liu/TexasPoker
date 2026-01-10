using System.Collections.Generic;
using TexasPoker.Logic;
using TexasPoker.Models;
using Xunit;

namespace TexasPoker.Tests.Logic
{
    public class PotTests
    {
        [Fact]
        public void Constructor_InitializesTotalAmountToZero()
        {
            // Arrange
            var pot = new Pot();

            // Assert
            Assert.Equal(0, pot.TotalAmount);
        }

        [Fact]
        public void AddAmount_IncreasesTotalAmount()
        {
            // Arrange
            var pot = new Pot();
            int initialAmount = pot.TotalAmount;

            // Act
            pot.AddAmount(100);

            // Assert
            Assert.Equal(initialAmount + 100, pot.TotalAmount);
        }

        [Fact]
        public void CollectBets_AddsPlayerBetsToPotAndClearsPlayerBets()
        {
            // Arrange
            var pot = new Pot();
            var players = new List<Player>
            {
                new Player("Player1", 1000),
                new Player("Player2", 1000),
                new Player("Player3", 1000)
            };
            
            // 使用Bet方法设置玩家的下注
            players[0].Bet(50);
            players[1].Bet(75);
            players[2].Bet(100);

            // Act
            pot.CollectBets(players);

            // Assert
            Assert.Equal(225, pot.TotalAmount);
            Assert.All(players, player => Assert.Equal(0, player.CurrentBet));
        }

        [Fact]
        public void Payout_GivesAllChipsToWinnerAndResetsPot()
        {
            // Arrange
            var pot = new Pot();
            var winner = new Player("Winner", 1000);
            pot.AddAmount(300);

            // Act
            pot.Payout(winner);

            // Assert
            Assert.Equal(1300, winner.Chips);
            Assert.Equal(0, pot.TotalAmount);
        }

        [Fact]
        public void PayoutTie_DividesChipsAmongWinnersAndHandlesRemainder()
        {
            // Arrange
            var pot = new Pot();
            var winners = new List<Player>
            {
                new Player("Winner1", 1000),
                new Player("Winner2", 1000),
                new Player("Winner3", 1000)
            };
            pot.AddAmount(325); // 325 / 3 = 108 remainder 1

            // Act
            pot.PayoutTie(winners);

            // Assert
            Assert.Equal(1109, winners[0].Chips); // Gets 108 + 1 remainder = 109 more chips
            Assert.Equal(1108, winners[1].Chips); // Gets 108 chips
            Assert.Equal(1108, winners[2].Chips); // Gets 108 chips
            Assert.Equal(0, pot.TotalAmount);
        }

        [Fact]
        public void PayoutTie_WithZeroWinners_DoesNothing()
        {
            // Arrange
            var pot = new Pot();
            var winners = new List<Player>();
            pot.AddAmount(100);

            // Act
            pot.PayoutTie(winners);

            // Assert
            Assert.Equal(100, pot.TotalAmount); // Pot should remain unchanged
        }

        [Fact]
        public void Reset_SetsTotalAmountToZero()
        {
            // Arrange
            var pot = new Pot();
            pot.AddAmount(500);

            // Act
            pot.Reset();

            // Assert
            Assert.Equal(0, pot.TotalAmount);
        }
    }
}