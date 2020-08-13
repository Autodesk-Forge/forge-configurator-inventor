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
import Enzyme, { shallow, mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import { Downloads, downloadColumns } from './downloads';

Enzyme.configure({ adapter: new Adapter() });

const props = {
    activeProject: {
        id: 'foo',
        modelDownloadUrl: 'a/b/c/',
        bomDownloadUrl: 'd/e/f/'
    }
};

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

  it.each([
    [ {}, 0], // empty project info => no download links
    [ { id: 'foo' }, 1], // no URLs => only RFA link if available (extreme case for code coverage)
    [ props.activeProject, 2 ], // no `isAssembly` field. Assuming - no BOM is available (extreme case for code coverage)
    [ { ...props.activeProject, isAssembly: false }, 2 ], // no BOM available for parts
    [ { ...props.activeProject, isAssembly: true }, 3 ] // BOM download is expected
  ])('Base table renders expected count of links and icons - %0 case',
    (project, count) => {

      const props = { activeProject: project };
      const wrapper = mount(<Downloads { ...props } />);
      const as = wrapper.find('AutoResizer');
      const bt = as.renderProp('children')( {width: 100, height: 200} );

      const icons = bt.find('Icon');
      const hyperlinks = bt.find('a');

      expect(icons.length).toEqual(count);
      expect(hyperlinks.length).toEqual(count);
    });

  it('Base table renders NO links and icons when projects are empty', () => {
    // simulate activeProject (getActiveProject) like we have in these two scenarios:
    // 1) don't have initialized projects yet
    // 2) user don't have any projects
    const noActiveProjectProps = { activeProject: {} };
    const wrapper = mount(<Downloads { ...noActiveProjectProps } />);
    const as = wrapper.find('AutoResizer');
    const bt = as.renderProp('children')( {width: 100, height: 200} );
    const icons = bt.find('Icon');
    const hyperlinks = bt.find('a');
    expect(icons.length).toEqual(0);
    expect(hyperlinks.length).toEqual(0);
  });

});
