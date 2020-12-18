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

Feature('Embedded Adoption');

// This test uses existing json pointing to existing dataset.
// It's purpose is just to verify the first step of adoption. it's fine if it uses cached data -
// - the embedded adoption processing is already tested on server side.
Before((I) => {
    I.amOnPage('/?url=https://inventorio-dev-holecep.s3-us-west-2.amazonaws.com/Interaction/wrench.json');
});

Scenario('Should check the adoption is started and finished', async (I) => {
    // check if exists the Model tab
    I.see("Model", locators.modelTab);

    // viewer loaded
    const viewerModelSelector = '#ViewerModelStructurePanel';
    I.waitForElement(locators.xpViewerCanvas, 300);
    I.waitForElement(viewerModelSelector, 300);
});
