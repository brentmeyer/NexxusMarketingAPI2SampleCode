//===================================================================
//=																	=
//= Copyright (c) 2016 IMS Health Incorporated. All rights reserved.=
//=																	=
//===================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace FindMerge {
    /// <summary>
    /// General purpose exception class for Nexxus Marketing Bulk REST and SOAP operations.
    /// </summary>
    [Serializable]
    public class NexxusException : Exception {

        public string ErrorMessage;
        public string NexxusErrorString;
        public string NexxusRequestId;
        public string NexxusApiJobId;
        public int NexxusOperationType;
        public string NexxusOperation;

        //general purpose exception constructor.
        public NexxusException(string errorString, string errorMessage, string nexusRequestId,  string operation) {
            ErrorMessage = errorMessage;
            NexxusErrorString = errorString;
            NexxusRequestId = nexusRequestId;
            HelpLink = "http://nexxusdocs.imshealth.com/nexxusmarketing/api2/default.htm";
            Source = "Nexxus Exception Sample Code";
            NexxusOperation = operation;
        }

       
        // Constructor for Bulk REST exception.
        public NexxusException(string errorString, WebException inner, string operation)
            : base(errorString, inner) {
            char[] delimiters = new char[] { ':' };
            var resp = new StreamReader(inner.Response.GetResponseStream()).ReadToEnd();

            // Extract the error message from the error string & error message combination.
            // Assume XXXX denotes the start of a BULK API Error message.
            // : divides the error string from the error message.
            if (resp.IndexOf("####") > -1) {
                int divider = resp.IndexOf(':');
                if (divider > -1) {
                    ErrorMessage = resp.Substring(divider + 2);
                }
                else {
                    ErrorMessage = resp;
                }
            }
            else {
                ErrorMessage = resp;
            }

            WebHeaderCollection headers = inner.Response.Headers;
            NexxusErrorString = headers[BulkHeaderFields.NexusErrorString];
            NexxusRequestId = headers[BulkHeaderFields.NexusRequestId];
            NexxusApiJobId = headers[BulkHeaderFields.NexusApiJobId];

            HelpLink = "http://nexxusdocs.imshealth.com/nexxusmarketing/api2/default.htm";
            Source = "Nexxus Exception Sample Code";
        }


        // Constructor to support innner Exception.
        public NexxusException(string errorMessage, Exception inner, string operation)
            : base(errorMessage, inner) {
                ErrorMessage = errorMessage;
                NexxusErrorString = inner.Message;
                Console.WriteLine(inner.GetType());
           
            HelpLink = "http://nexxusdocs.imshealth.com/nexxusmarketing/api2/default.htm";
            Source = "Nexxus Exception Sample Code";
            NexxusOperation = operation;
        }
        // Uses to write custom properties during serialization.
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {

            info.AddValue(BulkHeaderFields.ErrorMessage, this.ErrorMessage);
            info.AddValue(BulkHeaderFields.NexusErrorString, this.NexxusErrorString);
            info.AddValue(BulkHeaderFields.NexusRequestId, this.NexxusRequestId);
            info.AddValue(BulkHeaderFields.NexusApiJobId, this.NexxusApiJobId);
            info.AddValue(NexxusAPIOperationInfo.Operation, this.NexxusOperation);
            base.GetObjectData(info, context);
        }
        // Used to read custom properties dueing serialization.
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected NexxusException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            ErrorMessage = info.GetString(BulkHeaderFields.ErrorMessage);
            NexxusErrorString = info.GetString(BulkHeaderFields.NexusErrorString);
            NexxusRequestId = info.GetString(BulkHeaderFields.NexusRequestId);
            NexxusApiJobId = info.GetString(BulkHeaderFields.NexusApiJobId);
            NexxusOperation = info.GetString(NexxusAPIOperationInfo.Operation);
        }
    }
}
