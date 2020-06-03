import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Message } from './message';
// import Checkbox from '@hig/checkbox';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('message', () => {
    it('dismiss message with UNCHECKED dont Show Again', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock
        };

        const wrapper = shallow(<Message {...props}/>);
        const wrapperComponent = wrapper.find('Banner');
        expect(wrapperComponent.length).toEqual(1);
        wrapperComponent.props().onDismiss();
        expect(fnMock).toHaveBeenCalledWith(false);
      });
    it('dismiss message with CHECKED dont Show Again', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock
        };

        const wrapper = mount(<Message {...props}/>);
        const bannerComponent = wrapper.find('Banner');
        expect(bannerComponent.length).toEqual(1);

        const dontShowAgain = wrapper.find(Checkbox);
        expect(dontShowAgain.length).toEqual(1);
        dontShowAgain.props().onChange(true);

        bannerComponent.props().onDismiss();
        expect(fnMock).toHaveBeenCalledWith(true);
      });
  });
});
