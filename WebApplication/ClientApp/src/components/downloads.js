import React, { Component } from 'react';
import { connect } from 'react-redux';
import BaseTable, { AutoResizer, Column } from 'react-base-table';
import 'react-base-table/styles.css';
import { getActiveProject } from '../reducers/mainReducer';
import { rfaProgressProjectId } from '../reducers/mainReducer';
import { showRFAModalProgress, hideRFAModalProgress } from '../actions/uiFlagsActions';
import ModalProgress from './modalProgress';
import { Jobs } from '../JobManager';
import { addError, addLog } from '../actions/notificationActions';

const Icon = ({ iconname }) => (
    <div>
      <img src={iconname} alt=''/>
    </div>
);

const iconRenderer = ({ cellData: iconname }) => <Icon iconname={iconname} />;

const columns = [
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

export class Downloads extends Component {

    constructor(props) {
        super(props);
        this.startRFAJob = this.startRFAJob.bind(this);
    }

    onProgressCloseClick() {
        // close is not supported now
        //this.props.hideRFAModalProgress();
    }

    async startRFAJob(projectId, hash) {
        const jobManager = Jobs();

        // launch signalR to make RFA here and wait for result
        try {
            await jobManager.doRFAJob(projectId, hash,
                // start job
                () => {
                    addLog('JobManager: HubConnection started for project : ' + projectId);
                },
                // onComplete
                (rfaUrl) => {
                    addLog('JobManager: Received onComplete ' + rfaUrl);

                    // hide modal dialog
                    this.props.hideRFAModalProgress();

                    // prepare URL, what parameters we will receive ????
                }
            );
        } catch (error) {
            addError('JobManager: Error : ' + error);
        }
    }

    render() {
        let iamDownloadHyperlink = null;
        const iamDownloadLink = <a href={this.props.activeProject.modelDownloadUrl} onClick={(e) => { e.stopPropagation(); }} ref = {(h) => {
            iamDownloadHyperlink = h;
        }}>IAM</a>;

        let rfaDownloadHyperlink = null;
        const hasRFAlink = false; // read real rfa link
        const rfaDownloadLink =
        <a
            href={hasRFAlink ? this.props.activeProject.rfaDownloadUrl : "#"}
            onClick={(e) => { hasRFAlink ? e.stopPropagation() : e.preventDefault(); }} ref = {(h) => {
            rfaDownloadHyperlink = h;
        }}>RFA</a>;

        const data = [
            {
                id: 'updatedIam',
                icon: 'products-and-services-24.svg',
                type: 'IAM',
                env: 'Model',
                link: iamDownloadLink,
                clickHandler: () => {
                    iamDownloadHyperlink.click();
                    console.log('IAM');
                }
            },
            {
                id: 'rfa',
                icon: 'products-and-services-24.svg',
                type: 'RFA',
                env: 'Model',
                link: rfaDownloadLink,
                clickHandler: async () => {

                    // check if we have RFA link
                    // yes, use it
                    if (hasRFAlink) {
                        rfaDownloadHyperlink.click();
                    } else {
                        this.props.showRFAModalProgress(this.props.activeProject.id);
                        await this.startRFAJob(this.props.activeProject.id, this.props.activeProject.hash);

                        // create new href and click
                    }

                    console.log('RFA');
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
                    return <BaseTable
                        width={newWidth}
                        height={newHeight}
                        columns={columns}
                        data={data}
                        rowEventHandlers={{
                            onClick: ({ rowData }) => { rowData.clickHandler(); }
                        }}
                    />;
                }}
            </AutoResizer>;
                {this.props.rfaProgressProjectId && <ModalProgress
                    open={true}
                    title="Preparing Archive"
                    label={this.props.rfaProgressProjectId}
                    icon='/Archive.svg'
                    onClose={() => this.onProgressCloseClick()}/>}
        </React.Fragment>
        );
    }
}

/* istanbul ignore next */
export default connect(function(store) {
    const activeProject = getActiveProject(store);
    return {
        activeProject: activeProject,
        rfaProgressProjectId: rfaProgressProjectId(store)
    };
}, { Downloads, showRFAModalProgress, hideRFAModalProgress })(Downloads);