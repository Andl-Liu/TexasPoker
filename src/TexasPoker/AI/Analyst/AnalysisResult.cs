using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TexasPoker.AI.Analyst
{
    public struct AnalysisResult
    {
        // 胜率(0.0 ~ 1.0)
        public float WinProbability;
        // 当前牌力评分
        public float HandStrength;
        // 底池赔率
        public float PotOdds;
        // 补牌数（能够让我提升牌型的剩余数量）
        public float OutsCount;
    }
}