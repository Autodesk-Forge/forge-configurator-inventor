/* eslint-disable no-console */
/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
});

Feature('Authentication');

Scenario('check Sign-in and Sign-out workflow', async (I) => {
    await I.signIn();

    I.signOut();
});
