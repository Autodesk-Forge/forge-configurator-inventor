import React, { Component } from 'react';

import TopNav, {
    Logo,
    LogoText,
    Interactions,
    ProfileAction,
    NavAction,
    Separator
  } from '@hig/top-nav';


import styled from 'styled-components';
import {Service24} from "@hig/icons";

const PlaceCenterContainer = styled.div`
  place-items: center;
  display: flex;
`;

export default class Toolbar extends Component {

  render () {
    return (
        <TopNav
          logo={
            <Logo link="https://forge.autodesk.com" label="Autodesk HIG">
              <PlaceCenterContainer>
                <img src={"logo.png"}/>
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
                <Separator/>
                <NavAction title="Log" icon={<Service24/>}>
                  <div>
                    <h3>Navigation Action</h3>
                    <p>
                      You can put what ever you want in here. You can also change
                      the icon and the title of the button.
                    </p>
                  </div>
                </NavAction>
                <ProfileAction avatarName='anonymous user'/>
              </Interactions>
            </React.Fragment>
          }
        />
        );
  }
}