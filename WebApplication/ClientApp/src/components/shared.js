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

// use fixed version of the viewer to avoid usage of untested viewer version
const viewerVersion = '7.29.0';

// in case you need to debug Viewer script/css - remove '.min' from the URLs
export const viewerCss = `https://developer.api.autodesk.com/modelderivative/v2/viewers/${viewerVersion}/style.min.css`;
export const viewerJs = `https://developer.api.autodesk.com/modelderivative/v2/viewers/${viewerVersion}/viewer3D.min.js`;
