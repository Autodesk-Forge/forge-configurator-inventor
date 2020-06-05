/* eslint-disable no-undef */
const locators = require('./elements_definition.js');

Before((I) => {
    I.amOnPage('https://localhost:5001');
});

const progressDialog = locate('article').withAttr({ role: 'document' });
const divDownloads = locate('div').withAttr({ id: 'downloads' });
const dataFileWrench = locate('p').withText('Wrench');
const dataFileConveyor = locate('p').withText('Conveyor');
const linkRFA = locate('a').withText('RFA');
const clickHere = locate('section').find('a').withText('Click here');

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
    I.seeElement(dataFileConveyor);

    // wait for a link to download a file
    I.waitForElement(clickHere, 30);
});

Scenario('should check downloads tab with RFA link for Wrench', async (I) => {

    I.wait(2);

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
    I.seeElement(dataFileWrench);

    // wait for a link to download a file
    I.waitForElement(clickHere, 30);
});