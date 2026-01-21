using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Logic;
using TexasPoker.Enums;
using TexasPoker.Models;
using System.Runtime.CompilerServices;

namespace TexasPoker.Controller
{
    public class TexasGameController
    {
        // -- 核心组件 --
        // 牌组
        private Deck _deck;
        // 投注规则管理
        private BettingManager _bettingManager;
        // pot
        private Pot _pot;
        // 牌局玩家
        private List<Player> _players;
        // 公共牌
        private List<Card> _communityCards;

        // -- 游戏状态 --
        public GamePhase CurrentPhase { get; private set; }
        // 庄家索引
        private int _dealerIndex = 0;
        // 游戏结束标志
        private bool _isGameOver = false;
        // 当前玩家
        private int _currentPlayerIndex = 0; 

        // -- 表现层接口 --
        // 表现层订阅这些事件来播放动画/更新UI
        
        /// <summary>
        /// 当游戏阶段改变时触发的事件
        /// </summary>
        /// <param name="gamePhase">新的游戏阶段</param>
        public event Action<GamePhase> OnPhaseChanged;
        
        /// <summary>
        /// 当向玩家发牌时触发的事件
        /// </summary>
        /// <param name="card">发出的牌</param>
        /// <param name="player">接收牌的玩家</param>
        public event Action<Card, Player> OnCardDealt;
        
        /// <summary>
        /// 当发出公共牌时触发的事件
        /// </summary>
        /// <param name="cards">发出的公共牌列表</param>
        public event Action<List<Card>> OnCommunityCardsDealt;
        
        /// <summary>
        /// 当需要玩家操作时触发的事件
        /// </summary>
        /// <param name="player">需要操作的玩家</param>
        /// <param name="string">需要显示的提示信息</param>
        public event Action<Player, string> OnActionRequired;
        
        /// <summary>
        /// 当玩家执行操作时触发的事件
        /// </summary>
        /// <param name="player">执行操作的玩家</param>
        /// <param name="actionType">执行的操作类型</param>
        /// <param name="amount">操作涉及的金额（如果有的话）</param>
        public event Action<Player, PlayerActionType, int> OnPlayerActionExecuted;
        
        public event Action<Player> OnTurnStarted;

        public event Action<BestHandResult, Player> OnShowdownResult;

        public event Action<Player, int> OnChipsUpdated;
        
        // 外部注入的动画等待逻辑
        // 这个Func允许Unity告知控制器：动画有没有播完
        public Func<string, Task> WaitAnimation;

        public TexasGameController(List<Player> players, int smallBlind, int bigBlind) {
            _players = players;
            _bettingManager = new BettingManager(smallBlind, bigBlind);
            _deck = new Deck();
            _pot = new Pot();
            _communityCards = new List<Card>();
        }

        // -- 主驱动循环 --
        public async Task StartGameLoop() {
            while (!_isGameOver) {
                await RunSingleHand();
                // 判断是否要退出游戏
                // 双人德州扑克，任何一个玩家的筹码数量小于等于0，则游戏结束
                if (_players.Any(p => p.Chips <= 0))
                    _isGameOver = true;
            }
        }

        // -- 单局游戏逻辑 --
        private async Task RunSingleHand() { 
            // 1. 初始化阶段
            PrepareNewHand();
            await NotifyPhase(GamePhase.Init);

            // 2. 底牌圈 PreFlop
            await NotifyPhase(GamePhase.PreFlop);
            // 下盲注
            _bettingManager.PostBlinds(_players[_dealerIndex], _players[(_dealerIndex + 1) % _players.Count]);
            // 通知表现层，更新筹码
            foreach (var player in _players) {
                OnChipsUpdated?.Invoke(player, player.Chips);
            }
            // 发两张底牌
            for (int i = 0; i < 2; i++) {
                foreach (var player in _players) {
                    var card = _deck.Draw();
                    player.Hand.Add(card);
                    OnCardDealt?.Invoke(card, player);
                    await Task.Delay(200); // 模拟发牌间隔
                }
            }
            // 开始下注
            await HandleBettingRound();

            // 3. 翻牌圈 Flop
            if (CanProceed())
                await RunPublicCardPhase(GamePhase.Flop, 3);

            // 4. 转牌圈 Turn
            if (CanProceed())
                await RunPublicCardPhase(GamePhase.Turn, 1);

            // 5. 河牌圈 River
            if (CanProceed())
                await RunPublicCardPhase(GamePhase.River, 1);

            // 6. 摊牌结算 Showdown
            if (CanProceed(true)) {
                await NotifyPhase(GamePhase.Showdown);
                // 处理摊牌
                ExecuteShowdown();
            }

            // 7. 清理阶段 Cleanup
            await NotifyPhase(GamePhase.Cleanup);
            await Task.Delay(2000);
        }

        // 初始化阶段
        private void PrepareNewHand() {
            // 重新初始化一套扑克
            _deck.Reset();
            // 洗牌
            _deck.Shuffle();
            // 清空公共牌
            _communityCards.Clear();
            // 清空pot
            _pot.Reset();
            // 重置投注规则
            _bettingManager.ResetRound();

            foreach (var player in _players) {
                // 玩家重置
                player.RestForNewHand();
            }

            // 交换庄家位置
            _dealerIndex = (_dealerIndex + 1) % _players.Count;
        } 

        // 通知表现层当前进入了哪个阶段，并等待表现层处理
        private async Task NotifyPhase(GamePhase phase) {
            CurrentPhase = phase;
            OnPhaseChanged?.Invoke(phase);

            if (WaitAnimation != null)
                await WaitAnimation(phase.ToString());
        }

