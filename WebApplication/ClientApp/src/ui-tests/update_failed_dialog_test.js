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

/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
const locators = require('./elements_definition.js');
const updatingDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Updating Project")]]';
const failedDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Update Failed")]]';
const projectShelves = '//div[@role="row"]//div[text()="shelves"]';

Before((I) => {
    I.amOnPage('/');
});

Feature('Failed Dialog');

//ensure that Failed Dialog is displayed when iLogic Failed!!!
Scenario('should check incorrect input to show failed dialog', async (I) => {
    await I.signIn();

    // uploaded an assembly with iLogic error (missing parts for iLogic)
    I.uploadProject('src/ui-tests/dataset/shelves.zip', 'shelves.iam');

    // click on Shelves project
    I.waitForElement(projectShelves, 10);
    I.click(projectShelves);

    // set value for parameter
    I.setParamValue('iTrigger0', '5'  );

    // click on Update button
    I.click( locators.xpButtonUpdate);

    // waiting for Updating dialog
    I.waitForVisible(updatingDialogTitle, 10);
    I.waitForInvisible(updatingDialogTitle, locators.FDAActionTimeout);

    // check if Failed dialog is displayed
    I.waitForVisible(failedDialogTitle, 30);

});

Scenario('delete workflow', async (I) => {
   await I.signIn();

   I.deleteProject('shelves');
});

