const playwright = require('playwright');

const pageUrl = "https://localhost:5001"

const config = {
    headless: false, // NOTE: uncomment to see what is actually going o the screen
    devtools: true,
    args: [
        // to avoid errors about self-signed SSL certificate: https://stackoverflow.com/q/55207690
        '--ignore-certificate-errors',
        '--ignore-certificate-errors-spki-list',
    ]};


describe('Integration UI tests', () => {

    // give browser more time to start (https://stackoverflow.com/a/49864436)
    jest.setTimeout(30000);

    let browser = null;
    let page = null;

    beforeEach(async () => {

        // TODO: try different browser kinds
        browser = await playwright["chromium"].launch(config);
        page = await browser.newPage();

        if (!page) {
            throw new Error("Connection wasn't established");
        }

        await page.route('**', route => {
            console.log(route.url());
            route.continue();
        });

        // Open the page
        await page.goto(pageUrl, { waitUntil: "networkidle0" });
    });

    afterEach(async () => {
        await browser.close();
    });

    it('should check the page is loaded', async () => {

        // check the page is loaded
        expect(page).not.toBeNull();
        expect(await page.title()).not.toBeNull();
    });

    it(`should project loading`, async () => {

        // wait until project list is refreshed
        await page.waitForSelector("#project-list ul");

        // check updated list content
        // it's assumed that the bucket is not empty
        const updatedContent = await page.evaluate(() => document.querySelector("#project-list").textContent);
        expect(updatedContent).not.toBe("No projects loaded");
    });

    it('PROJECTS is loaded', async () => {

        // check initial project list is filled
        const initialContent = await page.evaluate(() => document.evaluate('//p', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        //expect(initialContent).toBe("Local Project 1");
        expect(initialContent).toBe("Project1.zip");
    });

    it('Check Autodesk Forge link', async () => {

        // check Forge link            
        const link = await page.waitFor('a[aria-label="Autodesk HIG"]', {timeout: 3000});
        await link.click();
        const element = await page.waitForSelector('.adskf__navbar-logo', {visible: true, timeout: 5000})
        expect(element).not.toBeNull();
        expect(page).not.toBeNull();
        expect(await page.title()).toBe("Autodesk Forge");
    });

    it('PROJECTS menu is loaded', async () => {

        // wait until project combo is displayed        
        const project = await page.waitFor('//p',{timeout: 3000});
        await project.click();

        // wait until project list is displayed
        //*[@id="root"]/div/div[1]/div[3]/div/div[2]
        await page.waitForSelector('//div/div[2]', {visible: true, timeout: 2000});

        // check content of PROJECTS menu
        //*[@id="root"]/div/div[1]/div[3]/div/div[2]/div[2]/div/div/div/ul/span
        const caption = await page.evaluate(() => document.evaluate('//ul/span', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(caption).toBe("Projects");

        // check name of the first project
        //*[@id="root"]/div/div[1]/div[3]/div/div[2]/div[2]/div/div/div/ul/li[1]/span[2]
        const firstDemoProject = await page.evaluate(() => document.evaluate('//ul/li[1]/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(firstDemoProject).toBe("Project1.zip");

        // check name of the second project
        //*[@id="root"]/div/div[1]/div[3]/div/div[2]/div[2]/div/div/div/ul/li[2]/span[2]
        const secondDemoProject = await page.evaluate(() => document.evaluate('//ul/li[2]/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(secondDemoProject).toBe("Project2.zip");
    });

    it('check if the right PROJECT is set', async () => {

        // wait until project combo is displayed
        const project = await page.waitFor('//p',{timeout: 3000});
        await project.click();
        
        // wait until project list is displayed
        await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

        // emulate click to trigger project loading
        await page.evaluate(() => document.evaluate('//ul/li[1]/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click());
        let project_name = await page.evaluate(() => document.evaluate('//p', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(project_name).toBe("Project1.zip");

        // click to show popup menu with list of projects
        await page.evaluate(() => document.evaluate('//p', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click());
        
        // wait until project list is displayed
        await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

        // emulate click to trigger project loading
        await page.evaluate(() => document.evaluate('//ul/li[2]/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click());
        project_name = await page.evaluate(() => document.evaluate('//p', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(project_name).toBe("Project2.zip");        
     });

     it('Check Log Button', async () => {

        // check if exists the button
        const button = await page.waitFor('//div/div[1]/button');
        expect(button).not.toBeNull();
        await button.click();
        
        // wait until log popup is displayed
        //*[@id="root"]/div/div[1]/div[4]/div[2]/div/div[2]
        await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

        const log_description = await page.evaluate(() => document.evaluate('//h3', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(log_description).toBe("Navigation Action");
    });

    it('Check User Button', async () => {

        // check if exists the button
        await page.waitFor('span[aria-label="Avatar for anonymous user"]', {timeout: 2000});

        // validate user name
        const text = await page.evaluate(() => document.evaluate('//button/span/span', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent);
        expect(text).toBe("AU") 
    });

/*
    it('test version', async () =>{
        await page.goto("https://localhost:5001/version", {waitUntil: 'networkidle0'});
        const version = await page.evaluate(() => document.body.textContent);

        expect(version).toBe("1.0.1");
    });
    */
});