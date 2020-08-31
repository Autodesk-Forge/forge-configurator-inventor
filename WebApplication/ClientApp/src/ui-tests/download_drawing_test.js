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

/* eslint-disable no-undef */
const assert = require('assert');

Before((I) => {
    I.amOnPage('/');
});

Feature('Download drawing');

Scenario('should check to download drawing ZIP file when you click on the downloads links', async (I) => {

    I.selectProject('Wheel');
    I.goToDownloadsTab();

    // find Drawing download item
    const drawingLink = '//div[@role="gridcell"]//a[text()="Drawing"]';
    I.waitForElement(drawingLink, 10);
    I.click(drawingLink);

    // wait for 'click here' link in progress dialog
    const linkClickHere = '//article[@role="document"] //a[contains(.,"click here")]';
    const preparingDrawingsDialog = '//article[@role="document"] //p[text()="Preparing Drawings"]';
    I.waitForElement(preparingDrawingsDialog, 10);
    I.waitForElement(linkClickHere, 120);

    // validate the Link
    const link = await I.grabAttributeFrom(linkClickHere, 'href');
    assert.equal(true, link.includes('download/Wheel'));
    assert.equal(true, link.includes('drawing'));
    I.wait(2); // we seem to have a timing issue in test that end with physical file downloads
});