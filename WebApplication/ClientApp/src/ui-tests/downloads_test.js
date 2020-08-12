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

Feature('Downloads');

Scenario('should check switch to downloads tab shows the downloads links', async (I) => {
    I.see('Downloads', locators.downloadsTab);
    I.click(locators.downloadsTab);
    I.waitForElement('.BaseTable');
    I.seeNumberOfElements('.BaseTable__row', 3);
    // all expected download types are available
    I.see('IAM', '.BaseTable__row-cell a');
    I.see('RFA', '.BaseTable__row-cell a');
    I.see('BOM', '.BaseTable__row-cell a');
    // check icons
    I.seeNumberOfElements({ css: '[src="products-and-services-24.svg"]'}, 3);
});
