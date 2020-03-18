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
      return (<div>No projects loaded</div>)
    } else {

      //console.log(projects);

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
      isPrimary: true,
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
          title={ this.state.isPrimary ? "I am Autodesk HIG button and I am doing nothing" : "Oops"}
          type={ this.state.isPrimary ? "primary" : "secondary"}
          onClick={ async () => {
            this.setState({ isPrimary: ! this.state.isPrimary });

            // load projects data
            const projects = await getRepo().loadProjects();
            this.setState( Object.assign({}, this.state, { projects: projects } ) );
          } }
        />
        <ProjectList projects={ this.state.projects } />
      </Surface>
    );
  }
}
