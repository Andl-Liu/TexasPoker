using System.Collections.Generic;
using TexasPoker.Enums;
using TexasPoker.Models;

namespace TexasPoker.Tests.Models
{
    public class PlayerTests
    {
        [Fact]
        public void Player_CreatePlayer_PlayerInitializedCorrectly()
        {
            // Arrange & Act
            var player = new Player("TestPlayer", 1000);

            // Assert
            Assert.Equal("TestPlayer", player.Name);
            Assert.Equal(1000, player.Chips);
            Assert.Equal(0, player.CurrentBet);
            Assert.False(player.IsFold);
            Assert.False(player.IsAllIn);
            Assert.True(player.IsActive);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void Player_ResetForNewHand_ResetsPlayerState()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);
            player.Hand.Add(new Card(Suit.Heart, Rank.Ace));
            player.Hand.Add(new Card(Suit.Spade, Rank.King));
            player.Bet(100);
            player.Fold();

            // Act
            player.RestForNewHand();

            // Assert
            Assert.Empty(player.Hand);
            Assert.Equal(0, player.CurrentBet);
            Assert.False(player.IsFold);
            Assert.False(player.IsAllIn);
            Assert.True(player.IsActive);
        }

        [Fact]
        public void Player_Bet_LowersChipsAndSetsCurrentBet()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);

            // Act
            player.Bet(100);

            // Assert
            Assert.Equal(900, player.Chips);
            Assert.Equal(100, player.CurrentBet);
        }

        [Fact]
        public void Player_BetMoreThanChips_GoesAllIn()
        {
            // Arrange
            var player = new Player("TestPlayer", 500);

            // Act
            player.Bet(1000);

            // Assert
            Assert.Equal(0, player.Chips);
            Assert.Equal(500, player.CurrentBet);
            Assert.True(player.IsAllIn);
        }

        [Fact]
        public void Player_AddChips_IncreasesChips()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);

            // Act
            player.AddChips(500);

            // Assert
            Assert.Equal(1500, player.Chips);
        }

        [Fact]
        public void Player_Fold_SetsIsFoldToTrue()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);
            Assert.True(player.IsActive); // Initially active

            // Act
            player.Fold();

            // Assert
            Assert.True(player.IsFold);
            Assert.False(player.IsActive);
        }

        [Fact]
        public void Player_AllIn_GoesAllIn()
        {
            // Arrange
            var player = new Player("TestPlayer", 750);

            // Act
            player.AllIn();

            // Assert
            Assert.Equal(0, player.Chips);
            Assert.Equal(750, player.CurrentBet);
            Assert.True(player.IsAllIn);
        }

        [Fact]
        public void Player_ClearCurrentBet_ResetsCurrentBetToZero()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);
            player.Bet(200); // CurrentBet is now 200

            // Act
            player.ClearCurrentBet();

            // Assert
            Assert.Equal(0, player.CurrentBet);
        }

        [Fact]
        public void Player_HandProperty_IsInitiallyEmpty()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);

            // Assert
            Assert.NotNull(player.Hand);
            Assert.Empty(player.Hand);
        }

        [Fact]
        public void Player_AddCardsToHand_AddsCardsSuccessfully()
        {
            // Arrange
            var player = new Player("TestPlayer", 1000);
            var card1 = new Card(Suit.Heart, Rank.Ace);
            var card2 = new Card(Suit.Spade, Rank.King);

            // Act
            player.Hand.Add(card1);
            player.Hand.Add(card2);

            // Assert
            Assert.Equal(2, player.Hand.Count);
            Assert.Contains(card1, player.Hand);
            Assert.Contains(card2, player.Hand);
        }
    }
}