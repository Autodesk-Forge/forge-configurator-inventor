/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Tabs');

Scenario('should check All tabs is avaiable and clickable', async (I) => {

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

Scenario('should check Projects tab is loaded', async (I) => {

    // click on Project tab
    I.click({xpath: "//ul/li[1]/div"});

    // check that Project tab has correct content
    I.see("The page is not yet implemented\nPlease switch to the Model tab", {xpath: "//*[@id='project-list']"});
});
