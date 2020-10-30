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
import 'react-base-table/styles.css';
import './bom.css';

import { Alert24 } from "@hig/icons";

import { getActiveProject, getBom } from '../reducers/mainReducer';
import { fetchBom } from '../actions/bomActions';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import styled from 'styled-components';
import { getMaxColumnTextWidth } from './bomUtils';

const Container = styled.div`
  height: 100vh;
`;

export class Bom extends Component {

  componentDidMount() {
    this.props.fetchBom(this.props.activeProject);
  }

  componentDidUpdate(prevProps) {
      // refresh bom data when BOM tab was clicked before projects initialized
      if (this.props.activeProject !== prevProps.activeProject) {
          this.props.fetchBom(this.props.activeProject);
      }
  }

  render() {

    const bom = this.props.bomData;
    if (! bom?.columns?.length) {
      // if BOM is empty it's possible that the project has problems (like - IPT is missing).
      // So check for adoption warnings, and if any - show the warning message(s).
      const warnings = this.props.activeProject.adoptWarnings || [];
      const hasWarnings = this.props.activeProject.isAssembly ? (warnings.length > 0) : null;
      return <div className="fullheight">
        <div className="bomEmpty">
          <div className="title">
            <Alert24 />&nbsp;BOM is Empty
          </div>
          <div className="image"></div>
          {hasWarnings && <div className="details">
            Please check the following adoption {warnings.length === 1 ? 'message' : 'messages'}:
            <ul>
                { warnings.map(message => (<li key={message}>{message}</li>)) }
            </ul>
          </div>}
        </div>
      </div>;
    }

    let columns = [{ key: 'leftAlignColumn', width: 79, minWidth: 79}];
    let data = [];

    const columnWidths = [];
    // extract all strings from all columns to prepare columns widths
    const allStrings = [];
    bom.columns.forEach( (column, index) => {
      const columnStrings = [ column.label ];
      bom.data.forEach(row => {
        columnStrings.push(row[index]);
      });
      allStrings.push(columnStrings);
    });

    allStrings.forEach(columnStrings => {
      // compute column width
      columnWidths.push(getMaxColumnTextWidth(columnStrings) + 15 /*right+left padding*/);
    });

    // prepare columns
    columns = columns.concat(bom.columns.map( (column, columnIndex) => (
      {
        key: 'column-'+columnIndex,
        dataKey: 'column-'+columnIndex,
        title: column.label,
        align: column?.numeric ? Column.Alignment.RIGHT : Column.Alignment.LEFT,
        width: columnWidths[columnIndex],
        resizable: true
      }
    )));
    columns.push({ key: 'rightAlignColumn', width: 93, minWidth: 93});

    // prepare data
    data = bom.data.map( (value, rowIndex) => {
          return value.reduce(
            (rowData, value, columnIndex) => {
              rowData['column-'+columnIndex] = value;
              return rowData;
            },
            {
              id: 'row-'+rowIndex,
              parentId: null
            }
          );
        });

    return (
      <div className="fullheight">
        <Container className="bomContainer">
        <AutoResizer>
          {({ width, height }) => {
              return <BaseTable
                  width={width}
                  height={height}
                  columns={columns}
                  data={data}
              />;
          }}
        </AutoResizer>
        </Container>
      </div>
    );
  }
}

/* istanbul ignore next */
export default connect(function (store) {
  const activeProject = getActiveProject(store);
  return {
    activeProject: activeProject,
    bomData: getBom(activeProject.id, store)
  };
}, { fetchBom })(Bom);
