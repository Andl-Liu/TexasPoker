using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TexasPoker.Models
{
    // NPC 配置模型
    [System.Serializable]
    public class NPCProfile
    {
        public string Name;

        // 激进程度： 决定加注的频率和额度
        [Range(0, 1)]
        public float Aggression;

        // 诈唬频率： 在牌差的时候偷鸡的概率
        [Range(0, 1)]
        public float BluffFrequency;

        // 耐心/紧度： 决定角色愿意参与游戏的起手牌范围
        [Range(0, 1)]
        public float Patience;

        // 动态情绪偏移
        public float CurrentMoodBias;
    }
}