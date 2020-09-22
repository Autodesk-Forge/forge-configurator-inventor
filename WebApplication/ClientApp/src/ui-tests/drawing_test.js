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
const { debug } = require('console');

/* eslint-disable no-undef */
const noDrawingElement = '.drawingEmptyText';
const locators = require('./elements_definition.js');
const viewCubeElement = '//div[@id="ForgePdfViewer"] //div[@class="viewcubeWrapper"]';

// compare two Arrays and return true or false
function compareDrawings(array1, array2)
{
  if (array1.length != array2.length)
  {
    debug("Error: different number of drawings!");
    return false;
  }

  // compare if drawings are in the same order
  for (let index = 0; index < array1.length; ++index)
  {
    if(array1[index] !== array2[index])
    {
      debug("Error: drawings are not in the same order!");
      return false;
    }
  }

  return true;
}

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
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // check the dialog will appear with Ok button
    const drawingProgress = '//p[text()="Generating Drawing"]';
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    // wait for drawing to be displayed
    I.waitForVisible(viewCubeElement, locators.FDAActionTimeout);

});

Scenario('should check if an Assembly do not have any drawings then No data page is displayed', async (I) => {

    // select project in the Project Switcher
    I.selectProject('Wrench');

    // click on drawing tab
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // wait for no drawing page to be displayed
    I.waitForVisible(noDrawingElement, locators.FDAActionTimeout);
    I.see("You don't have any drawings in package.", noDrawingElement);

});

Scenario('should check id a drawing has more sheet is will show arrow buttons', async (I) => {

    I.signIn();

    I.uploadProject('src/ui-tests/dataset/SimpleBox2asm.zip', 'Assembly1.iam');

    // select project in the Project Switcher
    I.selectProject('SimpleBox2asm');

    // click on drawing tab
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // check the dialog will appear with Ok button
    const drawingProgress = '//p[text()="Generating Drawing"]';
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    // wait for drawing page to be displayed with extra arrow buttons
    const customDrwToolbar = '//div[@id="custom-drawing-toolbar"]';
    const prevButtonEnabled = '//div[@id="drawing-button-prev" and not(contains(@class,"disabled"))]';
    const nextButtonEnabled = '//div[@id="drawing-button-next" and not(contains(@class,"disabled"))]';
    const prevButtonDisabled = '//div[@id="drawing-button-prev" and contains(@class,"disabled")]';
    const nextButtonDisabled = '//div[@id="drawing-button-next" and contains(@class,"disabled")]';

    I.waitForVisible(customDrwToolbar, locators.FDAActionTimeout);
    I.waitForVisible(prevButtonDisabled, locators.FDAActionTimeout);
    I.waitForVisible(nextButtonEnabled, locators.FDAActionTimeout);

    // show next sheet
    I.click(nextButtonEnabled);

    // check button states
    I.seeElement(prevButtonEnabled);
    I.seeElement(nextButtonEnabled);

    // show next sheet
    I.click(nextButtonEnabled);

    // check button states
    I.seeElement(prevButtonEnabled);
    I.seeElement(nextButtonDisabled);

});

Scenario('should check that IPT do not display any data', async (I) => {

    I.signIn();

    I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');

    // select project in the Project Switcher
    I.selectProject('EndCap');

    // click on drawing tab
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // wait for no drawing page to be displayed
    I.waitForVisible(noDrawingElement, locators.FDAActionTimeout);
    I.see("You don't have any drawings in package.", noDrawingElement);

});

Scenario('should check if Wheel has more drawings listed in drawing panel and with correct order', async (I) => {

    // select project in the Project Switcher
    I.selectProject('Wheel');

    // click on drawing tab
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // check the dialog will appear with Ok button
    const drawingProgress = '//p[text()="Generating Drawing"]';
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    const drawingListTableHeader = '//div[@class="BaseTable__header-cell-text" and contains(text(),"Drawings")]';
    I.waitForVisible(drawingListTableHeader, 10);

    // get all drawing names and check if they are correctly sorted
    const tableRows = '//div[@class="BaseTable__row" or @class="BaseTable__row drawing-selected"]';
    const currentDrawings = await I.grabTextFrom(tableRows);

    const correctOrder = [
        "WheelAssembly.idw",
        "M-BR-0003-A 5 Stud Disc bell.idw",
        "M-FS-0002-A Upright.idw"];

    //debug("currentDrawings: " + currentDrawings);
    //debug("correctOrder: " + correctOrder);

    const res = compareDrawings(correctOrder, currentDrawings);
    assert(res, true);

    const drawing2 = '//div[@class="BaseTable__row-cell" and contains(text(),"M-BR-0003-A 5 Stud Disc bell.idw")]';
    I.click(drawing2);
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    const drawing3 = '//div[@class="BaseTable__row-cell" and contains(text(),"M-FS-0002-A Upright.idw")]';
    I.click(drawing3);
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

});

Scenario('should check drawing PDF download when "Export PDF" button click', async (I) => {

    I.selectProject('Wheel');

    // click on drawing tab
    I.waitForForgeViewerToPreventItFromCrashing(30);
    I.goToDrawingTab();

    // check the dialog will appear with Ok button
    const drawingProgress = '//p[text()="Generating Drawing"]';
    I.waitForVisible(drawingProgress, 10);
    I.waitForVisible(locators.xpButtonOk, locators.FDAActionTimeout);
    I.click(locators.xpButtonOk);

    // click on 'Export PDF' button
    I.waitForVisible(locators.xpButtonExportPDF, 10);
    I.click(locators.xpButtonExportPDF);

    // wait for 'click here' link in progress dialog
    const linkClickHere = '//article[@role="document"] //a[contains(.,"click here")]';
    const preparingDrawingsDialog = '//article[@role="document"] //p[text()="Preparing Drawing PDF"]';
    I.waitForElement(preparingDrawingsDialog, 10);
    I.waitForElement(linkClickHere, locators.FDAActionTimeout);

    // validate the Link
    const link = await I.grabAttributeFrom(linkClickHere, 'href');
    assert.strictEqual(true, link.includes('download/Wheel'));
    debug('link: ' + link);
    assert.match(link, /\/drawing\.pdf$/);
    I.wait(2); // we seem to have a timing issue in test that end with physical file downloads
});

Scenario('Delete the project', (I) => {

    I.signIn();
    I.deleteProject('EndCap');
    I.deleteProject('SimpleBox2asm');
});
