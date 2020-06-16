import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ProjectList, projectListColumns } from './projectList';

import { Provider } from 'react-redux';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

Enzyme.configure({ adapter: new Adapter() });

// mock store
const middlewares = [thunk];
const mockStore = configureMockStore(middlewares);
const mockState = {
  uiFlagsReducer: {
    showUploadPackage: false,
    package: { file: '', root: '' }
  }
};

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

const props = {
    projectList: projectList
};

describe('ProjectList components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<ProjectList { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('width')).toEqual(100); // no longer reducing the size
    expect(bt.prop('height')).toEqual(200); // no longer reducing the size
  });

  it('Base table has expected columns', () => {
    const wrapper = shallow(<ProjectList { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('columns')).toEqual(projectListColumns);
  });

  it('Base table has expected data', () => {
    const wrapper = shallow(<ProjectList { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const btdata = bt.prop('data');
    expect(btdata.length).toEqual(projectList.projects.length);
    btdata.forEach((datarow, index) => {
        expect(datarow.id).toEqual(projectList.projects[index].id);
        expect(datarow.label).toEqual(projectList.projects[index].label);
    });
  });

  it('Base table renders expected count of links and icons', () => {
    let store = mockStore(mockState);

    const wrapper = mount(
      <Provider store={store}>
        <ProjectList { ...props } />
      </Provider>
    );

    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const icons = bt.find('Icon');
    expect(icons.length).toEqual(projectList.projects.length);
  });
});
