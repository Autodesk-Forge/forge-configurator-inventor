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
const assert = require('assert');
const newParamValue = '24 mm';
const paramName = 'Jaw Offset';

Feature('Update params');

Before((I) => {
    I.amOnPage('/');
});

Scenario('Updating parameters for model', async (I) => {

    I.selectProject("Wrench");

    // enter new parameter value
    I.setParamValue(paramName, newParamValue);

    // check that stripe appeared
    I.waitForVisible(locators.xpStripeElement, 10);

    // Click on Update button
    I.updateProject();

    // check that stripe disappeared
    I.waitForInvisible(locators.xpStripeElement, 5);

    // check for updated parameter value
    const jawOffsetInput = '//div[text() = "'+ paramName +'"]//input';
    I.waitForVisible(jawOffsetInput, 20);
    const currentParamValue = await I.grabValueFrom(jawOffsetInput);
    assert.equal(newParamValue, currentParamValue, 'Error: Parameter "' + paramName + '" has incorrect value!');
});