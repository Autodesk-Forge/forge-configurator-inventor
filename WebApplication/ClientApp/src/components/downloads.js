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
import { getActiveProject, rfaProgressShowing, rfaDownloadUrl, downloadRfaFailedShowing, reportUrl } from '../reducers/mainReducer';
import { getRFADownloadLink } from '../actions/downloadActions';
import { showRFAModalProgress, showRfaFailed } from '../actions/uiFlagsActions';
import ModalProgressRfa from './modalProgressRfa';
import ModalFail from './modalFail';

import repo from '../Repository';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

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

    onProgressCloseClick() {
        this.props.showRFAModalProgress(false);
    }

    onRfaFailedCloseClick() {
        this.props.showRfaFailed(false);
    }

    render() {

        // array with rows data
        const data = [];

        if (this.props.activeProject?.id) {

            const { modelDownloadUrl, bomDownloadUrl } = getDownloadUrls(this.props.activeProject);
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

            const rfaJsx = <a href="" onClick={(e) => { e.preventDefault(); }}>RFA</a>;
            data.push({
                id: 'rfa',
                icon: 'products-and-services-24.svg',
                type: 'RFA',
                env: 'Model',
                link: rfaJsx,
                clickHandler: async () => {
                    this.props.getRFADownloadLink(this.props.activeProject.id, this.props.activeProject.hash);
                }
            });

            if (bomDownloadUrl) {

                let downloadHyperlink;
                const bomJsx = <a href={bomDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
                    downloadHyperlink = h;
                }}>BOM CSV</a>;

                // register the row
                data.push({
                    id: 'bom',
                    icon: 'products-and-services-24.svg',
                    type: 'BOM',
                    env: 'Model',
                    link: bomJsx,
                    clickHandler: () => downloadHyperlink.click()
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
                {this.props.rfaProgressShowing && <ModalProgressRfa
                    open={true}
                    title="Preparing RFA"
                    label={this.props.activeProject.id}
                    icon='/Archive.svg'
                    onClose={() => this.onProgressCloseClick()}
                    url={this.props.rfaDownloadUrl}
                    onUrlClick={() => this.onProgressCloseClick()}
                    />}
                {this.props.rfaFailedShowing && <ModalFail
                    open={true}
                    title="Preparing RFA Failed"
                    contentName="Project:"
                    label={this.props.activeProject.id}
                    onClose={() => this.onRfaFailedCloseClick()}
                    url={this.props.reportUrl}/>}
        </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function(store) {
    const activeProject = getActiveProject(store);
    return {
        activeProject: activeProject,
        rfaProgressShowing: rfaProgressShowing(store),
        rfaFailedShowing: downloadRfaFailedShowing(store),
        rfaDownloadUrl: rfaDownloadUrl(store),
        reportUrl: reportUrl(store)
    };
}, { Downloads, getRFADownloadLink, showRFAModalProgress, showRfaFailed })(Downloads);
