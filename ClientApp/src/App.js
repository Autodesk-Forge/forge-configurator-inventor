import React, { Component } from 'react';
import {connect} from 'react-redux';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css' 
import Toolbar from './toolbar';
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