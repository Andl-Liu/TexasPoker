using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TexasPoker.Models
{
    public class Player
    {
        public string Name { get; set; }
        // 总筹码数
        public int Chips { get; private set;}
        // 当前下注
        public int CurrentBet { get; private set; }
        // 玩家手牌（2张）
        public List<Card> Hand { get; private set;}
        // 是否是玩家
        public bool IsPlayer { get; set; } = false;

        // 状态标志
        // 是否弃牌
        public bool IsFold { get; set; }
        // 是否AllIn
        public bool IsAllIn { get; private set; }
        // 是否还在局中
        public bool IsActive => !IsFold;

        // 创建玩家
        public Player(string name, int initialChips) {
            Name = name;
            Chips = initialChips;
            Hand = [];
            RestForNewHand();
        }

        public Player(string name, int initialChips, bool isPlayer) {
            Name = name;
            Chips = initialChips;
            Hand = [];
            IsPlayer = isPlayer;
            RestForNewHand();
        } 

        // 每一局新开始时的重置
        public void RestForNewHand() {
            Hand.Clear();
            CurrentBet = 0;
            IsFold = false;
            IsAllIn = false;
        }

        // 下注逻辑
        public void Bet(int amount) {
            // 筹码不足，强制all in
            if (amount > Chips) {
                amount = Chips;
                IsAllIn = true;
            }

            Chips -= amount;
            CurrentBet += amount;
        }

        // 赢得筹码
        public void AddChips(int amount) {
            Chips += amount;
        }

        // 弃牌
        public void Fold() {
            IsFold = true;
        }

        // all in
        public void AllIn() {
            Bet(Chips);
            IsAllIn = true;
        }

        // 重置本轮下注
        public void ClearCurrentBet() {
            CurrentBet = 0;
        }
    }
}