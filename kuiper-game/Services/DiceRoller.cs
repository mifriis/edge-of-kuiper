using System;

namespace Kuiper.Services
{
    public class DiceRoller : IDiceRoller
    {
        private readonly IRandom _random;

        public DiceRoller(IRandom random)
        {
            _random = random;
        }

        public bool D12(int target, int modifier)
        {
            return RollDice(1,12,target,modifier);
        }

        public bool D20(int target, int modifier)
        {
            return RollDice(1,20,target,modifier);
        }

        public bool D6(int target, int modifier)
        {
           return RollDice(1,6,target,modifier);
        }

        public bool D100(int target, int modifier)
        {
            return RollDice(1,100,target,modifier);
        }

        private bool RollDice(int begin, int end, int target, int modifier)
        {
            if(target >= begin && target <= end)
            {
                if(_random.Next(begin,end) < target + modifier)
                    return true;
                else
                    return false;
            }
            throw new ArgumentOutOfRangeException($"Target must be between {begin} and {end}");
        }
    }
}