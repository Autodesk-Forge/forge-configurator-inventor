/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

import React, { Component } from 'react';
import { connect } from 'react-redux';
import Tabs, { Tab } from "@hig/tabs";
import ProjectList from './projectList';
import ForgeView from './forgeView';
import ParametersContainer from './parametersContainer';
import PageNYI from './pageNYI';
import Downloads from './downloads';
import './tabs.css';
import { activeTabIndex } from '../reducers/mainReducer';
import { updateActiveTabIndex } from '../actions/uiFlagsActions';

export class TabsContainer extends Component {

    onTabChange(index) {
      this.props.updateActiveTabIndex(index);
    }

    render() {

        const idx = this.props.activeTabIndex;

        return (
            <div className="tabsContainer">
            <Tabs
              className="fullheight"
              align="center"
              showTabDivider={false}
              onTabChange={(index) => { this.onTabChange(index); }}
              onTabClose={() => {}}
              activeTabIndex={idx}
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

/* istanbul ignore next */
export default connect(function (store){
  return {
    activeTabIndex: activeTabIndex(store)
  };
}, { updateActiveTabIndex } )(TabsContainer);