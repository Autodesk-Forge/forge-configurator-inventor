/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

const projectButton =  '//div[contains(@role, "button") and .//*[local-name()="img"]]';
const currentProjectName =  locate('p').inside(projectButton);

Before((I) => {
    I.amOnPage('/');
});

Feature('Project Switcher');

Scenario('should check Project switcher is loaded', async (I) => {
    // wait until project combo is displayed
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
    I.waitForElement( locators.xpComboProjects, 10);
    I.click( locators.xpComboProjects);

    // wait until project list is displayed
    I.waitForElement( locators.xpProjectList, 10);

    // emulate click to trigger project loading
    I.click( locators.xpProjectConveyor);

    // check the current project name
    I.see("Conveyor", currentProjectName);

    // click to show popup menu with list of projects
    I.selectProject('Wrench');

    // check the current project name
    I.see("Wrench", currentProjectName);
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
    I.waitForElement('span[aria-label="Avatar for Anonymous"]', 2);

    // validate user name
    I.see("A", '//button/span/span');
});
