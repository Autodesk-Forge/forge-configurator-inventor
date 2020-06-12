import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { App } from './App';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('App', () => {
    it('Test that app will fetch info about showing changed parameters ', () => {
        const fetchShowParametersChanged = jest.fn();
        const detectToken = jest.fn();

        const props = {
          fetchShowParametersChanged,
          detectToken
        };

        shallow(<App {...props}/>);
        expect(detectToken).toHaveBeenCalled();
        expect(fetchShowParametersChanged).toHaveBeenCalled();
    });
  });
});