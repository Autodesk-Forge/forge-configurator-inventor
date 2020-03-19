import React, { Component } from 'react';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css'
import { getRepo } from './Repository';

/** Dummy class to display project list */
export class ProjectList extends Component {

  render() {

    const projects = this.props.projects;

    if (! projects) {

      return (<span>No projects loaded</span>)
    } else {

      console.log(projects);

      return (
        <ul>
          {
            projects.map((project, index) => {
              return (<li key={index}>{project.name}</li>)
            })
          }
        </ul>
      )
    }
  }
}

export default class App extends Component {

  constructor(props) {
    super(props);

    this.state = {
      projects: null
    };
  }

  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Button
          size="standard"
          title="I am Autodesk HIG button and I can load projects"
          type="primary"
          onClick={ async () => {

            // load projects data
            const projects = await getRepo().loadProjects();
            this.setState( { projects: projects });
          } }
        />
        <div id="project-list">
          <ProjectList projects={ this.state.projects } />
        </div>
      </Surface>
    );
  }
}
