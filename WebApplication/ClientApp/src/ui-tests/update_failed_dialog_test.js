/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
const locators = require('./elements_definition.js');
const updatingDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Updating Project")]]';
const failedDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Update Failed")]]';

Before((I) => {
    I.amOnPage('/');
});

Feature('Failed Dialog');

//ensure that Failed Dialog is displayed when you entered an incorrect value!!!
Scenario('should check incorrect input to show failed dialog', (I) => {

    // select Wrench project in the Project Switcher
    I.selectProject('Wrench');

    // set incorrect value - 'xyz'
    I.setParamValue('JawOffset', 'xyz'  );

    // click on Update button
    I.see("Update", locators.xpButtonUpdate);
    I.click( locators.xpButtonUpdate);

    // waiting for Updating dialog
    I.waitForVisible(updatingDialogTitle, 10);
    I.waitForInvisible(updatingDialogTitle, 120);

    // check if Failed dialog is displayed
    I.waitForVisible(failedDialogTitle, 30);
});