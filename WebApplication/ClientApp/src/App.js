import React, { Component } from 'react';
import Surface from '@hig/surface';
import './app.css' 
import Toolbar from './components/toolbar';
import TabsContainer from './components/tabsContainer';
import ProjectSwitcher from './components/projectSwitcher';

class App extends Component {
  render () {
    return (
      <Surface id="main" level={200}
        shadow="high"
        borderRadius="m">
        <Toolbar>
          <ProjectSwitcher />
        </Toolbar>
        <TabsContainer/>      
      </Surface>
    );
  }
}

export default App;