import React, { Component } from 'react';
import { connect } from 'react-redux';
import { loadProfile } from '../actions/profileActions';
import { getProfile } from '../reducers/mainReducer';
import UserDetails from './userDetails.js';

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
  place-items: center;
  display: flex;
`;

export class Toolbar extends Component {
  componentDidMount() {
    this.props.loadProfile();
  }
  render() {
    return (
      <TopNav
        logo={
          <Logo link="https://forge.autodesk.com" label="Autodesk HIG">
            <PlaceCenterContainer>
              <img src={"logo.png"} alt="" />
              <LogoText>
                AUTODESK<sup>®</sup> <strong>FORGE</strong>
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
              <ProfileAction avatarName={this.props.profile.name} avatarImage={this.props.profile.avatarUrl}>
                <UserDetails profile={this.props.profile} />
              </ProfileAction>
            </Interactions>
          </React.Fragment>
        }
      />
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  return {
    profile: getProfile(store)
  };
}, { loadProfile })(Toolbar);
