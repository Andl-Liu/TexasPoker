using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TexasPoker.Enums;

namespace TexasPoker.Models
{
    // 玩家动作
    public class PlayerAction
    {
        public PlayerActionType Type { get; set; }
        public int Amount { get; set; }

        public PlayerAction(PlayerActionType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}