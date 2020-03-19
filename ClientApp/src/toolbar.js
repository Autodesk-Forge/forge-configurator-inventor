import React, { Component } from 'react';
import {connect} from 'react-redux';

import TopNav, {
    Logo,
    LogoText,
    Interactions, 
    ProfileAction,
    NavAction,
    Separator
  } from '@hig/top-nav';

import ProjectAccountSwitcher from '@hig/project-account-switcher';
import styled from 'styled-components';

const PlaceCenterContainer = styled.div`
  place-items: center;
  display: flex;
`;

class Toolbar extends Component {

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
              <ProjectAccountSwitcher
                defaultProject="1"
                activeProject={null}
                projects={this.props.projectList}
                projectTitle="Projects"
              />
              </PlaceCenterContainer>
              <Interactions>
                <Separator/>
                <NavAction title="Log" icon={<img src="log.png"/>}>
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

Toolbar = connect(function (store){
  return {
    projectList: store.projectList
  }
})(Toolbar);

export default Toolbar;
