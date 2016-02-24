//===================================================================
//=																	=
//= Copyright (c) 2016 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Configuration;

namespace FindMerge
{
    /// <summary>
    /// Class to manage MergeExisting operation.
    /// </summary>
    class Merge
    {
        NapierClient api;
        string userName;
        string password;
        string bulkURL;

        public Merge() {
            api = new NapierClient();
            userName = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];
            bulkURL = ConfigurationManager.AppSettings["BulkURL"];
        }

        /// <summary>
        /// Finds and merges contacts with the supplied email address. Uses BulkFind and MergeExisting.
        /// </summary>
        /// <param name="email">The email address of the contacts to merge.</param>
        public void ProcessFindMerge(string email){
            List<Resource> resources = new List<Resource>();
            MergeSpecification mergeResources = new MergeSpecification();

            string query = "/api/bulk/2.0/find/Contact?fields=Id,FirstName,Title,LastName,Email&query=Email='" + email + "'&orderBy=Id ASC";
           
            resources = FindResources(bulkURL + query, userName, password);

            string[] ids = new string[resources.Count];
            int count = 0;
           
            //Get the ids
            foreach (var resource in resources){
                var id = resource
                        .Field
                        .First(fvp => fvp.Id == "Id")
                        .Value;
                ids[count++] = id;
            }
            // Winner is the id with the lowest id value (the Bulk Find query sorts its result in ascending order).
            mergeResources.WinnerId = ids[0];
            mergeResources.IdsToMerge = ids.Skip(1).ToArray();

            MergeExistingResponse mergeResponse = null;
            try{
                mergeResponse = api.MergeExisting("Contact", new[] { mergeResources }, null);
            }
            catch(Exception e){
                throw new NexxusException("MergeExisting API Call failed", e, NexxusOperations.MergeExisting);
            }

            if (mergeResponse.BatchCompleted){
                foreach (var result in mergeResponse.Results){
                    if (result.OperationSucceeded){
                        if (result.Results.Count()==0){
                            Console.WriteLine("No resources provided to merge into {0}", result.WinnerId);
                            continue;
                        }

                        Console.WriteLine("The following resources attempted to merged into {0}:", result.WinnerId);

                            // result.Resource contains the merged resource
                        foreach (var operationResult in result.Results){
                            Console.Write("Id: {0}. Result: ", operationResult.Id);
                            if (operationResult.OperationSucceeded){
                                Console.WriteLine("Success");
                            }
                            else{
                                Console.WriteLine("Failed");
                            }
                        }
                    }
                    else{
                        Console.WriteLine("The specified resource could not be merged.");
                        Console.WriteLine(result.ErrorString + " " + result.ErrorMessage);
                    }
                }
            }
            else{
                throw new NexxusException(mergeResponse.ErrorString,
                    mergeResponse.ErrorMessage,
                    mergeResponse.RequestId,
                    NexxusOperations.MergeExisting);
            }
        }
        /// <summary>
        /// Finds resources that match the supplied query url. Returns a list of Resource objects containing the query results. 
        /// </summary>
        /// <param name="url">Nexxus Marketing site URL and NQL query. </param>
        /// <param name="userName">Nexxus Marketing instance user name.</param>
        /// <param name="password">Nexxus Marketing instance user password.</param>
        /// <returns></returns>
        protected List<Resource> FindResources(string url, string userName, string password) {
            List<Resource> resources = new List<Resource>();
            WebClient wc = new WebClient();
            //Set up basic authorization.
            var credential = userName + ":" + password;
            wc.Headers["Authorization"] =
                "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));
 
            try {
                //Make the query and download to file.
                string result = wc.DownloadString(url);
                resources = Hydrate.HydrateBulkResources(result);

                if (resources.Count == 0) {
                    Exception webEx = new Exception("Call successful: No results found");
                    throw new NexxusException(NexxusErrorCodes.IdsNotFound, 
                        "No resource(s) in the query were found  - " + url,
                        wc.ResponseHeaders["NexusRequestId"],
                        NexxusOperations.BulkFind);
                }
            }
            catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    string nexxusErrorString = e.Response.Headers[BulkHeaderFields.NexusErrorString];
                    if (nexxusErrorString != null) {
                        throw new NexxusException(nexxusErrorString,
                            e,
                            NexxusOperations.BulkFind);
                    }
                    else {
                    }
                    Console.WriteLine("Operation Failed : " + e.Message);
                }
                else {
                    Console.WriteLine(e.Message);
                }
                throw e;
            }
            catch (Exception e) {
                throw e;
            }
            return resources;
        }
     }
}
