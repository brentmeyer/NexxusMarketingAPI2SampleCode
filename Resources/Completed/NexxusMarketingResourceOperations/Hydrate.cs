using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexxusMarketingAPISamples {
    public static class Hydrate {
        /// <summary>
        /// Scan the input string to create seperate fields delimited by pipes. Pipe values within fields (within quotes) are kept. Escaped quotes are kept.
        /// </summary>
        /// <param name="resourceLine">A list of fields sperarated by pipes.</pa>
        /// <returns>A list of resource fields</returns>
        public static List<String> HydrateFields(string resourceLine) {
            bool escape = false;
            bool start = false;
            List<char> fieldChars = new List<char>();
            List<string> fields = new List<string>();

            foreach (var c in resourceLine) {
                if (c == '"' && escape == false) { //not escaped, quote must be start or end of field.
                    start = !start;
                    continue;
                }

                if (c == '|') {
                    if (start == true) { // pipe within a field; don't remove.
                        fieldChars.Add(c);
                        continue;
                    }
                    else {
                        //End of field found. Add it to fields list.
                        string fieldEnd = new string(fieldChars.ToArray());
                        // Console.WriteLine(fieldEnd);
                        fields.Add(fieldEnd);
                        fieldChars = new List<char>();
                        continue;
                    }
                }
                if (escape == true) { //keep the escaped character
                    fieldChars.Add(c);
                    escape = false;
                    continue;
                }
                //   escape = false;
                if (c == '\\') {
                    escape = true;
                }
                else fieldChars.Add(c);
            }
            // Add the last one
            string field = new string(fieldChars.ToArray());
            // Console.WriteLine(field);
            fields.Add(field);
            return fields;

        }

        /// <summary>
        /// Hydrates multiple resources from a Bulk Find operation
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<Resource> HydrateBulkResources(String result) {
            List<Resource> resources = new List<Resource>();
            Resource resource = null;

            //Break result into string array - one item for each resource.
            char[] delimiters = new char[] { '\r', '\n' };
            string[] resourceStrings = result.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);


            //Only return the first resource in result set.
            //Row 0 is headers. Row 1 is field values.

            List<string> headerFields = new List<string>();
            List<string> resourceFields = new List<string>();

            for (int n = 0; n < resourceStrings.Count(); n++) {
                if (n == 0) {
                    headerFields = HydrateFields(resourceStrings[n]);
                    continue;
                }

                resourceFields = HydrateFields(resourceStrings[n]);

                // build a Resource from second row
                FieldValuePair[] fvp = new FieldValuePair[headerFields.Count];
                for (int i = 0; i < headerFields.Count; i++) {
                    fvp[i] = new FieldValuePair();
                    fvp[i].Id = headerFields[i];
                    fvp[i].Value = resourceFields[i];
                }
                resource = new Resource {
                    Field = fvp
                };
                resources.Add(resource);
            }
            return resources;
        }

    }
}
