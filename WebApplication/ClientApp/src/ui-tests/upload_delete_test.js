/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
   I.amOnPage('/');
});

Feature('Upload and delete');

Scenario('upload workflow', async (I) => {
   await I.signIn();

   I.uploadProject('src/ui-tests/dataset/SimpleBox.zip', 'SimpleBox.iam');
});

Scenario('delete workflow', async (I) => {
   await I.signIn();

   I.deleteProject('SimpleBox');
});
