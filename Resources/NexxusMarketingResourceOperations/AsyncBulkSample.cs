using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;


namespace NexxusMarketingAPISamples
{
    class AsyncBulkSample
    {
        //Maps BulkJob resource 
        enum BulkJob
        {
            Id = 0,
            JobEntityType,
            Operation,
            OperationMetaData,
            Created,
            CreatedBy,
            Updated,
            Status,
            AbortedBy,
            TotalRows,
            SuccessRows,
            ErrorRows,
            ErrorCode,
            ErrorMessage
        }
        string bulkJobId = null;
        bool completed = false;
        string userName = null;
        string password = null;
        string url = null;
        string fileName = null;

        public void Start(string bulkURL, string file, string user, string pass)
        {
            userName = user;
            password = pass;
            url = bulkURL;
            fileName = file;
            
            Task status = new Task(StatusAsync);
            Task createWithFile = new Task(CreateWithFileAsync);

            status.Start();
            createWithFile.Start();
            while (!completed)
            {
                ; //wait until completed.
            }

        }
        //Keeps track of the current status of the bulk job.
        async void StatusAsync()
        {
            completed = false;

            while (!completed)
            {
                int count = 0;
                while (bulkJobId == null)
                {
                    Console.WriteLine("Waiting for job Identifier...");
                    System.Threading.Thread.Sleep(1000);
                    if (count++ == 60) //Wait a minute before giving up.
                    {
                        Console.WriteLine("Taking too long - quitting");
                        return;
                    }
                }
                if (bulkJobId == "Failed")
                {
                    Console.WriteLine("Bulk job failed to start");
                    return;
                }

                using (var client = new HttpClient())
                {
                    var credential = userName + ":" + password;
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/text"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credential)));

                    HttpResponseMessage response = await client.GetAsync("/api/bulk/2.0/find/BulkJob?query=ID%3D" + bulkJobId);
                    if (response.IsSuccessStatusCode){
                        string result = await response.Content.ReadAsStringAsync();
                        string[] bulkJobs = result.Split('\n');

                        string[] bulkJob = bulkJobs[1].Split('|');  //0 element is the header column

                        Console.WriteLine("ID: " + bulkJob[(int)BulkJob.Id] + " Operation: " + bulkJob[(int)BulkJob.Operation] + " Status: " + bulkJob[(int)BulkJob.Status]);

                        string status = bulkJob[(int)BulkJob.Status].Replace("\"", "");
                        if (status == "Aborted" || status == "Failed" || status == "Completed"){
                            Console.WriteLine("\r\nFinished: " + status);
                            completed = true;
                        }
                        if (!completed && (status != "Uploaded" && status != "Uploading" && status != "Running" && status != "Aborting" && status != "PendingRestart")){
                            Console.WriteLine("Unrecognized status: " + status);
                            completed = true;
                        }
                    }
                    else{
                        Console.WriteLine(response.StatusCode.ToString());
                        Console.WriteLine(response.Headers.ToString());
                    }
                }
            }
        }

        //Starts the bulk creation operation.
        async void CreateWithFileAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                //Authentication information
                var credential = userName + ":" + password;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credential)));

                // Header Content configuration
                var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                streamContent.Headers.ContentDisposition.Name = "\"file\"";
                streamContent.Headers.ContentDisposition.FileName = "\"" + fileName + "\"";
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                string boundary = Guid.NewGuid().ToString();
                var fContent = new MultipartFormDataContent(boundary);
                fContent.Headers.Remove("Content-Type");
                fContent.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                fContent.Add(streamContent);

                // Make REST call
                HttpResponseMessage response = null;
                try
                {
                    response = await client.PostAsync("/api/bulk/2.0/Create/Contact", fContent);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed: " + e.Message);
                    bulkJobId = "Failed";
                    return;
                }

                streamContent.Dispose();
                fileStream.Dispose();

                if (response.IsSuccessStatusCode)
                {


                    //Get the bulk job identifier for ongoing status checks
                    foreach (var header in response.Headers)
                    {
                        if (header.Key == "NexusApiJobId")
                            bulkJobId = header.Value.First();
                    }

                    Console.WriteLine("Bulk Job: " + bulkJobId + " started");
                }
                else
                {
                    Console.WriteLine("Failed: " + response.StatusCode.ToString());
                    Console.WriteLine(response.Headers.ToString());
                    bulkJobId = "Failed";
                }
            }
        }

    }



}