        // -- 处理下注逻辑与等待输入 --
        private TaskCompletionSource<PlayerAction> _inputTask;

        // 外部调用方法 推进逻辑
        public void ReceiveAction(PlayerActionType actionType, int amount) {
            _inputTask.TrySetResult(new PlayerAction(actionType, amount));
        }

        private async Task HandleBettingRound() {
            // 只要下注还没平衡，且至少有两人在玩
            while (!_bettingManager.IsBettingRoundOver(_players) && _players.Count(p => p.IsActive) > 1) {
                // 现在是谁在说话
                Player player = GetCurrentPlayer();
                OnTurnStarted?.Invoke(player);

                // 等待输入
                _inputTask = new TaskCompletionSource<PlayerAction>();
                PlayerAction action = await _inputTask.Task; // 挂起线程，等待输入                

                string errorMessage;
                if (!_bettingManager.TryProcessAction(player, action.Type, action.Amount, out errorMessage)) {
                    // 输入有误，提示错误信息
                    OnActionRequired?.Invoke(player, errorMessage);
                    continue;
                }

                // 输入正确，执行操作
                OnPlayerActionExecuted?.Invoke(player, action.Type, action.Amount);
                OnChipsUpdated?.Invoke(player, player.Chips);
                AdvanceToNextPlayer();
            }

            // 收集赌注
            _pot.CollectBets(_players);
        }

        // 公共牌阶段
        private async Task RunPublicCardPhase(GamePhase phase, int count) {
            // 通知表现层
            await NotifyPhase(phase);

            // 发牌
            List<Card> cards = new List<Card>();
            for (int i = 0; i < count; i++) {
                var card = _deck.Draw();
                cards.Add(card);
                await Task.Delay(200); // 模拟发牌间隔
            }
            // 通知表现层
            OnCommunityCardsDealt?.Invoke(cards);

            // 进入下注阶段
            await HandleBettingRound();
        }

        // 处理摊牌
        // 只有两个玩家
        private void ExecuteShowdown() { 
            var activePlayers = _players.Where(p => p.IsActive).ToList();

            // 情况A：只有一个人存活
            if (activePlayers.Count == 1) {
                var winner = activePlayers[0];
                OnChipsUpdated?.Invoke(winner, winner.Chips);
                return;
            }

            // 情况B：有两个人存活
            // 计算玩家最佳手牌
            // 按照牌型和分数进行排序
            var showdownResults 
                = activePlayers.Select(p => new {
                        Player = p, 
                        BestFiveCards = HandEvaluator.GetBestHand(p.Hand.Concat(_communityCards).ToList())
                    })
                    .OrderByDescending(x => x.BestFiveCards.Score)
                    .ToList();
            // 判定赢家
            long winningScore = showdownResults[0].BestFiveCards.Score;
            var winners = showdownResults.Where(x => x.BestFiveCards.Score == winningScore).ToList();
            // 判断是否平局
            if (winners.Count > 1) {
                _pot.PayoutTie(winners.Select(x => x.Player).ToList());
            } else {
                _pot.Payout(winners[0].Player);
            }

            // 通知表现层
            foreach (var winner in winners) {
                OnShowdownResult?.Invoke(winner.BestFiveCards, winner.Player);
                OnChipsUpdated?.Invoke(winner.Player, winner.Player.Chips);
            }
        }

        // 获取当前玩家
        private Player GetCurrentPlayer() {
            return _players[_currentPlayerIndex];
        }

        // 推进到下一个玩家
        private void AdvanceToNextPlayer() {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        // 判断流程是否继续
        private bool CanProceed(bool isShowdown = false) {
            // 检查还在局中的玩家数量
            int activePlayerCount = _players.Count(p => p.IsActive);
            // 如果只有一个玩家，则游戏结束
            if (activePlayerCount <= 1)
                return false;

            // 如果是准备进入比牌阶段，且有多于一个玩家，则游戏继续
            if (isShowdown) 
                return true;

            // 如果有人all in, 并且其余在局中的玩家要么all in, 要么跟注
            // 也就是如果没有人可以再下注，则跳过阶段
            // 如果有人可以下注，则游戏继续
            if (_players.Any(p => p.IsAllIn)) {
                foreach (var player in _players) {
                    if (player.IsActive && !player.IsAllIn && player.CurrentBet < _bettingManager.CurrentMaxBet) {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        // ---- 获取当前牌桌的上下文 ----

        public TableContext GetTableContext()
        {
            return GetTableContext(GetCurrentPlayer());
        }

        public TableContext GetTableContext(Player player)
        {
            // 1.计算当前实时底池
            // 真实的底池 = Pot里的钱 + 这一轮桌上大家还没有收进Pot的筹码
            int currentRoundBets = _players.Sum(p => p.CurrentBet);
            int potSize = _pot.TotalAmount + currentRoundBets;

            // 2.获取下注规则数据
            int amountToCall = _bettingManager.GetAmountToCall(player);
            int minRaiseAmount = _bettingManager.MinRaiseAmount;

            // 3.生成快照
            return new TableContext
            {
                GamePhase = CurrentPhase.ToString(),
                PotSize = potSize,
                CommunityCardsCount = _communityCards.Count,
                MyChips = player.Chips,
                MyCurrentBet = player.CurrentBet,
                AmountToCall = amountToCall,
                MinRaiseAmount = minRaiseAmount
            };
       }
    }
}