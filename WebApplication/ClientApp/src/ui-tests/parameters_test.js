/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Parameters panel');

Scenario('should check if Parameter panel has Cancel and Update button', async (I) => {

    // click on Model tab
    I.wait(3); // allow the projects combo  to be  loaded - https://jira.autodesk.com/browse/INVGEN-40998
    I.click({xpath: XPathElements.xpTabModel });

    // check that Model tab has correct content
    I.see("Cancel",{xpath: XPathElements.xpButtonCancel });
    I.see("Update",{xpath: XPathElements.xpButtonUpdate });
});

//ensure that Stripe panel is not diabled!!!
Scenario('should check if Stripe panel is displayed and hidden', async (I) => {

    // click on Model tab
    I.wait(3); // allow the projects combo  to be  loaded - https://jira.autodesk.com/browse/INVGEN-40998
    I.click({xpath: XPathElements.xpTabModel});

    // check that Model tab has a parameter
    I.waitForElement('//*[@id="model"]/div/div[1]/div[1]/div[1]/div/input', 2);

    // change the Lenght parameter
    I.clearField('//div[1]/div[1]/div/input');
    I.fillField('//div[1]/div[1]/div/input', "12500 mm");

    // check if the Stripe element is displayed
    I.seeElement(XPathElements.xpStripeElement);

    // change the Lenght parameter
    I.clearField('//div[1]/div[1]/div/input');
    I.fillField('//div[1]/div[1]/div/input', "12000 mm");

    // check if the Stripe element was hidden
    I.waitForInvisible(XPathElements.xpStripeElement);
});


