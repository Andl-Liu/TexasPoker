using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Models;

namespace TexasPoker.Logic
{
    // 底池
    public class Pot
    {
        public int TotalAmount { get; private set; }

        public void AddAmount(int amount)
        {
            TotalAmount += amount;
        }

        // 从所有玩家处收集本轮下注的筹码
        public void CollectBets(List<Player> players) {
            foreach (var player in players) {
                TotalAmount += player.CurrentBet;
                player.ClearCurrentBet();
            }
        }

        // 结算：将筹码给赢家
        public void Payout(Player winner) {
            winner.AddChips(TotalAmount);
            Reset();
        }

        // 平局：将筹码平分给所有玩家
        public void PayoutTie(List<Player> winners) {
            if (winners.Count == 0) return;
            var chipsPerPlayer = TotalAmount / winners.Count;
            foreach (var player in winners) {
                player.AddChips(chipsPerPlayer);
            }
            // 剩余筹码，分给第一个玩家
            winners.First().AddChips(TotalAmount % winners.Count);
            Reset();
        }

        // 重置
        public void Reset() {
            TotalAmount = 0;
        }
    }
}