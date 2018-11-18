namespace Horsesoft.Music.Data.Model.Horsify
{
    public class RandomSelectOption
    {
        private int value;
        private int rangeLower;
        private int rangeUpper;

        /// <summary>
        /// Creates option for selecting random amounts with rating.
        /// </summary>
        /// <remarks>
        /// Rating is checked if lower is higher than the higher value
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="rangeLower"></param>
        /// <param name="rangeUpper"></param>
        public RandomSelectOption(int value, int rangeLower, int rangeUpper)
        {
            this.Amount = value;

            if (rangeLower > rangeUpper)
                rangeUpper = rangeLower;

            this.rangeLower = rangeLower;
            this.rangeUpper = rangeUpper;
        }

        public int Amount { get; }
        public int RatingLower { get; }
        public int RatingHigher { get; }
    }
}
