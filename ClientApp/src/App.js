import React, { Component } from 'react';
import axios from 'axios';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css'

export class ProjectList extends Component {

  render() {

    const projects = this.props.projects;

    console.log(projects);

    if (! projects) {
      return (<div>Loading...</div>)
    } else {

      return (
        <ul>
          {
            projects.map(project => {
              return (<li>{project.name}</li>)
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
          onClick={ () => {
            this.setState({ isPrimary: !this.state.isPrimary });

            axios("/Project")
              .then(response => { 

                console.log(response);
                this.setState( Object.assign({}, this.state, { projects: response.data } ) );
              })
              .catch(e => console.log(`Project loading error: ${e}`));
          } }
        />
        <ProjectList projects={ this.state.projects } />
      </Surface>
    );
  }
}
