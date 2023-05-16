using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesctopApp.Models
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
}
