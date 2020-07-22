/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
 });

 Feature('Failed Upload Dialog');

 Scenario('upload IPT and verify that exists report.txt url', async (I) => {
   await I.signIn();

   I.uploadInvalidIPTFile('src/ui-tests/dataset/invalid.ipt');
 });