namespace Kuiper.Services
{
    public static class DiceFactory
    {
        private static IDiceRoller dicerollerInstance;
        private static IRandom randomInstance;

        public static IDiceRoller Roller()
        {
            if(dicerollerInstance != null)
                return dicerollerInstance;
            dicerollerInstance = new DiceRoller(Random());
            return dicerollerInstance;
        }

        public static IRandom Random()
        {
            if(randomInstance != null)
                return randomInstance;
            randomInstance = new Random();
            return randomInstance;
        }

        public static void setRandomInstance(IRandom random)
        {
            randomInstance = random;
        }

        public static void setDiceRollerInstance(IDiceRoller diceRoller)
        {
            dicerollerInstance = diceRoller;
        }
    }
    
}