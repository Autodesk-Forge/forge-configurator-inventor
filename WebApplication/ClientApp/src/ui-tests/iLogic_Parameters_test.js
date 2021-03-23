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

const parametersElement = '.parameters';

const elements = '//div[@class="parameter" or @class="parameter checkbox"]';
const iLogicParameterList = ['Wheel Size', 'Number Of Spokes', 'Slot', 'Wheel Finish', 'Brake Material', 'Caliper Finish', 'Total Price'];

const readOnlyElements = '//div[(@class="parameter" or @class="parameter checkbox") and .//input[@disabled]]';
const iLogicReadOnlyParameterList = ['ReadOnly', 'Diameter [mm]'];

// compare two Arrays and return true or false
function compareArrays(array1, array2)
{
  if (array1.length != array2.length)
  {
    debug("Error: different number of parameters!");
    debug(array1.length + " vs " + array2.length);
    return false;
  }

  // compare if All iLogic parameters have the same order
  for (let index = 0; index < array1.length; ++index)
  {
    if(array1[index] !== array2[index])
    {
      debug("Error: parameters are not the same!");
      return false;
    }
  }

  return true;
}

Feature('iLogic Parameters');

Before(({ I }) => {
    I.amOnPage('/');
});

// validate that all parameters in iLogic form are displayed in the List of Parameters
Scenario('should check parameters in iLogic Form with list of parameters in Model Tab', async ({ I }) => {

  // select Wheel project in the Project Switcher
  I.selectProject('Wheel');
  I.waitForElement(parametersElement, 20);
  I.wait(1); // Wait for element didnt work here after the update, codeceptjs was unable to grab the values since they didnt appear to be loaded for him yet

  // get list of parameter from Model tab
  const modelTabParamList = await I.grabTextFromAll(elements);

  // compare all parameters and validate
  const result = compareArrays(iLogicParameterList, modelTabParamList);
  assert.strictEqual(result, true, "There is an incorrect number of parameters or parameter names");
});

// validate that all Read only parameters in iLogic form are displayed in the List of Parameters
Scenario('should check parameters in iLogic Form with list of Read Only parameters in Model Tab', async ({ I }) => {

  await I.signIn();

  I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');

  // select EndCap project in the Project Switcher
  I.selectProject('EndCap');
  I.waitForElement(parametersElement, 20);
  I.wait(1); // Wait for element didnt work here after the update, codeceptjs was unable to grab the values since they didnt appear to be loaded for him yet

  // get list of read Only inputs from Model tab
  const modelTabReadOnlyParamList = await I.grabTextFromAll(readOnlyElements);

  // compare Read Only labels and validate
  const readOnlyResult = compareArrays(iLogicReadOnlyParameterList, modelTabReadOnlyParamList);
  assert.strictEqual(readOnlyResult, true);

});

Scenario('Delete the project', async ({ I }) => {

  await I.signIn();
  I.deleteProject('EndCap');
});