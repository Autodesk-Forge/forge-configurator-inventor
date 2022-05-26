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
import Adapter from '@wojtekmaj/enzyme-adapter-react-17';
import { Message } from './message';
import Checkbox from '@hig/checkbox';

Enzyme.configure({ adapter: new Adapter() });

describe('components', () => {
  describe('message', () => {
    it('dismiss message with UNCHECKED dont Show Again', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock,
          profile: {isLoggedIn: true}
        };

        const wrapper = shallow(<Message {...props}/>);
        const wrapperComponent = wrapper.find('Banner');
        expect(wrapperComponent.length).toEqual(1);
        wrapperComponent.props().onDismiss();
        expect(fnMock).toHaveBeenCalledWith(false);
      });
    it('dismiss message with CHECKED dont Show Again', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock,
          profile: {isLoggedIn: true}
        };

        const wrapper = mount(<Message {...props}/>);
        const bannerComponent = wrapper.find('Banner');
        expect(bannerComponent.length).toEqual(1);

        const dontShowAgain = wrapper.find(Checkbox);
        expect(dontShowAgain.length).toEqual(1);
        dontShowAgain.props().onChange(true);

        bannerComponent.props().onDismiss();
        expect(fnMock).toHaveBeenCalledWith(true);
      });
      it('check dont Show Again does not exist when anonymous', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock,
          profile: {isLoggedIn: false}
        };

        const wrapper = mount(<Message {...props}/>);
        const bannerComponent = wrapper.find('Banner');
        expect(bannerComponent.length).toEqual(1);

        const dontShowAgain = wrapper.find(Checkbox);
        expect(dontShowAgain.length).toEqual(0);
      });
      it('check dont Show Again exists when loggedIn', () => {
        const fnMock = jest.fn();
        const props = {
          parametersEditedMessageVisible: true,
          hideUpdateMessageBanner: fnMock,
          profile: {isLoggedIn: true}
        };

        const wrapper = mount(<Message {...props}/>);
        const bannerComponent = wrapper.find('Banner');
        expect(bannerComponent.length).toEqual(1);

        const dontShowAgain = wrapper.find(Checkbox);
        expect(dontShowAgain.length).toEqual(1);
      });
  });
});
