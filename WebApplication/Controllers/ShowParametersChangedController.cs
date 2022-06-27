/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Mvc;
using webapplication.State;
using webapplication.Utilities;

namespace webapplication.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowParametersChangedController : ControllerBase
{
    private readonly UserResolver _userResolver;

    public ShowParametersChangedController(UserResolver userResolver)
    {
        _userResolver = userResolver;
    }

    [HttpGet]
    public async Task<bool> Get()
    {
        bool result = true;

        ApiResponse<dynamic>? ossObjectResponse = null;

        try
        {
            var bucket = await _userResolver.GetBucketAsync();
            ossObjectResponse = await bucket.GetObjectAsync(ONC.ShowParametersChanged);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            // the file is not found. Just swallow the exception
        }

        if (ossObjectResponse != null)
        {
            using (Stream objectStream = ossObjectResponse.Data)
            {
                result = await JsonSerializer.DeserializeAsync<bool>(objectStream);
            }
        }

        return result;
    }

    [HttpPost]
    public async Task<bool> Set([FromBody] bool show)
    {
        var bucket = await _userResolver.GetBucketAsync();
        await bucket.UploadAsJsonAsync(ONC.ShowParametersChanged, show);
        return show;
    }
}
