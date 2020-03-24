import React, { Component } from 'react';
import {connect} from 'react-redux';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css' 
import Toolbar from './toolbar';
import ProjectList from './components/projectList';

import {updateProjectList} from './actions/projectListActions';
import {addError, addLog} from './actions/notificationActions';

import repo from './Repository';

const thunkedLoadProjects = () => async (dispatch, getState) => {
    dispatch(addLog('Load Projects invoked'));
    try {
        const data = await repo.loadProjects();
        dispatch(addLog('Load Projects received'));
        dispatch(updateProjectList(data));
    } catch (error) {
        dispatch(addError('Failed to get Project list.'));
    }
}

class App extends Component {
  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Button
          size="standard"
          title="I am Autodesk HIG button (AwsPipeline) and I can load projects"
          type="primary"
          onClick={ () => {
            this.props.thunkedLoadProjects();
          } }
        />
        <Toolbar/>
        <div id="project-list">
          <ProjectList/>
        </div>
      </Surface>
    );
  }
}

App = connect(function (store){ return {} }, { thunkedLoadProjects })(App);

export default App;