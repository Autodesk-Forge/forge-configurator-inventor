import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ForgeView } from './forgeView';

Enzyme.configure({ adapter: new Adapter() });

const viewerDocumentMock = {
  getRoot: () => ({
    getDefaultGeometry: () => ''
  })
}

const loadDocumentNodeMock = jest.fn();

class GuiViewer3DMock {
  addEventListener = jest.fn();
  loadDocumentNode = loadDocumentNodeMock;
  start = jest.fn();
}

const AutodeskMock = {
  Viewing: {
    GuiViewer3D: GuiViewer3DMock,
    Initializer: (_, handleViewerInit) => {
      handleViewerInit();
    },
    Document: {
      load: (_, onLoadSuccess) => {
        onLoadSuccess(viewerDocumentMock);
      }
    }
  }
}

beforeEach(() => {
  loadDocumentNodeMock.mockClear();
});

describe('components', () => {
  describe('ForgeView', () => {
    it('load gets called when svf provided', () => {      
      const baseProps = { activeProject: { svf: 'aaa111' } };  
      const wrapper = shallow(<ForgeView { ...baseProps } />);

      const viewer = wrapper.find('.viewer');
      expect(viewer).toHaveLength(1);
      const script = viewer.find('Script');
      expect(script).toHaveLength(1);

      window.Autodesk = AutodeskMock;
      script.simulate('load');
      expect(loadDocumentNodeMock).toBeCalled();
    });

    it('load gets called when svf changes', () => {
      const baseProps = { activeProject: { svf: 'aaa111' } };  
      const wrapper = shallow(<ForgeView { ...baseProps } />);

      window.Autodesk = AutodeskMock;
      const script = wrapper.find('Script');
      script.simulate('load');

      const updateProps = { activeProject: { svf: 'newurl' } };  
      wrapper.setProps(updateProps);
      expect(loadDocumentNodeMock).toHaveBeenCalledTimes(2);
    });

    it('returns without loading when svf is null', () => {
      const baseProps = { activeProject: { svf: null } };  
      const wrapper = shallow(<ForgeView { ...baseProps } />);

      window.Autodesk = AutodeskMock;
      const script = wrapper.find('Script');
      script.simulate('load');

      expect(loadDocumentNodeMock).toHaveBeenCalledTimes(0);
    });    
  });
});