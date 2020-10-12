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
import { getDrawingPdfUrl } from '../reducers/mainReducer';
import repo from '../Repository';
import './forgePdfView.css';

let Autodesk = null;

export class ForgePdfView extends Component {

    constructor(props){
      super(props);

      this.viewerDiv = React.createRef();
      this.viewer = null;
    }

    async handleScriptLoad() {

        const options = repo.hasAccessToken() ?
                            { accessToken: repo.getAccessToken() } :
                            { env: 'Local' };

        Autodesk = window.Autodesk;

        try {
            await import('./forgePdfViewExtension');
        } catch (error) {
            // TODO unit test is crashing here, verify if some mock resolves it
        }

        const container = this.viewerDiv.current;
        this.viewer = new Autodesk.Viewing.GuiViewer3D(container,
            { extensions: ['ForgePdfViewExtension'],
              // these options (enableBrowserNavigation) are used when switching PDF sheets
              enableBrowserNavigation: false });

        // uncomment this for Viewer debugging
        //this.viewer.debugEvents(true);

        Autodesk.Viewing.Initializer(options, this.handleViewerInit.bind(this));
    }

    handleViewerInit() {
        const errorCode = this.viewer.start();
        if (errorCode)
            return;

        // these options (enableBrowserNavigation) are used when switching TAB (creating pdf view)
        this.viewer.loadExtension('Autodesk.PDF', { enableBrowserNavigation: false });

        // skip loading of svf when here is no active project drawingPdf
        if (!this.props.drawingPdf)
            return;

        this.viewer.loadModel( this.props.drawingPdf, { page: 1 } ); // load page 1 by default
    }

    componentDidUpdate(prevProps) {
        if (this.viewer && Autodesk && (this.props.drawingPdf !== prevProps.drawingPdf)) {

            const findModelForUrn = function(viewer, urn) {
                const allModels = viewer.getAllModels();
                let modelForUrn = null;
                for (const model of allModels) {
                    const modelUrn = model.getData().urn;
                    if (modelUrn != urn)
                        continue;

                        modelForUrn = model;
                    break;
                }

                return modelForUrn;
            };

            if (prevProps.drawingPdf != null) {
                // try to find model in viewer.allModels and hide it
                const modelToHide = findModelForUrn(this.viewer, prevProps.drawingPdf);
                if (modelToHide != null)
                    this.viewer.hideModel(modelToHide);
            }

            if (this.props.drawingPdf != null) {
                // try to find model of specific urn and show it or load
                const modelToShow = findModelForUrn(this.viewer, this.props.drawingPdf);
                if (modelToShow != null) {
                    this.viewer.showModel(modelToShow);
                } else {
                    this.viewer.loadModel( this.props.drawingPdf, { page: 1 }); // load page 1 by default
                }
            }
        }
    }

    componentWillUnmount() {
        if (this.viewer) {
            this.viewer.finish();
            this.viewer = null;
            Autodesk.Viewing.shutdown();
        }
    }

    render() {
        return (
            <div className="viewer" id="ForgePdfViewer">
                <div ref={this.viewerDiv}></div>
                <link rel="stylesheet" type="text/css" href={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/style.css`}/>
                <Script url={`https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/viewer3D.js`}
                    onLoad={this.handleScriptLoad.bind(this)}
                />
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store){
    return {
        drawingPdf: getDrawingPdfUrl(store)
    };
  })(ForgePdfView);