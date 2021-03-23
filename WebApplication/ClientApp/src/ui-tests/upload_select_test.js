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

Before(async ({ I }) => {
    I.amOnPage('/');
    await I.signIn();
});

Scenario('upload workflow 2nd assembly', ({ I }) => {
    I.uploadProject('src/ui-tests/dataset/SimpleBox2asm.zip', 'Assembly2.iam');
});

Scenario('upload assembly with non-supported addins', async ({ I }) => {

    I.dontSeeElement(locators.getProjectByName('NotSupportedAddins'));

    I.uploadProjectWarning(
        'src/ui-tests/dataset/NotSupportedAddins.zip',
        'notSupportedAddins.iam');

    // get warning message
    I.waitForVisible(locators.xpWarningMessage, locators.FDAActionTimeout);
    const warningMessage = await I.grabTextFromAll(locators.xpWarningMessage);

    // validate if all names of all unsupported plugins are there
    [
        /Detected unsupported plugins/,
        /Frame Generator/,
        /Tube & Pipe/,
        /Cable & Harness/,
        /Mold Design/,
        /Design Accelerator/,
    ].forEach((snippet) => assert.match(warningMessage, snippet));

    // validate that the dialog has warning Icon
    I.seeElement('#warningIcon');

    I.closeCompletionDialog();

    // ensure the project is uploaded
    I.waitForVisible(locators.getProjectByName('NotSupportedAddins'), 30);

    // get Details text for uploaded project
    const projectRow = await I.grabTextFromAll(locators.getProjectRowByName('NotSupportedAddins'));

    const [, details1, details2, details3] = projectRow.split('\n');

    // validate the warning message
    assert.strictEqual(details1, 'Detected unsupported plugins: Mold Design, Tube & Pipe, Frame Generator, Design Accelerator, Cable & Harness.', 'Error: Details text with list of unsupported plugins is not as expected!');
    assert.strictEqual(details2, 'Unresolved file: \'ASME B16.11 90 Deg Elbow Threaded - Class 3000 1_2.ipt\'.', 'Error: Warning for missing file is not correct!');
    assert.strictEqual(details3, 'Change of parameters may lead to incorrect results.', 'Error: Warning for incorrect result on parameter change is not correct!');
});


Scenario('delete workflow', ({ I }) => {
    I.deleteProject('SimpleBox2asm');
    I.deleteProject('NotSupportedAddins');
});