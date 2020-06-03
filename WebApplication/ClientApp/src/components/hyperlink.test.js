import React from 'react';
import Enzyme, { mount, shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import HyperLink from './hyperlink';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('hyperlink', () => {
    it('verify that when clicked link, called passed function(used for close modal UI)', () => {
        const onUrlClickMock = jest.fn();
        const props = {
          autostart: false,
          href: "",
          prefix: "P ",
          link: "link",
          suffix: " S",
          onUrlClick: onUrlClickMock
        };

        const wrapper = mount(<HyperLink {...props}/>);
        const href = wrapper.find('a');
        href.simulate('click');
        expect(onUrlClickMock).toHaveBeenCalledTimes(1);
      });
    it('verify that is automatically started download of specified link', () => {
        const onAutostartMock = jest.fn();
        const props = {
          autostart: true,
          href: "link to file",
          prefix: "P ",
          link: "link text",
          suffix: " S",
          onAutostart: onAutostartMock
        };

        mount(<HyperLink {...props}/>);
        expect(onAutostartMock).toHaveBeenCalledTimes(1);
      });

  });
});
