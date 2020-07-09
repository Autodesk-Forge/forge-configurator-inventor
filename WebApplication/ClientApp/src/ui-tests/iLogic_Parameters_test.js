/* eslint-disable prefer-const */
/* eslint-disable jest/no-standalone-expect */
/* eslint-disable no-console */
/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */

const assert = require('assert');
const locators = require('./elements_definition.js');

const parametersElement = '.parameters';
const iLogicParameterList = new Array('WrenchSz', 'JawOffset', 'PartMaterial', 'iTrigger0');
//const iLogicParameterList = ['Length', 'Width', 'Legs', 'Height', 'Chute', 'Rollers'];

// compare two Arrays and return true or false
async function compareArrays(array1, array2)
{
  if (array1.length != array2.length)
  {
    return false;
  }

  // commpare if All iLogic params are the same as Model Tab params
  for (let index = 0; index < array1.length; ++index)
  {
    if(array2.indexOf(array1[index], 0) === -1)
      return false;
  }

  return true;
}

Before((I) => {
    I.amOnPage('/');
});

Feature('iLogic Parameters');

// validate that all Parameters in iLogic form is displayed in the List of Parameters
Scenario('should check parameters in iLogic Form with list of parameters in Model Tab', async (I) => {

    // select Conveyor project in the Project Switcher
    I.selectProject('Wrench');
    I.waitForElement(parametersElement, 10);

    // check iLogic Form Parameters
    const modelTabParamList = await I.executeScript(function(){
        let parameterList = new Array();
        const elements = document.getElementsByClassName('parameter');

        if (!elements)
            return parameterList;

        for (let index = 0; index < elements.length; ++index) {
          parameterList.push(elements[index].innerText);
          console.log('parameter ' + index + ' value : ' + elements[index].innerText);
        }

        return parameterList;
    });
    const result = await compareArrays(iLogicParameterList, modelTabParamList);
    assert.equal(result, true);
});