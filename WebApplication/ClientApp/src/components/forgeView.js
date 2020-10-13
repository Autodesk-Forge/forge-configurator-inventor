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
import Script from 'react-load-script';
import {connect} from 'react-redux';
import { getActiveProject } from '../reducers/mainReducer';
import './forgeView.css';
import Message from './message';
import repo from '../Repository';
import { viewerCss, viewerJs } from './shared';

let Autodesk = null;

export class ForgeView extends Component {

    constructor(props){
      super(props);

      this.viewerDiv = React.createRef();
      this.viewer = null;
    }

    handleScriptLoad() {

        const options = repo.hasAccessToken() ?
                            { accessToken: repo.getAccessToken() } :
                            { env: 'Local' };

        Autodesk = window.Autodesk;

        const container = this.viewerDiv.current;
        this.viewer = new Autodesk.Viewing.GuiViewer3D(container);

        // uncomment this for Viewer debugging
        //this.viewer.debugEvents(true);

        Autodesk.Viewing.Initializer(options, this.handleViewerInit.bind(this));
    }

    handleViewerInit() {
        const errorCode = this.viewer.start();
        if (errorCode)
            return;

        // orient camera in the same way as it's on the thumbnail
        // corresponding to ViewOrientationTypeEnum.kIsoTopRightViewOrientation
        const viewer = this.viewer;
        this.viewer.addEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT, (event) => {

            const viewCubeExtensionId = "Autodesk.ViewCubeUi";

            // this is not perfect, because the view transition is visible, so it's a place to improve someday
            if (event.extensionId === viewCubeExtensionId) {

                const viewCubeUI = event.target.getExtension(viewCubeExtensionId);
                viewCubeUI.setViewCube("front top right");

                viewer.removeEventListener(Autodesk.Viewing.EXTENSION_LOADED_EVENT);
            }
        });

        // skip loading of svf when there is no active project svf
        if (!this.props.activeProject.svf)
            return;

        Autodesk.Viewing.Document.load(
            this.getSvfUrl(), this.onDocumentLoadSuccess.bind(this), () => {}
        );
    }

    componentDidUpdate(prevProps) {
        if (Autodesk && (this.props.activeProject.svf !== prevProps.activeProject.svf)) {
            Autodesk.Viewing.Document.load(
                this.getSvfUrl(), this.onDocumentLoadSuccess.bind(this), () => {}
            );
        }
    }

    componentWillUnmount() {
        if (this.viewer) {
            this.viewer.finish();
            this.viewer = null;
            Autodesk.Viewing.shutdown();
        }
    }

    getSvfUrl() {
        return this.props.activeProject.svf + `/bubble.json`;
    }

    onDocumentLoadSuccess(viewerDocument) {
        const defaultModel = viewerDocument.getRoot().getDefaultGeometry();
        this.viewer.loadDocumentNode(viewerDocument, defaultModel);
    }

    render() {
        return (
            <div className="modelContainer fullheight">
                <Message/>
                <div className="viewer" id="ForgeViewer">
                    <div ref={this.viewerDiv}></div>
                    <link rel="stylesheet" type="text/css" href={ viewerCss } />
                    <Script url={ viewerJs } onLoad={this.handleScriptLoad.bind(this)} />
                </div>
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
      activeProject: getActiveProject(store)
    };
  })(ForgeView);