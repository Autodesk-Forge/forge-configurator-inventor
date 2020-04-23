import React, { Component } from 'react';
import Tabs, { Tab } from "@hig/tabs";
import ProjectList from './projectList';
import {connect} from 'react-redux';  
import ForgeViewer from 'react-forge-viewer';
import ViewerHadlers from '../actions/viewerActions'
import './tabs.css'

export class TabsContainer extends Component {

    constructor(props){
      super(props);

      this.state = {
        view:null
      }
    }

    render() {
        var currentProject = this.props.projectList.projects.find(proj => proj.id === this.props.projectList.activeProjectId);
        var viewableUrn = `/data/${currentProject.currentHash}/svf/bubble.json`;

        return (
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
                <div id="model" className="tabContent">
                  <ForgeViewer
                    // version="6.0"
                    urn={viewableUrn}
                    view={this.state.view}
                    headless={false}
                    onViewerError={ViewerHadlers.handleViewerError.bind(this)}
                    onTokenRequest={ViewerHadlers.handleTokenRequested.bind(this)}
                    onDocumentLoad={ViewerHadlers.handleDocumentLoaded.bind(this)}
                    onDocumentError={ViewerHadlers.handleDocumentError.bind(this)}
                    onModelLoad={ViewerHadlers.handleModelLoaded.bind(this)}
                    onModelError={ViewerHadlers.handleModelError.bind(this)}
                  />
                </div>
              </Tab>
              <Tab label="BOM">
                <div id="bom" className="tabContent">
                  BOM content
                </div>
              </Tab>
              <Tab label="Drawing">
                <div id="drawing" className="tabContent">
                  Drawing content
                </div>
              </Tab>
              <Tab label="Downloads">
                <div id="downloads" className="tabContent">
                  Downloads content
                </div>
              </Tab>
            </Tabs>
          </div>
        )
    }
}

export default connect(function (store){
  return {
    projectList: store.projectList
  }
})(TabsContainer);
