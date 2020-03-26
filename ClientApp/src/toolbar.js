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
import {Service24} from "@hig/icons";
import repo from './Repository';

import {updateProjectList, updateActiveProject} from './actions/projectListActions';
import {addError, addLog} from './actions/notificationActions';

export const fetchProjects = () => async (dispatch, getState) => {
    dispatch(addLog('Load Projects invoked'));
    try {
        const data = await repo.loadProjects();
        dispatch(addLog('Load Projects received'));
        dispatch(updateProjectList(data));
      } catch (error) {
        dispatch(addError('Failed to get Project list. (' + error + ')'));
    }
}

const PlaceCenterContainer = styled.div`
  place-items: center;
  display: flex;
`;

class Toolbar extends Component {

  constructor(props) {
    super(props);
    this.onProjectChange = this.onProjectChange.bind(this);
  }

  componentDidMount() {
    this.props.fetchProjects(this);
  }

  onProjectChange(data)
  {
    const id = data.project.id;
    this.props.updateActiveProject(id);
    this.props.addLog('Selected: ' + id);
  }

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
                defaultProject={this.props.projectList.activeProjectId}
                activeProject={null}
                projects={this.props.projectList.projects}
                projectTitle="Projects"
                onChange={this.onProjectChange}
              />
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

export default Toolbar = connect(function (store){
  return {
    projectList: store.projectList
  }
}, { fetchProjects, updateActiveProject, addLog } )(Toolbar);
