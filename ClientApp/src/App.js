import React, { Component } from 'react';

import Surface from '@hig/surface';
import Button from '@hig/button';
import './app.css' 

export default class App extends Component {

  constructor(props) {
    super(props);
    this.state = {
      isPrimary: true
    };
  }

  render () {
    return (
      <Surface id="main" level={200}
        horizontalPadding="m"
        verticalPadding="m"
        shadow="high"
        borderRadius="m">
        <Button
          size="standard"
          title={ this.state.isPrimary ? "I am Autodesk HIG button and I am doing nothing" : "Oops"}
          type={ this.state.isPrimary ? "primary" : "secondary"}
          onClick={ () => this.setState({ isPrimary: !this.state.isPrimary }) }
        />
      </Surface>
    );
  }
}
