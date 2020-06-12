import React, { Component } from 'react';
import Tabs, { Tab } from "@hig/tabs";
import ProjectList from './projectList';
import ForgeView from './forgeView';
import ParametersContainer from './parametersContainer';
import PageNYI from './pageNYI';
import Downloads from './downloads';
import './tabs.css';

class TabsContainer extends Component {

    render() {
        return (
            <div className="tabsContainer">
            <Tabs
              className="fullheight"
              align="center"
              showTabDivider={false}
              onTabChange={() => {}}
              onTabClose={() => {}}
            >
              <Tab label="Projects">
                <div id="project-list" className="tabContent fullheight">
                  <ProjectList/>
                </div>
              </Tab>
              <Tab label="Model" >
                <div id="model" className='tabContent fullheight'>
                  <div className='inRow fullheight'>
                    <ParametersContainer/>
                    <ForgeView/>
                  </div>
                </div>
              </Tab>
              <Tab label="BOM">
                <div id="bom" className="tabContent fullheight">
                  <PageNYI/>
                </div>
              </Tab>
              <Tab label="Drawing">
                <div id="drawing" className="tabContent fullheight">
                  <PageNYI/>
                </div>
              </Tab>
              <Tab label="Downloads">
                <div id="downloads" className="tabContent fullheight">
                  <Downloads/>
                </div>
              </Tab>
            </Tabs>
          </div>
        );
    }
}

export default TabsContainer;