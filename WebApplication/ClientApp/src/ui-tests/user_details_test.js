/* eslint-disable no-undef */

Before((I) => {
    I.amOnPage('/');
});

Feature('User Details Control');

Scenario('should check if user details control has the expected items', async (I) => {
    I.see("USER", locate('div').find('span.user'));
    I.see("A", locate('div').find('span.avatar-custom-style'));
    I.see("Anonymous", locate('div').find('span.username'));
    I.see("Sign In", locate('div').find('span.auth-button-text'));
});