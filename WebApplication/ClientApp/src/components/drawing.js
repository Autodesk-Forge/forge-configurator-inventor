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
import { getActiveProject } from '../reducers/mainReducer';
import './drawing.css';
import ForgePdfView from './forgePdfView';
import { fetchDrawing } from '../actions/downloadActions';
import { getDrawingPdfUrl } from '../reducers/mainReducer';
import { drawingProgressShowing } from '../reducers/mainReducer';
import ModalProgress from './modalProgress';

export class Drawing extends Component {

  componentDidMount() {
    const isAssembly = this.props.activeProject?.isAssembly;
    const hasDrawing = this.props.activeProject?.hasDrawing;
    if (isAssembly === true && hasDrawing === true && this.props.drawingPdf === null)
      this.props.fetchDrawing(this.props.activeProject);
  }

  componentDidUpdate(prevProps) {
    // refresh bom data when Drawing tab was clicked before projects initialized
    const isAssembly = this.props.activeProject?.isAssembly;
    const hasDrawing = this.props.activeProject?.hasDrawing;
    if (isAssembly === true && hasDrawing === true && this.props.activeProject !== prevProps.activeProject) {
          if (this.props.drawingPdf === null)
            this.props.fetchDrawing(this.props.activeProject);
      }
  }

  render() {
    const initialized = !this.props.activeProject?.hasDrawing || this.props.drawingPdf !== null;
    const isAssembly = this.props.activeProject?.isAssembly;
    // for now, we don't detect hasDrawing on the server, but return empty url in case there is no drawing. So keep the check for drawingPdf?.length
    const hasDrawing = this.props.activeProject?.hasDrawing && this.props.drawingPdf?.length > 0;
    const empty = (initialized && !hasDrawing) || isAssembly === false;
    const containerClass = empty ? "drawingContainer empty" : "drawingContainer";

    return (
      <div className="fullheight">
        <div className={containerClass}>
        {empty &&
          <div className="drawingEmptyText">You don&apos;t have any drawings in package.</div>
        }
        {!empty &&
          <ForgePdfView/>
        }
        {this.props.drawingProgressShowing &&
        <ModalProgress
            open={true}
            title="Generating Drawing"
            label={this.props.activeProject.id}
            icon="/Assembly_icon.svg"
            onClose={() => {}}/>
        }
        </div>
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  const activeProject = getActiveProject(store);
  return {
    activeProject: activeProject,
    drawingPdfUrl: getDrawingPdfUrl(store),
    drawingPdf: getDrawingPdfUrl(store),
    drawingProgressShowing: drawingProgressShowing(store)
  };
}, { fetchDrawing })(Drawing);
