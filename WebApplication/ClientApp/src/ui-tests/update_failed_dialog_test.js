/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
const locators = require('./elements_definition.js');
const jawOffsetInput = '//div//div[text()="JawOffset"]//input';
const updatingDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Updating Project")]]';
const failedDialogTitle = '//div[@role="dialog" and .//p[contains(.,"Update Failed")]]';

Before((I) => {
    I.amOnPage('/');
});

Feature('Failed Dialog');

//ensure that Stripe panel is not diabled!!!
Scenario('should check incorrect input to show failed dialog', (I) => {

    // click to show popup menu with list of projects
    I.selectProject('Wrench');

    // wait for Input element
    I.waitForVisible(jawOffsetInput, 10);

    // set incorrect value - 'xyz'
    I.clearField(jawOffsetInput);
    I.fillField(jawOffsetInput, 'xyz');

    // click on Update button
    I.see("Update", locators.xpButtonUpdate);
    I.click( locators.xpButtonUpdate);

    I.waitForVisible(updatingDialogTitle, 10);
    I.waitForInvisible(updatingDialogTitle, 30);

    // check if Failed dialog is displayed
    I.waitForVisible(failedDialogTitle, 15);

});