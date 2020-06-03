import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { getActiveProject, rfaProgressShowing, rfaDownloadUrl } from '../reducers/mainReducer';
import { getRFADownloadLink } from '../actions/downloadActions';
import { showRFAModalProgress } from '../actions/uiFlagsActions';
import ModalProgress from './modalProgress';

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

export class DownloadsTable extends Component {
    render() {
        return <BaseTable
            width={this.props.width}
            height={this.props.height}
            columns={this.props.columns}
            data={this.props.data}
            rowEventHandlers={{
                onClick: ({ rowData }) => { rowData.clickHandler(); }
            }}
    />;
    }
}

export class Downloads extends Component {

    onProgressCloseClick() {
        this.props.showRFAModalProgress(false);
    }

    render() {
        let iamDownloadHyperlink = null;
        const iamDownloadLink = <a href={this.props.activeProject.modelDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
            iamDownloadHyperlink = h;
        }}>IAM</a>;

        const rfaDownloadLink =
        <a href="" onClick={(e) => { e.preventDefault(); }}>RFA</a>;

        const data = [
            {
                id: 'updatedIam',
                icon: 'products-and-services-24.svg',
                type: 'IAM',
                env: 'Model',
                link: iamDownloadLink,
                clickHandler: () => {
                    iamDownloadHyperlink.click();
                    //console.log('IAM');
                }
            },
            {
                id: 'rfa',
                icon: 'products-and-services-24.svg',
                type: 'RFA',
                env: 'Model',
                link: rfaDownloadLink,
                clickHandler: async () => {
                    this.props.getRFADownloadLink(this.props.activeProject.id, this.props.activeProject.hash);
                }
            }
        ];

        return (
        <React.Fragment>
            <AutoResizer>
                {({ width, height }) => {
                    // reduce size by 16 (twice the default border of tabContent)
                    const newWidth = width-16;
                    const newHeight = height-16;
                    const props = {
                        'width': newWidth,
                        'height': newHeight,
                        'columns': downloadColumns,
                        'data': data
                    };
                    return <DownloadsTable { ...props} />;
                }}
            </AutoResizer>;
                {this.props.rfaProgressShowing && <ModalProgress
                    open={true}
                    title="Preparing Archive"
                    label={this.props.activeProject.id}
                    icon='/Archive.svg'
                    onClose={() => this.onProgressCloseClick()}
                    url={this.props.rfaDownloadUrl}
                    />}
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
        rfaDownloadUrl: rfaDownloadUrl(store)
    };
}, { Downloads, getRFADownloadLink, showRFAModalProgress })(Downloads);