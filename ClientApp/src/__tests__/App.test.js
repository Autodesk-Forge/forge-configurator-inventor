import React from 'react';
import { unmountComponentAtNode, render } from 'react-dom';
import { act } from "react-dom/test-utils";
import App from '../App';

let container = null;
beforeEach(() => {
  // setup a DOM element as a render target
  container = document.createElement("div");
  document.body.appendChild(container);
});

afterEach(() => {
  // cleanup on exiting
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

it('Check expected context', () => {

    act(() => { render(<App />, container); });

    const button = container.querySelector('button');
    expect(button.textContent).toBe("I am Autodesk HIG button and I am doing nothing");
    //expect(button.type).toBe("primary");

    // change state on button click
    act(() => { button.dispatchEvent(new MouseEvent('click', { bubbles: true }));});
    expect(button.textContent).toBe("Oops");
});
