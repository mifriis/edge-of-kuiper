namespace Kuiper.Services
{
    public interface IDiceRoller
    {
        bool D6(int target, int modifier);
        bool D12(int target, int modifier);
        bool D20(int target, int modifier);


        /// <summary>Randomly between 1 and 100, is the outcome less than your chance plus modifier? Higher target the more likely to happen</summary>
        bool D100(int target, int modifier);
    }
    
}