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
const parametersElement = '.parameters';
const updatedElements = '//div[(@class = "parameter" or @class = "parameter checkbox")] //input[contains(@class , "changedOnUpdate")]';
const tooltipTestNotify = '//div[contains(@class,"paramTooltip__flyout-container")][ancestor::div[contains(@class , "parameter") and contains(text(),"TestNotify")]]';
const parameterTestNotify = '//div[contains(@class , "parameter") and contains(text(),"TestNotify")]';

Feature('Parameter Notification');

Before(async ({ I }) => {
    I.amOnPage('/');
    await I.signIn();
});

// validate that Parameter notification is displayed
Scenario('should check parameter notification', async ({ I }) => {

    I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');

    // select EndCap project in the Project Switcher
    I.selectProject('EndCap');
    I.waitForElement(parametersElement, 20);

    // change paramter
    I.setParamValue("NumberOfBolts", "5");

    I.updateProject();

    // check if there is correct number of changeOnUpdate inputs
    I.seeNumberOfElements(updatedElements, 1);

    // check if tooltip is displayed
    I.moveCursorTo(parameterTestNotify);
    I.waitForVisible(tooltipTestNotify, 5);

    I.seeTextEquals("Parameter has changed\nInventor Server updated the parameter. Your initial input was overridden.", tooltipTestNotify);
  });

  Scenario('Delete the project', ({ I }) => {

    I.deleteProject('EndCap');
  });