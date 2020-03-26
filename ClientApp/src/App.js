import React, { Component } from 'react';
import Surface from '@hig/surface';
import './app.css' 
import Toolbar from './components/toolbar';
import ProjectList from './components/projectList';

class App extends Component {
  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Toolbar/>
        <div id="project-list">
          <ProjectList/>
        </div>
      </Surface>
    );
  }
}

export default App;