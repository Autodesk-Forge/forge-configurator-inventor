import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Toolbar } from './toolbar';


Enzyme.configure({ adapter: new Adapter() });

describe('Components', () => {
  describe('toolbar', () => {
    it('verify that toolbar will load profile when its mounted', () => {

        const loadProfileMock = jest.fn();
        const toolbarProps = {
          loadProfile: loadProfileMock,
          profile: {
            name: 'profileName',
            avatarUrl: 'avatarUrl'
          }
        };

        shallow(
          <Toolbar {...toolbarProps}/>
        );

        expect(loadProfileMock).toHaveBeenCalled();
    });

    it('verify that profilename and avatarUrl is sent to properties of ProfileAction', () => {
      const loadProfileMock = jest.fn();
        const toolbarProps = {
          loadProfile: loadProfileMock,
          profile: {
            name: 'profileName',
            avatarUrl: 'avatarUrl'
          }
        };

      const toolbar = (<Toolbar {...toolbarProps} />);
      const wrapper = shallow(
        toolbar,
        {disableLifecycleMethods: true}
      );

      const rightActionsFragment = wrapper.find('TopNav').prop('rightActions');
      const rightActionsWrapper = shallow(rightActionsFragment.props.children[1]);
      const profileActionWrapper = rightActionsWrapper.find('ProfileAction');
      expect(profileActionWrapper.prop('avatarName')).toEqual(toolbarProps.profile.name);
      expect(profileActionWrapper.prop('avatarImage')).toEqual(toolbarProps.profile.avatarUrl);
    });
  });
});
