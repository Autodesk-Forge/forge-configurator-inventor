import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { CheckboxTable, projectListColumns } from './checkboxTable';

import { Provider } from 'react-redux';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

Enzyme.configure({ adapter: new Adapter() });

const projectList = {
  activeProject: {
      id: '2'
  },
  projects: [
      {
          id: '1',
          label: 'One'
      },
      {
          id: '2',
          label: 'Two'
      },
      {
          id: '3',
          label: 'Three'
      }
  ]
};

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);
const mockState = {
  uiFlags: {
    showUploadPackage: false,
    package: { file: '', root: '' },
    checkedProjects: []
  },
  projectList: projectList
};

const props = {
    projectList: projectList
};

describe('CheckboxTable components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<CheckboxTable { ...props} />);

    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('width')).toEqual(100); // no longer reducing the size
    expect(bt.prop('height')).toEqual(200); // no longer reducing the size
  });
});
