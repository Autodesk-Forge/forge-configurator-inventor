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
import './drawingsContainer.css';
import Button from '@hig/button';
import { getActiveProject, getDrawingsList, getActiveDrawing, downloadProgressShowing, downloadProgressTitle, downloadUrl, downloadFailedShowing, errorData } from '../reducers/mainReducer';
import { getDownloadLink } from '../actions/downloadActions';
import { showDownloadProgress, showDownloadFailed } from '../actions/uiFlagsActions';
import { fetchDrawingsList } from '../actions/drawingsListActions';
import { updateActiveDrawing } from '../actions/uiFlagsActions';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import ModalDownloadProgress from './modalDownloadProgress';
import ModalFail from './modalFail';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt='' width='16px' height='16px'/>
    </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

export const drawingColumns = [
    {
        key: 'icon',
        title: '',
        dataKey: 'icon',
        cellRenderer: iconRenderer,
        align: Column.Alignment.RIGHT,
        width: 30,
    },
    {
        key: 'type',
        title: 'Drawings',
        dataKey: 'name',
        cellRenderer: ( { rowData } ) => rowData.label,
        align: Column.Alignment.LEFT,
        width: 200,
    }
];

export class DrawingsContainer extends Component {

    componentDidMount() {
        this.props.fetchDrawingsList(this.props.activeProject);
    }

    componentDidUpdate(prevProps) {
        // fetch drawings list when tab was active before projects initialized
        if (this.props.activeProject.id !== prevProps.activeProject.id)
            this.props.fetchDrawingsList(this.props.activeProject);
    }

    onDownloadPDF() {
        const project = this.props.activeProject;
        if (project?.id) {
            // generate/download selected drawing PDF
            this.props.getDownloadLink('CreateDrawingPdfJob', project.id, project.hash, 'Preparing Drawing PDF', this.props.activeDrawing);
        }
    }

    onRowClick( rowData) {
        if (this.props.activeDrawing === rowData.id)
            return;

        this.props.updateActiveDrawing(rowData.id);
    }

    render() {
        const drawingsList = this.props.activeProject ? this.props.drawingsList : [];
        const buttonsContainerClass = drawingsList ? "buttonsContainer" : "buttonsContainer hidden";

        const project = this.props.activeProject;

        // drawings for table
        let data = [];
        //drawingsList ? drawingsList.map((drawing, index) => ( <div key={index}>{drawing}</div> ))
        if(this.props.drawingsList) {
            const shortName = function(name) {
                const onlyName = name.split('\\').pop().split('/').pop();
                return onlyName;
            };

            data = this.props.drawingsList.map((drawing) => (
              {
                id: drawing,
                icon: 'document-drawing-24.svg',
                label: shortName(drawing)
              }
            ));
        }

        return (
            <div className="drawingsListContainer">
                <div className="pencilContainer">
                </div>
                <div className="drawingsList">
                    <AutoResizer>
                    {({ width, height }) => {
                        // reduce size by 16 (twice the default border of tabContent)
                        const newWidth = width;
                        const newHeight = height-16;
                        return <BaseTable className="drawingList"
                            width={newWidth}
                            height={newHeight}
                            columns={drawingColumns}
                            data={data}
                            rowEventHandlers={{
                                onClick: ({ rowData }) => { this.onRowClick(rowData); }
                            }}
                            rowClassName={({ rowData }) => {
                                // check if use selected class
                                if (rowData.id == this.props.activeDrawing)
                                    return "drawing-selected";
                                else
                                    return "";
                            }}
                        />;
                    }}
                    </AutoResizer>
                </div>
                <hr className="drawingsListSeparator"/>
                <div className={buttonsContainerClass}>
                    <Button style={{}}
                        size="standard"
                        title="Export PDF"
                        type="primary"
                        width="grow"
                        onClick={() => { this.onDownloadPDF(); }}
                        disabled={!this.props.activeDrawing}
                    />
                </div>
                {this.props.downloadProgressShowing && <ModalDownloadProgress
                    open={true}
                    title={ this.props.downloadProgressTitle }
                    label={project.id}
                    icon='/Archive.svg'
                    onClose={ () => this.props.showDownloadProgress(false) }
                    url={ this.props.downloadUrl }
                    statsKey={ this.props.activeDrawing } />}

                {this.props.downloadFailedShowing && <ModalFail
                    open={true}
                    title={ `${this.props.downloadProgressTitle} Failed` }
                    contentName="Project:"
                    label={project.id}
                    onClose={ () => this.props.showDownloadFailed(false) }
                    errorData={this.props.errorData}/>}
            </div>
        );
    }
}

/* istanbul ignore next */
export default connect(function (store) {
    const activeProject = getActiveProject(store);
    return {
        activeProject: activeProject,
        downloadProgressShowing: downloadProgressShowing(store),
        downloadProgressTitle: downloadProgressTitle(store),
        downloadFailedShowing: downloadFailedShowing(store),
        downloadUrl: downloadUrl(store),
        errorData: errorData(store),
        activeDrawing: getActiveDrawing(store),
        drawingsList: getDrawingsList(store)
    };
}, { getDownloadLink, fetchDrawingsList, updateActiveDrawing, showDownloadProgress, showDownloadFailed })(DrawingsContainer);
