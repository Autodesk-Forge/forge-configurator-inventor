/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Viewer');

Scenario('should check switch to model tab loads the viewer', async (I) => {

    const modelTabSelector = '.tabsContainer li:nth-of-type(2) p';
    const viewerSelector = '.canvas-wrap canvas';
    const viewerModelSelector = '#ViewerModelStructurePanel';

    I.see('Model', modelTabSelector);
    I.wait(3); // allow the projects combo to be loaded
    I.click(modelTabSelector);
    I.waitForElement(viewerSelector, 20);
    I.waitForElement(viewerModelSelector, 20);
});