import React, { Component } from 'react';
import Surface from '@hig/surface';
import Tabs, { Tab } from "@hig/tabs";
import './app.css' 
import './tabs.css'
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
        <div className="tabsContainer">
          <Tabs
            align="center"
            showTabDivider={false}
            onTabChange={() => {}}
            onTabClose={() => {}}
          >
            <Tab label="Projects">
              <div id="project-list" className="tabContent">
                <ProjectList/>
              </div>
            </Tab>
            <Tab label="Model">
              <div id="project-list" className="tabContent">
                Model content
              </div>
            </Tab>
            <Tab label="BOM">
              <div id="project-list" className="tabContent">
                BOM content
              </div>
            </Tab>
            <Tab label="Drawing">
              <div id="project-list" className="tabContent">
                Drawing content
              </div>
            </Tab>
            <Tab label="Downloads">
              <div id="project-list" className="tabContent">
                Downloads content
              </div>
            </Tab>
          </Tabs>
        </div>      
      </Surface>
    );
  }
}

export default App;