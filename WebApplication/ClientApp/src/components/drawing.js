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

export class Drawing extends Component {

  render() {
    const hasDrawing = this.props.activeProject?.drawing!=null;
    const containerClass = !hasDrawing ? "drawingContainer empty" : "drawingContainer";

    return (
      <div className="fullheight">
        <div className={containerClass}>
        {!hasDrawing &&
          <div className="drawingEmptyText">You don&apos;t have any drawings in package.</div>
        }
        {hasDrawing &&
          <ForgePdfView/>
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
    activeProject: activeProject
  };
})(Drawing);
