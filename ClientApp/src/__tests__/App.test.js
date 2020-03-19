// prepare mocks
jest.mock('../Repository');
import { getRepo } from '../Repository';

const loadProjectsMock = jest.fn();
getRepo.mockImplementation(
  () => ({
    loadProjects: loadProjectsMock
  })
);

import React from 'react';
import { unmountComponentAtNode, render } from 'react-dom';
import { act } from "react-dom/test-utils";
import App from '../App';


describe.skip('test App component', () => {

  let container = null;
  beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    document.body.appendChild(container);
    act(() => { render(<App />, container); });

    loadProjectsMock.mockClear();
  });

  afterEach(() => {
    // cleanup on exiting
    unmountComponentAtNode(container);
    container.remove();
    container = null;
  });

  /** Get text content from HTML element */
  function tc(selector) {
    return container.querySelector(selector).textContent;
  }

  it('should check empty list on start', () => {

    expect(tc('#project-list')).toBe("No projects loaded");
    expect(loadProjectsMock).toHaveBeenCalledTimes(0);
  });

  it('check loaded content', () => {

    loadProjectsMock.mockReturnValue([{ name: 'foo' }]);

    // change state on button click
    act(() => { button.dispatchEvent(new MouseEvent('click', { bubbles: true })); });

    expect(loadProjectsMock).toHaveBeenCalledTimes(1);
    expect(tc("#project-list")).toMatch(/foo/);
  });
});
