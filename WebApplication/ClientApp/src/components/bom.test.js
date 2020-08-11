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
import {Bom} from './bom';

jest.mock('./bomUtils');
import {getMaxColumnTextWidth} from './bomUtils';
getMaxColumnTextWidth.mockImplementation(() => 10);

Enzyme.configure({ adapter: new Adapter() });

const activeProject = { id: 'projectB' };
const bomDataB = {
    columns: [ {label: "Part Number"}, {label: "Description"} ],
    data: [ [ "1101", "Crew cabin"], [ "7.11", "Ejector"] ]
};
const fetchBomMock = jest.fn();
const props = {
    activeProject: activeProject,
    bomData: bomDataB,
    fetchBom: fetchBomMock
};

// base table data look like:
const sampleBTData = [
    {
        id: "row-0",
        parentId: null,
        'column-0': "1101",
        'column-1': "Crew cabin"
    },
    {
        id: "row-1",
        parentId: null,
        'column-0': "7.11",
        'column-1': "Ejector"
    },
];

describe('BOM component', () => {
    beforeEach(() => {
        fetchBomMock.mockClear();
    });

    it('Base table has expected columns', () => {
    const wrapper = shallow(<Bom { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    expect(basetable.prop('columns').length).toEqual(bomDataB.columns.length + 2); // +2 for left and right align column
    });

    it('Base table has expected data', () => {
    const wrapper = shallow(<Bom { ...props } />);
    const autoresizer = wrapper.find('AutoResizer');
    const basetable = autoresizer.renderProp('children')( {width: 100, height: 200} );
    const tabledata = basetable.prop('data');

    expect(tabledata).toEqual(sampleBTData);
    });

    it('Fetches bom on first render', () => {
        shallow(<Bom { ...props } />);
        expect(fetchBomMock).toHaveBeenCalledWith(activeProject);
    });

    it('Fetched bom on project change', () => {
        const localProps = {
            activeProject: { id: 'projectB' },
            fetchBom: fetchBomMock
        };

        const wrapper = shallow(<Bom {...localProps} />);
        fetchBomMock.mockClear();

        const newActiveProject = { id: '1' };
        const updatedProps = Object.assign(localProps, { activeProject: newActiveProject });
        wrapper.setProps(updatedProps);
        expect(fetchBomMock).toHaveBeenCalledWith(newActiveProject);
    });
});