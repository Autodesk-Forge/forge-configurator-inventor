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
import { fetchShowParametersChanged } from './actions/uiFlagsActions';
import { detectToken } from './actions/profileActions';
import ModalProgress from './components/modalProgress';
import { adoptWithParamsProgressShowing } from './reducers/mainReducer';
import { adoptProjectWithParameters } from './actions/adoptWithParamsActions';

export class App extends Component {
  constructor(props) {
    super(props);
    props.detectToken();
  }
  componentDidMount() {
    this.props.fetchShowParametersChanged();

    const rawParams = window.location.search.substring(1);
    if (rawParams !== '') {
      const params = JSON.parse('{"' + rawParams.replace(/&/g, '","').replace(/=/g,'":"') + '"}', function(key, value) { return key===""?value:decodeURIComponent(value);});

      if (params.url) {
        this.props.adoptProjectWithParameters(params.url);
      }
    }
  }

  render () {
    return (
      <Surface className="fullheight" id="main" level={200}>
        <Toolbar>
          <ProjectSwitcher />
        </Toolbar>
        <TabsContainer/>
        {this.props.adoptWithParamsProgressShowing &&
          <ModalProgress
              open={true}
              title="Adopting Project"
              label=" "
              icon="/Assembly_icon.svg"
              onClose={() => this.onModalProgressClose()}/>
        }
      </Surface>
    );
  }
}

export default connect(function (store) {
  return {
    adoptWithParamsProgressShowing: adoptWithParamsProgressShowing(store)
  };}, {
    adoptProjectWithParameters, fetchShowParametersChanged, detectToken
})(App);

