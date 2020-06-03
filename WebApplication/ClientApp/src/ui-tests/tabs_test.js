/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Tabs');

Scenario('should check if All tabs are available', async (I) => {

    // check if exists the Projects tab
    I.see("Projects", {xpath: XPathElements.xpTabProjects});

    // check if exists the Model tab
    I.see("Model", {xpath: XPathElements.xpTabModel});

    // check if exists the BOM tab
    I.see("BOM", {xpath: XPathElements.xpTabBOM});

    // check if exists the Drawing tab
    I.see("Drawing", {xpath: XPathElements.xpTabDrawing});

    // check if exists the Downloads tab
    I.see("Downloads", {xpath: XPathElements.xpTabDownloads});
});

Scenario('should check if all Tabs are loaded after click', async (I) => {

    // click on Model tab
    I.click({xpath: XPathElements.xpTabModel});

    // check that Model tab has correct content
    I.waitForVisible({xpath: XPathElements.xpDivParameterContainer}, 30);
    I.seeElement({xpath: XPathElements.xpDivForgeViewer});

    // click on BOM tab
    I.click({xpath: XPathElements.xpTabBOM});

    // check that BOM tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='bom']"});

    // click on Drawing tab
    I.click({xpath: XPathElements.xpTabDrawing});

    // check that Drawing tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='drawing']"});

    // click on Downloads tab
    I.click({xpath: XPathElements.xpTabDownloads});

    // check that Downloads tab has correct content
    I.seeElement('#downloads .BaseTable');

    // click on Project tab
    I.click({xpath: XPathElements.xpTabProjects});

    // check that Project tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='project-list']"});
});
