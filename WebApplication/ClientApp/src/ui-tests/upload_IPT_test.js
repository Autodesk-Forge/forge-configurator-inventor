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

/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
 });

 Feature('Upload and delete IPT design');

 Scenario('upload IPT design workflow', async (I) => {
    await I.signIn();

    I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');
 });

 Scenario('delete IPT design workflow', async (I) => {
    await I.signIn();

    I.deleteProject('EndCap');
 });