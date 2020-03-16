import React, { Component } from 'react';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css' 

export default class App extends Component {

  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Button
          size="standard"
          title="I am Autodesk HIG button and I am doing nothing"
          type="primary"
        />
      </Surface>
    );
  }
}
