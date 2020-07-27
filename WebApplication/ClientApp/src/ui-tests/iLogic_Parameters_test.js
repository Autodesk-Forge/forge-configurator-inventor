/* eslint-disable no-undef */

const assert = require('assert');

const parametersElement = '.parameters';
const elements = '//div[@class="parameter" or @class="parameter checkbox"]';
const iLogicParameterList = ['LENGTH', 'WIDTH', 'HEIGHT', 'CHUTE', 'LEGS (MIN:12 | MAX:8 | STEP:2)', 'ROLLERS (MIN:3 | MAX:11 | STEP:2)'];

// compare two Arrays and return true or false
function compareArrays(array1, array2)
{
<<<<<<< HEAD
  if (array1.length != array2.length)
  {
    return false;
  }

  // commpare if All iLogic parameters have the same order
  for (let index = 0; index < array1.length; ++index)
  {
    if(array1[index] !== array2[index])
      return false;
  }

  return true;
=======
    if (array1.length != array2.length)
    {
        return false;
    }

    // compare if All iLogic parameters are the same as Model Tab has
    for (let index = 0; index < array1.length; ++index)
    {
        if(array2.indexOf(array1[index], 0) === -1)
            return false;
    }

    return true;
>>>>>>> origin/er/INVGEN-37608-ilogic-form-ui-changes
}

Before((I) => {
    I.amOnPage('/');
});

Feature('iLogic Parameters');

// validate that all parameters in iLogic form is displayed in the List of Parameters
Scenario('should check parameters in iLogic Form with list of parameters in Model Tab', async (I) => {

    // select Conveyor project in the Project Switcher
    I.selectProject('Conveyor');
    I.waitForElement(parametersElement, 10);

    // get list of paramater from Model tab
    const modelTabParamList = await I.grabTextFrom(elements);

    // comapre lists and validate
    const result = compareArrays(iLogicParameterList, modelTabParamList);
    assert.equal(result, true);
});