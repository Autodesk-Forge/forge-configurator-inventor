/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Tabs');

Scenario('should check if All tabs are avaiable', async (I) => {

    // check if exists the Projects tab
    I.see("Projects", {xpath: '//ul/li[1]/div'});

    // check if exists the Model tab
    I.see("Model", {xpath: '//ul/li[2]/div'});

    // check if exists the BOM tab
    I.see("BOM", {xpath: '//ul/li[3]/div'});

    // check if exists the Drawing tab
    I.see("Drawing", {xpath: '//ul/li[4]/div'});

    // check if exists the Drawing tab
    I.see("Downloads", {xpath: '//ul/li[5]/div'});
});

Scenario('should check if all Tabs are loaded after click', async (I) => {

    // click on Model tab
    I.wait(3); // allow the projects to load
    I.click({xpath: "//ul/li[2]/div"});

    // check that Model tab has correct content
    I.waitForElement({xpath: '//*[@id="model"]/div/div[1]'}, 5);
    I.seeElement({xpath: '//*[@id="ForgeViewer"]'});

    // click on BOM tab
    I.click({xpath: "//ul/li[3]/div"});

    // check that BOM tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='bom']"});

    // click on Drawing tab
    I.click({xpath: "//ul/li[4]/div"});

    // check that Drawing tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='drawing']"});

    // click on Downloads tab
    I.click({xpath: "//ul/li[5]/div"});

    // check that Downloads tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='downloads']"});

    // click on Project tab
    I.click({xpath: "//ul/li[1]/div"});

    // check that Project tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='project-list']"});
});
