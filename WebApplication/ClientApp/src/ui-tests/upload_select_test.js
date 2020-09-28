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

Feature('Select Upload Assembly');

Before(async (I) => {
   I.amOnPage('/');
   await I.signIn();
});

Scenario('upload workflow 2nd assembly', (I) => {

   I.uploadProject('src/ui-tests/dataset/SimpleBox2asm.zip', 'Assembly2.iam');
});

Scenario('delete workflow', (I) => {

   I.deleteProject('SimpleBox2asm');
});

const uploadFailedDialog = '//p[text()="Upload Failed"]';
const closeButton = '//button[@title="Close"]';

const locators = require('./elements_definition.js');
const assert = require('assert');

Scenario('upload assembly with non-supported addins', async (I) => {

   I.uploadProjectBase('src/ui-tests/dataset/NotSupportedAddins.zip', 'notSupportedAddins.iam');

   // Wait for Upload Failed dialog
   I.waitForVisible(uploadFailedDialog, locators.FDAActionTimeout);
   // const uploadFailLogLink = '//*[contains(@href,"report.txt")]';
   // I.seeElement(uploadFailLogLink);

   // const errorTitle = locate('p').withAttr({class: 'errorMessageTitle'});
   I.see('Adoption failed', '//div[@class="modalFailContent"]//p[contains(@class,"errorMessageTitle")]');
   const errorMessage = await I.grabTextFrom('//div[@class="modalFailContent"]//p[contains(@class,"errorMessage")][2]');
   assert.match(errorMessage, /Detected unsupported plugins/);

   I.click(closeButton);
});
