/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

Feature('Viewer');

Scenario('should check switch to model tab loads the viewer', async (I) => {

    const viewerModelSelector = '#ViewerModelStructurePanel';

    I.see('Model', locators.modelTab);
    I.click(locators.modelTab);
    I.waitForElement(locators.xpViewerCanvas, 10);
    I.waitForElement(viewerModelSelector, 10);
});
