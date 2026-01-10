using System;
using System.Collections.Generic;
using TexasPoker.Logic;
using TexasPoker.Models;
using TexasPoker.Enums;
using Xunit;

namespace TexasPoker.Tests.Logic
{
    public class BettingManagerTests
    {
        private BettingManager _bettingManager;
        private Player _player1;
        private Player _player2;
        private Player _player3;

        public BettingManagerTests()
        {
            _bettingManager = new BettingManager(10, 20); // 小盲注10，大盲注20
            _player1 = new Player("Player1", 1000);
            _player2 = new Player("Player2", 1000);
            _player3 = new Player("Player3", 1000);
        }

        [Fact]
        public void Constructor_SetsBlindValuesCorrectly()
        {
            Assert.Equal(10, _bettingManager.SmallBlind);
            Assert.Equal(20, _bettingManager.BigBlind);
        }

        [Fact]
        public void ResetRound_ResetsValuesCorrectly()
        {
            // 通过Raise操作设置初始值
            _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 100, out _);
            
            _bettingManager.ResetRound();

            Assert.Equal(0, _bettingManager.CurrentMaxBet);
            Assert.Equal(20, _bettingManager.LastRaiseAmount); // Should be set to BigBlind
        }

        [Fact]
        public void PostBlinds_SetsCorrectBets()
        {
            // 重置玩家状态
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            _bettingManager.PostBlinds(_player1, _player2);

            Assert.Equal(10, _player1.CurrentBet);
            Assert.Equal(20, _player2.CurrentBet);
            Assert.Equal(20, _bettingManager.CurrentMaxBet);
        }

        [Fact]
        public void GetAmountToCall_ReturnsCorrectAmount()
        {
            // 先重置玩家状态
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            // 设置玩家1的下注
            _player1.Bet(10);
            
            // 使用Raise操作设置CurrentMaxBet
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 30, out _);

            var amountToCall = _bettingManager.GetAmountToCall(_player1);

            Assert.Equal(20, amountToCall); // 30 - 10 = 20
        }

        [Fact]
        public void TryProcessAction_CheckWhenNoBetToCall_Succeeds()
        {
            _player1.RestForNewHand(); // 重置玩家状态
            
            // 模拟无下注的情况
            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Check, 0, out string errorMessage);

            Assert.True(result);
            Assert.Equal(string.Empty, errorMessage);
        }

        [Fact]
        public void TryProcessAction_CheckWhenBetToCall_Fails()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            // 设置一个下注情况，玩家1下注10
            _player1.Bet(10);
            
            // 使用Raise操作设置CurrentMaxBet
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 30, out _);

            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Check, 0, out string errorMessage);

            Assert.False(result);
            Assert.Equal("必须跟注，不能看牌", errorMessage);
        }

        [Fact]
        public void TryProcessAction_CallWhenNoBetToCall_Fails()
        {
            _player1.RestForNewHand();
            
            // 通过下注操作设置当前玩家的下注
            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Call, 0, out string errorMessage);

            Assert.False(result);
            Assert.Equal("没有需要跟注的筹码", errorMessage);
        }

        [Fact]
        public void TryProcessAction_CallWhenBetToCall_Succeeds()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            // 设置玩家1的当前下注
            _player1.Bet(10);
            
            // 使用Raise操作设置CurrentMaxBet
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 30, out _);

            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Call, 0, out string errorMessage);

            Assert.True(result);
            Assert.Equal(30, _player1.CurrentBet);
        }

        [Fact]
        public void TryProcessAction_RaiseWithInsufficientAmount_Fails()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            // 首先通过Raise操作设置初始状态
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 30, out _);
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 50, out _); // This sets LastRaiseAmount to 20

            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 40, out string errorMessage);

            Assert.False(result);
            Assert.Contains("加注额不足", errorMessage);
        }

        [Fact]
        public void TryProcessAction_RaiseWithSufficientAmount_Succeeds()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            
            // 设置初始状态
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 30, out _);
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Raise, 60, out _); // This sets LastRaiseAmount to 30

            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 90, out string errorMessage);

            Assert.True(result);
            Assert.Equal(90, _player1.CurrentBet);
            Assert.Equal(90, _bettingManager.CurrentMaxBet);
            Assert.Equal(30, _bettingManager.LastRaiseAmount); // 90 - 60 = 30
        }

        [Fact]
        public void TryProcessAction_Fold_Succeeds()
        {
            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.Fold, 0, out string errorMessage);

            Assert.True(result);
            Assert.False(_player1.IsActive);
        }

        [Fact]
        public void TryProcessAction_AllIn_Succeeds()
        {
            var result = _bettingManager.TryProcessAction(_player1, PlayerActionType.AllIn, 0, out string errorMessage);

            Assert.True(result);
            Assert.True(_player1.IsAllIn);
        }

        [Fact]
        public void IsBettingRoundOver_AllActivePlayersHaveSameBet_ReturnsTrue()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            _player3.RestForNewHand();
            
            // 设置所有玩家的赌注
            _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 100, out _);
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Call, 0, out _);
            _bettingManager.TryProcessAction(_player3, PlayerActionType.Call, 0, out _);

            var result = _bettingManager.IsBettingRoundOver(new List<Player> { _player1, _player2, _player3 });

            Assert.True(result);
        }

        [Fact]
        public void IsBettingRoundOver_NotAllActivePlayersHaveSameBet_ReturnsFalse()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            _player3.RestForNewHand();
            
            // 设置不同赌注
            _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 100, out _);
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Call, 0, out _);
            // Player3 doesn't call, remains at 50 bet

            var result = _bettingManager.IsBettingRoundOver(new List<Player> { _player1, _player2, _player3 });

            Assert.False(result);
        }

        [Fact]
        public void IsBettingRoundOver_WithFoldedPlayer_ReturnsTrueIfOthersMatch()
        {
            _player1.RestForNewHand();
            _player2.RestForNewHand();
            _player3.RestForNewHand();
            
            _bettingManager.TryProcessAction(_player1, PlayerActionType.Raise, 100, out _);
            _bettingManager.TryProcessAction(_player2, PlayerActionType.Call, 0, out _);
            // Player3 will fold
            _bettingManager.TryProcessAction(_player3, PlayerActionType.Fold, 0, out _);

            var result = _bettingManager.IsBettingRoundOver(new List<Player> { _player1, _player2, _player3 });

            Assert.True(result); 
        }
    }
}