import React, { Component } from 'react';
import {connect} from 'react-redux';
import PropTypes from 'prop-types';

/** Dummy class to display project list */
class ProjectList extends Component {
    render() {

      const projects = this.props.projectList.projects;
      const infos = this.props.notifications;

      if (! projects) {

        return (<span>No projects loaded</span>);
      } else {

        return (
          <div>
            <ul>
              {
                projects.map((project) => (<li key={project.id}>{project.label}</li>))
              }
            </ul>
            {
              infos.map((info, index) => (<div key={index}>{info}</div>))
            }
          </div>
        );
      }
    }
}

ProjectList.propTypes = {
  projectList: PropTypes.array,
  notifications: PropTypes.array
};

export default connect(function (store) {
  return {
    projectList: store.projectList,
    notifications: store.notifications
  };
})(ProjectList);