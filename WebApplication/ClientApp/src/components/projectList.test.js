import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { ProjectList, projectListColumns } from './projectList';

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

describe('ProjectList components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<ProjectList { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('width')).toEqual(84);
    expect(bt.prop('height')).toEqual(184);
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
    const wrapper = mount(<ProjectList { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const icons = bt.find('Icon');
    expect(icons.length).toEqual(projectList.projects.length);
  });
});
