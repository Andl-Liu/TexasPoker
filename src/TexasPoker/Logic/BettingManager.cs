using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Models;
using TexasPoker.Enums;

namespace TexasPoker.Logic
{
    // 下注管理
    public class BettingManager
    {
        // 小盲注
        public int SmallBlind { get; }
        // 大盲注
        public int BigBlind { get; }

        // 本轮当前最高的下注额度
        public int CurrentMaxBet { get; private set; }
        // 最后一次加注的幅度
        public int LastRaiseAmount { get; private set; }
        // 最小加注额
        public int MinRaiseAmount => CurrentMaxBet + Math.Max(SmallBlind, BigBlind);

        public BettingManager(int smallBlind, int bigBlind)
        {
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
        }

        // 每一轮开始前重置
        public void ResetRound() 
        {
            CurrentMaxBet = 0;
            LastRaiseAmount = BigBlind;
        }

        // 处理盲注，仅在Pre-flop开始时调用
        public void PostBlinds(Player sbPlayer, Player bbPlayer) {
            sbPlayer.Bet(SmallBlind);
            bbPlayer.Bet(BigBlind);

            CurrentMaxBet = BigBlind;
        }

        // 计算跟注额度：玩家还需要放多少筹码才能齐平
        public int GetAmountToCall(Player player) {
            return CurrentMaxBet - player.CurrentBet;
        }

        // 验证并执行下注动作
        public bool TryProcessAction(Player player, PlayerActionType actionType, int amount, out string errorMessage) {
            errorMessage = string.Empty;
            int toCall = GetAmountToCall(player);

            switch (actionType) {
                case PlayerActionType.Check:
                    if (toCall > 0) {
                        errorMessage = "必须跟注，不能看牌";
                        return false;
                    }
                    break;

                case PlayerActionType.Call:
                    if (toCall == 0) {
                        errorMessage = "没有需要跟注的筹码";
                        return false;
                    }
                    player.Bet(toCall);
                    break;

                case PlayerActionType.Raise:
                    // amount 是玩家想要加注到的总额
                    int minTotal = MinRaiseAmount;
                    if (amount < minTotal && amount < player.Chips + player.CurrentBet) {
                        errorMessage = $"加注额不足，至少要加注到 {minTotal}";
                        return false;
                    }

                    // 计算差额
                    player.Bet(amount - player.CurrentBet);
                    // 更新当前最大注额
                    LastRaiseAmount = amount - CurrentMaxBet;
                    CurrentMaxBet = amount;
                    break;

                case PlayerActionType.Fold:
                    player.Fold();
                    break;

                case PlayerActionType.AllIn:
                    // 计算玩家AllIn后的总下注额（当前已下注 + 剩余筹码）
                    int finalBetAmount = player.CurrentBet + player.Chips;

                    // 执行扣款和状态变更
                    player.AllIn();

                    // 如果All-In金额超过了当前最大注额，视为Raise行为
                    if (finalBetAmount > CurrentMaxBet)
                    {
                        int allInRaiseIncrement = finalBetAmount - CurrentMaxBet;
                        if (allInRaiseIncrement >= LastRaiseAmount)
                        {
                            LastRaiseAmount = allInRaiseIncrement;
                        }
                        CurrentMaxBet = finalBetAmount;
                    }

                    break;
            }

            return true;
        }

        // 判断下注轮是否结束
        // 所有活跃玩家的CurrentBet相等,为CurrentMaxBet
        // 或者已经all in
        public bool IsBettingRoundOver(List<Player> players) {
            int activePlayer = 0;
            int finishedPlayer = 0;

            foreach (var player in players) {
                if (player.IsActive) {
                    activePlayer++;
                    if (player.CurrentBet == CurrentMaxBet || player.IsAllIn) {
                        finishedPlayer++;
                    }
                }
            }

            return activePlayer == finishedPlayer;
        }
    }
}