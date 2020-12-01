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

import React, { Component } from 'react';
import { connect } from 'react-redux';

import { getProfile, getActiveProject, parametersEditedMessageVisible} from '../reducers/mainReducer';
import { hideUpdateMessageBanner } from '../actions/uiFlagsActions';

import './message.css';
import Banner from '@hig/banner';
import Button from '@hig/button';
import Checkbox from '@hig/checkbox';

import ThemeContext from '@hig/theme-context';
import HIGMediumDensityTheme from '@hig/theme-data/build/esm/webLightMediumDensityTheme';

import styled from 'styled-components';

// use 283px instead of the 296px, to move 'X' close button 13px more to the right
const Fixed = styled('div')`
  position: fixed;
  z-index: 100;
  width: calc(100% - 283px);
`;

export class Message extends Component {

    constructor(props) {
        super(props);
        this.onDismiss = this.onDismiss.bind(this);
        this.dontShowAgain = false;
    }

    onDismiss() {
        this.props.hideUpdateMessageBanner(this.dontShowAgain);
    }

    render() {
        const loggedIn = this.props.profile.isLoggedIn;
        const visible = this.props.parametersEditedMessageVisible;

        return (
          <ThemeContext.Provider value={HIGMediumDensityTheme}>
          <Fixed>
            <Banner
            type="primary"
            actions={({ isWrappingActions }) => (
                <Banner.Interactions isWrappingActions={isWrappingActions}>
                  <Banner.Action>
                    <Button className="button" style={
                      { width: '99px', height: '36px', borderRadius: '2px', marginLeft: '12px'}}
                      type="secondary"
                      size="small"
                      width={isWrappingActions ? "grow" : "shrink"}
                      title="Ok"
                      onClick={this.onDismiss}
                    />
                  </Banner.Action>
                  <div className="verticalseparator"/>
                  { loggedIn && <div className="dontshowplaceholder">
                    <Banner.Action>
                      <Checkbox onChange={(checked) => this.dontShowAgain = checked}/>
                      <ThemeContext.Consumer>{({ resolvedRoles }) => (
                        <div style={{
                            fontFamily: resolvedRoles["basics.fontFamilies.main"],
                            fontSize: resolvedRoles["basics.fontSizes.mediumMedium"],
                            marginLeft: '12px'
                        }}>Don&apos;t show again.
                        </div>
                    )}</ThemeContext.Consumer>
                    </Banner.Action>
                    <div className="verticalseparator"/>
                  </div>
                  }
                </Banner.Interactions>
              )}
            onDismiss={this.onDismiss}
            isVisible={visible}
            >
            The model is out-of-date. Click Update to display the updated state.
            </Banner>
            </Fixed>
            </ThemeContext.Provider>
            );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    return {
        activeProject: getActiveProject(store),
        profile: getProfile(store),
        parametersEditedMessageVisible: parametersEditedMessageVisible(store)
    };
}, { hideUpdateMessageBanner })(Message);