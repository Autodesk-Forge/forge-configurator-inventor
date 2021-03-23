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

const bomTable = '//div[@class="BaseTable__body"]';
const parametersElement = '.parameters';
const bomRows = '//div[@class="BaseTable__row"]';

const baseRows =  [
    ['1', 'Wrench_Bar_Complete', '1', 'WRENCH BAR', 'Copper'],
    ['2', 'DS51F1496-03', '1', 'FRAME', 'Gray Iron'],
    ['3', 'DS51F1496-04', '1', 'JAW', 'Steel'],
    ['5', 'DS51F1496-08', '1', 'BACK SPRING', 'Steel'],
    ['7', 'DS51F1496-05', '1', 'PIN - HARDENED GROUND PRODUCTION DOWEL', 'Steel, Mild'],
    ['8', 'DS51F1496-02', '1', 'NUT NO. 11220', 'Steel, High Strength Low Alloy'],
    ['11', 'DS51F1496-06', '1', 'FRONT SPRING', 'Steel'],
    ['12', 'DS51F1496-07', '2', 'CLEVIS PINS AND COTTER PINS - CLEVIS PIN', 'Steel, Mild']
];

const updatedRows =  [
    ['1', 'Wrench_Bar_Complete', '1', 'WRENCH BAR', 'Stainless Steel'],
    ['2', 'DS51F1496-03', '1', 'FRAME', 'Gray Iron'],
    ['3', 'DS51F1496-04', '1', 'JAW', 'Steel'],
    ['5', 'DS51F1496-08', '1', 'BACK SPRING', 'Steel'],
    ['7', 'DS51F1496-05', '1', 'PIN - HARDENED GROUND PRODUCTION DOWEL', 'Steel, Mild'],
    ['8', 'DS51F1496-02', '1', 'NUT NO. 11220', 'Steel, High Strength Low Alloy'],
    ['11', 'DS51F1496-06', '1', 'FRONT SPRING', 'Steel'],
    ['12', 'DS51F1496-07', '2', 'CLEVIS PINS AND COTTER PINS - CLEVIS PIN', 'Steel, Mild']
];

Feature('Bom Data Validation');

Before(({ I }) => {
    I.amOnPage('/');
});

// this test checks that BOM data are correct. There are two validations
// first validation is before update
// second validation is after a parameter change - PartMaterial parameter
Scenario('should check BOM data after change', async ({ I }) => {

    // select project in the Project Switcher
    I.selectProject('Wrench');

    // click to BOM tab
    I.click(locators.bomTab);
    I.waitForVisible(bomTable, 5);

    const htmlData = await I.grabTextFromAll(bomRows);

    assert.strictEqual(htmlData.length , baseRows.length, 'Error: Different number of BOM rows!');

    for(let i = 0; i < htmlData.length; ++i) // first validation
    {
        const baseRow = baseRows[i].join('\n');
        I.say(baseRow);
        const rowNumber = i + 1;
        assert.strictEqual(htmlData[i], baseRow, 'Error: BOM row ' + rowNumber + ' is not identical with base data!');
    }

    // change parameter - select an item from a listbox
    const partMaterialParameter = '//div[@class="parameter"][text()="Material"]//input';
    const listbox = '//div[@role="listbox" and .//div[contains(.,"Stainless Steel")]]';
    const optionStainlessSteel = '//div[@role="option" and .//span[contains(.,"Stainless Steel")]]';
    I.click(locators.modelTab);
    I.waitForElement(parametersElement, 20);
    I.waitForVisible(partMaterialParameter,10);
    I.clearField(partMaterialParameter);
    I.click(partMaterialParameter);
    I.waitForVisible(listbox, 3);
    I.click(optionStainlessSteel);

    I.updateProject();

    // click to BOM tab
    I.click(locators.bomTab);
    I.waitForVisible(bomTable, 5);

    const updatedHtmlData = await I.grabTextFromAll(bomRows);

    assert.strictEqual(updatedHtmlData.length , updatedRows.length, 'Error: Different number of BOM rows after update!');

    for(let i = 0; i < updatedHtmlData.length; ++i) // second validation
    {
        const updatedRow = updatedRows[i].join('\n');
        const rowNumber = i + 1;
        assert.strictEqual(updatedHtmlData[i], updatedRow, 'Error: BOM row ' + rowNumber + ' is not identical with base data after update!');
    }

  });