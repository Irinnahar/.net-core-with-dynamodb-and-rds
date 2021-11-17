using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301173198_Nahar__Lab3.Models
{
    public class DbContext
    {
        DynamoDBContext context;
        public DbContext(IConfiguration configuration)
        {
            var accessKeyID = configuration["AccesskeyID"];
            var secretKey = configuration["Secretaccesskey"];
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
            var client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
            context = new DynamoDBContext(client);
        }

        public async Task<IEnumerable<Movie>> GetAllByUserId(string userId)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("UserId", ScanOperator.Equal, userId));
            var res = await context.ScanAsync<Movie>(scanConditions).GetRemainingAsync();
            return res;
        }
        public async Task<IEnumerable<Movie>> GetByTitleAsync(string title)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("MovieTitle", ScanOperator.Equal, title));
            var res = await context.ScanAsync<Movie>(scanConditions).GetRemainingAsync();
            return res;
        }
        public async Task<Movie> GetByIdAsync(string id)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("MovieId", ScanOperator.Equal, id));
            var entity = await context.ScanAsync<Movie>(scanConditions).GetRemainingAsync();
            return entity.FirstOrDefault();
        }

     
       /* public async Task<IEnumerable<Movie>> GetByGenreAsync(string genre)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Genre", ScanOperator.Equal, genre));
            var res = await context.ScanAsync<Movie>(scanConditions).GetRemainingAsync();
            return res;
        }*/
        public async Task SaveAsync(Movie item)
        {
            await context.SaveAsync(item);
        }
        public async Task DeleteAsync(Movie item)
        {
            await context.DeleteAsync(item);
        }
        public async Task<IEnumerable<Movie>> GetAll()
        {
            var conditions = new List<ScanCondition>();
            var res = await context.ScanAsync<Movie>(conditions).GetRemainingAsync();
            return res;
        }
        public async Task Update(Movie item)
        {
            context.DeleteAsync(item).GetAwaiter().GetResult();
            await context.SaveAsync(item);
        }

        /* public async Task<IEnumerable<Movie>> GetByRating(double rating, string userId)
         {
             var scanConditions = new List<ScanCondition>();
             scanConditions.Add(new ScanCondition("Rating", ScanOperator.GreaterThanOrEqual, rating));
             scanConditions.Add(new ScanCondition("UserId", ScanOperator.Equal, userId));
             var res = await context.ScanAsync<Movie>(scanConditions).GetRemainingAsync();
             return res;
         }*/

        public async Task<IEnumerable<Movie>> GetMovieRank(string movieTitle)
        {
            var config = new DynamoDBOperationConfig
            {
                IndexName = "Rating-index"
            };
            return await context.QueryAsync<Movie>(movieTitle, config).GetRemainingAsync();
        }
    }
}
