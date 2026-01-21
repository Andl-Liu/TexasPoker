using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TexasPoker.Models
{
    // 牌桌上下文
    // 做一个数据快照，避免多线程问题
    public class TableContext
    {

        // ---- 基础信息 ----
        // 当前阶段
        public string GamePhase;
        // 当前总底池
        public int PotSize;
        // 公共牌数量
        public int CommunityCardsCount;

        // ---- 玩家信息 ----
        // 当前玩家筹码
        public int MyChips;
        // 当前玩家已下注金额
        public int MyCurrentBet;
        // 需要跟注多少
        public int AmountToCall;
        // 最小加注到目标总额
        public int MinRaiseAmount;

        // ---- 辅助属性 ----
        // 是否可以看牌（即不需要跟注）
        public bool CanCheck => AmountToCall == 0;
        // 是否可以加注
        public bool CanRaise(int amount) => MyChips >= amount && amount >= MinRaiseAmount;
        // 是否可以AllIn
        public bool CanAllIn => MyChips > 0;
    }
}