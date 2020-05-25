import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ForgeView } from './forgeView';

Enzyme.configure({ adapter: new Adapter() });

const activeProject = {
  id: '1',
  label: 'New Project',
  image: 'new_image.png',
  svf: 'aaa111'
};

const baseProps = {
  activeProject
};

describe('components', () => {
  describe('ForgeView', () => {
    it('ForgeView DOM core structure is as expected', () => {
      const wrapper = shallow(<ForgeView { ...baseProps } />);
      const handleScriptLoadMock = jest.fn();
      wrapper.instance().handleScriptLoad = handleScriptLoadMock;
      wrapper.instance().forceUpdate();
      const viewer = wrapper.find('.viewer');
      expect(viewer).toHaveLength(1);
      const script = viewer.find('Script');
      expect(script).toHaveLength(1);
      script.simulate('load');
      expect(handleScriptLoadMock).toBeCalled();
    });
  });
});