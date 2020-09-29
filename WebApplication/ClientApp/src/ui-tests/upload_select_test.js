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
const locators = require('./elements_definition.js');

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

Scenario('upload assembly with non-supported addins', async (I) => {

    I.dontSeeElement(locators.getProject('NotSupportedAddins'));

    I.uploadProjectFailure(
        'src/ui-tests/dataset/NotSupportedAddins.zip',
        'notSupportedAddins.iam');

    // check the error box title
    I.see('Adoption failed', locators.xpErrorMessageTitle);

    // get error message
    const errorMessage = await I.grabTextFrom(locators.xpErrorMessage);

    // validate if all names of all unsupported plugins are there
    [
        /Detected unsupported plugins/,
        /Frame Generator/,
        /Tube & Pipe/,
        /Cable & Harness/,
        /Mold Design/,
        /Design Accelerator/,
    ].forEach((snippet) => assert.match(errorMessage, snippet));

    I.closeCompletionDialog();

    // ensure the project is deleted
    I.dontSeeElement(locators.getProject('NotSupportedAddins'));
});
