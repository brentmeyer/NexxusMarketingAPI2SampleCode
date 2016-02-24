//===================================================================
//=																	=
//= Copyright (c) 2015 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexxusMarketingAPISamples {
    /// <summary>
    /// These samples are used in the API documentation
    /// </summary>
    public class NapierSamples {
        public void ShowSample() {
            var api = new NapierClient();
            var showResponse = api.Show();

            if (showResponse.BatchCompleted) {
                foreach (var typeId in showResponse.TypeIds) {
                    Console.WriteLine("ID: " + typeId);
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(showResponse.ErrorMessage);
            }
        }

        public void DescribeSample() {
            var api = new NapierClient();
            var descriptionResponse = api.Describe("Contact");

            if (descriptionResponse.BatchCompleted) {
                Console.WriteLine("\nFields:");
                foreach (var field in descriptionResponse.Fields) {
                    Console.WriteLine("Field Name: " + field.FieldId);
                    Console.WriteLine("Type: " + field.Type);
                    Console.WriteLine("Max Length: " + field.MaxLength);
                    Console.WriteLine("IsPrimaryKey: " + field.IsPrimaryKey);
                    Console.WriteLine("Default: " + field.Default);
                    Console.WriteLine("IsNullable: " + field.IsNullable);
                    Console.WriteLine("IsReadOnly: " + field.IsReadOnly);
                    Console.WriteLine("IsImmutable: " + field.IsImmutable);

                    var allowedVals = field.AllowedValues ?? new string[] { };
                    Console.WriteLine("\nAllowedValues: " + String.Join(",", allowedVals));
                }
                Console.WriteLine("\nIndexes:");
                foreach (var index in descriptionResponse.Indexes) {
                    Console.WriteLine("Index Name: " + index.Name);
                    Console.WriteLine("Constraint: " + index.Constraint);
                }
                Console.WriteLine("\nAllowed operations:");
                foreach (var op in descriptionResponse.AllowedOperations) {
                    Console.WriteLine(op);
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(descriptionResponse.ErrorMessage);
            }
        }

        public void GetSample() {
            var api = new NapierClient();

            // fetch a Contact with the identifier <CONTACTID> 
            //
            var typeId = "Contact";
            var ids = new string[] { "<CONTACTID>"};

            var getResponse = api.Get(typeId, ids);
            if (getResponse.BatchCompleted) {
                foreach(var resourceResult in getResponse.Resources)
                {
                    if (resourceResult.OperationSucceeded){
                        var noNullFVPList = from item in resourceResult.Resource.Field
                                            where item != null
                                            select item;
                        Console.WriteLine("\r\nContact\r\n");

                        foreach (var field in noNullFVPList){
                            Console.WriteLine("Field: " + field.Id + " Value: " + field.Value);
                        }
                    }
                    else{
                        Console.WriteLine("\r\nError: " + resourceResult.ErrorMessage);
                    }
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(getResponse.ErrorMessage);
            }
        }

        public void CreateSample() {
            var api = new NapierClient();

            // Creates and normalizes a new Contact record. Note that no
            // Primary Key is specified for this Contact since we don't know
            // what the Primary Key is yet. (The Primary Key may not be set
            // when calling Create().)
            var typeId = "Contact";
            var fieldValuePairs = new[] {
                new FieldValuePair { Id = "FirstName", Value = "Robert" },
                new FieldValuePair { Id = "LastName", Value = "Jones" },
                new FieldValuePair { Id = "Email", Value = "rj@fake.com" },
                new FieldValuePair { Id = "ExternalContactId", Value = "EID11" }
            };
            var resource = new Resource {
                Field = fieldValuePairs
            };

            var createResponse = api.Create(typeId, new[] { resource });

            if (createResponse.BatchCompleted) {
                var result = createResponse.Resources[0];

                // Retrieve the Primary Key for the created Contact
                if (result.OperationSucceeded) {
                    var primaryKey = result.Resource
                                            .Field
                                            .First(fvp => fvp.Id == "Id")
                                            .Value;
                    Console.WriteLine("Contact created with Id: " + primaryKey);
                } else {
                    Console.WriteLine("The Contact could not be created.");
                    Console.WriteLine(result.ErrorMessage);
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(createResponse.ErrorMessage);
            }
        }

        public void UpdateSample() {
            var api = new NapierClient();

            // Updates a pre-existing Contact in Nexxus Marketing. Only the
            // fields that are specified will be modified.

            var typeId = "Contact";
            var fieldValuePairs = new[] {
                new FieldValuePair { Id = "Id", Value = "<CONTACTID>" },
                new FieldValuePair { Id = "Email", Value = "user@newhost.com" }
            };
            var resource = new Resource {
                Field = fieldValuePairs
            };

            // Update based on the primary key field
            var updateResponse = api.Update(typeId, new[] { resource }, IndexId: null);

            if (updateResponse.BatchCompleted) {
                var result = updateResponse.Resources[0];

                // Retrieve the updated value from the response
                if (result.OperationSucceeded) {

                    var noNullFVPList = from item in result.Resource.Field
                                        where item != null
                                        select item;

                    var newValue = noNullFVPList
                                            .First(fvp => fvp.Id == "Email")
                                            .Value;
                    Console.WriteLine("Email updated to: " + newValue);
                } else {
                    Console.WriteLine("The Contact could not be updated.");
                    Console.WriteLine(result.ErrorMessage);
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(updateResponse.ErrorMessage);
            }
        }

        public void UpsertSample() {
            var api = new NapierClient();

            // Creates and normalizes a new Contact record
            // (assuming that a Contact with the 'ExternalContactId'
            // of 'EID10393' does not exist) with the following
            // attributes.
            var typeId = "Contact";
            var indexId = "ExternalContactId";
            var fieldValuePairs = new[] {
                new FieldValuePair { Id = indexId, Value = "EID10393" },
                new FieldValuePair { Id = "FirstName", Value = "Jon" },
                new FieldValuePair { Id = "LastName", Value = "Jones" },
                new FieldValuePair { Id = "Email", Value = "user@host.com" }
            };
            var resourceCreate = new Resource {
                Field = fieldValuePairs
            };

            // Updates the 'FirstName' on the Contact created in
            // the previous Upsert request to 'Robert' because
            // the 'ExternalContactId' is the same between both records.
            fieldValuePairs = new[] {
                new FieldValuePair { Id = indexId, Value = "EID10393" },
                new FieldValuePair { Id = "FirstName", Value = "Jonathon" },
            };
            var resourceUpdate = new Resource {
                Field = fieldValuePairs
            };

            var upsertResponse =
                api.Upsert(
                    typeId,
                    indexId, new[] { resourceCreate, resourceUpdate }
                );

            if (upsertResponse.BatchCompleted) {
                if (upsertResponse.Resources.All(r => r.OperationSucceeded)) {
                    // should be 'Create'
                    Console.WriteLine(upsertResponse.Resources[0].UpsertResult);
                    // should be 'Update'
                    Console.WriteLine(upsertResponse.Resources[1].UpsertResult);
                } else {
                    Console.WriteLine(
                        "One or more of the Contacts could not be upserted"
                    );
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(upsertResponse.ErrorMessage);
            }
        }

        public void DeleteSample() {
            var api = new NapierClient();

            // fetch a set of Contact records identified by
            // Primary Key Ids 5, 7, and 9
            var typeId = "Contact";
            var ids = new string[] { "<CONTACTID>"};

            var deleteResponse = api.Delete(typeId, ids);
            if (deleteResponse.BatchCompleted) {
                // The primary key of the deleted key is returned. This code
                // displays a message for each successfully deleted Contact.
                foreach (var result in deleteResponse.Results) {
                    if (result.OperationSucceeded) {
                        Console.WriteLine(
                            "The Contact with Id {0} was deleted.",
                            result.Id
                        );
                    }
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(deleteResponse.ErrorMessage);
            }
        }

        public void MatchSample()
        {
            var api = new NapierClient();

            // External contact
                var fieldValuePairs = new[] {
            new FieldValuePair { Id = "LastName", Value = "Jones" },
            new FieldValuePair { Id = "MiddleName", Value = "Ian" },
            new FieldValuePair { Id = "Email", Value = "rj@fake.com" }
            };
            var resource = new Resource
            {
                Field = fieldValuePairs
            };

            var typeId = "Contact";
            // Call Match without explicitly specifying config;
            // this will use the default config for the given typeId.
            var matchResponse = api.Match(typeId, new[] { resource }, null, null);
            if (matchResponse.BatchCompleted)
            {
                foreach (var result in matchResponse.Results)
                {
                    if (result.OperationSucceeded)
                    {
                        if (result.BestMatchType == DeduplicationMatchType.New)
                        {
                            Console.WriteLine("No matches found; this resource has no duplicates.");
                        }
                        else
                        {
                            if (result.BestMatchType == DeduplicationMatchType.PotentialDuplicate)
                            {
                                Console.WriteLine("The resource might or might not be a duplicate." +
                                                    "This suggests a human will need to examine " +
                                                    "the data to make a definitive decision");
                                Console.WriteLine("The SmartMerge operation would treat this as New");
                            }
                            else if (result.BestMatchType == DeduplicationMatchType.Duplicate)
                            {
                                Console.WriteLine("A duplicate was found in the database.");
                            }
                            else if (result.BestMatchType == DeduplicationMatchType.InternalDuplicate)
                            {
                                Console.WriteLine("A duplicate was found in the input provided to Match");
                            }

                            var bestMatch = result.BestMatchResource;

                            Console.WriteLine("Here is the BestMatchResource for the operation.");
                            Console.WriteLine("Normally, this would now be used as input to a Merge() " +
                                                "call, to Merge these duplicates into a single entity and " +
                                                "persist the results.");
                            Console.WriteLine("Note that if this is an InternalDuplicate, " +
                                                "you will need to Create one resource and merge " +
                                                "the duplicate with the newly-created resource. " +
                                                "If this seems annoying, consider using SmartMerge.");

                            // Create a list with the null entries removed.           
                            var noNullFVPList = from item in bestMatch.Field
                                                where item != null
                                                select item;

                            foreach (var kvp in noNullFVPList)
                            {
                                Console.WriteLine("Key: '{0}', Value: '{1}'.", kvp.Id, kvp.Value);
                            }


                            Console.WriteLine("This Match call found a total of {0} matches in the DB " +
                                                "and {1} self-matches in the input. These may be inspected " +
                                                "if more control over the process is desired, but usually " +
                                                "the BestMatchResource is what you want here.",
                                                result.MatchDetails.Count(),
                                                result.InternalMatchDetails.Count());
                        }
                    }
                    else
                    {
                        Console.WriteLine("The specified resource could not be matched.");
                        Console.WriteLine(result.ErrorMessage);
                    }
                }
            }
            else
            {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(matchResponse.ErrorMessage);
            }
        }
          
        public void MergeSample() {
            var api = new NapierClient();

            var typeId = "Contact";
            var fieldValuePairs = new[] {
                new FieldValuePair { Id = "LastName", Value = "Jones" },
                new FieldValuePair { Id = "MiddleName", Value = "Ian" },
                new FieldValuePair { Id = "Email", Value = "rjones@fake.com" }
            };
            var resource = new Resource {
                Field = fieldValuePairs
            };

            // Through some other channel -- likely a Match() call -- we've decided that
            // the Resource above should be merged with some entity already in the DB,
            // and we know the Id of that entity.

            var id = "1263663";

            // Call Merge without explicitly specifying config;
            // this will use the default config for the given typeId. 
            var mergeResponse = api.Merge(typeId, new[] { resource }, new[] { id }, null);
            if (mergeResponse.BatchCompleted) {
                foreach (var result in mergeResponse.Resources) {
                    if (result.OperationSucceeded) {
                        Console.WriteLine("Provided resource successfully merged with DB entity.");
                        // result.Resource contains the merged resource
                    } else {
                        Console.WriteLine("The specified resource could not be merged.");
                        Console.WriteLine(result.ErrorMessage);
                    }
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(mergeResponse.ErrorMessage);
            }
        }

        public void SmartMergeSample() {
            var api = new NapierClient();

            var typeId = "Contact";
            var fieldValuePairs = new[] {
                new FieldValuePair { Id = "LastName", Value = "Jones" },
                new FieldValuePair { Id = "City", Value = "Seattle" },
                new FieldValuePair { Id = "Email", Value = "rjones@fake.com" }
            };
            var resource = new Resource {
                Field = fieldValuePairs
            };

            // Use default config for all 3 phases (search/match/merge)
            var upsertResponse = api.SmartMerge(typeId, new[] { resource }, null, null, null);
            if (upsertResponse.BatchCompleted) {
                foreach (var result in upsertResponse.Resources) {
                    if (result.OperationSucceeded) {
                        if (result.UpsertResult == UpsertResult.Create) {
                            Console.WriteLine("No match found, resource was created.");
                        } else if (result.UpsertResult == UpsertResult.Update) {
                            Console.WriteLine("Match found, resource was merged.");
                        }

                        // Either way, result.Resource contains the resource that was persisted
                    } else {
                        Console.WriteLine("The specified resource had an error.");
                        Console.WriteLine(result.ErrorMessage);
                    }
                }
            } else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(upsertResponse.ErrorMessage);
            }
        }

        public void MergeExistingSample() {
            var api = new NapierClient();
            MergeSpecification mergeResources = new MergeSpecification();

            var winner = "<WINNERID>";
            var loser1 = "<LOSERID1>";
            var loser2 = "<LOSERID2>";

            mergeResources.WinnerId = winner;
            mergeResources.IdsToMerge = new[] { loser1, loser2 };
            var mergeResponse = api.MergeExisting("Contact", new[] { mergeResources }, null);
            if (mergeResponse.BatchCompleted) {
                foreach (var result in mergeResponse.Results) {
                    if (result.OperationSucceeded) {
                        Console.WriteLine("The following resources attempted to merged into {0}:", result.WinnerId);

                        foreach (var operationResult in result.Results) {
                            Console.Write("Id: {0}. Result: ", operationResult.Id);
                            if (operationResult.OperationSucceeded) {
                                Console.WriteLine("Success");
                            }
                            else {
                                Console.WriteLine("Failed");
                            }
                        }
                    }
                    else {
                        Console.WriteLine("The specified resource could not be merged.");
                        Console.WriteLine(result.ErrorString + " " + result.ErrorMessage);
                    }
                }
            }
            else {
                Console.WriteLine("The operation could not be performed.");
                Console.WriteLine(mergeResponse.ErrorMessage);
            }
        }

        public void GetServerTimestamp() {
            var api = new NapierClient();
            var timestampResponse = api.GetServerTimestamp();

            if (timestampResponse.BatchCompleted) {
                var timestamp = timestampResponse.ServerTimestamp;

                Console.WriteLine(
                    "The Server's Timestamp is currently: " +
                    timestamp.ToString("u")
                );
            }
        }
    }
}
