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
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { getActiveProject, downloadProgressShowing, downloadProgressTitle, downloadUrl, downloadFailedShowing, errorData } from '../reducers/mainReducer';
import { downloadDrawingFailedShowing } from '../reducers/mainReducer';
import { getDownloadLink } from '../actions/downloadActions';
import { showDownloadProgress, showDownloadFailed } from '../actions/uiFlagsActions';
import ModalDownloadProgress from './modalDownloadProgress';
import ModalFail from './modalFail';

import repo from '../Repository';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

/** Hyperlink, which leads nowhere. */
function deadEndLink(title) {
    return <a href='' onClick={(e) => { e.preventDefault(); }}>{ title }</a>;
}

export const downloadColumns = [
    {
        key: 'icon',
        title: '',
        dataKey: 'icon',
        cellRenderer: iconRenderer,
        align: Column.Alignment.RIGHT,
        width: 100,
    },
    {
        key: 'type',
        title: 'File Type',
        dataKey: 'type',
        cellRenderer: ( { rowData } ) => rowData.link,
        align: Column.Alignment.LEFT,
        width: 150,
    },
    {
        key: 'env',
        title: 'Environment',
        dataKey: 'env',
        align: Column.Alignment.CENTER,
        width: 200,
    }
];

// add token to download URLs if necessary
function getDownloadUrls(project) {

    const { modelDownloadUrl, bomDownloadUrl } = project;

    const token = repo.getAccessToken();
    const suffix = token ? "/" + token : "";

    return {
        modelDownloadUrl: modelDownloadUrl ? modelDownloadUrl + suffix : modelDownloadUrl,
        bomDownloadUrl: bomDownloadUrl ? bomDownloadUrl + suffix : bomDownloadUrl
    };
}

export class Downloads extends Component {
    render() {

        // array with rows data
        const data = [];

        const project = this.props.activeProject;
        if (project?.id) {

            const { modelDownloadUrl, bomDownloadUrl } = getDownloadUrls(project);
            if (modelDownloadUrl) {

                let downloadHyperlink;
                const modelJsx = <a href={modelDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
                    downloadHyperlink = h;
                }}>IAM/IPT</a>;

                // register the row
                data.push({
                    id: 'updatedIam',
                    icon: 'products-and-services-24.svg',
                    type: 'IAM',
                    env: 'Model',
                    link: modelJsx,
                    clickHandler: () => downloadHyperlink.click()
                });
            }

            data.push({
                id: 'rfa',
                icon: 'products-and-services-24.svg',
                type: 'RFA',
                env: 'Model',
                link: deadEndLink('RFA'),
                clickHandler: async () => this.props.getDownloadLink('CreateRFAJob', project.id, project.hash, 'Preparing RFA')
            });

            if (bomDownloadUrl && project.isAssembly) {

                let downloadHyperlink;
                const bomJsx = <a href={bomDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
                    downloadHyperlink = h;
                }}>BOM</a>;

                // register the row
                data.push({
                    id: 'bom',
                    icon: 'products-and-services-24.svg',
                    type: 'BOM',
                    env: 'BOM',
                    link: bomJsx,
                    clickHandler: () => downloadHyperlink.click()
                });
            }

            const hasDrawingUrl = project.hasDrawing;
            if (hasDrawingUrl) {
                data.push(
                    {
                        id: 'drawing',
                        icon: 'products-and-services-24.svg',
                        type: 'IDW',
                        env: 'Drawing',
                        link: deadEndLink('Drawing'),
                        clickHandler: async () => this.props.getDownloadLink('CreateDrawingDownloadJob', project.id, project.hash, 'Preparing Drawings')
                    });
            }
        }

        return (
        <React.Fragment>
            <AutoResizer>
                {({ width, height }) => {
                    // reduce size by 16 (twice the default border of tabContent)
                    const newWidth = width-16;
                    const newHeight = height-16;
                    return <BaseTable
                        width={newWidth}
                        height={newHeight}
                        columns={downloadColumns}
                        data={data}
                        rowEventHandlers={{
                            onClick: ({ rowData }) => { rowData.clickHandler(); }
                        }}
                    />;
                }}
            </AutoResizer>
                {this.props.downloadProgressShowing && <ModalDownloadProgress
                    open={true}
                    title={ this.props.downloadProgressTitle }
                    label={project.id}
                    icon='/Archive.svg'
                    onClose={ () => this.props.showDownloadProgress(false) }
                    url={ this.props.downloadUrl } />}

                {this.props.downloadFailedShowing && <ModalFail
                    open={true}
                    title={ `${this.props.downloadProgressTitle} Failed` }
                    contentName="Project:"
                    label={project.id}
                    onClose={ () => this.props.showDownloadFailed(false) }
                    url={this.props.errorData}/>}

        </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function(store) {
    const activeProject = getActiveProject(store);
    return {
        activeProject: activeProject,
        downloadProgressShowing: downloadProgressShowing(store),
        downloadProgressTitle: downloadProgressTitle(store),
        downloadFailedShowing: downloadFailedShowing(store),
        downloadUrl: downloadUrl(store),
        errorData: errorData(store),
        drawingDownloadFailedShowing: downloadDrawingFailedShowing(store)
    };
}, { Downloads, getDownloadLink, showDownloadProgress, showDownloadFailed })(Downloads);
