/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

Feature('Tabs');

Scenario('should check if All tabs are available', async (I) => {

    // check if exists the Projects tab
    I.see("Projects", locators.projectsTab);

    // check if exists the Model tab
    I.see("Model", locators.modelTab);

    // check if exists the BOM tab
    I.see("BOM", locators.xpTabBOM);

    // check if exists the Drawing tab
    I.see("Drawing", locators.drawingTab);

    // check if exists the Downloads tab
    I.see("Downloads", locators.downloadsTab);
});

Scenario('should check if all Tabs are loaded after click', async (I) => {

    // click on Model tab
    I.clickToModelTab();

    // check that Model tab has correct content
    I.waitForVisible( locators.ParametersContainer, 240);
    I.seeElement( locators.ForgeViewer);

    // click on BOM tab
    I.click( locators.bomTab);

    // check that BOM tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='bom']"});

    // click on Drawing tab
    I.click( locators.drawingTab);

    // check that Drawing tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='drawing']"});

    // click on Downloads tab
    I.click( locators.downloadsTab);

    // check that Downloads tab has correct content
    I.seeElement('#downloads .BaseTable');

    // click on Project tab
    I.click( locators.projectsTab);

    // check that Project tab has correct content
    I.seeElement('#project-list .BaseTable');
    I.waitForText('Wrench', 5); // wait for projects to be loaded, the second one should not be visible anywhere else
});
