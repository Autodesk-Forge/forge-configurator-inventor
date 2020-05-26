/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Viewer');

Scenario('should check switch to model tab loads the viewer', async (I) => {

    const viewerModelSelector = '#ViewerModelStructurePanel';

    I.see('Model', XPathElements.xpTabModel);
    I.click(XPathElements.xpTabModel);
    I.waitForElement(XPathElements.xpViewerCanvas, 5);
    I.waitForElement(viewerModelSelector, 5);
});
