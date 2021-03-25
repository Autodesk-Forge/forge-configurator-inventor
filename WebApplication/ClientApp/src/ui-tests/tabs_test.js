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

Feature('Tabs');

Before(({ I }) => {
    I.amOnPage('/');
});

Scenario('should check if All tabs are available', async ({ I }) => {

    // check if exists the Projects tab
    I.see("Projects", locators.projectsTab);

    // check if exists the Model tab
    I.see("Model", locators.modelTab);

    // check if exists the BOM tab
    I.see("BOM", locators.bomTab);

    // check if exists the Drawing tab
    I.see("Drawing", locators.drawingTab);

    // check if exists the Downloads tab
    I.see("Downloads", locators.downloadsTab);
});

Scenario('should check if all Tabs are loaded after click', async ({ I }) => {

    // select the Wheel project
    I.selectProject('Wheel');

    // click on Model tab
    I.clickToModelTab();

    // check that Model tab has correct content
    I.waitForVisible( locators.ParametersContainer, 240);
    I.seeElement( locators.ForgeViewer);

    // click on BOM tab
    I.click( locators.bomTab);

    // check that BOM tab has correct content
    I.waitForVisible( locators.BomContainer, 10);

    // click on Drawing tab
    I.click( locators.drawingTab);

    // check the dialog will appear with Ok button
    const drawingProgress = '//p[text()="Generating Drawing"]';
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    // check that Drawing tab has correct content
    I.waitForVisible( locators.DrawingContainer, 10);

    // click on Downloads tab
    I.goToDownloadsTab();

    // check that Downloads tab has correct content
    I.seeElement('#downloads .BaseTable');

    // click on Project tab
    I.click( locators.projectsTab);

    // check that Project tab has correct content
    I.seeElement('#project-list .BaseTable');
    I.waitForText('Wrench', 5); // wait for projects to be loaded, the second one should not be visible anywhere else
});
