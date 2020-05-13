Feature('tab');

Scenario('test something', (I) => {
    I.amOnPage('https://localhost:5001');
    I.seeElement({xpath: '//ul/li[2]/div'});
    I.click({xpath: '//ul/li[2]/div'});

    I.wait(5);

});
