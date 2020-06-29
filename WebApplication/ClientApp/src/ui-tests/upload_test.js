/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
   I.amOnPage('/');
});

Feature('Upload');

Scenario('upload workflow', async (I) => {
   await I.signIn();

   await I.uploadProject('src\\ui-tests\\SimpleBox.zip', 'SimpleBox.iam');
});
