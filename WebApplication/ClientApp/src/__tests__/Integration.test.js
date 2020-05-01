const playwright = require('playwright');

// IMPORTANT: this is the page URL.  And it's expected that backend is running.
const pageUrl = "https://localhost:5001";

const config = {
    //headless: false, // NOTE: uncomment to see what is actually going o the screen
    // devtools: true, // NOTE: uncomment for debugging
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

    beforeAll(async () => {

        // TODO: try different browser kinds
        browser = await playwright["chromium"].launch(config);
    });

    afterAll(async () => {
        await browser.close();
    });

    beforeEach(async () => {

        page = await browser.newPage();

        if (!page) {
            throw new Error("Connection wasn't established");
        }

        // report about requested URLs
        await page.route('**', async (request) => {

            // console.log(request.url());
            await request.continue();
        });

        // Open the page
        await page.goto(pageUrl, { waitUntil: "networkidle0" });

        // check the page is loaded
        expect(page).not.toBeNull();
        expect(await page.title()).not.toBeNull();
    });

    afterEach(async () => {
        await page.close();
    });

    /** Get text content for the given single node at the xpath */
    async function getTextContent(xpath) {
        return await page.evaluate(
                            expression => document.evaluate(expression, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent,
                            xpath);
    }

    /** Click on the single node at the xpath */
    async function clickXPath(xpath) {
        return await page.evaluate(
                            expression => document.evaluate(expression, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click(),
                            xpath);
    }

    describe('General', () => {

        it('should check if Autodesk Forge link works', async () => {

            // check Forge link
            const link = await page.waitFor('a[aria-label="Autodesk HIG"]', {timeout: 3000});
            await link.click();

            //wait for Autodesk Forge page
            await page.waitForSelector('.adskf__navbar-logo', {visible: true, timeout: 5000});
            expect(await page.title()).toBe("Autodesk Forge");
        });

        it('should check Version endpoint', async () => {
            await page.goto(pageUrl + '/Version');
            expect(await page.$('pre')).not.toBeNull();
        });
    });

    describe('Projects tab', () => {

        it(`should check projects list is loaded`, async () => {

            // this is the default tab, so no switch is required

            // wait until project list is refreshed.
            // This selector does not exist on page opening, and will be created after projects are loaded from the server. Wait for it.
            const listItems = await page.$$("#project-list ul li");
            expect(listItems).toHaveLength(2); // expecting two default projects
        });
    });

    describe('Toolbar', () => {

        describe('Project switcher', () => {

            it('should check Project switcher is loaded', async () => {

                // wait until project combo is displayed
                const project = await page.waitFor('//p',{timeout: 3000});
                await project.click();

                // wait until project list is displayed
                await page.waitForSelector('//div/div[2]', {visible: true, timeout: 2000});

                // check content of PROJECTS menu
                const caption = await getTextContent('//ul/span');
                expect(caption).toBe("Projects");

                // check name of the first project
                const firstDemoProject = await getTextContent('//ul/li[1]/span[2]');
                expect(firstDemoProject).toBe("Conveyor.zip");

                // check name of the second project
                const secondDemoProject = await getTextContent('//ul/li[2]/span[2]');
                expect(secondDemoProject).toBe("Wrench.zip");
            });

            it('should check if Project switcher allows to switch project', async () => {

                // wait until project combo is displayed
                const project = await page.waitFor('//p',{timeout: 3000});
                await project.click();

                // wait until project list is displayed
                await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

                // emulate click to trigger project loading
                await clickXPath('//ul/li[1]/span[2]');
                let project_name = await getTextContent('//p');
                expect(project_name).toBe("Conveyor.zip");

                // click to show popup menu with list of projects
                await clickXPath('//p');

                // wait until project list is displayed
                await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

                // emulate click to trigger project loading
                await clickXPath('//ul/li[2]/span[2]');
                project_name = await getTextContent('//p');
                expect(project_name).toBe("Wrench.zip");
             });

        });


        it('should check presence of Log button', async () => {

            // check if exists the button
            const button = await page.waitFor('//div/div[1]/button');
            expect(button).not.toBeNull();
            await button.click();

            // wait until log popup is displayed
            await page.waitForSelector('//div/div[2]', {visible: true, timeout: 3000});

            const log_description = await getTextContent('//h3');
            expect(log_description).toBe("Navigation Action");
        });

        it('should check presence of User button', async () => {

            // check if exists the button
            await page.waitFor('span[aria-label="Avatar for anonymous user"]', {timeout: 2000});

            // validate user name
            const text = await getTextContent('//button/span/span');
            expect(text).toBe("AU");
        });
    });
});
