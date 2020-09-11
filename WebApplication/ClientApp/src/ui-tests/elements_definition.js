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

//If the locator is an object, it should have a single element,
// with the key signifying the locator type (id, name, css, xpath, link, react, or class)
// and the value being the locator itself. This is called a "strict" locator.
// https://codecept.io/locators/#css-and-xpath

module.exports = {
     xpButtonReset : locate('button').find('span').withText('Reset'),
     xpButtonUpdate : locate('button').find('span').withText('Update'),
     xpButtonOk: locate('button').find('span').withText('Ok'),
     ForgeViewer : '#ForgeViewer',
     xpLinkAdskForge : '//a[@href="https://forge.autodesk.com"]',
     xpComboProjects : '//div[@role="button"] //*[local-name()="svg"]',
     xpProjectWrench : '//li[contains(@role,"menuitem") and .//span[contains(., "Wrench")]]',
     xpProjectWheel : '//li[contains(@role,"menuitem") and .//span[contains(., "Wheel")]]',
     xpProjectList : '//ul//span[text()="Projects"]',
     xpButtonLog : '//button[contains(@title, "Log")]',
     xpStripeElement : '//p[contains(text(),"The model is out-of-date.")]',
     PrametersList : '.parameters',
     ParametersContainer : '.parametersContainer',
     BomContainer : '.bomContainer',
     DrawingContainer : '.drawingContainer',
     xpPopUpLog : '//div[contains(h3, "Navigation Action")]',
     xpViewerCanvas : '//*[@id="ForgeViewer"] //canvas',
     projectsTab : locate('li').find('p').withText('Projects'),
     modelTab : locate('li').find('p').withText('Model'),
     bomTab : locate('li').find('p').withText('BOM'),
     drawingTab : locate('li').find('p').withText('Drawing'),
     downloadsTab : locate('li').find('p').withText('Downloads'),
     xpFirstInput : '//div[2]/div[1] //input',
     xpFirstInputOnModelTab : '//*[@id="model"]/div/div[1]/div[2]/div[1] //input',
     FDAActionTimeout: 600
};