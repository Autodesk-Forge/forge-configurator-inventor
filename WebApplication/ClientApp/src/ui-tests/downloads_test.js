/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Downloads');

Scenario('should check switch to downloads tab shows the downloads links', async (I) => {

    const viewerModelSelector = '#ViewerModelStructurePanel';

    I.see('Downloads', XPathElements.xpTabDownloads);
    I.click(XPathElements.xpTabDownloads);
    I.waitForElement('.BaseTable');
    I.seeNumberOfElements('.BaseTable__row', 2);
});
