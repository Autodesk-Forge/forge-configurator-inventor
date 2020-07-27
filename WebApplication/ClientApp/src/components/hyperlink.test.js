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
import Enzyme, { mount } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import HyperLink from './hyperlink';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('hyperlink', () => {
    it('verify that when clicked link, called passed function(used for close modal UI)', () => {
        const onUrlClickMock = jest.fn();
        const props = {
          href: "",
          prefix: "P ",
          link: "link",
          suffix: " S",
          onUrlClick: onUrlClickMock
        };

        const wrapper = mount(<HyperLink {...props}/>);
        const href = wrapper.find('a');
        href.simulate('click');
        expect(onUrlClickMock).toHaveBeenCalledTimes(1);
      });
    it('verify that is automatically started download of specified link', () => {
        const onAutostartMock = jest.fn();
        const props = {
          href: "link to file",
          prefix: "P ",
          link: "link text",
          suffix: " S",
          onAutostart: onAutostartMock
        };

        mount(<HyperLink {...props}/>);
        expect(onAutostartMock).toHaveBeenCalledTimes(1);
      });

  });
});
