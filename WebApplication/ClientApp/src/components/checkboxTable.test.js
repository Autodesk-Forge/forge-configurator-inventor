import React from 'react';
import Enzyme, { shallow/*, mount*/ } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { CheckboxTable } from './checkboxTable';

//import { Provider } from 'react-redux';
// import configureMockStore from 'redux-mock-store';
// import thunk from 'redux-thunk';

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
//const middlewares = [thunk];
// const mockStore = configureMockStore(middlewares);
// const mockState = {
//   uiFlags: {
//     showUploadPackage: false,
//     package: { file: '', root: '' },
//     checkedProjects: []
//   },
//   projectList: projectList
// };

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

  it('Base table has expected columns', () => {
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const cols = bt.prop('columns');
    expect(cols.length).toEqual(3);
  });

  it('Base table has expected data', () => {
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const btdata = bt.prop('data');
    expect(btdata.length).toEqual(projectList.projects.length);
    btdata.forEach((datarow, index) => {
        expect(datarow.id).toEqual(projectList.projects[index].id);
        expect(datarow.label).toEqual(projectList.projects[index].label);
    });
  });

  // eslint-disable-next-line jest/no-commented-out-tests
  // it('Base table renders expected count of links and icons', () => {
  //   const store = mockStore(mockState);

  //   const wrapper = mount(
  //     <Provider store={store}>
  //       <CheckboxTable { ...props } />
  //     </Provider>
  //   );

  //   const as = wrapper.find('AutoResizer');
  //   const bt = as.renderProp('children')( {width: 100, height: 200} );
  //   const icons = bt.find('Icon');
  //   expect(icons.length).toEqual(projectList.projects.length);
  // });

  // eslint-disable-next-line jest/no-commented-out-tests
  // it('Click on project row will start change of active project and tab', () => {
  //   const store = mockStore(mockState);
  //   const updateActiveProjectMockFn = jest.fn();
  //   const updateActiveTabIndexMockFn = jest.fn();
  //   const onProjectClickMockFn = jest.fn();
  //   const propsWithMock = { ...props,
  //     updateActiveProject: updateActiveProjectMockFn,
  //     updateActiveTabIndex: updateActiveTabIndexMockFn,
  //     onProjectClick: onProjectClickMockFn,
  //     projectList: projectList
  //    };

  //   const wrapper = mount(
  //     <Provider store={store}>
  //       <CheckboxTable store={store} { ...propsWithMock } />
  //     </Provider>
  //   );

  //   const as = wrapper.find('AutoResizer');
  //   const bt = as.renderProp('children')( {width: 100, height: 200} );
  //   const row = bt.find({ rowKey: "3" });
  //   expect(row.length).toEqual(1);
  //   row.simulate('click');
  //   expect(updateActiveProjectMockFn).toHaveBeenCalledWith('3');
  //   expect(updateActiveTabIndexMockFn).toHaveBeenCalledWith(1);
  // });

});
