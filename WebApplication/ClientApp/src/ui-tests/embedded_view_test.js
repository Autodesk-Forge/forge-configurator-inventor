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

Feature('Embedded View');

// This test intentionally uses non existing json.
// It's purpose is just to verify correct parts of UI are hidden and correct parts are available.
// As a side effect, it tests also the adoption failure path.
Before(({ I }) => {
    I.amOnPage('/?url=foo.json');
});

Scenario('Toolbar elements are not present', async ({ I }) => {
    // wait until page loads
    //I.dismissContentLoadingFailDlg();

    I.dontSee("AUTODESK");
});

Scenario('should check if only required tabs are available', async ({ I }) => {
    // wait until page loads
    //I.dismissContentLoadingFailDlg();

    // check if exists the Model tab
    I.see("Model", locators.modelTab);

    // check if exists the BOM tab
    I.see("BOM", locators.bomTab);

    // check if exists the Drawing tab
    I.see("Drawing", locators.drawingTab);

    // check if exists the Downloads tab
    I.see("Downloads", locators.downloadsTab);

    // check if the Projects tab is hidden
    I.dontSee("Projects", locate('li').find('p'));
});

Scenario('should check that model tab doesnt have parameters pane', async ({ I }) => {
    // wait until page loads
    //I.dismissContentLoadingFailDlg();

    // click on Model tab
    I.clickToModelTab();

    // check that Model tab has correct content
    I.waitForVisible(locators.ForgeViewer, 240);
    I.dontSeeElement(locators.ParametersContainer);
});

Scenario('should check if other Tabs are loaded after click', async ({ I }) => {
    // wait until page loads
    //I.dismissContentLoadingFailDlg();

    // click on BOM tab
    I.click( locators.bomTab);
    I.see("BOM is Empty");

    // click on Drawing tab
    I.click( locators.drawingTab);
    I.see("don't have any drawing");

    // check that Drawing tab has correct content
    I.waitForVisible( locators.DrawingContainer, 10);

    // click on Downloads tab
    I.goToDownloadsTab();

    // check that Downloads tab has correct content
    I.seeElement('#downloads .BaseTable');
});

