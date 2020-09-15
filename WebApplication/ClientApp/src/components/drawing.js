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
import { getActiveProject, getDrawingPdfUrl, drawingProgressShowing } from '../reducers/mainReducer';
import './drawing.css';
import ForgePdfView from './forgePdfView';
import { fetchDrawing } from '../actions/downloadActions';
import ModalProgress from './modalProgress';
import { showDrawingExportProgress } from '../actions/uiFlagsActions';
import DrawingsContainer from './drawingsContainer';

export class Drawing extends Component {

  componentDidMount() {
    const isAssembly = this.props.activeProject?.isAssembly;
    const hasDrawing = this.props.activeProject?.hasDrawing;
    if (isAssembly === true && hasDrawing === true && this.props.drawingPdf === null)
      this.props.fetchDrawing(this.props.activeProject);
  }

  componentDidUpdate(prevProps) {
    // refresh drawing data when Drawing tab was clicked before projects initialized
    const isAssembly = this.props.activeProject?.isAssembly;
    const hasDrawing = this.props.activeProject?.hasDrawing;
    if (isAssembly === true && hasDrawing === true && this.props.activeProject !== prevProps.activeProject) {
          if (this.props.drawingPdf === null)
            this.props.fetchDrawing(this.props.activeProject);
      }
  }

  onModalProgressClose() {
    this.props.hideModalProgress();
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
          // <div id="drawings" className='drawingsContent fullheight'>
            <div className='inRow fullheight'>
              <DrawingsContainer/>
              {/* <ForgePdfView/> */}
            </div>
          // </div>
        }
        {!empty && this.props.drawingProgressShowing &&
          <ModalProgress
              open={true}
              title="Generating Drawing"
              label={this.props.activeProject.id}
              icon="/Assembly_icon.svg"
              onClose={() => this.onModalProgressClose()}
          />
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
}, { fetchDrawing, hideModalProgress: () => async (dispatch) => { dispatch(showDrawingExportProgress(false)); } })(Drawing);
