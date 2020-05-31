/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Downloads');

Scenario('should check switch to downloads tab shows the downloads links', async (I) => {
    I.see('Downloads', XPathElements.xpTabDownloads);
    I.click(XPathElements.xpTabDownloads);
    I.waitForElement('.BaseTable');
    I.seeNumberOfElements('.BaseTable__row', 2);
    // all expected download types are available
    I.see('IAM', '.BaseTable__row-cell-text');
    I.see('RFA', '.BaseTable__row-cell-text');
    // check icons
    I.seeNumberOfElements({ css: '[src="products-and-services-24.svg"]'}, 2);
});
