using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301173198_Nahar__Lab3.Models
{

    [DynamoDBTable("MovieTable")]
    public class Movie
    {
        [DynamoDBHashKey]
        public string MovieId { get; set; }
        [DynamoDBRangeKey]
        public string UserId { get; set; }
        public double Rating { get; set; }
        public string MovieTitle { get; set; }
        public string Genre { get; set; }
        public List<string> Cast { get; set; }
        public string FileName { get; set; }
        public int Year { get; set; }
        public string Comment { get; set; }
        public string CommentDate { get; set; }
    }
}
