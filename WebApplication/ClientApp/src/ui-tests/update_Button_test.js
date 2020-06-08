/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

Feature('Update Button');

//ensure that Stripe panel is not diabled!!!
Scenario('should check if Update button displays a message', (I) => {

    // click on Model tab
    I.click( locators.modelTab);

    // Click on Update button
    I.see("Update", locators.xpButtonUpdate);
    I.click( locators.xpButtonUpdate);

    // TO DO
    // check an action after Click on Update button - not implemneted yet

});