using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_App.Models;

namespace MVC_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadCSV(IFormFile file)
        {
            HashSet<string> emailUniqueSet = new HashSet<string>();
            var stream = new StreamReader(file.OpenReadStream());
            List<string> emailList = null;

            HttpContext.Session.SetString("Error", "");

            int emailIndex;

            // get headers
            if (stream.Peek() != -1)
            {
                string[] headerArray = stream.ReadLine().Split(',');
                emailIndex = Array.IndexOf(headerArray, "email_address");
            }
            else
            {
                _logger.LogError("UploadCSV: Empty file given.");
                HttpContext.Session.SetString("Error", "File cannot be empty.");
                return View(nameof(Index));
            }
            

            if (emailIndex != -1)
            {
                emailList = new List<string>();
                while (stream.Peek() != -1)
                {
                    string row = stream.ReadLine();
                    string[] rowArray = row.Split(',');
                    if (rowArray.Length > emailIndex)
                    {
                        string email = rowArray[emailIndex];

                        // checking against a hashset for faster access time
                        if (!emailUniqueSet.Contains(email))
                        {
                            emailUniqueSet.Add(email);
                            emailList.Add(email);
                        }
                        else
                        {
                            _logger.LogInformation("UploadCSV: Encountered duplicate email: {0}", email);
                        }
                          
                    }
                    else
                    {
                        if (row.Trim() == "")
                        {
                            _logger.LogInformation("UploadCSV: Skipping blank line.");
                        }
                        else
                        {
                            _logger.LogError("UploadCSV: Line did not have enough columns. Encountered the following text from file: {0}", row);        
                        }
                    }
                    
                } // end while
                emailList.Sort();
            }
            else
            {
                HttpContext.Session.SetString("Error", "Email header not found. Expected header: \"email_address\"");
                _logger.LogError("Error: Email header not found. Expected header: \"email_address\"");
            }

            return View(nameof(Index), emailList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
