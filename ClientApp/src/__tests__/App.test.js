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

let container = null;
beforeEach(() => {
  // setup a DOM element as a render target
  container = document.createElement("div");
  document.body.appendChild(container);

  loadProjectsMock.mockClear();
});

afterEach(() => {
  // cleanup on exiting
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

it('Check expected context', () => {

  loadProjectsMock.mockReturnValue([]);

  act(() => { render(<App />, container); });

  const button = container.querySelector('button');
  expect(button.textContent).toBe("I am Autodesk HIG button and I am doing nothing");

  // change state on button click
  act(() => { button.dispatchEvent(new MouseEvent('click', { bubbles: true })); });
  expect(button.textContent).toBe("Oops");

  expect(loadProjectsMock).toHaveBeenCalledTimes(1);
});
