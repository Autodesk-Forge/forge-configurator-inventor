/* eslint-disable jest/no-standalone-expect */
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
//const { debug } = require('console');

const bomTable = '//div[@class="BaseTable__body"]';
//const parametersElement = '.parameters';
const bomRows = '//div[@class="BaseTable__row"]';

const csvRow = {...
    [
        'IAM/IPT,Model',
        'RFA,Model'
    ]
};

Before((I) => {
    I.amOnPage('/');
});

Feature('Bom Data Validation');

// validate that Parameter notification is displayed
Scenario('should check BOM data after change', async (I) => {

    // select project in the Project Switcher
    I.selectProject('Conveyor');

    // click to BOM tab
    I.click(locators.downloadsTab); //temporary use downloads
    I.waitForVisible(bomTable, 5);

    const data = await I.grabTextFrom(bomRows);
    I.say('***********************************');
    I.say(csvRow[0]);
    I.say(csvRow[1]);

    for(let i=0; i< data.length; ++i)
    {
        const htmlRow = data[i].replace('\n', ',');
        assert.equal(csvRow[i], htmlRow, 'Error: BOM row is not identical with CSV!');

        // check number of ...
        if(i == 6)
        {
            const quantity = htmlRow.split(',')[2];
            assert.equal(quantity, '4', 'Error: Unexpected BOM quantity!');
        }
    }

    /*
    // change paramter
    I.waitForElement(parametersElement, 20);
    I.setParamValue("LEGS (MIN:12 | MAX:8 | STEP:2)", "6");

    // Click on Update button
    I.waitForVisible(locators.xpButtonUpdate, 10);
    I.click(locators.xpButtonUpdate);

    // wait for progress bar shows and disappears
    I.waitForVisible(updatingProjectProgress, 10);
    I.waitForInvisible(updatingProjectProgress, 120);
    */

   for(let i=0; i< data.length; ++i)
   {
       const htmlRow = data[i].replace('\n', ',');
       assert.equal(csvRow[i], htmlRow, 'Error: BOM row is not identical with CSV!');
       
       // check number of ...
       if(i == 6)
       {
           const quantity = htmlRow.split(',')[2];
           assert.equal(quantity, '6', 'Error: Unexpected BOM quantity!');
       }
   }

  });