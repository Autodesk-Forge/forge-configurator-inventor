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
import Typography from "@hig/typography";

export class HyperLink extends Component {

    componentDidMount() {
        if (this.props.onAutostart)
            this.props.onAutostart(this.downloadHyperlink);
    }

    render() {
        const downloadProps =  {};
        if(this.props.download) {
          downloadProps['download'] = '';
        }

        const downloadLink = <a href={this.props.href} onClick={(e) => {
            e.stopPropagation();
            if (this.props.onUrlClick) this.props.onUrlClick();
        }} ref = {(h) => {
            this.downloadHyperlink = h;
        }} {...downloadProps}>{this.props.link}</a>;

      return(
          <Typography style={{ width: 'fit-content'}}>
            {this.props.prefix}{downloadLink}{this.props.suffix}
          </Typography>
      );
    }
  }

  export default HyperLink;