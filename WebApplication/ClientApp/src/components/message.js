import React, { Component } from 'react';
import { connect } from 'react-redux';

import { getActiveProject, showUpdateNotification} from '../reducers/mainReducer';
import { dismissUpdateMessage } from '../actions/dismissUpdateMessageActions';

import './message.css';
import Banner from '@hig/banner';
import Button from '@hig/button';
import Checkbox from '@hig/checkbox';

import ThemeContext from '@hig/theme-context';
import HIGHighDensityTheme from '@hig/theme-data/build/esm/webLightHighDensityTheme';

import styled from 'styled-components';

const Fixed = styled('div')`
  position: fixed;
  z-index: 100;
  width: calc(100% - 240px);
`;

export class Message extends Component {

    constructor(props) {
        super(props);
        this.onDismiss = this.onDismiss.bind(this);
    }

    onDismiss() {
        this.props.dismissUpdateMessage(this.props.activeProject.id);
    }

    render() {
        const visible = this.props.showUpdateNotification;

        return (
          <ThemeContext.Provider value={HIGHighDensityTheme}>
          <Fixed>
            <Banner
            type="primary"
            actions={({ isWrappingActions }) => (
                <Banner.Interactions isWrappingActions={isWrappingActions}>
                  <Banner.Action>
                    <Button
                      type="secondary"
                      size="small"
                      width={isWrappingActions ? "grow" : "shrink"}
                      title="Close"
                      onClick={this.onDismiss}
                    />
                  </Banner.Action>
                  <div className="verticalseparator"/>
                  <Banner.Action>
                    <Checkbox/>
                    <ThemeContext.Consumer>{({ resolvedRoles }) => (
                      <div style={{
                          fontFamily: resolvedRoles["basics.fontFamilies.main"],
                          fontSize: resolvedRoles["basics.fontSizes.highMedium"],
                          marginLeft: '12px'
                      }}>Don&apos;t show again.
                      </div>
                  )}</ThemeContext.Consumer>
                  </Banner.Action>
                  <div className="verticalseparator"/>
                </Banner.Interactions>
              )}
            onDismiss={this.onDismiss}
            isVisible={visible}
            >
            The assembly is out-of-date. Click Update to display the most actual state.
            </Banner>
            </Fixed>
            </ThemeContext.Provider>
            );
    }
}

export default connect(function (store) {
    return {
        activeProject: getActiveProject(store),
        showUpdateNotification: showUpdateNotification(store)
    };
}, { dismissUpdateMessage })(Message);