using _301173198_Nahar__Lab3.Models;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _301173198_Nahar__Lab3.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        IConfiguration configuration;
        DbContext context;
        public MovieController(IConfiguration configuration)
        {
            this.configuration = configuration;
            context = new DbContext(configuration);

        }
        public async Task<IActionResult> Index()
        {

            return View(await context.GetAllByUserId(User.Identity.Name));
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0 && Request.Form.Files["file"].FileName != null)
                {
                    var credentials = new BasicAWSCredentials(configuration["AccesskeyID"], configuration["Secretaccesskey"]);
                    var config = new AmazonS3Config
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.CACentral1
                    };

                    using var client = new AmazonS3Client(credentials, config);
                    var bucketName = "movies-list-bucket";
                    var fileExtension = Path.GetExtension(Request.Form.Files["file"].FileName);
                    var movieName = $"{Request.Form.Files["file"].FileName}";
                    // URL for Accessing Document for Demo
                    var result = $"https://{bucketName}.s3.amazonaws.com/{movieName}";

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = Request.Form.Files["file"].OpenReadStream(),
                        Key = movieName,
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    //upload

                    movie.FileName = movieName;
                }
                movie.MovieId = Guid.NewGuid().ToString();
                movie.UserId = User.Identity.Name;
                if(movie.Comment != null)
                {
                    movie.CommentDate = DateTime.Now.ToString();
                }
                await context.SaveAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await context.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(string id, string fileName)
        {
            
            try
            {

                var credentials = new BasicAWSCredentials(configuration["AccesskeyID"], configuration["Secretaccesskey"]);
                var bucketName = "movies-list-bucket";

                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.CACentral1
                };
                using var client = new AmazonS3Client(credentials, config);


                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };
                await client.DeleteObjectAsync(deleteObjectRequest);
                if (id == null)
                {
                    return NotFound();
                }

                var movie = await context.GetByIdAsync(id);
                if (movie == null)
                {
                    return NotFound();
                }
                return View(movie);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movie = await context.GetByIdAsync(id);

            await context.DeleteAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await context.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0 && Request.Form.Files["file"].FileName != null)
                {
                    var credentials = new BasicAWSCredentials(configuration["AccesskeyID"], configuration["Secretaccesskey"]);
                    var config = new AmazonS3Config
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.CACentral1
                    };


                    using var client = new AmazonS3Client(credentials, config);
                    var bucketName = "movies-list-bucket";
                    var fileExtension = Path.GetExtension(Request.Form.Files["file"].FileName);
                    var movieName = $"{Request.Form.Files["file"].FileName}";
                    // URL for Accessing Document for Demo
                    var result = $"https://{bucketName}.s3.amazonaws.com/{movieName}";

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = Request.Form.Files["file"].OpenReadStream(),
                        Key = movieName,
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    //upload

                    movie.FileName = movieName;
                }
                 if(movie.CommentDate != null && movie.Comment != null)
                {
                    if((DateTime.Now - DateTime.Parse(movie.CommentDate)).TotalHours < 1.00)
                    {
                        movie.CommentDate = DateTime.Now.ToString();

                    } 
                } else
                {
                    movie.CommentDate = DateTime.Now.ToString();
                }
                await context.Update(movie);

                return RedirectToAction(nameof(Index));
            }
            return View(movie);//DateTime.Now.Subtract(DateTime.Now).TotalHours > 24
        }

        public async Task<IActionResult> DownloadMovie(string filename)
        {
            try
            {
                var credentials = new BasicAWSCredentials(configuration["AccesskeyID"], configuration["Secretaccesskey"]);
                var bucketName = "movies-list-bucket";

                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.CACentral1
                };
                using var client = new AmazonS3Client(credentials, config);
                var fileTransferUtility = new TransferUtility(client);

                var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filename
                });

                if (objectResponse.ResponseStream == null)
                {
                    return NotFound();
                }
                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, filename);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }

        }

    }
}