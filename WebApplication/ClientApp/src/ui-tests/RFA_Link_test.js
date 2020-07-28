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
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

const progressDialog = locate('div').withAttr({ role: 'dialog' });
const rowForRFA = locate('div').withAttr({role: 'gridcell'});
const divDownloads = locate('div').withAttr({ id: 'downloads' });
const titleDataFileForWrench = locate('p').withText('Wrench').inside(progressDialog);
const titleDataFileFroConveyor = locate('p').withText('Conveyor').inside(progressDialog);
const linkRFA = locate('a').withText('RFA').inside(rowForRFA);
//const clickHere = locate('section').find('a').withText('Click here');

Feature('Downloads RFA');

Scenario('should check downloads tab with RFA link for Conveyor', async (I) => {

    //check Download Tab
    I.see('Downloads', locators.downloadsTab);

    // click on downlod tab
    I.click(locators.downloadsTab);

    //check if Div download exists
    I.seeElement(divDownloads);

    // check if RFA link exists
    I.see('RFA', linkRFA);

    // click on RFA link
    I.click(linkRFA);

    // check if Progress download window is displayed with correct data
    I.waitForElement(progressDialog, 30);
    I.seeElement(titleDataFileFroConveyor);

    // wait for a link to download a file
    //I.waitForElement(clickHere, 50);
});

Scenario('should check downloads tab with RFA link for Wrench', async (I) => {

    // wait until project combo is displayed
    I.selectProject('Wrench');

    //check Download Tab
    I.see('Downloads', locators.downloadsTab);

    // click on downlod tab
    I.click(locators.downloadsTab);

    //check if Div download exists
    I.seeElement(divDownloads);

    // check if RFA link exists
    I.see('RFA', linkRFA);

    // click on RFA link
    I.click(linkRFA);

    // check if Progress download window is displayed with correct data
    I.waitForElement(progressDialog, 30);
    I.seeElement(titleDataFileForWrench);

    // wait for a link to download a file
    //I.waitForElement(clickHere, 50);
});