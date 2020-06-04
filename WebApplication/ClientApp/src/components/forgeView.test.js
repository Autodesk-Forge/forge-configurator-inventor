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

class GuiViewer3DMock {
  addEventListener = jest.fn();
  loadDocumentNode = jest.fn();
  start = jest.fn();
}

const AutodeskMock = {
  Viewing: {
    GuiViewer3D: GuiViewer3DMock,
    Initializer: (_, handleViewerInit) => {
      handleViewerInit();
    },
    Document: {
      load: jest.fn()
    }
  }
}

describe('components', () => {
  describe('ForgeView', () => {
    it('ForgeView DOM core structure is as expected', () => {
      const wrapper = shallow(<ForgeView { ...baseProps } />);

      const viewer = wrapper.find('.viewer');
      expect(viewer).toHaveLength(1);
      const script = viewer.find('Script');
      expect(script).toHaveLength(1);

      window.Autodesk = AutodeskMock;
      script.simulate('load');
      expect(AutodeskMock.Viewing.Document.load).toBeCalled();
    });
  });
});