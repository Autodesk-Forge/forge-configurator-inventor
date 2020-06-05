/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Downloads');

Scenario('should check switch to downloads tab shows the downloads links', async (I) => {
    I.see('Downloads', locators.downloadsTab);
    I.click(locators.downloadsTab);
    I.waitForElement('.BaseTable');
    I.seeNumberOfElements('.BaseTable__row', 2);
    // all expected download types are available
    I.see('IAM', '.BaseTable__row-cell a');
    I.see('RFA', '.BaseTable__row-cell a');
    // check icons
    I.seeNumberOfElements({ css: '[src="products-and-services-24.svg"]'}, 2);
});
