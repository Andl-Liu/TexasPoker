using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;
using TexasPoker.Models;

namespace TexasPoker.Logic
{
    // 一套扑克
    // 洗牌和发牌逻辑
    public class Deck
    {
        // 存放52张牌的容器
        private readonly List<Card> cards;
        private readonly Random rng;

        public Deck() {
            cards = [];
            rng = new Random();
            Reset();
        }

        // 初始化牌组
        public void Reset() {
            cards.Clear();
            // 每个花色
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                // 每个点数
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }
        }

        // 洗牌算法：Fisher-Yates Shuffle
        public void Shuffle() {
            int n = cards.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                (cards[n], cards[k]) = (cards[k], cards[n]);
            }
        }

        // 发牌
        public Card Draw() {
            if (cards.Count == 0) throw new Exception("No more cards");
            Card card = cards[^1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }
        
        // 获取剩余牌的数量 - 用于测试
        public int GetCardCount() => cards.Count;
        
        // 获取所有牌的副本 - 用于测试
        public List<Card> GetAllCards() => [..cards];
    }
}