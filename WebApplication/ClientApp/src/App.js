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
      <Surface className="fullheight" id="main" level={200}
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

export default connect(null, {
  fetchShowParametersChanged, detectToken
})(App);

