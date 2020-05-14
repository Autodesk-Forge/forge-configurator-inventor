/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Forge Link');

Scenario('should check if Autodesk Forge link works', async (I) => {

    // check Forge link
    I.waitForElement({xpath: "//div[1]/div[1]/a"},5);
    I.clickLink({xpath: "//*[@id='root']/div/div[1]/div[1]/a"});

    // wait for Autodesk Forge page
    I.waitForElement(".adskf__navbar-logo", 5);

    // check the page name
    I.seeTitleEquals("Autodesk Forge");
});

Feature('Project Switcher');

Scenario('should check Project switcher is loaded', async (I) => {

    // wait until project combo is displayed
    I.waitForElement({xpath: "//p"}, 3);
    I.click({xpath: "//p"});

    // wait until project list is displayed
    I.waitForElement({xpath: "//div/div[2]"}, 2);

    // check content of PROJECTS menu
    I.see("PROJECTS", {xpath: "//ul/span"});

    // check name of the first project
    I.see("Conveyor", {xpath: "//ul/li[1]/span[2]"});

    // check name of the second project
    I.see("Wrench", {xpath: "//ul/li[2]/span[2]"});
});

Scenario('should check Project switcher is loaded', async (I) => {

    // wait until project combo is displayed
    I.waitForElement({xpath: "//p"}, 3);
    I.click({xpath: "//p"});

    // wait until project list is displayed
    I.waitForElement({xpath: "//div/div[2]"}, 2);

    // emulate click to trigger project loading
    I.click({xpath: '//ul/li[1]/span[2]'});

    // check the current project name
    I.see("Conveyor", {xpath: '//p'});

    // click to show popup menu with list of projects
    I.click({xpath: "//p"});

    // wait until project list is displayed
    I.waitForElement({xpath: "//div/div[2]"}, 2);

    // emulate click to trigger project loading
    I.click({xpath: '//ul/li[2]/span[2]'});

    // check the current project name
    I.see("Wrench", {xpath: '//p'});
});

Feature('Log button');

Scenario('should check presence of Log button', async (I) => {

    // check if exists the button
    I.waitForElement({xpath: '//div/div[1]/button'}, 2);
    I.click({xpath: '//div/div[1]/button'});

    // wait until log popup is displayed
    I.waitForElement({xpath: '//div/div[2]'}, 2);

    // check content of the log popup
    I.see("Navigation Action", '//h3');
});

Feature('User button');

Scenario('should check presence of User button', async (I) => {

    //I.amOnPage(testPage);

    // check if exists the button
    I.waitForElement('span[aria-label="Avatar for anonymous user"]', 2);

    // validate user name
    I.see("AU", '//button/span/span');
});

