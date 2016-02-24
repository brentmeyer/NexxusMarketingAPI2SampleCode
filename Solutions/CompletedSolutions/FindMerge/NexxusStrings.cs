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

namespace FindMerge {

    struct NexxusOperations {
        public const string BulkFind = "BulkFind";
        public const string BulkCreate = "BulkCreate";
        public const string BulkDelete = "BulkDelete";
        public const string BulkUpdate = "BulkUpdate";
        public const string BulkAbort = "BulkAbort";
        public const string BulkResults = "BulkResults";
        public const string Show = "Show";
        public const string Describe = "Describe";
        public const string Create = "Create";
        public const string Delete = "Delete";
        public const string Get = "Get";
        public const string Match = "Match";
        public const string Merge = "Merge";
        public const string MergeExisting = "MergeExisting";
        public const string SmartMerge = "SmartMerge";
        public const string Update = "Update";
        public const string Upsert = "Upsert";
    }

    struct NexxusErrorCodes {
        public const string IdsNotFound = "ID(S)_NOT_FOUND";
        public const string InvalidFileCount = "INVALID_FILE_COUNT";
        public const string InvalidQueryStringParameter = "INVALID_QUERY_STRING_PARAMETER";
        public const string EncodingNotSupported = "ENCODING_NOT_SUPPORTED";
        public const string MissingHeader = "MISSING_HEADER";
        public const string InvalidVersion = "INVALID_VERSION";
        public const string InsecureConnection = "InsecureConnection";
        public const string MissingAuthorizationCredential = "MissingAuthorizationCredential";
        public const string InvalidCredentials = "InvalidCredentials";
        public const string UnauthorizedAccess = "UnauthorizedAccess";

        public const string InvalidTypeId = "INVALID_TYPE_ID";
        public const string InvalidConfigId = "INVALID_CONFIG_ID";
        public const string MismatchedMergeInputs = "MISMATCHED_MERGE_INPUTS";
        public const string OperationNotSupported = "OPERATION_NOT_SUPPORTED";
        public const string OperationTimeout = "OPERATION_TIMEOUT";
        public const string MismatchedFieldType = "MISMATCHED_FIELD_TYPE";
        public const string ServerBusy = "SERVER_BUSY";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";

    }

    struct BulkHeaderFields {
        public const string NexusErrorString = "NexusErrorString";
        public const string NexusRequestId = "NexusRequestId";
        public const string ErrorMessage = "ErrorMessage";
        public const string NexusApiJobId = "NexusApiJobId";
    }

    struct NexxusAPIOperationInfo {
        public const string Operation = "NexxusOperation";

    }

}
