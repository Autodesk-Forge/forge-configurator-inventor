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

Feature('Update params');

const updatingProjectProgress = '//p[text()="Updating Project"]';

Scenario('Updating parameters for model', (I) => {

    I.selectProject("Wrench");

    // enter new parameter value
    I.setParamValue('JawOffset', '20 mm');

    // check that stripe appeared
    I.waitForVisible(locators.xpStripeElement);

    // Click on Update button
    I.waitForVisible(locators.xpButtonUpdate);
    I.click(locators.xpButtonUpdate);

    // wait for progress bar shows and disappeared
    I.waitForVisible(updatingProjectProgress, 10);
    I.waitForInvisible(updatingProjectProgress, 120);

    // check that stripe disappeared
    I.waitForInvisible(locators.xpStripeElement, 5);

    // TODO: check for updated parameters values
});