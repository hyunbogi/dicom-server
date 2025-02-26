// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Microsoft.Health.Dicom.Blob.Extensions;

internal static class AzureStorageErrorExtensions
{
    private static readonly List<BlobErrorCode> Customer403ErrorCodes = new List<BlobErrorCode>
    {
        BlobErrorCode.AuthorizationFailure,
        BlobErrorCode.AuthorizationPermissionMismatch,
        BlobErrorCode.InsufficientAccountPermissions,
        BlobErrorCode.AccountIsDisabled,
        "KeyVaultEncryptionKeyNotFound",
        "KeyVaultAccessTokenCannotBeAcquired",
        "KeyVaultVaultNotFound",
    };

    private static readonly List<BlobErrorCode> Customer404ErrorCodes = new List<BlobErrorCode>
    {
        BlobErrorCode.ContainerNotFound,
        "FilesystemNotFound",
    };

    private static readonly List<BlobErrorCode> Customer409ErrorCodes = new List<BlobErrorCode>
    {
        BlobErrorCode.ContainerBeingDeleted,
        BlobErrorCode.ContainerDisabled,
    };

    public static bool IsConnectedStoreCustomerError(this RequestFailedException rfe)
    {
        return (rfe.Status == 403 && Customer403ErrorCodes.Any(e => e.ToString().Equals(rfe.ErrorCode, StringComparison.OrdinalIgnoreCase))) ||
            (rfe.Status == 404 && Customer404ErrorCodes.Any(e => e.ToString().Equals(rfe.ErrorCode, StringComparison.OrdinalIgnoreCase))) ||
            (rfe.Status == 409 && Customer409ErrorCodes.Any(e => e.ToString().Equals(rfe.ErrorCode, StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsStorageAccountUnknownHostError(this Exception exception)
    {
        return exception.Message.Contains("No such host is known", StringComparison.OrdinalIgnoreCase) ||
            (exception is AggregateException ag && ag.InnerExceptions.All(e => e.Message.Contains("No such host is known", StringComparison.OrdinalIgnoreCase)));
    }
}
