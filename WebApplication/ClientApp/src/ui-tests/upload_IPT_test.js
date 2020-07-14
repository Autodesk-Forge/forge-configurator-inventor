/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
 });
 
 Feature('Upload and delete IPT File');
 
 Scenario('upload workflow', async (I) => {
    await I.signIn();
 
    I.uploadIPTFile('src\\ui-tests\\EndCap.ipt');
 });
 
 Scenario('delete workflow', async (I) => {
    await I.signIn();
 
    I.deleteProject('EndCap');
 });