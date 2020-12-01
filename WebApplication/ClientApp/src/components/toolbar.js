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
import { loadProfile } from '../actions/profileActions';
import { getProfile, loginFailedShowing } from '../reducers/mainReducer';
import UserDetails from './userDetails.js';
import Typography from "@hig/typography";
import ModalFail from './modalFail';
import { showLoginFailed } from '../actions/uiFlagsActions';
import './toolbar.css';

import TopNav, {
  Logo,
  LogoText,
  Interactions,
  ProfileAction,
  NavAction,
  Separator
} from '@hig/top-nav';


import styled from 'styled-components';
import { Service24 } from "@hig/icons";

const PlaceCenterContainer = styled.div`
  align-items: center;
  display: flex;
`;

export class Toolbar extends Component {
  componentDidMount() {
    this.props.loadProfile();
  }

  onLoginFailedCloseClick() {
    this.props.showLoginFailed(false);
    this.logout();
  }

  logout() {
    const logoutFrame = document.getElementById('hiddenLogoutFrame');
    logoutFrame.onload = () => {
        logoutFrame.removeEventListener('load', this, false);
        window.location.reload(false);
    };
    logoutFrame.src = 'https://accounts.autodesk.com/Authentication/LogOut';
  }

  render() {
    return (
      <div>
        <TopNav
          logo={
            <Logo link="https://forge.autodesk.com" label="Autodesk HIG">
              <PlaceCenterContainer>
                <img src={"logo.png"} alt="" />
                <LogoText>
                    <Typography style={{ fontSize: '16px'}}>AUTODESK<sup>®</sup> <strong>FORGE</strong></Typography>
                </LogoText>
              </PlaceCenterContainer>
            </Logo>
          }
          rightActions={
            <React.Fragment>
              <PlaceCenterContainer>
                {this.props.children}
              </PlaceCenterContainer>
              <Interactions>
                <Separator />
                <NavAction title="Log" icon={<Service24 />}>
                  <div>
                    <h3>Navigation Action</h3>
                    <p>
                      You can put what ever you want in here. You can also change
                      the icon and the title of the button.
                    </p>
                  </div>
                </NavAction>
                <span id="ProfileActionHolder">
                <ProfileAction avatarName={this.props.profile.name} avatarImage={this.props.profile.avatarUrl}>
                  <UserDetails profile={this.props.profile} logout={this.logout} />
                </ProfileAction>
                </span>
              </Interactions>
            </React.Fragment>
          }
        />
        {this.props.loginFailedShowing &&
          <ModalFail
              open={this.props.loginFailedShowing}
              title="Login Failed"
              label="Access to this site is by invitation only. Your email is not authorized."
              onClose={() => this.onLoginFailedCloseClick()}/>
        }
        {/* Use a hidden iframe to make a logout request to autodesk since it isn't supported in a better way yet */}
        <iframe id="hiddenLogoutFrame" />
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    profile: getProfile(store),
    loginFailedShowing: loginFailedShowing(store)
  };
}, { loadProfile, showLoginFailed })(Toolbar);
