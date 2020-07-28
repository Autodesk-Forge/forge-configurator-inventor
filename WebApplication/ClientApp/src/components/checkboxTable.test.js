/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import React from 'react';
import Enzyme, { shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { CheckboxTable } from './checkboxTable';

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

const props = {
    projectList: projectList
};

describe('CheckboxTable components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<CheckboxTable { ...props} />);

    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    expect(basetable.prop('width')).toEqual(100); // no longer reducing the size
    expect(basetable.prop('height')).toEqual(200); // no longer reducing the size
  });

  it('Base table has expected columns', () => {
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const cols = basetable.prop('columns');
    expect(cols.length).toEqual(3);
  });

  it('Base table has expected data', () => {
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const basetabledata = basetable.prop('data');
    expect(basetabledata.length).toEqual(projectList.projects.length);
    basetabledata.forEach((datarow, index) => {
        expect(datarow.id).toEqual(projectList.projects[index].id);
        expect(datarow.label).toEqual(projectList.projects[index].label);
    });
  });

  it('Row event handler passes projectId to onProjectClick', () => {
    const onProjectClick = jest.fn();
    const wrapper = shallow(<CheckboxTable { ...props } onProjectClick={onProjectClick} />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    basetable.prop('rowEventHandlers').onClick({ rowData: { id: '7' }});
    expect(onProjectClick).toHaveBeenCalledWith('7');
  });

  describe('Column driven click handling', () => {
    const emock = {
      stopPropagation: jest.fn(),
      preventDefault: jest.fn()
    };
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );

    it('OnClick is stopped for the first column', () => {
      const cellPropObject = basetable.prop('cellProps')({ columnIndex: 0});
      cellPropObject.onClick(emock);
      expect(emock.stopPropagation).toHaveBeenCalled();
      expect(emock.preventDefault).toHaveBeenCalled();
    });

    it('OnClick is allowed for other columns', () => {
      let cellPropObject = basetable.prop('cellProps')({ columnIndex: 1});
      expect(cellPropObject.onClick).toBeUndefined();

      cellPropObject = basetable.prop('cellProps')({ columnIndex: 2});
      expect(cellPropObject.onClick).toBeUndefined();
    });
  });

  describe("CheckboxTable checkboxes", () => {

    const clearCheckedProjects = jest.fn();
    const setProjectChecked = jest.fn();
    const setCheckedProjects = jest.fn();
    const wrapper = shallow(<CheckboxTable
      setProjectChecked={setProjectChecked}
      clearCheckedProjects={clearCheckedProjects}
      setCheckedProjects={setCheckedProjects}
      selectable={true}
      {...props } />);
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
        expect(setProjectChecked).toBeCalledWith('2', true);
      });
    });

  });

  it("Renders Icon cell", () => {
    const wrapper = shallow(<CheckboxTable { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const columns = basetable.prop('columns');

    const iconName = 'myIcon.svg';
    const cell = columns[1].cellRenderer({ cellData: iconName });
    const cellWrapper = shallow(cell);
    expect(cellWrapper.find('img').prop('src')).toEqual(iconName);
  });
});
