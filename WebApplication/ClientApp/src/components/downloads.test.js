import React from 'react';
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Downloads, downloadColumns } from './downloads';

Enzyme.configure({ adapter: new Adapter() });

const props = {
    activeProject: {
        modelDownloadUrl: 'a/b/c/'
    }
}

describe('Downloads components', () => {
  it('Resizer reduces size', () => {
    const wrapper = shallow(<Downloads { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('width')).toEqual(84);
    expect(bt.prop('height')).toEqual(184);
  });

  it('Row event handlers calls the inner function', () => {
    const wrapper = shallow(<Downloads { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const jestfce = jest.fn();
    bt.prop('rowEventHandlers').onClick({ rowData: { clickHandler: jestfce }});
    expect(jestfce).toHaveBeenCalledTimes(1);
  });

  it('Base table has expected columns', () => {
    const wrapper = shallow(<Downloads { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    expect(bt.prop('columns')).toEqual(downloadColumns);
  });

  it('Base table has expected data', () => {
    const wrapper = shallow(<Downloads { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const btdata = bt.prop('data');

    const iam = btdata[0];
    expect(iam.id).toEqual('updatedIam');
    const iamlink = shallow(iam.link);
    expect(iamlink.prop('href')).toEqual(props.activeProject.modelDownloadUrl);
    const stopPropagation = jest.fn();
    iamlink.simulate('click', { stopPropagation });
    expect(stopPropagation).toHaveBeenCalled();
    // iam.clickHandler();

    const rfa = btdata[1];
    expect(rfa.id).toEqual('rfa');
    const rfalink = shallow(rfa.link);
    expect(rfalink.prop('href')).toEqual('');
    const preventDefault = jest.fn();
    rfalink.simulate('click', { preventDefault });
    expect(preventDefault).toHaveBeenCalled();
  });

  it('Base table renders expected count of links and icons', () => {
    const wrapper = mount(<Downloads { ...props } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const icons = bt.find('Icon');
    const hyperlinks = bt.find('a');
    expect(icons.length).toEqual(2);
    expect(hyperlinks.length).toEqual(2);
  });
});
