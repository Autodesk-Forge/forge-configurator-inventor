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

export class App extends Component {
  constructor(props) {
    super(props);
    props.detectToken();
  }
  componentDidMount() {
    this.props.fetchShowParametersChanged();
  }
  render () {
    return (
      <Surface className="fullheight" id="main" level={200}>
        <Toolbar>
          <ProjectSwitcher />
        </Toolbar>
        <TabsContainer/>
      </Surface>
    );
  }
}

export default connect(null, {
  fetchShowParametersChanged, detectToken
})(App);

