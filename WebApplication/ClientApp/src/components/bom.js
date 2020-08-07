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
import { getActiveProject, getBom } from '../reducers/mainReducer';
import { fetchBom } from '../actions/bomActions';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import styled from 'styled-components';

const Container = styled.div`
  height: 100vh;
`;

// compute text width from canvas
function getMaxColumnTextWidth(strings) {
  const font = "13px ArtifaktElement, sans-serif";
  const canvas = document.createElement("canvas");
  const context2d = canvas.getContext("2d");
  context2d.font = font;
  let maxWidth = 0;
  strings.forEach(element => {
    const width = context2d.measureText(element).width;
    maxWidth = width>maxWidth ? width : maxWidth;
  });

  // round to 10times number, like 81.5 -> 90, 87.1 -> 90, etc
  const roundTo = 10;
  const rounded = (maxWidth % roundTo==0) ? maxWidth : maxWidth-maxWidth%roundTo + roundTo;
  // console.log('width of "'+ JSON.stringify(strings) +'" is: ' + rounded);
  return rounded;
}

export class Bom extends Component {

  componentDidMount() {
    this.props.fetchBom(this.props.activeProject.id);
  }

  componentDidUpdate(prevProps) {
      // refresh bom data when BOM tab was clicked before projects initialized
      if (this.props.activeProject.id !== prevProps.activeProject.id) {
          this.props.fetchBom(this.props.activeProject.id);
      }
  }

  render() {
    let columns = [{ key: 'leftAlignColumn', width: 79, minWidth: 79}];
    let data = [];
    if(this.props.bomData) {
      const columnWidths = [];
      // extract all strings from per all columns to prepare columns widths
      const allStrings = [];
      this.props.bomData.columns.forEach( (column, index) => {
        const columnStrings = [ column.label ];
        this.props.bomData.data.forEach(row => {
          columnStrings.push(row[index]);
        });
        allStrings.push(columnStrings);
      });

      allStrings.forEach(columnStrings => {
        // compute column width
        columnWidths.push(getMaxColumnTextWidth(columnStrings) + 15 /*right+left padding*/);
      });

      // prepare columns
      columns = columns.concat(this.props.bomData.columns.map( (column, columnIndex) => (
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
      data = this.props.bomData.data.map( (value, rowIndex) => {
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
    }

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
