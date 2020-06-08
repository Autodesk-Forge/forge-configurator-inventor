/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('/');
});

const progressDialog = locate('div').withAttr({ role: 'dialog' });
const divDownloads = locate('div').withAttr({ id: 'downloads' });
const titleDataFileForWrench = locate('p').withText('Wrench').inside(progressDialog);
const titleDataFileFroConveyor = locate('p').withText('Conveyor').inside(progressDialog);
const linkRFA = locate('a').withText('RFA');
//const clickHere = locate('section').find('a').withText('Click here');

Feature('Downloads');

Scenario('should check downloads tab with RFA link for Conveyor', async (I) => {

    //check Download Tab
    I.see('Downloads', locators.downloadsTab);

    // click on downlod tab
    I.click(locators.downloadsTab);

    //check if Div download exists
    I.seeElement(divDownloads);

    // check if RFA link exists
    I.see('RFA', linkRFA);

    // click on RFA link
    I.click(linkRFA);

    // check if Progress download window is displayed with correct data
    I.waitForElement(progressDialog, 30);
    I.seeElement(titleDataFileFroConveyor);

    // wait for a link to download a file
    //I.waitForElement(clickHere, 50);
});

Scenario('should check downloads tab with RFA link for Wrench', async (I) => {

    // wait until project combo is displayed
    I.selectProject('Wrench');

    //check Download Tab
    I.see('Downloads', locators.downloadsTab);

    // click on downlod tab
    I.click(locators.downloadsTab);

    //check if Div download exists
    I.seeElement(divDownloads);

    // check if RFA link exists
    I.see('RFA', linkRFA);

    // click on RFA link
    I.click(linkRFA);

    // check if Progress download window is displayed with correct data
    I.waitForElement(progressDialog, 30);
    I.seeElement(titleDataFileForWrench);

    // wait for a link to download a file
    //I.waitForElement(clickHere, 50);
});