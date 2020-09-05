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
import { ParametersContainer } from './parametersContainer';
import { ModalProgress } from './modalProgress';

Enzyme.configure({ adapter: new Adapter() });

const params = [
    {
        "name": "Length",
        "value": "12000 mm",
        "type": "NYI",
        "units": "mm",
        "allowedValues": []
    },
    {
        "name": "Width",
        "value": "2000 mm",
        "type": "NYI",
        "units": "mm",
        "allowedValues": []
    }
];

const fetchMock = jest.fn();
const projectId = 1;

const emptyProps = {
    activeProject: { id: projectId },
    fetchParameters: fetchMock
};

describe('parameters container', () => {

    beforeEach(() => {
        fetchMock.mockClear();
    });

    it('verify fetchParameters called immediatelly when component rendered', () => {
        shallow(<ParametersContainer {...emptyProps} />);
        expect(fetchMock).toHaveBeenCalledWith(projectId);
    });

    it('verify fetchParameters called immediatelly when active projecte id was changed', () => {
        const noProj = {
            activeProject: {},
            fetchParameters: fetchMock
        };

        const wrapper = shallow(<ParametersContainer {...noProj} />);
        fetchMock.mockClear();

        const updatedProps = Object.assign(noProj, { activeProject: { id: 2 } });
        wrapper.setProps(updatedProps);
        expect(fetchMock).toHaveBeenCalledWith(2);
    });

    it('should show special message for empty parameters', () => {

        const wrapper = shallow(<ParametersContainer {...emptyProps} />);
        const wrapperComponent = wrapper.find('.parameters');
        const content = wrapperComponent.prop('children');
        expect(content).toEqual("No parameters");
    });

    it('should propagate data to proper properties', () => {

        // prefill props with properties
        const props = Object.assign({ projectUpdateParameters: params }, emptyProps);

        const wrapper = shallow(<ParametersContainer {...props} />);
        const wrapperComponent = wrapper.find('.parameters');
        const children = wrapperComponent.prop('children');

        expect(children).toHaveLength(2);
        expect(children[0].props.parameter).toBe(params[0]);
        expect(children[1].props.parameter).toBe(params[1]);
    });

    it('verify update button connected', () => {
        const fnMock = jest.fn();
        const invalidateDrawingMock = jest.fn();
        const props = {
            activeProject: { id: projectId },
            fetchParameters: () => {},
            updateModelWithParameters: fnMock,
            invalidateDrawing: invalidateDrawingMock,
            projectUpdateParameters: null
        };

        const wrapper = shallow(<ParametersContainer {...props} />);
        const wrapperComponent = wrapper.find({ title: 'Update' });
        wrapperComponent.simulate('click');
        expect(fnMock).toHaveBeenCalledWith(projectId, props.projectUpdateParameters);
    });

    it('verify reset button connected', () => {
        const fnMock = jest.fn();
        const props = {
            activeProject: { id: projectId },
            fetchParameters: () => {},
            resetParameters: fnMock,
            projectSourceParameters: null
        };

        const wrapper = shallow(<ParametersContainer {...props} />);
        const wrapperComponent = wrapper.find({ title: 'Reset' });
        wrapperComponent.simulate('click');
        expect(fnMock).toHaveBeenCalledWith(projectId, props.projectSourceParameters);
    });

    it('verify when modal close called, here is no operation called NOW', () => {
        const fnMock = jest.fn();
        const props = {
            activeProject: { id: projectId },
            fetchParameters: () => {},
            showModalProgress: fnMock,
            modalProgressShowing: true
        };

        const wrapper = shallow(<ParametersContainer {...props} />);
        const wrapperComponent = wrapper.find(ModalProgress);
        wrapperComponent.props().onClose();
        expect(fnMock).toHaveBeenCalledTimes(0);
    });
});
