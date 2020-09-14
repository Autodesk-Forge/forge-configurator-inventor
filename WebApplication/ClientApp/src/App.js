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
import Surface from '@hig/surface';
import './app.css';
import Toolbar from './components/toolbar';
import TabsContainer from './components/tabsContainer';
import ProjectSwitcher from './components/projectSwitcher';
import { showAdoptWithParamsFailed, setEnableEmbededMode, fetchShowParametersChanged } from './actions/uiFlagsActions';
import { detectToken } from './actions/profileActions';
import ModalProgress from './components/modalProgress';
import { adoptWithParamsFailed, embededModeEnabled, adoptWithParamsProgressShowing } from './reducers/mainReducer';
import { adoptProjectWithParameters } from './actions/adoptWithParamsActions';
import ModalFail from './components/modalFail';

export class App extends Component {
  constructor(props) {
    super(props);
    props.detectToken();
  }
  componentDidMount() {
    this.props.fetchShowParametersChanged();

    /* we are looking for url parameter that points to configuration JSON file for project adoption / project update
       the expected format should look like: ?url=www.mydata.com/jsonConfig

       the configuration JSON consists of:
       "Url": url to your project zip
       "Name": unique project name
       "TopLevelAssembly": example.iam
       "Config": desired parameters for adoption/update
    */
    const rawParams = window.location.search.substring(1);
    let pocEnabled = false;
    if (rawParams !== '') {
      const params = JSON.parse('{"' + rawParams.replace(/&/g, '","').replace(/=/g,'":"') + '"}', function(key, value) { return key===""?value:decodeURIComponent(value);});

      if (params.url) {
        pocEnabled = true;
        this.props.adoptProjectWithParameters(params.url);
      }
    }
    // ugly way to do this, just for POC demo
    this.props.setEnableEmbededMode(pocEnabled);
  }

  render () {
    const showToolbar = this.props.embededModeEnabled;

    return (
      <Surface className="fullheight" id="main" level={200}>
        { !showToolbar &&
          <Toolbar>
            <ProjectSwitcher />
          </Toolbar>
        }
        <TabsContainer/>
        {this.props.adoptWithParamsProgressShowing &&
          <ModalProgress
              open={true}
              title="Loading Content"
              label=" "
              icon="/Assembly_icon.svg"/>
        }
        {this.props.adoptWithParamsFailed &&
          <ModalFail
              open={true}
              title={ "Content loading failed" }
              contentName=""
              label="See console for more details"
              onClose={ () => this.props.showAdoptWithParamsFailed(false) } />}
          </Surface>
    );
  }
}

export default connect(function (store) {
  return {
    adoptWithParamsProgressShowing: adoptWithParamsProgressShowing(store),
    adoptWithParamsFailed: adoptWithParamsFailed(store),
    embededModeEnabled: embededModeEnabled(store)
  };}, {
    showAdoptWithParamsFailed, adoptProjectWithParameters, setEnableEmbededMode, fetchShowParametersChanged, detectToken
})(App);

