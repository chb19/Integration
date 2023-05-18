using Microsoft.ML.Data;

namespace ML
{
    public class NewsRating
    {
        [LoadColumn(0)]
        public float userId;
        [LoadColumn(1)]
        public float NewsId;
        [LoadColumn(2)]
        public float Label;
    }

    public class NewsRatingPrediction
    {
        public float Label;
        public float Score;
    }
}
