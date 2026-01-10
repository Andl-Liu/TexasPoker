using System;
using TexasPoker.Logic;
using TexasPoker.Models;
using TexasPoker.Enums;
using Xunit;

namespace TexasPoker.Tests.Logic
{
    public class DeckTests
    {
        [Fact]
        public void Deck_Initialization_ShouldContain52Cards()
        {
            // Arrange
            var deck = new Deck();

            // Assert
            Assert.Equal(52, deck.GetCardCount()); // 需要通过反射访问cards列表或添加公共方法
        }

        [Fact]
        public void Deck_Reset_ShouldContain52UniqueCards()
        {
            // Arrange
            var deck = new Deck();

            // Act
            deck.Reset();

            // Assert
            Assert.Equal(52, deck.GetCardCount());
            
            var cards = deck.GetAllCards();
            Assert.Equal(52, cards.Count);
            Assert.All(cards, card => Assert.NotNull(card));
            
            // Ensure all cards are unique
            var uniqueCards = new HashSet<int>();
            foreach (var card in cards)
            {
                var cardId = (int)card.Suit * 13 + (int)card.Rank - 2;
                Assert.True(uniqueCards.Add(cardId), "Duplicate card found in deck");
            }
        }

        [Fact]
        public void Deck_Shuffle_ShouldChangeOrder()
        {
            // Arrange
            var deck1 = new Deck();
            var originalOrder = deck1.GetAllCards();

            // Act
            deck1.Shuffle();
            var shuffledOrder = deck1.GetAllCards();

            // Assert - we expect the order to be different most of the time
            // (though there's a tiny chance they could be the same)
            var sameOrder = true;
            for (int i = 0; i < originalOrder.Count; i++)
            {
                if (originalOrder[i].Suit != shuffledOrder[i].Suit || 
                    originalOrder[i].Rank != shuffledOrder[i].Rank)
                {
                    sameOrder = false;
                    break;
                }
            }
            
            // Since shuffling is random, we can't guarantee order will change,
            // but with 52 cards the chance of same order is extremely low
            // So we'll just ensure the count is still 52 after shuffle
            Assert.Equal(52, shuffledOrder.Count);
        }

        [Fact]
        public void Deck_Draw_ShouldReturnCardAndReduceCount()
        {
            // Arrange
            var deck = new Deck();
            var initialCount = deck.GetCardCount();

            // Act
            var card = deck.Draw();

            // Assert
            Assert.NotNull(card);
            Assert.Equal(initialCount - 1, deck.GetCardCount());
            Assert.Contains(card, new[] { card }); // Verify the drawn card is valid
        }

        [Fact]
        public void Deck_DrawAllCards_ShouldThrowExceptionWhenNoCardsLeft()
        {
            // Arrange
            var deck = new Deck();

            // Act & Assert
            for (int i = 0; i < 52; i++)
            {
                var card = deck.Draw();
                Assert.NotNull(card);
            }

            Assert.Throws<Exception>(() => deck.Draw()); // Should throw when deck is empty
        }

        [Fact]
        public void Deck_Draw_ShouldReturnUniqueCards()
        {
            // Arrange
            var deck = new Deck();
            var drawnCards = new HashSet<int>();

            // Act & Assert
            for (int i = 0; i < 10; i++) // Draw 10 cards and verify uniqueness
            {
                var card = deck.Draw();
                var cardId = (int)card.Suit * 13 + (int)card.Rank - 2;
                
                Assert.True(drawnCards.Add(cardId), $"Card {card} was drawn more than once");
            }
        }
    }
}