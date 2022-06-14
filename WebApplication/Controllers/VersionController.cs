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

using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace webapplication.Controllers;

[Route("[controller]")]
public class VersionController : ControllerBase
{
    private const string default_version = "1.0.0";

    [HttpGet]
    public string Get()
    {
        string version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        if(version == null || version == default_version)
        {
            try
            {
                // Open the text file using a stream reader.
                using (var sr = new StreamReader("version.txt"))
                {
                    version = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                //Just swallow the exception for now
            }
        }

        return version;
    }
}
