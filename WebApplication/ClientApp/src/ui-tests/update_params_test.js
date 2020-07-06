/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

Feature('Update params');

const updatingProjectProgress = '//p[text()="Updating Project"]';

Scenario('Updating parameters for conveyor model', (I) => {

    // click on Model tab
    I.clickToModelTab();

    // enter new parameter value
    I.setParamValue('Legs', '6 ul');

    // check that stripe appeared
    I.waitForVisible(locators.xpStripeElement);

    // Click on Update button
    I.waitForVisible(locators.xpButtonUpdate);
    I.click(locators.xpButtonUpdate);

    // wait for progress bar shows and disappearse
    I.waitForVisible(updatingProjectProgress, 10);
    I.waitForInvisible(updatingProjectProgress, 120);

    // check that stripe disappered
    I.waitForInvisible(locators.xpStripeElement, 5);
});