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

Feature('Drawing Validation');

// this test checks that Drawing tab displays correct data
// if an assembly has a drawing then the drawing should be displayed
// If there is no drawing or only IPT - no content is displayed
Scenario('should check that Drawing tab shows drawing for an Assembly', async (I) => {

    // select project in the Project Switcher
    I.selectProject('Wheel');

    // click on drawing tab
    I.waitForVisible('//div[@id="ForgeViewer"] //div[@class="viewcube"]', 30);
    I.click(locators.drawingTab);

    // wait for drawing to be displayed
    I.waitForVisible('//div[@id="ForgePdfViewer"] //div[@class="viewcubeWrapper"]', 30);

});

Scenario('should check that Drawing tab shows drawing for an Assembly', async (I) => {

    // select project in the Project Switcher
    I.selectProject('Wrench');

    // click on drawing tab
    I.waitForVisible('//div[@id="ForgeViewer"] //div[@class="viewcube"]', 30);
    I.click(locators.drawingTab);

    // wait for no drawing page to be displayed
    I.waitForVisible('.drawingEmptyText', 20);
    I.see("You don't have any drawings in package.", '.drawingEmptyText');

});

Scenario('should check that IPT do not display any data', async (I) => {

    I.signIn();

    I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');

    // select project in the Project Switcher
    I.selectProject('EndCap');

    // click on drawing tab
    I.waitForVisible('//div[@id="ForgeViewer"] //div[@class="viewcube"]', 30);
    I.click(locators.drawingTab);

    // wait for no drawing page to be displayed
    I.waitForVisible('.drawingEmptyText', 20);
    I.see("You don't have any drawings in package.", '.drawingEmptyText');

});

Scenario('Delete the project', (I) => {

    I.signIn();
    I.deleteProject('EndCap');
});
