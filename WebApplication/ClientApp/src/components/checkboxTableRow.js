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
import Checkbox from '@hig/checkbox';
import { checkedProjects } from '../reducers/mainReducer';

export class CheckboxTableRow extends Component {

  onChange(checked) {
    const { rowData } = this.props;
    this.props.onChange( checked, rowData );
  }

  render() {
    const { rowData, selectable } = this.props;
    const isChecked = this.props.checkedProjects.includes(rowData.id);

      return (
        <div id={isChecked ? "checkbox_checked_visible" : "checkbox_hover_visible"}>
          {selectable && <Checkbox id="checkbox_row"
            onChange={(checked) => {this.onChange(checked); }}
            checked={isChecked}
          />}
        </div>
        );
    }
  }

/* istanbul ignore next */
export default connect(function (store) {
  return {
    checkedProjects: checkedProjects(store)
  };
})(CheckboxTableRow);