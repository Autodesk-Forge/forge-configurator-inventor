/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Update Button');

//ensure that Stripe panel is not diabled!!!
Scenario('should check if Update button displays a message', (I) => {

    // click on Model tab
    I.click({xpath: XPathElements.xpTabModel});

    // Click on Update button
    I.see("Update", XPathElements.xpButtonUpdate);
    I.click( XPathElements.xpButtonUpdate);

    // TO DO
    // check an action after Click on Update button - not implemneted yet

});