/* eslint-disable no-undef */
const XPathElements = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

Feature('Forge Link');

Scenario('should check if Autodesk Forge link works', async (I) => {

    // check Forge link
    I.waitForElement({xpath: XPathElements.xpLinkAdskForge},5);
    I.click({xpath: XPathElements.xpLinkAdskForge});

    // wait for Autodesk Forge page
    I.waitForElement(".adskf__navbar-logo", 5);

    // check the page name
    I.seeTitleEquals("Autodesk Forge");
});

Feature('Project Switcher');

Scenario('should check Project switcher is loaded', async (I) => {

    // wait until project combo is displayed
    I.waitForElement({xpath: XPathElements.xpComboProjects}, 3);
    I.click({xpath: XPathElements.xpComboProjects});

    // wait until project list is displayed2
    I.waitForElement({xpath: XPathElements.xpProjectList}, 2);

    // check content of PROJECTS menu
    I.see("PROJECTS", {xpath: XPathElements.xpMenuProjectsName});

    // check name of the first project
    I.see("Conveyor", {xpath: XPathElements.xpProjectCoveyor});

    // check name of the second project
    I.see("Wrench", {xpath: XPathElements.xpProjectWrench});
});

Scenario('should check Project switcher is correctly changed', async (I) => {

    // wait until project combo is displayed
    I.waitForElement({xpath: XPathElements.xpComboProjects}, 3);
    I.click({xpath: XPathElements.xpComboProjects});

    // wait until project list is displayed
    I.waitForElement({xpath: XPathElements.xpProjectList}, 2);

    // emulate click to trigger project loading
    I.click({xpath: XPathElements.xpProjectCoveyor});

    // check the current project name
    I.see("Conveyor", {xpath: XPathElements.xpComboProjects});

    // click to show popup menu with list of projects
    I.click({xpath: XPathElements.xpComboProjects});

    // wait until project list is displayed
    I.waitForElement({xpath: XPathElements.xpProjectList}, 2);

    // emulate click to trigger project loading
    I.click({xpath: XPathElements.xpProjectWrench});

    // check the current project name
    I.see("Wrench", {xpath: XPathElements.xpComboProjects});
});

Feature('Log button');

Scenario('should check presence of Log button', async (I) => {

    // check if exists the button
    I.waitForElement({xpath: XPathElements.xpButtonLog}, 2);
    I.click({xpath: XPathElements.xpButtonLog});

    // wait until log popup is displayed
    I.waitForElement({xpath: XPathElements.xpPopUpLog}, 2);

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

