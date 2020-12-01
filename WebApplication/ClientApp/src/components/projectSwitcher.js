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
import {connect} from 'react-redux';
import ProjectAccountSwitcher from '@hig/project-account-switcher';
import { fetchProjects, updateActiveProject } from '../actions/projectListActions';
import { updateActiveTabIndex, invalidateDrawing } from '../actions/uiFlagsActions';
import { fetchParameters } from '../actions/parametersActions';
import {addLog} from '../actions/notificationActions';
import './projectSwitcher.css';

export class ProjectSwitcher extends Component {

    constructor(props) {
        super(props);
        this.onProjectClick = this.onProjectClick.bind(this);
    }

    componentDidMount() {
        this.props.fetchProjects();
    }

    onProjectClick(event) {
        const id = event.target.innerHTML;
        this.props.updateActiveProject(id);
        // mark drawing as not valid if any available
        this.props.invalidateDrawing();
        // switch to the model tab
        this.props.updateActiveTabIndex(1);
        //this.props.fetchParameters(id);
        this.props.addLog('Selected: ' + id);
    }

    render() {
        return (
            <span id="PAS">
            <ProjectAccountSwitcher
                defaultProject={null}
                activeProject={this.props.projectList.activeProjectId}
                projects={this.props.projectList.projects}
                projectTitle="Projects"
                onClick={this.onProjectClick}
            />
            </span>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      projectList: store.projectList
    };
  }, { fetchProjects, fetchParameters, updateActiveProject, updateActiveTabIndex, addLog, invalidateDrawing } )(ProjectSwitcher);
