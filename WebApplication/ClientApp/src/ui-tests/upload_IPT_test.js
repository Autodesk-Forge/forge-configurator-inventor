/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
 });

 Feature('Upload and delete IPT design');

 Scenario('upload IPT design workflow', async (I) => {
    await I.signIn();

    I.uploadIPTFile('src/ui-tests/dataset/EndCap.ipt');
 });

 Scenario('delete IPT design workflow', async (I) => {
    await I.signIn();

    I.deleteProject('EndCap');
 });