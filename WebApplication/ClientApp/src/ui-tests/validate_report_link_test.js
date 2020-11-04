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

const uploadFinishedDialog = '//p[text()="Upload Finished"]';
const updateFinishedDialog = '//p[text()="Update Finished"]';
const generatingDrawingDialog = '//p[text()="Generating Drawing"]';
const preparingDrawingsDialog = '//p[text()="Preparing Drawings"]';
const openButton = '//article[@role="document"]//button[contains(.,"Open")]';
const OK_Button = '//div[@id="modalDone"]//button';
const OK_Button_Downloads = '//div[@class="modalLink"]//button';
const drawingLink = '//div[@role="gridcell"]//a[text()="Drawing"]';

Feature('Validate report link');

Before(async (I) => {
    I.amOnPage('/');
    await I.signIn();
});

Scenario('should check dialogs where is Report link located', (I) => {
    I.uploadProjectBase('src/ui-tests/dataset/SimpleBox2asm.zip', 'Assembly1.iam');

    // Wait for Upload Finished dialog
    I.waitForVisible(uploadFinishedDialog, locators.FDAActionTimeout);

    // validate report link
    I.checkReportLink();

    I.click(openButton);

    I.setParamValue('Width', '1.5 in');

    I.click(locators.xpButtonUpdate);

    // Wait for Update Finished dialog
    I.waitForVisible(updateFinishedDialog, locators.FDAActionTimeout);

    // validate report link
    I.checkReportLink();

    I.click(OK_Button);

    I.goToDrawingTab();

    // Wait for Generating Drawing dialog
    I.waitForVisible(generatingDrawingDialog, 10);
    I.waitForVisible(OK_Button, locators.FDAActionTimeout);

    // validate report link
    I.checkReportLink();

    I.click(OK_Button);

    I.goToDownloadsTab();

    I.waitForVisible(drawingLink, 10);

    I.click(drawingLink);

    // Wait for Preparing Drawings dialog
    I.waitForVisible(preparingDrawingsDialog, 10);
    I.waitForVisible(OK_Button_Downloads, locators.FDAActionTimeout);

    // validate report link
    I.checkReportLink();

    I.click(OK_Button_Downloads);
    I.waitForInvisible(OK_Button_Downloads, 10);

    I.goToProjectsTab();
    I.deleteProject('SimpleBox2asm');

});
