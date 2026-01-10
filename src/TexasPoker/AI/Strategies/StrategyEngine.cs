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
        public static PlayerAction Decide(NPCProfile npc, AnalysisResult analysisResult) {
            // 1. 结合性格计算“胆量因子”
            float courage = analysisResult.WinProbability + (npc.Aggression * 0.2f);

            // 2.考虑赔率
            // 如果赔率极好，即使勇气一般，也会选择跟注
            bool goodPotOdds = analysisResult.WinProbability > analysisResult.PotOdds;

            // 3.基础决策树
            if (courage >= 0.85f) return new PlayerAction(PlayerActionType.Raise, (int)(analysisResult.PotOdds * 10f));
            if (courage > 0.6f) return new PlayerAction(PlayerActionType.Call, 0);

            // 如果赔率划算且不是太离谱，尝试Check
            if (goodPotOdds && courage > 0.40f) return new PlayerAction(PlayerActionType.Check, 0);

            // 诈唬
            if (Random.Shared.NextDouble() < npc.BluffFrequency) {
                Debug.Print($"诈唬：{npc.Name} StrategyEngine.Decide()");
                return new PlayerAction(PlayerActionType.Raise, (int)(analysisResult.PotOdds * 10f));
            }

            return new PlayerAction(PlayerActionType.Fold, 0);
        }
    }
}