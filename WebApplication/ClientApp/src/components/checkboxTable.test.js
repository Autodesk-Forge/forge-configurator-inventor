import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { CheckboxTable as CheckboxTableShallow } from './checkboxTable';
import CheckboxTable from './checkboxTable';
import { Provider } from 'react-redux';
import configureMockStore from 'redux-mock-store';
import thunk from 'redux-thunk';

jest.mock('./checkboxTableHeader');
jest.mock('./checkboxTableRow');

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
  projectList: projectList,
  isLoggedIn: true
};

const props = {
    projectList: projectList
};

describe('CheckboxTable components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<CheckboxTableShallow { ...props} />);

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    expect(basetable.prop('width')).toEqual(100); // no longer reducing the size
    expect(basetable.prop('height')).toEqual(200); // no longer reducing the size
  });

  it('Base table has expected columns', () => {
    const wrapper = shallow(<CheckboxTableShallow { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const cols = basetable.prop('columns');
    expect(cols.length).toEqual(3);
  });

  it('Base table has expected data', () => {
    const wrapper = shallow(<CheckboxTableShallow { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const basetabledata = basetable.prop('data');
    expect(basetabledata.length).toEqual(projectList.projects.length);
    basetabledata.forEach((datarow, index) => {
        expect(datarow.id).toEqual(projectList.projects[index].id);
        expect(datarow.label).toEqual(projectList.projects[index].label);
    });
  });
  describe("CheckboxTable checkboxes", () => {

    const clearCheckedProjects = jest.fn();
    const setCheckedProjects = jest.fn();
    const wrapper = shallow(<CheckboxTableShallow clearCheckedProjects={clearCheckedProjects} setCheckedProjects={setCheckedProjects} selectable={true} {...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const columns = basetable.prop('columns');

    beforeEach(() => {
      clearCheckedProjects.mockClear();
      setCheckedProjects.mockClear();
    });

    describe("Header rendering", () => {
      const header = columns[0].headerRenderer();
      it('Clears all checked', () => {
        header.props.onChange(true);
        expect(clearCheckedProjects).toBeCalled();
      });

      it('Checks all projects', () => {
        header.props.onChange(false);
        expect(setCheckedProjects).toBeCalledWith(projectList.projects.map(project => project.id));
      });

      it('Receive selectable from props', () => {
        expect(header.props.selectable).toBeTruthy();
      });
    });

    describe("Cell rendering", () => {
      const rowData = {id: "2"};
      const cell = columns[0].cellRenderer({rowData});

      it('RowData given in cell renderer are propagated to rendered cell', () => {
        expect(cell.props.rowData).toEqual(rowData);
      });
      it('Checking checkbox', () => {
        cell.props.onChange(true, rowData);
        expect(setCheckedProjects).toBeCalledWith(['2']);
      });
    });

  });

/*  it('Click on project row will launch onProjectClick', () => {
    const store = mockStore(mockState);
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const onProjectClickMockFn = jest.fn();
    const propsWithMock = { ...props,
      onProjectClick: onProjectClickMockFn,
      projectList: projectList
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const row = basetable.find({ rowKey: "3" });
    expect(row.length).toEqual(1);
    row.simulate('click');
    expect(onProjectClickMockFn).toHaveBeenCalledWith('3');
  });

  it('Click on project row, at checkbox cell, will not launch project click action', () => {
    const store = mockStore(mockState);
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const onProjectClickMockFn = jest.fn();
    const propsWithMock = { ...props,
      onProjectClick: onProjectClickMockFn,
      projectList: projectList
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const row = basetable.find({ rowKey: "3" });
    const cell = row.find({ id: "checkbox_hover_visible" });
    expect(cell.length).toEqual(1);
    cell.simulate('click'); // the first is checkbox cell
    expect(onProjectClickMockFn).toHaveBeenCalledTimes(0);
  });

  it('Click on project row, at icon cell, it will launch project click action', () => {
    const store = mockStore(mockState);
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const onProjectClickMockFn = jest.fn();
    const propsWithMock = { ...props,
      onProjectClick: onProjectClickMockFn,
      projectList: projectList
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const row = basetable.find({ rowKey: "3" });
    const cell = row.find({ iconname: "Archive.svg" });
    expect(cell.length).toEqual(1);
    cell.simulate('click'); // click on cell with icon
    expect(onProjectClickMockFn).toHaveBeenCalledTimes(1);
  });

  it('verify that here are no checkboxes when not signed in (selectable)', () => {
    const store = mockStore(mockState);
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const propsWithMock = { ...props,
      projectList: projectList,
      selectable: false // = not logged in
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const checkboxes = basetable.find('#checkbox_header');
    expect(checkboxes.length).toEqual(0);
  });

  it('Click on "select all" checkbox and verify that is called action SET_CHECKED_PROJECTS', () => {
    const expectedCheckedProjects = ['1','2','3']; // prepare what we expected
    const store = mockStore({ ...mockState,  uiFlags: { checkedProjects: expectedCheckedProjects }});
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const propsWithMock = { ...props,
      projectList: projectList,
      selectable: true
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const tableHeader = basetable.find('CheckboxTableHeader');
    const checkbox = tableHeader.find('input');
    expect(checkbox.length).toEqual(1);
    checkbox.simulate('change', { target: { checked: true } }); // click on checkbox to select all

    const expectedAction = [
      {
        'projects': expectedCheckedProjects,
        'type': 'SET_CHECKED_PROJECTS',
      },
    ];

    expect(store.getActions()).toEqual(expectedAction);
  });

  it('Click on "select all" checkbox and verify that is called action CLEAR_CHECKED_PROJECTS', () => {
    const expectedCheckedProjects = ['1','2','3']; // prepare what we expected to be in store
    const store = mockStore({ ...mockState,  uiFlags: { checkedProjects: expectedCheckedProjects }});
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const propsWithMock = { ...props,
      projectList: projectList,
      selectable: true
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const tableHeader = basetable.find('CheckboxTableHeader');
    const checkbox = tableHeader.find('input');
    expect(checkbox.length).toEqual(1);
    checkbox.simulate('change', { target: { checked: true } }); // click on checkbox to select all
    store.clearActions();
    checkbox.simulate('change', { target: { checked: false } }); // click on checkbox to clear all

    const expectedAction = [
      {
        'type': 'CLEAR_CHECKED_PROJECTS',
      },
    ];

    expect(store.getActions()).toEqual(expectedAction);
  });

  it('Click on row checkbox and verify that header checkbox is in indeterminate state', () => {
    const expectedCheckedProjects = ['3']; // prepare what we expected to be in store
    const store = mockStore({ ...mockState,  uiFlags: { checkedProjects: expectedCheckedProjects }});
    function MyProvider(props) {
      return (<Provider store={store}>{props.children}</Provider>);
    }

    const propsWithMock = { ...props,
      projectList: projectList,
      selectable: true
     };

    const wrapper = mount(<CheckboxTable { ...propsWithMock } />, { wrappingComponent: MyProvider } );

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const row = basetable.find({ rowKey: "3" });
    const checkbox = row.find('input');
    expect(checkbox.length).toEqual(1);
    checkbox.simulate('change', { target: { checked: true } }); // click on checkbox to select row

    const expectedAction = [
      {
        'projects': ['3'],
        'type': 'SET_CHECKED_PROJECTS',
      },
    ];

    expect(store.getActions()).toEqual(expectedAction);

    const tableHeader = basetable.find('CheckboxTableHeader');
    const header_checkbox = tableHeader.find('Checkbox');
    expect(header_checkbox.props().checked).toEqual(true);
    expect(header_checkbox.props().indeterminate).toEqual(true);
  });*/
});
