import React, { Component } from 'react';
import Surface from '@hig/surface';
import './app.css' 
import Toolbar from './components/toolbar';
import TabsContainer from './components/tabsContainer';

class App extends Component {
  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Toolbar/>
        <TabsContainer/>      
      </Surface>
    );
  }
}

export default App;