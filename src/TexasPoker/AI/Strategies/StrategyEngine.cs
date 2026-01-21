using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using TexasPoker.AI.Analyst;
using TexasPoker.Enums;
using TexasPoker.Models;

namespace TexasPoker.AI.Strategies
{
    public class StrategyEngine
    {
        public static readonly Random random = new Random();

        public static PlayerAction Decide(NPCProfile npc, AnalysisResult analysisResult, TableContext context) {
            // 1. 结合性格计算“胆量因子”
            float courage = analysisResult.WinProbability + (npc.Aggression * 0.15f);

            // 2.考虑赔率: 是否值得跟注
            // 如果胜率 > 赔率，说明数学上是划算的
            bool isPotOddsGood = analysisResult.WinProbability > analysisResult.PotOdds;

            // ---- 决策逻辑 ----

            // A. 强牌 / 极度自信 -> 加注
            if (courage >= 0.80f)
            {
                // 简单的加注策略：底池的 50% ~ 100%
                int raiseAmount = (int)(context.PotSize * (0.5f + npc.Aggression / 2));

                // 确保加注额合法：至少是 MinRaiseAmount
                if (raiseAmount < context.MinRaiseAmount)
                    raiseAmount = context.MinRaiseAmount;

                return new PlayerAction(PlayerActionType.Raise, raiseAmount);
            }

            // B. 中等牌力 -> 跟注 或 看牌
            // 如果胆量还行，或者赔率很好
            if (courage > 0.5f || (isPotOddsGood && courage > 0.3f))
            {
                // 如果能免费看牌，那就看牌
                if (context.CanCheck)
                {
                    return new PlayerAction(PlayerActionType.Check);
                } else
                {
                    // 如果需要花钱跟注，再次确认赔率是否划算
                    // 如果赔率极好，或者赔率还行但是不想放弃
                    if (isPotOddsGood || courage > 0.6f)
                    {
                        return new PlayerAction(PlayerActionType.Call);
                    }
                }
            }

            // C. 诈唬逻辑
            // 只有在能 Check 或者 对手下注不大 的时候尝试偷鸡
            // 如果对手下重注，通常不建议用弱牌诈唬
            bool isCheapToBluff = context.AmountToCall < context.PotSize * 0.3f;
            if (isCheapToBluff && random.NextDouble() < npc.BluffFrequency)
            {
                // 打底池的80%
                int bluffAmount = (int)(context.PotSize * 0.8f);
                return new PlayerAction(PlayerActionType.Raise, bluffAmount);
            }

            // D. 实在没有牌也没有胆 -> 弃牌
            // 但是有免费牌的话，还是要看一看
            if (context.CanCheck)
            {
                return new PlayerAction(PlayerActionType.Check);
            }

            return new PlayerAction(PlayerActionType.Fold);
        }
    }
}