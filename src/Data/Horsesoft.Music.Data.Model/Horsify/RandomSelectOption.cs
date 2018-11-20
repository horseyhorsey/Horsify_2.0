namespace Horsesoft.Music.Data.Model.Horsify
{
    public class RandomSelectOption
    {
        /// <summary>
        /// Creates option for selecting random amounts with rating.
        /// </summary>
        /// <remarks>
        /// Rating is checked if lower is higher than the higher value
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="rangeLower"></param>
        /// <param name="rangeUpper"></param>
        public RandomSelectOption(int value, bool? ratingEnabled, byte rangeLower, byte rangeUpper)
        {
            this.Amount = value;

            if (rangeLower > rangeUpper)
                rangeUpper = rangeLower;

            RatingRange = new RangeFilterOption<byte>(rangeLower, rangeUpper);
            RatingRange.IsEnabled = ratingEnabled ?? false;
        }

        public int Amount { get; }

        public RangeFilterOption<byte> RatingRange { get; set; }
    }
}
