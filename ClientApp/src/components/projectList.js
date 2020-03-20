import React, { Component } from 'react';
import {connect} from 'react-redux';

/** Dummy class to display project list */
class ProjectList extends Component {

    render() {
  
      const projects = this.props.projectList;
      const infos = this.props.notifications;
  
      if (! projects) {
  
        return (<span>No projects loaded</span>)
      } else {
  
        console.log(projects);
  
        return (
          <div>
            <ul>
              {
                projects.map((project) => (<li key={project.id}>{project.label}</li>))
              }
            </ul>
            {
              infos.map((info) => (<div>{info}</div>))
            }
          </div>
        )
      }
    }
}
  
ProjectList = connect(function (store){
    return {
      projectList: store.projectList,
      notifications: store.notifications
    }
  })(ProjectList);
  
export default ProjectList;