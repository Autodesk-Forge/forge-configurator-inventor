import React, { Component } from 'react';
import {connect} from 'react-redux';
import ProjectAccountSwitcher from '@hig/project-account-switcher';
import { fetchProjects, updateActiveProject } from '../actions/projectListActions';
import { fetchParameters } from '../actions/parametersActions';
import {addLog} from '../actions/notificationActions';

export class ProjectSwitcher extends Component {

    constructor(props) {
        super(props);
        this.onProjectChange = this.onProjectChange.bind(this);
    }

    componentDidMount() {
        this.props.fetchProjects();
    }

    onProjectChange(data) {
        const id = data.project.id;
        this.props.updateActiveProject(id);
        this.props.fetchParameters(id);
        this.props.addLog('Selected: ' + id);
    }

    render() {
        return (
            <ProjectAccountSwitcher
                defaultProject={this.props.projectList.activeProjectId}
                activeProject={null}
                projects={this.props.projectList.projects}
                projectTitle="Projects"
                onChange={this.onProjectChange}
            />
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      projectList: store.projectList
    };
  }, { fetchProjects, fetchParameters, updateActiveProject, addLog } )(ProjectSwitcher);
