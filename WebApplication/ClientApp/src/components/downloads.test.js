import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Downloads, DownloadsTable, downloadColumns as columns } from './downloads';

Enzyme.configure({ adapter: new Adapter() });

const activeProject = {
    id: '1',
    label: 'New Project',
    modelDownloadUrl: 'a/b/c'
  };
  
  const baseProps = {
    activeProject
  };

  // get rid of these fake data ?

  const iamClickHandler = jest.fn();
  let iamDownloadHyperlink = null;
  const iamDownloadLink = <a href={activeProject.modelDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
    iamDownloadHyperlink = h;
}}>IAM</a>;

  const rfaClickHandler = jest.fn();
  const rfaDownloadLink = <a href="#" onClick={(e) => { e.preventDefault(); }}>RFA</a>;

  const data = [
    {
        id: 'updatedIam',
        icon: 'products-and-services-24.svg',
        type: 'IAM',
        env: 'Model',
        link: iamDownloadLink,
        clickHandler: iamClickHandler
    },
    {
        id: 'rfa',
        icon: 'products-and-services-24.svg',
        type: 'RFA',
        env: 'Model',
        link: rfaDownloadLink,
        clickHandler: rfaClickHandler
    }
];

// end of fake data

  const tableProps = {
    'width': 100,
    'height': 50,
    'columns': columns,
    'data': data
  }

describe('components', () => {
  describe('Downloands', () => {
    it('renders Downloads AutoResizer', () => {
        const wrapper = shallow(<Downloads { ...baseProps } />);
        const wrapperComponent = wrapper.find('AutoResizer');
        expect(wrapperComponent.length).toEqual(1);
      });

      it('passes base props to base table', () => {
        const wrapper = shallow(<DownloadsTable { ...tableProps } />);

        const wrapperComponent = wrapper.find('BaseTable');
        expect(wrapperComponent.prop('width')).toEqual(100);
        expect(wrapperComponent.prop('height')).toEqual(50);
        expect(wrapperComponent.prop('columns').length).toEqual(3);
        expect(wrapperComponent.prop('data').length).toEqual(2);
        expect(wrapperComponent.prop('rowEventHandlers').onClick).toBeDefined();
      });

      it('has the correct row data', () => {
        const wrapper = mount(<DownloadsTable { ...tableProps } />);
        const wrapperComponent = wrapper.find('BaseTable');
        const tableRows = wrapperComponent.find('TableRow');
        expect(tableRows.length).toEqual(2);

        tableRows.forEach((row, index) => {
            const icon = row.find('Icon');
            expect(icon.prop('iconname')).toEqual(data[index].icon);

            const hyperlink = row.find('a');
            switch(index) {
                case 0:
                    expect(hyperlink.prop('href')).toEqual(activeProject.modelDownloadUrl);
                    break;

                case 1:
                    expect(hyperlink.prop('href')).toEqual('#');                    
                    break;
            } 
        });     
    });
  });
});
