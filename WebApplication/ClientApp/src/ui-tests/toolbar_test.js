/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

Feature('Forge Link');

Scenario('should check if Autodesk Forge link works', async (I) => {

    // check Forge link
    I.waitForElement( locators.xpLinkAdskForge, 10);
    I.click( locators.xpLinkAdskForge);

    // wait for Autodesk Forge page
    I.waitForElement(".adskf__navbar-logo", 10);

    // check the page name
    I.seeTitleEquals("Autodesk Forge");
});

Feature('Project Switcher');

Scenario('should check Project switcher is loaded', async (I) => {

    // wait until project combo is displayed
    I.wait(3);
    I.waitForElement( locators.xpComboProjects, 10);
    I.click( locators.xpComboProjects);

    // wait until project list is displayed2
    I.waitForElement( locators.xpProjectList, 10);

    // check content of PROJECTS menu
    I.see("PROJECTS", locators.xpProjectList);

    // check name of the first project
    I.see("Conveyor", locators.xpProjectConveyor);

    // check name of the second project
    I.see("Wrench", locators.xpProjectWrench);
});

Scenario('should check Project switcher is correctly changed', async (I) => {

    // wait until project combo is displayed
    I.wait(3);
    I.waitForElement( locators.xpComboProjects, 10);
    I.click( locators.xpComboProjects);

    // wait until project list is displayed
    I.waitForElement( locators.xpProjectList, 10);

    // emulate click to trigger project loading
    I.click( locators.xpProjectConveyor);

    // check the current project name
    I.see("Conveyor", locators.xpComboProjects);

    // click to show popup menu with list of projects
    I.click( locators.xpComboProjects);

    // wait until project list is displayed
    I.waitForElement( locators.xpProjectList, 10);

    // emulate click to trigger project loading
    I.click( locators.xpProjectWrench);

    // check the current project name
    I.see("Wrench", locators.xpComboProjects);
});

Feature('Log button');

Scenario('should check presence of Log button', async (I) => {

    // check if exists the button
    I.waitForElement( locators.xpButtonLog, 2);
    I.click( locators.xpButtonLog);

    // wait until log popup is displayed
    I.waitForElement( locators.xpPopUpLog, 2);

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

