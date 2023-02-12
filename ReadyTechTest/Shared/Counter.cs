namespace ReadyTechTest.API.Shared
{
    //bit hacky, would store it in a db if it needs to be persisted, something like redis 
    public static class Counter
    {
        private static int Count = 0;

        public static int IncrementCount()
        {
            Count++;
            return Count;
        }

        public static int ResetCount()
        {
            Count = 0; 
            return Count;
        }
    }
}
